<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.master" AutoEventWireup="true" CodeBehind="UserNew.aspx.cs" Inherits="QueryReport.Admin.UserNew" %>
<asp:Content ID="header" ContentPlaceHolderID="header" runat="server">
<style type="text/css">
.checkboxlist_nowrap label
{
     display:inline;
     white-space: nowrap;
}
</style>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class='page-header'>
    <h1 class='pull-left'>
        <span>Administration - User Maintenance</span>
    </h1>
    <div class='pull-right'>
        <div class='btn-group'>
            <asp:Button ID="Button2" runat="server" Text="Delete" CssClass="btn btn-primary"  onclick="Delete" />
        </div>
        <div class='btn-group'>
            <asp:Button ID="Button1" runat="server" Text="Save" CssClass="btn btn-primary"  onclick="Button1_Click" ValidationGroup="SAVE" />
        </div>
    </div>
</div>
<div class='row'><label class="col-md-1 control-label">Login ID</label><div class="col-xs-4"><asp:TextBox ID="txtuid" runat="server" CssClass=" form-control"></asp:TextBox></div>
    <label class="label"><asp:RequiredFieldValidator ID="RequiredFieldValidator1" 
        runat="server" ErrorMessage="* required field" ControlToValidate="txtuid" 
        ForeColor="Red" ValidationGroup="SAVE"></asp:RequiredFieldValidator></label>
</div><br/>
<div class='row'><label class="col-md-1 control-label">Password</label><div class="col-xs-4">
    <asp:TextBox ID="txtp1" runat="server" CssClass=" form-control" TextMode="Password"></asp:TextBox></div>
        <label class="label">
    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="* required field" ControlToValidate="txtp1" ForeColor="Red" ValidationGroup="SAVE"></asp:RequiredFieldValidator></label>
        </div><br/>
<div class='row'><label class="col-md-1 control-label">Confirm password</label><div class="col-xs-4">
    <asp:TextBox ID="txtp2" runat="server" CssClass=" form-control" TextMode="Password"></asp:TextBox></div><label class="label">
    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="* required field" ControlToValidate="txtp2" ForeColor="Red" ValidationGroup="SAVE"></asp:RequiredFieldValidator></label>
</div><br/>
<div class='row'><label class="col-md-1 control-label">Query Security Level</label><div class="col-xs-4"><asp:DropDownList ID="ddlsensitivitylevel" runat="server"  CssClass=" form-control"></asp:DropDownList></div></div><br/>
<div class='row'><label class="col-md-1 control-label">User Name</label><div class="col-xs-4"><asp:TextBox ID="txtusername" runat="server" CssClass=" form-control"></asp:TextBox></div></div><br/>
<div class='row'><label class="col-md-1 control-label">Email</label><div class="col-xs-4"><asp:TextBox ID="txtemail" runat="server" CssClass=" form-control"></asp:TextBox></div></div><br/>
<div class='row'><label class="col-md-1 control-label">User Group</label><div class="col-xs-4"><asp:DropDownList ID="DDLUSERGROUP" runat="server"></asp:DropDownList>
<label class="label"><asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="* required field" ControlToValidate="DDLUSERGROUP" ForeColor="Red" ValidationGroup="SAVE"></asp:RequiredFieldValidator></label>
</div></div><br/>
<div class='row'><label class="col-md-1 control-label">Report Group</label><div class="col-xs-4"><asp:CheckBoxList ID="CBLReportGroup" RepeatColumns="6" runat="server" RepeatLayout="Flow" CssClass="form-control MR8 checkboxlist_nowrap" style="height: auto"></asp:CheckBoxList></div></div><br/>
<div class='row'><label class="col-md-1 control-label">Report Access Right</label><div class="col-xs-4"><asp:CheckBoxList ID="CBLReportRight" CssClass="form-control MR8" runat="server" RepeatLayout="Flow" RepeatColumns="6"><asp:ListItem Value="0">Add</asp:ListItem><asp:ListItem Value="1">Modify</asp:ListItem><asp:ListItem Value="2">Delete</asp:ListItem><asp:ListItem Value="3">View</asp:ListItem></asp:CheckBoxList></div></div><br />
<%--<div class='row'><label class="col-md-1 control-label">Setup Right</label><div class="col-xs-4"><asp:CheckBoxList ID="CBLUSERSETUPRight" CssClass="form-control MR8" runat="server" RepeatLayout="Flow" RepeatColumns="10"><asp:ListItem Value="0">Setup</asp:ListItem><asp:ListItem Value="1">User setup</asp:ListItem></asp:CheckBoxList></div></div>--%>
</asp:Content>