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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using NPOI.HPSF;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using CUSTOMRP.Model;
using QueryReport.Code;

//new    get report info to save.
//       get select column to show seri.
//edit   get report info to save
//       get select column to show seri. and set value to seri.

namespace QueryReport
{
    public partial class rpexcel2_bak : LoginUserPage
    {
        /*
        private const string strSessionRpExcel2_PreInfo = "__SESSION_RPEXCEL2_PREINFO";
        private const string strSessionRpExcel2_Controls = "__SESSION_RPEXCEL2_CONTROLS";

        private CUSTOMRP.BLL.COMMON bllcommon = new CUSTOMRP.BLL.COMMON();

        public preInfo mypre = new preInfo();
        private IList<control> myControls = new List<control>();

        private CUSTOMRP.Model.REPORT myReport = null;
        private IList<CUSTOMRP.Model.REPORTCOLUMN> selectColumns = null;
        public DataTable rpdt = null;
        public IList<string> rpcr = null;
        public IList<string> sums = null;
        public IList<string> avgs = null;
        public IList<string> groups = null;
        public IList<string> grouptotal = null;
        public IList<string> groupavg = null;
        public IList<string> groupcount = null;
        public IList<string> counts = null;

        public string rptitle;
        public string rpname;

        [Serializable]
        public struct preInfo
        {
            public string rpName;
            public List<string> contentColumn;
            public List<string> sortonColumn;
            public string svname;
            public int svid;
            public int categoryid;
            public string ReportTitle;
            public int Distribution;
            public List<string> criteriaColumn;
            public int defaultFormat;
            public string avgColumn;
            public string sumColumn;
            public string groupColumn;
            public string groupsumColumn;
            public string groupavgColumn;
            public string groupCountColumn;
            public string countColumn;
            public string showType;
        }

        [Serializable]
        private class control
        {
            public string controlTypes;
            public string columns;
            public string prefixs;
            public string dbScript { get; set; }
            public string op1 { get; set; }
            public string range1;
            public string range2;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["id"]) == false)
            {
                int id = Int32.Parse(Request.QueryString["id"]);
                myReport = WebHelper.bllReport.GetModel(id);
                selectColumns = WebHelper.bllReportColumn.GetModelList("rc.RPID=" + id);

                if (myReport == null)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.parameter_error, "rplist.aspx");
                    Response.End();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.SaveSessionState();
            }
            else
            {
                this.LoadSessionState();
                if (Request.Form[this.ddlFormat.UniqueID] != null)
                {
                    mypre.defaultFormat = Convert.ToInt32(Request.Form[this.ddlFormat.UniqueID]);
                    Session[strSessionRpExcel2_PreInfo] = mypre;
                }
            }
            this.ddlFormat.SelectedValue = Convert.ToString(mypre.defaultFormat);
            //v1.0.0 - Cheong - 2015/03/31 if no right to modify, disable Save button
            if (!me.rp_modify)
            {
                this.btnSave.Visible = false;
            }
        }

        //get list of custome control first.
        protected void btnSave_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < myControls.Count(); i++)
            {
                if ((myControls[i].controlTypes == "string") || (myControls[i].controlTypes == "datetime"))
                {
                    string rb = Request.Form[myControls[i].prefixs + "rd1"];
                    if (rb == "r1")
                    {
                        myControls[i].op1 = "r1";
                        myControls[i].range1 = Request.Form[myControls[i].prefixs + "ddl1"];
                        myControls[i].range2 = Request.Form[myControls[i].prefixs + "tb1"];
                        switch (Request.Form[myControls[i].prefixs + "ddl1"])
                        {
                            case "Begins With":
                                {
                                    myControls[i].dbScript = " and [" + myControls[i].columns + "] like \'" + Request.Form[myControls[i].prefixs + "tb1"] + "%\'";
                                    break;
                                }
                            case "Contains":
                                {
                                    myControls[i].dbScript = " and [" + myControls[i].columns + "] like \'%" + Request.Form[myControls[i].prefixs + "tb1"] + "%\'";

                                    break;
                                }
                            case "=":
                                {
                                    myControls[i].dbScript = " and [" + myControls[i].columns + "] = \'" + Request.Form[myControls[i].prefixs + "tb1"] + "\'";
                                    break;
                                }
                            case "<=":
                                {
                                    myControls[i].dbScript = " and [" + myControls[i].columns + "] <= \'" + Request.Form[myControls[i].prefixs + "tb1"] + "\'";
                                    break;
                                }
                            case ">=":
                                {
                                    myControls[i].dbScript = " and [" + myControls[i].columns + "] >= \'" + Request.Form[myControls[i].prefixs + "tb1"] + "\'";
                                    break;
                                }
                            case "Does not contain":
                                {
                                    myControls[i].dbScript = " and [" + myControls[i].columns + "] not like \'%" + Request.Form[myControls[i].prefixs + "tb1"] + "%\'";
                                    break;
                                }
                            case "Does not equal":
                                {
                                    myControls[i].dbScript = " and [" + myControls[i].columns + "] <> \'" + Request.Form[myControls[i].prefixs + "tb1"] + "\'";
                                    break;
                                }
                        }
                    }
                    else if(rb == "r2")
                    {
                        string b1, b2;
                        b1 = Request.Form[myControls[i].prefixs + "tb2"];
                        b2 = Request.Form[myControls[i].prefixs + "tb3"];
                        myControls[i].dbScript = " and [" + myControls[i].columns + "] between \'" + b1 + "\' and  \'" + b2 + "\' ";

                        myControls[i].op1 = "r2";
                        myControls[i].range1 = b1;
                        myControls[i].range2 = b2;
                    }
                    else if (rb == "r3")
                    {
                        myControls[i].dbScript = " and ISNULL([" + myControls[i].columns + "],N'0')='0' or " + myControls[i].columns + " =\'\'";
                        myControls[i].op1 = "r3";
                    }
                }

                else if (myControls[i].controlTypes == "int")
                {
                    string rb = Request.Form[myControls[i].prefixs + "rd1"];
                    if (rb == "r1")
                    {
                        myControls[i].op1 = "r1";
                        myControls[i].range1 = Request.Form[myControls[i].prefixs + "ddl1"];
                        myControls[i].range2 = Request.Form[myControls[i].prefixs + "tb1"];

                        switch (Request.Form[myControls[i].prefixs + "ddl1"])
                        {
                            case "=":
                                {
                                    if (Request.Form[myControls[i].prefixs + "tb1"].Trim() != "")
                                    {
                                        myControls[i].dbScript = " and [" + myControls[i].columns + "] = \'" + Request.Form[myControls[i].prefixs + "tb1"] + "\'";
                                    }
                                    break;
                                }
                            case ">":
                                {
                                    if (Request.Form[myControls[i].prefixs + "tb1"].Trim() != "")
                                    {
                                        myControls[i].dbScript = " and [" + myControls[i].columns + "] > \'" + Request.Form[myControls[i].prefixs + "tb1"] + "\'";
                                    }
                                    break;
                                }
                            case "<":
                                {
                                    if (Request.Form[myControls[i].prefixs + "tb1"].Trim() != "")
                                    {
                                        myControls[i].dbScript = " and [" + myControls[i].columns + "] < \'" + Request.Form[myControls[i].prefixs + "tb1"] + "\'";
                                    }
                                    break;
                                }
                            case "<>":
                                {
                                    if (Request.Form[myControls[i].prefixs + "tb1"].Trim() != "")
                                    {
                                        myControls[i].dbScript = " and [" + myControls[i].columns + "] <> \'" + Request.Form[myControls[i].prefixs + "tb1"] + "\'";
                                    }
                                    break;
                                }
                            case ">=":
                                {
                                    if (Request.Form[myControls[i].prefixs + "tb1"].Trim() != "")
                                    {
                                        myControls[i].dbScript = " and [" + myControls[i].columns + "] >= \'" + Request.Form[myControls[i].prefixs + "tb1"] + "\'";
                                    }
                                    break;
                                }
                            case "<=":
                                {
                                    if (Request.Form[myControls[i].prefixs + "tb1"].Trim() != "")
                                    {
                                        myControls[i].dbScript = " and [" + myControls[i].columns + "] <= \'" + Request.Form[myControls[i].prefixs + "tb1"] + "\'";
                                    }
                                    break;
                                }
                        }
                    }
                    else if (rb == "r2")
                    {
                        string b1, b2;
                        b1 = Request.Form[myControls[i].prefixs + "tb2"];
                        b2 = Request.Form[myControls[i].prefixs + "tb3"];
                        if (b1 != "" && b2 != "")
                        {
                            myControls[i].dbScript = " and [" + myControls[i].columns + "] between \'" + b1 + "\' and  \'" + b2 + "\' ";
                        }

                        myControls[i].op1 = "r2";
                        myControls[i].range1 = b1;
                        myControls[i].range2 = b2;
                    }
                    else if (rb == "r3")
                    {
                        myControls[i].dbScript = " and ISNULL([" + myControls[i].columns + "],N'0')='0' ";
                        myControls[i].op1 = "r3";
                    }
                }
                else if (myControls[i].controlTypes == "enum")
                {
                    string rb = Request.Form[myControls[i].prefixs.Replace("_","$")];
                    myControls[i].dbScript = " and [" + myControls[i].columns + "] = \'" + rb + "\'";
                }
            }

            int insertedID = 0;
            if (myReport != null)
            {
                myReport.REPORTNAME = mypre.rpName;
                myReport.RPTITLE = mypre.ReportTitle;
                myReport.SVID = mypre.svid;
                myReport.REPORTGROUPLIST = mypre.Distribution;
                myReport.CATEGORY = mypre.categoryid;
                myReport.DEFAULTFORMAT = mypre.defaultFormat;
                myReport.EXTENDFIELD = mypre.showType;
                WebHelper.bllReport.Update(myReport);
                insertedID = myReport.ID;
                string sql_delete = "delete from reportcolumn where rpid=" + Request.QueryString["id"];
                bllcommon.executesql(sql_delete);
            }
            else
            {
                CUSTOMRP.Model.REPORT addreport = new CUSTOMRP.Model.REPORT();
                addreport.ADDUSER = me.ID;
                addreport.CATEGORY = mypre.categoryid;
                addreport.DATABASEID = me.DatabaseID;
                addreport.REPORTGROUPLIST = mypre.Distribution;
                addreport.REPORTNAME = mypre.rpName;
                addreport.RPTITLE = mypre.ReportTitle;
                addreport.SVID = mypre.svid;
                addreport.TYPE = 1;
                addreport.DEFAULTFORMAT = mypre.defaultFormat;
                addreport.EXTENDFIELD = mypre.showType;

                insertedID = WebHelper.bllReport.Add(addreport);
            }


            //insert into database.
            //1.insert to report.2.get rpid,3 insert to table named reportcolumn.
            //string sql_insert = "";
            //sql_insert = "insert into report(svid,reportname,category,type,distributionlevel,rptitle) values ('"+mypre.svid+"','"+mypre.rpName+"','"+mypre.categoryid+"','1','"+mypre.Distribution+"','"+mypre.ReportTitle+"') SELECT @@IDENTITY";

            
            //contents
            List<string> contents = mypre.contentColumn;
            foreach (string content in contents)
            {
                CUSTOMRP.Model.REPORTCOLUMN myselect = new CUSTOMRP.Model.REPORTCOLUMN();
                myselect.COLUMNNAME = content;
                myselect.RPID = insertedID;
                myselect.COLUMNFUNC = 1;

                WebHelper.bllReportColumn.Add(myselect);
            }

            //sort on
            List<string> sorts = new List<string>();
            if (mypre.sortonColumn !=null)
            {
                sorts = mypre.sortonColumn;
            }

            foreach (string content in sorts)
            {
                CUSTOMRP.Model.REPORTCOLUMN myselect = new CUSTOMRP.Model.REPORTCOLUMN();
                myselect.COLUMNNAME = content;
                myselect.RPID = insertedID;
                myselect.COLUMNFUNC = 3;

                WebHelper.bllReportColumn.Add(myselect);
            }

            //avg
            List<string> avgs = new List<string>();
            if (mypre.avgColumn != null)
            {
                avgs = mypre.avgColumn.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            }

            foreach (string content in avgs)
            {
                CUSTOMRP.Model.REPORTCOLUMN myselect = new CUSTOMRP.Model.REPORTCOLUMN();
                myselect.COLUMNNAME = content;
                myselect.RPID = insertedID;
                myselect.COLUMNFUNC = 4;

                WebHelper.bllReportColumn.Add(myselect);
            }

            //sum
            List<string> sum = new List<string>();
            if (mypre.sumColumn != null)
            {
                sum = mypre.sumColumn.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            }

            foreach (string content in sum)
            {
                CUSTOMRP.Model.REPORTCOLUMN myselect = new CUSTOMRP.Model.REPORTCOLUMN();
                myselect.COLUMNNAME = content;
                myselect.RPID = insertedID;
                myselect.COLUMNFUNC = 5;

                WebHelper.bllReportColumn.Add(myselect);
            }
            //group
            List<string> group = new List<string>();
            if (mypre.groupColumn != null)
            {
                group = mypre.groupColumn.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            }

            foreach (string content in group)
            {
                CUSTOMRP.Model.REPORTCOLUMN myselect = new CUSTOMRP.Model.REPORTCOLUMN();
                myselect.COLUMNNAME = content;
                myselect.RPID = insertedID;
                myselect.COLUMNFUNC = 6;

                WebHelper.bllReportColumn.Add(myselect);
            }

            //sub total
            List<string> subtotal = new List<string>();
            if (mypre.groupsumColumn != null)
            {
                subtotal = mypre.groupsumColumn.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            }

            foreach (string content in subtotal)
            {
                CUSTOMRP.Model.REPORTCOLUMN myselect = new CUSTOMRP.Model.REPORTCOLUMN();
                myselect.COLUMNNAME = content;
                myselect.RPID = insertedID;
                myselect.COLUMNFUNC = 7;

                WebHelper.bllReportColumn.Add(myselect);
            }

            //sub avg
            List<string> subavg = new List<string>();
            if (mypre.groupavgColumn != null)
            {
                subavg = mypre.groupavgColumn.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            }

            foreach (string content in subavg)
            {
                CUSTOMRP.Model.REPORTCOLUMN myselect = new CUSTOMRP.Model.REPORTCOLUMN();
                myselect.COLUMNNAME = content;
                myselect.RPID = insertedID;
                myselect.COLUMNFUNC = 8;

                WebHelper.bllReportColumn.Add(myselect);
            }

            //sub count
            List<string> groupcount = new List<string>();
            if (mypre.groupCountColumn != null)
            {
                groupcount = mypre.groupCountColumn.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            }

            foreach (string content in groupcount)
            {
                CUSTOMRP.Model.REPORTCOLUMN myselect = new CUSTOMRP.Model.REPORTCOLUMN();
                myselect.COLUMNNAME = content;
                myselect.RPID = insertedID;
                myselect.COLUMNFUNC =9;

                WebHelper.bllReportColumn.Add(myselect);
            }

            //sub total
            List<string> rpcount = new List<string>();
            if (mypre.countColumn != null)
            {
                rpcount = mypre.countColumn.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            }

            foreach (string content in rpcount)
            {
                CUSTOMRP.Model.REPORTCOLUMN myselect = new CUSTOMRP.Model.REPORTCOLUMN();
                myselect.COLUMNNAME = content;
                myselect.RPID = insertedID;
                myselect.COLUMNFUNC = 10;

                WebHelper.bllReportColumn.Add(myselect);
            }

            foreach (control c in myControls)
            {
                CUSTOMRP.Model.REPORTCOLUMN myselect = new CUSTOMRP.Model.REPORTCOLUMN();
                myselect.COLUMNNAME = c.columns;
                myselect.CRITERIA1 = c.dbScript;
                myselect.RPID = insertedID;
                myselect.COLUMNFUNC = 2;
                myselect.CRITERIA2 = c.op1;
                myselect.CRITERIA3 = c.range1;
                myselect.CRITERIA4 = c.range2;

                WebHelper.bllReportColumn.Add(myselect);
            }
            Common.JScript.AlertAndRedirect("Success!", "RPLIST.ASPX");
        }

        protected void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                //just print .no need save to database.
                rpcr = new List<string>();
                sums = new List<string>();
                avgs = new List<string>();
                groups = new List<string>();
                grouptotal = new List<string>();
                groupavg = new List<string>();
                groupcount = new List<string>();
                counts = new List<string>();

                if (string.IsNullOrEmpty(mypre.sumColumn) == false)
                {
                    sums = mypre.sumColumn.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                }
                if (string.IsNullOrEmpty(mypre.avgColumn) == false)
                {
                    avgs = mypre.avgColumn.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                }
                if (string.IsNullOrEmpty(mypre.groupColumn) == false)
                {
                    groups = mypre.groupColumn.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                }
                if (string.IsNullOrEmpty(mypre.groupsumColumn) == false)
                {
                    grouptotal = mypre.groupsumColumn.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                }
                if (string.IsNullOrEmpty(mypre.groupavgColumn) == false)
                {
                    groupavg = mypre.groupavgColumn.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                }
                if (string.IsNullOrEmpty(mypre.groupCountColumn) == false)
                {
                    groupcount = mypre.groupCountColumn.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                }
                if (string.IsNullOrEmpty(mypre.countColumn) == false)
                {
                    counts = mypre.countColumn.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                }

                string strSqlColumn, strSqlPlus, strSqlCriteria, strSqlSortOn;
                //string sql = getSql(ref rpcr);
                getSql(ref rpcr, out strSqlColumn, out strSqlCriteria, out strSqlPlus, out strSqlSortOn);
                //rpdt = WebHelper.bllCommon.query(sql);
                rptitle = mypre.ReportTitle;
                rpname = mypre.rpName;
                //string showType = mypre.showType;

                rpdt = CUSTOMRP.BLL.AppHelper.getDataForReport(mypre.svid, me.DatabaseNAME, strSqlColumn, strSqlPlus, strSqlCriteria, strSqlSortOn, true);

                //v1.0.0 Fai 2015.04.09 - Distinct Value - Begin
                string[] l_strColumnNameArray = rpdt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
                rpdt = rpdt.DefaultView.ToTable(true, l_strColumnNameArray);
                //v1.0.0 Fai 2015.04.09 - Distinct Value - End

                if (mypre.defaultFormat == 1)
                {
                    Server.Transfer("htmlexport.aspx?active=html", true);
                }
                else if (mypre.defaultFormat == 0)
                {
                    //v1.0.0 - Cheong - 2015/05/27 - commented because of function signature change
                    //XSSFWorkbook XSSFworkbook = NPOIHelper.GetWorkbookFromDt(rpdt, mypre.ReportTitle, rpcr, avgs, sums, groups, showType, grouptotal, groupavg, groupcount, counts);

                    string fileName = mypre.rpName + ".xlsx";
                    string folder = PathHelper.getTempFolderName();
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    string filePath = folder + PathHelper.getSafePath(fileName);
                    using (FileStream outFs = new FileStream(filePath, FileMode.Create))
                    {
                        //v1.0.0 - Cheong - 2015/05/27 - commented because of previous comment
                        //XSSFworkbook.Write(outFs);
                        //CS2202
                        //outFs.Close();
                    }
                    DownloadFile(filePath, fileName);
                }
                else
                {
                    string fontPath = PathHelper.getFontFolderName() + "simsun.ttc,1";
                    //v1.0.0 - Cheong - 2015/05/27 - commented because of function signature change
                    //Common.MyPdf.exp_Pdf(showType,grouptotal, groupavg, groupcount, counts, groups, avgs, sums, AppNum.companyName, rpcr.ToArray(), rpdt, rptitle,
                    //    fontPath, 14, 1, iTextSharp.text.BaseColor.BLACK,
                    //    fontPath, 11, 1, iTextSharp.text.BaseColor.BLACK,//列头字体、大小、样式、颜色
                    //    PathHelper.getTempFolderName(), rpname,
                    //    fontPath, 11, 1, iTextSharp.text.BaseColor.BLACK);//正文字体、大小、样式、颜色
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
            catch
            {
                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.GeneralError, "rplist.aspx");
                Response.End();
            }
#endif
        }

        public void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("rplist.aspx", true);
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
                //HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert(" + ex.Message + ")</script>");
            }
        }

        private void SaveSessionState()
        {
            rpexcel prePage = null;
            try
            {
                prePage = (rpexcel)Context.Handler;
            }
            catch
            {
                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.parameter_error, "rpexcel?id=" + myReport.ID);
                Response.End();
            }
            ContentPlaceHolder placeholder = (ContentPlaceHolder)prePage.Controls[0].FindControl("ContentPlaceHolder1");
            mypre = new preInfo()
            {
                rpName = ((TextBox)placeholder.FindControl("txtReportName")).Text,
                svid = Int32.Parse(((DropDownList)placeholder.FindControl("ddlQueryName")).SelectedValue),
                svname = (((DropDownList)placeholder.FindControl("ddlQueryName")).SelectedItem.Text.Split(new char[] { '|' })[0]),
                categoryid = Int32.Parse(((DropDownList)placeholder.FindControl("ddlCategory")).SelectedValue),
                ReportTitle = ((TextBox)placeholder.FindControl("txtReportTitle")).Text,
                Distribution = Int32.Parse(((DropDownList)placeholder.FindControl("ddlReportGroup")).SelectedValue),
                defaultFormat = Int32.Parse(((DropDownList)placeholder.FindControl("ddlFormat")).SelectedValue),
                showType = (((DropDownList)placeholder.FindControl("ddlShowType")).SelectedValue),
            };

            ListBox lbcontent = (ListBox)placeholder.FindControl("lbcontents");
            ListBox lbcriteria = (ListBox)placeholder.FindControl("lbcriteria");
            ListBox lbsorton = (ListBox)placeholder.FindControl("lbsorton");
            ListBox lbavg = (ListBox)placeholder.FindControl("lbavg");
            ListBox lbsum = (ListBox)placeholder.FindControl("lbsum");
            ListBox lbgroup = (ListBox)placeholder.FindControl("lbhiden");
            ListBox lbgrouptotal = (ListBox)placeholder.FindControl("lbgrouptotal");
            ListBox lbgroupavg = (ListBox)placeholder.FindControl("lbgroupavg");
            ListBox lbgroupcount = (ListBox)placeholder.FindControl("lbgroupcount");
            ListBox lbrpcount = (ListBox)placeholder.FindControl("lbrpcount");

            mypre.contentColumn = new List<string>();
            foreach (ListItem li in lbcontent.Items)
            {
                mypre.contentColumn.Add(li.Value);
            }

            mypre.criteriaColumn = new List<string>();
            foreach (ListItem li in lbcriteria.Items)
            {
                mypre.criteriaColumn.Add(li.Value);
            }

            mypre.sortonColumn = new List<string>();
            foreach (ListItem li in lbsorton.Items)
            {
                mypre.sortonColumn.Add(li.Value);
                if (!mypre.contentColumn.Contains(li.Value))
                {
                    mypre.contentColumn.Add(li.Value);
                }
            }

            foreach (ListItem li in lbavg.Items)
            {
                if (string.IsNullOrEmpty(mypre.avgColumn))
                {
                    mypre.avgColumn = li.Value;
                }
                else
                {
                    mypre.avgColumn = mypre.avgColumn + "," + li.Value;
                }

            }

            foreach (ListItem li in lbsum.Items)
            {
                if (string.IsNullOrEmpty(mypre.sumColumn))
                {
                    mypre.sumColumn = li.Value;
                }
                else
                {
                    mypre.sumColumn = mypre.sumColumn + "," + li.Value;
                }
            }


            foreach (ListItem li in lbgroup.Items)
            {
                if (string.IsNullOrEmpty(mypre.groupColumn))
                {
                    mypre.groupColumn = li.Value;
                }
                else
                {
                    mypre.groupColumn = mypre.groupColumn + "," + li.Value;
                }
            }

            foreach (ListItem li in lbgrouptotal.Items)
            {
                if (string.IsNullOrEmpty(mypre.groupsumColumn))
                {
                    mypre.groupsumColumn = li.Value;
                }
                else
                {
                    mypre.groupsumColumn = mypre.groupsumColumn + "," + li.Value;
                }
            }

            foreach (ListItem li in lbgroupavg.Items)
            {
                if (string.IsNullOrEmpty(mypre.groupavgColumn))
                {
                    mypre.groupavgColumn = li.Value;
                }
                else
                {
                    mypre.groupavgColumn = mypre.groupavgColumn + "," + li.Value;
                }
            }

            foreach (ListItem li in lbgroupcount.Items)
            {
                if (string.IsNullOrEmpty(mypre.groupCountColumn))
                {
                    mypre.groupCountColumn = li.Value;
                }
                else
                {
                    mypre.groupCountColumn = mypre.groupCountColumn + "," + li.Value;
                }
            }

            foreach (ListItem li in lbrpcount.Items)
            {
                if (string.IsNullOrEmpty(mypre.countColumn))
                {
                    mypre.countColumn = li.Value;
                }
                else
                {
                    mypre.countColumn = mypre.countColumn + "," + li.Value;
                }
            }


            //load cir
            IList<CUSTOMRP.Model.REPORTCOLUMN> criteriaColumns = null;
            if (myReport != null && selectColumns != null)
            {
                criteriaColumns = selectColumns.Where(f => f.COLUMNFUNC == 2).ToList();
            }
            //eload cir


            foreach (string cn in mypre.criteriaColumn)
            {

                //load cir
                CUSTOMRP.Model.REPORTCOLUMN thisColumn = null;
                if (criteriaColumns != null)
                {
                    foreach (CUSTOMRP.Model.REPORTCOLUMN c in criteriaColumns)
                    {
                        if (c.COLUMNNAME == cn)
                        {
                            thisColumn = c;
                        }
                    }
                }
                //eload cir


                if ("String" == CUSTOMRP.BLL.AppHelper.getColumnType(mypre.svid, mypre.svname, cn, me))
                {
                    Controls.CriteriaString control = (Controls.CriteriaString)Page.LoadControl("~/controls/CriteriaString.ascx");
                    control.ColumnName = cn;


                    if (thisColumn != null)
                    {
                        control.op1 = thisColumn.CRITERIA2;
                        control.range1 = thisColumn.CRITERIA3;
                        control.range2 = thisColumn.CRITERIA4;
                    }

                    this.Panel1.Controls.Add(control);

                    control ct = new control();
                    ct.columns = cn;
                    ct.controlTypes = control.ControlType;
                    ct.prefixs = control.prefix;
                    ct.dbScript = "";

                    myControls.Add(ct);
                }
                if ("DateTime" == CUSTOMRP.BLL.AppHelper.getColumnType(mypre.svid, mypre.svname, cn, me))
                {
                    Controls.CriteriaString control = (Controls.CriteriaString)Page.LoadControl("~/controls/CriteriaString.ascx");
                    control.ColumnName = cn;
                    control.ControlType = "datetime";

                    if (thisColumn != null)
                    {
                        control.op1 = thisColumn.CRITERIA2;
                        control.range1 = thisColumn.CRITERIA3;
                        control.range2 = thisColumn.CRITERIA4;
                    }

                    this.Panel1.Controls.Add(control);

                    control ct = new control();
                    ct.columns = cn;
                    ct.controlTypes = control.ControlType;
                    ct.prefixs = control.prefix;
                    ct.dbScript = "";

                    myControls.Add(ct);
                }
                else if ("Int" == CUSTOMRP.BLL.AppHelper.getColumnType(mypre.svid, mypre.svname, cn, me) || ("Decimal" == CUSTOMRP.BLL.AppHelper.getColumnType(mypre.svid, mypre.svname, cn, me)))
                {
                    Controls.CriteriaNumber control = (Controls.CriteriaNumber)Page.LoadControl("~/Controls/CriteriaNumber.ascx");
                    control.ColumnName = cn;

                    if (thisColumn != null)
                    {
                        control.op1 = thisColumn.CRITERIA2;
                        control.range1 = thisColumn.CRITERIA3;
                        control.range2 = thisColumn.CRITERIA4;
                    }


                    this.Panel1.Controls.Add(control);

                    control ct = new control();
                    ct.columns = cn;
                    ct.controlTypes = control.controlType;
                    ct.prefixs = control.prefix;
                    ct.dbScript = "";

                    myControls.Add(ct);

                }
                else if ("Enum" == CUSTOMRP.BLL.AppHelper.getColumnType(mypre.svid, mypre.svname, cn, me))
                {
                    Controls.CriteriaInt control = (Controls.CriteriaInt)Page.LoadControl("~/controls/CriteriaInt.ascx");
                    control.ColumnName = cn;
                    CUSTOMRP.Model.RpEnum rp = new RpEnum();
                    Type a = rp.GetType().GetNestedType((mypre.svname + "_" + cn).ToUpper());
                    control.dt = Common.Utils.GetTableFEnum(a, "text", "value");

                    this.Panel1.Controls.Add(control);

                    control ct = new control();
                    ct.columns = cn;
                    ct.controlTypes = control.controlType;
                    ct.prefixs = control.prefix;
                    ct.dbScript = "";

                    myControls.Add(ct);
                }
            }
            Session[strSessionRpExcel2_PreInfo] = mypre;
            Session[strSessionRpExcel2_Controls] = myControls;
        }

        private void LoadSessionState()
        {
            myControls = (IList<control>)Session[strSessionRpExcel2_Controls];
            mypre = (preInfo)Session[strSessionRpExcel2_PreInfo];
        }

        private ReportCriteria[] GetCriteria()
        {
            List<ReportCriteria> result = new List<ReportCriteria>();
            ReportCriteria current = null;

            for (int i = 0; i < myControls.Count(); i++)
            {
                current = new ReportCriteria()
                    {
                        COLUMNNAME = myControls[i].columns,
                        DATATYPE = myControls[i].controlTypes,
                    };
                if ((myControls[i].controlTypes == "string") || (myControls[i].controlTypes == "datetime"))
                {
                    string rb = Request.Form[myControls[i].prefixs + "rd1"];
                    current.CRITERIA2 = rb;
                    if (rb == "r1")
                    {
                        switch (Request.Form[myControls[i].prefixs + "ddl1"])
                        {
                            case "Begins With":
                            case "Contains":
                            case "=":
                            case "<=":
                            case ">=":
                            case "Does not contain":
                            case "Does not equal":
                                {
                                    current.CRITERIA3 = Request.Form[myControls[i].prefixs + "ddl1"];
                                    current.CRITERIA4 = Request.Form[myControls[i].prefixs + "tb1"];
                                }
                                break;
                        }
                    }
                    else if (rb == "r2")
                    {
                        current.CRITERIA3 = Request.Form[myControls[i].prefixs + "tb2"];
                        current.CRITERIA4 = Request.Form[myControls[i].prefixs + "tb3"];
                    }
                    else if (rb == "r3")
                    {
                        current.CRITERIA3 = String.Empty;
                        current.CRITERIA4 = String.Empty;
                    }
                }
                else if (myControls[i].controlTypes == "int")
                {
                    string rb = Request.Form[myControls[i].prefixs + "rd1"];
                    current.CRITERIA2 = rb;
                    if (rb == "r1")
                    {
                        switch (Request.Form[myControls[i].prefixs + "ddl1"])
                        {
                            case "=":
                            case ">":
                            case "<":
                            case "<>":
                            case ">=":
                            case "<=":
                                {
                                    current.CRITERIA3 = Request.Form[myControls[i].prefixs + "ddl1"];
                                    current.CRITERIA4 = Request.Form[myControls[i].prefixs + "tb1"];
                                }
                                break;
                        }
                    }
                    else if (rb == "r2")
                    {
                        current.CRITERIA3 = Request.Form[myControls[i].prefixs + "tb2"];
                        current.CRITERIA4 = Request.Form[myControls[i].prefixs + "tb3"];
                    }
                    else if (rb == "r3")
                    {
                        current.CRITERIA3 = String.Empty;
                        current.CRITERIA4 = String.Empty;
                    }
                }
                else if (myControls[i].controlTypes == "enum")
                {
                    current.CRITERIA3 = "=";
                    current.CRITERIA4 = Request.Form[myControls[i].prefixs.Replace("_", "$")];
                }

                result.Add(current);
            }

            return result.ToArray();
        }

        private void getSql(ref IList<string> cir, out string contens, out string sqlplus, out string criterias, out string sorton)
        {
            contens = "[" + String.Join("], [", mypre.contentColumn) + "]";
            sorton = String.Empty;

            if (mypre.sortonColumn.Count > 0)
            {
                sorton = "[" + String.Join("], [", mypre.sortonColumn) + "]";
            }

            ReportCriteria[] criteria = this.GetCriteria();
            CUSTOMRP.BLL.AppHelper.ParseParam(ref criteria);

            StringBuilder crical = new StringBuilder();
            foreach (ReportCriteria rc in criteria)
            {
                if (!String.IsNullOrWhiteSpace(rc.CRITERIA1))
                {
                    crical.AppendLine(rc.CRITERIA1);
                    cir.Add(rc.CRITERIA1);
                }
            }

            sqlplus = CUSTOMRP.BLL.AppHelper.sql_plus(mypre.svid, me);

            sorton = string.IsNullOrEmpty(sorton) == true ? "" : " order by " + sorton;
            criterias = crical.ToString();
        }
        */
    }
}