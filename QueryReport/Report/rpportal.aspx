<%@ Page Language="C#" MasterPageFile="~/Controls/mp.master" AutoEventWireup="true" CodeBehind="rpportal.aspx.cs" Inherits="QueryReport.rpportal" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="header" runat="server">
    <title>Dashboard</title>
    <link rel="stylesheet" type="text/css" href="../assets/stylesheets/fa4/css/font-awesome.min.css" />
    <link rel="stylesheet" type="text/css" href="../assets/stylesheets/bootstrap/bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="../assets/stylesheets/multi_widget/multi.css" />
    <link rel="stylesheet" type="text/css" href="../assets/stylesheets/bootstrap/bootstrap-switch.min.css" />
    <style>
        .closeButton {
            position: absolute;
            top: 0;
            right: 0;
        }

        .newChart {
            font-size: x-large;
            font-weight: bold;
            text-align: center;
            vertical-align: middle;
        }

        .well {
            padding: 5px;
            width: 600px;
            height: 500px;
            position: relative;
        }

        .lastWell {
            background-color: linen;
        }

        .vcenter {
            display: inline-block;
            vertical-align: middle;
            float: none;
        }

        svg {
            margin: 30px;
            width: 500px;
            height: 400px;
            text-align: center;
        }

        .bar--positive {
            fill: #00acec;
        }

        .bar--positive:hover {
            fill: darkblue;
        }

        .bar--negative {
            fill: indianred;
        }

        .bar--negative:hover {
            fill: darkred;
        }

        .axis text {
            font: 10px sans-serif;
        }

        .axis path,
        .axis line {
            fill: none;
            stroke: #000;
            shape-rendering: crispEdges;
        }

        .title {
            font-size: 18px;
            font-family: arial;
            /*font-weight: bold;*/
        }

        .sort_switch {
          position: absolute;
          top: 0;
          left: 0;
        }

        .ctooltip {
          position: absolute;
          width: 200px;
          height: auto;
          padding: 10px;
          background-color: white;
          -webkit-border-radius: 10px;
          -moz-border-radius: 10px;
          border-radius: 10px;
          -webkit-box-shadow: 4px 4px 10px rgba(0, 0, 0, 0.4);
          -moz-box-shadow: 4px 4px 10px rgba(0, 0, 0, 0.4);
          box-shadow: 4px 4px 10px rgba(0, 0, 0, 0.4);
          pointer-events: none;
        }

        .ctooltip.hidden {
          display: none;
        }

        .ctooltip p {
          margin: 0;
          font-size: 16px;
          line-height: 20px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class='page-header'>
        <h1 class='pull-left'>
            <span>Dashboard</span>
        </h1>
    </div>
    <asp:ScriptManager runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/assets/javascripts/d3/d3.min.js" />
            <asp:ScriptReference Path="~/assets/javascripts/d3/d3pie.min.js" />
            <asp:ScriptReference Path="~/assets/javascripts/jquery/jquery.min.js" />
            <asp:ScriptReference Path="~/assets/javascripts/bootstrap/bootstrap.min.js" />
            <asp:ScriptReference Path="~/assets/javascripts/bootstrap/bootstrap-switch.min.js" />
            <asp:ScriptReference Path="~/assets/javascripts/multi_widget/multi.js" />
        </Scripts>
    </asp:ScriptManager>
    <asp:Table ID="dashboardTable" runat="server">
    </asp:Table>
    <asp:Literal ID="lblJavascript" runat="server"></asp:Literal>
</asp:Content>