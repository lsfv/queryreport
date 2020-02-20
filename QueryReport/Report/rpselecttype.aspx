<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.master" AutoEventWireup="true" CodeBehind="rpselecttype.aspx.cs" Inherits="QueryReport.rpselecttype" %>
<asp:Content ID="Header1" ContentPlaceHolderID="header" runat="server">
<link rel="stylesheet" type="text/css" href="../assets/stylesheets/fa4/css/font-awesome.min.css" />
<style type="text/css">
INPUT[type=radio] { margin-right: 5px; }
</style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class='page-header'>
    <h1 class='pull-left'>
        <span>Select Report Type</span>
    </h1>
    <div class='pull-right'>
        <div class='btn-group'>
            <a name="buttom1" value="Next" id="ContentPlaceHolder1_Button2" class="btn btn-primary" onserverclick="Choose" runat="server"><i class="fa fa-arrow-right fa-lg"></i> Next</a>
        </div>
    </div>
</div>
<div class="row">
    <div  class="col-sm-8">
        <label class=" text">Please select report type</label> 
    </div>
    <div style=" height:40px">&nbsp;</div>
    <div  class="col-sm-8" style=" padding-left:30px;">
        <div class="row" style="margin-bottom: 15px">
            <div class="col-sm-2">
                <input id="rd_Excel" type="radio" value="1" checked="checked" name="rd1"/><label class="text" for="rd_Excel">Excel</label>
            </div>
            <div class="col-sm-8">
                (For output excel or on screen(Html))
            </div>
        </div>
        <div class="row">
            <div class="col-sm-2">
                <input id="rd_Word" type="radio" value="2" name="rd1"/><label class="text" for="rd_Word">Word</label>
            </div>
            <div class="col-sm-8">
                (For output word)
            </div>
        </div>
    </div>
</div>
</asp:Content>