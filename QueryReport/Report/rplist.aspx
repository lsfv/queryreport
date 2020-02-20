<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.master" AutoEventWireup="true" CodeBehind="rplist.aspx.cs" Inherits="QueryReport.rplist" EnableEventValidation="false" %>
<asp:Content ID="header" ContentPlaceHolderID="header" runat="server">
    <link rel="stylesheet" type="text/css" href="../assets/stylesheets/fa4/css/font-awesome.min.css" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class='page-header'>
        <h1 class='pull-left'>
            <span>Query Report List</span>
        </h1>
        <div class='pull-right'>
            <div class='btn-group'>
                <a ID="btnNew" runat="server" onserverclick="btnNew_Click" class="btn btn-primary"><i class="fa fa-plus fa-lg"></i> New</a>
            </div>
        </div>
    </div>
    <div class='row'>
        <div class='col-sm-12'>
            <div class='box bordered-box orange-border' style='margin-bottom: 0;'>
                <div class='box-header blue-background'>
                    <div class='title'>
                        Query Report List</div>
                    <div class='actions'>
                        <a class="btn box-collapse btn-xs btn-link" href="#"><i></i></a>
                    </div>
                </div>
                <div class='box-content box-no-padding'>
                    <div class='responsive-table'>
                        <div class='scrollable-area'>
                            <table class='data-table-column-filter table table-bordered-th table-striped desc' data-sort-name='Report Name' data-sort-order='asc' style='margin-bottom: 1;'>
                                <thead>
                                    <tr>
                                        <th>Report Name</th>
                                        <th>Report Group</th>
                                        <th>Category</th>
                                        <th>Format</th>
                                        <th>Detail</th>
                                        <th>Run</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td>
                                                    <%# DataBinder.Eval(Container.DataItem, "REPORTNAME")%>
                                                </td>
                                                <td>
                                                    <%# DataBinder.Eval(Container.DataItem, "distributiondesc")%>
                                                </td>
                                                <td>
                                                    <%# DataBinder.Eval(Container.DataItem, "NAME")%>
                                                </td>
                                                <td>
                                                    <!--
                                                    <%# DataBinder.Eval(Container.DataItem, "TYPE").ToString() == "1" ? "<select class='form-control' style='width:120px;' name='Select" + DataBinder.Eval(Container.DataItem, "ID").ToString() + "'><option " + DataBinder.Eval(Container.DataItem, "DEFAULTFORMAT").ToString().Replace("0", "selected='selected'").Replace("1", "").Replace("2", "") + ">Excel</option><option " + DataBinder.Eval(Container.DataItem, "DEFAULTFORMAT").ToString().Replace("1", "selected='selected'").Replace("0", "").Replace("2", "") + ">On Screen</option><option " + DataBinder.Eval(Container.DataItem, "DEFAULTFORMAT").ToString().Replace("2", "selected='selected'").Replace("0", "").Replace("1", "") + ">PDF</option></select>" : "<select id='Select1' class='form-control' style='width:120px;'><option>Word</option></select>"%>
                                                    -->
                                                    <%# DataBinder.Eval(Container.DataItem, "TYPE").ToString() == "1" ? "Excel" : "Word" %>
                                                </td>
                                                <td>
                                                    <asp:Button ID="Button3" runat="server" Text="Detail" Width="60px" CssClass="btn-default" OnClick="EDIT" ToolTip='<%# DataBinder.Eval(Container.DataItem, "ID").ToString().Trim()+"|"+DataBinder.Eval(Container.DataItem, "TYPE").ToString().Trim()%>' />
                                                </td>
                                                <td>
                                                    <asp:Button ID="Button1" runat="server" Text="Run" OnClick="show" Width="60px" CssClass="btn-default" ToolTip='<%# DataBinder.Eval(Container.DataItem, "ID").ToString().Trim()+"|"+DataBinder.Eval(Container.DataItem, "TYPE").ToString().Trim()%>' />
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                                <tfoot>
                                    <tr style="font-weight: bold">
                                        <td>
                                            Report Name
                                        </td>
                                        <td>
                                            Report Group
                                        </td>
                                        <td>
                                            Catagory
                                        </td>
                                        <td>
                                            Format
                                        </td>
                                        <td colspan="2">
                                        </td>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <br />
</asp:Content>
