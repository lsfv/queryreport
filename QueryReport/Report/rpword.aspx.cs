using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using QueryReport.Code;

namespace QueryReport.Report
{
    public partial class rpword : LoginUserPage
    {
        private List<CUSTOMRP.Model.ColumnInfo> columninfos = null;

        private bool p_fSuppressRender = false;     // Whether to render page contents

        #region Event Handlers

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.p_fSuppressRender)
            {
                base.Render(writer);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.lblJavascript.Text = String.Empty;

            if (!IsPostBack)
            {
                this.FillPageHeaderUI();
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (!this.ValidateParameters()) { return; }

            CUSTOMRP.Model.REPORT myReport = null;
            List<CUSTOMRP.Model.ColumnInfo> columninfos = null;

            string path = g_Config["WordTemplatePath"];
            string savedFileName = String.Empty;
            try
            {
                savedFileName = this.SaveTemplateFile();
            }
            catch
            {
                this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}\");</script>", AppNum.ErrorMsg.uploadFailed);
                return;
            }

            string strReportName = this.txtReportName.Text.Trim();
            int svID = Convert.ToInt32(this.ddlQueryName.SelectedValue);

            #region Read CustomProps from file

            System.Collections.Specialized.NameValueCollection nvc = QueryReport.Code.MailMerge.ReadCustomPropsFromFile(path + savedFileName);
            if (String.IsNullOrWhiteSpace(strReportName))
            {
                strReportName = nvc["RptName"];
                if (strReportName != null)
                {
                    strReportName = strReportName.Trim();
                }
            }
            string strQueryName = nvc["QueryName"];
            if (!String.IsNullOrWhiteSpace(strQueryName))
            {
                CUSTOMRP.Model.SOURCEVIEW svFromFile = WebHelper.bllSOURCEVIEW.GetModelByQueryName(me.ID, strQueryName.Trim(), me.DatabaseID);
                svID = (svFromFile != null) ? svFromFile.ID : -1;
            }

            #endregion

