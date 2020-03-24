<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.Master" AutoEventWireup="true" CodeBehind="ViewNew2.aspx.cs" Inherits="QueryReport.Admin.ViewNew2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
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
        <label class="col-xs-2" style="max-width:150px">Query Name</label>
        <div class="col-xs-5" style="max-width:300px"><asp:TextBox ID="txtuid" runat="server" CssClass=" form-control" ></asp:TextBox></div>
        <label class="col-xs-5"><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="* required field" ControlToValidate="txtuid" ForeColor="Red" ValidationGroup="SAVE"></asp:RequiredFieldValidator></label>
    </div>
    <br />
    <div class='row'>
        <label class="col-xs-2" style="max-width:150px">Description</label>
        <div class="col-xs-5" style="max-width:300px"><asp:TextBox ID="txtp1" runat="server" CssClass=" form-control" ></asp:TextBox></div>
    </div>
    <br />
    <div class='row'>
        <label class="col-xs-2" style="max-width:150px">View name</label>
        <div class="col-xs-5" style="max-width:300px"><asp:DropDownList ID="ddlTblViewName" runat="server" CssClass=" form-control" AutoPostBack="true"  OnSelectedIndexChanged="ddlTblViewName_SelectedIndexChanged"></asp:DropDownList></div>
    </div>
    <br />
    <div class="row">
        <label class="col-xs-2" style="max-width:150px">Query Security Level</label>
        <div class="col-xs-5" style="max-width:300px"><asp:DropDownList ID="ddlLevel" runat="server" CssClass=" form-control" ></asp:DropDownList></div>
    </div>
    <br />
    <div class="row">
        <label class="col-xs-2" style="max-width:150px">Apply to</label>
        <div class="col-xs-5" style="max-width:300px"><asp:CheckBox ID="cbExcel" runat="server" Text="Excel"  OnCheckedChanged="excelCheckChanged" AutoPostBack="true" Checked="true" /><asp:CheckBox ID="cbWord" runat="server" Text="WinWord"  OnCheckedChanged="wordCheckChanged" AutoPostBack="true" /></div>
    </div>
    <br />
    <asp:Label ID="lblJavascript" runat="server" Text=""></asp:Label>
</asp:Content>