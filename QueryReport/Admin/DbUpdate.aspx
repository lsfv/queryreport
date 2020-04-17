<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.Master" AutoEventWireup="true" CodeBehind="DbUpdate.aspx.cs" Inherits="QueryReport.Admin.DbUpdate" %>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class='page-header'>
    <h1 class='pull-left'>
        <span>Administration</span>
    </h1>
</div>

<div class='row'>
    <div class='col-sm-12'>
        <div class=' box bordered-box orange-border' style='margin-bottom:0;'>
            <div class='box-header blue-background'>
                <div class='title'>DB Update</div>
                <div class='actions'>
                    <a class="btn box-collapse btn-xs btn-link" href="#"><i></i></a>
                </div>
            </div>
            <div style=" background-color:White ; height:1px;"></div>
                <div style="margin-left:10px;margin-top:20px"><asp:Label ID="Label_Tip" runat="server" Text="Label"></asp:Label></div>
                <br />
                <div style="margin-bottom:20px;"><asp:Button ID="btn_update" runat="server" Text="Update" CssClass="btn" OnClick="btn_update_Click" /></div>
            <div class='box-content box-no-padding'>
            </div>
        </div>
    </div>
</div>


    
</asp:Content>