using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QueryReport.Code;
using CUSTOMRP.Model;

//new    get report info to save.
//       get select column to show seri.
//edit   get report info to save
//       get select column to show seri. and set value to seri.
namespace QueryReport
{
    public partial class rpwordsave : QueryReport.Controls.RptSavePage
    {
        private CUSTOMRP.Model.REPORT myReport = null;
        private List<CUSTOMRP.Model.ColumnInfo> columninfos = null;

        // Alex 2018.09.21 QueryParams - Begin
        public const string strSessionKeyQueryParams = "__SESSION_REPORT_QueryParams";
        // Alex 2018.09.21 QueryParams - End

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session[rpexcel.strSessionKeyMyReport] != null)
            {
                myReport = (CUSTOMRP.Model.REPORT)Session[rpexcel.strSessionKeyMyReport];
                if (!String.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    int id = Int32.Parse(Request.QueryString["id"]);
                    if (myReport.ID != id) { myReport = null; } // not the same report
                }
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
            //v1.0.0 - Cheong - 2016/05/09 - rptitle cannot be null
            myReport.RPTITLE = myReport.REPORTNAME;
            myReport.TYPE = 2;
            //v1.0.0 - Cheong - 2015/03/31 if no right to modify, disable Save button
            if (!me.rp_modify)
            {
                this.btnSave.Visible = false;
            }
            else
            {
                if ((!this.IsPostBack) && (Request.Params["cmd"] != "run") && (myReport != null))
                {
                    //try
                    //{
                    //    WebHelper.bllReport.Replace(myReport);
                    //    Session[rpexcel.strSessionKeyMyReport] = myReport;
                    //}
                    //catch
                    //{
                    //    // ignore exception here, as user will try to save / run again anyway
                    //}

                    if (WebHelper.bllReport.Replace(myReport))
                    {
                        Session[rpexcel.strSessionKeyMyReport] = myReport;
                    }
                    else
                    {
                        this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}\");</script>", "Error occurred. Settings are not saved. Please go back and create again.");
                    };
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (myReport != null)
            {
                //v1.0.0 - Cheong - 2016/05/09 - rptitle cannot be null
                myReport.RPTITLE = myReport.REPORTNAME;
                myReport.TYPE = 2;
                if (WebHelper.bllReport.Replace(myReport))
                {
                    //v1.2.0 - Cheong - 2016/07/05 - Do not redirect to main page when report settings is saved successfully.
                    //v1.2.0 - Cheong - 2016/07/06 - Redirect if report mode is new because error will occur on subsequent click.
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
            }
            this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}\");</script>", "Error occurred. Settings are not saved.");
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                string WordFilePath = null;
                string downloadFilename = null;
                if ((myReport != null) && (myReport.WordFile != null) && (File.Exists(g_Config["WordTemplatePath"] + myReport.WordFile.WordFileName)))
                {
                    WordFilePath = myReport.WordFile.WordFileName;
                    downloadFilename = myReport.WordFile.OrigFileName;
                }
                else
                {
                    CUSTOMRP.Model.WORDTEMPLATE template = WebHelper.bllWORDTEMPLATE.GetModelByReportID(me.ID, myReport.ID, me.ID);
                    if (template != null)
                    {
                        if (File.Exists(g_Config["WordTemplatePath"] + template.TemplateFileName))
                        {
                            WordFilePath = template.TemplateFileName;
                            downloadFilename = myReport.RPTITLE + ".docx";
                        }
                    }
                }

                if (WordFilePath == null)
                {
                    Response.Write(String.Format("<script type=\"text/javascript\">alert(\"{0}\")</script>",
                        AppNum.ErrorMsg.filenotfounderror));
                    return;
                }

                string strSqlColumn, strSqlPlus, strSqlCriteria, strSqlSortOn;
                getSql(ref rpcr, out strSqlColumn, out strSqlCriteria, out strSqlPlus, out strSqlSortOn);

