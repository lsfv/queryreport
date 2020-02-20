using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using QueryReport.Code;

namespace QueryReport.report
{
    //generate html tag and set them to a server level .
    public partial class HtmlExport : LoginUserPage
    {
        private CUSTOMRP.Model.REPORT myReport = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Request.QueryString["rpid"]) == false)
                {
                    if (Session[rpexcel.strSessionKeyMyReport] != null)
                    {
                        myReport = (CUSTOMRP.Model.REPORT)Session[rpexcel.strSessionKeyMyReport];
                    }
                    else
                    {
                        int rpid = Int32.Parse(Request.QueryString["rpid"]);
                        myReport = WebHelper.bllReport.GetModel(me.ID, rpid);

                        if (myReport != null)
                        {
                            this.ltr.Text = myReport.REPORTNAME;
                            HtmlExport1(myReport.ID);
                        }
                        else
                        {
                            Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.parameter_error, "rplist.aspx");
                            Response.End();
                        }
                    }
                }
                else if (string.IsNullOrEmpty(Request.QueryString["active"]) == false)
                {
                    if (Session[rpexcel.strSessionKeyMyReport] != null)
                    {
                        myReport = (CUSTOMRP.Model.REPORT)Session[rpexcel.strSessionKeyMyReport];
                    }

                    QueryReport.Controls.RptSavePage prepage = (QueryReport.Controls.RptSavePage)Context.Handler;
                    DataTable rpdt = prepage.rpdt;
                    List<string> rpci = prepage.rpcr;
                    string rptitle = prepage.container.ReportTitle;
                    this.ltr.Text = prepage.container.ReportName;
                    //string showType = prepage.container.fChangeValueOnly ? "1" : "0";
                    string ExtendedFields = prepage.container.ExtendedFields;

                    List<string> sums = prepage.container.sumColumn != null ? prepage.container.sumColumn.Select(x => x.ColumnName).ToList() : new List<string>();
                    List<string> avgs = prepage.container.avgColumn != null ? prepage.container.avgColumn.Select(x => x.ColumnName).ToList() : new List<string>();
                    List<string> groups = prepage.container.groupColumn != null ? prepage.container.groupColumn.Select(x => x.ColumnName).ToList() : new List<string>();
                    List<string> subtotal = prepage.container.grouptotalColumn != null ? prepage.container.grouptotalColumn.Select(x => x.ColumnName).ToList() : new List<string>();
                    List<string> subavg = prepage.container.groupavgColumn != null ? prepage.container.groupavgColumn.Select(x => x.ColumnName).ToList() : new List<string>();
                    List<string> subcount = prepage.container.groupcountColumn != null ? prepage.container.groupcountColumn.Select(x => x.ColumnName).ToList() : new List<string>();
                    List<string> count = prepage.container.rpcountColumn != null ? prepage.container.rpcountColumn.Select(x => x.ColumnName).ToList() : new List<string>();
                    var tmp = myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.SortOn);
                    List<string> sortonCols = tmp.Select(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DisplayName).ToList();
                    List<bool> isAscending = tmp.Select(x => x.IS_ASCENDING).ToList();
                    List<int> seq = tmp.Select(x => x.SEQ).ToList();
                    string subcountLabel = !String.IsNullOrEmpty(myReport.SUBCOUNT_LABEL) ? myReport.SUBCOUNT_LABEL : null;
                    bool pdfGridLines = myReport.PDF_GRID_LINES;

                    List<string> hideRows = myReport.ReportColumns
                        .Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content
                            && x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal
                            && x.EXCEL_COLWIDTH == 0m)
                        .Select(x => x.COLUMNNAME).ToList();

                    HtmlExport2(ExtendedFields.Split(','),
                        //prepage.container.contentColumn.ToDictionary(x => x.ColumnName, y => y.DisplayName),
                        myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content).ToDictionary(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DisplayName, y => y.DisplayName),
                        subtotal, subavg, subcount, count, rpdt, rpci, rptitle, sums, avgs, groups, subcountLabel, sortonCols, isAscending, seq, hideRows);
                }
            }
        }

        protected void back(object sender, EventArgs e)
        {
            Response.Redirect("rplist.aspx", true);
        }

        protected void HtmlExport2(string[] ExtendedFields, Dictionary<string, string> content, List<string> subtotal, List<string> subavg, List<string> SubCount, List<string> Count, DataTable dt, List<string> cr, string rpstr, List<string> sums, List<string> avgs, List<string> group,
            string subcountLabel, List<string> sortonCols, List<bool> isAscending, List<int> seq, List<string> hiderows)
        {
            //v1.1.0 - Cheong - 2016/06/01 - Hide criteria text
            bool hideHeaders = ExtendedFields[CUSTOMRP.Model.REPORT.EXTENDFIELDs.ReportType] == "2";
            bool hideCriteria = ExtendedFields[CUSTOMRP.Model.REPORT.EXTENDFIELDs.HideCriteria] == "1";

            if (!hideHeaders)
            {
                this.ltrcompanyname.Text = AppNum.companyName;
                this.ltrreportTitle.Text = rpstr;

                if (!hideCriteria)
                {
                    foreach (string cir in cr)
                    {
                        //v1.2.0 Kim 2016.12.08 replace criteria str from sql str to readable text
                        this.ltrCriterial.Text += "<div class='col-sm-12'>" + cir + "</div>";
                        ////v1.0.0 - Cheong - 2016/03/17 - Add formatting to criteria
                        ////this.ltrCriterial.Text += "<div class='col-sm-12'>" + cir + "</div>";
                        //this.ltrCriterial.Text += "<div class='col-sm-12'>" + cir.Remove(0, 6).TrimEnd(')') + "</div>";
                    }
                }

                this.ltrreportDate.Text = "Print on : " + DateTime.Today.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            }

            this.ltrTable.Text = AppHelper.GetHtmlTableString(ExtendedFields, content, subtotal, subavg, SubCount, Count, dt, sums, avgs, group,
                subcountLabel: subcountLabel, sortonCols: sortonCols, isAscending: isAscending, seq: seq, hideRows: hiderows);
        }

        protected void HtmlExport1(int rpid)
        {
            //string sql = AppHelper.getSql(rpid, me);
            //DataTable dt = WebHelper.bllCommon.query(sql);

            List<string> comments = new List<string>();
            List<string> avgs = new List<string>();
            List<string> sums = new List<string>();
            List<string> groups = new List<string>();
            List<string> subtotal = new List<string>();
            List<string> subavg = new List<string>();
            List<string> subcount = new List<string>();
            List<string> count = new List<string>();

            //v1.2.0 - Cheong - 2016/07/04 - Add option to hide duplicate items
            //DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(ref comments, ref avgs, ref sums, ref groups, ref subtotal, ref subavg, ref subcount, ref count, me, rpid, true);
            DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(me.ID, ref comments, ref avgs, ref sums, ref groups, ref subtotal, ref subavg, ref subcount, ref count, me,
                rpid, myReport.fHideDuplicate);

            this.ltrcompanyname.Text = AppNum.companyName;
            this.ltrreportTitle.Text = myReport.RPTITLE.Trim();

            //v1.1.0 - Cheong - 2016/06/01 - Hide criteria text
            if (!myReport.fHideCriteria)
            {
                foreach (string cir in comments)
                {
                    //v1.0.0 - Cheong - 2016/03/17 - Add formatting to criteria
                    //this.ltrCriterial.Text += "<div  class='col-sm-12'>" + cir + "</div>";
                    this.ltrCriterial.Text += "<div  class='col-sm-12'>" + cir.Remove(0, 6).TrimEnd(')') + "</div>";
                }
            }
            this.ltrreportDate.Text = "Print on : " + DateTime.Now.ToString("yyyy-MM-dd");

            this.ltrTable.Text = AppHelper.GetHtmlTableString(myReport.EXTENDFIELD.Split(','),
                myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content).ToDictionary(x => x.COLUMNNAME, y => y.DisplayName),
                subtotal, subavg, subcount, count, dt, sums, avgs, groups);
        }
    }
}