<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.master" AutoEventWireup="true" CodeBehind="rpexcel_bak.aspx.cs" Inherits="QueryReport.rpexcel_bak" %>

<asp:Content ID="header" ContentPlaceHolderID="header" runat="server">
    <style type="text/css">
        .LabelStyle
        {
            margin-right: 5px;
        }
        #div_formula SELECT, #div_formula INPUT, #div_formula BUTTON 
        {
            height: 30px;
        }
        #div_formula DIV
        {
            padding: 3px 10px;
        }
    </style>
    <script type="text/javascript">
        function Validate_Action(ctrlname, action, selection) {
            switch (action) {
                case "add":
                    {
                        switch (ctrlname) {
                            case "lbgroupavg":
                            case "lbgroupcount":
                            case "lbgrouptotal":
                                {
                                    if (selection == null) return false;
                                    var elements = $('[data-controlname=lbhiden] option');
                                    var i;
                                    var j;
                                    var found;
                                    for (i = 0; i < selection.length; i++) {
                                        found = false;
                                        j = 0;
                                        while ((!found) && (j < elements.length)) {
                                            if (elements[j].value == selection[i].value) {
                                                found = true;
                                            }
                                            j++;
                                        }
                                        if (!found) {
                                            alert("Field [" + selection[i].text + "] must be added to group first!");
                                            return false;
                                        }
                                    }
                                    return true;
                                }
                                break;
                        }
                        return true;
                    }
                    break;
                case "remove":
                    {
                        switch (ctrlname) {
                            case "lbhiden":
                                {
                                    var selection = $('[data-controlname=lbhiden] option:selected');
                                    var ctrllist = ['lbgroupavg', 'lbgroupcount', 'lbgrouptotal'];
                                    var ctrlcnt = 0;
                                    var elements = null;
                                    var i;
                                    var j;
                                    var found;

                                    for (ctrlcnt = 0; ctrlcnt < ctrllist.length; ctrlcnt++) {
                                        elements = $('[data-controlname=' + ctrllist[ctrlcnt] + '] option');

                                        for (i = 0; i < selection.length; i++) {
                                            found = false;
                                            j = 0;
                                            while ((!found) && (j < elements.length)) {
                                                if (elements[j].value == selection[i].value) {
                                                    found = true;
                                                }
                                                j++;
                                            }
                                            if (found) {
                                                if (ctrllist[ctrlcnt] == 'lbgroupavg') {
                                                    alert("Field [" + selection[i].text + "] must be removed from group average first!");
                                                } else if (ctrllist[ctrlcnt] == 'lbgroupcount') {
                                                    alert("Field [" + selection[i].text + "] must be removed from group count first!");
                                                } else if (ctrllist[ctrlcnt] == 'lbgrouptotal') {
                                                    alert("Field [" + selection[i].text + "] must be removed from group total first!");
                                                }
                                                return false;
                                            }
                                        }
                                    }
                                    return true;
                                }
                                break;
                        }
                        return true;
                    }
                    break;
                case "reset":
                    {
                        switch (ctrlname) {
                            case "lbhiden":
                                {
                                    var selection = $('[data-controlname=lbhiden] option');
                                    var ctrllist = ['lbgroupavg', 'lbgroupcount', 'lbgrouptotal'];
                                    var ctrlcnt = 0;
                                    var elements = null;
                                    var i;
                                    var j;
                                    var found;

                                    for (ctrlcnt = 0; ctrlcnt < ctrllist.length; ctrlcnt++) {
                                        elements = $('[data-controlname=' + ctrllist[ctrlcnt] + '] option');

                                        for (i = 0; i < selection.length; i++) {
                                            found = false;
                                            j = 0;
                                            while ((!found) && (j < elements.length)) {
                                                if (elements[j].value == selection[i].value) {
                                                    found = true;
                                                }
                                                j++;
                                            }
                                            if (found) {
                                                if (ctrllist[ctrlcnt] == 'lbgroupavg') {
                                                    alert("Field [" + selection[i].text + "] must be removed from group average first!");
                                                } else if (ctrllist[ctrlcnt] == 'lbgroupcount') {
                                                    alert("Field [" + selection[i].text + "] must be removed from group count first!");
                                                } else if (ctrllist[ctrlcnt] == 'lbgrouptotal') {
                                                    alert("Field [" + selection[i].text + "] must be removed from group total first!");
                                                }
                                                return false;
                                            }
                                        }
                                    }
                                    return true;
                                }
                                break;
                        }
                        return true;
                    }
                    break;
            }
            alert("Validation rule is undefined");
            return false;
        }

        function btnAdd_Clicked(src, ctrlname, d) {
            var selection = [];
            if (src == "lbAllColumns") {
                $('[data-controlname=' + src + '] :selected').each(function (i, selected) {
                    selection[i] = selected;
                });
            } else {
                $('[data-controlname=' + src + '] option').each(function (i, selected) {
                    selection[i] = selected;
                });
            }

            if (d == null) { // d indicates it's triggered action so bypass validation to prevent going into loop.
                if (!Validate_Action(ctrlname, "add", selection)) return false;
            }

            var i = 0;
            var j = 0;
            var currentitem = null;
            var found = false;
            var optionstate = false;
            if (d != null) { optionstate = d; }
            var ctrl = $('[data-controlname=' + ctrlname + ']');
            var elements = $('[data-controlname=' + ctrlname + '] option');
            var greylength = 0;

            if ((ctrlname == "lbsorton") && (elements.length > 0)) {
                greylength = $('[data-controlname=' + ctrlname + '] :disabled').length;
            }

            var optlist = ctrl[0].options;
            for (i = 0; i < selection.length; i++) {
                currentitem = $(selection[i]);
                // check for duplicates
                found = false;
                for (j = 0; j < optlist.length; j++) {
                    if (optlist[j].value == currentitem.val()) {
                        if (d != null) {
                            $(optlist[j]).prop("disabled", d);    // touch the "disabled" state
                        }
                        found = true;
                    }
                }
                if (!found) {
                    if ((!d) || (elements.length < 1)) {
                        ctrl.append($("<option></option>").attr("value", currentitem.val()).text(currentitem.text())
                            .prop("data-datatype", currentitem.prop("data-datatype")).prop("disabled", optionstate));
                        if (d) { greylength++; }
                    } else {
                        if (greylength < 1) {
                            $('[data-controlname=' + ctrlname + '] option').eq(0).before($("<option></option>").attr("value", currentitem.val()).text(currentitem.text())
                            .prop("data-datatype", currentitem.prop("data-datatype")).prop("disabled", optionstate));
                            greylength++;
                        } else {
                            $('[data-controlname=' + ctrlname + '] :disabled:last').after($("<option></option>").attr("value", currentitem.val()).text(currentitem.text())
                                .prop("data-datatype", currentitem.prop("data-datatype")).prop("disabled", optionstate));
                        }
                    }
                }
            }

            // lbhiden
            if (ctrlname == 'lbhiden') {
                btnRemove_Clicked('lbsorton', true);
                btnAdd_Clicked('lbhiden', 'lbsorton', true);
            }

            return false;
        }

        function btnRemove_Clicked(ctrlname, d) {
            if (d == null) { // d indicates it's triggered action so bypass validation to prevent going into loop.
                if (!Validate_Action(ctrlname, "remove")) return false;
            }

            if (d) {
                $('[data-controlname=' + ctrlname + '] :disabled').each(function (i, selected) {
                    $(selected).remove();
                });
            } else {
                $('[data-controlname=' + ctrlname + '] :selected').each(function (i, selected) {
                    $(selected).remove();
                });
            }

            // lbhiden
            if (ctrlname == 'lbhiden') {
                btnRemove_Clicked('lbsorton', true);
                btnAdd_Clicked('lbhiden', 'lbsorton', true);
            }

            return false;
        }

        function btnReset_Clicked(ctrlname) {
            if (!Validate_Action(ctrlname, "reset")) return false;

            $('[data-controlname=' + ctrlname + '] option').each(function (i, selected) {
                $(selected).remove();
            });

            if (ctrlname == 'lbhiden') {
                btnRemove_Clicked('lbsorton', true);
            }
            return false;
        }

        function btnUp_Clicked(ctrlname) {
            $('[data-controlname=' + ctrlname + '] :selected').each(function (i, selected) {
                $(selected).insertBefore($(selected).prev());
            });
            return false;
        }

        function btnDown_Clicked(ctrlname) {
            $($('[data-controlname=' + ctrlname + '] :selected').get().reverse()).each(function (i, selected) {
                $(selected).insertAfter($(selected).next());
            });
            return false;
        }

        function unselectAll(ctrlname) {
            $('[data-controlname=' + ctrlname + '] :selected').each(function (i, selected) {
                $(selected).prop("selected", false);
            });
        }

        function lbFormula_selectionChanged() {
            var formulas = $('[data-controlname=lbFormula] option');
            var cnt = 0;
            var i = 0;
            for (i = 0; i < formulas.length; i++) {
                if (formulas[i].selected) {
                    $('#hid_formulaidx').val(i);
                    cnt++;
                }
            }
            // disable edit button if not exactly 1 button is pressed
            if (cnt != 1) {
                $('#btnFormulaEdit').addClass("disabled");
                $('#btnFormulaDelete').addClass("disabled");
            } else {
                $('#btnFormulaEdit').removeClass("disabled");
                $('#btnFormulaDelete').removeClass("disabled");
            }
        }

        function newFormula() {
            // set index to the next item
            var idx = $('[data-controlname=lbFormula] option').length;
            $('#hid_formulaidx').val(idx);
            return true;
        }

        function removeFormula() {
            if ($('[data-controlname=lbFormula] option:selected').length != 1) {
                alert("You can only remove one item at a time");
                return false;
            }
            var idx = -1;
            var i = 0;
            var opts = $('[data-controlname=lbFormula] option');
            for (i = 0; i < opts.length; i++) {
                if (opts[i].selected) {
                    idx = i;
                }
            }
            jQuery.ajax({
                url: "rpexcel.aspx?action=removeFormula",
                type: "POST",
                data: {
                    id: idx
                },
                success: function (data, textStatus, jqXHR) {
                    if (data.fields != null) {
                        $('[data-controlname=lbFormula] option').remove();
                        var ctrl = $('[data-controlname=lbFormula]');
                        var i = 0
                        for (i = 0; i < data.fields.length; i++) {
                            ctrl.append($("<option></option>").attr("value", data.fields[i]).text(data.fields[i]));
                        }
                    } else {
                        if (data.indexOf('DOCTYPE') > 0) {
                            alert("Session expired. Please login again.");
                        } else {
                            alert(data);
                        }
                    }
                }
            });
            return false;
        }
        function pageSubmission() {
            var payload = {};
            var optlist = null;
            var i = 0;

            payload.content = [];
            optlist = $('[data-controlname=lbcontents] option');
            for (i = 0; i < optlist.length; i++) {
                payload.content.push({
                    columnname: optlist[i].value
                });
            }

            jQuery.ajax({
                url: "rpexcel.aspx?action=pageSubmission",
                type: "POST",
                data: payload,
                error: function (jqXHR, textStatus, errorThrown) {
                    //only error handler is needed
                    alert(errorThrown);
                }
            });
            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class='page-header'>
        <h1 class='pull-left'>
            <span>Report Detail - Report Infomation</span>
        </h1>
        <div class='pull-right'>
            <div class='btn-group'>
                <asp:Button ID="btnNext" runat="server" Text="Next" OnClick="btnNext_Click" CssClass="btn btn-primary" ValidationGroup="sub" /></div>
            <div class='btn-group'>
                <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" CssClass="btn btn-primary" ValidationGroup="sub" Visible="false" /></div>
        </div>
    </div>
    <div class="error" style="color: Red;">
        <asp:Literal ID="lblErrorText" runat="server"></asp:Literal></div>
    <div class='row'>
        <div class='col-xs-12'>
            <div class="row">
                <div class="col-xs-4">
                    <div class="col-xs-4">
                        <label class="LabelStyle">
                            Report Name</label></div>
                    <div class="col-xs-8">
                        <asp:TextBox ID="txtReportName" runat="server" CssClass="form-control" MaxLength="50" Style='margin-bottom: 0;'></asp:TextBox>
                        <label class="label">
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtReportName" ErrorMessage="*" ValidationGroup="sub" ForeColor="Red"></asp:RequiredFieldValidator></label>
                    </div>
                </div>
                <div class="col-xs-4">
                    <div class="col-xs-4">
                        <label class="LabelStyle">
                            Report Title</label></div>
                    <div class="col-xs-8">
                        <asp:TextBox ID="txtReportTitle" runat="server" CssClass="char-max-length form-control inline" MaxLength="50" Style='margin-bottom: 0;'></asp:TextBox></div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-4">
                    <div class="col-xs-4">
                        <label class="LabelStyle">
                            Query Name</label></div>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlQueryName" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlQueryName_SelectedIndexChanged" CssClass="form-control inline">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="ddlQueryNameRequiredFieldValidator" runat="server" ErrorMessage="*" ControlToValidate="ddlQueryName" ForeColor="Red" ValidationGroup="SAVE"></asp:RequiredFieldValidator></div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-4">
                    <div class="col-xs-4">
                        <label class="LabelStyle">
                            Report Group</label></div>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlReportGroup" runat="server" CssClass="form-control inline">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-xs-4">
                    <div class="col-xs-4">
                        <label class="LabelStyle">
                            Category</label></div>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control inline">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-xs-4">
                    <div class="col-xs-4">
                        <label>
                            Report Type</label></div>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlShowType" runat="server" CssClass="form-control inline" AutoPostBack="true" OnSelectedIndexChanged="ddlShowType_SelectedIndexChanged">
                            <asp:ListItem Text="Show All" Value="0"></asp:ListItem>
                            <asp:ListItem Text="Show changed data only" Value="1"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-xs-4">
                    <div class="col-xs-4">
                        <label class="LabelStyle">
                            Default Format</label></div>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlFormat" runat="server" CssClass="form-control inline">
                            <asp:ListItem Text="Excel" Value="0"></asp:ListItem>
                            <asp:ListItem Text="On Screen" Value="1"></asp:ListItem>
                            <asp:ListItem Text="PDF" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div style="height: 20px">
        &nbsp;</div>
    <div class="row">
        <div class='col-md-4' style="min-width: 210px; max-width: 280px; padding-left: 5px;">
            <div class='faq'>
                <div class='tabbable'>
                    <ul class='nav nav-responsive nav-tabs'>
                        <li class='active'><a href="#tabAllColumns" data-toggle="tab">Columns</a></li>
                        <li><a href="#tabFormulas" data-toggle="tab">Fomulas</a></li>
                    </ul>
                    <div class='tab-content' style="margin: 0px; padding: 0px;">
                        <div class='tab-pane active' id="tabAllColumns">
                            <div class='panel panel-default'>
                                <div class='panel-heading' style="margin: 1px; padding: 1px;">
                                    <asp:ListBox ID="lbAllColumns" runat="server" SelectionMode="Multiple" CssClass="col-sm-12" Rows="50" data-controlname="lbAllColumns"></asp:ListBox>
                                </div>
                            </div>
                        </div>
                        <div class='tab-pane' id="tabFormulas">
                            <div class='panel panel-default'>
                                <div class='panel-heading center-block text-center' style="margin: 1px; padding: 1px;">
                                    <div class="btn-group center-block" style="margin-top: 15px; margin-bottom: 15px">
                                        <input type="button" id="btnFormulaNew" class="btn" value="New" data-toggle="modal" data-target="#dlgFormula" onclick="return newFormula();">
                                        <input type="button" id="btnFormulaEdit"  class="btn disabled" value="Edit" data-toggle="modal" data-target="#dlgFormula" />
                                        <input type="button" id="btnFormulaDelete"  class="btn disabled" value="Delete" onclick="return removeFormula();" />
                                    </div>
                                    <asp:ListBox ID="lbFormula" runat="server" SelectionMode="Multiple" CssClass="col-sm-12" Rows="46" data-controlname="lbFormula"></asp:ListBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-md-8" style="padding-left: 5px; margin-right: 0px; padding-right: 0px;">
            <div class="row">
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnContentInsert" runat="server" Text="add to contents" UseSubmitBehavior="false" CssClass="btn" Width="150px" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbcontents')" /></div>
                    <div>
                        <asp:ListBox ID="lbcontents" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbcontents"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnContentUp" runat="server" Text="Up" CssClass="btn" OnClientClick="return btnUp_Clicked('lbcontents')" />
                        <asp:Button ID="btnContentDown" runat="server" Text="Down" CssClass="btn " OnClientClick="return btnDown_Clicked('lbcontents')" />
                        <asp:Button ID="btnContentDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbcontents')" />
                    </div>
                </div>
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnCriteriaInsert" runat="server" Text="add to criterias" CssClass="btn " Width="150px" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbcriteria')" /></div>
                    <div>
                        <asp:ListBox ID="lbcriteria" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbcriteria"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnCriteriaDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbcriteria')" /></div>
                </div>
            </div>
            <div class="row">
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnSortonInsert" runat="server" Text="add to sort on" CssClass="btn " Width="150px" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbsorton')" /></div>
                    <div>
                        <asp:ListBox ID="lbsorton" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbsorton"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnSortonUp" runat="server" Text="Up" CssClass="btn " OnClientClick="return btnUp_Clicked('lbsorton')" />
                        <asp:Button ID="btnSortonDown" runat="server" Text="Down" CssClass="btn " OnClientClick="return btnDown_Clicked('lbsorton')" />
                        <asp:Button ID="btnSortonDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbsorton')" /></div>
                </div>
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnGroupByInsert" runat="server" Text="add to group by" CssClass="btn " Width="150px" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbhiden')" /></div>
                    <div>
                        <asp:ListBox ID="lbhiden" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbhiden"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnGroupbyDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbhiden')" />
                        <asp:Button ID="btnGroupbyReset" runat="server" Text="Reset" CssClass="btn " OnClientClick="return btnReset_Clicked('lbhiden')" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnSumInsert" runat="server" Text="add to report total" CssClass="btn " Width="150px" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbsum')" /></div>
                    <div>
                        <asp:ListBox ID="lbsum" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbsum"></asp:ListBox>
                    </div>
                    <div>
                        <asp:Button ID="btnSumDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbsum')" /></div>
                </div>
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnGrouptotalInsert" runat="server" Text="add to group total" CssClass="btn " Width="150px" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbgrouptotal')" /></div>
                    <div>
                        <asp:ListBox ID="lbgrouptotal" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbgrouptotal"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnGrouptotalDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbgrouptotal')" /></div>
                </div>
            </div>
            <div class="row">
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnAvgInsert" runat="server" Text="add to report avg" CssClass="btn " Width="150px" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbavg')" /></div>
                    <div>
                        <asp:ListBox ID="lbavg" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbavg"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnAvgDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbavg')" /></div>
                </div>
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnGroupavgInsert" runat="server" Text="add to group avg" CssClass="btn " Width="150px" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbgroupavg')" /></div>
                    <div>
                        <asp:ListBox ID="lbgroupavg" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbgroupavg"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnGroupavgDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbgroupavg')" /></div>
                </div>
            </div>
            <div class="row">
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnRpcountInsert" runat="server" Text="add to report count" CssClass="btn " Width="150px" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbrpcount')" /></div>
                    <div>
                        <asp:ListBox ID="lbrpcount" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbrpcount"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnRpcountDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbrpcount')" /></div>
                </div>
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnGroupcountInsert" runat="server" Text="add to group count" CssClass="btn " Width="150px" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbgroupcount')" /></div>
                    <div>
                        <asp:ListBox ID="lbgroupcount" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbgroupcount"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnGroupcountDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbgroupcount')" /></div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            $('[data-toggle=tab]').click(function () {
                if ($(this).parent().hasClass('active')) {
                    $($(this).attr("href")).toggleClass('active');
                }
            })
            $('[data-controlname=lbFormula]').change(function () {
                lbFormula_selectionChanged();
            });
        });
    </script>
