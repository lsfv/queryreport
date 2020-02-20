<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CriteriaString.ascx.cs" Inherits="QueryReport.Controls.CriteriaString" %>
<div class="box">
    <div class="box-content">
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label> <% if (this.ControlType != "datetime") { %><asp:Button ID="autocomplete" runat="server" OnClick="Autocomplete_Click" Text="Browse values" CssClass="btn btn-primary btn-sm" /><% } %><br />
        <br />
        <div class="row" style="margin-bottom: 12px;">
            <div class="col-xs-12">
                <input type="radio" name='<%=prefix%>rd1' value="r1" <%=op1=="r1"?"checked='checked'":""%> <%=string.IsNullOrEmpty(op1)?"checked='checked'":""%> />
                <select id="Select1" name='<%=prefix%>ddl1'>
                    <option <%=range1=="Begins With"?"selected=\"selected\"":""%>>Begins With</option>
                    <option <%=range1=="Contains"?"selected=\"selected\"":""%>>Contains</option>
                    <% if (this.ControlType != "datetime")
                       {%>
                    <option <%=range1=="In"?"selected=\"selected\"":""%>>In</option>
                    <option <%=range1=="Not In"?"selected=\"selected\"":""%>>Not In</option>
                    <% } %>
                    <option <%=range1=="="?"selected=\"selected\"":""%>>=</option>
                    <option <%=range1==">="?"selected=\"selected\"":""%>>&gt;=</option>
                    <option <%=range1=="<="?"selected=\"selected\"":""%>>&lt;=</option>
                    <option <%=range1=="Does not contain"?"selected=\"selected\"":""%>>Does not contain</option>
                    <option <%=range1=="Does not equal"?"selected=\"selected\"":""%>>Does not equal</option>
                </select>
                <input type="text" autocomplete="off" id='<%=prefix%>tb1' name='<%=prefix%>tb1' class="form-control-smallinline <%= (this.ControlType == "datetime") ? "bootstrap-datetimepicker-widget" : String.Empty %>" value='<%=op1=="r1"?range2:""%>' style="width: 200px" data-datatype='<%=this.ControlType %>' />
            </div>
        </div>
        <div class="row" style="margin-bottom: 12px;">
            <div class="col-xs-12">
                <input type="radio" name='<%=prefix%>rd1' value="r2" <%=op1=="r2"?"checked='checked'":""%> />Between<input type="text" id='<%=prefix%>tb2' name='<%=prefix%>tb2' class="form-control-smallinline <%= (this.ControlType == "datetime") ? "bootstrap-datetimepicker-widget" : String.Empty %>" style="width: 200px" value='<%=op1=="r2"?range1:""%>' data-datatype='<%=this.ControlType %>' />
                And
                <input type="text" autocomplete="off" id='<%=prefix%>tb3' name='<%=prefix%>tb3' class="form-control-smallinline <%= (this.ControlType == "datetime") ? "bootstrap-datetimepicker-widget" : String.Empty %>" style="width: 200px" value='<%=op1=="r2"?range2:""%>' data-datatype='<%=this.ControlType %>' />
            </div>
        </div>
        <div class="row" style="margin-bottom: 12px;">
            <div class="col-xs-12">
                <input type="radio" name='<%=prefix%>rd1' value="r3" <%=op1=="r3"?"checked='checked'":""%> />Empty
            </div>
        </div>
    </div>
</div>
<asp:Literal ID="lbLiteral" runat="server"></asp:Literal>
