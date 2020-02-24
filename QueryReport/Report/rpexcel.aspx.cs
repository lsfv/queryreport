using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using QueryReport.Code;
using System.Diagnostics;

namespace QueryReport
{
    public partial class rpexcel : LoginUserPage
    {
        public const string strSessionKeyMyReport = "__SESSION_REPORT_MyReport";    // only saved when pressed "Next" in this page
        public const string strSessionKeyColumnInfo = "__SESSION_REPORT_ColumnInfo";
        public const string strSessionKeyReportParameterContainer = "__SESSION_REPORT_ReportParameterContainer";
        public const string strSessionKeyFormulaFields = "__SESSION_REPORT_FormulaFields";
        public const string strBtnText_replace = "Upload";
        public const string strBtnText_add = "Upload";
        public const string TEMPLATEMESSAGEPREFIX = "Template : ";
        public const string TEMPLATEEMTYFILE = "No template file.";


        // v1.0.0 - Cheong - 2015/05/29 - these field not set as public properties because of flow change that allows user to visit rpexcel2.aspx directly if no right to modify.
        private CUSTOMRP.Model.REPORT myReport = null;
        private CUSTOMRP.Model.SOURCEVIEW mySV = null;
        private List<CUSTOMRP.Model.ColumnInfo> columninfos = null;
        private List<CUSTOMRP.Model.REPORTCOLUMN> selectedColumns = null;   // not to be mixed from the columns in the database report records
        private List<CUSTOMRP.Model.REPORTCOLUMN> formulaFields = null;
        private bool p_fSuppressRender = false;     // Whether to render page contents

        private bool isRefresh = false;

        #region Event Handlers

