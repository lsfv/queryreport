<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.master" AutoEventWireup="true" CodeBehind="WordTemplate.aspx.cs" Inherits="QueryReport.Admin.WordTemplate" EnableEventValidation="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class='page-header'>
    <h1 class='pull-left'>
        <span>Administration</span>
    </h1>
    <div class='pull-right'>
        <div class='btn-group'>
            <asp:Button ID="btnNew" runat="server" Text="New" CssClass="btn btn-primary"  onclick="btnNew_Click" ValidationGroup="SAVE" />
        </div>
    </div>
</div>

<div class='row'>
    <div class='col-sm-12'>
        <div class=' box bordered-box orange-border' style='margin-bottom:0;'>
            <div class='box-header blue-background'>
                <div class='title'>Word Template List</div>
                <div class='actions'>
                    <a class="btn box-collapse btn-xs btn-link" href="#"><i></i></a>
                </div>
            </div>
            <div style=" background-color:White ; height:1px;"></div>
            <div class='box-content box-no-padding'>
                <div class='responsive-table'>
                <div class='scrollable-area'>
                    <table class='data-table-column-filter table table table-bordered-th table-striped' style='margin-bottom:0;'>
                        <thead>
                            <tr>
                                <th>Template Name</th>
                                <th>Description</th>
                                <th>Query name</th>
                                <th>Query level</th>
                                <th>Template File</th>
                                <th>Download</th>
                                <th>Edit</th>
                            </tr>
                        </thead>
                        <tfoot>
                            <tr>
                                <th>Template Name</th>
                                <th>Description</th>
                                <th>Query name</th>
                                <th>Query level</th>
                                <th>Template File</th>
                                <td></td>
                                <td></td>
                            </tr>
                        </tfoot>
                        <tbody>
                            <asp:Repeater ID="repeaterTemplate" runat="server">
                                <ItemTemplate>
                                <tr>
                                <td><%# DataBinder.Eval(Container.DataItem, "WORDTEMPLATEName")%></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Description")%></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "SOURCEVIEWNAME")%></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "VIEWLEVEL")%></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "TemplateFileName")%></td>
                                    <td><asp:Button ID="btnDownload" runat="server" Text="Download" Width="80px" CssClass="btn-default" OnClick="Download" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TemplateFileName")%>' /></td>
                                    <td><asp:Button ID="btnEdit" runat="server" Text="Detail" Width="60px" CssClass="btn-default" OnClick="EDIT" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "WORDTEMPLATEID")%>' /></td>
                                </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                      </tbody>
                    </table>
                </div>
                </div>
            </div>
        </div>
    </div>
</div>

<%--<div class='row'>
    <div class='col-sm-12'>
        <div class='box bordered-box orange-border' style='margin-bottom:0;'>
            <div class='box-header blue-background'>
                <div class='title'>View Level</div>
                <div class='actions'>
                    <a class="btn box-collapse btn-xs btn-link" href="#"><i></i></a>
                </div>
            </div>
            <div style=" background-color:White ; height:1px;"></div>
            <div class='box-content box-no-padding'>
                <div class='responsive-table'>
                <div class='scrollable-area'>
                    <table class='data-table-column-filter table table-bordered table-striped' style='margin-bottom:0;'>
                        <thead>
                            <tr>
                                <th>View Name</th>
                                <th>Level</th>
                                <th>Edit</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="Repeater1" runat="server">
                                <ItemTemplate>
                                <tr>
                                    <td><%# DataBinder.Eval(Container.DataItem, "SOURCEVIEWNAME")%></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "VIEWLEVEL")%></td>
                                    <td>
                                        <asp:DropDownList ID="DropDownList1" runat="server">
                                            <asp:ListItem Value="1">Normal</asp:ListItem>
                                            <asp:ListItem Value="2">Medium</asp:ListItem>
                                            <asp:ListItem Value="3">High</asp:ListItem>
                                            <asp:ListItem Value="4">Top</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>   
                                </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                      </tbody>
                      <tfoot>
                            <tr>
                            <th>View Name</th>
                            <td colspan="2"></td>
                            </tr>
                      </tfoot>
                    </table>
                </div>
                </div>
            </div>
        </div>
    </div>
</div>


<div class='row'>
    <div class='col-sm-12'>
        <div class='box bordered-box orange-border' style='margin-bottom:0;'>
            <div class='box-header blue-background'>
                <div class='title'>Distribution</div>
                <div class='actions'>
                    <a class="btn box-collapse btn-xs btn-link" href="#"><i></i></a>
                </div>
            </div>
            <div style=" background-color:White ; height:1px;"></div>
            <div class='box-content box-no-padding'>
                <div class='responsive-table'>
                <div class='scrollable-area'>
                    <table class='data-table-column-filter table table-bordered table-striped' style='margin-bottom:0;'>
                        <thead>
                            <tr>
                                <th>Report Name</th>
                                <th>Level</th>
                                <th>Edit</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="Repeater2" runat="server">
                                <ItemTemplate>
                                <tr>
                                    <td><%# DataBinder.Eval(Container.DataItem, "REPORTNAME")%></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "REPORTGROUPLIST")%></td>
                                    <td>
                                        <asp:DropDownList ID="DropDownList2" runat="server">
                                            <asp:ListItem Value="1">Normal</asp:ListItem>
                                            <asp:ListItem Value="2">Medium</asp:ListItem>
                                            <asp:ListItem Value="3">High</asp:ListItem>
                                            <asp:ListItem Value="4">Top</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>   
                                </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                      </tbody>
                      <tfoot>
                            <tr>
                            <th>Report Name</th>
                            <td colspan="2"></td>
                            </tr>
                      </tfoot>
                    </table>
                </div>
                </div>
            </div>
        </div>
    </div>
</div>--%>
</asp:Content>