            if (String.IsNullOrWhiteSpace(strReportName))
            {
                this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}\");</script>",
                    String.Format(AppNum.ErrorMsg.fieldcannotbeempty, "Report Name"));
                return;
            }

            if (svID == -1)
            {
                this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}\");</script>",
                    String.Format(AppNum.ErrorMsg.pleaseselectvalidvaluefrom, "Query Name"));
                return;
            }

            CUSTOMRP.Model.RPCATEGORY rpcat = WebHelper.bllcategory.GetModelList(me.ID, "DATABASEID='" + me.DatabaseID + "'").FirstOrDefault();
            CUSTOMRP.Model.REPORTGROUP rpgrp = WebHelper.bllrpGroup.GetModelList(me.ID, "DATABASEID='" + me.DatabaseID + "'").FirstOrDefault();

            #region Save columninfos

            CUSTOMRP.Model.SOURCEVIEW sv = WebHelper.bllSOURCEVIEW.GetModel(me.ID, svID);
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

            #endregion Save columninfos

            #region myReport
            DateTime currTime = DateTime.Now;
            int UserID = me.ID;

            myReport = new CUSTOMRP.Model.REPORT();
            myReport.REPORTNAME = strReportName;
            myReport.RPTITLE = myReport.REPORTNAME;
            myReport.DATABASEID = me.DatabaseID;
            myReport.SVID = svID;
            myReport.CATEGORY = rpcat != null ? rpcat.ID : -1;
            myReport.REPORTGROUPLIST = rpgrp != null ? rpgrp.ID : -1;
            myReport.DEFAULTFORMAT = 0;
            myReport.ADDUSER = UserID;
            myReport.AUDODATE = currTime;
            myReport.ADDUSER = UserID;

            myReport.ReportColumns = new List<CUSTOMRP.Model.REPORTCOLUMN>();
            foreach (CUSTOMRP.Model.ColumnInfo c in columninfos)
            {
                CUSTOMRP.Model.SOURCEVIEWCOLUMN svc = svclist.Where(x => x.COLUMNNAME == c.ColName).First();
                myReport.ReportColumns.Add(new CUSTOMRP.Model.REPORTCOLUMN()
                    {
                        COLUMNNAME = c.ColName,
                        COLUMNFUNC = (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content,
                        CRITERIA1 = String.Empty,
                        CRITERIA2 = String.Empty,
                        CRITERIA3 = String.Empty,
                        CRITERIA4 = String.Empty,
                        AUDODATE = currTime,
                        SOURCEVIEWCOLUMNID = svc.ID,
                        DISPLAYNAME = svc.DISPLAYNAME,
                        COLUMNTYPE = (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal,
                        COLUMNCOMMENT = String.Empty,
                        HIDDEN = false,
                    });
            }

            myReport.WordFile = new CUSTOMRP.Model.WORDFILE();
            myReport.WordFile.Description = String.Format("Word file for {0}", myReport.RPTITLE);
            myReport.WordFile.OrigFileName = Path.GetFileName(fUploadTemplate.PostedFile.FileName).Replace(' ', '_');
            myReport.WordFile.WordFileName = savedFileName;
            myReport.WordFile.CreateDate = currTime;
            myReport.WordFile.CreateUser = UserID;
            myReport.WordFile.ModifyDate = currTime;
            myReport.WordFile.ModifyUser = UserID;

            #endregion myReport

            Session[rpexcel.strSessionKeyMyReport] = myReport;
            Session[rpexcel.strSessionKeyColumnInfo] = columninfos;

            Response.Redirect("rpworddetail.aspx");
        }

        protected void btnDownloadRawTemplate_Click(object sender, EventArgs e)
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

            string srcfile = Server.MapPath("~/File/blank.docx");
            string downloadFilename = this.txtReportName.Text.Trim().Replace(' ', '_') + ".docx";
            using (MemoryStream filestream = QueryReport.Code.MailMerge.CreateBlankMailMergeTemplate(srcfile, this.txtReportName.Text.Trim(),
                sv.TBLVIEWNAME, columninfos.Select(x => x.DisplayName).ToArray()))
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
                p_fSuppressRender = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }

        protected void btnSampleDataFile_Click(object sender, EventArgs e)
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

            StringBuilder selectclause = new StringBuilder(" TOP 5 ");
            selectclause.Append(String.Join(",", columninfos.Select(x => "[" + x.DisplayName + "]").ToArray()));
            //v1.2.0 - Cheong - 2016/07/04 - Add option to hide duplicate items - do not get duplicate data when getting sample
            DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(me.ID, sv.ID, me.DatabaseNAME, selectclause.ToString(), CUSTOMRP.BLL.AppHelper.sql_plus(sv.ID, me), "", "", true);

            NPOI.XSSF.UserModel.XSSFWorkbook XSSFworkbook = NPOIHelper.GetWorkbookAsMailMergeDataSource(dt);

            string downloadFilename = this.txtReportName.Text.Trim().Replace(' ', '_') + ".xlsx";

            string folder = PathHelper.getTempFolderName();
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string filePath = folder + PathHelper.getSafePath(downloadFilename);
            try
            {
                using (FileStream outFs = new FileStream(filePath, FileMode.Create))
                {
                    XSSFworkbook.Write(outFs);
                    //outFs.Close();
                }

                FileInfo file = new System.IO.FileInfo(filePath);

                Response.ContentType = "application/octet-stream";
                //Response.AddHeader("Content-Disposition", "attachment; filename=\"" + downloadFilename + "\"");
                Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"; filename*=utf-8''{1}", downloadFilename, HttpUtility.UrlPathEncode(downloadFilename)));
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

        #endregion

        #region Private methods

        private void FillPageHeaderUI()
        {
            this.FillddlQueryNames();
        }

        private void FillddlQueryNames()
        {
            DataTable myDt = WebHelper.bllSOURCEVIEW.GetQueryListForDropdown(me, 2).Tables[0];

            this.ddlQueryName.DataSource = myDt;
            this.ddlQueryName.DataTextField = "NEWDesc";
            this.ddlQueryName.DataValueField = "ID";
            this.ddlQueryName.DataBind();
        }

        private bool ValidateParameters()
        {
            StringBuilder errmsg = new StringBuilder();
            bool result = true;

            if (!this.fUploadTemplate.HasFile)
            {
                errmsg.AppendFormat(AppNum.ErrorMsg.fieldcannotbeempty, "Template File");
                result = false;
            }
            else
            {
                String fileExtension = System.IO.Path.GetExtension(this.fUploadTemplate.FileName).ToLower();
                String[] allowedExtensions = { ".docx" };
                bool fileOK = false;
                for (int i = 0; i < allowedExtensions.Length; i++)
                {
                    if (fileExtension == allowedExtensions[i])
                    {
                        fileOK = true;
                    }
                }

                if (!fileOK)
                {
                    errmsg.AppendLine(AppNum.ErrorMsg.onlyWordFileIsAllowed);
                    result = false;
                }
            }

            this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}\");</script>", errmsg.ToString());
            return result;
        }

        private string SaveTemplateFile()
        {
            string path = g_Config["WordTemplatePath"];
            string strFileName = Path.GetFileNameWithoutExtension(fUploadTemplate.PostedFile.FileName).Replace(' ', '_') + DateTime.Now.ToString(".yyyyMMddHHmmss", CultureInfo.InvariantCulture) + ".docx";
            //v1.0.0 - Cheong - 2015/03/30 - Add code to create word template folder when not exist.
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            try
            {
                this.fUploadTemplate.SaveAs(path + strFileName);
            }
            catch
            {
                throw new SystemException(AppNum.ErrorMsg.uploadFailed);
            }
            return strFileName;
        }

        #endregion
    }
}