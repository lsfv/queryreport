using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using QueryReport.Code;

namespace QueryReport
{
    public partial class rpworddetail : LoginUserPage
    {
        private CUSTOMRP.Model.REPORT myReport = null;
        private CUSTOMRP.Model.SOURCEVIEW mySV = null;
        private List<CUSTOMRP.Model.ColumnInfo> columninfos = null;
        private List<CUSTOMRP.Model.REPORTCOLUMN> selectedColumns = null;   // not to be mixed from the columns in the database report records
        private List<CUSTOMRP.Model.REPORTCOLUMN> formulaFields = null;

        private bool p_fSuppressRender = false;     // Whether to render page contents

        #region Event Handlers

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session[rpexcel.strSessionKeyColumnInfo] != null)
            {
                columninfos = (List<CUSTOMRP.Model.ColumnInfo>)Session[rpexcel.strSessionKeyColumnInfo];
            }

            if (Session[rpexcel.strSessionKeyFormulaFields] != null)
            {
                formulaFields = (List<CUSTOMRP.Model.REPORTCOLUMN>)Session[rpexcel.strSessionKeyFormulaFields];
            }
            else
            {
                formulaFields = new List<CUSTOMRP.Model.REPORTCOLUMN>();
                Session[rpexcel.strSessionKeyFormulaFields] = formulaFields;
            }

            if ((myReport == null) && (Session[rpexcel.strSessionKeyMyReport] != null))
            {
                myReport = (CUSTOMRP.Model.REPORT)Session[rpexcel.strSessionKeyMyReport];
            }

