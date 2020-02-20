<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.master" AutoEventWireup="true" CodeBehind="HtmlExport.aspx.cs" Inherits="QueryReport.report.HtmlExport" %>
<asp:Content ID="header" ContentPlaceHolderID="header" runat="server">
<style type="text/css">
TR[data-rowtype=groupheader]
{
    font-weight: bold;
}
</style>
<script type="text/javascript">
    jQuery("[data-tblname=custom]").ready(function () {
        jQuery("[data-tblname=custom]").addClass("table table-bordered table-striped");
    });
</script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class='page-header' style=" padding-bottom:2px;  margin-bottom:2px;">
    <h1 class='pull-left'>
        <span>Query Report - <asp:Literal ID="ltr" runat="server"></asp:Literal></span>
    </h1>
    <div class='pull-right'>
        <div class='btn-group'>
            <input type="submit" name="buttom1" value="Query Report List" id="ContentPlaceHolder1_Button2" class="btn btn-primary" onserverclick="back" runat="server"/>
        </div>
    </div>
</div>
<div class="row" style=" padding-top:0px; margin-top:0px;">
    <div  class="col-sm-12"><h4><asp:Literal ID="ltrcompanyname" runat="server"></asp:Literal></h4></div>
    <div  class="col-sm-12" style=" margin-bottom:3px"><asp:Literal ID="ltrreportTitle" runat="server"></asp:Literal></div>
    <asp:Literal ID="ltrCriterial" runat="server"></asp:Literal>
    <div  class="col-sm-12" style=" margin-bottom:3px ; margin-top:3px;"><asp:Literal ID="ltrreportDate" runat="server"></asp:Literal></div>
    <br />
    <asp:Literal ID="ltrTable" runat="server"></asp:Literal>
</div>
</asp:Content>