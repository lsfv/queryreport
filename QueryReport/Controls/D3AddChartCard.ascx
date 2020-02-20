<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="D3AddChartCard.ascx.cs" Inherits="QueryReport.Controls.D3AddChartCard" %>
<div id="well_last" class="well well-lg lastWell col-md-6">
    <svg id="chart_last" width="400" height="300" style="display: none" />

    <!-- Insert the following into the last well - Begin -->
    <a href="javascript: $('.newChart').hide(); $('#newform').show()">
        <div id="newChart" class="newChart">
            <span class="fa fa-plus"></span>
            <span class="fa fa-bar-chart"></span>
            <p>Add new chart</p>
        </div>
    </a>
    <div id="newform" style="display: none">
        <div class="form-inline">
            <div class="form-group">
                <label for="newform_view">View:</label>
                <asp:DropDownList runat="server" ID="newform_view" ClientIDMode="Static" class="form-control" required="true" onchange="newChartViewChange()" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="itemSelected">
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <label for="newform_type">Chart type:</label>
                <asp:DropDownList runat="server" ID="newform_type" ClientIDMode="Static" class="form-control" required="true" onchange="chartTypeChange()">
                </asp:DropDownList>
            </div>
        </div>
        <div class="form-group">
            <label for="newform_groupby">Group by:</label>
            <asp:ListBox runat="server" ID="newform_groupby" ClientIDMode="Static" multiple="multiple"></asp:ListBox>
        </div>
        <div class="form-group" id="aggregate_form">
            <label for="newform_agg">Aggregate type:</label>
            <asp:RadioButtonList runat="server" ID="newform_agg" ClientIDMode="Static" CSSclass="radio-inline">
                <asp:ListItem Text="Count" Value="0" Selected="True" />
                <asp:ListItem Text="Sum" Value="1" />
                <asp:ListItem Text="Average" Value="2" />
                <asp:ListItem Text="Max" Value="3" />
                <asp:ListItem Text="Min" Value="4" />
            </asp:RadioButtonList>
        </div>
        <div class="form-group" id="aggregate_column_form">
            <label id="label_newform_agg_column" for="newform_agg_column">Aggregate of:</label>
            <asp:DropDownList runat="server" ID="newform_agg_column" ClientIDMode="Static"  onchange="chartTypeChange()"></asp:DropDownList>
        </div>
        <!-- Reserved for HAVING clause
            <div class="form-group" id="having_form">
                <input type="text"></input>
            </div> -->
        <div class="form-group">
            <label for="newform_chartTitle">Chart title:</label>
            <asp:TextBox runat="server" class="form-control" ID="newform_chartTitle" ClientIDMode="Static" required="true" />
        </div>
        <asp:LinkButton ID="SaveNewChart" CssClass="btn btn-success mb-2" runat="server" OnClick="SaveNewChart_Click" ><i class="fa fa-floppy-o"></i> Save</asp:LinkButton>
        <asp:LinkButton ID="CancelNewChart" CssClass="btn btn-danger mb-2" runat="server" href="javascript: $('.newChart').show(); $('#newform').hide()"><i class="fa fa-ban"></i> Cancel</asp:LinkButton>
  
        <% if (this.IsPostBack)
           { %>
        <script type="text/javascript">
            $('.newChart').hide();
            $('#newform').show();
        </script>
        <% } %>

        <script type="text/javascript">
            chartTypeChange();
            function chartTypeChange() {
                if ($('#newform_type option:selected').index() != 2) {
                    $('#aggregate_form').hide();
                    $('#aggregate_column_form').hide();
                    $('#newform_agg input[value="0"]').prop("checked", true);
                    <% this.newform_agg.SelectedIndex = 0; %>
                } else {
                    $('#aggregate_form').show();
                    $('#aggregate_column_form').show();
                }
                newChartViewChange();
            }
            function newChartViewChange() {
                //var selectedEntries = $('#newform_groupby option:selected');
                var selected = [];
                $('#newform_groupby option:selected').each(function (index, brand) {
                    selected.push([$(this).val()]);
                });
                if ($('#newform_type option:selected').index() != 2) {
                    $('#newform_chartTitle').val(
                    $('label[for="' + $('#newform_agg input:checked').attr('id') + '"]').text() + ' of ' +
                    $("#newform_view").children("option").filter(":selected").text().replace(/(_|qreport.(TBD_)?v_(QR_)?)/gi, '') + ' by ' +
                    selected.toString());
                } else {
                    $('#newform_chartTitle').val(
                    $('label[for="' + $('#newform_agg input:checked').attr('id') + '"]').text() + ' of ' +
                    $("#newform_agg_column").children("option").filter(":selected").text().replace(/([A-Z])/g, ' $1').replace(/^./, function (str) { return str.toUpperCase(); }) + ' by ' +
                    selected.toString());
                }
            }
            //    function multiSel() {                                                           // $(document).ready(
            //        $('#newform_groupby').multiselect({
            //            selectAllValue: 'multiselect-all',
            //            enableCaseInsensitiveFiltering: true,
            //            enableFiltering: true,
            //            onChange: function (element, checked) {
            //                newChartViewChange();
            //            }
            //        });
            //    }
            $('#newform_groupby').change(newChartViewChange);
            $('#newform_agg input').change(newChartViewChange);
            //Sys.Application.add_load(multiSel);  // Load chart on start
        </script>
    </div>
</div>
<asp:Literal ID="lblJavascript" runat="server"></asp:Literal>
