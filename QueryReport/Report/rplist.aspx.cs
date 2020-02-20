using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.HPSF;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using QueryReport.Code;

namespace QueryReport
{
    public partial class rplist : LoginUserPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //v1.2.0 - Cheong - 2016/12/22 - Clear report related session variables when entering main page
            Session[rpexcel.strSessionKeyMyReport] = null;
            Session[rpexcel.strSessionKeyColumnInfo] = null;
            Session[rpexcel.strSessionKeyReportParameterContainer] = null;
            Session[rpexcel.strSessionKeyFormulaFields] = null;
            if (!IsPostBack)
            {
                DataTable dt = WebHelper.bllReport.GetlistByDisplay(me.ID, me.DatabaseID, me.ReportGroup, me.rp_view, me.ViewLevel);
                this.Repeater1.DataSource = dt;
                this.Repeater1.DataBind();
            }
        }

        protected void show(object sender, EventArgs e)
        {
            Button b1 = (Button)sender;
            if (me.rp_view)
            {
                int rpid = Int32.Parse(b1.ToolTip.Split(new char[] { '|' })[0]);
                int type = Int32.Parse(b1.ToolTip.Split(new char[] { '|' })[1]);
                if (1 == type)
                {
                    //v1.1.0 - Cheong - 2016/06/01 - Always goto criteria page and do not download excel/word report here
                    ////v1.0.0 - Cheong - 2016/01/06 - goto criteria page if at least 1 criteria exist
                    //List<CUSTOMRP.Model.REPORTCOLUMN> rc = WebHelper.bllReportColumn.getCriteriaColumnModelList(rpid);
                    //if (rc.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Criteria).Count() == 0)
                    //{
                    //    if (string.IsNullOrEmpty(Request.Form["Select" + rpid]) == false)
                    //    {
                    //        string formatOption = Request.Form["Select" + rpid];
                    //        if (formatOption.ToUpper() == "EXCEL")
                    //        {
                    //            this.ExcelExport(rpid);
                    //        }
                    //        else if (formatOption.ToUpper() == "PDF")
                    //        {
                    //            this.PDF(rpid);
                    //        }
                    //        else if (formatOption.ToUpper() == "ON SCREEN")
                    //        {
                    //            this.HtmlExport(rpid);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    Response.Redirect("rpexcel.aspx?id=" + rpid + "&cmd=run");
                    //}
                    Response.Redirect("rpexcel.aspx?id=" + rpid + "&cmd=run");
                }
                else
                {
                    //v1.1.0 - Cheong - 2016/06/01 - Always goto criteria page and do not download excel/word report here
                    //WordExport(rpid);
                    Response.Redirect("rpworddetail.aspx?id=" + rpid + "&cmd=run");
                }
            }
            else
            {
                Common.JScript.Alert(AppNum.ErrorMsg.accesserror);
                Common.JScript.GoHistory(-1);
                Response.End();
            }
        }

        //v1.7.0 Kim 2016.10.24 not using (ref to v1.1.0 - Cheong - 2016/06/01 - Always goto criteria page and do not download excel/word report here)
        //protected void ExcelExport(int rpid)
        //{
        //    //string sql = AppHelper.getSql(rpid, me);
        //    //DataTable dt = WebHelper.bllCommon.query(sql);

        //    CUSTOMRP.Model.REPORT myReport = WebHelper.bllReport.GetModel(me.ID, rpid);

        //    string rpName = myReport.REPORTNAME;
        //    string rpTitle = myReport.RPTITLE;

        //    List<string> comments = new List<string>();
        //    List<string> avgs = new List<string>();
        //    List<string> sums = new List<string>();
        //    List<string> groups = new List<string>();
        //    List<string> subtotal = new List<string>();
        //    List<string> subavg = new List<string>();
        //    List<string> subcount = new List<string>();
        //    List<string> count = new List<string>();

        //    //v1.2.0 - Cheong - 2016/07/04 - Add option to hide duplicate items
        //    //DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(ref comments, ref avgs, ref sums, ref groups, ref subtotal, ref subavg, ref subcount, ref count, me, rpid, true);
        //    DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(me.ID, ref comments, ref avgs, ref sums, ref groups, ref subtotal, ref subavg, ref subcount, ref count, me,
        //        rpid, myReport.fHideDuplicate);

        //    Dictionary<string, decimal> colwidths = myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content)
        //        .ToDictionary(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DisplayName,
        //        y => y.EXCEL_COLWIDTH);

        //    List<string> rptHeader = !String.IsNullOrEmpty(myReport.REPORT_HEADER) ? new List<string>(myReport.REPORT_HEADER.Split('\n')) : null;
        //    List<string> rptFooter = !String.IsNullOrEmpty(myReport.REPORT_FOOTER) ? new List<string>(myReport.REPORT_FOOTER.Split('\n')) : null;

        //    //v1.0.0 - Cheong - 2016/03/17 - Add formatting to criteria
        //    comments = comments.Select(x => x.Remove(0, 6).TrimEnd(')')).ToList();

        //    XSSFWorkbook XSSFworkbook = NPOIHelper.GetWorkbookFromDt(dt, rpTitle, myReport.EXTENDFIELD.Split(','), rptHeader ?? comments,
        //        myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content).ToDictionary(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DisplayName, y => y.DisplayName),
        //        avgs, sums, groups, subtotal, subavg, subcount, count, colwidths, myReport.PrintOrientation, myReport.PRINT_FITTOPAGE,
        //        rptFooter);

        //    string fileName = rpName + ".xlsx";
        //    string folder = PathHelper.getTempFolderName();
        //    if (!Directory.Exists(folder))
        //    {
        //        Directory.CreateDirectory(folder);
        //    }
        //    string filePath = folder + PathHelper.getSafePath(fileName);
        //    using (FileStream outFs = new FileStream(filePath, FileMode.Create))
        //    {
        //        XSSFworkbook.Write(outFs);
        //        //CA2202
        //        //outFs.Close();
        //    }
        //    DownloadFile(filePath, fileName);
        //}

        //v1.7.0 Kim 2016.10.24 not using (ref to v1.1.0 - Cheong - 2016/06/01 - Always goto criteria page and do not download excel/word report here)
        //protected void WordExport(int rpid)
        //{
        //    CUSTOMRP.Model.REPORT myReport = WebHelper.bllReport.GetModel(me.ID, rpid);
        //    string WordFilePath = null;
        //    if ((myReport.WordFile != null) && (File.Exists(g_Config["WordTemplatePath"] + myReport.WordFile.WordFileName)))
        //    {
        //        WordFilePath = myReport.WordFile.WordFileName;
        //    }
        //    else
        //    {
        //        CUSTOMRP.Model.WORDTEMPLATE template = WebHelper.bllWORDTEMPLATE.GetModelByReportID(me.ID, myReport.ID, me.ID);
        //        if (template != null)
        //        {
        //            if (File.Exists(g_Config["WordTemplatePath"] + template.TemplateFileName))
        //            {
        //                WordFilePath = template.TemplateFileName;
        //            }
        //        }
        //    }

        //    if (WordFilePath == null)
        //    {
        //        Response.Write(String.Format("<script type=\"text/javascript\">alert(\"{0}\")</script>",
        //            AppNum.ErrorMsg.filenotfounderror));
        //        return;
        //    }

        //    CUSTOMRP.Model.SOURCEVIEW mySV = WebHelper.bllSOURCEVIEW.GetModel(me.ID, myReport.SVID);
        //    CUSTOMRP.Model.WORDTEMPLATE myTemplate = WebHelper.bllWORDTEMPLATE.GetModelByReportID(me.ID, rpid, me.ID);
        //    string[] colnames;

        //    switch (mySV.SourceType)
        //    {
        //        case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.StoredProc:
        //            {
        //                colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForStoredProc(me.ID, me.DatabaseNAME, mySV.TBLVIEWNAME);
        //            }
        //            break;
        //        default:
        //            {
        //                colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForTblView(me.ID, me.DatabaseNAME, mySV.TBLVIEWNAME);
        //            }
        //            break;
        //    }

        //    string rpName = myReport.REPORTNAME;
        //    string rpTitle = myReport.RPTITLE;

        //    List<string> comments = new List<string>();
        //    List<string> avgs = new List<string>();
        //    List<string> sums = new List<string>();
        //    List<string> groups = new List<string>();
        //    List<string> subtotal = new List<string>();
        //    List<string> subavg = new List<string>();
        //    List<string> subcount = new List<string>();
        //    List<string> count = new List<string>();

        //    //v1.2.0 - Cheong - 2016/07/04 - Add option to hide duplicate items
        //    //DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(ref comments, ref avgs, ref sums, ref groups, ref subtotal, ref subavg, ref subcount, ref count, me, rpid, true);
        //    DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(me.ID, ref comments, ref avgs, ref sums, ref groups, ref subtotal, ref subavg, ref subcount, ref count, me,
        //        rpid, myReport.fHideDuplicate);

        //    string path = g_Config["WordTemplatePath"] + WordFilePath;
        //    string downloadFilename = rpName.Replace(' ', '_') + ".docx";

        //    using (MemoryStream filestream = MailMerge.PerformMailMergeFromTemplate(path, dt, colnames))
        //    {
        //        Context.Response.ContentType = "application/octet-stream";
        //        //中文文件名需要UTF8编码
        //        //Context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + downloadFilename + "\"");
        //        Context.Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"; filename*=utf-8''{1}", downloadFilename, HttpUtility.UrlPathEncode(downloadFilename)));
        //        Context.Response.AddHeader("Content-Length", filestream.Length.ToString());
        //        byte[] fileBuffer = new byte[filestream.Length];
        //        filestream.Read(fileBuffer, 0, (int)filestream.Length);
        //        //CA2202
        //        //filestream.Close();
        //        Context.Response.BinaryWrite(fileBuffer);
        //        Context.Response.End();
        //    }
        //}

        protected void HtmlExport(int rpid)
        {
            Response.Redirect("htmlexport.aspx?rpid=" + rpid, true);
        }

        protected void PDF(int rpid)
        {
            CUSTOMRP.Model.REPORT myReport = WebHelper.bllReport.GetModel(me.ID, rpid);

            string rpName = myReport.REPORTNAME;
            string rpTitle = myReport.RPTITLE;

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

            string[] rptHeader = !String.IsNullOrEmpty(myReport.REPORT_HEADER) ? myReport.REPORT_HEADER.Split('\n') : null;
            string[] rptFooter = !String.IsNullOrEmpty(myReport.REPORT_FOOTER) ? myReport.REPORT_FOOTER.Split('\n') : null;

            //v1.0.0 - Cheong - 2016/03/17 - Add formatting to criteria
            comments = comments.Select(x => x.Remove(0, 6).TrimEnd(')')).ToList();

            string fontPath = PathHelper.getFontFolderName() + "simsun.ttc,1";

            bool showChangeOnly = myReport.EXTENDFIELD.Split(',')[CUSTOMRP.Model.REPORT.EXTENDFIELDs.ReportType] == CUSTOMRP.Model.REPORT.ExtReportType.ChangeOnly;
            bool hideHeaders = myReport.EXTENDFIELD.Split(',')[CUSTOMRP.Model.REPORT.EXTENDFIELDs.ReportType] == CUSTOMRP.Model.REPORT.ExtReportType.DataExport;
            bool hideCriteria = myReport.EXTENDFIELD.Split(',')[CUSTOMRP.Model.REPORT.EXTENDFIELDs.HideCriteria] == "1";

            Common.MyPdf.exp_Pdf(showChangeOnly, hideHeaders, hideCriteria,
                myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content).ToDictionary(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DisplayName, y => y.DisplayName),
                subtotal, subavg, subcount, count, groups, avgs, sums, AppNum.companyName, rptHeader ?? comments.ToArray(), dt, rpTitle,
                fontPath, 14, 1, iTextSharp.text.BaseColor.BLACK,
                fontPath, 11, 1, iTextSharp.text.BaseColor.BLACK,//列头字体、大小、样式、颜色
                PathHelper.getTempFolderName(), rpName,
                fontPath, 11, 1, iTextSharp.text.BaseColor.BLACK,//正文字体、大小、样式、颜色
                rptFooter);
        }

        protected void EDIT(object sender, EventArgs e)
        {
            Button b1 = (Button)sender;
            int rpid = Int32.Parse(b1.ToolTip.Split(new char[] { '|' })[0]);
            int type = Int32.Parse(b1.ToolTip.Split(new char[] { '|' })[1]);
            if (type == 1)
            {
                if (me.rp_modify)
                {
                    Response.Redirect("rpexcel.aspx?id=" + rpid);
                }
                else
                {
                    Common.JScript.Alert(AppNum.ErrorMsg.accesserror);
                    Common.JScript.GoHistory(-1);
                    Response.End();
                }
            }
            else
            {
                if (me.rp_modify)
                {
                    Response.Redirect("rpworddetail.aspx?id=" + rpid);
                }
                else
                {
                    Common.JScript.Alert(AppNum.ErrorMsg.accesserror);
                    Common.JScript.GoHistory(-1);
                    Response.End();
                }
            }
        }

        public void DownloadFile(string path, string name)
        {
            System.IO.FileInfo file = null;
            try
            {
                file = new System.IO.FileInfo(path);
                string downloadFilename = Server.UrlEncode(name.Replace(' ', '_'));
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

        protected void btnNew_Click(object sender, EventArgs e)
        {
            if (me.rp_add)
            {
                Response.Redirect("rpSelectType.aspx");
            }
            else
            {
                Common.JScript.Alert(AppNum.ErrorMsg.accesserror);
                Common.JScript.GoHistory(-1);
                Response.End();
            }
        }
    }
}