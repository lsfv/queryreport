using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QueryReport.Code;

namespace QueryReport.Controls
{
    public partial class D3AddChartCard : System.Web.UI.UserControl
    {
        private Dictionary<String, String> currentView=null;
        private static readonly string[] number = {"int", "float", "smallint", "bigint"};

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ViewState.Clear();
            }
            //string js = "function newChartViewChange(){var selected=[];$('#newform_groupby option:selected').each(function(index,brand){selected.push([$(this).val()])});$('#newform_chartTitle').val($('#newform_view').children('option').filter(':selected').text().replace(/(_|qreport.(TBD_)?v_(QR_)?)/gi,'')+' by '+selected.toString())}$(document).ready(function(){$('#newform_groupby').multiselect({selectAllValue:'multiselect-all',enableCaseInsensitiveFiltering:true,enableFiltering:true,onChange:function(element,checked){newChartViewChange()}})});";
            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "mydata3", js, true);
        }

        protected IEnumerable<string> GetViewNames()
        {
            return CUSTOMRP.BLL.AppHelper.GetQueryReportViewNames(1, "DWHRMS_DEMO");
        }

        protected IEnumerable<string> GetTypes()
        {
            return Enum.GetNames(typeof(CUSTOMRP.BLL.AppHelper.ChartType));
        }

        //[WebMethod]
        protected IEnumerable<string> GetColumnNamesOfView(string view)
        {
            currentView = CUSTOMRP.BLL.AppHelper.GetColumnNamesByView(1, "DWHRMS_DEMO", view);
            return currentView.Keys.ToList();
        }

        public void InitDropDown()
        {
            var tmp = GetViewNames();
            foreach (var x in tmp)
            {
                this.newform_view.Items.Add(x);
            }
            foreach (var x in GetTypes())
            {
                this.newform_type.Items.Add(x);
            }
            foreach (var x in GetColumnNamesOfView(tmp.ElementAt(0)))
            {
                this.newform_groupby.Items.Add(x);
                this.newform_agg_column.Items.Add(x);
            }
        }

        protected void SaveNewChart_Click(object sender, EventArgs e)
        {
            try
            {
                if (bValidate())
                {
                    CUSTOMRP.BLL.AppHelper.AddNewChart(1, "CUSTOMRP", new CUSTOMRP.BLL.AppHelper.t_QueryReportDashboard
                    {
                        View = this.newform_view.SelectedValue.Substring(8),
                        Type = this.newform_type.SelectedIndex,
                        GroupBy = this.newform_groupby.SelectedValue,
                        Title = this.newform_chartTitle.Text,
                        AggregateType = int.Parse(this.newform_agg.SelectedValue),
                        AggregateColumn = this.newform_agg_column.SelectedValue
                    });
                    ViewState.Clear();
                    Response.Redirect("./rpportal.aspx");
                }
                else
                {
                    ViewState.Clear();
                }
            }
            catch
            {

            }
        }

        protected void itemSelected(object sender, EventArgs e)
        {
            this.lblJavascript.Text = String.Empty;

            this.newform_groupby.Items.Clear();
            this.newform_agg_column.Items.Clear();
            foreach (var x in GetColumnNamesOfView(this.newform_view.SelectedValue))
            {
                this.newform_groupby.Items.Add(x);
                if (number.Contains(this.currentView[x]))
                {
                    this.newform_agg_column.Items.Add(x);
                }
            }

            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "test3", "<script type=\"text/javascript\">$(document).ready(function(){$('#newform_groupby').multiselect({selectAllValue:'multiselect-all',enableCaseInsensitiveFiltering:true,enableFiltering:true,onChange:function(element,checked){newChartViewChange()}})});</script>", false);
            //this.lblJavascript.Text = "<script type=\"text/javascript\"> $('newform_groupby').empty(); </script>";
        }

        protected bool bValidate()
        {
            if (!CUSTOMRP.BLL.AppHelper.DashbordDistinctCount(1, "DWHRMS_DEMO", this.newform_view.SelectedValue.Substring(8), this.newform_groupby.SelectedValue))
            {
                //ScriptManager.RegisterOnSubmitStatement(this.Page, typeof(System.Web.UI.Page), "ShowEmailPreview", "alert('hi');", true);
                this.lblJavascript.Text = "<script type=\"text/javascript\">alert('Result returned too many columns. Please select another Group by column.');</script>";
                //((Literal) this.Page.FindControl("lblJavascript")).Text = "<script type=\"text/javascript\">alert('Too much');</script>";
                //ScriptManager.RegisterOnSubmitStatement(this.Page, typeof(System.Web.UI.Page), "alert", "alert('Result returned too many columns. Please select another Group by column.');");
                return false;
            }
            if (this.newform_groupby.SelectedItem == null)
            {
                //ScriptManager.RegisterStartupScript(this.Page, typeof(System.Web.UI.Page), "ShowEmailPreview", "alert('hi');", true);
                this.lblJavascript.Text = "<script type=\"text/javascript\">alert('Please select the item to Group by.');</script>";
                //((Literal)this.Page.FindControl("lblJavascript")).Text = "<script type=\"text/javascript\">alert('Too much');</script>";
                //ScriptManager.RegisterOnSubmitStatement(this.Page, typeof(System.Web.UI.Page), "alert", "alert('Please select the item to Group by.');");
                return false;
            }
            else if (this.newform_type.SelectedIndex == 2 && this.newform_agg_column.SelectedItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}