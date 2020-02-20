<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CriteriaNumber.ascx.cs" Inherits="QueryReport.Controls.CriteriaNumber" %>
<div class="box">
<div class="box-content">
<asp:Label ID="Label1" runat="server" Text="Label"></asp:Label> <asp:Button ID="autocomplete" runat="server" OnClick="Autocomplete_Click" Text="Browse values" CssClass="btn btn-primary btn-sm" /><br /><br />
<div class="row" style=" margin-bottom:12px;"><div class="col-xs-12">
    <input type="radio" name='<%=prefix%>rd1' value="r1" <%=op1=="r1"?"checked='checked'":""%>  <%=string.IsNullOrEmpty(op1)?"checked='checked'":""%>/>
    <select id="Select1" name='<%=prefix%>ddl1'>
        <option <%=range1=="="?"selected=\"selected\"":""%>>=</option>
        <option <%=range1==">"?"selected=\"selected\"":""%>>></option>
        <option <%=range1==">="?"selected=\"selected\"":""%>>>=</option>
        <option <%=range1=="<"?"selected=\"selected\"":""%>><</option>
        <option <%=range1=="<="?"selected=\"selected\"":""%>><=</option>
        <option <%=range1=="<>"?"selected=\"selected\"":""%>><></option>
        <option <%=range1=="In"?"selected=\"selected\"":""%>>In</option>
        <option <%=range1=="Not In"?"selected=\"selected\"":""%>>Not In</option>
    </select>
    <input id='<%=prefix%>tb1' type="text" autocomplete="off" name='<%=prefix%>tb1'  class="form-control-smallinline" style=" width:200px" value='<%=op1=="r1"?range2:""%>'/>
    </div></div>
    <div class="row" style=" margin-bottom:12px;"><div class="col-xs-12">

    <input type="radio" name='<%=prefix%>rd1' value="r2" <%=op1=="r2"?"checked='checked'":""%>/>Between
    <input id='<%=prefix%>tb2' type="text" autocomplete="off" name='<%=prefix%>tb2' class="form-control-smallinline" style=" width:200px" value='<%=op1=="r2"?range1:""%>'/> And <input id='<%=prefix%>tb3' type="text" name='<%=prefix%>tb3' class="form-control-smallinline"style=" width:200px" value='<%=op1=="r2"?range2:""%>'/>
</div></div>
<div class="row" style=" margin-bottom:12px;"><div class="col-xs-12">
    <input type="radio" name='<%=prefix%>rd1' value="r3" <%=op1=="r3"?"checked='checked'":""%>/>Empty
</div>
</div>
</div>
</div>
<asp:Literal ID="lbLiteral" runat="server"></asp:Literal>