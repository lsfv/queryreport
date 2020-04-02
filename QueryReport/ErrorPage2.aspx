<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.Master" AutoEventWireup="true" CodeBehind="ErrorPage2.aspx.cs" Inherits="QueryReport.ErrorPage2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="vsDoc" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br/>
    <br/>
    <div style="text-align:center"><asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Report/rplist.aspx"> Home    </asp:HyperLink>Sorry.Error!</div>
    <br/>
    <br/>
    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cp_nonFormControls" runat="server">
</asp:Content>
