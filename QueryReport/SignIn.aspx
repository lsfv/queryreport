<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="QueryReport.SignIn" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Query Report</title>
    <meta content='width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no'
        name='viewport' />
    <meta content='text/html;charset=utf-8' http-equiv='content-type' />
    <!-- / START - page related stylesheets [optional] -->
    <link rel="shortcut icon" href="favicon.ico" />
    <!-- / END - page related stylesheets [optional] -->
    <!-- / bootstrap [required] -->
    <link href="assets/stylesheets/bootstrap/bootstrap.css" media="all" rel="stylesheet"
        type="text/css" />
    <!-- / theme file [required] -->
    <link href="assets/stylesheets/light-theme.css" media="all" rel="stylesheet" type="text/css" />
    <!-- / coloring file [optional] (if you are going to use custom contrast color) -->
    <link href="assets/stylesheets/theme-colors.css" media="all" rel="stylesheet" type="text/css" />
    <!-- / demo file [not required!] -->
    <link href="assets/stylesheets/demo.css" media="all" rel="stylesheet" type="text/css" />
    <!--[if lt IE 9]>
      <script src="assets/javascripts/ie/html5shiv.js" type="text/javascript"></script>
      <script src="assets/javascripts/ie/respond.min.js" type="text/javascript"></script>
    <![endif]-->
    <link href="assets/stylesheets/plugins/select2/select2.css" media="all" rel="stylesheet"
        type="text/css" />
</head>
<body class='contrast-blue login contrast-background'>
    <form id="form1" runat="server">
    <div class='middle-container'>
        <div class='middle-row'>
            <div class='middle-wrapper'>
                <div class='login-container-header'>
                    <div class='container'>
                        <div class='row'>
                            <div class='col-sm-12'>
                                <div class='text-center'>
                                    <!--<img width="121" height="31" src="assets/images/logo_lg.svg" />-->
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class='login-container'>
                    <div class='container'>
                        <div class='row'>
                            <div class='col-sm-4 col-sm-offset-4'>
                                <h1 class='text-center title'>
                                    Query Report</h1>
                                <table cellspacing="0" cellpadding="0" style="width: 100%; border: 0">
                                    <tr>
                                        <td align="center">
                                            <div class='form-group' style="width: 400px; font-size: 12px;">
                                                <asp:Label ID="lblBuildNumber" runat="server" Text="" Visible="true"></asp:Label>
                                            </div>
                                            <div class='form-group' style="width: 400px;">
                                                <div class='controls with-icon-over-input'>
                                                    <input value="" class="form-control" data-rule-required="true" name="email" type="text"
                                                        runat="server" id="uid" />
                                                    <i class='icon-user text-muted'></i>
                                                </div>
                                            </div>
                                            <div class='form-group' style="width: 400px;">
                                                <div class='controls with-icon-over-input'>
                                                    <input value="" class="form-control" data-rule-required="true" name="password" type="password"
                                                        runat="server" id="password" />
                                                    <i class='icon-lock text-muted'></i>
                                                </div>
                                            </div>
                                            <div class='form-group' style="width: 400px;">
                                                <asp:DropDownList ID="ddlDatabase" runat="server" CssClass="form-control inline"
                                                    Width="100%">
                                                </asp:DropDownList>
                                                <label style="margin-left: 20px;">
                                                    Application & Database</label>
                                                <div class='form-group' style="width: 400px;">
                                                    <button class='btn btn-block' runat="server" id="btn" onserverclick="signin">
                                                        Sign in</button>
                                                </div>
                                                <div class='text-center'>
                                                    <hr class='hr-normal'>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                            </div>
                        </div>
                    </div>
                    <table cellspacing="0" cellpadding="0" style="width: 100%; border: 0">
                        <tr>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: center; vertical-align: top; padding-top: 0px;">
                                <asp:Image ID="imgCompanyIcon" runat="server" Width="45" Visible="false" />
                                <div class="EngText footer-inner" style="padding-bottom: 3px; font-weight: bold">
                                    <asp:Label ID="lblLicensedTo" runat="server" Text=""></asp:Label>
                                    Copyright © 2015 Data World Solutions Ltd. All rights reserved.
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    </form>
    <!-- / jquery [required] -->
    <script src="assets/javascripts/jquery/jquery.min.js" type="text/javascript"></script>
    <!-- / jquery mobile (for touch events) -->
    <script src="assets/javascripts/jquery/jquery.mobile.custom.min.js" type="text/javascript"></script>
    <!-- / jquery migrate (for compatibility with new jquery) [required] -->
    <script src="assets/javascripts/jquery/jquery-migrate.min.js" type="text/javascript"></script>
    <!-- / jquery ui -->
    <script src="assets/javascripts/jquery/jquery-ui.min.js" type="text/javascript"></script>
    <!-- / jQuery UI Touch Punch -->
    <script src="assets/javascripts/plugins/jquery_ui_touch_punch/jquery.ui.touch-punch.min.js"
        type="text/javascript"></script>
    <!-- / bootstrap [required] -->
    <script src="assets/javascripts/bootstrap/bootstrap.js" type="text/javascript"></script>
    <!-- / modernizr -->
    <script src="assets/javascripts/plugins/modernizr/modernizr.min.js" type="text/javascript"></script>
    <!-- / retina -->
    <script src="assets/javascripts/plugins/retina/retina.js" type="text/javascript"></script>
    <!-- / theme file [required] -->
    <script src="assets/javascripts/theme.js" type="text/javascript"></script>
    <!-- / demo file [not required!] -->
    <script src="assets/javascripts/demo.js" type="text/javascript"></script>
    <!-- / START - page related files and scripts [optional] -->
    <script src="assets/javascripts/plugins/validate/jquery.validate.min.js" type="text/javascript"></script>
    <script src="assets/javascripts/plugins/validate/additional-methods.js" type="text/javascript"></script>
    <!-- / END - page related files and scripts [optional] -->
    <script type="text/javascript">
        jQuery(document).ready(function () {
            jQuery("#btn").click(function (e) {
                // Fix __doPostBack() cause double postback problem
                return false;
            });
        });
    </script>
</body>
</html>
