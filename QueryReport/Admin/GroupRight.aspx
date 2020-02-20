<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.master" AutoEventWireup="true" CodeBehind="GroupRight.aspx.cs" Inherits="QueryReport.Admin.GroupRight" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class='page-header'>
    <h1 class='pull-left'>
        <span>Administration</span>
    </h1>
    <div class='pull-right'>
        <div class='btn-group'><asp:Button ID="Button1" runat="server" Text="Save" CssClass="btn btn-primary"  onclick="Button1_Click" ValidationGroup="SAVE" /></div>
    </div>
</div>

<div class=' box bordered-box orange-border' style='margin-bottom:0;'>
<div class='box-header blue-background'>
                <div class='title'>User Group Right</div>
                <div class='actions'>
                    <a class="btn box-collapse btn-xs btn-link" href="#"><i></i></a>
                </div>
            </div>
<div class="row" style=" margin-top:20px;">
    <label class="col-xs-2" style=" max-width:120px; margin:0px;  margin-left:15px ; margin-top:6px;">User Group</label><div class="col-xs-5" style="max-width:300px;"><asp:DropDownList ID="DDLUSERGROUP" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="changeGID"></asp:DropDownList></div>
</div>
<h5></h5>
<table class="table table-bordered table-striped ML20">
    <tr><td style=" width:200px;">Company Maintenance</td><td><input type="checkbox" id="companyview"  runat="server"/>View<input type="checkbox" id="companyadd"  runat="server"/>Add<input  type="checkbox" id="companymodify"  runat="server"/>Modify<input type="checkbox" id="companydel" runat="server"/>Delete</td></tr>
    <tr><td>Report Group</td><td><input name="rg" type="checkbox" id="rgview"  runat="server"/>View<input name="rg" type="checkbox" id="rgadd"  runat="server"/>Add<input name="rg" type="checkbox" id="rgmodify"  runat="server"/>Modify<input name="rg" type="checkbox" id="rgdel" runat="server"/>Delete</td></tr>
    <tr><td>Category</td><td><input name="category" type="checkbox" id="categoryview"  runat="server"/>View<input name="rg" type="checkbox" id="categoryadd"  runat="server"/>Add<input name="rg" type="checkbox" id="categorymodify"  runat="server"/>Modify<input name="rg" type="checkbox" id="categorydel" runat="server"/>Delete</td></tr>
    <tr><td>Query Security Level</td><td><input name="sl" type="checkbox" id="slview"  runat="server"/>View<input name="sl" type="checkbox" id="sladd"  runat="server"/>Add<input name="sl" type="checkbox" id="slmodify"  runat="server"/>Modify<input name="sl" type="checkbox" id="sldel" runat="server"/>Delete</td></tr>
    <tr><td>Query Maintenance</td><td><input name="query" type="checkbox" id="queryview"  runat="server"/>View<input name="query" type="checkbox" id="queryadd"  runat="server"/>Add<input name="query" type="checkbox" id="querymodify"  runat="server"/>Modify<input name="query" type="checkbox" id="querydel" runat="server"/>Delete</td></tr>
    <tr><td>User Group</td><td><input name="usergroup" type="checkbox" id="usergroupview"  runat="server"/>View<input name="usergroup" type="checkbox" id="usergroupadd"  runat="server"/>Add<input name="usergroup" type="checkbox" id="usergroupmodify"  runat="server"/>Modify<input name="usergroup" type="checkbox" id="usergroupdel" runat="server"/>Delete</td></tr>
    <tr><td>User Group Right</td><td><input name="usergroup" type="checkbox" id="ugrview"  runat="server"/>View<input name="usergroup" type="checkbox" id="ugradd"  runat="server"/>Add<input name="usergroup" type="checkbox" id="ugrmodify"  runat="server"/>Modify<input name="usergroup" type="checkbox" id="ugrdel" runat="server"/>Delete</td></tr>
    <tr><td>User Maintenance</td><td><input name="user" type="checkbox" id="userview"  runat="server"/>View<input name="user" type="checkbox" id="useradd"  runat="server"/>Add<input name="user" type="checkbox" id="usermodify"  runat="server"/>Modify<input name="user" type="checkbox" id="userdel" runat="server"/>Delete</td></tr>
    <tr runat="server" visible="false"><td>Word Template</td><td><input name="user" type="checkbox" id="Wordview"  runat="server"/>View<input name="user" type="checkbox" id="Wordadd"  runat="server"/>Add<input name="user" type="checkbox" id="Wordmodify"  runat="server"/>Modify<input name="user" type="checkbox" id="Worddel" runat="server"/>Delete</td></tr>
    <tr><td>Copy Security</td><td><input name="user" type="checkbox" id="Copyview"  runat="server"/>View<input name="user" type="checkbox" id="Copyadd"  runat="server"/>Add<input name="user" type="checkbox" id="Copymodify"  runat="server"/>Modify<input name="user" type="checkbox" id="Copydel" runat="server"/>Delete</td></tr>
</table>
</div>
</asp:Content>