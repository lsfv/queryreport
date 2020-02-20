<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.Master" AutoEventWireup="true" CodeBehind="rpexcel_printconfig.aspx.cs" Inherits="QueryReport.Report.rpexcel_printconfig" %>

<asp:Content ID="Content1" ContentPlaceHolderID="header" runat="server">
    <style type="text/css">
        .pd8 {
            padding: 8px 8px 8px 24px;
        }

        .form-control.n_lines_3 {
            height: 72px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script>
        $(function () {
            $('.cp').colorpickerplus().on('changeColor', function (e, color) {
                if (color == null)
                    $(this).val(color).css('background-color', '#fff');//tranparent
                else
                    $(this).val(color).css('background-color', color);
            }).each(
                function () {
                    $(this).trigger('changeColor', $(this).val());
                }
            );
        });
    </script>
    <div class='page-header'>
        <h1 class='pull-left'>
            <span>Printing related settings for Excel</span>
        </h1>
        <div class='pull-right'>
            <div class='btn-group'>
                <asp:Button ID="btnSave" runat="server" Text="Close" OnClick="btnSave_Click" CssClass="btn btn-primary" />
            </div>
        </div>
    </div>
    <div class='row' style="margin-bottom: 20px">
        <div class='col-sm-12'>
            <div class='box bordered-box orange-border' style='margin-bottom: 0;'>
                <div class='box-header blue-background'>
                    <div class='title'>
                        <span>Print Settings</span>
                    </div>
                    <div class='actions'>
                        <a class="btn box-collapse btn-xs btn-link" href="#"><i></i></a>
                    </div>
                </div>
                <div class='box-content box-no-padding'>
                    <div class="row">
                        <div class="col-xs-2 pd8">
                            <span>Print Orientation</span>
                        </div>
                        <div class="col-xs-4 pd8">
                            <select id="ddl_print_orientation" runat="server" class="col-xs-6">
                                <option value="-1">Not Set</option>
                                <option value="1">Portrait</option>
                                <option value="2">Landscape</option>
                            </select>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-2 pd8">
                            <span>Page Width</span>
                        </div>
                        <div class="col-xs-4 pd8">
                            <span class="col-xs-3">Fit to</span><input type="text" id="txt_print_fittopage" runat="server" class="form-control-smallinline" style="float: left; width: 16.66%" /><span class="col-xs-2"> pages</span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-2 pd8">
                            <span>Report Header<br />
                                (max. 255 chars)</span>
                        </div>
                        <div class="col-xs-8 pd8">
                            <asp:TextBox ID="txtReportHeader" runat="server" CssClass="form-control n_lines_3 inline" TextMode="MultiLine" Rows="3" MaxLength="255" Style='margin-bottom: 0;'></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-2 pd8">
                            <span>Report Footer<br />
                                (max. 255 chars)</span>
                        </div>
                        <div class="col-xs-8 pd8">
                            <asp:TextBox ID="txtReportFooter" runat="server" CssClass="form-control n_lines_3 inline" TextMode="MultiLine" Rows="3" MaxLength="255" Style='margin-bottom: 0;'></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-2 pd8">
                            <span>Sub Count Label (if applies)</span>
                        </div>
                        <div class="col-xs-4 pd8">
                            <input type="text" id="txtSubCntLbl" runat="server" class="form-control-smallinline" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-2 pd8">
                            <span>Font Family</span>
                        </div>
                        <div class="col-xs-4 pd8">
                            <input type="text" id="txtFontFamily" runat="server" class="form-control-smallinline" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-2 pd8">
                            <span>Grid lines (for PDF)</span>
                        </div>
                        <div class="col-xs-4 pd8">
                            <input type="checkbox" id="chkPdfGridLines" runat="server" class="form-control-smallinline" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class='row'>
        <div class='col-sm-12'>
            <div class='box bordered-box orange-border' style='margin-bottom: 0;'>
                <div class='box-header blue-background'>
                    <div class='title'>
                        <span>Column Settings</span>
                    </div>
                    <div class='actions'>
                        <a class="btn box-collapse btn-xs btn-link" href="#"><i></i></a>
                    </div>
                </div>
                <div class='box-content box-no-padding'>
                    <div class='responsive-table'>
                        <div class='scrollable-area'>
                            <asp:Table ID="gvReportColumns" runat="server" CssClass='table table-bordered-th table-striped desc'>
                            </asp:Table>
                        </div>
                        <div class="alert alert-info">
                            <span>Note: Set column width to "Auto" for autofit.</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <br />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cp_nonFormControls" runat="server">
    <asp:Literal ID="lblJavascript" runat="server" />
</asp:Content>