                if (String.IsNullOrWhiteSpace(strSqlColumn))
                {
                    #region Get Column Names

                    string[] colnames = null;
                    CUSTOMRP.Model.SOURCEVIEW sv = WebHelper.bllSOURCEVIEW.GetModel(me.ID, myReport.SVID);
                    List<CUSTOMRP.Model.SOURCEVIEWCOLUMN> svc = WebHelper.bllSOURCEVIEWCOLUMN.GetModelsForSourceView(me.ID, sv.ID, true).OrderBy(x => x.DisplayName).ToList();

                    switch (sv.SourceType)
                    {
                        case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.View:
                        case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.Table:
                            {
                                try
                                {
                                    columninfos = CUSTOMRP.BLL.AppHelper.GetColumnInfoForTblView(me.ID, me.DatabaseNAME, sv.TBLVIEWNAME);
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                                    columninfos = new List<CUSTOMRP.Model.ColumnInfo>();
                                    this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert('Error in retrieving columns for [{0}]. Please check view defination.');</script>", sv.TBLVIEWNAME);
                                }
                                if (svc == null)
                                {
                                    colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForTblView(me.ID, me.DatabaseNAME, sv.TBLVIEWNAME);
                                    // Filter result to only columns that is requested
                                    columninfos = columninfos.Where(x => colnames.Contains(x.ColName)).ToList();
                                }
                            }
                            break;
                        case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.StoredProc:
                            {
                                try
                                {
                                    columninfos = CUSTOMRP.BLL.AppHelper.GetColumnInfoForStoredProc(me.ID, me.DatabaseNAME, sv.ID);
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                                    columninfos = new List<CUSTOMRP.Model.ColumnInfo>();
                                    this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert('Error in retrieving columns for [{0}]. Please check stored proc defination.');</script>", sv.TBLVIEWNAME);
                                }
                                if (svc == null)
                                {
                                    colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForStoredProc(me.ID, me.DatabaseNAME, sv.TBLVIEWNAME);
                                    // Filter result to only columns that is requested
                                    columninfos = columninfos.Where(x => colnames.Contains(x.ColName)).ToList();
                                }
                            }
                            break;
                    }

                    if (svc.Count == 0)
                    {
                        // do sorting
                        columninfos = columninfos.OrderBy(p => p.ColName).ToList();
                    }
                    else
                    {
                        columninfos = (from c in columninfos
                                       join s in svc.Where(x => x.ColumnType == CUSTOMRP.Model.SOURCEVIEWCOLUMN.ColumnTypes.Normal) on c.ColName equals s.COLUMNNAME into lefts
                                       from s in lefts.DefaultIfEmpty()
                                       where (s == null || !s.HIDDEN)
                                       orderby s.DisplayName, c.ColName
                                       select new CUSTOMRP.Model.ColumnInfo()
                                       {
                                           ColName = c.ColName,
                                           DisplayName = (s == null || String.IsNullOrEmpty(s.DISPLAYNAME)) ? c.ColName : s.DISPLAYNAME, // no need to show actual column name here if DisplayName is supplied
                                           DataType = c.DataType,
                                       }).ToList();
                    }

                    strSqlColumn = String.Join(",", columninfos.Select(x => "[" + x.ColName + "]").ToArray());

                    #endregion
                }

                // v1.8.8 Alex 2018.09.24 - Overloaded getDataForReport to accept QueryParams - Begin
                var sqlParams_queryParams = new List<SqlParameter>();
                foreach (var x in container.queryParams)
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

                //v1.2.0 - Cheong - 2016/07/04 - Add option to hide duplicate items
                //this.rpdt = CUSTOMRP.BLL.AppHelper.getDataForReport(myReport.SVID, me.DatabaseNAME, strSqlColumn, strSqlPlus, strSqlCriteria, strSqlSortOn, true);
                this.rpdt = CUSTOMRP.BLL.AppHelper.getDataForReport(me.ID, myReport.SVID, me.DatabaseNAME, strSqlColumn, strSqlPlus, strSqlCriteria, strSqlSortOn, myReport.fHideDuplicate, sqlParams_queryParams);
                // v1.8.8 Alex 2018.09.24 - Overloaded getDataForReport to accept QueryParams - End

                if (container.Format == 1)
                {
                    Server.Transfer("htmlexport.aspx?active=html", true);
                }
                else if (container.Format == 0)
                {
                    string path = g_Config["WordTemplatePath"] + WordFilePath;
                    string fileName = container.ReportName + ".docx";

                    using (MemoryStream filestream = MailMerge.PerformMailMergeFromTemplate(path, rpdt, columninfos.Select(x => x.DisplayName).ToArray()))
                    {
                        Context.Response.ContentType = "application/octet-stream";
                        //Context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + downloadFilename + "\"");
                        Context.Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"; filename*=utf-8''{1}", downloadFilename, HttpUtility.UrlPathEncode(downloadFilename)));
                        Context.Response.AddHeader("Content-Length", filestream.Length.ToString());
                        byte[] fileBuffer = new byte[filestream.Length];
                        filestream.Read(fileBuffer, 0, (int)filestream.Length);
                        //CA2202
                        //filestream.Close();
                        Context.Response.BinaryWrite(fileBuffer);
                        Context.Response.End();
                    }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                // This is normal for Server.Transfer()
            }
#if DEBUG
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
#else
            catch (Exception ex)
            {
                Common.JScript.AlertAndRedirect("tEST", "rplist.aspx"); //++
                Response.End();
            }
