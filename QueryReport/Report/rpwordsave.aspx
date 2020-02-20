<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.master" AutoEventWireup="true" CodeBehind="rpwordsave.aspx.cs" Inherits="QueryReport.rpwordsave" %>
<asp:Content ID="header" ContentPlaceHolderID="header" runat="server">
    <style type="text/css">
        label {
            font-weight: normal;
        }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class='page-header'>
        <h1 class='pull-left'>
            <span>Report Detail</span>
        </h1>
        <div class='pull-right'>
            <div class='btn-group'>
                <asp:Button ID="btnPrint" runat="server" Text="Run" OnClick="btnPrint_Click" CssClass="btn btn-primary" />
            </div>
            <div class='btn-group'>
                <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-primary" />
            </div>
            <div class='btn-group'>
                <asp:Button ID="btnBack" runat="server" Text="Query Report List" OnClick="btnBack_Click" CssClass="btn btn-primary" />
            </div>
        </div>
        <div class="col-xs-4 pull-right">
            <div class="col-xs-4" style="margin-top: 6px">
                <label class="LabelStyle">
                    Default Format</label>
            </div>
            <div class="col-xs-8">
                <asp:DropDownList ID="ddlFormat" runat="server" CssClass="form-control inline">
                    <asp:ListItem Text="Word" Value="0"></asp:ListItem>
                    <asp:ListItem Text="On Screen" Value="1"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
    </div>
    <asp:PlaceHolder ID="PlaceHolder_QueryParamsWrapper" runat="server" />
    <div class='row invoice'>
        <div class='col-sm-12'>
            <div class='box-content'>
                <asp:Panel ID="Panel1" runat="server">
                </asp:Panel>
            </div>
        </div>
    </div>
    <asp:Literal ID="lblJavascript" runat="server" />
    <script type="text/javascript">
        jQuery(document).ready(function () {
            jQuery("[data-datatype=datetime]").datepicker({
                currentDay: new Date(),
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy-mm-dd',
                duration: 'fast',
                selectFirst: false,
                onSelect: function (dateText, inst) {
                    $(this).change();
                }
            }).bind('blur', function () {
                var p_IsValid = true;

                try {
                    var p_strDate = this.value;
                    if (p_strDate.length != 10)
                        if (p_strDate.length != 0)
                            p_IsValid = false;

                    if (p_strDate.length != 0) {
                        var p_DateArray = p_strDate.split("-");
                        var p_ParsedDate = new Date(p_DateArray[0], p_DateArray[1] - 1, p_DateArray[2]);

                        var p_Year = parseInt(p_DateArray[0], 10);
                        var p_Month = parseInt(p_DateArray[1], 10);
                        var p_Day = parseInt(p_DateArray[2], 10);

                        if (p_Month > 12)
                            p_IsValid = false;

                        if (p_Day > 31)
                            p_IsValid = false;

                        if (isNaN(p_ParsedDate))
                            p_IsValid = false;

                        //check date is valid or not
                        var dateValidate = new Date(p_Year, p_Month - 1, p_Day);
                        if (dateValidate.getDate() != p_Day || dateValidate.getMonth() + 1 != p_Month || dateValidate.getFullYear() != p_Year)
                            p_IsValid = false;

                    }

                }
                catch (exDatePicker) { p_IsValid = false; }

                if (!p_IsValid) {
                    alert("Wrong date format, please input again");
                    this.value = "";
                }
            });
        });
    </script>
</asp:Content>
