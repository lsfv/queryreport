<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.master" AutoEventWireup="true" CodeBehind="ViewList.aspx.cs" Inherits="QueryReport.Admin.ViewList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class='page-header'>
    <h1 class='pull-left'>
        <span>Administration</span>
    </h1>
    <div class='pull-right'>
        <div class='btn-group'>
            <asp:Button ID="Button1" runat="server" Text="New" CssClass="btn btn-primary"
                onclick="Button1_Click" />
        </div>
    </div>
</div>

<div class='row'>
    <div class='col-sm-12'>
        <div class=' box bordered-box orange-border' style='margin-bottom:0;'>
            <div class='box-header blue-background'>
                <div class='title'>Query Maintenance</div>
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
                                <th>Name</th>
                                <th>Description</th>
                                <th>Query Security Level</th>
                                <th>Create Date</th>
                                <th>EDIT</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="Repeater3" runat="server">
                                <ItemTemplate>
                                <tr>
                                    <td><%# DataBinder.Eval(Container.DataItem, "SOURCEVIEWNAME")%></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "DESC")%></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "name")%></td>
                                    <td><%# ((DateTime)DataBinder.Eval(Container.DataItem, "AUDODATE")).ToString("yyyy-MM-dd")%></td>
                                    <td><asp:Button ID="Button3" runat="server" Text="Detail" Width="60px" CssClass="btn-default" OnClick="EDIT" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID")%>' /></td>
                                </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                      </tbody>
                      <tfoot>
                        <tr>
                            <th>Name</th>
                            <th>Description</th>
                            <th>View Level</th>
                            <td colspan="1"></td>
                        </tr>
                      </tfoot>
                    </table>
                </div>
                </div>
            </div>
        </div>
    </div>
</div>
</asp:Content>
