<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.Master" AutoEventWireup="true" CodeBehind="rpword.aspx.cs" Inherits="QueryReport.Report.rpword" %>
<asp:Content ID="header" ContentPlaceHolderID="header" runat="server">
    <style type="text/css">
        DIV.row {
            padding-bottom: 20px
        }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class='page-header'>
        <h1 class='pull-left'>
            <span>Report Detail - Base Information</span>
        </h1>
        <div class='pull-right'>
            <div class='btn-group'>
                <button id="btnNext" class="btn btn-primary" runat="server" onserverclick="btnNext_Click"><span>Next</span></button>
            </div>
        </div>
    </div>
    <div class="error" style="color: Red;">
        <asp:Literal ID="lblErrorText" runat="server"></asp:Literal>
    </div>
    <div class='row'>
        <div class='col-xs-12'>
            <div class="row">
                <div class="col-xs-6">
                    <div class="col-xs-4"><label>Step 1: Select the Query.</label>
                    </div>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlQueryName" runat="server" data-controlname="ddlQueryName" CssClass="form-control inline">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-6">
                    <div class="col-xs-4"><label>Step 2: Fill the Report Name.</label>
                    </div>
                    <div class="col-xs-8">
                        <asp:TextBox ID="txtReportName" runat="server" CssClass="form-control" data-controlname="txtReportName" MaxLength="50" Style='margin-bottom: 0;'></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12">
                    <div class="col-xs-12"><label>Step 3: Download Raw Template to the same folder and start editing.</label></div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-6">
                    <div class="col-xs-6">
                        <button id="btnDownloadRawTemplate" runat="server" onserverclick="btnDownloadRawTemplate_Click"><span style="white-space: nowrap">Download Raw Template</span></button>
                        <button id="btnSampleDataFile" style="margin-left: 5px" runat="server" onserverclick="btnSampleDataFile_Click"><span style="white-space: nowrap">Download Sample Data</span></button>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12">
                    <div class="col-xs-12"><label>Step 4: Upload modified templated file. Fill the report name above if you don't want to use report name included in the file.</label></div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-6">
                    <div class="col-xs-8">
                        <asp:FileUpload ID="fUploadTemplate" Width="100%" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <asp:Literal ID="lblJavascript" runat="server" />
</asp:Content>
<asp:Content ID="contentNonFormItems" ContentPlaceHolderID="cp_nonFormControls" runat="server">
</asp:Content>