</asp:Content>
<asp:Content ID="contentNonFormItems" ContentPlaceHolderID="cp_nonFormControls" runat="server" ViewStateMode="Disabled">
    <script type="text/javascript">
        function paramForumulaNameChanged() {
            id = $('#hid_formulaidx').val();
            fieldname = $('#txtFormulaName').val();
            jQuery.ajax({
                url: "rpexcel.aspx?action=updateFieldName",
                type: "POST",
                data: {
                    id: id,
                    fieldname: fieldname
                },
                success: function (data, textStatus, jqXHR) {
                    if (data.Idx != null) {
                        $('#hid_formulaidx').val(data.Idx);
                        $('#txtFormulaName').val(data.FieldName);
                        $('#div_formula').html(data.RenderText);
                        $('#preSQLText').text(data.SQLText);
                        regHighlightPair();
                    } else {
                        if (data.indexOf('DOCTYPE') > 0) {
                            alert("Session expired. Please login again.");
                        } else {
                            alert(data);
                        }
                    }
                }
            });
            return false;
        }
        function paramAdd(p) {
            // p = parent
            id = $('#hid_formulaidx').val();
            jQuery.ajax({
                url: "rpexcel.aspx?action=paramAdd",
                type: "POST",
                data: {
                    id: id,
                    ParamId: p
                },
                success: function (data, textStatus, jqXHR) {
                    if (data.Idx != null) {
                        $('#hid_formulaidx').val(data.Idx);
                        $('#txtFormulaName').val(data.FieldName);
                        $('#div_formula').html(data.RenderText);
                        $('#preSQLText').text(data.SQLText);
                        regHighlightPair();
                    } else {
                        if (data.indexOf('DOCTYPE') > 0) {
                            alert("Session expired. Please login again.");
                        } else {
                            alert(data);
                        }
                    }
                }
            });
            return false;
        }
        function paramFuncChanged(p) {
            id = $('#hid_formulaidx').val();
            newFunc = $('#ddlFuncName_' + p).val();
            jQuery.ajax({
                url: "rpexcel.aspx?action=paramFuncChg",
                type: "POST",
                data: {
                    id: id,
                    ParamId: p,
                    newFunc: newFunc
                },
                success: function (data, textStatus, jqXHR) {
                    if (data.Idx != null) {
                        $('#hid_formulaidx').val(data.Idx);
                        $('#txtFormulaName').val(data.FieldName);
                        $('#div_formula').html(data.RenderText);
                        $('#preSQLText').text(data.SQLText);
                        regHighlightPair();
                    } else {
                        if (data.indexOf('DOCTYPE') > 0) {
                            alert("Session expired. Please login again.");
                        } else {
                            alert(data);
                        }
                    }
                }
            });
            return false;
        }
        function paramTextChanged(p) {
            id = $('#hid_formulaidx').val();
            newValue = $('#txtCellText_' + p).val();
            jQuery.ajax({
                url: "rpexcel.aspx?action=paramTextChg",
                type: "POST",
                data: {
                    id: id,
                    ParamId: p,
                    newValue: newValue
                },
                success: function (data, textStatus, jqXHR) {
                    if (data.Idx != null) {
                        $('#hid_formulaidx').val(data.Idx);
                        $('#txtFormulaName').val(data.FieldName);
                        $('#div_formula').html(data.RenderText);
                        $('#preSQLText').text(data.SQLText);
                        regHighlightPair();
                    } else {
                        if (data.indexOf('DOCTYPE') > 0) {
                            alert("Session expired. Please login again.");
                        } else {
                            alert(data);
                        }
                    }
                }
            });
            return false;
        }
        function checkDelParam(e, p) {
            if (e.keyCode == 46) {
                id = $('#hid_formulaidx').val();
                jQuery.ajax({
                    url: "rpexcel.aspx?action=paramRemove",
                    type: "POST",
                    data: {
                        id: id,
                        ParamId: p
                    },
                    success: function (data, textStatus, jqXHR) {
                        if (data.Idx != null) {
                            $('#hid_formulaidx').val(data.Idx);
                            $('#txtFormulaName').val(data.FieldName);
                            $('#div_formula').html(data.RenderText);
                            $('#preSQLText').text(data.SQLText);
                            regHighlightPair();
                        } else {
                            if (data.indexOf('DOCTYPE') > 0) {
                                alert("Session expired. Please login again.");
                            } else {
                                alert(data);
                            }
                        }
                    }
                });
            }
            return false;
        }
        function paramTextToFormula(p) {
            id = $('#hid_formulaidx').val();
            jQuery.ajax({
                url: "rpexcel.aspx?action=paramToFormula",
                type: "POST",
                data: {
                    id: id,
                    ParamId: p
                },
                success: function (data, textStatus, jqXHR) {
                    if (data.Idx != null) {
                        $('#hid_formulaidx').val(data.Idx);
                        $('#txtFormulaName').val(data.FieldName);
                        $('#div_formula').html(data.RenderText);
                        $('#preSQLText').text(data.SQLText);
                        regHighlightPair();
                    } else {
                        if (data.indexOf('DOCTYPE') > 0) {
                            alert("Session expired. Please login again.");
                        } else {
                            alert(data);
                        }
                    }
                }
            });
            return false;
        }
        function refreshdlg(paramid) {
            var id = paramid;
            if (id == null) { id = $('#hid_formulaidx').val(); }
            jQuery.ajax({
                url: "rpexcel.aspx?action=reloadFormulaDlg",
                type: "POST",
                data: {
                    id: id
                },
                success: function (data, textStatus, jqXHR) {
                    if (data.Idx != null) {
                        $('#hid_formulaidx').val(data.Idx);
                        $('#txtFormulaName').val(data.FieldName);
                        $('#div_formula').html(data.RenderText);
                        $('#preSQLText').text(data.SQLText);
                        regHighlightPair();
                    } else {
                        if (data.indexOf('DOCTYPE') > 0) {
                            alert("Session expired. Please login again.");
                        } else {
                            alert(data);
                        }
                    }
                }
            });
            return false;
        }
        function formulaSaveChange() {
            var id = $('#hid_formulaidx').val();
            jQuery.ajax({
                url: "rpexcel.aspx?action=formulaSaveChange",
                type: "POST",
                data: {
                    id: id
                },
                success: function (data, textStatus, jqXHR) {
                    if (data.fields != null) {
                        $('[data-controlname=lbFormula] option').remove();
                        var ctrl = $('[data-controlname=lbFormula]');
                        var i = 0
                        for (i = 0; i < data.fields.length; i++) {
                            ctrl.append($("<option></option>").attr("value", data.fields[i]).text(data.fields[i]));
                        }
                    } else {
                        if (data.indexOf('DOCTYPE') > 0) {
                            alert("Session expired. Please login again.");
                        } else {
                            alert(data);
                        }
                    }
                }
            });
            return false;
        }
        function regHighlightPair() {
            $('[data-formulactrl=true]').hover(function () {
                var id = $(this).attr('data-formulaid');
                $('[data-formulaid=' + id + ']').css("background-color", "yellow");
            }, function () {
                $('[data-formulactrl=true]').css("background-color", "");
            });
        }
    </script>
    <div class="modal fade" id="dlgFormula" tabindex="-1" role="dialog" aria-labelledby="dlgCaption" aria-hidden="true" style="margin-top: 50px; overflow: hidden">
        <div class="modal-dialog" style="width: 80%">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title" id="dlgCaption">Edit Formula</h4>
                </div>
                <div class="modal-body">
                    <div class='row' style="margin-bottom: 15px">
                        <div class="col-xs-2"><label class="col-md-1 control-label">Description</label></div>
	                    <div class="col-xs-10"><input type="text" id="txtFormulaName" class="form-control" onchange="return paramForumulaNameChanged();" /><input type="hidden" id="hid_formulaidx" value="0" /></div>
                    </div>
                    <div class='row' style="margin-bottom: 15px">
                        <div class="col-xs-2"><label class="col-md-1 control-label">Formula</label></div>
	                    <div class="col-xs-10" id="div_formula" style="max-height: 500px; overflow: auto"></div>
                    </div>
                    <div class='row'>
                        <div class="col-xs-2"><label class="col-md-12 control-label">Resulting Text</label></div>
                        <div class="col-xs-10"><pre id="preSQLText" style="height: 60px; overflow: auto"></pre></div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn pull-left" onclick="return refreshdlg();">Refresh</button>
                    <button type="button" class="btn btn-primary" data-dismiss="modal" onclick="formulaSaveChange();">
                        Save and Close</button>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#dlgFormula').on('show.bs.modal', function () {
                refreshdlg();
            });
        });
    </script>
</asp:Content>