            if (!this.IsPostBack)
            {
                if (!String.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    int id = Int32.Parse(Request.QueryString["id"]);
                    myReport = WebHelper.bllReport.GetModel(me.ID, id);

                    this.btnDelete.Visible = true;
                    if (myReport == null)
                    {
                        Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.parameter_error, "rplist.aspx");
                        Response.End();
                    }
                    else
                    {
                        Session[rpexcel.strSessionKeyMyReport] = myReport;
                        CUSTOMRP.BLL.SOURCEVIEW svBLL = new CUSTOMRP.BLL.SOURCEVIEW();
                        mySV = svBLL.GetModel(me.ID, myReport.SVID);
                    }
                }
                else
                {
                    // creating new report
                    myReport = (CUSTOMRP.Model.REPORT)Session[rpexcel.strSessionKeyMyReport];
                    columninfos = (List<CUSTOMRP.Model.ColumnInfo>) Session[rpexcel.strSessionKeyColumnInfo] ;

                }
            }
            else
            {
                // v1.2.0 Kim 2016.11.22 do this both IsPostBack or not
                //if ((myReport == null) && (Session[rpexcel.strSessionKeyMyReport] != null))
                //{
                //    myReport = (CUSTOMRP.Model.REPORT)Session[rpexcel.strSessionKeyMyReport];
                //}

                if (!String.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    int id = Int32.Parse(Request.QueryString["id"]);

                    if (Session[rpexcel.strSessionKeyReportParameterContainer] != null)
                    {
                        ReportParameterContainer container = (ReportParameterContainer)Session[rpexcel.strSessionKeyReportParameterContainer];
                        CUSTOMRP.Model.REPORT rptFromContainer = container.GetReportModel(me, columninfos, id);

                        //v1.7.0 - Cheong - 2016/07/04 - Since we also need to receive setting data from rpword.aspx, instead of directly assigning
                        // report object decoded, we try to merge settings here instead.
                        #region Merge myReport data

                        if (myReport == null)
                        {
                            myReport = rptFromContainer;
                        }
                        else
                        {
                            myReport.REPORTNAME = rptFromContainer.REPORTNAME;
                            myReport.CATEGORY = rptFromContainer.CATEGORY;
                            myReport.REPORTGROUPLIST = rptFromContainer.REPORTGROUPLIST;
                            myReport.RPTITLE = rptFromContainer.RPTITLE;
                            myReport.EXTENDFIELD = rptFromContainer.EXTENDFIELD;
                        }

                        #endregion Merge myReport data
                    }
                }
            }

            if (!IsPostBack)
            {
                if (myReport != null)
                {
                    selectedColumns = myReport.ReportColumns;
                }
                else
                {
                    selectedColumns = new List<CUSTOMRP.Model.REPORTCOLUMN>();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ProcessAjaxRequestParameters()) { return; } // stop further process if it's Ajax event

            this.lblJavascript.Text = String.Empty;

            if (!IsPostBack)
            {
                this.FillPageHeaderUI();
            }

            #region get column names

            string[] colnames = null;
            if (mySV == null)
            {
                mySV = WebHelper.bllSOURCEVIEW.GetModel(me.ID, Int32.Parse(this.ddlQueryName.SelectedValue));
            }
            colnames = WebHelper.bllSOURCEVIEWCOLUMN.GetModelsForSourceView(me.ID, mySV.ID).OrderBy(x => x.DisplayName).Select(x => x.DisplayName).ToArray();
            if (colnames == null)
            {
                switch (mySV.SourceType)
                {
                    case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.View:
                    case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.Table:
                        {
                            colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForTblView(me.ID, me.DatabaseNAME, mySV.TBLVIEWNAME);
                        }
                        break;
                    case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.StoredProc:
                        {
                            colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForStoredProc(me.ID, me.DatabaseNAME, mySV.TBLVIEWNAME);
                        }
                        break;
                }
            }

            #endregion

            if (selectedColumns != null)
            {
                foreach (CUSTOMRP.Model.REPORTCOLUMN dr in selectedColumns)
                {
                    if (dr.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Formula)
                    {
                        this.formulaFields.Add(dr);
                        //this.lbFormula.Items.Add(new ListItem(dr.DisplayName));

                        switch (dr.ColumnFunc)
                        {
                            case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content:
                                {
                                    this.lbcontents.Items.Add(new ListItem(dr.DisplayName));
                                }
                                break;
                            case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Criteria:
                                {
                                    this.lbcriteria.Items.Add(new ListItem(dr.DisplayName));
                                }
                                break;
                            case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.SortOn:
                                {
                                    this.lbsorton.Items.Add(new ListItem(dr.DisplayName));
                                }
                                break;

                        }
                    }
                    else
                    {
                        //v1.0.0 - Cheong - 2015/03/31 - Filter missing columns from list when running reports
                        if (colnames.Contains(dr.COLUMNNAME))
                        {
                            switch (dr.ColumnFunc)
                            {
                                case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content:
                                    {
                                        if ((dr.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal) && (!dr.COLUMNNAME.Equals(dr.DISPLAYNAME)))
                                        {
                                            this.lbcontents.Items.Add(new ListItem(String.Format("{0} = [{1}]", dr.DisplayName, dr.COLUMNNAME), dr.COLUMNNAME));
                                        }
                                        else
                                        {
                                            this.lbcontents.Items.Add(new ListItem(dr.DisplayName, dr.COLUMNNAME));
                                        }
                                    }
                                    break;
                                case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Criteria:
                                    {
                                        this.lbcriteria.Items.Add(new ListItem(dr.DisplayName, dr.COLUMNNAME));
                                    }
                                    break;
                                case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.SortOn:
                                    {
                                        this.lbsorton.Items.Add(new ListItem(dr.DisplayName, dr.COLUMNNAME));
                                    }
                                    break;
                            }
                        }
                    }
                }

                // After formula fields are loaded, save it to session
                Session[rpexcel.strSessionKeyFormulaFields] = formulaFields;
            }

            //v1.0.0 - Cheong - 2015/03/31 if no right to modify, go directly to rpwordsave.aspx
            if ((!me.rp_modify) || (Request.Params["cmd"] == "run"))
            {
                //v1.2.0 Kim 2016.12.08 clear Container session to force rpwordsave.aspx load from report setting if go directly
                Session[rpexcel.strSessionKeyReportParameterContainer] = null;
                Server.Transfer("rpwordsave.aspx", true);
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.p_fSuppressRender)
            {
                base.Render(writer);
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["id"] == null)
            {
                if (WebHelper.bllReport.GetList(me.ID, " reportname='" + this.txtReportName.Text + "' AND DATABASEID='" + me.DatabaseID + "'").Tables[0].Rows.Count > 0)
                {
                    Common.JScript.Alert(AppNum.ErrorMsg.Commonexits);
                    Common.JScript.GoHistory(-1);
                    Response.End();
                }
            }

            //ReportParameterContainer container = (ReportParameterContainer)Session[rpexcel.strSessionKeyReportParameterContainer];

            //foreach (CUSTOMRP.Model.REPORTCOLUMN c in myReport.ReportColumns)
            //{
            //    if (c.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content && c.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Formula)
            //    {
            //        Fields f = container.contentColumn.Where(x => x.ColumnName == c.COLUMNNAME).FirstOrDefault();
            //        if (f != null)
            //        {
            //            c.COLUMNCOMMENT = f.Formula;
            //        }
            //    }
            //}

            if (myReport != null)
            {
                myReport.REPORTNAME = this.txtReportName.Text.Trim();
            }

            Session[rpexcel.strSessionKeyMyReport] = myReport;
            Server.Transfer("rpwordsave.aspx", true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (me.rp_delete)
            {
                if (myReport != null)
                {
                    string strrpid = Request.QueryString["id"];
                    int rpid;
                    if (Int32.TryParse(strrpid, out rpid))
                    {
                        //string sql_delete = "delete from [report] where [id]=" + rpid + "; delete from [REPORTCOLUMN] where [rpid]=" + rpid;
                        //WebHelper.bllCommon.executesql(sql_delete);
                        WebHelper.bllReport.Delete(me.ID, rpid);
                    }
                }
                Response.Redirect("rplist.aspx", false);
            }
            else
            {
                Common.JScript.Alert(AppNum.ErrorMsg.accesserror);
                Common.JScript.GoHistory(-1);
                Response.End();
            }
        }

        protected void ddlQueryName_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ddlQueryNameChange();
        }

        protected void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            string path = g_Config["WordTemplatePath"];
            string filenamewithoutextension = null;
            string datafilename = null;
            string originalfilename = null;
            if ((myReport.WordFile != null) && File.Exists(path + myReport.WordFile.WordFileName))
            {
                filenamewithoutextension = Path.GetFileNameWithoutExtension(myReport.WordFile.WordFileName);
                if (filenamewithoutextension.LastIndexOf('.') > 0)
                {
                    filenamewithoutextension = filenamewithoutextension.Substring(0, filenamewithoutextension.LastIndexOf('.'));
                }
                datafilename = filenamewithoutextension + ".xlsx";
                originalfilename = myReport.WordFile.OrigFileName;
            }
            else
            {
                int svid = Convert.ToInt32(this.ddlQueryName.SelectedValue);
                CUSTOMRP.Model.WORDTEMPLATE wordtemplate = WebHelper.bllWORDTEMPLATE.GetModelBySVID(me.ID, svid, me.ID);
                if (wordtemplate != null)
                {
                    filenamewithoutextension = Path.GetFileNameWithoutExtension(wordtemplate.TemplateFileName);
                    datafilename = wordtemplate.DataFileName;
                    originalfilename = wordtemplate.TemplateFileName;
                }
            }

            if (filenamewithoutextension == null)
            {
                this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}\");</script>", AppNum.ErrorMsg.filenotfounderror);
                return;
            }

            MemoryStream filestream = MailMerge.ChangeDataFilePath(path + myReport.WordFile.WordFileName, datafilename);
            Context.Response.ContentType = "application/octet-stream";
            //Encode filename according to RFC5987
            //Response.AddHeader("Content-Disposition", "attachment; filename=\"" + originalfilename + "\"");
            Context.Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"; filename*=utf-8''{1}", originalfilename, HttpUtility.UrlPathEncode(originalfilename)));
            Context.Response.AddHeader("Content-Length", filestream.Length.ToString());
            byte[] fileBuffer = new byte[filestream.Length];
            filestream.Read(fileBuffer, 0, (int)filestream.Length);
            filestream.Close();
            Context.Response.BinaryWrite(fileBuffer);
            p_fSuppressRender = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        protected void btnDownloadDatafile_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.txtReportName.Text))
            {
                this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}\");</script>",
                    String.Format(AppNum.ErrorMsg.fieldcannotbeempty, "Report Name"));
                return;
            }

            #region Get ColumnInfo
            CUSTOMRP.Model.SOURCEVIEW sv = WebHelper.bllSOURCEVIEW.GetModel(me.ID, Int32.Parse(this.ddlQueryName.SelectedValue));
            List<CUSTOMRP.Model.SOURCEVIEWCOLUMN> svclist = WebHelper.bllSOURCEVIEWCOLUMN.GetModelsForSourceView(me.ID, sv.ID).OrderBy(x => x.DisplayName).ToList();
            string[] colnames = svclist.Select(x => x.DisplayName).ToArray();

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
                        if (colnames == null)
                        {
                            colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForTblView(me.ID, me.DatabaseNAME, sv.TBLVIEWNAME);
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
                        if (colnames == null)
                        {
                            colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForStoredProc(me.ID, me.DatabaseNAME, sv.TBLVIEWNAME);
                        }
                    }
                    break;
            }
            // Filter result to only columns that is requested
            columninfos = columninfos.Where(x => colnames.Contains(x.ColName)).ToList();
            #endregion Get ColumnInfo

            StringBuilder selectclause = new StringBuilder("TOP 5 ");

            //v1.2.0 Fai 2017.01.11 - Avoid to throw Exception if columninfos have no any items - Begin
            //selectclause.Append(String.Join(",", columninfos.Select(x => "[" + x.DisplayName + "]").ToArray()));
            if (columninfos.Select(x => "[" + x.DisplayName + "]").ToArray().Length > 0)
                selectclause.Append(String.Join(",", columninfos.Select(x => "[" + x.DisplayName + "]").ToArray()));
            else
                selectclause.Append("*");
            //v1.2.0 Fai 2017.01.11 - Avoid to throw Exception if columninfos have no any items - End

            //v1.2.0 - Cheong - 2016/07/04 - Add option to hide duplicate items
            //DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(sv.ID, me.DatabaseNAME, selectclause.ToString(), CUSTOMRP.BLL.AppHelper.sql_plus(sv.ID, me), "", "", true);
            DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(me.ID, sv.ID, me.DatabaseNAME, selectclause.ToString(), CUSTOMRP.BLL.AppHelper.sql_plus(sv.ID, me), "", "", myReport.fHideDuplicate);

            NPOI.XSSF.UserModel.XSSFWorkbook XSSFworkbook = NPOIHelper.GetWorkbookAsMailMergeDataSource(dt);

            string path = g_Config["WordTemplatePath"];
            string filenamewithoutextension = null;
            string datafilename = null;
            if ((myReport.WordFile != null) && File.Exists(path + myReport.WordFile.WordFileName))
            {
                filenamewithoutextension = Path.GetFileNameWithoutExtension(myReport.WordFile.WordFileName);
                if (filenamewithoutextension.LastIndexOf('.') > 0)
                {
                    filenamewithoutextension = filenamewithoutextension.Substring(0, filenamewithoutextension.LastIndexOf('.'));
                }
                datafilename = filenamewithoutextension + ".xlsx";
            }
            else
            {
                CUSTOMRP.Model.WORDTEMPLATE wordtemplate = WebHelper.bllWORDTEMPLATE.GetModelBySVID(me.ID, sv.ID, me.ID);
                if (wordtemplate != null)
                {
                    filenamewithoutextension = Path.GetFileNameWithoutExtension(wordtemplate.TemplateFileName);
                    datafilename = wordtemplate.DataFileName;
                }
            }

            if (datafilename == null)
            {
                this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}\");</script>", AppNum.ErrorMsg.parameter_error);
                return;
            }

            string folder = PathHelper.getTempFolderName();
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string filePath = folder + PathHelper.getSafePath(datafilename);
            try
            {
                using (FileStream outFs = new FileStream(filePath, FileMode.Create))
                {
                    XSSFworkbook.Write(outFs);
                    //CA2202
                    //outFs.Close();
                }

                FileInfo file = new System.IO.FileInfo(filePath);

                Response.ContentType = "application/octet-stream";
                //Encode filename according to RFC5987
                //Response.AddHeader("Content-Disposition", "attachment; filename=\"" + datafilename + "\"");
                Context.Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"; filename*=utf-8''{1}", datafilename, HttpUtility.UrlPathEncode(datafilename)));
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.WriteFile(file.FullName);
                Response.Flush();
                p_fSuppressRender = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        protected void btnUploadTemplate_Click(object sender, EventArgs e)
        {
            DateTime currTime = DateTime.Now;
            int UserID = me.ID;
            string path = g_Config["WordTemplatePath"];
            string strFileName = Path.GetFileNameWithoutExtension(fUploadTemplate.PostedFile.FileName).Replace(' ', '_') + DateTime.Now.ToString(".yyyyMMddHHmmss", CultureInfo.InvariantCulture) + ".docx";
            //v1.0.0 - Cheong - 2015/03/30 - Add code to create word template folder when not exist.
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            try
            {
                this.fUploadTemplate.PostedFile.SaveAs(path + strFileName);
            }
            catch
            {
                this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}\");</script>", AppNum.ErrorMsg.uploadFailed);
                return;
            }

            myReport.WordFile.Description = String.Format("Word file for {0}", myReport.RPTITLE);
            myReport.WordFile.OrigFileName = Path.GetFileName(fUploadTemplate.PostedFile.FileName).Replace(' ', '_');
            myReport.WordFile.WordFileName = strFileName;
            myReport.WordFile.ModifyDate = currTime;
            myReport.WordFile.ModifyUser = UserID;

            Session[rpexcel.strSessionKeyMyReport] = myReport;

            this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}\");</script>", AppNum.ErrorMsg.success);

        }

        #endregion

        #region Helper Methods

        private void FillPageHeaderUI()
        {
            this.FillddlQueryNames();

            DataTable mydtCategory = WebHelper.bllcategory.GetList(me.ID, 100000, "DATABASEID='" + me.DatabaseID + "'", "NAME").Tables[0];

            this.ddlCategory.DataSource = mydtCategory;
            this.ddlCategory.DataTextField = "NAME";
            this.ddlCategory.DataValueField = "ID";
            this.ddlCategory.DataBind();

            //v1.2.0 - Cheong - 2016/06/23 - Do not allow people to select report groups that they can't see
            //DataTable mydtRPGROUP = WebHelper.bllrpGroup.GetList(100000, "DATABASEID='" + me.DatabaseID + "'", "NAME").Tables[0];
            DataTable mydtRPGROUP = WebHelper.bllrpGroup.GetList(me.ID, 100000, "DATABASEID='" + me.DatabaseID + "' AND ID IN (" + me.ReportGroup + ")", "NAME").Tables[0];
            this.ddlReportGroup.DataSource = mydtRPGROUP;
            this.ddlReportGroup.DataTextField = "NAME";
            this.ddlReportGroup.DataValueField = "ID";
            this.ddlReportGroup.DataBind();

            CUSTOMRP.BLL.SOURCEVIEW bllsv = new CUSTOMRP.BLL.SOURCEVIEW();
            CUSTOMRP.Model.SOURCEVIEW sv = null;
            if (myReport != null)
            {
                sv = bllsv.GetModel(me.ID, myReport.SVID);
                this.txtReportName.Text = myReport.REPORTNAME;
                this.txtReportTitle.Text = myReport.RPTITLE;
                this.ddlFormat.SelectedValue = myReport.DEFAULTFORMAT.ToString();
                this.ddlQueryName.SelectedValue = myReport.SVID.ToString();
                //v1.2.0 - Cheong - 2016/07/04 - Add option to hide duplicate items
                this.chkHideDuplicate.Checked = myReport.fHideDuplicate;
                this.ddlCategory.SelectedValue = myReport.CATEGORY.ToString();
                this.ddlReportGroup.SelectedValue = myReport.REPORTGROUPLIST.ToString();
                //this.ddlPrintType.SelectedValue = myReport.EXTENDFIELD.ToString().Split(',')[0].ToString();
            }

            if (this.ddlQueryName.SelectedItem != null)
            {
                int svid = this.ddlQueryNameChange();
                sv = WebHelper.bllSOURCEVIEW.GetModel(me.ID, svid);
            }
            else { this.ddlQueryName.Items.Add(new ListItem("N/A", "")); }
        }

        private void FillddlQueryNames()
        {
            DataTable myDt = WebHelper.bllSOURCEVIEW.GetQueryListForDropdown(me, 2).Tables[0];

            this.ddlQueryName.DataSource = myDt;
            this.ddlQueryName.DataTextField = "NEWDesc";
            this.ddlQueryName.DataValueField = "ID";
            this.ddlQueryName.DataBind();
        }

        /// <summary>
        /// Update ListBox values
        /// </summary>
        /// <returns>SVID of selected query</returns>
        private int ddlQueryNameChange()
        {
            string[] colnames = null;
            CUSTOMRP.Model.SOURCEVIEW sv = WebHelper.bllSOURCEVIEW.GetModel(me.ID, Int32.Parse(this.ddlQueryName.SelectedValue));
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

            //v1.0.0 - Cheong - 2015/05/29 - Removed this line to hide unwanted columns from users
            // ad-hoc add all columns to SOURCEVIEWCOLUMN
            //WebHelper.bllSOURCEVIEWCOLUMN.AddColList(sv.ID, colnames);

            this.lbAllColumns.Items.Clear();
            this.lbAllColumns_master.Items.Clear();

            // changing query will make the formula useless, so purge them.
            //this.lbFormula.Items.Clear();
            this.formulaFields.Clear();
            Session.Remove(rpexcel.strSessionKeyFormulaFields);

            //v1.0.0 Fai 2015.03.19 - Order by Column Name
            if (svc.Count == 0)
            {
                // do sorting
                columninfos = columninfos.OrderBy(p => p.ColName).ToList();
            }
            else
            {
                // patch displayname and do sorting
                //v1.1.0 - Cheong - 2016/05/20 - Allow new columns be displayed in report
                //columninfos = (from c in columninfos
                //               join s in svc.Where(x => x.ColumnType == CUSTOMRP.Model.SOURCEVIEWCOLUMN.ColumnTypes.Normal) on c.ColName equals s.COLUMNNAME
                //               orderby s.DisplayName
                //               select new CUSTOMRP.Model.ColumnInfo()
                //               {
                //                   ColName = c.ColName,
                //                   DisplayName = String.IsNullOrEmpty(s.DISPLAYNAME) ? c.ColName : s.DISPLAYNAME, // no need to show actual column name here if DisplayName is supplied
                //                   DataType = c.DataType,
                //               }).ToList();

                columninfos = (from c in columninfos
                               join s in svc.Where(x => x.ColumnType == CUSTOMRP.Model.SOURCEVIEWCOLUMN.ColumnTypes.Normal) on c.ColName equals s.COLUMNNAME into lefts
                               from s in lefts.DefaultIfEmpty()
                               where (s == null || !s.HIDDEN)
                               orderby c.ColName //s.DisplayName,
                               select new CUSTOMRP.Model.ColumnInfo()
                               {
                                   ColName = c.ColName,
                                   DisplayName = (s == null || String.IsNullOrEmpty(s.DISPLAYNAME)) ? c.ColName : s.DISPLAYNAME, // no need to show actual column name here if DisplayName is supplied
                                   DataType = c.DataType,
                               }).ToList();
            }

            foreach (CUSTOMRP.Model.ColumnInfo col in columninfos)
            {
                ListItem option = new ListItem(col.DisplayName, col.ColName);
                option.Attributes.Add("data-datatype", col.DataType);
                this.lbAllColumns.Items.Add(option);
                //v1.6.8 - Cheong - 2016/05/26 - Add a master select box to hold values
                this.lbAllColumns_master.Items.Add(option);
            }

            // cache column type data in session
            Session[rpexcel.strSessionKeyColumnInfo] = columninfos;

            this.lbcontents.Items.Clear();
            this.lbcriteria.Items.Clear();
            this.lbsorton.Items.Clear();

            return sv.ID;
        }

        #endregion

        #region Ajax handling Methods

        private bool ProcessAjaxRequestParameters()
        {
            bool result = false;

            string p_strAction = (Request.Params["action"] ?? String.Empty).ToLower();

            switch (p_strAction)
            {
                case "pagesubmission":
                    {
                        this.AjaxPushSelectionToSession();
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private void AjaxPushSelectionToSession()
        {
            string strpayload = Request.Params["payload"];
            ReportParameterContainer payload = null;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ReportParameterContainer));
            try
            {
                payload = (ReportParameterContainer)serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(strpayload)));
                //v1.0.0 - Cheong - 2015/05/29 - Add code to place formula in selected fields here.
                //foreach (Fields f in payload.contentColumn)
                //{
                //    CUSTOMRP.Model.REPORTCOLUMN formula = this.formulaFields.Where(x => x.DISPLAYNAME == f.DisplayName).FirstOrDefault();
                //    if (formula != null)
                //    {
                //        f.Formula = formula.COLUMNCOMMENT;
                //    }
                //}

                Session[rpexcel.strSessionKeyReportParameterContainer] = payload;

                AjaxShowError("OK");
            }
            catch (Exception ex)
            {
                AjaxShowError(ex.Message);
            }
        }

        private void AjaxShowError(string message)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(string));
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, message);

            this.p_fSuppressRender = true;
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.ContentEncoding = Encoding.UTF8;
            ms.WriteTo(Response.OutputStream);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        #endregion
    }
}