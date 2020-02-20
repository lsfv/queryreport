<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.master" AutoEventWireup="true" CodeBehind="SlevelNew.aspx.cs" Inherits="QueryReport.Admin.SlevelNew" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class='page-header'>
    <h1 class='pull-left'>
        <span>Administration - Query Security Level</span>
    </h1>
    <div class='pull-right'>
        <div class='btn-group'>
            <asp:Button ID="Button2" runat="server" Text="Delete" CssClass="btn btn-primary"  onclick="Delete" ValidationGroup="SAVE" />
        </div>
        <div class='btn-group'>
            <asp:Button ID="Button1" runat="server" Text="Save" CssClass="btn btn-primary"  onclick="Button1_Click" ValidationGroup="SAVE" />
        </div>
    </div>
</div>

<div class='row'>
    <label class=" col-xs-2">Name</label><div class="col-xs-3"><asp:TextBox ID="TextBox1" runat="server" CssClass=" form-control"></asp:TextBox></div>
       <label class="label"><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox1" ErrorMessage="* Invalid value." ForeColor="#FF3300" ValidationGroup="SAVE"></asp:RequiredFieldValidator></label>
   </div>
   <br />
   <div class='row'>
    <label class=" col-xs-2">Description</label><div class="col-xs-3"><asp:TextBox ID="TextBox2" runat="server" CssClass=" form-control"></asp:TextBox></div></div>
    <br />
   <div class='row'>
    <label class=" col-xs-2">Parent Security Level</label><div class="col-xs-3"><asp:DropDownList ID="DropDownList1" runat="server" CssClass=" form-control"></asp:DropDownList></div>
</div>
</asp:Content>