<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.master" AutoEventWireup="true" CodeBehind="DataTransfer.aspx.cs" Inherits="QueryReport.Admin.DataTransfer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class='page-header'>
    <h1 class='pull-left'>
        <span>Administration - Copy Security</span>
    </h1>
    <div class='pull-right'>
        <div class='btn-group'>
            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary"  onclick="btnSave_Click" ValidationGroup="SAVE"  OnClientClick="return confirm('All data in destination will be cleared before import.Are you sure?')" />
        </div>
    </div>
</div>
<div class='row'>
     <label class="col-xs-2">Copy to database</label>
     <div class="col-xs-8"><asp:DropDownList ID="DDLDESTINATION" runat="server"></asp:DropDownList></div>
</div>
<br />
<div class='row'>
     <label class="col-xs-2">Report Group</label>
     <div class="col-xs-8"><asp:CheckBoxList ID="CBLReportGroup" runat="server" RepeatDirection="Horizontal" Enabled="false" CellPadding="4"></asp:CheckBoxList></div>
</div>
<br />
<div class='row'>
    <label class="col-xs-2">Report Category</label>
    <div class="col-xs-8">
        <asp:CheckBoxList ID="CBLCATEGORY" runat="server" Enabled="False" RepeatDirection="Horizontal" CellPadding="4" RepeatColumns="5"></asp:CheckBoxList></div>
</div>
<br />
<div class='row'>
    <label class="col-xs-2">Report Security Level</label>
    <div class="col-xs-8">
        <asp:CheckBoxList ID="CBLVIEWLEVEL" runat="server" Enabled="False" RepeatDirection="Horizontal" CellPadding="4" RepeatColumns="5"></asp:CheckBoxList></div>
</div>
<br />
<div class='row'>
    <label class="col-xs-2">Query</label>
    <div class="col-xs-8"><asp:CheckBoxList ID="CBLQUERY" runat="server" RepeatDirection="Horizontal" CellPadding="4" RepeatColumns="3"></asp:CheckBoxList></div>
</div>
<br />
<div class='row'>
    <label class="col-xs-2">Excel Report</label>
    <div class="col-xs-8"><asp:CheckBoxList ID="CBLREPORT" runat="server" BorderWidth="0px" RepeatColumns="3" RepeatDirection="Horizontal" CellPadding="4"></asp:CheckBoxList></div>
</div>
<br />
<div class='row'>
    <label class="col-xs-2">Word Report</label>
    <div class="col-xs-8"><asp:CheckBoxList ID="CBLREPORT2" runat="server" BorderWidth="0px" RepeatColumns="3" RepeatDirection="Horizontal" CellPadding="4"></asp:CheckBoxList></div>
</div>
<br />
<div class='row'>
    <label class="col-xs-2">User Group</label>
    <div class="col-xs-8"><asp:CheckBoxList ID="CBLUSERGROUP" runat="server" Enabled="false" RepeatDirection="Horizontal" CellPadding="4" RepeatColumns="5"></asp:CheckBoxList></div>
</div>
<br />
<div class='row'>
    <label class="col-xs-2">User</label>
    <div class="col-xs-8"><asp:CheckBoxList ID="CBLUSER" runat="server" RepeatDirection="Horizontal" CellPadding="4" RepeatColumns="5"></asp:CheckBoxList></div>
</div>
<br />
</asp:Content>