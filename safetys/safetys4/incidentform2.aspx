<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="incidentform2.aspx.cs" Inherits="safetys4.incidentform2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


<link href="template/css/plugins/dataTables/jquery.dataTables.min.css" rel="stylesheet">
<script src="template/js/plugins/dataTables/jquery.dataTables.min.js"></script>  
 



<style>
.stepwizard-step p {
    margin-top: 10px;
}
.stepwizard-row {
    display: table-row;
}
.stepwizard {
    display: table;
    width: 100%;
    position: relative;
}
.stepwizard-step button[disabled] {
    opacity: 1 !important;
    filter: alpha(opacity=100) !important;
}
.stepwizard-row:before {
    top: 14px;
    bottom: 0;
    position: absolute;
    content: " ";
    width: 100%;
    height: 1px;
    background-color: #ccc;

}
.stepwizard-step {
    display: table-cell;
    text-align: center;
    position: relative;
}
.btn-circle {
    width: 30px;
    height: 30px;
    text-align: center;
    padding: 6px 0;
    font-size: 12px;
    line-height: 1.428571429;
    border-radius: 15px;
}


hr {
   
    border-top-color: #DE2F13;
    border-top-width: 3px;
    margin-top: 5px !important;
    margin-bottom: 5px !important;

}

 strong{
    color: #DE2F13;
 }


.dz-max-files-reached {
        background-color: red;
    }


/*.ui-dialog-titlebar-close {
    visibility: hidden;
  }*/


.row {
 
    padding-bottom: 20px !important;
}


.a-step{
    color:black !important;
}



     a {
        color:black ;
    }