        protected void Page_Init(object sender, EventArgs e)
        {
            //v1.8.8 Alex 2019.03.18 - Do not run v/sp to get the column names, cause it's slow
            //isRefresh = false;
            if (Request.Form.AllKeys.Contains("__EVENTTARGET") && Request.Form["__EVENTTARGET"].Equals("ctl00$ContentPlaceHolder1$btnRefresh"))
            {
                isRefresh = true;
            }

            if (Session[strSessionKeyColumnInfo] != null)
            {
                columninfos = (List<CUSTOMRP.Model.ColumnInfo>)Session[strSessionKeyColumnInfo];
            }

            if (Session[strSessionKeyFormulaFields] != null)
            {
                formulaFields = (List<CUSTOMRP.Model.REPORTCOLUMN>)Session[strSessionKeyFormulaFields];
            }
            else
            {
                formulaFields = new List<CUSTOMRP.Model.REPORTCOLUMN>();
                Session[strSessionKeyFormulaFields] = formulaFields;
            }
            if (!this.IsPostBack || isRefresh)
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
                        CUSTOMRP.BLL.SOURCEVIEW svBLL = new CUSTOMRP.BLL.SOURCEVIEW();
                        mySV = svBLL.GetModel(me.ID, myReport.SVID);
                    }
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    int id = Int32.Parse(Request.QueryString["id"]);
                    if (Session[strSessionKeyReportParameterContainer] != null)
                    {
                        ReportParameterContainer container = (ReportParameterContainer)Session[strSessionKeyReportParameterContainer];
                        myReport = container.GetReportModel(me, columninfos, id);
                    }
                }
            }
            if (myReport != null)
            {
                Session[strSessionKeyMyReport] = myReport;
            }

            if (!IsPostBack || isRefresh)
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

        protected void btnCopy_Click(object sender, EventArgs e)
        {
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            // Update SOURCEVIEWCOLUMN usign columninfos

            CUSTOMRP.Model.SOURCEVIEWCOLUMN[] updatedColumns = columninfos.Select(x => new CUSTOMRP.Model.SOURCEVIEWCOLUMN
            {
                SVID = mySV.ID,
                COLUMNTYPE = 1,
                COLUMNCOMMENT = String.Empty,
                HIDDEN = false,
                COLUMNNAME = x.ColName,
                DISPLAYNAME = String.Empty,
                DATA_TYPE = x.DataType
            }).ToArray();

            WebHelper.bllSOURCEVIEWCOLUMN.UpdateModelList(me.ID, updatedColumns);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // v1.8.8 2019.04.03 - Hide refresh button
            if (!Request.QueryString.AllKeys.Contains("id"))
            {
                btnRefresh.Visible = false;
                btnCopy.Visible = false;
            }

            if (ProcessAjaxRequestParameters()) { return; } // stop further process if it's Ajax event

            this.lblJavascript.Text = String.Empty;

            if (!IsPostBack || Request.Form.AllKeys.Contains("__EVENTTARGET") && Request.Form["__EVENTTARGET"] == "ctl00$ContentPlaceHolder1$btnRefresh")
            {
                this.FillPageHeaderUI();
            }

            #region get column names

            string[] colnames = null;
            //v1.7.0 Janet 2017.06.19 - when Query name is "--please select--" no need to run - Begin

            if (this.ddlQueryName.SelectedValue != string.Empty || isRefresh)
            {
                if (mySV == null)
                {
                    mySV = WebHelper.bllSOURCEVIEW.GetModel(me.ID, Int32.Parse(this.ddlQueryName.SelectedValue));
                }
                List<CUSTOMRP.Model.SOURCEVIEWCOLUMN> svcolnames;

                //v1.8.8 Alex 2019.03.18 - Do not run v/sp to get the column names, cause it's slow
                if (isRefresh)
                {
                    svcolnames = WebHelper.bllSOURCEVIEWCOLUMN.RefreshModelsForSourceView(me.ID, mySV.ID, true).OrderBy(x => x.DisplayName).ToList();

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
                else
                {
                    svcolnames = WebHelper.bllSOURCEVIEWCOLUMN.GetModelsForSourceView(me.ID, mySV.ID, true).OrderBy(x => x.DisplayName).ToList();
                    colnames = svcolnames.Select(x => x.COLUMNNAME).ToArray();
                }

                foreach (string col in colnames)
                {
                    if (svcolnames.Where(x => x.COLUMNNAME == col && x.ColumnType == CUSTOMRP.Model.SOURCEVIEWCOLUMN.ColumnTypes.Normal).Count() == 0)
                    {
                        svcolnames.Add(new CUSTOMRP.Model.SOURCEVIEWCOLUMN() { COLUMNNAME = col, DISPLAYNAME = col, HIDDEN = false });
                    }
                }

                //v1.2.0 - Cheong - 2016/12/28 - Column existance check need to check column name instead of display name
                //colnames = svcolnames.Where(x => !x.HIDDEN).Select(x => x.DisplayName).ToArray();
                colnames = svcolnames.Where(x => !x.HIDDEN).Select(x => x.COLUMNNAME).ToArray();

            #endregion

                if (selectedColumns != null)
                {
                    foreach (CUSTOMRP.Model.REPORTCOLUMN dr in selectedColumns)
                    {
                        if (dr.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Formula)
                        {
                            this.formulaFields.Add(dr);
                            ListItem li = new ListItem(dr.DisplayName);
                            li.Attributes.Add("numeric", dr.IS_NUMERIC ? "true" : "false");
                            this.lbFormula.Items.Add(li);

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
                                        var newEle = new ListItem(dr.DisplayName);
                                        newEle.Attributes.Add("class", dr.IS_ASCENDING ? "asc" : "desc");
                                        this.lbsorton.Items.Add(newEle);
                                    }
                                    break;
                                case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Avg:
                                    {
                                        this.lbavg.Items.Add(new ListItem(dr.DisplayName));
                                    }
                                    break;
                                case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Sum:
                                    {
                                        this.lbsum.Items.Add(new ListItem(dr.DisplayName));
                                    }
                                    break;
                                case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Group:
                                    {
                                        this.lbhiden.Items.Add(new ListItem(dr.DisplayName));
                                    }
                                    break;
                                case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.GroupSum:
                                    {
                                        this.lbgrouptotal.Items.Add(new ListItem(dr.DisplayName));
                                    }
                                    break;
                                case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.GroupAvg:
                                    {
                                        this.lbgroupavg.Items.Add(new ListItem(dr.DisplayName));
                                    }
                                    break;
                                case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.GroupCount:
                                    {
                                        this.lbgroupcount.Items.Add(new ListItem(dr.DisplayName));
                                    }
                                    break;
                                case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Count:
                                    {
                                        this.lbrpcount.Items.Add(new ListItem(dr.DisplayName));
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
                                            if (selectedColumns.Where(x => (x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Group) && (x.DisplayName == dr.DisplayName)).Count() == 0)
                                            {
                                                var newEle = new ListItem(dr.DisplayName, dr.COLUMNNAME);
                                                newEle.Attributes.Add("class", dr.IS_ASCENDING ? "asc" : "desc");
                                                this.lbsorton.Items.Add(newEle);
                                            }
                                            else
                                            {
                                                ListItem item = new ListItem(dr.DisplayName, dr.COLUMNNAME);
                                                item.Attributes.Add("class", dr.IS_ASCENDING ? "asc grouped" : "desc grouped");
                                                //item.Attributes.Add("disabled", "");
                                                this.lbsorton.Items.Add(item);
                                            }
                                        }
                                        break;
                                    case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Avg:
                                        {
                                            this.lbavg.Items.Add(new ListItem(dr.DisplayName, dr.COLUMNNAME));
                                        }
                                        break;
                                    case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Sum:
                                        {
                                            this.lbsum.Items.Add(new ListItem(dr.DisplayName, dr.COLUMNNAME));
                                        }
                                        break;
                                    case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Group:
                                        {
                                            this.lbhiden.Items.Add(new ListItem(dr.DisplayName, dr.COLUMNNAME));
                                        }
                                        break;
                                    case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.GroupSum:
                                        {
                                            this.lbgrouptotal.Items.Add(new ListItem(dr.DisplayName, dr.COLUMNNAME));
                                        }
                                        break;
                                    case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.GroupAvg:
                                        {
                                            this.lbgroupavg.Items.Add(new ListItem(dr.DisplayName, dr.COLUMNNAME));
                                        }
                                        break;
                                    case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.GroupCount:
                                        {
                                            this.lbgroupcount.Items.Add(new ListItem(dr.DisplayName, dr.COLUMNNAME));
                                        }
                                        break;
                                    case CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Count:
                                        {
                                            this.lbrpcount.Items.Add(new ListItem(dr.DisplayName, dr.COLUMNNAME));
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    //v1.7.0 Janet 2017.06.19 - when Query name is "--please select--" no need to run - End
                    // After formula fields are loaded, save it to session
                    Session[strSessionKeyFormulaFields] = formulaFields;
                }
            }
            //v1.0.0 - Cheong - 2015/03/31 if no right to modify, go directly to rpexcel2.aspx
            //v1.0.0 - Cheong - 2016/01/06 - goto criteria page if at least 1 criteria exist
            //if (!me.rp_modify)
            if ((!me.rp_modify) || (Request.Params["cmd"] == "run"))
            {
                Session[rpexcel.strSessionKeyReportParameterContainer] = null;
                Server.Transfer("rpexcel2.aspx", true);
            }
        }

        //不想加入这个方法的.但是发现全局变量 myReport 居然在有id的时候,会是空. 难道是我之前的逻辑错了?还是后面别人修改出vbug了?
        private CUSTOMRP.Model.REPORT GetCurrentPageReport()
        {
            CUSTOMRP.Model.REPORT res = null;
            if (!String.IsNullOrEmpty(Request.QueryString["id"]))
            {
                int id = Int32.Parse(Request.QueryString["id"]);
                res = WebHelper.bllReport.GetModel(me.ID, id);
            }

            return res;
        }

        protected void btn_uploadTemplate_ServerClick(object sender, EventArgs e)
        {
            //check file.  2.get story path 3. upload and insert record to database. 4.change ui text.
            //1.check pivotable =>label ,btn 2. upload=>label btn, 
            string absoluteDir = Server.MapPath("~/"+AppNum.STR_EXCELTEMPLATEPATH);
            List<string> types = new List<string>();
            types.Add("xlsx");
            string fileOrigName = this.fu_exceltempletea.FileName;
            string filename;
            string errmsg;
            bool isUpload = Common.IncUpfile.SaveFile(absoluteDir, this.fu_exceltempletea, types, out filename, out errmsg);

            if (isUpload)
            {
                CUSTOMRP.Model.REPORT theRp = GetCurrentPageReport();
                if (theRp == null)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.parameter_error, "/rplist.aspx");
                    Response.End();
                }
                CUSTOMRP.Model.WORDFILE template = WebHelper.bllWordFile.GetModelByReportID(me.ID, theRp.ID);
                if (template == null)
                {
                    //add record
                    CUSTOMRP.Model.WORDFILE newtemplate = new CUSTOMRP.Model.WORDFILE();

                    newtemplate.RPID = theRp.ID;
                    newtemplate.CreateDate = DateTime.Now;
                    newtemplate.Description = "excel template";
                    newtemplate.CreateUser = me.ID;
                    newtemplate.WordFileName = filename;
                    newtemplate.OrigFileName = fileOrigName;
                    newtemplate.ModifyDate = DateTime.Now;
                    newtemplate.ModifyUser = me.ID;
                    int id = WebHelper.bllWordFile.AddFile(newtemplate);
                    //this.lblErrorText.Text = "add" + id;
                    //这里体现了 asp.net 开发的一种模式.   pageload 只初始化控件,所有控件的修改都是在事件中处理,而是不放入到pageload
                    this.btn_uploadTemplateb.Value = strBtnText_replace;
                    this.lt_filenamec.InnerText = TEMPLATEMESSAGEPREFIX + fileOrigName;
                }
                else
                {
                    //updateRecord(tempID);

                    template.WordFileName = filename;
                    template.OrigFileName = fileOrigName;
                    template.ModifyDate = DateTime.Now;
                    template.ModifyUser = me.ID;
                    WebHelper.bllWordFile.UpdateFile(template);
                    //this.lblErrorText.Text = "replace" + template.WordFileID;
                    this.lt_filenamec.InnerText = TEMPLATEMESSAGEPREFIX + fileOrigName;
                }
            }
            String successMsg = string.Format("Success!");
            string failMsg = string.Format("Fail:{0}", errmsg);
            //this.lblErrorText.Text = isUpload == true ? successMsg : failMsg;

            this.lblJavascript.Text = WebHelper.GetJSModelShow();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.p_fSuppressRender)
            {
                base.Render(writer);
            }
        }

        protected void ddlQueryName_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ddlQueryNameChange();
        }

        //protected void ddlShowType_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //      this.ShowTypeChanged();
        //}

        protected void btnNext_Click(object sender, EventArgs e)
        {
            ReportParameterContainer container = (ReportParameterContainer)Session[strSessionKeyReportParameterContainer];
            //if (selectedColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content).Count() == 0)
            if ((container == null) || (container.contentColumn.Length == 0))
            {
                Common.JScript.Alert("Please select content columns.");
                Common.JScript.GoHistory(-1);
                Response.End();
            }
            else
            {
                if (myReport == null)
                {
                    // Disallow report with same report name
                    //if (WebHelper.bllReport.GetList(" reportname='" + this.txtReportName.Text + "'  AND DATABASEID='" + me.DatabaseID + "'").Tables[0].Rows.Count > 0)
                    if (WebHelper.bllReport.GetList(me.ID, " reportname='" + container.ReportName + "'  AND DATABASEID='" + me.DatabaseID + "'").Tables[0].Rows.Count > 0)
                    {
                        Common.JScript.Alert(AppNum.ErrorMsg.Commonexits);
                        Common.JScript.GoHistory(-1);
                        Response.End();
                    }
                }
            }
            if (myReport == null)
            {
                myReport = container.GetReportModel(me, columninfos, myReport != null ? myReport.ID : 0);
            }
            else
            {
                foreach (CUSTOMRP.Model.REPORTCOLUMN c in myReport.ReportColumns)
                {
                    if (c.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content && c.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Formula)
                    {
                        Fields f = container.contentColumn.Where(x => x.ColumnName.ToUpper() == c.COLUMNNAME.ToUpper()).FirstOrDefault();
                        if (f != null)
                        {
                            c.COLUMNCOMMENT = f.Formula;
                        }
                    }
                }
            }
            Session[strSessionKeyMyReport] = myReport;
            Server.Transfer("rpexcel2.aspx", true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (me.rp_delete)
            {
                string strrpid = Request.QueryString["id"];
                int rpid;
                if (Int32.TryParse(strrpid, out rpid))
                {
                    //string sql_delete = "delete from [report] where [id]=" + rpid + "; delete from [REPORTCOLUMN] where [rpid]=" + rpid;
                    //WebHelper.bllCommon.executesql(sql_delete);
                    WebHelper.bllReport.Delete(me.ID, rpid);
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

        #endregion

        #region Private methods

        private void FillPageHeaderUI()
        {
            DataTable myDt = WebHelper.bllSOURCEVIEW.GetQueryListForDropdown(me).Tables[0];

            this.ddlQueryName.DataSource = myDt;
            this.ddlQueryName.DataTextField = "NEWDesc";
            this.ddlQueryName.DataValueField = "ID";
            this.ddlQueryName.DataBind();

            //v1.7.0 Janet 2017.06.19 - Query Name default set 'please select'- Begin
            this.ddlQueryName.Items.Insert(0, (new ListItem("N/A", string.Empty)));
            this.ddlQueryName.SelectedValue = string.Empty;
            //v1.7.0 Janet 2017.06.19 - Query Name - End

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

            if (myReport != null)
            {
                this.txtReportName.Text = myReport.REPORTNAME;
                this.txtReportTitle.Text = myReport.RPTITLE;
                this.ddlFormat.SelectedValue = myReport.DEFAULTFORMAT.ToString();
                this.ddlQueryName.SelectedValue = myReport.SVID.ToString();
                this.ddlCategory.SelectedValue = myReport.CATEGORY.ToString();
                this.ddlReportGroup.SelectedValue = myReport.REPORTGROUPLIST.ToString();
                this.ddlShowType.SelectedValue = myReport.ReportType;
                //v1.1.0 - Cheong - 2016/06/07 - Add UI for "Hide Criteria" option
                this.chkHideCriteria.Checked = myReport.fHideCriteria;
                //v1.2.0 - Cheong - 2016/07/04 - Add option to hide duplicate items
                this.chkHideDuplicate.Checked = myReport.fHideDuplicate;
                //v1.2.0 Kim 2016.11.02 move to printconfig
                ////v1.0.0 - Cheong - 2016/02/29 - Add UI for Report Header and Footer
                //this.txtReportHeader.Text = myReport.REPORT_HEADER;
                //this.txtReportFooter.Text = myReport.REPORT_FOOTER;

                //this.ShowTypeChanged();
                CUSTOMRP.Model.WORDFILE file = WebHelper.bllWordFile.GetModelByReportID(me.ID, myReport.ID);
                if (file != null)
                {
                    this.btn_uploadTemplateb.Value = strBtnText_replace;
                    this.lt_filenamec.InnerText = TEMPLATEMESSAGEPREFIX + file.OrigFileName;
                }
                else
                {
                    this.btn_uploadTemplateb.Value = strBtnText_add;
                    this.lt_filenamec.InnerText = TEMPLATEMESSAGEPREFIX + TEMPLATEEMTYFILE;
                }
            }

            //if (this.ddlQueryName.SelectedItem != null)
            if (this.ddlQueryName.SelectedValue != string.Empty || isRefresh)
            {
                this.ddlQueryNameChange();
            }
            //else { this.ddlQueryName.Items.Add(new ListItem("N/A", "")); }


        }

        private void ddlQueryNameChange()
        {
            string[] colnames = null;
            CUSTOMRP.Model.SOURCEVIEW sv = WebHelper.bllSOURCEVIEW.GetModel(me.ID, Int32.Parse(this.ddlQueryName.SelectedValue));
            List<CUSTOMRP.Model.SOURCEVIEWCOLUMN> svc = WebHelper.bllSOURCEVIEWCOLUMN.GetModelsForSourceView(me.ID, sv.ID, true).OrderBy(x => x.DisplayName).ToList();
            if (Request.Form.AllKeys.Contains("__EVENTTARGET") && Request.Form["__EVENTTARGET"].Equals("ctl00$ContentPlaceHolder1$btnRefresh"))
            {
                svc = WebHelper.bllSOURCEVIEWCOLUMN.RefreshModelsForSourceView(me.ID, sv.ID, true).OrderBy(x => x.DisplayName).ToList();
            }



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
                            this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert('Error in retrieving columns for [{0}]. Please check view definition.');</script>", sv.TBLVIEWNAME);
                        }
                        if (svc == null)
                        {
                            if (isRefresh) {
                                colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForTblView(me.ID, me.DatabaseNAME, sv.TBLVIEWNAME);
                            }
                            else
                            {
                                colnames = WebHelper.bllSOURCEVIEWCOLUMN.GetModelsForSourceView(me.ID, mySV.ID, true).OrderBy(x => x.DisplayName).ToList().Select(x => x.COLUMNNAME).ToArray();
                            }
                            // Filter result to only columns that is requested
                            columninfos = columninfos.Where(x => colnames.Contains(x.ColName)).ToList();
                        }
                    }
                    break;
                case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.StoredProc:
                    {
                        try
                        {
                            if (isRefresh)
                            {
                                columninfos = CUSTOMRP.BLL.AppHelper.GetColumnInfoForStoredProcRefresh(me.ID, me.DatabaseNAME, sv.TBLVIEWNAME);
                            }
                            else
                            {
                                columninfos = CUSTOMRP.BLL.AppHelper.GetColumnInfoForStoredProc(me.ID, me.DatabaseNAME, sv.ID);
                                if (columninfos.Any(x => string.IsNullOrEmpty(x.DataType)))
                                {
                                    columninfos = CUSTOMRP.BLL.AppHelper.GetColumnInfoForStoredProcRefresh(me.ID, me.DatabaseNAME, sv.TBLVIEWNAME);
                                    CUSTOMRP.Model.SOURCEVIEWCOLUMN[] updatedColumns = columninfos.Select(x => new CUSTOMRP.Model.SOURCEVIEWCOLUMN
                                    {
                                        SVID = sv.ID,
                                        COLUMNTYPE = 1,
                                        COLUMNCOMMENT = String.Empty,
                                        HIDDEN = false,
                                        COLUMNNAME = x.ColName,
                                        DISPLAYNAME = String.Empty,
                                        DATA_TYPE = x.DataType
                                    }).ToArray();

                                    WebHelper.bllSOURCEVIEWCOLUMN.UpdateModelList(me.ID, updatedColumns);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                            columninfos = new List<CUSTOMRP.Model.ColumnInfo>();
                            this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert('Error in retrieving columns for [{0}]. Please check stored proc defination.');</script>", sv.TBLVIEWNAME);
                        }
                        if (svc == null)
                        {
                            if (isRefresh)
                            {
                                colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForStoredProc(me.ID, me.DatabaseNAME, sv.TBLVIEWNAME);
                            }
                            else
                            {
                                colnames = WebHelper.bllSOURCEVIEWCOLUMN.GetModelsForSourceView(me.ID, mySV.ID, true).OrderBy(x => x.DisplayName).ToList().Select(x => x.COLUMNNAME).ToArray();
                            }
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
            this.lbFormula.Items.Clear();
            this.formulaFields.Clear();
            Session.Remove(strSessionKeyFormulaFields);

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
                               orderby c.ColName //s.DisplayName
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
            Session[strSessionKeyColumnInfo] = columninfos;

            this.lbcontents.Items.Clear();
            this.lbcriteria.Items.Clear();
            this.lbsorton.Items.Clear();
            this.lbhiden.Items.Clear();
            this.lbgrouptotal.Items.Clear();
            this.lbsum.Items.Clear();
            this.lbgroupavg.Items.Clear();
            this.lbavg.Items.Clear();
            this.lbgroupcount.Items.Clear();
            this.lbrpcount.Items.Clear();
        }

        #endregion

        #region Ajax handling Methods

        private bool ProcessAjaxRequestParameters()
        {
            bool result = false;

            string p_strAction = (Request.Params["action"] ?? String.Empty).ToLower();

            switch (p_strAction)
            {
                case "checkreportname":
                    {
                        this.AjaxCheckReportName();
                        result = true;
                    }
                    break;
                case "refreshdlgcolumns":
                    {
                        this.AjaxReloadDlgColumns();
                        result = true;
                    }
                    break;
                case "reloadformuladlg":
                    {
                        this.AjaxReloadFormulaDlg();
                        result = true;
                    }
                    break;
                case "checkfieldtext":
                    {
                        this.AjaxValidateFormula();
                        result = true;
                    }
                    break;
                case "updatefieldname":
                    {
                        this.AjaxUpdateFieldName();
                        result = true;
                    }
                    break;
                case "updatefieldtext":
                    {
                        this.AjaxUpdateFieldText();
                        result = true;
                    }
                    break;
                case "formulasavechange":
                    {
                        this.AjaxUpdatelbformula();
                        result = true;
                    }
                    break;
                case "removeformula":
                    {
                        this.AjaxFormulaRemove();
                        result = true;
                    }
                    break;
                case "pagesubmission":
                    {
                        this.AjaxPushSelectionToSession();
                        result = true;
                    }
                    break;
                case "copy":
                    {
                        this.AjaxCopy();
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private void AjaxCheckReportName()
        {
            string strReportName = Request.Params["ReportName"];
            int rpid = 0;

            CUSTOMRP.Model.REPORT currentrp = (CUSTOMRP.Model.REPORT)Session[strSessionKeyMyReport];
            if (currentrp != null)
            {
                rpid = currentrp.ID;
            }
            AjaxShowError(WebHelper.bllReport.CheckReportNameExist(me.ID, strReportName, me.DatabaseID, rpid) ? "Report name already in use." : "OK");
        }

        private void AjaxReloadDlgColumns()
        {
            Fields[] FieldNames = this.columninfos.Select(x => new Fields() { ColumnName = x.ColName, DisplayName = x.DisplayName }).ToArray();

            ColumnReloadOptions payload = new ColumnReloadOptions()
            {
                fields = FieldNames
            };

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ColumnReloadOptions));
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, payload);

            this.p_fSuppressRender = true;
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.ContentEncoding = Encoding.UTF8;
            ms.WriteTo(Response.OutputStream);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Will automatically new entry if not exist
        /// </summary>
        private void AjaxReloadFormulaDlg()
        {
            int idx = 0;

            if (!Int32.TryParse(Request.Params["id"], out idx))
            {
                AjaxShowError("Invalid ID number!");
                return;
            }

            if (idx > this.formulaFields.Count)
            {
                AjaxShowError("Index out of range!");
                return;
            }

            if (idx == this.formulaFields.Count)    // new
            {
                CUSTOMRP.Model.REPORTCOLUMN newfield = new CUSTOMRP.Model.REPORTCOLUMN();
                newfield.COLUMNFUNC = 1;            // content
                newfield.COLUMNTYPE = 2;            // formula
                newfield.COLUMNNAME = "Field" + Convert.ToString(idx + 1);
                newfield.DISPLAYNAME = "Field" + Convert.ToString(idx + 1);
                newfield.COLUMNCOMMENT = String.Empty;
                if (myReport != null)
                {
                    newfield.RPID = myReport.ID;
                    newfield.SVID = myReport.SVID;
                }
                formulaFields.Add(newfield);

                Session[strSessionKeyFormulaFields] = formulaFields;
            }

            CUSTOMRP.Model.REPORTCOLUMN f = this.formulaFields[idx];
            DlgParam payload = new DlgParam()
            {
                Idx = idx,
                FieldName = f.DisplayName,
                RenderText = f.COLUMNCOMMENT,
                //IsNumeric = f.IS_NUMERIC
            };

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(DlgParam));
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, payload);

            this.p_fSuppressRender = true;
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.ContentEncoding = Encoding.UTF8;
            ms.WriteTo(Response.OutputStream);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private int ValidateFormula()
        {
            string strSVID = Request.Params["queryid"];
            string fieldname = Request.Params["fieldname"];
            string fieldtext = Request.Params["fieldtext"];

            int svid = 0;
            if (!(Int32.TryParse(strSVID, out svid))) { return 0; }

            CUSTOMRP.Model.REPORTCOLUMN newfield = new CUSTOMRP.Model.REPORTCOLUMN();
            newfield.COLUMNFUNC = 1;            // content
            newfield.COLUMNTYPE = 2;            // formula
            newfield.COLUMNNAME = fieldname;
            newfield.DISPLAYNAME = fieldname;
            newfield.COLUMNCOMMENT = fieldtext;

            // v1.8.8 Alex 2018.10.03 - Parse fields and check if they are numeric
            //CUSTOMRP.BLL.AppHelper.ReportCheckFieldsAreNumeric(me.ID, me.DatabaseNAME, svid, );

            // v1.8.8 Handle cases like 1+1
            int validationResult = CUSTOMRP.BLL.AppHelper.CheckFormulaForReport(me.ID, svid, me.DatabaseNAME, newfield.DisplayName, newfield.SelectStatement);
            return validationResult;
        }

        ////lbsum, lbavg, lbrpcount
        //private void checkSelectedFormulaNumeric()
        //{
        //    if (Contains(this.lbFormula.SelectedItem.Attributes.Add().SelectedValue))
        //    {

        //    }
        //}

        private void AjaxValidateFormula()
        {
            if (ValidateFormula() > 0)
            {
                AjaxShowError("OK");
            }
            else
            {
                AjaxShowError("Error: The SQL text appears to be invalid!");
            }
        }

        private void AjaxUpdateFieldName()
        {
            string fieldname = Request.Params["fieldname"];
            int idx = 0;

            if (!Int32.TryParse(Request.Params["id"], out idx))
            {
                AjaxShowError("Invalid ID number!");
                return;
            }

            if (this.formulaFields.Count < (idx - 1))
            {
                AjaxShowError("Index out of range!");
                return;
            }

            for (int i = 0; i < formulaFields.Count; i++)
            {
                if ((i != idx) && (formulaFields[i].DISPLAYNAME == fieldname))
                {
                    AjaxShowError("Field name already exist!");
                    return;
                }
            }

            for (int i = 0; i < columninfos.Count; i++)
            {
                if ((i != idx) && (columninfos[i].ColName == fieldname))
                {
                    AjaxShowError("Field name cannot be the same as column name!");
                    return;
                }
            }

            CUSTOMRP.Model.REPORTCOLUMN f = formulaFields[idx];
            f.COLUMNNAME = fieldname;
            f.DISPLAYNAME = fieldname;

            AjaxReloadFormulaDlg();
        }

        private void AjaxUpdateFieldText()
        {
            string fieldtext = Request.Params["fieldtext"];
            int idx = 0;

            if (!Int32.TryParse(Request.Params["id"], out idx))
            {
                AjaxShowError("Invalid ID number!");
                return;
            }

            if (this.formulaFields.Count < (idx - 1))
            {
                AjaxShowError("Index out of range!");
                return;
            }

            int validationResults = ValidateFormula();
            if (validationResults == 0)
            {
                AjaxShowError("Error: The SQL text appears to be invalid!");
                return;
            }

            CUSTOMRP.Model.REPORTCOLUMN f = formulaFields[idx];
            f.COLUMNCOMMENT = fieldtext;
            switch (validationResults)
            {
                case 1:                   // 1: Absolutely not numeric
                    f.IS_NUMERIC = false;
                    break;
                case 2:                   // 2: Looks 'numeric'
                    f.IS_NUMERIC = true;
                    break;
            }

            AjaxReloadFormulaDlg();
        }

        private void AjaxUpdatelbformula()
        {
            string fieldname = Request.Params["fieldname"];
            string fieldtext = Request.Params["fieldtext"];

            int idx = 0;

            if (!Int32.TryParse(Request.Params["id"], out idx))
            {
                AjaxShowError("Invalid ID number!");
                return;
            }

            if (this.formulaFields.Count < (idx - 1))
            {
                AjaxShowError("Index out of range!");
                return;
            }

            for (int i = 0; i < formulaFields.Count; i++)
            {
                if ((i != idx) && (formulaFields[i].DISPLAYNAME == fieldname))
                {
                    AjaxShowError("Field name already exist!");
                    return;
                }
            }

            for (int i = 0; i < columninfos.Count; i++)
            {
                if ((i != idx) && (columninfos[i].ColName == fieldname))
                {
                    AjaxShowError("Field name cannot be the same as column name!");
                    return;
                }
            }

            int validationResults = ValidateFormula();
            if (validationResults == 0)
            {
                AjaxShowError("Error: The SQL text appears to be invalid!");
                return;
            }


            CUSTOMRP.Model.REPORTCOLUMN f = formulaFields[idx];
            f.COLUMNNAME = fieldname;
            f.DISPLAYNAME = fieldname;
            f.COLUMNCOMMENT = fieldtext;  //fieldText seems to be null

            switch (validationResults)
            {
                case 1:                                     // 1: Absolutely not numeric
                    f.IS_NUMERIC = false;
                    break;
                case 2:                                     // 2: Looks 'numeric'
                    f.IS_NUMERIC = true;
                    break;
            }

            Refresh();
        }

        private void Refresh()
        {
            string[] FieldNames = this.formulaFields.Select(x => x.DisplayName).ToArray();
            bool[] IsNumeric = this.formulaFields.Select(x => x.IS_NUMERIC).ToArray();

            FormulaReloadOptions payload = new FormulaReloadOptions()
            {
                fields = FieldNames,
                IsNumeric = IsNumeric
            };

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(FormulaReloadOptions));
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, payload);

            this.p_fSuppressRender = true;
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.ContentEncoding = Encoding.UTF8;
            ms.WriteTo(Response.OutputStream);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private void DeleteRefresh(string DeletedFormulaName)
        {
            //v1.8.8 Alex 2018.10.04 - Deleting an item should not validate fields, since the request body has only one field (the id).
            string[] FieldNames = this.formulaFields.Select(x => x.DisplayName).ToArray();

            FormulaReloadOptions payload = new FormulaReloadOptions()
            {
                fields = FieldNames,
                DeletedFormulaName = DeletedFormulaName
            };

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(FormulaReloadOptions));
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, payload);

            this.p_fSuppressRender = true;
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.ContentEncoding = Encoding.UTF8;
            ms.WriteTo(Response.OutputStream);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private void AjaxFormulaRemove()
        {
            int idx = 0;

            if (!Int32.TryParse(Request.Params["id"], out idx))
            {
                AjaxShowError("Invalid ID number!");
                return;
            }

            if (this.formulaFields.Count < (idx - 1))
            {
                AjaxShowError("Index out of range!");
                return;
            }
            string formulaName = formulaFields[idx].COLUMNNAME;

            //ListboxRemoveItem(this.lbcontents, formulaName);
            //ListboxRemoveItem(this.lbcriteria, formulaName);
            //ListboxRemoveItem(this.lbsorton, formulaName);
            //ListboxRemoveItem(this.lbsum, formulaName);
            //ListboxRemoveItem(this.lbgrouptotal, formulaName);
            //ListboxRemoveItem(this.lbavg, formulaName);
            //ListboxRemoveItem(this.lbgroupavg, formulaName);
            //ListboxRemoveItem(this.lbrpcount, formulaName);
            //ListboxRemoveItem(this.lbgroupcount, formulaName);

            formulaFields.RemoveAt(idx);
            DeleteRefresh(formulaName);

        }

        //private void ListboxRemoveItem(ListBox lb, string formulaName)
        //{
        //    if (lb.Items.Count > 0)
        //    lb.Items.RemoveAt(0);
        //    for(var i=lb.Items.Count - 1; i > -1; i--)
        //    {
        //        if (lb.Items[i].Text == formulaName)
        //        {
        //            lb.Items.RemoveAt(i);
        //        }
        //    }
        //}

        private void AjaxPushSelectionToSession()
        {
            string strpayload = Request.Params["payload"];
            ReportParameterContainer payload = null;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ReportParameterContainer));
            try
            {
                payload = (ReportParameterContainer)serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(strpayload)));
                //v1.0.0 - Cheong - 2015/05/29 - Add code to place formula in selected fields here.
                foreach (Fields f in payload.contentColumn)
                {
                    CUSTOMRP.Model.REPORTCOLUMN formula = this.formulaFields.Where(x => x.DISPLAYNAME == f.DisplayName).FirstOrDefault();
                    if (formula != null)
                    {
                        f.Formula = formula.COLUMNCOMMENT;
                        //v1.8.8 Alex 2018.10.04 Added IS_NUMERIC column - Start
                        f.IsNumeric = formula.IS_NUMERIC;
                        //v1.8.8 Alex 2018.10.04 Added IS_NUMERIC column - End
                    }
                }

                //v1.7.0 - Cheong - 2016/07/07 - Add checking for existing report name
                int rpid = 0;
                CUSTOMRP.Model.REPORT currentrp = (CUSTOMRP.Model.REPORT)Session[strSessionKeyMyReport];

                if (currentrp != null)
                {
                    rpid = currentrp.ID;
                }
                if (WebHelper.bllReport.CheckReportNameExist(me.ID, payload.ReportName, me.DatabaseID, rpid))
                {
                    AjaxShowError("Report name already in use.");
                }
                else
                {
                    Session[strSessionKeyReportParameterContainer] = payload;
                    AjaxShowError("OK");
                }

            }
            catch (Exception ex)
            {
                AjaxShowError(ex.Message);
            }
        }


        private void AjaxCopy()
        {
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(string));
            if (((CUSTOMRP.Model.REPORT)Session[strSessionKeyMyReport]).REPORTNAME.Length + 10 > 50)
            {
                serializer.WriteObject(ms, "_");
            }
            else {
                string oldId = Request.Params["id"];
                int newID = CUSTOMRP.BLL.AppHelper.CopyReport(me.ID, int.Parse(oldId));
                serializer.WriteObject(ms, "rpexcel.aspx?id=" + newID);
            }
            this.p_fSuppressRender = true;
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.ContentEncoding = Encoding.UTF8;
            ms.WriteTo(Response.OutputStream);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            //lblJavascript.Text = "<script type=\"text/javascript\"> location.replace(\"rpexcel.aspx?id=" + newID + "\"); </script>";
            //Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "rpexcel.aspx?id=" + newID);
            //Response.End();
            //Response.Redirect("rpexcel.aspx?id=" + newID, false);
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

    #region AjaxContainers

    [Serializable, DataContract]
    public class ReportParameterContainer
    {
        [DataMember]
        public string ReportName { get; set; }
        [DataMember]
        public string ReportTitle { get; set; }
        [DataMember]
        public int SVID { get; set; }
        [DataMember]
        public int ReportGroupID { get; set; }
        [DataMember]
        public int CategoryID { get; set; }
        [DataMember]
        public string ReportType { get; set; }
        [DataMember]
        public bool fHideCriteria { get; set; }
        //v1.2.0 - Cheong - 2016/07/04 - Add option to hide duplicate items
        [DataMember]
        public bool fHideDuplicate { get; set; }
        [DataMember]
        public int Format { get; set; }
        //v1.8.8 - Alex - 2018/09/21 - Add QueryParams - Begin
        [DataMember]
        public List<CUSTOMRP.BLL.AppHelper.QueryParamsObject> queryParams { get; set; }
        //v1.8.8 - Alex - 2018/09/21 - Add QueryParams - End
        [DataMember]
        public Fields[] contentColumn { get; set; }
        [DataMember]
        public Fields[] criteriaColumn { get; set; }
        [DataMember]
        public Fields[] sortonColumn { get; set; }
        [DataMember]
        public bool[] sortOrders { get; set; }
        [DataMember]
        public Fields[] groupColumn { get; set; }
        [DataMember]
        public Fields[] sumColumn { get; set; }
        [DataMember]
        public Fields[] grouptotalColumn { get; set; }
        [DataMember]
        public Fields[] avgColumn { get; set; }
        [DataMember]
        public Fields[] groupavgColumn { get; set; }
        [DataMember]
        public Fields[] rpcountColumn { get; set; }
        [DataMember]
        public Fields[] groupcountColumn { get; set; }
        [DataMember]
        public string ReportHeader { get; set; }
        [DataMember]
        public string ReportFooter { get; set; }
        [DataMember]
        public string FontFamily { get; set; }

        // for feeding report generation methods
        public string ExtendedFields
        {
            get
            {
                List<string> result = new List<string>();
                result.Add(this.ReportType);
                result.Add(this.fHideCriteria ? "1" : "0");
                result.Add(this.fHideDuplicate ? "1" : "0");
                return String.Join(",", result);
            }
        }

        public static bool LoadReport(int RPID, CUSTOMRP.Model.LoginUser me, out ReportParameterContainer container, out List<CUSTOMRP.Model.ColumnInfo> columninfos)
        {
            bool result = false;
            container = new ReportParameterContainer();
            columninfos = new List<CUSTOMRP.Model.ColumnInfo>();
            CUSTOMRP.Model.REPORT rpt = WebHelper.bllReport.GetModel(me.ID, RPID);
            CUSTOMRP.Model.SOURCEVIEW sv = WebHelper.bllSOURCEVIEW.GetModel(me.ID, rpt.SVID);
            if (rpt == null) { return false; }  // report does not exist

            bool updateSourceColumn = false;
            try
            {
                switch (sv.SourceType)
                {
                    case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.View:
                    case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.Table:
                        {
                            columninfos = CUSTOMRP.BLL.AppHelper.GetColumnInfoForTblView(me.ID, me.DatabaseNAME, sv.TBLVIEWNAME);
                        }
                        break;
                    case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.StoredProc:
                        {
                            columninfos = CUSTOMRP.BLL.AppHelper.GetColumnInfoForStoredProc(me.ID, me.DatabaseNAME, sv.ID);
                            if (columninfos.Any(x => x.DataType == null)) {
                                columninfos = CUSTOMRP.BLL.AppHelper.GetColumnInfoForStoredProcRefresh(me.ID, me.DatabaseNAME, sv.TBLVIEWNAME);
                                updateSourceColumn = true;
                            }
                        }
                        break;
                }

                container.ReportName = rpt.REPORTNAME;
                container.ReportTitle = rpt.RPTITLE;
                container.SVID = rpt.SVID;
                container.ReportGroupID = rpt.REPORTGROUPLIST;
                container.CategoryID = rpt.CATEGORY;
                //v1.7.0 - Cheong
                //container.fChangeValueOnly = rpt.fChangeOnly;
                container.ReportType = rpt.ReportType;
                container.fHideCriteria = rpt.fHideCriteria;
                //v1.2.0 - Cheong - 2016/07/04 - Add option to hide duplicate items
                container.fHideDuplicate = rpt.fHideDuplicate;
                container.Format = rpt.DEFAULTFORMAT;
                //v1.2.0 Kim 2016.11.02 move to printconfig
                ////v1.0.0 - Cheong - 2016/02/29 - Add UI for Report Header and Footer
                //container.ReportHeader = rpt.REPORT_HEADER;
                //container.ReportFooter = rpt.REPORT_FOOTER;
                //container.FontFamily = rpt.FONT_FAMILY;

                container.contentColumn = rpt.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content).Select(x => new Fields()
                {
                    ColumnName = x.COLUMNNAME,
                    DisplayName = x.DISPLAYNAME,
                    Formula = (x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Formula) ? x.COLUMNCOMMENT : null,
                }).ToArray();

                container.sortonColumn = rpt.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.SortOn).Select(x => new Fields() { ColumnName = x.COLUMNNAME }).ToArray();
                container.sortOrders = rpt.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.SortOn).Select(x => x.IS_ASCENDING).ToArray();
                container.groupColumn = rpt.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Group).Select(x => new Fields() { ColumnName = x.COLUMNNAME }).ToArray();
                container.sumColumn = rpt.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Sum).Select(x => new Fields() { ColumnName = x.COLUMNNAME }).ToArray();
                container.grouptotalColumn = rpt.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.GroupSum).Select(x => new Fields() { ColumnName = x.COLUMNNAME }).ToArray();
                container.avgColumn = rpt.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Avg).Select(x => new Fields() { ColumnName = x.COLUMNNAME }).ToArray();
                container.groupavgColumn = rpt.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.GroupAvg).Select(x => new Fields() { ColumnName = x.COLUMNNAME }).ToArray();
                container.rpcountColumn = rpt.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Count).Select(x => new Fields() { ColumnName = x.COLUMNNAME }).ToArray();
                container.groupcountColumn = rpt.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.GroupCount).Select(x => new Fields() { ColumnName = x.COLUMNNAME }).ToArray();

                container.criteriaColumn = rpt.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Criteria).Select(x => new Fields()
                {
                    ColumnName = x.COLUMNNAME,
                    //v1.8.6 Ben 2018.06.14 - DISPLAYNAME can be blank but DisplayName is not
                    //v1.7.0 Ben 2017.12.20 - DISPLAYNAME is added as it can be used later in rpexcel2.aspx
                    //DisplayName = x.DISPLAYNAME,
                    DisplayName = x.DisplayName,
                    Operator = (x.CRITERIA2 == "r1") ? x.CRITERIA3 : "BETWEEN",
                    Value1 = (x.CRITERIA2 == "r1") ? x.CRITERIA4 : x.CRITERIA3,
                    Value2 = (x.CRITERIA2 == "r2") ? x.CRITERIA4 : String.Empty,
                }).ToArray();
                result = true;

                if (updateSourceColumn)
                {
                    CUSTOMRP.Model.SOURCEVIEWCOLUMN[] updatedColumns = columninfos.Select(x => new CUSTOMRP.Model.SOURCEVIEWCOLUMN
                    {
                        SVID = sv.ID,
                        COLUMNTYPE = 1,
                        COLUMNCOMMENT = String.Empty,
                        HIDDEN = false,
                        COLUMNNAME = x.ColName,
                        DISPLAYNAME = String.Empty,
                        DATA_TYPE = x.DataType
                    }).ToArray();

                    WebHelper.bllSOURCEVIEWCOLUMN.UpdateModelList(me.ID, updatedColumns);
                }
            }
            catch
            {
                // simply return false here
            }
            return result;
        }

        public CUSTOMRP.Model.REPORT GetReportModel(CUSTOMRP.Model.LoginUser me, List<CUSTOMRP.Model.ColumnInfo> columninfos = null, int rpid = 0)
        {
            List<CUSTOMRP.Model.REPORTCOLUMN> dbcols = (rpid == 0) ? new List<CUSTOMRP.Model.REPORTCOLUMN>() : WebHelper.bllReport.GetReportColumnModelListForReport(me.ID, rpid);
            List<CUSTOMRP.Model.REPORTCOLUMN> cols = new List<CUSTOMRP.Model.REPORTCOLUMN>();
            List<CUSTOMRP.Model.SOURCEVIEWCOLUMN> svcs = (this.SVID == 0) ? new List<CUSTOMRP.Model.SOURCEVIEWCOLUMN>() : WebHelper.bllSOURCEVIEWCOLUMN.GetModelsForSourceView(me.ID, SVID);
            CUSTOMRP.Model.REPORTCOLUMN srcCol = null;
            CUSTOMRP.Model.SOURCEVIEWCOLUMN svCol = null;
            CUSTOMRP.Model.ColumnInfo ci = null;
            DateTime currentTime = DateTime.Now;

            CUSTOMRP.Model.REPORT SourceRpt = null;
            if (rpid > 0)
            {
                SourceRpt = WebHelper.bllReport.GetModel(me.ID, rpid);
            }

            #region Simple Columns

            #region Contents

            foreach (Fields f in this.contentColumn)
            {
                srcCol = dbcols.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.COLUMNFUNC == (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content)).FirstOrDefault();
                svCol = svcs.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.ColumnType == CUSTOMRP.Model.SOURCEVIEWCOLUMN.ColumnTypes.Normal)).FirstOrDefault();
                ci = (columninfos != null) ? columninfos.Where(x => x.ColName == f.ColumnName).FirstOrDefault() : null;
                if (srcCol == null)
                {
                    srcCol = new CUSTOMRP.Model.REPORTCOLUMN()
                    {
                        RPID = rpid,
                        COLUMNNAME = f.ColumnName,
                        COLUMNFUNC = (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content,
                        AUDODATE = currentTime,
                        SOURCEVIEWCOLUMNID = (svCol == null) ? -1 : svCol.ID,
                        DISPLAYNAME = (ci != null) ? ci.DisplayName : f.ColumnName,
                        //COLUMNTYPE = 1,
                        COLUMNTYPE = (String.IsNullOrWhiteSpace(f.Formula)) ? 1 : 2,  // if not formula, there should be columninfo
                        COLUMNCOMMENT = (String.IsNullOrWhiteSpace(f.Formula)) ? String.Empty : f.Formula,
                        //v1.0.0 - Cheong - 2016/02/25 - Add EXCEL_COLWIDTH
                        EXCEL_COLWIDTH = -1,    // default
                        FONT_SIZE = null,    // default
                        FONT_BOLD = false,    // default
                        FONT_ITALIC = false,    // default
                        HORIZONTAL_TEXT_ALIGN = 0,    // default
                        CELL_FORMAT = "",    // default
                        BACKGROUND_COLOR = "",    // default
                        FONT_COLOR = "",    // default
                        IS_NUMERIC = f.IsNumeric,
                        IS_ASCENDING = true,
                    };
                }
                cols.Add(srcCol);
            }

            #endregion

            #region SortOn

            foreach (Fields f in this.sortonColumn)
            {
                srcCol = dbcols.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.COLUMNFUNC == (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.SortOn) && (x.IS_ASCENDING == f.IsAscending)).FirstOrDefault();
                svCol = svcs.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.ColumnType == CUSTOMRP.Model.SOURCEVIEWCOLUMN.ColumnTypes.Normal)).FirstOrDefault();
                ci = (columninfos != null) ? columninfos.Where(x => x.ColName == f.ColumnName).FirstOrDefault() : null;
                if (srcCol == null)
                {
                    srcCol = new CUSTOMRP.Model.REPORTCOLUMN()
                    {
                        RPID = rpid,
                        COLUMNNAME = f.ColumnName,
                        COLUMNFUNC = (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.SortOn,
                        AUDODATE = currentTime,
                        SOURCEVIEWCOLUMNID = (svCol == null) ? -1 : svCol.ID,
                        DISPLAYNAME = (ci != null) ? ci.DisplayName : f.ColumnName,
                        COLUMNTYPE = 1,
                        COLUMNCOMMENT = String.Empty,
                        IS_ASCENDING = f.IsAscending,
                    };
                }
                cols.Add(srcCol);

                //srcCol = dbcols.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.COLUMNFUNC == (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.SortOn)).FirstOrDefault();
                //svCol = svcs.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.ColumnType == CUSTOMRP.Model.SOURCEVIEWCOLUMN.ColumnTypes.Normal)).FirstOrDefault();
                //ci = (columninfos != null) ? columninfos.Where(x => x.ColName == f.ColumnName).FirstOrDefault() : null;
                //if (srcCol == null)
                //{
                //    srcCol = new CUSTOMRP.Model.REPORTCOLUMN()
                //    {
                //        RPID = rpid,
                //        COLUMNNAME = f.ColumnName,
                //        COLUMNFUNC = (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.SortOn,
                //        AUDODATE = currentTime,
                //        SOURCEVIEWCOLUMNID = (svCol == null) ? -1 : svCol.ID,
                //        DISPLAYNAME = (ci != null) ? ci.DisplayName : f.ColumnName,
                //        COLUMNTYPE = 1,
                //        COLUMNCOMMENT = String.Empty,
                //        IS_ASCENDING = f.IsAscending,
                //    };
                //}
                //cols.Add(srcCol);
            }

            #endregion

            #region Group

            if (this.groupColumn != null)
            {
                foreach (Fields f in this.groupColumn)
                {
                    srcCol = dbcols.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.COLUMNFUNC == (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Group)).FirstOrDefault();
                    svCol = svcs.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.ColumnType == CUSTOMRP.Model.SOURCEVIEWCOLUMN.ColumnTypes.Normal)).FirstOrDefault();
                    ci = (columninfos != null) ? columninfos.Where(x => x.ColName == f.ColumnName).FirstOrDefault() : null;
                    if (srcCol == null)
                    {
                        srcCol = new CUSTOMRP.Model.REPORTCOLUMN()
                        {
                            RPID = rpid,
                            COLUMNNAME = f.ColumnName,
                            COLUMNFUNC = (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Group,
                            AUDODATE = currentTime,
                            SOURCEVIEWCOLUMNID = (svCol == null) ? -1 : svCol.ID,
                            DISPLAYNAME = (ci != null) ? ci.DisplayName : f.ColumnName,
                            COLUMNTYPE = 1,
                            COLUMNCOMMENT = String.Empty,
                        };
                    }
                    cols.Add(srcCol);
                }
            }

            #endregion

            #region Sum

            if (this.sumColumn != null)
            {
                foreach (Fields f in this.sumColumn)
                {
                    srcCol = dbcols.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.COLUMNFUNC == (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Sum)).FirstOrDefault();
                    svCol = svcs.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.ColumnType == CUSTOMRP.Model.SOURCEVIEWCOLUMN.ColumnTypes.Normal)).FirstOrDefault();
                    ci = (columninfos != null) ? columninfos.Where(x => x.ColName == f.ColumnName).FirstOrDefault() : null;
                    if (srcCol == null)
                    {
                        srcCol = new CUSTOMRP.Model.REPORTCOLUMN()
                        {
                            RPID = rpid,
                            COLUMNNAME = f.ColumnName,
                            COLUMNFUNC = (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Sum,
                            AUDODATE = currentTime,
                            SOURCEVIEWCOLUMNID = (svCol == null) ? -1 : svCol.ID,
                            DISPLAYNAME = (ci != null) ? ci.DisplayName : f.ColumnName,
                            COLUMNTYPE = 1,
                            COLUMNCOMMENT = String.Empty,
                        };
                    }
                    cols.Add(srcCol);
                }
            }

            #endregion

            #region Group Total

            if (this.grouptotalColumn != null)
            {
                foreach (Fields f in this.grouptotalColumn)
                {
                    srcCol = dbcols.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.COLUMNFUNC == (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.GroupSum)).FirstOrDefault();
                    svCol = svcs.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.ColumnType == CUSTOMRP.Model.SOURCEVIEWCOLUMN.ColumnTypes.Normal)).FirstOrDefault();
                    ci = (columninfos != null) ? columninfos.Where(x => x.ColName == f.ColumnName).FirstOrDefault() : null;
                    if (srcCol == null)
                    {
                        srcCol = new CUSTOMRP.Model.REPORTCOLUMN()
                        {
                            RPID = rpid,
                            COLUMNNAME = f.ColumnName,
                            COLUMNFUNC = (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.GroupSum,
                            AUDODATE = currentTime,
                            SOURCEVIEWCOLUMNID = (svCol == null) ? -1 : svCol.ID,
                            DISPLAYNAME = (ci != null) ? ci.DisplayName : f.ColumnName,
                            COLUMNTYPE = 1,
                            COLUMNCOMMENT = String.Empty,
                        };
                    }
                    cols.Add(srcCol);
                }
            }

            #endregion

            #region Avg

            if (this.avgColumn != null)
            {
                foreach (Fields f in this.avgColumn)
                {
                    srcCol = dbcols.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.COLUMNFUNC == (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Avg)).FirstOrDefault();
                    svCol = svcs.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.ColumnType == CUSTOMRP.Model.SOURCEVIEWCOLUMN.ColumnTypes.Normal)).FirstOrDefault();
                    ci = (columninfos != null) ? columninfos.Where(x => x.ColName == f.ColumnName).FirstOrDefault() : null;
                    if (srcCol == null)
                    {
                        srcCol = new CUSTOMRP.Model.REPORTCOLUMN()
                        {
                            RPID = rpid,
                            COLUMNNAME = f.ColumnName,
                            COLUMNFUNC = (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Avg,
                            AUDODATE = currentTime,
                            SOURCEVIEWCOLUMNID = (svCol == null) ? -1 : svCol.ID,
                            DISPLAYNAME = (ci != null) ? ci.DisplayName : f.ColumnName,
                            COLUMNTYPE = 1,
                            COLUMNCOMMENT = String.Empty,
                        };
                    }
                    cols.Add(srcCol);
                }
            }

            #endregion

            #region Group Avg

            if (this.groupavgColumn != null)
            {
                foreach (Fields f in this.groupavgColumn)
                {
                    srcCol = dbcols.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.COLUMNFUNC == (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.GroupAvg)).FirstOrDefault();
                    svCol = svcs.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.ColumnType == CUSTOMRP.Model.SOURCEVIEWCOLUMN.ColumnTypes.Normal)).FirstOrDefault();
                    ci = (columninfos != null) ? columninfos.Where(x => x.ColName == f.ColumnName).FirstOrDefault() : null;
                    if (srcCol == null)
                    {
                        srcCol = new CUSTOMRP.Model.REPORTCOLUMN()
                        {
                            RPID = rpid,
                            COLUMNNAME = f.ColumnName,
                            COLUMNFUNC = (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.GroupAvg,
                            AUDODATE = currentTime,
                            SOURCEVIEWCOLUMNID = (svCol == null) ? -1 : svCol.ID,
                            DISPLAYNAME = (ci != null) ? ci.DisplayName : f.ColumnName,
                            COLUMNTYPE = 1,
                            COLUMNCOMMENT = String.Empty,
                        };
                    }
                    cols.Add(srcCol);
                }
            }

            #endregion

            #region RP Count

            if (this.rpcountColumn != null)
            {
                foreach (Fields f in this.rpcountColumn)
                {
                    srcCol = dbcols.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.COLUMNFUNC == (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Count)).FirstOrDefault();
                    svCol = svcs.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.ColumnType == CUSTOMRP.Model.SOURCEVIEWCOLUMN.ColumnTypes.Normal)).FirstOrDefault();
                    ci = (columninfos != null) ? columninfos.Where(x => x.ColName == f.ColumnName).FirstOrDefault() : null;
                    if (srcCol == null)
                    {
                        srcCol = new CUSTOMRP.Model.REPORTCOLUMN()
                        {
                            RPID = rpid,
                            COLUMNNAME = f.ColumnName,
                            COLUMNFUNC = (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Count,
                            AUDODATE = currentTime,
                            SOURCEVIEWCOLUMNID = (svCol == null) ? -1 : svCol.ID,
                            DISPLAYNAME = (ci != null) ? ci.DisplayName : f.ColumnName,
                            COLUMNTYPE = 1,
                            COLUMNCOMMENT = String.Empty,
                        };
                    }
                    cols.Add(srcCol);
                }
            }

            #endregion

            #region Group Count

            if (this.groupcountColumn != null)
            {
                foreach (Fields f in this.groupcountColumn)
                {
                    srcCol = dbcols.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.COLUMNFUNC == (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.GroupCount)).FirstOrDefault();
                    svCol = svcs.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.ColumnType == CUSTOMRP.Model.SOURCEVIEWCOLUMN.ColumnTypes.Normal)).FirstOrDefault();
                    ci = (columninfos != null) ? columninfos.Where(x => x.ColName == f.ColumnName).FirstOrDefault() : null;
                    if (srcCol == null)
                    {
                        srcCol = new CUSTOMRP.Model.REPORTCOLUMN()
                        {
                            RPID = rpid,
                            COLUMNNAME = f.ColumnName,
                            COLUMNFUNC = (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.GroupCount,
                            AUDODATE = currentTime,
                            SOURCEVIEWCOLUMNID = (svCol == null) ? -1 : svCol.ID,
                            DISPLAYNAME = (ci != null) ? ci.DisplayName : f.ColumnName,
                            COLUMNTYPE = 1,
                            COLUMNCOMMENT = String.Empty,
                        };
                    }
                    cols.Add(srcCol);
                }
            }

            #endregion

            #endregion

            #region Criteria

            foreach (Fields f in this.criteriaColumn)
            {
                srcCol = dbcols.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.COLUMNFUNC == (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Criteria)).FirstOrDefault();
                svCol = svcs.Where(x => (x.COLUMNNAME == f.ColumnName) && (x.ColumnType == CUSTOMRP.Model.SOURCEVIEWCOLUMN.ColumnTypes.Normal)).FirstOrDefault();
                ci = (columninfos != null) ? columninfos.Where(x => x.ColName == f.ColumnName).FirstOrDefault() : null;
                if (srcCol == null)
                {
                    srcCol = new CUSTOMRP.Model.REPORTCOLUMN()
                    {
                        RPID = rpid,
                        COLUMNNAME = f.ColumnName,
                        COLUMNFUNC = (int)CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Criteria,
                        AUDODATE = currentTime,
                        SOURCEVIEWCOLUMNID = (svCol == null) ? -1 : svCol.ID,
                        DISPLAYNAME = (ci != null) ? ci.DisplayName : f.ColumnName,
                        COLUMNTYPE = 1,
                        COLUMNCOMMENT = String.Empty,
                        CRITERIA2 = "r1",
                        CRITERIA3 = "Begins With",
                        CRITERIA4 = String.Empty,
                        IS_NUMERIC = true,
                        IS_ASCENDING = true,
                    };
                }
                cols.Add(srcCol);
            }

            #endregion

            return new CUSTOMRP.Model.REPORT()
            {
                ID = rpid,
                DATABASEID = Convert.ToInt32(me.DatabaseID),
                SVID = this.SVID,
                REPORTNAME = this.ReportName,
                AUDODATE = DateTime.Now,
                CATEGORY = this.CategoryID,
                REPORTGROUPLIST = this.ReportGroupID,
                RPTITLE = this.ReportTitle,
                ADDUSER = me.ID,
                DEFAULTFORMAT = this.Format,
                EXTENDFIELD = this.ExtendedFields,
                //v1.0.0 - Cheong - 2016/02/25 - Handle PRINT_FITTOPAGE and PrintOrientation in container.GerReportModel()
                //v1.0.0 - Cheong - 2016/02/25 - Default value for PRINT_FITTOPAGE = 1
                PRINT_FITTOPAGE = SourceRpt == null ? (short)1 : SourceRpt.PRINT_FITTOPAGE,
                PrintOrientation = SourceRpt == null ? CUSTOMRP.Model.REPORT.Orientation.NotSet : SourceRpt.PrintOrientation,
                ReportColumns = cols,
                //v1.2.0 Kim 2016.11.02 move to printconfig
                //v1.0.0 - Cheong - 2016/02/25 - Add field to store Report Header and footer settings for reports
                //REPORT_HEADER = ProcessMultilineText(this.ReportHeader) ?? (SourceRpt == null ? String.Empty : SourceRpt.REPORT_HEADER),
                //REPORT_FOOTER = ProcessMultilineText(this.ReportFooter) ?? (SourceRpt == null ? String.Empty : SourceRpt.REPORT_FOOTER),
                REPORT_HEADER = SourceRpt == null ? String.Empty : SourceRpt.REPORT_HEADER,
                REPORT_FOOTER = SourceRpt == null ? String.Empty : SourceRpt.REPORT_FOOTER,
                SUBCOUNT_LABEL = SourceRpt == null ? "Sub Count" : SourceRpt.SUBCOUNT_LABEL,
                FONT_FAMILY = SourceRpt == null ? String.Empty : SourceRpt.FONT_FAMILY,
                //v1.6.9 - Cheong - 2016/05/31 - Also fetch WordFile settings here
                WordFile = (SourceRpt == null) ? null : SourceRpt.WordFile,
            };
        }

        //private string ProcessMultilineText(string input)
        //{
        //    return input == null ? null : input.Replace("\r\n", "\n").TrimEnd();
        //}

        // should be done in REPORT instead
        #region old code
        /*
        public bool SaveReport(CUSTOMRP.Model.REPORT myReport, out string ErrorMessage)
        {
            bool result = false;
            ErrorMessage = String.Empty;

            if (myReport == null) {
                ErrorMessage = "Session expired.";
                return false;
            }
            if (this.contentColumn.Count() == 0)
            {
                ErrorMessage = "No column is selected.";
                return false;
            }
            CUSTOMRP.Model.SOURCEVIEWCOLUMN[] svc = WebHelper.bllSOURCEVIEWCOLUMN.GetModelsForSourceView(myReport.SVID).ToArray();
            if (svc.Length == 0)
            {
                ErrorMessage = "Specified source query defination does not exist.";
                return false;
            }

            #region Save report

            try
            {
                if (myReport.ID == 0)
                {
                    myReport.ID = WebHelper.bllReport.Add(myReport);
                }
                else
                {
                    WebHelper.bllReport.Update(myReport);
                }
            }
            catch
            {
                ErrorMessage = "Error occurs when saving Report master data.";
            }

            #endregion

            #region Save ReportColumn

            if (String.IsNullOrEmpty(ErrorMessage))
            {
                try
                {
                    foreach (Fields f in this.contentColumn)
                    {
                        WebHelper.bllReportColumn.Replace(new CUSTOMRP.Model.REPORTCOLUMN()
                            {
                                
                            });
                    }
                    result = true;
                }
                catch
                {
                    ErrorMessage = "Error occurs when saving Report columns.";
                }
            }

            #endregion

            if ((!result) && (String.IsNullOrEmpty(ErrorMessage)))
            {
                ErrorMessage = "Unspecified error occurs";
            }

            return result;
        }
        */

        #endregion
    }

    [Serializable, DataContract]
    public class Fields
    {
        [DataMember]
        public string ColumnName { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public string Operator { get; set; }
        [DataMember]
        public string Value1 { get; set; }
        [DataMember]
        public string Value2 { get; set; }
        [DataMember]
        public string Formula { get; set; }
        [DataMember]
        public bool IsNumeric { get; set; }
        [DataMember]
        public bool IsAscending { get; set; }
        public string SelectStatement
        {
            get
            {
                return String.IsNullOrWhiteSpace(Formula) ? String.Format("[{0}]", this.ColumnName) : String.Format("[{0}] = {1}", this.DisplayName, this.Formula);
            }
        }
    }

    [Serializable, DataContract]
    public class DlgParam
    {
        [DataMember]
        public int Idx { get; set; }
        [DataMember]
        public string FieldName { get; set; }
        [DataMember]
        public string RenderText { get; set; }
        //[DataMember]
        //public bool IsNumeric { get; set; }
    }

    [Serializable, DataContract]
    public class ColumnReloadOptions
    {
        [DataMember]
        public Fields[] fields { get; set; }
    }

    [Serializable, DataContract]
    public class FormulaReloadOptions
    {
        [DataMember]
        public string[] fields { get; set; }
        [DataMember]
        public bool[] IsNumeric { get; set; }
        [DataMember]
        public string DeletedFormulaName { get; set; }
    }

    #endregion
}