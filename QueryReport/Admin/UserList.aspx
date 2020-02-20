<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.master" AutoEventWireup="true" CodeBehind="UserList.aspx.cs" Inherits="QueryReport.Admin.UserList"  EnableEventValidation="false"%>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class='page-header'>
    <h1 class='pull-left'>
        <span>Administration</span>
    </h1>
    <div class='pull-right'>
        <div class='btn-group'>
            <asp:Button ID="Button3" runat="server" Text="Quick Setup" CssClass="btn btn-primary"  onclick="setup_Click" ValidationGroup="SAVE" />
        </div>
        <div class='btn-group'>
            <asp:Button ID="Button1" runat="server" Text="New" CssClass="btn btn-primary"  onclick="Button1_Click" ValidationGroup="SAVE" />
        </div>
    </div>
</div>

<div class='row'>
    <div class='col-sm-12'>
        <div class=' box bordered-box orange-border' style='margin-bottom:0;'>
            <div class='box-header blue-background'>
                <div class='title'>User Maintenance</div>
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
                                <th>User ID</th>
                                <th>Name</th>
                                <th>User Group</th>
                                <th>Query Security Level</th>
                                <th>Access Right</th>
                                <th>Report Group</th>
                                <th>Edit</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="Repeater3" runat="server">
                                <ItemTemplate>
                                <tr>
                                    <td><%# DataBinder.Eval(Container.DataItem, "UID")%></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "NAME")%></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "USERGROUPNAME")%></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "SNAME")%></td>
                                    <td><%# getLevel((int)DataBinder.Eval(Container.DataItem, "REPORTRIGHT"))%></td>
                                    <td><%# getRPGroup(DataBinder.Eval(Container.DataItem, "REPORTGROUPLIST").ToString())%></td>
                                    <td><asp:Button ID="Button3" runat="server" Text="Detail" Width="60px" CssClass="btn-default" OnClick="EDIT" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID")%>' /></td>
                                </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                      </tbody>
                      <tfoot>
                            <tr>
                            <th>UID</th>
                            <th>NAME</th>
                            <th>User Group</th>
                            <th>Query Security Level</th>
                                <th>Access Right</th>
                                <th>Report Group</th>
                            <td></td>
                            </tr>
                      </tfoot>
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