.btn-circle-new {
    border-bottom-left-radius: 15px;
    border-bottom-right-radius: 15px;
    border-top-left-radius: 15px;
    border-top-right-radius: 15px;
    font-size: 12px;
    height: 20px;
    line-height: 1.42857;
    padding-bottom: 6px;
    padding-left: 0;
    padding-right: 0;
    padding-top: 2px;
    text-align: center;
    width: 20px;
}




    #tbDefinitionLevel tbody tr {
         cursor: pointer;
    }

  .selected_colunm {
    background-color: #B0BED9;
   
  }

   .selected_colunm_critical {
    background-color:  #e74c3c;
    color:white;
  }
     
    .wrapper-content {
    margin-left: 0px !important;
} 
</style>

 <script type="text/javascript">

    var id = "";
    var pagetype = "";
    var dialogInjury;
    var dialogDamageList;
    var dataTableInjury; //reference to your dataTable
    var dataTableEffect; //reference to your dataTable
    var dataTableDefinition;

    var dialogReason;

    var result_image = 0;
    var result_safety = 0;
    var result_environment = 0;
    var result_damage = 0;
    var result_process = 0;
    var result_legal = 0;
    var result_person = 0;

    var injury_id = 0;
    var damage_id = 0;
    var dialogGov;
    $(document).ready(function () {

        var url = window.location.href;
        var urlarr = url.split("=");

        if (urlarr.length > 2)
        {
            id = urlarr[2];
            pagetype_arr = urlarr[1].split("&");
            pagetype = pagetype_arr[0];

        } else {
            pagetype = urlarr[1];
            

        }
     
        if (pagetype == "view")
        {
           
            setShowedit();
          

        } else if (pagetype == "edit") {
          
            
            setShowedit();
            setDialogInjury();
          

        } else {
            setCurrency();
        }

        
        setDatatableInjury();
        setDatatableEffect();
        setDatatableDefinition();
        setReasonReject();

        $("input[type=radio][name='ctl00$MainContent$impact']").change(function () {
            if (this.value == 'N') {
                $("#hd_injury_fatality_involve").hide();
                $("#hd_effect_environment").hide();

                $("#hd_tbinjury").hide();
                $("#hd_tbeffect").hide();

                $("#hd_btaddinjury").hide();
                $("#hd_btaddeffect").hide();

                <%                       
                    if (Session["country"].ToString()=="thailand")        
                    {                            
       
                %>
                         $("#MainContent_ddConsequencelevel").val("6");//6 is near miss

                <%
                    }
                    else if (Session["country"].ToString() == "srilanka")
                    {
                %>
                <%                       
                   
                %>
                        $("#MainContent_ddConsequencelevel").val("12");//12 is near miss

                <%
                    }
                %>
                $("#hd_btDefinitionlevel").hide();
            }
            else if (this.value == 'Y') {
                $("#hd_injury_fatality_involve").show();
                $("#hd_effect_environment").show();

                $("#hd_tbinjury").show();
                $("#hd_tbeffect").show();

                $("#hd_btaddinjury").show();
                $("#hd_btaddeffect").show();

                $("#MainContent_ddConsequencelevel").val("");
                $("#hd_btDefinitionlevel").show();
            }
        });



        $("input[type=radio][name='ctl00$MainContent$effect_environment']").change(function () {
            if (this.value == 'N') {
                $("#hd_tbeffect").hide();
                $("#hd_btaddeffect").hide();

           
            }
            else if (this.value == 'Y') {
                $("#hd_tbeffect").show();
                $("#hd_btaddeffect").show();

            }
        });


        $("input[type=radio][name='ctl00$MainContent$injury_fatality_involve']").change(function () {
            if (this.value == 'N') {
                $("#hd_tbinjury").hide();
                $("#hd_btaddinjury").hide();



            }
            else if (this.value == 'Y') {
                $("#hd_tbinjury").show();             
                $("#hd_btaddinjury").show();
              
            }
        });


     


        dialogInjury = $("#injury-form").dialog({
            autoOpen: false,
            height: 580,
            width: 650,
            modal: true,

            close: function () {



            },
            open: function (event, ui) {
                //clearValidationErrors();
              
                $("#injury-form").css('overflow-x', 'hidden');
            },
            close: function() {
                clearValidationErrors();
                clearDataInjury();

            },
            modal: true,
        });



        dialogDamageList = $("#damage-list-form").dialog({
            autoOpen: false,
            height: 490,
            width: 450,
            modal: true,

            close: function () {



            },
            open: function (event, ui) {
                //clearValidationErrors();

                $("#damage-list-form").css('overflow-x', 'hidden');
            },
            close: function () {
                clearValidationErrors();
                clearDataDamage();
            },
            modal: true,
        });
       



        dialogReason = $("#create_reason_reject").dialog({
            autoOpen: false,
            height: 380,
            width: 400,
            modal: true,

            close: function () {



            },
            open: function (event, ui) {
                //clearValidationErrors();

                $("#create_reason_reject").css('overflow-x', 'hidden');
            },
            modal: true,
        });


        dialogGov = $("#show_goverment").dialog({
            autoOpen: false,
            height: 480,
            width: 1100,
            modal: true,

            close: function () {



            },
            open: function (event, ui) {

                $("#show_goverment").css('overflow-x', 'hidden');
            },
            modal: true,
        });


        $("#MainContent_txtName").autocomplete({
            source: "Masterdata.asmx/getEmployeeautocomplete",
            select: function (event, ui) {

               
                
              //  $("#MainContent_ddlFunction").val(ui.item.function_id);
                $("#MainContent_employee_injury").val(ui.item.employee_id);

                $("#MainContent_ddlFunction").prop('disabled', true);
                $("#MainContent_dddepartment").prop('disabled', true);

                  <%
                if (Session["country"].ToString() =="thailand")
                {                  
                %>
                    $("#MainContent_ddlTypeemployment").val(1);//employee
                    setFunction(ui.item.function_id, ui.item.department_id);
                <%                      
                }
       
                %>

                <%     
                                            
                else if (Session["country"].ToString() == "srilanka")
                {                   
                %>
                    $("#MainContent_ddlTypeemployment").val(6);//employee
                    setFunction(ui.item.function_id, ui.item.sub_function_id);
                <%
                }
                            
                %>

               
               

            }
        });

        $("#lbdaylost").hide();
        $("#MainContent_txtDaylost").hide();

        $("#MainContent_ddlSeverityInjury").change(function () {

           
            if (this.value == "3" || this.value == "9")//3 is LTI,8 is LTI for SK
            {
                $("#lbdaylost").show();
                $("#MainContent_txtDaylost").show();
            } else {

                $("#lbdaylost").hide();
                $("#MainContent_txtDaylost").hide();
            }
            
           
        });




        $("#MainContent_ddlTypeemployment").change(function () {

            if (this.value == "1" || this.value == "6")//1 is employee , 5 is employee LK
            {
                $("#MainContent_ddlFunction").prop('disabled', true);
                $("#MainContent_dddepartment").prop('disabled', true);
               
               
            } else {

                $("#MainContent_ddlFunction").prop('disabled', false);
                $("#MainContent_dddepartment").prop('disabled', false);

            }


        });
       

       
    });



     function CheckIncidentImmediateLength(oSrc, args)
     {
         if (args.Value.length > 4000) {

             args.IsValid = false;
         } else {

             args.IsValid = true;
         }


     }


    function setDialogInjury()
    {
        setTypeEmployment();
        setFunction("","");
        setNatureInjury();
        setBodyParts();
        setSeverityInjury(); setSeverityInjury()
    }



    function setTypeEmployment()
    {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getTypeEmployment',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddlTypeemployment");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

               
            }
        });



    }

    function showDialogGoverment()
    {

        dialogGov.dialog("open");



    }

    function setReasonReject()
    {
        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getReasonReject',
            dataType: 'json',
            success: function (json) {

                var $el = $("#ddReasonreject");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });


            }
        });



    }

    function setFunction(function_id,department_id)
    {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getFuctionlist',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddlFunction");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });
                $("#MainContent_ddlFunction").val(function_id);
                setDepartment(function_id, department_id);
            }
        });



    }


    function setDepartment(function_id,department_id)
    {

        $.ajax({
            type: "POST",
            data: { function: function_id, lang: lang },
            url: 'Masterdata.asmx/getDepartmentbyFunction',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_dddepartment");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

               
               $('#MainContent_dddepartment').val(department_id);
                 


            }
        });



    }

    function setNatureInjury()
    {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getNatureInjury',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddlNatureInjury");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });


            }
        });



    }


    function setBodyParts()
    {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getBodyParts',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddlBodyPart");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });


            }
        });



    }


    function setSeverityInjury()
    {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getSeverityInjury',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddlSeverityInjury");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });


            }
        });



    }



  
    function setShowedit()
    {

        // console.log(id);
        $.ajax({
            type: "POST",
            data: { id: id, lang: lang },
            url: 'Actionevent.asmx/getIncidentbyid',
            dataType: 'json',
            success: function (json) {
                setCurrency();

                $.each(json, function (value, key) {
                 
                    // $("#MainContent_txtincident_date").val(key.incident_date);
                   
                    $("#MainContent_lbEmployee").text(key.name_modify);
                    $("#MainContent_lbUpdate").text(key.datetime_modify);

                    $("#show_doc_status").html(key.doc_no + ' ' + key.status);

                    //if (key.work_relate != null)
                    //{
                    //    $("input[name='ctl00$MainContent$work_relate'][value='" + key.work_relate + "']").attr('checked', 'checked');
                    //}

                     if (country == "srilanka")
                     {                  
                           
                        if (key.responsible_area != null)
                        {
                            $("input[name='ctl00$MainContent$responsible_area'][value='" + key.responsible_area + "']").attr('checked', 'checked');
                        }
                    
                    }     
                     


                    if (key.impact != null)
                    {

                        $("input[name='ctl00$MainContent$impact'][value='" + key.impact + "']").attr('checked', 'checked');

                        if (key.impact == 'N') {
                            $("#hd_injury_fatality_involve").hide();
                            $("#hd_effect_environment").hide();

                            $("#hd_tbinjury").hide();
                            $("#hd_bteffect").hide();

                            $("#hd_btaddinjury").hide();
                            $("#hd_btaddeffect").hide();

                        }
                        else if (key.impact == 'Y') {
                            $("#hd_injury_fatality_involve").show();
                            $("#hd_effect_environment").show();

                            $("#hd_tbinjury").show();
                            $("#hd_bteffect").show();

                            $("#hd_btaddinjury").show();
                            $("#hd_btaddeffect").show();
                        }
                    }

                    if (key.injury_fatality_involve != null)
                    {
                        $("input[name='ctl00$MainContent$injury_fatality_involve'][value='" + key.injury_fatality_involve + "']").attr('checked', 'checked');

                        if (key.injury_fatality_involve == 'N')
                        {
                            $("#hd_tbinjury").hide();
                            $("#hd_btaddinjury").hide();

                        }
                        else if (key.injury_fatality_involve == 'Y')
                        {
                            $("#hd_tbinjury").show();
                            $("#hd_btaddinjury").show();

                        }
                      

                    } else {

                        $("#hd_tbinjury").hide();
                        $("#hd_btaddinjury").hide();

                    }


                    if (key.effect_environment != null)
                    {
                        
                        $("input[name='ctl00$MainContent$effect_environment'][value='" + key.effect_environment + "']").attr('checked', 'checked');
                        if (key.effect_environment == 'N')
                        {
                            $("#hd_tbeffect").hide();
                            $("#hd_btaddeffect").hide();

                        } else if (key.effect_environment == 'Y') {
                            $("#hd_tbeffect").show();
                            $("#hd_btaddeffect").show();

                        }
                    } else {

                        $("#hd_tbeffect").hide();
                        $("#hd_btaddeffect").hide();

                    }


                    $("#MainContent_ddLeveldamange").val(key.level_damange);
                    $("#MainContent_ddLevelenvironment").val(key.level_environment);
                   
                    if (key.other_impact != null)
                    {

                       // $("input[name='ctl00$MainContent$other_impact'][value='" + key.other_impact + "']").attr('checked', 'checked');
                    }
                   
                    if (key.critical != null)
                    {
                        $("input[name='ctl00$MainContent$critical'][value='" + key.critical + "']").attr('checked', 'checked');

                    }

                    if (key.external_reportable != null)
                    {
                        $("input[name='ctl00$MainContent$external_reportable'][value='" + key.external_reportable + "']").attr('checked', 'checked');

                    }
                    var other_impacts = key.other_impact;

                    $('#impact_checkbox').find(':checkbox[name^="ctl00$MainContent$other_impact"]').each(function () {
                        $(this).prop("checked", ($.inArray($(this).val(), other_impacts) != -1));

                    });


                    $("#MainContent_txtimmediate_temporary").val(key.immediate_temporary);
                    $("#MainContent_ddConsequencelevel").val(key.consequence_level);

                    //alert(key.currency);
                    if (key.currency != "" || key.currency !=null)
                    {
                        $("#MainContent_ddCurrency").val(key.currency);

                    } else {
                        
                            <%
                            if (Session["country"].ToString() =="thailand")
                            {                  
                            %>
                                $("#MainContent_ddCurrency").val("1");

                            <%                      
                            }
       
                            %>

                             <%     
                                            
                            else if (Session["country"].ToString() == "srilanka")
                            {                   
                            %>
                                $("#MainContent_ddCurrency").val("3");    
                            <%
                            }
                            
                            %>
                        

                    }
                   

                    ////////////////////////////////////////set level table/////////////////////////////////

                   


                    if (key.result_image != "0")
                    {
                        if (key.result_image==5)
                        {
                            $(dataTableDefinition.cell(0, key.result_image).node()).addClass('selected_colunm_critical');
                        } else {
                            $(dataTableDefinition.cell(0, key.result_image).node()).addClass('selected_colunm');
                        }
                        
                        result_image = key.result_image;
                      
                    }
                    
                    if (key.result_safety != "0")
                    {
                        if (key.result_safety == 5 || key.result_safety == 4 || key.result_safety == 3)
                        {
                            $(dataTableDefinition.cell(1, key.result_safety).node()).addClass('selected_colunm_critical');

                        } else {

                            $(dataTableDefinition.cell(1, key.result_safety).node()).addClass('selected_colunm');

                        }
                       
                        result_safety = key.result_safety;
                    }
                    
                    if (key.result_environment != "0")
                    {

                        if (key.result_environment == 5)
                        {
                            $(dataTableDefinition.cell(2, key.result_environment).node()).addClass('selected_colunm_critical');

                        } else {

                            $(dataTableDefinition.cell(2, key.result_environment).node()).addClass('selected_colunm');

                        }
                       
                        result_environment = key.result_environment;
                    }
                    
                    if (key.result_damage != "0")
                    {
                        if (key.result_damage == 5 || key.result_damage == 4)
                        {
                            $(dataTableDefinition.cell(3, key.result_damage).node()).addClass('selected_colunm_critical');

                        } else {

                            $(dataTableDefinition.cell(3, key.result_damage).node()).addClass('selected_colunm');

                        }

                        result_damage = key.result_damage;
                    }

                    if (key.result_process != "0") {
                        $(dataTableDefinition.cell(4, key.result_process).node()).addClass('selected_colunm');
                        result_process = key.result_process;
                    }

                    if (key.result_legal != "0") {
                        $(dataTableDefinition.cell(5, key.result_legal).node()).addClass('selected_colunm');
                        result_legal = key.result_legal;
                    }

                    if (key.result_person != "0") {

                        if (key.result_person == 5)
                        {
                            $(dataTableDefinition.cell(6, key.result_person).node()).addClass('selected_colunm_critical');

                        } else {

                            $(dataTableDefinition.cell(6, key.result_person).node()).addClass('selected_colunm');

                        }
                        
                        result_person = key.result_person;
                    }    

                    /////////////////////////////////////////////////////////////////////////////////////////////
                  
                    setLeveldamage(key.level_damange);
                    setLeveldenviroment(key.level_environment);
                    setConsequencelevel(key.consequence_level);

                   

                  
                });

         
              


            }
        });

    }




    function ShowEditInjuryPerson(injury_person_id)
    {
        $("#MainContent_btUpdateInjury").show();
        $("#MainContent_btCreateInjury").hide();
        injury_id = injury_person_id;
        dialogInjury.dialog("open");
        $.ajax({
            type: "POST",
            data: { id: injury_person_id },
            url: 'Actionevent.asmx/getInjuryPersonByID',
            dataType: 'json',
            success: function (json) {

                $.each(json, function (value, key) {

                    $("#MainContent_txtName").val(key.full_name);
                    $("#MainContent_ddlTypeemployment").val(key.type_employment_id);
                   // $("#MainContent_ddlFunction").val(key.function_id);
                    $("#MainContent_ddlNatureInjury").val(key.nature_injury_id);
                    $("#MainContent_ddlBodyPart").val(key.body_parts_id);
                    $("#MainContent_ddlSeverityInjury").val(key.severity_injury_id);
                    $("#MainContent_txtDaylost").val(key.day_lost);
                    $("#MainContent_txtremark").val(key.remark);
                    $("#MainContent_employee_injury").val(key.employee_id);

                    if (key.severity_injury_id == "3" || key.severity_injury_id == "9")//3 is LTI and 8 is LTI SK
                    {
                        $("#lbdaylost").show();
                        $("#MainContent_txtDaylost").show();
                    } else {
                        $("#lbdaylost").hide();
                        $("#MainContent_txtDaylost").hide();
                    }

                    if (key.type_employment_id == "1" || key.type_employment_id == "6")//employee,5 is employee SK
                    {
                        $("#MainContent_ddlFunction").prop('disabled', true);
                        $("#MainContent_dddepartment").prop('disabled', true);


                    }else{
                        $("#MainContent_ddlFunction").prop('disabled', false);
                        $("#MainContent_dddepartment").prop('disabled', false);

                    }
                   
                    setFunction(key.function_id, key.department_id);



                });





            }
        });

    }

    function ShowEditDamageList(damage_list_id)
    {
        $("#MainContent_btUpdatedamage").show();
        $("#MainContent_btCreatdamage").hide();
        damage_id = damage_list_id;
        dialogDamageList.dialog("open");
        $.ajax({
            type: "POST",
            data: { id: damage_list_id },
            url: 'Actionevent.asmx/getDamageListByID',
            dataType: 'json',
            success: function (json) {

                $.each(json, function (value, key) {

                    $("#MainContent_txtproperty_enviroment").val(key.property_environment_damage);
                   $("#MainContent_txtdetail_damage").val(key.detail_damage);
                   $("#MainContent_txtdamage_cost").val(key.damage_cost);

                });





            }
        });

    }

  
    function setLeveldamage(level_damange)
    {
      
        $.ajax({
            type: "POST",
            data: {lang: lang },
            url: 'Masterdata.asmx/getLevelDamage',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddLeveldamange");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                 $('#MainContent_ddLeveldamange').val(level_damange);
            }
        });



    }


    function setLeveldenviroment(level_environment)
    {

        $.ajax({
            type: "POST",
            data: {lang: lang },
            url: 'Masterdata.asmx/getLevelEnvironment',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddLevelenvironment");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                $('#MainContent_ddLevelenvironment').val(level_environment);
            }
        });



    }



    function setConsequencelevel(consequence_level)
    {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getLevelIncident',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddConsequencelevel");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                $('#MainContent_ddConsequencelevel').val(consequence_level);
            }
        });



    }


    function setCurrency()
    {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getCurrency',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddCurrency");
                $el.empty(); // remove old options

                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                
            }
        });



    }





    function updateIncident(typebutton)
    {
        
        var impact = $("input:radio[name='ctl00$MainContent$impact']:checked").val();
        var responsible_area = "";
        if (country == "srilanka")
        {
            responsible_area = $("input:radio[name='ctl00$MainContent$responsible_area']:checked").val();
        }



        if ((result_image != 0 || result_safety != 0 || result_environment != 0 || result_damage != 0
            || result_process != 0 || result_legal != 0 || result_person != 0) || impact=="N")
        {

            if (Page_ClientValidate("incident") && impact != undefined && ((responsible_area != undefined && country == "srilanka") || (responsible_area == "" && country == "thailand")))
            {

                var injury_fatality_involve = $("input:radio[name='ctl00$MainContent$injury_fatality_involve']:checked").val();
                if (injury_fatality_involve == undefined) {
                    injury_fatality_involve = "";
                }
                var count_tbinjury = dataTableInjury.data().length;

                var effect_environment = $("input:radio[name='ctl00$MainContent$effect_environment']:checked").val();
                if (effect_environment == undefined) {
                    effect_environment = "";
                }
                var count_tbeffect = dataTableEffect.data().length;


                    
                if ((injury_fatality_involve == "Y" && count_tbinjury != 0)
                    || (injury_fatality_involve == "N" && count_tbinjury == 0)
                    || (injury_fatality_involve == "" && count_tbinjury == 0))
                {

                        
                        
                    if((effect_environment == "Y" && count_tbeffect!=0)
                        || (effect_environment == "N" && count_tbeffect == 0)
                        || (effect_environment == "" && count_tbeffect == 0))
                    {

                        showLoading();

                        //var work_relate = $("input:radio[name='ctl00$MainContent$work_relate']:checked").val();
                        //if (work_relate == undefined)
                        //{
                        //    work_relate = "";
                        //}


                        //var ddLeveldamange = $("#MainContent_ddLeveldamange").val();
                        //var ddLevelenvironment = $("#MainContent_ddLevelenvironment").val();

                        //var other_impact = $("input:radio[name='ctl00$MainContent$other_impact']:checked").val();
                        //if (other_impact == "undefined")
                        //{
                        //    other_impact = "";
                        //}

                        var critical = $("input:radio[name='ctl00$MainContent$critical']:checked").val();
                        if (critical == undefined) {
                            critical = "";
                        }

                        var external_reportable = $("input:radio[name='ctl00$MainContent$external_reportable']:checked").val();
                        if (external_reportable == undefined) {
                            external_reportable = "";
                        }

                        var immediate_temporary = $("#MainContent_txtimmediate_temporary").val();
                        if (immediate_temporary == undefined) {
                            immediate_temporary = "";
                        }
                        var ddConsequencelevel = $("#MainContent_ddConsequencelevel").val();
                        var ddCurrency = $("#MainContent_ddCurrency").val();

                        var other_impacts = [];
                        $('#impact_checkbox input:checked').each(function () {
                            other_impacts.push(this.value);
                        });



                        var data_post = JSON.stringify({
                            work_relate: "Y",
                            responsible_area: responsible_area,
                            impact: impact,
                            injury_fatality_involve: injury_fatality_involve,
                            effect_environment: effect_environment,
                            //level_environment: ddLevelenvironment,
                            //level_damange: ddLeveldamange,
                            other_impact: other_impacts,
                            critical: critical,
                            external_reportable: external_reportable,
                            immediate_temporary: immediate_temporary,
                            consequence_level: ddConsequencelevel,
                            currency: ddCurrency,
                            result_image: result_image,
                            result_safety: result_safety,
                            result_environment: result_environment,
                            result_damage: result_damage,
                            result_process: result_process,
                            result_legal: result_legal,
                            result_person: result_person,
                            user_id: user_login_id,
                            typelogin: type_login,
                            incidentid: id,
                            stepform: 2,
                            typebutton: typebutton,
                            group_id: user_group_id
                        });

                        $.ajax({
                            type: "POST",
                            data: data_post,
                            url: 'Actionevent.asmx/updateIncident2',
                            contentType: "application/json; charset=utf-8",
                            success: function (id) {
                                window.location.href = "incidentform2.aspx?pagetype=view&id=" + id;
                            }
                        });

                        return false;


                    }else{

                        var require_effect = '<%= Resources.Incident.rqeffect %>';
                        $("#rqeffect").text(require_effect);

                        return false;


                    }



                } else {

                    var require_injury = '<%= Resources.Incident.rqinjury %>';
                    $("#rqinjury").text(require_injury);
                   
                    return false;

                }

               

            } else {

                var require_responsible_area = '<%= Resources.Incident.rqresponsiblearea %>';
                var require_impact = '<%= Resources.Incident.rqimpact %>';
                $("#rqresponsiblearea").text(require_responsible_area);
                $("#rqimpact").text(require_impact);

                return false;

            }
                

        } else {

            var message_require_select_table = '<%= Resources.Incident.require_select_table %>';
            alert(message_require_select_table);

            return false;

        }
           

 

    }

     
    function setDatatableInjury()
    {

        dataTableInjury = $("#tbInjury").DataTable({
            "bProcessing": true,
            "sProcessing": true,
       
            "bPaginate": false,
            "bInfo": false,
            "bFilter": false,
            "ordering": true,
            // "stateSave": true,
            "responsive": true,
           // "pageLength": 25,
            "lengthChange": false,
            "order": [],
            "language": {
                "url": 'Langdatatable.aspx'
               
            },
            "columnDefs": [
               {
                   "targets": [0],
                   "visible": false,
               }
            ],

            "fnFooterCallback": function (nRow, aaData, iDataStart, iDataEnd) {
                /* Calculate the total market share for all browsers in this table (ie inc. outside
                 * the pagination
                 */
                var day_lost = 0;
                for (var i = 0 ; i < aaData.length ; i++)
                {
                    day_lost += aaData[i][8];
                }

                /* Calculate the market share for browsers on this page */
                //var iPageMarket = 0;
                //for (var i = iDataStart ; i < iDataEnd ; i++) {
                //    iPageMarket += aaData[i][8] * 1;
                //}
               
                /* Modify the footer row to match what we want */
                var nCells = nRow.getElementsByTagName('th');
                //console.log(nCells);
                $("#total_day_lost").text(day_lost);
                //nCells[7].innerHTML = parseInt(day_lost);
            }

        });


        dataTableInjury.ajax.url('Datatablelist.asmx/getListInjuryPerson?incident_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype);



    }



    function setDatatableDefinition()
    {

        dataTableDefinition = $("#tbDefinitionLevel").DataTable({
            "bProcessing": true,
            "sProcessing": true,

            "bPaginate": false,
            "bInfo": false,
            "bFilter": false,
            "ordering": false,
            // "stateSave": true,
            "responsive": true,
            // "pageLength": 25,
            "lengthChange": false,
            "columnDefs": [
               {
                   "targets": [0],
                   "visible": false,
               }
            ]
     

        });

     
       // var cell = dataTableDefinition.cell(5,3);

       
        $('#tbDefinitionLevel tbody').on('click', 'td', function () {

            var check_remove = false;
            var row = dataTableDefinition.cell(this).index().row;
            var column = dataTableDefinition.cell(this).index().column;
          //  $(dataTableDefinition.cell(this).node()).addClass('selected_colunm');
           // var cell2 = dataTableDefinition.cell(this).node();
            //console.log(row + "," + column);
            console.log(column);
            console.log(row);
            //$(".selected_colunm").css("background-color", " #e74c3c");
            if ($(this).hasClass('selected_colunm'))
            {
                $(this).removeClass('selected_colunm');
                check_remove = true;
            }
            else {
               
                if (row == 0)//image
                {
                    if (column==5)//boycotting
                    {
                        $(this).closest("tr").find("td.selected_colunm").removeClass("selected_colunm");
                        $(this).closest("tr").find("td.selected_colunm_critical").removeClass("selected_colunm_critical");
                        $(this).addClass('selected_colunm_critical');
                    } else {

                        $(this).closest("tr").find("td.selected_colunm").removeClass("selected_colunm");
                        $(this).closest("tr").find("td.selected_colunm_critical").removeClass("selected_colunm_critical");
                        $(this).addClass('selected_colunm');

                    }

                } else if (row == 1) {

                    if (column == 5 || column == 4 || column == 3)
                    {
                        $(this).closest("tr").find("td.selected_colunm").removeClass("selected_colunm");
                        $(this).closest("tr").find("td.selected_colunm_critical").removeClass("selected_colunm_critical");
                        $(this).addClass('selected_colunm_critical');
                    } else {

                        $(this).closest("tr").find("td.selected_colunm").removeClass("selected_colunm");
                        $(this).closest("tr").find("td.selected_colunm_critical").removeClass("selected_colunm_critical");
                        $(this).addClass('selected_colunm');

                    }


                } else if (row == 2){

                    if (column == 5) {
                        $(this).closest("tr").find("td.selected_colunm").removeClass("selected_colunm");
                        $(this).closest("tr").find("td.selected_colunm_critical").removeClass("selected_colunm_critical");
                        $(this).addClass('selected_colunm_critical');
                    } else {

                        $(this).closest("tr").find("td.selected_colunm").removeClass("selected_colunm");
                        $(this).closest("tr").find("td.selected_colunm_critical").removeClass("selected_colunm_critical");
                        $(this).addClass('selected_colunm');

                    }


                } else if (row == 3){

                    if (column == 5 || column == 4) {
                        $(this).closest("tr").find("td.selected_colunm").removeClass("selected_colunm");
                        $(this).closest("tr").find("td.selected_colunm_critical").removeClass("selected_colunm_critical");
                        $(this).addClass('selected_colunm_critical');
                    } else {

                        $(this).closest("tr").find("td.selected_colunm").removeClass("selected_colunm");
                        $(this).closest("tr").find("td.selected_colunm_critical").removeClass("selected_colunm_critical");
                        $(this).addClass('selected_colunm');

                    }

                } else if (row == 6)
                {
                    if (column == 5) {
                        $(this).closest("tr").find("td.selected_colunm").removeClass("selected_colunm");
                        $(this).closest("tr").find("td.selected_colunm_critical").removeClass("selected_colunm_critical");
                        $(this).addClass('selected_colunm_critical');
                    } else {

                        $(this).closest("tr").find("td.selected_colunm").removeClass("selected_colunm");
                        $(this).closest("tr").find("td.selected_colunm_critical").removeClass("selected_colunm_critical");
                        $(this).addClass('selected_colunm');

                    }

                }else{

                     $(this).closest("tr").find("td.selected_colunm").removeClass("selected_colunm");
                     $(this).addClass('selected_colunm');

                }




            }

    

            if (row == 0)//row image
            {
                if (check_remove)
                {
                    result_image = 0;
                } else {

                    result_image = column;
                }
                

            } else if (row == 1) {

                if (check_remove)
                {
                    result_safety = 0;
                } else {

                    result_safety = column;
                }

            } else if (row == 2) {

                if (check_remove)
                {
                    result_environment = 0;
                } else {

                    result_environment = column;
                }

            } else if (row == 3) {

                if (check_remove)
                {
                    result_damage = 0;
                } else {
                    result_damage = column;
                }

            } else if (row == 4) {

                if (check_remove)
                {
                    result_process = 0;
                } else {
                    result_process = column;
                }

            } else if (row == 5) {

                if (check_remove)
                {
                    result_legal = 0;
                } else {
                    result_legal = column;
                }

            } else if (row == 6) {

                if (check_remove)
                {
                    result_person = 0;
                } else {
                    result_person = column;
                }
            }

            findMaxLevelIncident();
        });


    }


    function findMaxLevelIncident()
    {
        var arr_level = [result_image, result_safety, result_environment, result_damage, result_process, result_legal,result_person];
        var level_max = 0;
        for (i = 0; i < arr_level.length; i++)
        {         
            if (arr_level[i]>level_max)
            {
                level_max = arr_level[i];

            }
        }
        //alert(level_max);

        if(level_max==5)//วิกฤต level 1
        {
            <% if(Session["country"].ToString()=="thailand")
            {
             %>
                $("#MainContent_ddConsequencelevel").val("1");

            <% } else if(Session["country"].ToString()=="srilanka")
             {
           %>
                $("#MainContent_ddConsequencelevel").val("7");
            
            <%
              }
            %>
            

        } else if(level_max == 4){//รุนแรง level 2

            <% if(Session["country"].ToString()=="thailand")
            {
             %>
                $("#MainContent_ddConsequencelevel").val("2");

            <% } else if(Session["country"].ToString()=="srilanka")
             {
           %>
                $("#MainContent_ddConsequencelevel").val("8");

            <%
              }
            %>
           
        } else if (level_max == 3) {//ปานกลาง level 3

             <% if(Session["country"].ToString()=="thailand")
            {
             %>
                $("#MainContent_ddConsequencelevel").val("3");

            <% } else if(Session["country"].ToString()=="srilanka")
             {
           %>
                $("#MainContent_ddConsequencelevel").val("9");

            <%
              }
            %>
           

        } else if (level_max == 2) {//น้อย level 4

             <% if(Session["country"].ToString()=="thailand")
            {
             %>
                $("#MainContent_ddConsequencelevel").val("4");

            <% } else if(Session["country"].ToString()=="srilanka")
             {
           %>
                 $("#MainContent_ddConsequencelevel").val("10");

            <%
              }
            %>
            

        } else if (level_max == 1) {//น้อยมาก level 5

              <% if(Session["country"].ToString()=="thailand")
            {
             %>
                $("#MainContent_ddConsequencelevel").val("5");

            <% } else if(Session["country"].ToString()=="srilanka")
             {
           %>
            $("#MainContent_ddConsequencelevel").val("11");

            <%
              }
            %>
            
            
        }

       
    }

    function setDatatableEffect()
    {

        dataTableEffect = $("#tbEffect").DataTable({
            "bProcessing": true,
            "sProcessing": true,
       
            "bPaginate": false,
            "bInfo": false,
            "bFilter": false,
            "ordering": true,
            // "stateSave": true,
            "responsive": true,
            //"pageLength": 25,
            "lengthChange": false,
            "order": [],
            "language": {
                "url": 'Langdatatable.aspx',
                "decimal": ","
            },
            "columnDefs": [
               {
                   "targets": [0],
                   "visible": false,
               },
               {
                   "targets": [4],
                   "render": $.fn.dataTable.render.number(',', '.', 2)

               }
            ],
             "fnFooterCallback": function (nRow, aaData, iDataStart, iDataEnd) {
                 /* Calculate the total market share for all browsers in this table (ie inc. outside
                  * the pagination
                  */
            var damage_cost = 0;
            for (var i = 0 ; i < aaData.length ; i++)
            {
                damage_cost += aaData[i][4];
            }

                 /* Calculate the market share for browsers on this page */
                 //var iPageMarket = 0;
                 //for (var i = iDataStart ; i < iDataEnd ; i++) {
                 //    iPageMarket += aaData[i][8] * 1;
                 //}

                 /* Modify the footer row to match what we want */
            var nCells = nRow.getElementsByTagName('th');
                 //console.log(nCells);
            var damage_cost_new = addCommas(damage_cost.toFixed(2));
            $("#total_damage_cost").text(damage_cost_new);
                 //nCells[7].innerHTML = parseInt(day_lost);
        }
        });


        dataTableEffect.ajax.url('Datatablelist.asmx/getListDamageList?incident_id=' + id+'&pagetype='+pagetype);



    }

 
    function addCommas(nStr)
    {
        nStr += '';
        x = nStr.split('.');
        x1 = x[0];
        x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }
        return x1 + x2;
    }

    function showCreateInjury()
    {
        $("#MainContent_btCreateInjury").show();
        $("#MainContent_btUpdateInjury").hide();
        dialogInjury.dialog("open");

        return false;

    }

    function showCreateEffect()
    {
        $("#MainContent_btCreatdamage").show();
        $("#MainContent_btUpdatedamage").hide();
        dialogDamageList.dialog("open");

        return false;

    }




     function closeInjury()
    {
        dialogInjury.dialog("close");
        clearValidationErrors();
        clearDataInjury();

     }


     function closeDamage()
     {
         dialogDamageList.dialog("close");
         clearValidationErrors();
         clearDataDamage();
     }



     function addInjury()
     {

         if (Page_ClientValidate("injury"))
         {
             showLoading();
             var full_name = $("#MainContent_txtName").val();
             var type_employment = $("#MainContent_ddlTypeemployment").val();
             var function_id = $("#MainContent_ddlFunction").val();
             var department_id = $("#MainContent_dddepartment").val();
             var nature_injury = $("#MainContent_ddlNatureInjury").val();
             var body_part = $("#MainContent_ddlBodyPart").val();
             var severity_injury = $("#MainContent_ddlSeverityInjury").val();
             var day_lost = $("#MainContent_txtDaylost").val();
             var remark = $("#MainContent_txtremark").val();
             var employee_id = $("#MainContent_employee_injury").val();
           
             $.ajax({
                 type: "POST",
                 data: {
                     name_injury: full_name,
                     employee_id : employee_id,
                     type_employment_id: type_employment,
                     function_id: function_id,
                     department_id :department_id,
                     nature_injury_id: nature_injury,
                     body_part_id: body_part,
                     severity_injury_id: severity_injury,
                     day_cost: day_lost,
                     remark: remark,
                     incident_id: id,
                   
                 },
                 url: 'Actionevent.asmx/createInjuryPerson',
                 dataType: 'json',
                 success: function (result) {
                     closeLoading();                   
                     closeInjury();
                     clearDataInjury();
                     callInjury();
                 }
             });
            

         }
         else {
             return false;
         }


     }



     function updateInjury() {

         if (Page_ClientValidate("injury"))
         {
             showLoading();
             var full_name = $("#MainContent_txtName").val();
             var type_employment = $("#MainContent_ddlTypeemployment").val();
             var function_id = $("#MainContent_ddlFunction").val();
             var department_id = $("#MainContent_dddepartment").val();
             var nature_injury = $("#MainContent_ddlNatureInjury").val();
             var body_part = $("#MainContent_ddlBodyPart").val();
             var severity_injury = $("#MainContent_ddlSeverityInjury").val();
             var day_lost = $("#MainContent_txtDaylost").val();
             var remark = $("#MainContent_txtremark").val();
             var employee_id = $("#MainContent_employee_injury").val();

             $.ajax({
                 type: "POST",
                 data: {
                     name_injury: full_name,
                     employee_id: employee_id,
                     type_employment_id: type_employment,
                     function_id: function_id,
                     department_id: department_id,
                     nature_injury_id: nature_injury,
                     body_part_id: body_part,
                     severity_injury_id: severity_injury,
                     day_cost: day_lost,
                     remark: remark,
                     id: injury_id,

                 },
                 url: 'Actionevent.asmx/updateInjuryPerson',
                 dataType: 'json',
                 success: function (result) {
                     closeLoading();
                     closeInjury();
                     clearDataInjury();
                     callInjury();
                 }
             });


         }
         else {
             return false;
         }


     }



     function addDamage()
     {

         if (Page_ClientValidate("damage"))
         {
             showLoading();
             var property_enviroment = $("#MainContent_txtproperty_enviroment").val();
             var detail_damage = $("#MainContent_txtdetail_damage").val();
             var damage_cost = $("#MainContent_txtdamage_cost").val();
        

             $.ajax({
                 type: "POST",
                 data: {
                     property_enviroment: property_enviroment,
                     detail_damage: detail_damage,
                     damage_cost: damage_cost,
                     incident_id: id,

                 },
                 url: 'Actionevent.asmx/createDamageList',
                 dataType: 'json',
                 success: function (result) {
                     closeLoading();
                     closeDamage();
                     clearDataDamage();
                     callDamage();
                }
             });

             return true

         }
         else {
             return false;
         }


     }



     function updateDamage()
     {

         if (Page_ClientValidate("damage"))
         {
             showLoading();
             var property_enviroment = $("#MainContent_txtproperty_enviroment").val();
             var detail_damage = $("#MainContent_txtdetail_damage").val();
             var damage_cost = $("#MainContent_txtdamage_cost").val();


             $.ajax({
                 type: "POST",
                 data: {
                     property_enviroment: property_enviroment,
                     detail_damage: detail_damage,
                     damage_cost: damage_cost,
                     id: damage_id,

                 },
                 url: 'Actionevent.asmx/updateDamageList',
                 dataType: 'json',
                 success: function (result) {
                     closeLoading();
                     closeDamage();
                     clearDataDamage();
                     callDamage();
                 }
             });

            
             return true
         }
         else {
             return false;
         }


     }






     function clearDataInjury()
     {
         $("#MainContent_txtName").val("");
         $("#MainContent_ddlTypeemployment").val("");
         $("#MainContent_ddlFunction").val("");
         $("#MainContent_ddlNatureInjury").val("");
         $("#MainContent_ddlBodyPart").val("");
         $("#MainContent_ddlSeverityInjury").val("");
         $("#MainContent_txtDaylost").val("");
         $("#MainContent_txtremark").val("");
         $("#MainContent_employee_injury").val("");
         injury_id = 0;
     }

     function clearDataDamage()
     {
         $("#MainContent_txtproperty_enviroment").val("");
         $("#MainContent_txtdetail_damage").val("");
         $("#MainContent_txtdamage_cost").val("");
         damage_id = 0;

     }


     function DeleteInjuryPerson(id)
     {
         var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
         if (confirm(message_confirm_delete))
         {
             showLoading();
             $.ajax({
                 type: "POST",
                 data: { id: id },
                 url: 'Actionevent.asmx/deleteInjuryPerson',
                 dataType: 'json',
                 success: function (json) {

                     closeLoading();
                     callInjury();
                   
                 }
             });

         }


     }

     function callInjury()
     {

         dataTableInjury.ajax.url('Datatablelist.asmx/getListInjuryPerson?incident_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype).load();

     }

     function callDamage()
     {

         dataTableEffect.ajax.url('Datatablelist.asmx/getListDamageList?incident_id=' + id + '&pagetype=' + pagetype).load();

     }


     function DeleteDamageList(id)
     {
         var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
         if (confirm(message_confirm_delete)) {
             showLoading();
             $.ajax({
                 type: "POST",
                 data: { id: id },
                 url: 'Actionevent.asmx/deleteDamageList',
                 dataType: 'json',
                 success: function (json) {

                    
                     closeLoading();
                     callDamage();
                  }
             });

         }


     }


     function clearValidationErrors()
     {
        
         var i;
         for (i = 0; i < Page_Validators.length; i++)
         {
           
                 Page_Validators[i].style.display = "none";         

         }
     }







     function showUpdateReasonReject() {


         dialogReason.dialog("open");

         return false;

     }


     function UpdateReason()
     {

         var reason_type = $("#ddReasonreject").val();
         var reason = $("#MainContent_txtreasonreject").val();

         if (reason_type != "") {
             showLoading();
             $.ajax({
                 type: "POST",
                 data: {
                     incidentid: id,
                     reason_reject_type: reason_type,
                     reasonreject: reason,
                     userid: user_login_id,
                     typelogin: type_login,
                     step_form :2,
                     group_id: user_group_id,

                 },
                 url: 'Actionevent.asmx/updateReasonRejectIncident',
                 dataType: 'json',
                 success: function (result) {

                     closeLoading();
                     dialogReason.dialog("close");
                     $("#MainContent_txtreasonreject").val("");
                     $("#rqreason").text("");
                    // setShowedit();//update status
                     window.location.href = "incidentform2.aspx?pagetype=view&id=" + id;
                 }


             });
         } else {

             var require_reason = '<%= Resources.Incident.rqreasonreject %>';
             $("#rqreason").text(require_reason);
         }



     }


     function CloseReasonReject()
     {
         dialogReason.dialog("close");
         $("#ddReasonreject").val("");
         $("#MainContent_txtreasonreject").val("");


     }



     function validateDaylost(oSrc, args)
     {
         var v = $("#MainContent_ddlSeverityInjury").val();
         if (v == "3") {
             var d = $("#MainContent_txtDaylost").val();
             if (d != "")
             {
                 args.IsValid = true;
             } else {

                 args.IsValid = false;//require
             }
            

         } else {
            // alert("not require");
             args.IsValid = true;
         }
        
     }



     function validateFunction(oSrc, args)
     {
         var v = $("#MainContent_ddlTypeemployment").val();
         if (v != "3")//3 is thridparty not validate
         {
             var d = $("#MainContent_dddepartment").val();
             if (d != "") {
                 args.IsValid = true;
             } else {

                 args.IsValid = false;//require
             }


         } else {
             // alert("not require");
             args.IsValid = true;
         }


     }


     function validateDepartment(oSrc, args)
     {
         var v = $("#MainContent_ddlTypeemployment").val();
         if (v != "3")//3 is thridparty not validate
         {
             var d = $("#MainContent_dddepartment").val();
             if (d != "") {
                 args.IsValid = true;
             } else {

                 args.IsValid = false;//require
             }


         } else {
             // alert("not require");
             args.IsValid = true;
         }

     }



     function validateBodypart(oSrc, args)
     {
         var v = $("#MainContent_ddlSeverityInjury").val();
         if (v == "1") {
             args.IsValid = true;
         } else {
             var b = $("#MainContent_ddlBodyPart").val();
             if (b != "") {
                 args.IsValid = true;
             } else {

                 args.IsValid = false;//require
             }
         }
     }



     function validateInjury()
     {
      
         var v = $("input:radio[name='ctl00$MainContent$injury_fatality_involve']:checked").val();
         if (v == undefined)
         {
             v = "";
         }

         var data_post = JSON.stringify({
             incidentid: id,
         });

         $.ajax({
             type: "POST",
             data: data_post,
             url: 'Actionevent.asmx/checkinjurybyid',
             contentType: "application/json; charset=utf-8",
             success: function (result) {


                 if (v == "N"||v=="")
                 {
                    

                 } else {

                     if (result != "no")//มีค่า
                     {
                         
                     } else {

                        
                     }
                 }


             }
         });



     }



     function changFunction()
     {
         var ddl_function_id = $('#MainContent_ddlFunction').val();

         $.ajax({
             type: "POST",
             data: { function: ddl_function_id, lang: lang },
             url: 'Masterdata.asmx/getDepartmentbyFunction',
             dataType: 'json',
             success: function (json) {

                 var $el = $("#MainContent_dddepartment");
                 $el.empty(); // remove old options

                 $el.append($("<option></option>")
                            .attr("value", "").text(""));
                 $.each(json, function (value, key) {

                     $el.append($("<option></option>")
                             .attr("value", key.id).text(key.name));
                 });


             }
         });



     }
   
</script>


   <div id="show_goverment">
       <div class="row">
            <div class="col-md-12">
               
                          <%
                           
                              if (Session["lang"]=="en")        
                           {                            
       
                          %>
                               <img src="template/img/scco_goverment_eng.png"> 
                          <%                    
                           }
                              else if (Session["lang"] == "th")
                              { 
                          %>
                                <img src="template/img/scco_goverment_thai.png"> 

                         <%                    
                               }
                              else if (Session["lang"] == "si")
                              {      
                          %>
                                <img src="template/img/scco_goverment_si.png" height="304" width="1007"> 

                         <% 
                              }
                          %>




                </div>           				
            </div>   
        
    </div>

 <input type="hidden" id="employee_injury" name="employee_injury" runat="server">
 <div id="create_reason_reject">
      <div class="row">
            <div class="col-md-12">
                <div class="form-group">
                    <label class="control-label"><%= Resources.Incident.lbreasonreject %></label><div class="lbrequire"> *</div>
                      <select id="ddReasonreject" class="form-control">
                       
                        </select>                        
                    <label id="rqreason" class="text-danger"></label>  
                     
                </div>
                </div>

                              				
            </div>
       <div class="row">
            <div class="col-md-12">
                <div class="form-group">
                    <label class="control-label"><%= Resources.Incident.detailreject %></label>
                           
                    <textarea class="form-control" rows="4" id="txtreasonreject" runat="server"></textarea>
                    
                     
                </div>
                </div>

                              				
            </div>
       <div class="row">
             
            <div class="form-group">
                <div class="pull-right">
                 <button id="btConfirm"class="btn btn-sm btn-primary" onclick="UpdateReason()"><%= Resources.Main.btconfirm %></button>
                 <button class="btn btn-sm btn-default" onclick="CloseReasonReject();"><%= Resources.Main.btCancel %></button>
                 </div>
             </div>  
        </div>
        
    </div>

 <div id="injury-form" title="<%= Resources.Incident.add_injury %>">     
         
          	<div class="row">
				
				<div class="col-sm-6">
					<div class="form-group">
						<label class="control-label"><%= Resources.Incident.name_injury %></label><div class="lbrequire"> *</div>
						<input id="txtName" name="txtName"  type="text" class="form-control" runat="server">
                         <asp:RequiredFieldValidator ID="rqNameInjury" runat="server"  ValidationGroup="injury" ControlToValidate ="txtName" ErrorMessage="<%$ Resources:Incident, rqNameInjury %>" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>

                  <div class="col-sm-6">
					<div class="form-group">
						<label class="control-label"><%= Resources.Incident.type_employment %></label><div class="lbrequire"> *</div>
						 <select id="ddlTypeemployment" class="form-control" runat="server">
                            
                        </select>
                        <asp:RequiredFieldValidator ID="rqddltypeemployment" runat="server" ValidationGroup="injury" ControlToValidate ="ddlTypeemployment" ErrorMessage="<%$ Resources:Incident, rqddltypeemployment %>" CssClass="text-danger"  Display="Dynamic">
                       </asp:RequiredFieldValidator>			
                    </div>
				</div>

				
                  
		   </div>



            <div class="row">
                <div class="col-sm-4">
					<div class="form-group">
						<label class="control-label"> <%= Resources.Incident.lbfunction_injured %></label>
					    <select id="ddlFunction" class="form-control" runat="server" onchange="changFunction();">
                            
                        </select>
                         <asp:CustomValidator id="rqfunction" runat="server"  ValidationGroup="injury" ControlToValidate = "ddlFunction" ErrorMessage = "<%$ Resources:Incident, rqfunction %>"  CssClass="text-danger"  Display="Dynamic"   ClientValidationFunction="validateFunction" ValidateEmptyText="true">
                        </asp:CustomValidator>
                         <%--<asp:RequiredFieldValidator ID="rqfunction" runat="server" ValidationGroup="injury" ControlToValidate ="ddlFunction" ErrorMessage="<%$ Resources:Incident, rqfunction %>" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>--%>
					</div>
				</div>


                   <div class="col-md-4">
                    <div class="form-group">
                    <label class="control-label"><%= Resources.Incident.lbdepartment_injured %></label>                                             
                                    
                        <select id="dddepartment" class="form-control"  runat="server">
                       
                        </select>
                         <asp:CustomValidator id="rqDepartment" runat="server"  ValidationGroup="injury" ControlToValidate = "dddepartment" ErrorMessage = "<%$ Resources:Incident, rqdepartment %>"  CssClass="text-danger"  Display="Dynamic"   ClientValidationFunction="validateDepartment" ValidateEmptyText="true">
                        </asp:CustomValidator>
                         <%--   <asp:RequiredFieldValidator ID="rqDepartment" runat="server" ControlToValidate ="dddepartment" ErrorMessage="<%$ Resources:Incident, rqdepartment %>" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>--%>
                    </div>
                </div>

                <div class="col-sm-4">
					<div class="form-group">
						<label class="control-label"> <%= Resources.Incident.severity_injury %></label><div class="lbrequire"> *</div>
					    <select id="ddlSeverityInjury" class="form-control" runat="server">
                            
                        </select>
                         <asp:RequiredFieldValidator ID="rqseverityinjury" runat="server" ValidationGroup="injury" ControlToValidate ="ddlSeverityInjury" ErrorMessage="<%$ Resources:Incident, rqseverityinjury %>" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
				
			
                 
		   </div>


            <div class="row">

                	<div class="col-sm-4">
					<div class="form-group">
						<label class="control-label"><%= Resources.Incident.nature_injury %></label><div class="lbrequire"> *</div>
						 <select id="ddlNatureInjury" class="form-control" runat="server">
                            
                        </select>
                        <asp:RequiredFieldValidator ID="rqnatureinjury" runat="server" ValidationGroup="injury" ControlToValidate ="ddlNatureInjury" ErrorMessage="<%$ Resources:Incident, rqnatureinjury %>" CssClass="text-danger"  Display="Dynamic">
                       </asp:RequiredFieldValidator>			
                    </div>
				</div>
			
                	<div class="col-sm-4">
					<div class="form-group">
						<label class="control-label"><%= Resources.Incident.body_parts %></label>
						 <select id="ddlBodyPart" class="form-control" runat="server">
                            
                        </select>
                        <asp:CustomValidator id="rqbodypart" runat="server"  ValidationGroup="injury" ControlToValidate = "ddlBodyPart" ErrorMessage = "<%$ Resources:Incident, rqbodypart %>"  CssClass="text-danger"  Display="Dynamic"   ClientValidationFunction="validateBodypart" ValidateEmptyText="true" >
                        </asp:CustomValidator>
                       	 <%--<asp:RequiredFieldValidator ID="rqbodypart" runat="server" ValidationGroup="injury" ControlToValidate ="ddlBodyPart" ErrorMessage="<%$ Resources:Incident, rqbodypart %>" CssClass="text-danger"  Display="Dynamic">
                       </asp:RequiredFieldValidator>	--%>	
                    </div>
				</div>
				
				<div class="col-sm-4">
					<div class="form-group">
						<label class="control-label" id="lbdaylost"><%= Resources.Incident.day_lost %></label>
						 <input id="txtDaylost" name="txtDaylost" type="text" class="form-control" runat="server">
                        
                          <asp:CustomValidator id="rqdaylost" runat="server"  ValidationGroup="injury" ControlToValidate = "txtDaylost" ErrorMessage = "<%$ Resources:Incident, rqdaylost %>"  CssClass="text-danger"  Display="Dynamic"   ClientValidationFunction="validateDaylost" ValidateEmptyText="true">
                        </asp:CustomValidator>
                       <%-- <asp:RequiredFieldValidator ID="rqdaylost" runat="server" ValidationGroup="injury" ControlToValidate ="txtDaylost" ErrorMessage="<%$ Resources:Incident, rqdaylost %>" CssClass="text-danger"  Display="Dynamic">
                       </asp:RequiredFieldValidator>--%>	

                        <asp:RegularExpressionValidator ID="rqcheckdaylost" runat="server" ControlToValidate="txtDaylost" SetFocusOnError="true" ErrorMessage="<%$ Resources:Incident, rqdaylostcheck %>" ValidationExpression="^\d+$" Display="Dynamic" CssClass="text-danger">
                         </asp:RegularExpressionValidator>
                        
                  </div>
				</div>
			
                 
		   </div>

            <div class="row">
                <div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"> <%= Resources.Incident.remark %></label>
					     <textarea class="form-control" rows="5" id="txtremark" runat="server"></textarea>

					</div>
				</div>
				
                 
		   </div>

          

             <div class="row">
                <div class="col-sm-4">
                    
                </div>
               
                 <div class="col-sm-4">
                    
                </div>

                  <div class="col-sm-4">
                    <div class="form-group pull-right">
                        <asp:Button ID="btCreateInjury" runat="server"  ValidationGroup="injury" Text="<%$ Resources:Main, btadd %>" OnClientClick="addInjury();" CssClass="btn btn-primary"/>
                        <asp:Button ID="btUpdateInjury" runat="server"  ValidationGroup="injury" Text="<%$ Resources:Main, btsave %>" OnClientClick="updateInjury();" CssClass="btn btn-primary"/>
                        <button type="button" id="btCancel" class="btn btn-default" runat="server" onclick="closeInjury();"><%= Resources.Main.btCancel %></button>
                    </div>
                </div>
             </div>

      
    </div>



 <div id="damage-list-form" title="<%= Resources.Incident.add_damage_list %>">     
         
          	<div class="row">
				
				<div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"><%= Resources.Incident.list_property_enviroment_damage %></label><div class="lbrequire"> *</div>
						<input id="txtproperty_enviroment" name="txtproperty_enviroment"  type="text" class="form-control" runat="server">
                        
                         <asp:RequiredFieldValidator ID="rqpropertyenviroment" runat="server" ControlToValidate ="txtproperty_enviroment" ErrorMessage="<%$ Resources:Incident, rqpropertyenviroment %>" 
                             ValidationGroup="damage" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
                  
		   </div>



            <div class="row">
                <div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"> <%= Resources.Incident.detail_damage %></label><div class="lbrequire"> *</div>
					     <textarea class="form-control" rows="5" id="txtdetail_damage" runat="server"></textarea>
                         <asp:RequiredFieldValidator ID="rqdetaildmage" runat="server" ControlToValidate ="txtdetail_damage" ErrorMessage="<%$ Resources:Incident,rqdetaildmage %>" 
                           ValidationGroup="damage"   CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>				              
		   </div>

           <div class="row">
				
				<div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"><%= Resources.Incident.damage_cost %></label><div class="lbrequire"> *</div>
						<input id="txtdamage_cost" name="txtdamage_cost"  type="text" class="form-control" runat="server">
                         <asp:RequiredFieldValidator ID="rqdamagecost" runat="server" ControlToValidate ="txtdamage_cost" ErrorMessage="<%$ Resources:Incident, rqdamagecost %>" 
                               ValidationGroup="damage" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="rqdamagecostcheck" runat="server" ControlToValidate="txtdamage_cost" SetFocusOnError="true" ErrorMessage="<%$ Resources:Incident, rqdamagecostcheck %>" ValidationExpression="^\d*\.?\d*$" Display="Dynamic" CssClass="text-danger">
                         </asp:RegularExpressionValidator>
					</div>
				</div>
                  
		   </div>

          

             <div class="row">
      
                  <div class="col-sm-12">
                    <div class="form-group pull-right">
                        <asp:Button ID="btCreatdamage" runat="server" ValidationGroup="damage"  Text="<%$ Resources:Main, btadd %>" OnClientClick="addDamage();" CssClass="btn btn-primary"/>
                        <asp:Button ID="btUpdatedamage" runat="server" ValidationGroup="damage"  Text="<%$ Resources:Main, btsave %>" OnClientClick="updateDamage();" CssClass="btn btn-primary"/>
                        <button type="button" id="btCloseDamage" class="btn btn-default" runat="server" onclick="closeDamage();"><%= Resources.Main.btCancel %></button>
                    </div>
                </div>
             </div>

      
    </div>








 
  
<div class="ibox float-e-margins">
                
    <div class="ibox-content" style="display: block;">

               
<div class="stepwizard">
      <div class="stepwizard-row setup-panel">
        <div class="stepwizard-step">
            <asp:LinkButton ID="step1" runat="server" CssClass="btn btn-default btn-circle a-step" CausesValidation="False" OnClick="step1_Click">1</asp:LinkButton>
        <p><%= Resources.Incident.incidentstep1 %></p>
        </div>
        <div class="stepwizard-step">
        <asp:LinkButton ID="step2" runat="server" CssClass="btn btn-primary btn-circle" CausesValidation="False" OnClick="step2_Click">2</asp:LinkButton>
            <p><%= Resources.Incident.incidentstep2 %></p>
        </div>
        <div class="stepwizard-step">
         <asp:LinkButton ID="step3" runat="server" CssClass="btn btn-default btn-circle a-step" CausesValidation="False" OnClick="step3_Click">3</asp:LinkButton>
                            
        <p><%= Resources.Incident.incidentstep3 %></p>
        </div>
         <div class="stepwizard-step">
            <asp:LinkButton ID="step4" runat="server" CssClass="btn btn-default btn-circle a-step" CausesValidation="False" OnClick="step4_Click">4</asp:LinkButton>
                            
        <p><%= Resources.Incident.incidentstep4 %></p>
        </div>
    </div>
</div>

    <div class="row setup-content" id="step-1">
        <div class="col-xs-12">
            <div class="col-lg-12">
                <div class="row">
                    <div class="col-md-12">
                    <div style="font-weight:bold;" id="show_doc_status"></div>
                        <div class="pull-right">
                         
                           <%
                                                        
                            string id = Request.QueryString["id"];
                           
                            ArrayList per = Session["permission"] as ArrayList;
                            
                            bool pa = safetys4.Class.SafetyPermission.checkPermisionAction("report incident2 approve", id, "incident", Convert.ToInt32(Session["group_value"]));
                            bool area = safetys4.Class.SafetyPermission.checkPermisionInArea(id, "incident");


                            if (per.IndexOf("report incident2 approve") > -1 && pa == true && area == true)         
                           {                            
       
                          %>
                               <asp:Button ID="btUpdateReport" runat="server" Text="<%$ Resources:Main, btSubmitReport %>"  CssClass="btn btn-primary" OnClientClick="return updateIncident('report');" />           

                          <%                      
                            }
       
                          %>


                          <%
                              string id2 = Request.QueryString["id"];

                              ArrayList per2 = Session["permission"] as ArrayList;
                             
                              bool pa2 = safetys4.Class.SafetyPermission.checkPermisionAction("report incident2 confirm", id2, "incident", Convert.ToInt32(Session["group_value"]));
                              bool area2 = safetys4.Class.SafetyPermission.checkPermisionInArea(id2, "incident");

                              if (per2.IndexOf("report incident2 confirm") > -1 && pa2 == true && area2 == true)         
                           {                            
       
                          %>
                               <asp:Button ID="btIncidentconfirm" runat="server" Text="<%$ Resources:Incident, btIncidentconfirm %>" CssClass="btn btn-primary" ValidationGroup="incident" OnClick="btIncidentconfirm_Click"/>

                          <%                      
                            }
       
                          %>


                        
                          <%
                              string id5 = Request.QueryString["id"];
                              ArrayList per5 = Session["permission"] as ArrayList;
                              
                              bool pa5 = safetys4.Class.SafetyPermission.checkPermisionAction("report incident2 confirm groupohs", id5, "incident", Convert.ToInt32(Session["group_value"]));
                           

                              if (per5.IndexOf("report incident2 confirm groupohs") > -1 && pa5 == true)         
                           {                            
       
                          %>

                          <asp:Button ID="btIncidentconfirmgroup" runat="server" Text="<%$ Resources:Incident, btIncidentconfirmgroup %>" CssClass="btn btn-primary" CausesValidation="False" OnClick="btIncidentconfirmgroup_Click"/>

                         <%                      
                            }
       
                          %>





                          <%
                              string id3 = Request.QueryString["id"];
                              ArrayList per3 = Session["permission"] as ArrayList;
                              
                              bool pa3 = safetys4.Class.SafetyPermission.checkPermisionAction("report incident2 reject", id3, "incident", Convert.ToInt32(Session["group_value"]));
                              bool area3 = safetys4.Class.SafetyPermission.checkPermisionInArea(id3, "incident");

                           
                            if (per3.IndexOf("report incident2 reject") > -1 && pa3 == true && area3 == true)         
                           {                            
       
                          %>
                              <asp:Button ID="btIncidentreject" runat="server" Text="<%$ Resources:Incident, btIncidentreject %>" CssClass="btn btn-primary" CausesValidation="False" OnClientClick="return showUpdateReasonReject();"/>

                          <%                      
                            }
       
                          %>
                              
                          

                         </div>
                    </div>
                  </div>
                <hr>
                 <div class="row">
                    <div class="col-md-12">
                    <div class="pull-right">

                         <%
                             
                             string id4 = Request.QueryString["id"];
                             ArrayList per4 = Session["permission"] as ArrayList;
                             
                             bool pa4 = safetys4.Class.SafetyPermission.checkPermisionAction("report incident2 edit", id4, "incident", Convert.ToInt32(Session["group_value"]));
                             bool area4 = safetys4.Class.SafetyPermission.checkPermisionInArea(id4, "incident");


                             if (per4.IndexOf("report incident2 edit") > -1 && pa4 == true && area4 == true)         
                           {                            
       
                          %>
                             <asp:Button ID="btIncidentedit" runat="server" Text="<%$ Resources:Incident, btIncidentedit %>" CssClass="btn btn-primary"  CausesValidation="False" OnClick="btIncidentedit_Click"/>

                          <%                      
                            }
       
                          %>
                    </div>
                    </div>
                  </div>
                           
                
                
                
                 <div class="row" style="padding-bottom:10px;"> 
                   <%--  <div class="col-md-4"><strong><h3><%= Resources.Incident.sourceincident %></h3></strong></div></div>

                			<div class="row">
                                <div class="col-md-12">
                                    <label class="control-label"><%= Resources.Incident.workrelate %></label>
                                     <div  class="form-group">
                                        
                                         <div class="col-sm-6">
                                            <label> <input  value="N" id="work_relate1" name="work_relate" type="radio" runat="server">
                                            <%= Resources.Incident.no %> </label>
                                         </div>
                                         <div class="col-sm-6">
                                              <label> <input value="Y" id="work_relate2" name="work_relate" type="radio" runat="server">
                                            <%= Resources.Incident.yes %> </label>
                                         </div>
                                        
                                    </div>

                                </div>
                              
                            </div>--%>

                      <% if (Session["country"].ToString() == "srilanka")
                     {                  
                     %>  

                        <div class="row">
                                <div class="col-md-12">
                                    <label class="control-label"><strong><h3><%= Resources.Incident.responsible_area %></h3></strong></label><div class="lbrequire"> *</div>    
                                     <div class="form-group">
                                        
                                        <div class="col-sm-6">
                                          <label> <input value="IN" id="responsible_area1" name="responsible_area" type="radio" runat="server">
                                            <%= Resources.Incident.onsite %></label>
                                        </div>
                                        <div class="col-sm-6">
                                         <label> <input value="OUT" id="responsible_area2" name="responsible_area" type="radio" runat="server">
                                            <%= Resources.Incident.offsite %></label>
                                        </div>
                                         <label id="rqresponsiblearea" class="text-danger"></label>  
                                    </div>

                                </div>
                              
                            </div>

                     <% 
                      }    
                     %>  



                  

                    <div class="row">
                                <div class="col-md-12">
                                    <label class="control-label"><strong><h3><%= Resources.Incident.impact_incident %></h3></strong></label><div class="lbrequire"> *</div>    
                                     <div class="form-group">
                                        
                                        <div class="col-sm-6">
                                             <label class="control-label"> <input  value="N" id="impact1" name="impact" type="radio" runat="server">
                                            <%= Resources.Incident.no_impact %> </label>

                                        </div>
                                        <div class="col-sm-6">

                                            <label class="control-label"> <input value="Y" id="impact2" name="impact" type="radio"  runat="server">
                                            <%= Resources.Incident.impact %></label>
                                         
                                        </div>
                                          <label id="rqimpact" class="text-danger"></label>  
                                         
                                    </div>

                                </div>
                        
                   </div>

               

                <div id="hd_injury_fatality_involve" class="row">
                    <div class="col-md-12">
                         <label class="control-label"> <strong><h3><%= Resources.Incident.injury_fatality %></h3></strong></label>                             
                        <div  class="form-group">
                              
                                <div class="col-sm-6">      
                                    <label> <input  value="N" id="injury_fatality_involve1" name="injury_fatality_involve" type="radio" runat="server">
                                    <%= Resources.Incident.no_injury_fatality %></label>
                                </div>
                                <div class="col-sm-6">
                                    <label> <input value="Y" id="injury_fatality_involve2" name="injury_fatality_involve" type="radio" runat="server">
                                   <%= Resources.Incident.injury_fatality_involve %> </label>
                                </div>       
                        </div>

                       </div>
                              
                   </div>

                 <div id="hd_tbinjury" class="row">
                    <div class="col-md-12">
                          <table id="tbInjury" class="table table-bordered table-hover">
                             <thead>
                                <tr>
                                    <th> <%= Resources.Incident.no %></th>
                                    <th></th>
                                    <th> <%= Resources.Incident.name_injury %></th>
                                    <th> <%= Resources.Incident.type_employment %></th>
                                    <th> <%= Resources.Incident.lbfunction_injured %></th>
                                    <th> <%= Resources.Incident.nature_injury %></th>
                                    <th> <%= Resources.Incident.body_parts %></th>
                                    <th> <%= Resources.Incident.severity_injury %></th>
                                    <th> <%= Resources.Incident.day_lost %></th>                
                                    <th> <%= Resources.Incident.remark %></th>
                                    <th> <%= Resources.Incident.manage %></th>
                       
                    
                                </tr>
                            </thead>
                              <tfoot>
		                        <tr>
			                        <th style="text-align:right" colspan="8"> <%= Resources.Incident.total_lost_day %></th>
			                        <th id="total_day_lost"></th>
                                    <th  colspan="2"></th>
		                        </tr>
	                        </tfoot>
         
                            </table>
                              <label id="rqinjury" class="text-danger"></label> 



                       </div>
                              
                   </div>

                <div id="hd_btaddinjury" class=="row">
                    <div class="col-md-12">
                       <button type="button" id="btAddInjury" class="btn btn-primary" runat="server" onclick="showCreateInjury();"><i class="fa fa-plus"></i></button>  <%= Resources.Incident.add_injury %>

                       </div>
                              
                   </div>


                 <div class="row">
                    <div class="col-md-12">
                          

                       </div>
                              
                   </div>


               <div id="hd_effect_environment"  class="row">
                    <div class="col-md-12">
                             <label class="control-label"> <strong><h3> <%= Resources.Incident.consequence_property_environment %></h3></strong></label>
                          
                            <div class="form-group">
                                
                                <div class="col-sm-6">        
                                    <label> <input  value="N" id="effect_environment1" name="effect_environment" type="radio"  runat="server">
                                    <%= Resources.Incident.no_effect_property_environment %></label>
                                </div>
                                <div class="col-sm-6">
                                    <label> <input value="Y" id="effect_environment2" name="effect_environment" type="radio"  runat="server">
                                    <%= Resources.Incident.effect_property_environment %> </label>
                                </div>
                                         
                        </div>

                       </div>
                              
                   </div>

                  <div id="hd_tbeffect" class="row">
                    <div class="col-md-12">
                         <div class="pull-right form-inline" style="padding-bottom:5px;"><label style="padding-right:10px;"><%= Resources.Incident.currency %></label><select id="ddCurrency" class="form-control" runat="server"></select></div>
                          <table id="tbEffect" class="table table-bordered table-hover">
                             <thead>
                                <tr>
                                    <th> <%= Resources.Incident.no %></th>
                                    <th></th>
                                    <th> <%= Resources.Incident.list_property_enviroment_damage %></th>
                                    <th> <%= Resources.Incident.detail_damage %></th>
                                    <th> <%= Resources.Incident.damage_cost %> </th>
                                    <th> <%= Resources.Incident.manage %></th>

                                </tr>
                            </thead>
                            <tfoot>
		                        <tr>
			                        <th style="text-align:right" colspan="4"> <%= Resources.Incident.total_damage_cost %></th>
			                        <th id="total_damage_cost"></th>
                                    <th></th>
		                        </tr>
	                        </tfoot>
                            </table>
                            <label id="rqeffect" class="text-danger"></label> 



                       </div>
                              
                   </div>

                <div id="hd_btaddeffect" class="row">
                    <div class="col-md-12">
                       <button type="button" id="btAddEffect" class="btn btn-primary" runat="server" onclick="showCreateEffect();"><i class="fa fa-plus"></i></button> <%= Resources.Incident.add_damage_list %>

                       </div>
                              
                   </div>

                 <div class="row">
                    <div class="col-md-12">
                      
                       </div>
                              
                   </div>

                   <div class="row">
                    <div class="col-md-12">
                            <strong><h3><%= Resources.Incident.consequence_criteria_guideline %></h3></strong>
                       </div>
                              
                   </div>

                
              <div id="hd_btDefinitionlevel" class="row">
                   <div class="col-md-12">

                       <table id="tbDefinitionLevel" class="table table-bordered table-hover">
                             <thead>
                                  <tr>

                                    <%
                           
                                        if (Session["lang"]=="si")        
                                       {                            
       
                                    %>
                                            <th></th>
                                            <th>1</th>
                                            <th>2</th>
                                            <th>3</th>
                                            <th>4</th>
                                            <th>5</th>
                                            <th>#</th>
                                      <%
                                      }
                                      else    
                                       {                            
       
                                    %>
                                            <th></th>
                                            <th>5</th>
                                            <th>4</th>
                                            <th>3</th>
                                            <th>2</th>
                                            <th>1</th>
                                            <th>#</th>

                                       <%
                                      }
                                      
                                    %>
                                </tr>
                                <tr>
                                    <th></th>
                                    <th> <%= Resources.Incident.level5 %></th>
                                    <th> <%= Resources.Incident.level4 %></th>
                                    <th> <%= Resources.Incident.level3 %></th>
                                    <th> <%= Resources.Incident.level2 %></th>
                                    <th> <%= Resources.Incident.level1 %></th>
                                    <th> <%= Resources.Incident.definition_level %></th>

                                </tr>

                              </thead>                              
                              <tfoot>
		                        <tr>
			                        <th style="text-align:center;background-color:#f5f5f6;font-weight:bold;" colspan="7"> <%= Resources.Incident.level_incident_table %></th>

		                        </tr>
	                        </tfoot>

                              <tbody>
                                
                                <tr>
                                     <td>1</td>
                                    <td><%= Resources.Incident.level5_image %></td>
                                    <td><%= Resources.Incident.level4_image %></td>
                                    <td><%= Resources.Incident.level3_image %></td>
                                    <td><%= Resources.Incident.level2_image %></td>
                                    <td><%= Resources.Incident.level1_image %></td>
                                    <td style="background-color:#f5f5f6;font-weight:bold;"><%= Resources.Incident.definition_image %></td>
                                </tr>

                                  <tr>
                                    <td>2</td>
                                    <td><%= Resources.Incident.level5_safety %></td>
                                    <td><%= Resources.Incident.level4_safety %></td>
                                    <td><%= Resources.Incident.level3_safety %></td>
                                    <td><%= Resources.Incident.level2_safety %></td>
                                    <td><%= Resources.Incident.level1_safety %></td>
                                    <td style="background-color:#f5f5f6;font-weight:bold;"><%= Resources.Incident.definition_safety %></td>
                                </tr>

                                  <tr>
                                    <td>3</td>
                                    <td><%= Resources.Incident.level5_environment %></td>
                                    <td><%= Resources.Incident.level4_environment %></td>
                                    <td><%= Resources.Incident.level3_environment %></td>
                                    <td><%= Resources.Incident.level2_environment %></td>
                                    <td><%= Resources.Incident.level1_environment %></td>
                                    <td style="background-color:#f5f5f6;font-weight:bold;"><%= Resources.Incident.definition_environment %></td>
                                </tr>

                                  <tr>
                                    <td>4</td>
                                    <td><%= Resources.Incident.level5_damage %></td>
                                    <td><%= Resources.Incident.level4_damage %></td>
                                    <td><%= Resources.Incident.level3_damage %></td>
                                    <td><%= Resources.Incident.level2_damage %></td>
                                    <td><%= Resources.Incident.level1_damage %></td>
                                    <td style="background-color:#f5f5f6;font-weight:bold;"><%= Resources.Incident.definition_damage %></td>
                                </tr>
                                 <tr>
                                    <td>5</td>
                                    <td><%= Resources.Incident.level5_process %></td>
                                    <td><%= Resources.Incident.level4_process %></td>
                                    <td><%= Resources.Incident.level3_process %></td>
                                    <td><%= Resources.Incident.level2_process %></td>
                                    <td><%= Resources.Incident.level1_process %></td>
                                    <td style="background-color:#f5f5f6;font-weight:bold;"><%= Resources.Incident.definition_process %></td>
                                </tr>

                                 <tr>
                                    <td>6</td>
                                    <td><%= Resources.Incident.level5_legal %></td>
                                    <td><%= Resources.Incident.level4_legal %></td>
                                    <td><%= Resources.Incident.level3_legal %></td>
                                    <td><%= Resources.Incident.level2_legal %></td>
                                    <td><%= Resources.Incident.level1_legal %></td>
                                    <td style="background-color:#f5f5f6;font-weight:bold;"><%= Resources.Incident.definition_legal %></td>
                                </tr>

                                 <tr>
                                    <td>7</td>
                                    <td><%= Resources.Incident.level5_person %></td>
                                    <td><%= Resources.Incident.level4_person %></td>
                                    <td><%= Resources.Incident.level3_person %></td>
                                    <td><%= Resources.Incident.level2_person %></td>
                                    <td><%= Resources.Incident.level1_person %></td>
                                    <td style="background-color:#f5f5f6;font-weight:bold;"><%= Resources.Incident.definition_person %></td>
                                </tr>

                                </tbody>
         
                            </table>






                    </div>

               </div>


                 <div class="row">
                    <div class="col-md-12">
                            <label class="control-label"> <strong><h3><%= Resources.Incident.other_impact %></h3></strong></label> 
                            <div class="form-group" id="impact_checkbox">   
                                <div class="col-sm-4" >                                     
                                    <label> 
                                       
                                        <input  value="image" id="other_impact1" name="other_impact" type="checkbox" runat="server">
                                    <%= Resources.Incident.potential_image %> </label>
                                </div>
                               <div class="col-sm-4">   
                                    <label> 
                                        
                                      <input value="legal" id="other_impact2" name="other_impact" type="checkbox" runat="server">
                                   <%= Resources.Incident.potential_legal %> </label>
                                </div>
                                <div class="col-sm-4">   
                                     <label> 
                                          
                                   <input value="manufacturing" id="other_impact3" name="other_impact" type="checkbox" runat="server">
                                   <%= Resources.Incident.potential_issue %> </label>
                                 </div> 
                        </div>
                        
                       </div>
                              
                   </div>


                   <div class="row">
                    <div class="col-md-12">
                         <label class="control-label"> <strong><h3> <%= Resources.Incident.critical_incident %></h3></strong></label>
                          
                            <div class="form-group">
                                
                                <div class="col-sm-6">        
                                    <label> <input  value="N" id="critical1" name="critical" type="radio" runat="server">
                                    <%= Resources.Incident.no %> </label>
                                </div>
                                <div class="col-sm-6">        
                                    <label> <input value="Y" id="critical2" name="critical" type="radio" runat="server">
                                    <%= Resources.Incident.yes %> </label>
                                </div>
                                
                                         
                        </div>

                       </div>
                              
                   </div>


                 <div class="row">
                    <div class="col-md-12">
                           <label class="control-label"> <strong><h3> <%= Resources.Incident.external_reportable %></h3></strong></label>
                            <button type="button" class="btn btn-danger btn-circle-new" data-toggle="tooltip" data-placement="right" 
                                         onclick="showDialogGoverment();">
                                <i class="fa fa-info"></i></button>
                            
                            <div class="form-group">
                             
                                <div class="col-sm-6">             
                                    <label> <input  value="N" id="external_reportable1" name="external_reportable" type="radio" runat="server">
                                    <%= Resources.Incident.no %> </label>
                                </div>
                                <div class="col-sm-6">        
                                    <label> <input value="Y" id="external_reportable2" name="external_reportable" type="radio" runat="server">
                                   <%= Resources.Incident.yes %> </label>
                               </div>   
                               
                        </div>

                       </div>
                              
                   </div>

                 <div class="row">
                    <div class="col-md-12">
                       <strong><h3><%= Resources.Incident.corrective_action %></h3></strong>
                   
                    </div>
                  </div>
                    <div class="row">
                              
                        <div class="col-md-12">
                            <div class="form-group">
                            <label class="control-label"><%= Resources.Incident.immediate_temporary_action %> (4000 <%= Resources.Incident.rqCharacters %>)</label>
                                
                                <textarea class="form-control" rows="5" id="txtimmediate_temporary" runat="server"></textarea>
                                <asp:CustomValidator id="rqIncidentImmediateLength" runat="server" ValidationGroup="incident" ClientValidationFunction="CheckIncidentImmediateLength" Display="Dynamic" ControlToValidate="txtimmediate_temporary"  ErrorMessage="<%$ Resources:Incident, rqIncidentImmediateLength %>" CssClass="text-danger"></asp:CustomValidator>
      
                            </div>
                        </div>

                              				
                    </div>

               	<div class="row">
                    <div class="col-md-4">
                            <div class="form-group">
                            <label class="control-label"><%= Resources.Incident.consequence_level %></label><div class="lbrequire"> *</div>    
                                <select id="ddConsequencelevel" name="ddConsequencelevel" class="form-control"  runat="server">
                       
                            </select>

                           <asp:RequiredFieldValidator ID="rqconsequencelevel" ValidationGroup="incident" runat="server" ControlToValidate ="ddConsequencelevel" ErrorMessage="<%$ Resources:Incident, rqconsequencelevel %>" CssClass="text-danger" Display="Dynamic">
                          </asp:RequiredFieldValidator>
                                         
                        </div>

                    </div>
                             
                          
                </div>


                
                              				
            
             <div class="row">                             
                <div class="col-md-12">
                  <span class="pull-right">
                            <asp:Label ID="lbModify" runat="server" Text="<%$ Resources:Contractor, lbModify %>"></asp:Label>
                            <asp:Label ID="lbEmployee" runat="server" Text=""></asp:Label>
                            <asp:Label ID="lbDate" runat="server" Text="<%$ Resources:Contractor, lbUpdate %>"></asp:Label>
                            <asp:Label ID="lbUpdate" runat="server" Text=""></asp:Label>
                   </span>
               </div>
            </div>

            <div class="row">                             
                <div class="col-md-4">
                    <asp:Button ID="btUpdate" runat="server" ValidationGroup="incident" Text="<%$ Resources:Main, btsave%>"  CssClass="btn btn-primary" OnClientClick="return updateIncident('');" />           
               </div>
            </div>
              

            </div>
        </div>
    </div>
 



                       
                </div>
            </div>

</asp:Content>