#endif
        }

        public void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("rplist.aspx", true);
        }

        private void LoadCriteriaUIFromSession()
        {
            CUSTOMRP.Model.SOURCEVIEW mySV = WebHelper.bllSOURCEVIEW.GetModel(me.ID, container.SVID);

            // Alex 2018.09.20 - Begin
            //
            //
            //
            var prefilterParams = new List<CUSTOMRP.BLL.AppHelper.QueryParamsObject>();
            var QueryParams = CUSTOMRP.BLL.AppHelper.GetQueryParams(me.ID, me.DatabaseNAME, "qreport." + mySV.TBLVIEWNAME);
            //  mySV.TBLVIEWNAME may be a view or an sp

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

                //if (!prefilterParams.Contains(tmpModel))
                prefilterParams.Add(x);

                this.PlaceHolder_QueryParamsWrapper.Controls.Add(qpControl);
            }
            Session[strSessionKeyQueryParams] = prefilterParams;
            //
            //
            //
            // Alex 2018.09.20 - End

            foreach (Fields cn in container.criteriaColumn)
            {
                ColumnInfo columnInfo = columninfos.Where(x => x.ColName == cn.ColumnName).First();
                CUSTOMRP.Model.REPORTCOLUMN col = myReport.ReportColumns.Where(x => x.ColumnFunc == REPORTCOLUMN.ColumnFuncs.Criteria && x.COLUMNNAME == cn.ColumnName).FirstOrDefault();

                if ("String" == columnInfo.DataType)
                {
                    Controls.CriteriaString control = (Controls.CriteriaString)Page.LoadControl("~/controls/CriteriaString.ascx");
                    control.ColumnName = columnInfo.ColName;
                    control.DisplayName = columnInfo.DisplayName;

                    if ((!IsPostBack) && (col != null))
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
                    control.DisplayName = columnInfo.DisplayName;
                    control.ControlType = "datetime";

                    if ((!IsPostBack) && (col != null))
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
                    control.DisplayName = columnInfo.DisplayName;

                    if ((!IsPostBack) && (col != null))
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
                    control.DisplayName = columnInfo.DisplayName;
                    CUSTOMRP.Model.RpEnum rp = new RpEnum();
                    Type a = rp.GetType().GetNestedType((mySV.SOURCEVIEWNAME + "_" + cn).ToUpper());
                    control.dt = Common.Utils.GetTableFEnum(a, "text", "value");

                    this.Panel1.Controls.Add(control);
                }
            }
        }

        private void ParseDataToContainer()
        {
            // 2018.09.21 Alex - QueryParams - Begin
            var modelList = (List<CUSTOMRP.BLL.AppHelper.QueryParamsObject>)Session[strSessionKeyQueryParams];
            var newList = new List<CUSTOMRP.BLL.AppHelper.QueryParamsObject>();

            foreach (var x in modelList)
            {
                //throw new Exception(x.paramName);
                string checkboxVal = Request.Form[String.Format("{0}_select", x.ParamName)];
                if ((!String.IsNullOrEmpty(checkboxVal)) && checkboxVal.Equals("on"))
                {
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
                }
            }
            container.queryParams = newList;

            // 2018.09.21 Alex - QueryParams - End

            //v1.2.0 Kim 2016.11.18 handle remove criteriaColumn/sortonColumn
            myReport.ReportColumns = myReport.ReportColumns.Where(x =>
                (x.ColumnFunc != REPORTCOLUMN.ColumnFuncs.Criteria || container.criteriaColumn.Any(y => y.ColumnName == x.COLUMNNAME)) &&
                (x.ColumnFunc != REPORTCOLUMN.ColumnFuncs.SortOn || container.sortonColumn.Any(y => y.ColumnName == x.COLUMNNAME))
                ).ToList();

            if (myReport == null)
            {
                myReport = container.GetReportModel(me, columninfos);
            }

            for (int i = 0; i < container.criteriaColumn.Count(); i++)
            {
                Fields cn = container.criteriaColumn[i];
                ColumnInfo ci = columninfos.Where(x => x.ColName == cn.ColumnName).FirstOrDefault();

                //if (myReport == null)
                //{
                //    myReport = container.GetReportModel(me, columninfos);
                //}

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
                string postData = Request.Form[String.Format("ContentPlaceHolder1_ctl{0:00}_Label1rd1", i)];
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
                    }
                    switch (postData)
                    {
                        case "r2":
                            {
                                rc.CRITERIA3 = Request.Form[String.Format("ContentPlaceHolder1_ctl{0:00}_Label1tb2", i)];
                                rc.CRITERIA4 = Request.Form[String.Format("ContentPlaceHolder1_ctl{0:00}_Label1tb3", i)];

                                cn.Operator = "BETWEEN";
                                cn.Value1 = rc.CRITERIA3;
                                cn.Value2 = rc.CRITERIA4;
                            }
                            break;
                    }
                }
                ReportCriteria temp = rc;
                CUSTOMRP.BLL.AppHelper.ParseParam(me.ID, ref temp);
            }

            // v1.2.0 Kim 2016.11.25 handle create sort col
            for (int i = 0; i < container.sortonColumn.Count(); i++)
            {
                Fields cn = container.sortonColumn[i];
                ColumnInfo ci = columninfos.Where(x => x.ColName == cn.ColumnName).FirstOrDefault();

                CUSTOMRP.Model.REPORTCOLUMN rc = myReport.ReportColumns.Where(x => x.COLUMNNAME == cn.ColumnName && x.ColumnFunc == REPORTCOLUMN.ColumnFuncs.SortOn).FirstOrDefault();
                if (rc == null)
                {
                    rc = new REPORTCOLUMN()
                    {
                        COLUMNNAME = cn.ColumnName,
                        DISPLAYNAME = ci.DisplayName,
                        COLUMNTYPE = 1,
                        COLUMNFUNC = 3,
                        HIDDEN = false,
                    };
                    myReport.ReportColumns.Add(rc);
                }
            }

            Session[rpexcel.strSessionKeyMyReport] = myReport;
            Session[rpexcel.strSessionKeyReportParameterContainer] = container;
        }

        public void DownloadFile2(string path, string name)
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
                //HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert(" + ex.Message + ")</script>");
            }
        }

        protected void DownloadFile(string path, string name)
        {
            string downloadFilename = name.Replace(' ', '_');
            string fileName = path + name;
            FileStream fileStream = new FileStream(Server.MapPath(fileName), FileMode.Open);
            long fileSize = fileStream.Length;
            Context.Response.ContentType = "application/octet-stream";
            //Context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + downloadFilename  + "\"");
            Context.Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"; filename*=utf-8''{1}", downloadFilename, HttpUtility.UrlPathEncode(downloadFilename)));
            Context.Response.AddHeader("Content-Length", fileSize.ToString());
            byte[] fileBuffer = new byte[fileSize];
            fileStream.Read(fileBuffer, 0, (int)fileSize);
            fileStream.Close();
            Context.Response.BinaryWrite(fileBuffer);
            Context.Response.End();

        }

        private void getSql(ref List<string> cir, out string contens, out string sqlplus, out string criterias, out string sorton)
        {
            //v1.0.0 - Cheong - 2015/05/29 - Modification for formula fields
            //contens = "[" + String.Join("], [", container.contentColumn.Select(x => x.ColumnName).ToArray()) + "]";
            contens = String.Join(", ", container.contentColumn.Select(x => x.SelectStatement).ToArray());
            sorton = String.Empty;

            if (container.sortonColumn.Count() > 0)
            {
                sorton = "[" + String.Join("], [", container.sortonColumn.Select(x => x.ColumnName).ToArray()) + "]";   // It's good enough to use column alias for "sort by" even for formulas
            }

            sorton = string.IsNullOrEmpty(sorton) == true ? "" : " order by " + sorton;

            cir = myReport.ReportColumns.Where(x => x.ColumnFunc == REPORTCOLUMN.ColumnFuncs.Criteria && !String.IsNullOrWhiteSpace(x.CRITERIA1)).Select(x => x.CRITERIA1).ToList();

            criterias = String.Concat(cir.ToArray());

            sqlplus = CUSTOMRP.BLL.AppHelper.sql_plus(container.SVID, me);
        }

        #region old code
        //private void getSql(ref List<string> cir, out string contens, out string sqlplus, out string criterias, out string sorton)
        //{
        //    contens = "[" + String.Join("], [", mypre.contentColumn) + "]";
        //    sorton = String.Empty;

        //    if (mypre.sortonColumn.Count > 0)
        //    {
        //        sorton = "[" + String.Join("], [", mypre.sortonColumn) + "]";
        //    }

        //    ReportCriteria[] criteria = this.GetCriteria();
        //    CUSTOMRP.BLL.AppHelper.ParseParam(ref criteria);

        //    StringBuilder crical = new StringBuilder();
        //    foreach (ReportCriteria rc in criteria)
        //    {
        //        if (!String.IsNullOrWhiteSpace(rc.CRITERIA1))
        //        {
        //            crical.AppendLine(rc.CRITERIA1);
        //            cir.Add(rc.CRITERIA1);
        //        }
        //    }

        //    sqlplus = CUSTOMRP.BLL.AppHelper.sql_plus(mypre.svid, me);

        //    sorton = string.IsNullOrEmpty(sorton) == true ? "" : " order by " + sorton;
        //    criterias = crical.ToString();
        //}
        #endregion
    }
}