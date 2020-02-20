<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.master" AutoEventWireup="true" CodeBehind="rpworddetail.aspx.cs" Inherits="QueryReport.rpworddetail" %>
<asp:Content ID="header" ContentPlaceHolderID="header" runat="server">
    <style type="text/css">
        LABEL
        {
            margin-right: 5px;
        }
    </style>
    <script type="text/javascript">
        function txtAllColumnsFilter_invoked(e) {
            var target = $("#txtAllColumnsFilter").val().toUpperCase();
            window.status = target;
            $("[data-controlname=lbAllColumns] option").remove();
            $("[data-controlname=lbAllColumns_master] option").each(function (i, selected) {
                var opt = $(selected);
                if (opt.text().toUpperCase().indexOf(target) >= 0) {
                    $("[data-controlname=lbAllColumns]").append(opt.clone());
                }
            });
            return false;
        }

        function btnAdd_Clicked(src, ctrlname, d) {
            var selection = [];
            if ((src == "lbAllColumns") || (src == "lbFormula")) {
                $('[data-controlname=' + src + '] :selected').each(function (i, selected) {
                    selection[i] = selected;
                });
            } else {
                $('[data-controlname=' + src + '] option').each(function (i, selected) {
                    selection[i] = selected;
                });
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

            return false;
        }

        function btnRemove_Clicked(ctrlname, d) {
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

        function btnUp_Clicked(ctrlname) {
            $('[data-controlname=' + ctrlname + '] :selected').each(function (i, selected) {
                var opt_prev = $(selected).prev();
                if (opt_prev.attr("disabled") == null) {
                    $(selected).insertBefore(opt_prev);
                }
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
            if (ctrlname == null) {
                $('SELECT[multiple] :selected').each(function (i, selected) {
                    $(selected).prop("selected", false);
                });
            } else {
                $('[data-controlname=' + ctrlname + '] :selected').each(function (i, selected) {
                    $(selected).prop("selected", false);
                });
            }
            return true;
        }

        function pageSubmission() {
            var payload = {};
            var optlist = null;
            var i = 0;

            payload.ReportName = $("[data-controlname=txtReportName]").val();
            payload.ReportTitle = $("[data-controlname=txtReportTitle]").val();
            payload.SVID = $("[data-controlname=ddlQueryName]").val();
            payload.ReportGroupID = $("[data-controlname=ddlReportGroup]").val();
            payload.CategoryID = $("[data-controlname=ddlCategory]").val();
            payload.ReportType = "0";
            payload.fHideCriteria = false;
            payload.fHideDuplicate = $("[data-controlname=chkHideDuplicate] INPUT[type=checkbox]")[0].checked;
            payload.Format = $("[data-controlname=ddlFormat]").val();
            payload.ReportHeader = "";
            payload.ReportFooter = "";

            payload.contentColumn = [];
            optlist = $('[data-controlname=lbAllColumns] option');
            for (i = 0; i < optlist.length; i++) {
                payload.contentColumn.push({
                    ColumnName: optlist[i].value,
                    DisplayName: optlist[i].text
                });
            }

            payload.criteriaColumn = [];
            optlist = $('[data-controlname=lbcriteria] option');
            for (i = 0; i < optlist.length; i++) {
                payload.criteriaColumn.push({
                    ColumnName: optlist[i].value,
                    DisplayName: optlist[i].text
                });
            }

            payload.sortonColumn = [];
            optlist = $('[data-controlname=lbsorton] option');
            for (i = 0; i < optlist.length; i++) {
                payload.sortonColumn.push({
                    ColumnName: optlist[i].value,
                    DisplayName: optlist[i].text
                });
            }

            jQuery.ajax({
                url: "rpworddetail.aspx?action=pageSubmission",
                type: "POST",
                data: { payload: JSON.stringify(payload) },
                async: false,
                success: function (data, textStatus, jqXHR) {
                    if (data == "OK") {
                        unselectAll();
                        $('[data-controlname=btnNext]').click();
                    } else if (data.indexOf('DOCTYPE') > 0) {
                        alert("Session expired. Please login again.");
                    } else {
                        alert(data);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    //only error handler is needed
                    alert(errorThrown);
                }
            });
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class='page-header'>
        <h1 class='pull-left'>
            <span>Report Detail - Base Information</span><input type="button" id="btnNext" runat="server" onserverclick="btnNext_Click" data-controlname="btnNext" style="display: none" />
        </h1>
        <div class='pull-right'>
            <div class='btn-group'>
                <input type="button" id="btnSubmit" value="Save and Continue" class="btn btn-primary" onclick="return pageSubmission();" />
            </div>
            <div class='btn-group'>
                <input type="button" id="btnShowDeleteConfirm" value="Delete" class="btn btn-primary" data-toggle="modal" data-target="#delconfirm" />
            </div>
        </div>
    </div>
    <div class="error" style="color: Red;">
        <asp:Literal ID="lblErrorText" runat="server"></asp:Literal></div>
    <div class='row'>
        <div class='col-xs-12'>
            <div class="row">
                <div class="col-xs-4">
                    <div class="col-xs-4">
                        <label>Report Name</label>
                    </div>
                    <div class="col-xs-8">
                        <asp:TextBox ID="txtReportName" runat="server" CssClass="form-control" data-controlname="txtReportName" MaxLength="50" Style='margin-bottom: 0;'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtReportName" ErrorMessage="*" ValidationGroup="sub" ForeColor="Red"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="col-xs-4" style="display: none">
                    <div class="col-xs-4">
                        <label>Report Title</label>
                    </div>
                    <div class="col-xs-8">
                        <asp:TextBox ID="txtReportTitle" runat="server" CssClass="char-max-length form-control inline" MaxLength="50" Style='margin-bottom: 0;'></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-4">
                    <div class="col-xs-4">
                        <label>Template Name</label>
                    </div>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlQueryName" runat="server" data-controlname="ddlQueryName" CssClass="form-control inline" Enabled="false">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" ControlToValidate="ddlQueryName" ForeColor="Red" ValidationGroup="SAVE"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="col-xs-4">
                    <div class="col-xs-4">
                        <label class="LabelStyle">Hide Duplicate</label>
                    </div>
                    <div class="col-xs-2">
                        <asp:CheckBox ID="chkHideDuplicate" runat="server" data-controlname="chkHideDuplicate" Checked="true" />
                    </div>
                    <div class="col-xs-6">
                        <input type="button" id="btnTemplateManagement" value="Template Management" class="btn btn-primary" data-toggle="modal" data-target="#templatemgt" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-4">
                    <div class="col-xs-4">
                        <label>Report Group</label></div>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlReportGroup" runat="server" data-controlname="ddlReportGroup" CssClass="form-control inline">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-xs-4">
                    <div class="col-xs-4">
                        <label>Category</label>
                    </div>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlCategory" runat="server" data-controlname="ddlCategory" CssClass="form-control inline">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-xs-4">
                    <div class="col-xs-4">
                        <label>Default Format</label></div>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlFormat" runat="server" data-controlname="ddlFormat" CssClass="form-control inline">
                            <asp:ListItem Text="Word" Value="0"></asp:ListItem>
                            <asp:ListItem Text="On Screen" Value="1"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <br />
        </div>
    </div>
    <div style="height: 20px">
        &nbsp;</div>
    <div class="row">
        <div class='col-md-4' style="min-width: 210px; max-width: 280px; padding-left: 5px;">
            <div class='faq'>
                <div class='tabbable'>
                    <ul class='nav nav-responsive nav-tabs'>
                        <li class='active'><a data-toggle='tab' href='#faq1'>Select Column</a></li></ul>
                    <div class='tab-content' style="margin: 0px; padding: 0px;">
                        <div class='tab-pane active'>
                            <div class='panel panel-default'>
                                <div class='panel-heading' style="margin: 1px; padding: 1px;">
                                    <label for="txtAllColumnsFilter" style="height: 30px; width: 30%; float: left; margin-top: 5px; margin-left: 5px">Search:</label><input type="text" id="txtAllColumnsFilter" class="form-control" style="width: 65%; float: right" autocomplete="off" placeholder="Search..." />
                                    <asp:ListBox ID="lbAllColumns_master" runat="server" SelectionMode="Multiple" CssClass="col-sm-12" Rows="30" style="display: none" data-controlname="lbAllColumns_master"></asp:ListBox>
                                    <asp:ListBox ID="lbAllColumns" runat="server" data-controlname="lbAllColumns" Rows="10" SelectionMode="Multiple" CssClass="col-sm-12"></asp:ListBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-md-8" style="padding-left: 5px; margin-right: 0px; padding-right: 0px;">
            <div class="row">
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px; display: none">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnContentInsert" runat="server" Text="add to contents" UseSubmitBehavior="false" CssClass="btn" Width="150px" data-controlname="btnContentInsert" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbcontents') || btnAdd_Clicked('lbFormula', 'lbcontents')" /></div>
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
                        <asp:Button ID="btnCriteriaInsert" runat="server" Text="add to criterias" CssClass="btn " Width="150px" data-controlname="btnCriteriaInsert" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbcriteria')" /></div>
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
                        <asp:Button ID="btnSortonInsert" runat="server" Text="add to sort on" CssClass="btn " Width="150px" data-controlname="btnSortonInsert" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbsorton') || btnAdd_Clicked('lbFormula', 'lbsorton')" /></div>
                    <div>
                        <asp:ListBox ID="lbsorton" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbsorton"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnSortonUp" runat="server" Text="Up" CssClass="btn " OnClientClick="return btnUp_Clicked('lbsorton')" />
                        <asp:Button ID="btnSortonDown" runat="server" Text="Down" CssClass="btn " OnClientClick="return btnDown_Clicked('lbsorton')" />
                        <asp:Button ID="btnSortonDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbsorton')" /></div>
                </div>
            </div>
        </div>
    </div>
    <div id="delconfirm" class="modal fade" role="dialog" aria-hidden="true" style="margin-top: 50px" >
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-body">
                    Are you sure to delete?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" CssClass="btn btn-primary" ValidationGroup="sub" Visible="false" />
                    <button type="button" data-dismiss="modal" class="btn">Cancel</button>
                </div>
            </div>
        </div>
    </div>
    <div id="templatemgt" class="modal fade" role="dialog" aria-hidden="true" style="margin-top: 50px" >
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Template Management</h4>
                </div>
                <div class="modal-body">
                    <div class="row" style="margin-bottom: 20px;">
                        <div class="col-xs-2">
                            <label>Download</label>
                        </div>
                        <div class="col-xs-10">
                            <button id="btnDownloadTemplate" runat="server" onserverclick="btnDownloadTemplate_Click"><span style="white-space: nowrap">Download Template</span></button>
                            <button id="btnDownloadDatafile" runat="server" onserverclick="btnDownloadDatafile_Click"><span style="white-space: nowrap">Download Datafile</span></button>
                        </div>
                    </div>
                    <div class="row" style="margin-bottom: 20px;">
                        <div class="col-xs-2">
                            <label>Upload</label>
                        </div>
                        <div class="col-xs-10">
                            <input type="file" id="fUploadTemplate" runat="server" style="width: 70%; display: inline" />
                            <button id="btnUploadTemplate" runat="server" onserverclick="btnUploadTemplate_Click"><span style="white-space: nowrap">Upload Template</span></button>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" data-dismiss="modal" class="btn">Close</button>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            $("#txtAllColumnsFilter").keyup(txtAllColumnsFilter_invoked);
        });
    </script>
    <asp:Literal ID="lblJavascript" runat="server" />
</asp:Content>
