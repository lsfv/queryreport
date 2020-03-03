using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using NPOI.HPSF;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using CUSTOMRP.Model;
using QueryReport.Code;

namespace QueryReport
{
    public partial class rpexcel2 : QueryReport.Controls.RptSavePage
    {
        private CUSTOMRP.Model.REPORT myReport = null;
        private List<CUSTOMRP.Model.ColumnInfo> columninfos = null;
        private bool p_fSuppressRender = false;     // Whether to render page contents

        // Alex 2018.09.21 QueryParams - Begin
        public const string strSessionKeyQueryParams = "__SESSION_REPORT_QueryParams";
        // Alex 2018.09.21 QueryParams - End
        #region Event Handlers

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.p_fSuppressRender)
            {
                base.Render(writer);
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session[rpexcel.strSessionKeyMyReport] != null)
            {
                myReport = (CUSTOMRP.Model.REPORT)Session[rpexcel.strSessionKeyMyReport];
            }

            if (myReport == null)
            {
                if (!String.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    int id = Int32.Parse(Request.QueryString["id"]);
                    myReport = WebHelper.bllReport.GetModel(me.ID, id);

                    if (myReport == null)
                    {
                        Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.parameter_error, "rplist.aspx");
                        Response.End();
                    }
                }
            }

            if (Session[rpexcel.strSessionKeyColumnInfo] != null)
            {
                columninfos = (List<CUSTOMRP.Model.ColumnInfo>)Session[rpexcel.strSessionKeyColumnInfo];
            }

            if (Session[rpexcel.strSessionKeyReportParameterContainer] != null)
            {
                container = (ReportParameterContainer)Session[rpexcel.strSessionKeyReportParameterContainer];
                //v1.0.0 - Cheong - 2015/07/13 - Patch SEQ
                if (myReport != null)
                {
                    foreach (CUSTOMRP.Model.REPORTCOLUMN rc in myReport.ReportColumns)
                    {
                        rc.SEQ = -1;
                    }
                    CUSTOMRP.Model.REPORTCOLUMN l_ReportColumn = null;
                    int contentSEQ = 1;
                    foreach (Fields f in container.contentColumn)
                    {
                        l_ReportColumn = myReport.ReportColumns.Where(x => x.COLUMNFUNC == 1 && x.DisplayName == f.DisplayName).FirstOrDefault();
                        if (l_ReportColumn != null)
                        {
                            l_ReportColumn.SEQ = contentSEQ;
                            contentSEQ++;
                        }
                    }
                    //v1.0.0 - Cheong - 2016/03/23 - Preserve order on criteria and group columns
                    int criteriaSEQ = 1;
                    foreach (Fields f in container.criteriaColumn)
                    {
                        l_ReportColumn = myReport.ReportColumns.Where(x => x.COLUMNFUNC == 2 && x.DisplayName == f.DisplayName).FirstOrDefault();
                        if (l_ReportColumn != null)
                        {
                            l_ReportColumn.SEQ = criteriaSEQ;
                            criteriaSEQ++;
                        }
                    }
                    int sortonSEQ = 1;
                    foreach (Fields f in container.sortonColumn)
                    {
                        l_ReportColumn = myReport.ReportColumns.Where(x => x.COLUMNFUNC == 3 && x.DisplayName == f.DisplayName).FirstOrDefault();
                        if (l_ReportColumn != null)
                        {
                            l_ReportColumn.SEQ = sortonSEQ;
                            sortonSEQ++;
                        }
                    }
                    //v1.0.0 - Cheong - 2016/03/23 - Preserve order on criteria and group columns
                    int groupSEQ = 1;
                    foreach (Fields f in container.groupColumn)
                    {
                        l_ReportColumn = myReport.ReportColumns.Where(x => x.COLUMNFUNC == 6 && x.DisplayName == f.DisplayName).FirstOrDefault();
                        if (l_ReportColumn != null)
                        {
                            l_ReportColumn.SEQ = groupSEQ;
                            groupSEQ++;
                        }
                    }
                    myReport.ReportColumns.OrderBy(x => x.COLUMNFUNC).ThenBy(x => x.SEQ);
                }
            }
            else if (!ReportParameterContainer.LoadReport(myReport.ID, me, out container, out columninfos)) // try load default from table
            {
                Response.Redirect("rplist.aspx");
            }


        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.lblJavascript.Text = String.Empty;

            //v1.2.0 - Cheong - 2016/07/06 - Also change button text corresponding to save behaviour change
            if (!String.IsNullOrEmpty(Request.QueryString["id"]))
            {
                btnSave.Text = "Save";
            }
            else
            {
                btnSave.Text = "Save and Exit";
            }

            if (IsPostBack)
            {
                this.ParseDataToContainer();
                container.Format = Convert.ToInt32(this.ddlFormat.SelectedValue);
                myReport.DEFAULTFORMAT = container.Format;
            }
            this.LoadCriteriaUIFromSession();

            this.ddlFormat.SelectedValue = Convert.ToString(container.Format);
            myReport.TYPE = 1;
            //v1.0.0 - Cheong - 2015/03/31 if no right to modify, disable Save button
            if (!me.rp_modify)
            {
                this.btnSave.Visible = false;
            }
            else
            {
                //linson: 如果不是直接打印报表，那么就是新建立报表，这样的话直接保存报表数据。
                if ((!this.IsPostBack) && (Request.Params["cmd"] != "run") && (myReport != null))//!!!! logic is changed! 
                {
                    if (WebHelper.bllReport.Replace(myReport))
                    {
                        Session[rpexcel.strSessionKeyMyReport] = myReport;
                    }
                    else
                    {
                        var last = CUSTOMRP.BLL.AppHelper.GetLastError();
                        this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}:\n{1}\");</script>", last.ReportName, last.Message);
                    };
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (myReport != null)
            {
                try
                {
                    if (WebHelper.bllReport.Replace(myReport))
                    {
                        Session[rpexcel.strSessionKeyMyReport] = myReport;
                        //v1.2.0 - Cheong - 2016/07/05 - Do not redirect to main page when report settings is saved successfully.Redirect if report mode is new because error will occur on subsequent click.
                        if (container.queryParams != null)
                        {
                            foreach (var x in container.queryParams)
                            {
                                WebHelper.bllQUERYPARAMS.Delete(me.ID, myReport.ID, x.ParamName);
                                WebHelper.bllQUERYPARAMS.Add(me.ID, new QUERYPARAMS { REPORT = myReport.ID, NAME = x.ParamName, VALUE = x.Value });
                            }
                        }

                        var dt = WebHelper.bllQUERYPARAMS.GetList(me.ID, myReport.ID).Tables[0].AsEnumerable().ToDictionary<DataRow, string, string>(rw => Convert.ToString(rw["NAME"]),
                                                                         rw => Convert.ToString(rw["VALUE"]));
                        foreach (var x in dt)
                        {
                            var control = PlaceHolder_QueryParamsWrapper.FindControl(x.Key);
                            if (control != null)
                            {
                                ((Controls.CriteriaQueryParam)control).Value = x.Value;
                            }
                        }
                        if (Request.Params["id"] == null)
                        {
                            this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}\");window.location = \"rplist.aspx\";</script>", "Record has been saved successfully.");
                        }
                        else
                        {
                            this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}\");</script>", "Record has been saved successfully.");
                        }
                        return;
                    }
                    else
                    {
                        var last = CUSTOMRP.BLL.AppHelper.GetLastError();
                        this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}:\n{1}\");</script>", last.ReportName, last.Message);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("IX_REPORT"))   // something related to duplicate key
                    {
                        this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}\");</script>", "Duplicate report name is found in the database. Please check.");
                    }
                    else
                    {
                        var last = CUSTOMRP.BLL.AppHelper.GetLastError();
                        this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}:\n{1}\");</script>", last.ReportName, last.Message);
                    }
                }
            }
        }

        protected void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                this.rpdt = getReportDatatable();

                if (container.Format == 1)
                {
                    Server.Transfer("htmlexport.aspx?active=html", true);
                }
                else if (container.Format == 0)
                {
                    //1.处理不同的Report type.因为只有excel文件格式才需要处理多种Report type如,pivotable，所以看起来好像只有这里分支了.
                    if (myReport.IsPivoTable)
                    {
                        //get datatable 2.check wheater it has pivotable 3. generate new excel or load pivotable and reload data.4.download file
                        if (this.rpdt != null)
                        {
                            string pivotablePath =null;//null:there is no templte!.
                            CUSTOMRP.Model.WORDFILE file = WebHelper.bllWordFile.GetModelByReportID(me.ID, myReport.ID);
                            if(file!=null)
                            {
                                pivotablePath = Server.MapPath("~/"+AppNum.STR_EXCELTEMPLATEPATH)+"/"+file.WordFileName;
                            }

                            string pivotableFileName = "";
                            bool isSuccess = false;
                            string errMsg = null;
                            if (!string.IsNullOrEmpty(pivotablePath) && File.Exists(pivotablePath))//has pivotable template already.
                            {
                                Debug.WriteLine("update:" + pivotablePath);
                                pivotableFileName = System.IO.Path.GetFileName(pivotablePath);
                                //isSuccess = Common.incOpenXml.UpdataData4XlsxExcel(this.rpdt, "Report", out errMsg, pivotablePath);
                                isSuccess = CUSTOMRP.BLL.TemplateManager.UpdataData4XlsxExcel(this.rpdt, out errMsg, pivotablePath);
                            }
                            else//no template
                            {
                                string floder = Server.MapPath("~/" + AppNum.STR_EXCELTEMPLATEPATH);
                                bool res= Common.Utils.IOFile.CreateFloder(floder,out errMsg);
                                if (res)
                                {
                                    pivotablePath = floder + "/" + myReport.REPORTNAME + ".xlsx";
                                    pivotableFileName = System.IO.Path.GetFileName(pivotablePath);
                                    //isSuccess = Common.incOpenXml.GenerateXlsxExcel(this.rpdt, out errMsg, pivotablePath);
                                    isSuccess = CUSTOMRP.BLL.TemplateManager.GenerateXlsxExcel(this.rpdt, out errMsg, pivotablePath);
                                }
                                else
                                {
                                    isSuccess = false;
                                }
                            }
                            if (isSuccess)
                            {
                                DownloadFile(pivotablePath, pivotableFileName);
                            }
                            else
                            {
                                this.lblJavascript.Text = WebHelper.GetAlertJS(errMsg);
                            }
                        }
                    }
                    else
                    {
                        Dictionary<string, decimal> colwidths = myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content)
                            .ToDictionary(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DisplayName,
                            y => y.EXCEL_COLWIDTH);

                        Dictionary<string, QueryReport.Code.NPOIHelper.ColumnSetting> colSettings = myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content)
                            .ToDictionary(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DisplayName,
                            y => new QueryReport.Code.NPOIHelper.ColumnSetting(y.FONT_SIZE, y.FONT_BOLD, y.FONT_ITALIC, y.HORIZONTAL_TEXT_ALIGN, y.CELL_FORMAT, y.BACKGROUND_COLOR, y.FONT_COLOR)); //, y.IS_ASCENDING, y.SORT_SEQUENCE

                        //v1.8.8 Alex 2018.10.22 Add Group Indent Width
                        List<decimal> indentWidths = myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Group).Select(x => x.EXCEL_COLWIDTH).ToList();

                        //v1.8.8 Alex 2018.11.01 Move sort orders to rpexcel. Retrieve data here - Begin 
                        var tmp = myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.SortOn);
                        List<string> sortonCols = tmp.Select(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DisplayName).ToList();
                        List<bool> isAscending = tmp.Select(x => x.IS_ASCENDING).ToList();
                        List<int> seq = tmp.Select(x => x.SEQ).ToList();
                        //v1.8.8 Alex 2018.11.01 Move sort orders to rpexcel. Retrieve data here - End

                        List<string> rptHeader = !String.IsNullOrEmpty(myReport.REPORT_HEADER) ? new List<string>(myReport.REPORT_HEADER.Split('\n')) : null;
                        List<string> rptFooter = !String.IsNullOrEmpty(myReport.REPORT_FOOTER) ? new List<string>(myReport.REPORT_FOOTER.Split('\n')) : null;
                        string subcountLabel = !String.IsNullOrEmpty(myReport.SUBCOUNT_LABEL) ? myReport.SUBCOUNT_LABEL : null;
                        bool pdfGridLines = myReport.PDF_GRID_LINES;
                        string fontFamily = !String.IsNullOrEmpty(myReport.FONT_FAMILY) ? myReport.FONT_FAMILY : null;

                        //v1.2.0 Kim 2016.12.08 replace criteria str from sql str to readable text
                        ////v1.0.0 - Cheong - 2016/03/17 - Add formatting to criteria
                        //rpcr = rpcr.Select(x => x.Remove(0, 6).TrimEnd(')')).ToList();


                        //v1.2.0 Kim 2016.12.08 replace criteria str from sql str to readable text
                        //v1.8.8 Alex 2018.10.05
                        XSSFWorkbook XSSFworkbook = NPOIHelper.GetWorkbookFromDt(rpdt, container.ReportTitle, container.ExtendedFields.Split(','), rptHeader, rpcr,
                            //container.contentColumn.ToDictionary(x => !String.IsNullOrEmpty(x.Formula) ? x.DisplayName : x.ColumnName, y => y.DisplayName),
                            myReport.ReportColumns.Where(x => x.ColumnFunc == REPORTCOLUMN.ColumnFuncs.Content).ToDictionary(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DisplayName, y => y.DisplayName),
                            container.avgColumn.Select(x => x.ColumnName).ToList(), container.sumColumn.Select(x => x.ColumnName).ToList(), container.groupColumn.Select(x => x.ColumnName).ToList(),
                            container.grouptotalColumn.Select(x => x.ColumnName).ToList(), container.groupavgColumn.Select(x => x.ColumnName).ToList(), container.groupcountColumn.Select(x => x.ColumnName).ToList(), container.rpcountColumn.Select(x => x.ColumnName).ToList(),
                            colwidths, myReport.PrintOrientation, myReport.PRINT_FITTOPAGE,
                            rptFooter, colSettings, fontFamily, indentWidths, subcountLabel, sortonCols, isAscending, seq);

                        string fileName = container.ReportName + ".xlsx";
                        string folder = PathHelper.getTempFolderName();
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        string filePath = folder + PathHelper.getSafePath(fileName);
                        using (FileStream outFs = new FileStream(filePath, FileMode.Create))
                        {
                            XSSFworkbook.Write(outFs);
                            //CA2202
                            //outFs.Close();
                        }
                        DownloadFile(filePath, fileName);
                    }
                }
                else
                {
                    string fontPath = PathHelper.getFontFolderName() + "simsun.ttc,1";
                    string[] rptHeader = !String.IsNullOrEmpty(myReport.REPORT_HEADER) ? myReport.REPORT_HEADER.Split('\n') : null;
                    string[] rptFooter = !String.IsNullOrEmpty(myReport.REPORT_FOOTER) ? myReport.REPORT_FOOTER.Split('\n') : null;
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

                    //v1.2.0 Kim 2016.12.08 replace criteria str from sql str to readable text
                    ////v1.0.0 - Cheong - 2016/03/17 - Add formatting to criteria
                    //rpcr = rpcr.Select(x => x.Remove(0, 6).TrimEnd(')')).ToList();

                    //Console.Write(container.groupcountColumn.Select(x => x.ColumnName).ToList());

                    bool showChangeOnly = container.ExtendedFields.Split(',')[CUSTOMRP.Model.REPORT.EXTENDFIELDs.ReportType] == CUSTOMRP.Model.REPORT.ExtReportType.ChangeOnly;
                    bool hideHeaders = container.ExtendedFields.Split(',')[CUSTOMRP.Model.REPORT.EXTENDFIELDs.ReportType] == CUSTOMRP.Model.REPORT.ExtReportType.DataExport;
                    bool hideCriteria = container.ExtendedFields.Split(',')[CUSTOMRP.Model.REPORT.EXTENDFIELDs.HideCriteria] == "1";

                    Common.MyPdf.exp_Pdf(showChangeOnly,hideHeaders,hideCriteria,
                        myReport.ReportColumns.Where(x => x.ColumnFunc == REPORTCOLUMN.ColumnFuncs.Content).ToDictionary(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DisplayName, y => y.DisplayName),
                        container.grouptotalColumn.Select(x => x.ColumnName).ToList(), container.groupavgColumn.Select(x => x.ColumnName).ToList(), container.groupcountColumn.Select(x => x.ColumnName).ToList(),
                        container.rpcountColumn.Select(x => x.ColumnName).ToList(), container.groupColumn.Select(x => x.ColumnName).ToList(),
                        container.avgColumn.Select(x => x.ColumnName).ToList(), container.sumColumn.Select(x => x.ColumnName).ToList(),
                        AppNum.companyName, rptHeader ?? rpcr.ToArray(), rpdt, container.ReportTitle,
                        fontPath, 14, 1, iTextSharp.text.BaseColor.BLACK,
                        fontPath, 11, 1, iTextSharp.text.BaseColor.BLACK,//列头字体、大小、样式、颜色
                        PathHelper.getTempFolderName(), container.ReportName,
                        fontPath, 11, 1, iTextSharp.text.BaseColor.BLACK,//正文字体、大小、样式、颜色
                        rptFooter, subcountLabel, sortonCols, isAscending, seq, hideRows, pdfGridLines);
                }
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                CUSTOMRP.BLL.AppHelper.LogException(ex, me.ID, myReport.ID, myReport.REPORTNAME); // myReport.REPORTNAME
                this.lblJavascript.Text = WebHelper.GetAlertJS(ex.Message);
            }
            catch (Exception ex)
            {
                CUSTOMRP.BLL.AppHelper.LogException(ex, me.ID, myReport.ID, myReport.REPORTNAME); // myReport.REPORTNAME
                this.lblJavascript.Text = WebHelper.GetAlertJS(ex.Message);
            }
        }

        public void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("rplist.aspx", true);
        }

        #endregion

        #region Private Methods
        private  DataTable getReportDatatable()
        {
            string strSqlColumn, strSqlPlus, strSqlCriteria, strSqlSortOn;
            //v1.2.0 Kim 2016.12.08 replace criteria str from sql str to readable text ://getSql(ref rpcr, out strSqlColumn, out strSqlCriteria, out strSqlPlus, out strSqlSortOn);
            getSql(out strSqlColumn, out strSqlCriteria, out strSqlPlus, out strSqlSortOn);

            rpcr = new List<string> { };
            foreach (var cr in container.criteriaColumn)
            {
                if (cr.Operator == "EMPTY" || cr.Operator == "Does not equal" || !string.IsNullOrEmpty(cr.Value1) || !string.IsNullOrEmpty(cr.Value2))
                {
                    if (cr.Operator == "BETWEEN")
                    {
                        rpcr.Add(cr.SelectStatement + " " + cr.Operator + " \"" + cr.Value1 + "\" and \"" + cr.Value2 + "\"");
                    }
                    else if (cr.Operator == "EMPTY")
                    {
                        rpcr.Add(cr.SelectStatement + " " + cr.Operator);
                    }
                    else
                    {
                        rpcr.Add(cr.SelectStatement + " " + cr.Operator + " \"" + cr.Value1 + "\"");
                    }
                }
            }

            var sqlParams_queryParams = GetQueryParams(container.queryParams);//new varible:sqlParams_queryParams for function getDataForReport
            return CUSTOMRP.BLL.AppHelper.getDataForReport(me.ID, container.SVID, me.DatabaseNAME, strSqlColumn, strSqlPlus, strSqlCriteria, strSqlSortOn, myReport.fHideDuplicate, sqlParams_queryParams);
        }

        private List<SqlParameter> GetQueryParams(List<CUSTOMRP.BLL.AppHelper.QueryParamsObject> QueryParamsObjects)
        {
            List<SqlParameter> sqlParams_queryParams = new List<SqlParameter>();
            if (QueryParamsObjects != null)
            {
                foreach (var x in QueryParamsObjects)//container.queryParams
                {
                    switch (x.SqlType)
                    {
                        case "bit":
                            sqlParams_queryParams.Add(new SqlParameter() { ParameterName = "@QueryParam_" + x.ParamName, SqlDbType = SqlDbType.Bit, Value = int.Parse(x.Value) });
                            break;
                        case "int":
                            sqlParams_queryParams.Add(new SqlParameter() { ParameterName = "@QueryParam_" + x.ParamName, SqlDbType = SqlDbType.Int, Value = int.Parse(x.Value) });
                            break;
                        case "date":
                            sqlParams_queryParams.Add(new SqlParameter() { ParameterName = "@QueryParam_" + x.ParamName, SqlDbType = SqlDbType.Date, Value = x.Value });
                            break;
                        case "datetime":
                            sqlParams_queryParams.Add(new SqlParameter() { ParameterName = "@QueryParam_" + x.ParamName, SqlDbType = SqlDbType.DateTime, Value = x.Value });
                            break;
                        case "varchar":
                            sqlParams_queryParams.Add(new SqlParameter() { ParameterName = "@QueryParam_" + x.ParamName, Size = -1, SqlDbType = SqlDbType.VarChar, Value = x.Value });
                            break;
                        case "nvarchar":
                            sqlParams_queryParams.Add(new SqlParameter() { ParameterName = "@QueryParam_" + x.ParamName, Size = -1, SqlDbType = SqlDbType.NVarChar, Value = x.Value });
                            break;
                    }
                }
            }
            return sqlParams_queryParams;
        }

        private void LoadCriteriaUIFromSession()
        {
            CUSTOMRP.Model.SOURCEVIEW mySV = WebHelper.bllSOURCEVIEW.GetModel(me.ID, container.SVID);

            foreach (Fields cn in container.criteriaColumn)
            {
                ColumnInfo columnInfo = columninfos.Where(x => x.ColName == cn.ColumnName).FirstOrDefault();
                if (columnInfo != null) // skip column if it no longer exists.
                {
                    CUSTOMRP.Model.REPORTCOLUMN col = myReport.ReportColumns.Where(x => x.ColumnFunc == REPORTCOLUMN.ColumnFuncs.Criteria && x.COLUMNNAME == cn.ColumnName).FirstOrDefault();

                    if ("String" == columnInfo.DataType)
                    {
                        Controls.CriteriaString control = (Controls.CriteriaString)Page.LoadControl("~/controls/CriteriaString.ascx");
                        control.ColumnName = columnInfo.ColName;
                        //v1.7.0 Ben 2017.12.20 - columnInfo.DisplayName is ColumnName if directly from clicking run (generated by GetColumnInfoForStoredProc). While cn.DisplayName is generated from 'container.criteriaColumn =...' in rpexcel.aspx which must be the actual
                        //control.DisplayName = columnInfo.DisplayName;
                        control.DisplayName = cn.DisplayName;
                        control.DBName = me.DatabaseNAME;
                        control.SourceView = mySV.TBLVIEWNAME;
                        control.SourceType = mySV.SOURCETYPE;

                        //if ((!IsPostBack) && (col != null))
                        if (col != null)
                        {
                            control.op1 = col.CRITERIA2;
                            control.range1 = col.CRITERIA3;
                            control.range2 = col.CRITERIA4;
                        }

                        this.Panel1.Controls.Add(control);
                    }
                    if ("DateTime" == columnInfo.DataType)
                    {
                        Controls.CriteriaString control = (Controls.CriteriaString)Page.LoadControl("~/controls/CriteriaString.ascx");
                        control.ColumnName = columnInfo.ColName;
                        //v1.7.0 Ben 2017.12.20 - columnInfo.DisplayName is ColumnName if directly from clicking run (generated by GetColumnInfoForStoredProc). While cn.DisplayName is generated from 'container.criteriaColumn =...' in rpexcel.aspx which must be the actual
                        //control.DisplayName = columnInfo.DisplayName;
                        control.DisplayName = cn.DisplayName;
                        control.ControlType = "datetime";
                        control.DBName = me.DatabaseNAME;
                        control.SourceView = mySV.TBLVIEWNAME;
                        control.SourceType = mySV.SOURCETYPE;

                        //if ((!IsPostBack) && (col != null))
                        if (col != null)
                        {
                            control.op1 = col.CRITERIA2;
                            control.range1 = col.CRITERIA3;
                            control.range2 = col.CRITERIA4;
                        }

                        this.Panel1.Controls.Add(control);
                    }
                    else if (("Int" == columnInfo.DataType) || ("Decimal" == columnInfo.DataType))
                    {
                        Controls.CriteriaNumber control = (Controls.CriteriaNumber)Page.LoadControl("~/Controls/CriteriaNumber.ascx");
                        control.ColumnName = columnInfo.ColName;
                        //v1.7.0 Ben 2017.12.20 - columnInfo.DisplayName is ColumnName if directly from clicking run (generated by GetColumnInfoForStoredProc). While cn.DisplayName is generated from 'container.criteriaColumn =...' in rpexcel.aspx which must be the actual
                        //control.DisplayName = columnInfo.DisplayName;
                        control.DisplayName = cn.DisplayName;
                        control.DBName = me.DatabaseNAME;
                        control.SourceView = mySV.TBLVIEWNAME;
                        control.SourceType = mySV.SOURCETYPE;

                        //if ((!IsPostBack) && (col != null))
                        if (col != null)
                        {
                            control.op1 = col.CRITERIA2;
                            control.range1 = col.CRITERIA3;
                            control.range2 = col.CRITERIA4;
                        }

                        this.Panel1.Controls.Add(control);
                    }
                    else if ("Enum" == columnInfo.DataType)
                    {
                        Controls.CriteriaInt control = (Controls.CriteriaInt)Page.LoadControl("~/controls/CriteriaInt.ascx");
                        control.ColumnName = columnInfo.ColName;
                        //v1.7.0 Ben 2017.12.20 - columnInfo.DisplayName is ColumnName if directly from clicking run (generated by GetColumnInfoForStoredProc). While cn.DisplayName is generated from 'container.criteriaColumn =...' in rpexcel.aspx which must be the actual
                        //control.DisplayName = columnInfo.DisplayName;
                        control.DisplayName = cn.DisplayName;
                        CUSTOMRP.Model.RpEnum rp = new RpEnum();
                        Type a = rp.GetType().GetNestedType((mySV.SOURCEVIEWNAME + "_" + cn).ToUpper());
                        control.dt = Common.Utils.GetTableFEnum(a, "text", "value");

                        this.Panel1.Controls.Add(control);
                    }
                }
                //else
                //{
                //    // 2018.10.02 Alex - Name absent in model - Formulas
                //    Controls.CriteriaNumber control = (Controls.CriteriaNumber)Page.LoadControl("~/Controls/CriteriaNumber.ascx");
                //    control.ColumnName = cn.ColumnName;
                //    control.DisplayName = cn.DisplayName;
                //    HiddenField hf = new HiddenField();
                //    hf.Value = cn.Formula;
                //    control.Controls.Add(hf);
                //    this.Panel1.Controls.Add(control);
                //}
            }
            // Alex 2018.09.20 - Begin
            //
            //
            //
            var prefilterParams = new List<CUSTOMRP.BLL.AppHelper.QueryParamsObject>();
            var QueryParams = CUSTOMRP.BLL.AppHelper.GetQueryParams(me.ID, me.DatabaseNAME, "qreport." + mySV.TBLVIEWNAME);
            //  mySV.TBLVIEWNAME may be a view or an sp

            //Controls.CriteriaQueryParamWrapper wrapper = (Controls.CriteriaQueryParamWrapper) Page.LoadControl("~/controls/CriteriaQueryParamWrapper.ascx");


            var dt = WebHelper.bllQUERYPARAMS.GetList(me.ID, myReport.ID).Tables[0].AsEnumerable().ToDictionary<DataRow, string, string>(rw => Convert.ToString(rw["NAME"]),
                                                            rw => Convert.ToString(rw["VALUE"]));
            foreach (var x in QueryParams)
            {
                //Label label2 = new Label();
                //label2.Text = me.DatabaseNAME + " qreport." + mySV.TBLVIEWNAME + QueryParams.Count.ToString();
                //this.Panel1.Controls.Add(label2);

                Controls.CriteriaQueryParam qpControl = (Controls.CriteriaQueryParam)Page.LoadControl("~/controls/CriteriaQueryParam.ascx");
                qpControl.ID = x.ParamName.Substring(12);
                qpControl.ColumnName = x.ParamName.Substring(12);                                           // Real
                qpControl.DisplayName = qpControl.ColumnName.Replace("_", " ");                             // Display

                switch (x.SqlType)
                {
                    case "bit":         // radio buttons
                        qpControl.ControlType = "bool";
                        break;
                    case "date":        // calendar
                    case "datetime":
                        qpControl.ControlType = "datetime";
                        break;
                    case "int":         // <%=this.ControlType=="int" ? "type='number'" : "" %>
                        qpControl.ControlType = "int";
                        if (x.ParamName.Substring(12).Contains('$'))
                        {
                            qpControl.ControlType = "enum";
                            int separator = x.ParamName.IndexOf('$');                                                   // Separator is the $ sign
                            qpControl.ColumnName = x.ParamName.Substring(12, separator - 12);                           // Real
                            qpControl.DisplayName = qpControl.ColumnName.Replace("_", " ");                             // Display
                            String[] chopped = x.ParamName.Substring(12).Split('$');
                            qpControl.EnumArr = CUSTOMRP.BLL.AppHelper.QueryParamGetEnumValues(me.ID, me.DatabaseNAME, chopped[1], chopped[2].Replace("#", " "));
                        }
                        break;
                    case "varchar":      // normal
                    case "nvarchar":
                        qpControl.ControlType = "string";
                        if (x.ParamName.Substring(12).Contains('$'))
                        {
                            qpControl.ControlType = "enum";
                            int separator = x.ParamName.IndexOf('$');                                                   // Separator is the $ sign
                            qpControl.ColumnName = x.ParamName.Substring(12, separator - 12);                           // Real
                            qpControl.DisplayName = qpControl.ColumnName.Replace("_", " ");                             // Display
                            String[] chopped = x.ParamName.Substring(12).Split('$');
                            qpControl.EnumArr = CUSTOMRP.BLL.AppHelper.QueryParamGetEnumValues(me.ID, me.DatabaseNAME, chopped[1], chopped[2].Replace("#", " "));
                        }
                        break;
                }

                x.Type = qpControl.ControlType;
                if (x.Type == "enum")
                {
                    x.ParamName = x.ParamName.Substring(12);
                }
                else
                {
                    x.ParamName = qpControl.ColumnName;
                }

                prefilterParams.Add(x);
                this.PlaceHolder_QueryParamsWrapper.Controls.Add(qpControl);

                if (dt.Keys.Contains(qpControl.ColumnName))
                {
                    qpControl.Value = dt[qpControl.ColumnName];
                }
                //wrapper.Panel_QueryParams.Controls.Add(qpControl);
            }
            //if (QueryParams.Count > 0)
            //{
            //    this.PlaceHolder_QueryParamsWrapper.Controls.Add(wrapper);
            //}
            Session[strSessionKeyQueryParams] = prefilterParams;
            //
            //
            //
            // Alex 2018.09.20 - End
        }

        private void ParseDataToContainer()
        {
            // 2018.09.21 Alex - QueryParams - Begin
            var modelList = (List<CUSTOMRP.BLL.AppHelper.QueryParamsObject>)Session[strSessionKeyQueryParams];
            var newList = new List<CUSTOMRP.BLL.AppHelper.QueryParamsObject>();

            foreach (var x in modelList)
            {
                //throw new Exception(x.paramName);
                //string checkboxVal = Request.Form[String.Format("{0}_select", x.ParamName)];
                //if (!String.IsNullOrEmpty(checkboxVal))
                //{
                string fieldVal;
                switch (x.Type)
                // 2018.09.21 Alex - Get by POST values (<name> tag) - Begin
                {
                    case "bool":
                        fieldVal = Request.Form[String.Format("{0}_optradio", x.ParamName)];
                        break;
                    case "enum":
                        fieldVal = Request.Form[String.Format("{0}_autocomplete", x.ParamName.Split('$')[0])];
                        break;
                    default:
                        fieldVal = Request.Form[String.Format("{0}_field", x.ParamName)];
                        break;
                    // 2018.09.21 Alex - Get by POST values (<name> tag) - End
                }
                if (!String.IsNullOrEmpty(fieldVal))
                {
                    x.Value = fieldVal;
                    newList.Add(x);
                }
                //}
            }
            container.queryParams = newList;

            // 2018.09.21 Alex - QueryParams - End

            for (int i = 0; i < container.criteriaColumn.Count(); i++)
            {
                Fields cn = container.criteriaColumn[i];
                ColumnInfo ci = columninfos.Where(x => x.ColName == cn.ColumnName).FirstOrDefault();

                if (myReport == null)
                {
                    myReport = container.GetReportModel(me, columninfos);
                }

                CUSTOMRP.Model.REPORTCOLUMN rc = myReport.ReportColumns.Where(x => x.COLUMNNAME == cn.ColumnName && x.ColumnFunc == REPORTCOLUMN.ColumnFuncs.Criteria).FirstOrDefault();
                if (rc == null)
                {
                    rc = new REPORTCOLUMN()
                    {
                        COLUMNNAME = cn.ColumnName,
                        DISPLAYNAME = ci.DisplayName,
                        COLUMNTYPE = 1,
                        COLUMNFUNC = 2,
                        HIDDEN = false,
                    };
                    myReport.ReportColumns.Add(rc);
                }
                rc.DATATYPE = (ci != null) ? ci.DataType : "String";
                string postData = Request.Form[String.Format("ContentPlaceHolder1_ctl{0:00}_Label1rd1", i)];  // (modelList == null || modelList.Count == 0) ? i : i + 1)
                if (!String.IsNullOrEmpty(postData))    // has value
                {
                    rc.CRITERIA2 = postData;
                    switch (postData)
                    {
                        case "r1":
                            {
                                rc.CRITERIA3 = Request.Form[String.Format("ContentPlaceHolder1_ctl{0:00}_Label1ddl1", i)];
                                rc.CRITERIA4 = Request.Form[String.Format("ContentPlaceHolder1_ctl{0:00}_Label1tb1", i)];

                                cn.Operator = rc.CRITERIA3;
                                cn.Value1 = rc.CRITERIA4;
                            }
                            break;
                        case "r2":
                            {
                                rc.CRITERIA3 = Request.Form[String.Format("ContentPlaceHolder1_ctl{0:00}_Label1tb2", i)];
                                rc.CRITERIA4 = Request.Form[String.Format("ContentPlaceHolder1_ctl{0:00}_Label1tb3", i)];

                                cn.Operator = "BETWEEN";
                                cn.Value1 = rc.CRITERIA3;
                                cn.Value2 = rc.CRITERIA4;
                            }
                            break;
                        case "r3":
                            {
                                rc.CRITERIA3 = "";
                                rc.CRITERIA4 = "";

                                cn.Operator = "EMPTY";
                                cn.Value1 = rc.CRITERIA3;
                                cn.Value2 = rc.CRITERIA4;
                            }
                            break;
                    }
                }
                ReportCriteria temp = rc;
                CUSTOMRP.BLL.AppHelper.ParseParam(me.ID, ref temp);
            }

            Session[rpexcel.strSessionKeyMyReport] = myReport;
            Session[rpexcel.strSessionKeyReportParameterContainer] = container;
        }

        private void DownloadFile(string path, string name)
        {
            try
            {
                string downloadFilename = Server.UrlEncode(name.Replace(' ', '_'));
                System.IO.FileInfo file = new System.IO.FileInfo(path);
                Response.Clear();
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                // 添加头信息，为"文件下载/另存为"对话框指定默认文件名
                //Response.AddHeader("Content-Disposition", "attachment; filename=" + downloadFilename);
                Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"; filename*=utf-8''{1}", downloadFilename, HttpUtility.UrlPathEncode(downloadFilename)));
                // 添加头信息，指定文件大小，让浏览器能够显示下载进度
                Response.AddHeader("Content-Length", file.Length.ToString());
                // 指定返回的是一个不能被客户端读取的流，必须被下载
                Response.ContentType = "application/ms-excel";
                // 把文件流发送到客户端
                Response.WriteFile(file.FullName);
                // 停止页面的执行
                // Response.End();     
                this.p_fSuppressRender = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert(" + ex.Message + ")</script>");
            }
        }
        //v1.2.0 Kim 2016.12.08 replace criteria str from sql str to readable text
        //private void getSql(ref List<string> cir, out string contens, out string sqlplus, out string criterias, out string sorton)
        private void getSql(out string contens, out string sqlplus, out string criterias, out string sorton)
        {
            // out string query_params,


            //v1.0.0 - Cheong - 2015/05/29 - Modification for formula fields
            //contens = "[" + String.Join("], [", container.contentColumn.Select(x => x.ColumnName).ToArray()) + "]";
            contens = String.Join(", ", container.contentColumn.Select(x => x.SelectStatement).ToArray());
            sorton = String.Empty;

            if (container.sortonColumn.Count() > 0)
            {
                sorton = "[" + String.Join("], [", container.sortonColumn.Select(x => x.ColumnName).ToArray()) + "]";   // It's good enough to use column alias for "sort by" even for formulas
            }

            sorton = string.IsNullOrEmpty(sorton) == true ? "" : " order by " + sorton;
            //v1.2.0 Kim 2016.12.08 replace criteria str from sql str to readable text
            string[] cir = myReport.ReportColumns.Where(x => x.ColumnFunc == REPORTCOLUMN.ColumnFuncs.Criteria && !String.IsNullOrWhiteSpace(x.CRITERIA1)).Select(x => x.CRITERIA1).ToArray();
            criterias = String.Concat(cir.ToArray());
            //cir = myReport.ReportColumns.Where(x => x.ColumnFunc == REPORTCOLUMN.ColumnFuncs.Criteria && !String.IsNullOrWhiteSpace(x.CRITERIA1)).Select(x => x.CRITERIA1).ToList();
            //criterias = String.Concat(cir.ToArray());

            sqlplus = CUSTOMRP.BLL.AppHelper.sql_plus(container.SVID, me);
        }
        #endregion
    }
}