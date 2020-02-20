<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.master" AutoEventWireup="true" CodeBehind="WordTemplateNew.aspx.cs" Inherits="QueryReport.Admin.WordTemplateNew" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class='page-header'>
    <h1 class='pull-left'>
        <span>Administration - Word Template Detail</span>
    </h1>
    <div class='pull-right'>
        <div class='btn-group'>
            <asp:Button ID="Button2" runat="server" Text="Delete" 
                CssClass="btn btn-primary"  onclick="Delete" />
        </div>&nbsp;&nbsp;&nbsp;
        <div class='btn-group'>
            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary"  onclick="btnSave_Click" ValidationGroup="SAVE" />
        </div>
         <div class='btn-group'>
            <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn btn-primary"  onclick="btnBack_Click" ValidationGroup="BACK" />
        </div>
    </div>
</div>
<div class='row' runat="server" visible="false">
    <label class="col-md-1 control-label">Query type</label>
    <div class="col-xs-3">
        <asp:DropDownList ID="ddlQueryType" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlQueryType_SelectedIndexChanged" AutoPostBack="true">
        </asp:DropDownList>
    </div>
</div>
<br/>
<div class="row">
    <label class="col-md-1 control-label">Base query</label>
    <div class="col-xs-8">
        <asp:DropDownList ID="ddlTemplate" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTemplate_SelectedIndexChanged" CssClass="form-control inline" Width="600"></asp:DropDownList>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*" ControlToValidate="ddlTemplate" ForeColor="Red" 
                ValidationGroup="SAVE"></asp:RequiredFieldValidator></div></div>
<br/>
<div class="row">
    <label class="col-md-1 control-label">Query Name</label>
    <div class="col-xs-4" >
        <asp:TextBox ID="txtQueryName" runat="server" CssClass="form-control" Enabled="false" Width="600"></asp:TextBox></div></div>
<br/>
<div class="row">
    <label class="col-md-1 control-label">Query Level</label>
    <div class="col-xs-4" >
        <asp:TextBox ID="txtQueryLevel" runat="server" CssClass="form-control" Enabled="false" Width="600"></asp:TextBox></div></div>
<br/>   
<div class="row">
    <label class="col-md-1 control-label">Template Description</label>
    <div class="col-xs-4" >
        <asp:TextBox ID="txtDesc" runat="server" CssClass="form-control" Width="600"></asp:TextBox></div></div>
<br/>
<div class="row">
    <label class="col-md-1 control-label">Data file name</label>
    <div class="col-xs-6" >
        <asp:TextBox ID="txtDatafileName" runat="server" CssClass="form-control inline" Width="300" ToolTip="For generate sample data and replace data source of mail merge file only. Changes to this box will not be saved."></asp:TextBox>
        <asp:Button ID="btnDownloadSampleData" CssClass="form-control inline" runat="server" Text="Download Sample data" Width="200px" ToolTip="Download files contains all fields from the query with max.5 records"
        OnClick="btnDownloadSampleData_Click"/></div></div>
<br/>
<div class="row">
    <label class="col-md-1 control-label">Template Download</label>
    <div class="col-xs-6">
    <asp:Button ID="btnDownloadSample" runat="server" Text="Download Sample Template" ToolTip="Download Sample template file for selected query" Width="200px"
        CssClass="form-control inline" OnClick="btnDownloadSample_Click"/>
    <asp:Button ID="btnDownloadTemplate" runat="server" Text="Download Template file" CssClass="form-control inline" Width="200px" OnClick="btnDownloadTemplate_Click" Visible="false" />
    </div></div>
<div class="row">
    <label class="col-md-1 control-label">Template Upload</label>
    <div class="col-xs-4">
        <asp:FileUpload ID="FileUpload1" runat="server" Width="600"/>
        <asp:Label runat="server" ID="lbWordFile" Visible="false"></asp:Label></div></div>
<br/>
<asp:Literal ID="lblJavascript" runat="server" />
</asp:Content>