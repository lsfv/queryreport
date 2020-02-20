<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.master" AutoEventWireup="true" CodeBehind="QuickSetup.aspx.cs" Inherits="QueryReport.Admin.QuickSetup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class='page-header'>
    <h1 class='pull-left'>
        <span>Administration</span>
    </h1>
    <div class='pull-right'>
        <div class='btn-group'>
            <asp:Button ID="Button2" runat="server" Text="Back" CssClass="btn btn-primary" onclick="back_Click" />
        </div>
        <div class='btn-group'>
            <asp:Button ID="Button1" runat="server" Text="Save" CssClass="btn btn-primary" onclick="save_Click" />
        </div>
    </div>
</div>

<div class='row'>
    <div class='col-sm-12'>
        <div class=' box bordered-box orange-border' style='margin-bottom:0;'>
            <div class='box-header blue-background'>
                <div class='title'>Quick Setup</div>
                <div class='actions'><a class="btn box-collapse btn-xs btn-link" href="#"><i></i></a></div>
            </div>
            <div style=" background-color:White ; height:1px;"></div>
            <div class='box-content box-no-padding'>
                <div class='responsive-table'>
                <div class='scrollable-area'>
                    <table class='table table-bordered-th table-striped' style='margin-bottom:0;'>
                        <thead>
                            <tr>
                                <th>Login ID</th>
                                <th>Name</th>
                                <th>Query Security Level</th>
                                
                                <th>User Group</th>
                                <th>Report Group</th>
                                <th>Report Access Right</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="Repeater3" runat="server"  OnItemDataBound="BINDING">
                                <ItemTemplate>
                                <tr>
                                    <td><asp:Literal ID="uid" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "UID")%>'></asp:Literal></td>
                                    <td><asp:Literal ID="name" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "NAME")%>'></asp:Literal></td>
                                    <td><asp:DropDownList ID="DDLQUERYLEVEL" runat="server"></asp:DropDownList></td>
                                    <td><asp:DropDownList ID="DDLUSERGROUP" runat="server"></asp:DropDownList></td>
                                    <td><asp:CheckBoxList ID="CBLREPORTGROUP" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" CssClass="limitcheckbox"></asp:CheckBoxList></td>
                                    <td><asp:CheckBoxList ID="CBLReportRight" CssClass=" limitcheckbox" runat="server" RepeatLayout="Flow" RepeatColumns="10"><asp:ListItem Value="0">Add</asp:ListItem><asp:ListItem Value="1">Modify</asp:ListItem><asp:ListItem Value="2">Delete</asp:ListItem><asp:ListItem Value="3">View</asp:ListItem></asp:CheckBoxList></td>
                                </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                      </tbody>
<%--                      <tfoot>
                        <tr>
                            <th>User Source</th>
                            <th>Login ID</th>
                            <th>Name</th>
                            <td colspan="4"></td>
                        </tr>
                      </tfoot>--%>
                    </table>
                </div>
                </div>
            </div>
        </div>
    </div>
</div>
<asp:Literal ID="lblJavascript" runat="server"></asp:Literal>
</asp:Content>