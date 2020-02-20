using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using QueryReport.Code;

namespace QueryReport
{
    public partial class rpportal : LoginUserPage
    {
        private JavaScriptSerializer mSerializer = new JavaScriptSerializer();
        private Controls.D3AddChartCard addChartCard;

        protected string DataTableToJSON(DataTable table, string groupByName)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    if (col.ColumnName.Equals(groupByName))
                    {
                        dict["label"] = row[col];
                    }
                    else
                    {
                        dict["value"] = row[col];
                    }
                }
                list.Add(dict);
            }
            return mSerializer.Serialize(list);
        }

        //protected IEnumerable<string> GetViewNames()
        //{
        //    return CUSTOMRP.BLL.AppHelper.GetQueryReportViewNames(me.ID, me.DatabaseNAME);
        //}

        //protected IEnumerable<string> GetTypes()
        //{
        //    return Enum.GetNames(typeof(CUSTOMRP.BLL.AppHelper.ChartType));
        //}

        ////[WebMethod]
        //protected IEnumerable<string> GetColumnNamesOfView(string view)
        //{
        //    return CUSTOMRP.BLL.AppHelper.GetColumnNamesByView(me.ID, me.DatabaseNAME, view);
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            this.DataBind();

            //if (!this.IsPostBack)
            //{
            List<CUSTOMRP.BLL.AppHelper.t_QueryReportDashboard> charts = CUSTOMRP.BLL.AppHelper.GetCharts(me.ID, "CUSTOMRP");
            //StringBuilder sb = new StringBuilder("<script>");

            int realCount = charts.Count + 1;
            int rowCnt = (int)Math.Round((realCount + .1f) / 2f);
            int cellCnt = 2;

            int rowCtr;
            int cellCtr;

            for (rowCtr = 0; rowCtr < rowCnt; rowCtr++)
            #region Getting saved chart data
            {
                TableRow tRow = new TableRow();
                this.dashboardTable.Rows.Add(tRow);
                for (cellCtr = 0; cellCtr < cellCnt; cellCtr++)
                {
                    int number = rowCtr * 2 + cellCtr;
                    if (number >= realCount) break;

                    TableCell tCell = new TableCell();
                    tRow.Cells.Add(tCell);

                    if (number != realCount - 1)
                    {
                        DataTable dt = CUSTOMRP.BLL.AppHelper.DashboardAggregation(me.ID, me.DatabaseNAME, charts[number].View, charts[number].GroupBy, charts[number].AggregateType, charts[number].AggregateColumn);
                        string json = this.DataTableToJSON(dt, charts[number].GroupBy);

                        Controls.D3Chart control = (Controls.D3Chart)Page.LoadControl("~/controls/D3Chart.ascx");
                        control.cID = charts[number].ID;
                        control.Type = charts[number].Type;
                        control.chart_data = json;
                        control.chart_title = charts[number].Title;
                        if (control.Type == 2) {
                            StringBuilder sb = new StringBuilder();
                            switch (charts[number].AggregateType)
                            {
                                case 0:
                                    sb.Append("Count");
                                    break;
                                case 1:
                                    sb.Append("Sum");
                                    break;
                                case 2:
                                    sb.Append("Average");
                                    break;
                                case 3:
                                    sb.Append("Maximum");
                                    break;
                                case 4:
                                    sb.Append("Minimum");
                                    break;
                            }
                            if (charts[number].AggregateType != 0)
                            {
                                sb.Append(" of ");
                                sb.Append(charts[number].AggregateColumn);
                            }
                            control.bar_x_axis_label = sb.ToString();
                        }
                        control.bar_y_axis_label = charts[number].GroupBy.Replace('_', ' ');
                        tCell.Controls.Add(control);
                    }
                    else
                    {
                        addChartCard = (Controls.D3AddChartCard)Page.LoadControl("~/controls/D3AddChartCard.ascx");

                        string[] reportNames = CUSTOMRP.BLL.AppHelper.GetQueryReportViewNames(me.ID, me.DatabaseNAME);
                        string views = mSerializer.Serialize(reportNames.ToList());
                        addChartCard.InitDropDown();

                        tCell.Controls.Add(addChartCard);
                    }
                }
            }
            #endregion
        }
    }
}