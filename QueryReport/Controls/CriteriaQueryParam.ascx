<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CriteriaQueryParam.ascx.cs" Inherits="QueryReport.Controls.CriteriaQueryParam" %>
<div class="box">
    <div class="box-content">
        <div class="form-group row" style="padding: 6px 10px 0px 10px">
<%--            <input class="form-check-input" type="checkbox" id="<%=this.ColumnName%>_select" name="<%=this.ColumnName%>_select">--%>
            <label class="form-check-label" for="<%=this.ColumnName%>_select" style="width: 10%"><%=this.DisplayName%></label>
                <% if (this.ControlType == "bool")
                   { %>
                <label class="radio-inline">
                    <input id="<%=this.ColumnName%>_yes" type="radio" value="1" name="<%=this.ColumnName%>_optradio" onchange="$('#<%=this.ColumnName%>_select').prop('checked', true);" <% if (this.Value == "1") { %>checked="true"<% } %>>Yes</label>
                <label class="radio-inline">
                    <input id="<%=this.ColumnName%>_no" type="radio" value="0" name="<%=this.ColumnName%>_optradio" onchange="$('#<%=this.ColumnName%>_select').prop('checked', true);" <% if (this.Value == "0") { %>checked="true"<% } %>>No</label>
                <% }
                   else if (this.ControlType == "enum")
                   { %>
                <input id="<%=this.ColumnName%>_autocomplete" name="<%=this.ColumnName%>_autocomplete" type='text' class="form-control-smallinline" style="width: 30%" onchange="$('#<%=this.ColumnName%>_select').prop('checked', true);" <% if (this.Value != null) { %>value="<%=this.Value%>"<% } %> />
                <script type="text/javascript">
                    var options = [];
                   <% foreach (var foo in this.EnumArr)
                      { %>
                    options.push("<%= foo %>")
                    <% } %>
                    $("#<%=this.ColumnName%>_autocomplete").autocomplete({
                        minLength: 0,
                        source: options,
                        autoFocus: true
                    }).focus(function () {
                        if (this.value == "") {
                            $(this).autocomplete('search', '');
                            $('#<%=this.ColumnName%>_select').prop('checked', true);
                        }
                    });
                </script>
                <% }
                   else
                   { %>
                <input id='<%=this.ColumnName%>_field' name='<%=this.ColumnName%>_field' type='text' class='form-control-smallinline' <%=this.ControlType=="int" ? "type='number' min='0'" : String.Empty %>" class="<%= (this.ControlType == "datetime") ? "bootstrap-datetimepicker-widget form-control form-control-sm" : "form-control form-control-sm" %>" onchange="$('#<%=this.ColumnName%>_select').prop('checked', true);" style="width: 30%" data-datatype='<%=this.ControlType %>' <% if (this.Value != null) { %>value="<%=this.Value%>"<% } %> />
                <% } %>
        </div>
    </div>
</div>