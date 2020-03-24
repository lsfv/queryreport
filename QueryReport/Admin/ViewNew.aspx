<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.master" AutoEventWireup="true" CodeBehind="ViewNew.aspx.cs" Inherits="QueryReport.Admin.ViewNew" %>
<asp:Content ID="Header1" ContentPlaceHolderID="header" runat="server">
    <style type="text/css">
        TABLE[data-ctrlname=gvFields] { margin-bottom:0; }
    </style>
    <script type="text/javascript">
        function saveColData()
        {
            var result = false;
            var tbl = jQuery("TABLE[data-ctrlname=gvFields]");
            var i, tr;
            var payload = {};
            payload.items = new Array();
            for (i = 0; i < tbl.dataTableSettings[0].aoData.length; i++) {
                tr = jQuery(tbl.dataTableSettings[0].aoData[i].nTr);
                payload.items[i] = new Object();
                payload.items[i].COLUMNNAME = tr.attr("data-colname");
                payload.items[i].DISPLAYNAME = tr[0].children[1].children[0].value;
                payload.items[i].HIDDEN = !(tr[0].children[2].children[0].checked);
            }
            var targetUrl = window.location.href;
            if (targetUrl.indexOf('?') < 0) {
                targetUrl = targetUrl + "?action=savedata";
            } else {
                targetUrl = targetUrl + "&action=savedata";
            }
            jQuery.ajax({
                url: targetUrl,
                type: 'POST',
                async: false,
                data: {
                    __ASYNCPOST: true,
                    payload: JSON.stringify(payload)
                },
                success: function (data, textStatus, jqXHR) {
                    if (data.length == 0) {
                        result = true;
                    } else {
                        alert(data);
                    }
                },
                fail: function (jqXHR, textStatus) {
                    //HideLoadingMessage(this);
                    alert(textStatus);
                }
            });
            return result;
        }
        function setgvFieldsWidth(e, settings) {
            var rowwith = jQuery("TABLE[data-ctrlname=gvFields]>THEAD>TR").width();
            var ths = jQuery("TABLE[data-ctrlname=gvFields]>THEAD>TR>TH");
            jQuery(ths[0]).css("width", (rowwith - 75) / 2);
            jQuery(ths[1]).css("width", (rowwith - 75) / 2);
            jQuery(ths[2]).css("width", 75);
        }
        jQuery(document).ready(function () {
            jQuery("TABLE[data-ctrlname=gvFields]").on("draw.dt", setgvFieldsWidth);
        });
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class='page-header'>
        <h1 class='pull-left'>
            <span>Administation - Query Maintenance</span>
        </h1>
        <div class='pull-right'>
            <div class='btn-group'>
                <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-primary" Visible="false" OnClick="btnDelete_Click" />
            </div>
            <div class='btn-group'>
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClientClick="return saveColData();" OnClick="btnSave_Click" ValidationGroup="SAVE" />
            </div>
        </div>
    </div>
    <div class='row'>
        <label class="col-md-1 control-label">
            Query Name</label><div class="col-xs-3">
                <asp:TextBox ID="txtuid" runat="server" CssClass=" form-control" data-ctrlname="txtuid"></asp:TextBox></div>
        <label class="label">
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="* required field" ControlToValidate="txtuid" ForeColor="Red" ValidationGroup="SAVE"></asp:RequiredFieldValidator></label>
    </div>
    <br />
    <div class='row'>
        <label class="col-md-1 control-label">
            Description</label><div class="col-xs-3">
                <asp:TextBox ID="txtp1" runat="server" CssClass=" form-control" data-ctrlname="txtp1"></asp:TextBox></div>
    </div>
    <br />
        <div class='row'>
        <label class="col-md-1 control-label">
            View name</label><div class="col-xs-3">
                <asp:DropDownList ID="ddlTblViewName" runat="server" CssClass=" form-control" AutoPostBack="true" data-ctrlname="ddlTblViewName" OnSelectedIndexChanged="ddlTblViewName_SelectedIndexChanged"></asp:DropDownList></div>
    </div>
    <br />
    <div class="row">
        <label class="col-md-1 control-label">
            Query Security Level</label>
        <div class="col-xs-3">
            <asp:DropDownList ID="ddlLevel" runat="server" CssClass=" form-control" data-ctrlname="ddlLevel">
            </asp:DropDownList>
        </div>
    </div>
    <br />
    <div class="row">
        <label class="col-md-1 control-label">
            Apply to</label>
        <div class="col-xs-3 MR8">
            <asp:CheckBox ID="cbExcel" runat="server" Text="Excel" data-ctrlname="cbExcel" OnCheckedChanged="excelCheckChanged" AutoPostBack="true" /><asp:CheckBox ID="cbWord" runat="server" Text="WinWord" data-ctrlname="cbWord" OnCheckedChanged="wordCheckChanged" AutoPostBack="true" /></div>
    </div>
    <br />
    <div class="row">
        <label class="col-md-1 control-label">Field Selection:</label>
        <div class="col-xs-8">
            <div class='responsive-table'>
                <div class='scrollable-area'>
                    <asp:Table ID="gvFields" runat="server" CssClass='data-table-column-filter table table table-bordered-th table-striped' data-ctrlname="gvFields">
                    </asp:Table>
                </div>
            </div>
        </div>
    </div>
    <asp:Label ID="lblJavascript" runat="server" Text=""></asp:Label>
</asp:Content>
