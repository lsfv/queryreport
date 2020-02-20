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

namespace QueryReport
{
    public partial class rpexcel_bak : QueryReport.Code.LoginUserPage
    {
        /*
        public const string strSessionKeyColumnInfo = "__SESSION_REPORT_ColumnInfo";
        public const string strSessionKeyFormulaFields = "__SESSION_REPORT_FormulaFields";

        private CUSTOMRP.Model.REPORT myReport = null;
        private List<CUSTOMRP.Model.REPORTCOLUMN> reportcolumns = null;
        private List<CUSTOMRP.Model.ColumnInfo> columninfos = null;
        private List<CUSTOMRP.Model.FORMULAFIELD> formulaFields = null;
        private bool p_fSuppressRender = false;     // Whether to render page contents

        #region Event Handlers

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsOnSubmitStatementRegistered("pagesubmit"))
            {
                Page.ClientScript.RegisterOnSubmitStatement(this.GetType(), "pagesubmit", "return pageSubmission();");
            }
            if (string.IsNullOrEmpty(Request.QueryString["id"]) == false)
            {
                int id = Int32.Parse(Request.QueryString["id"]);
                myReport = WebHelper.bllReport.GetModel(id);

                reportcolumns = WebHelper.bllReportColumn.GetModelList("rc.RPID='" + id + "'");
                this.btnDelete.Visible = true;
                if (myReport == null)
                {
                    Common.JScript.AlertAndRedirect(QueryReport.Code.AppNum.ErrorMsg.parameter_error, "rplist.aspx");
                    Response.End();
                }
            }

            if (Session[strSessionKeyColumnInfo] != null)
            {
                columninfos = (List<CUSTOMRP.Model.ColumnInfo>)Session[strSessionKeyColumnInfo];
            }

            if (Session[strSessionKeyFormulaFields] != null)
            {
                formulaFields = (List<CUSTOMRP.Model.FORMULAFIELD>)Session[strSessionKeyFormulaFields];
            }
            else
            {
                formulaFields = new List<CUSTOMRP.Model.FORMULAFIELD>();
            }

            // testing formula
            if (Session[strSessionKeyFormulaFields] == null)
            {
                CUSTOMRP.Model.FORMULAFIELD f = new CUSTOMRP.Model.FORMULAFIELD();
                f.FIELDNAME = "TestAdd";
                f.FUNCTION = 1;
                f.Params.Add(new CUSTOMRP.Model.FORMULAPARAM()
                {
                    PARAMTYPE = 0,
                    VALUE1 = "1"
                });
                f.Params.Add(new CUSTOMRP.Model.FORMULAPARAM()
                {
                    PARAMTYPE = 0,
                    VALUE1 = "1"
                });

                formulaFields.Add(f);
                Session[strSessionKeyFormulaFields] = formulaFields;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ProcessAjaxRequestParameters()) { return; } // stop further process if it's Ajax event
            if (!IsPostBack)
            {
                this.FillddlQueryNames();

                DataTable mydtCategory = WebHelper.bllcategory.GetList(100000, "DATABASEID='" + me.DatabaseID + "'", "NAME").Tables[0];

                this.ddlCategory.DataSource = mydtCategory;
                this.ddlCategory.DataTextField = "NAME";
                this.ddlCategory.DataValueField = "ID";
                this.ddlCategory.DataBind();

                DataTable mydtRPGROUP = WebHelper.bllrpGroup.GetList(100000, "DATABASEID='" + me.DatabaseID + "'", "NAME").Tables[0];
                this.ddlReportGroup.DataSource = mydtRPGROUP;
                this.ddlReportGroup.DataTextField = "NAME";
                this.ddlReportGroup.DataValueField = "ID";
                this.ddlReportGroup.DataBind();

                CUSTOMRP.Model.SOURCEVIEW sv = null;
                if (myReport!=null)
                {
                    CUSTOMRP.BLL.SOURCEVIEW svBLL = new CUSTOMRP.BLL.SOURCEVIEW();
                    sv = svBLL.GetModel(myReport.SVID);
                    this.txtReportName.Text = myReport.REPORTNAME;
                    this.txtReportTitle.Text = myReport.RPTITLE;
                    this.ddlFormat.SelectedValue = myReport.DEFAULTFORMAT.ToString();
                    this.ddlQueryName.SelectedValue = myReport.SVID.ToString();
                    this.ddlCategory.SelectedValue = myReport.CATEGORY.ToString();
                    this.ddlReportGroup.SelectedValue = myReport.REPORTGROUPLIST.ToString();
                    this.ddlShowType.SelectedValue = myReport.fChangeOnly ? "1" : "0";

                    this.ShowTypeChanged();
                }

                if (this.ddlQueryName.SelectedItem != null)
                {
                    this.ddlQueryNameChange();
                }
                else{this.ddlQueryName.Items.Add(new ListItem("N/A", "")); }

                #region get column names

                string[] colnames = null;
                if (sv == null)
                {
                    sv = QueryReport.Code.WebHelper.bllSOURCEVIEW.GetModel(Int32.Parse(this.ddlQueryName.SelectedValue));
                }
                colnames = QueryReport.Code.WebHelper.bllSOURCEVIEWCOLUMN.GetModelsForSourceView(sv.ID).OrderBy(x => x.DisplayName).Select(x => x.DisplayName).ToArray();
                if (colnames == null)
                {
                    switch (sv.SourceType)
                    {
                        case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.View:
                        case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.Table:
                            {
                                colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForTblView(me.DatabaseNAME, sv.TBLVIEWNAME);
                            }
                            break;
                        case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.StoredProc:
                            {
                                colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForStoredProc(me.DatabaseNAME, sv.TBLVIEWNAME);
                            }
                            break;
                    }
                }

                #endregion

                if (reportcolumns != null)
                {
                    foreach (CUSTOMRP.Model.REPORTCOLUMN dr in reportcolumns)
                    {
                        //v1.0.0 - Cheong - 2015/03/31 - Filter missing columns from list when running reports
                        if (colnames.Contains(dr.COLUMNNAME))
                        {
                            if (dr.COLUMNFUNC == 1)
                            {
                                this.lbcontents.Items.Add(new ListItem(dr.COLUMNNAME, dr.COLUMNNAME));
                            }
                            else if (dr.COLUMNFUNC == 2)
                            {
                                this.lbcriteria.Items.Add(new ListItem(dr.COLUMNNAME, dr.COLUMNNAME));
                            }
                            else if (dr.COLUMNFUNC == 3)
                            {
                                this.lbsorton.Items.Add(new ListItem(dr.COLUMNNAME, dr.COLUMNNAME));
                            }
                            else if (dr.COLUMNFUNC == 4)
                            {
                                this.lbavg.Items.Add(new ListItem(dr.COLUMNNAME, dr.COLUMNNAME));
                            }
                            else if (dr.COLUMNFUNC == 5)
                            {
                                this.lbsum.Items.Add(new ListItem(dr.COLUMNNAME, dr.COLUMNNAME));
                            }
                            else if (dr.COLUMNFUNC == 6)
                            {
                                this.lbhiden.Items.Add(new ListItem(dr.COLUMNNAME, dr.COLUMNNAME));
                            }
                            else if (dr.COLUMNFUNC == 7)
                            {
                                this.lbgrouptotal.Items.Add(new ListItem(dr.COLUMNNAME, dr.COLUMNNAME));
                            }
                            else if (dr.COLUMNFUNC == 8)
                            {
                                this.lbgroupavg.Items.Add(new ListItem(dr.COLUMNNAME, dr.COLUMNNAME));
                            }
                            else if (dr.COLUMNFUNC == 9)
                            {
                                this.lbgroupcount.Items.Add(new ListItem(dr.COLUMNNAME, dr.COLUMNNAME));
                            }
                            else if (dr.COLUMNFUNC == 10)
                            {
                                this.lbrpcount.Items.Add(new ListItem(dr.COLUMNNAME, dr.COLUMNNAME));
                            }
                        }
                    }
                }
            }
            else{this.lblErrorText.Text = "";}

            //v1.0.0 - Cheong - 2015/03/31 if no right to modify, go directly to rpexcel2.aspx
            if (!me.rp_modify)
            {
                Server.Transfer("rpexcel2.aspx", true);
            }
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

        protected void ddlShowType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ShowTypeChanged();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (lbcontents.Items.Count <= 0)
            {
                Common.JScript.Alert("Please select content columns.");
                Common.JScript.GoHistory(-1);
                Response.End();
            }
            if (myReport == null)
            {
                if (QueryReport.Code.WebHelper.bllReport.GetList(" reportname='" + this.txtReportName.Text + "'  AND DATABASEID='" + me.DatabaseID + "'").Tables[0].Rows.Count > 0)
                {
                    Common.JScript.Alert(QueryReport.Code.AppNum.ErrorMsg.Commonexits);
                    Common.JScript.GoHistory(-1);
                    Response.End();
                }
            }
            Server.Transfer("rpexcel2.aspx", true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (me.rp_delete)
            {
                if (myReport != null)
                {
                    string rpid = Request.QueryString["id"];
                    string sql_delete = "delete from [report] where [id]=" + rpid + "; delete from [REPORTCOLUMN] where [rpid]=" + rpid;
                    WebHelper.bllCommon.executesql(sql_delete);
                }
                Response.Redirect("rplist.aspx", false);
            }
            else
            {
                Common.JScript.Alert(QueryReport.Code.AppNum.ErrorMsg.accesserror);
                Common.JScript.GoHistory(-1);
                Response.End();
            }
        }
        #endregion

        #region Helper Methods

        private bool ProcessAjaxRequestParameters()
        {
            bool result = false;

            string p_strAction = (Request.Params["action"] ?? String.Empty).ToLower();

            switch (p_strAction)
            {
                case "reloadformuladlg":
                    {
                        this.AjaxReloadFormulaDlg();
                        result = true;
                    }
                    break;
                case "updatefieldname":
                    {
                        this.AjaxUpdateFieldName();
                        result = true;
                    }
                    break;
                case "paramfuncchg":
                    {
                        this.AjaxParamFuncChg();
                        result = true;
                    }
                    break;
                case "paramtextchg":
                    {
                        this.AjaxParamTextChg();
                        result = true;
                    }
                    break;
                case "paramtoformula":
                    {
                        this.AjaxParamTypeChg();
                        result = true;
                    }
                    break;
                case "paramadd":
                    {
                        this.AjaxParamAdd();
                        result = true;
                    }
                    break;
                case "paramremove":
                    {
                        this.AjaxParamRemove();
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
            }

            return result;
        }

        private void FillddlQueryNames()
        {
            //DataTable myDt = WebHelper.bllSOURCEVIEW.GetList(100000, "[VIEWLEVEL]>='" + me.ViewLevel + "' AND DATABASEID='" + me.DatabaseID + "' AND SOURCETYPE = '" + this.ddlQueryType.SelectedValue + "' and (FORMATTYPE='1' or FORMATTYPE='3')", "SOURCEVIEWNAME").Tables[0];
            //DataColumn Mycolumn = new DataColumn("NEWDesc");
            //Mycolumn.DataType = typeof(string);
            //Mycolumn.Expression = "[SOURCEVIEWNAME] + '  |  ' +[DESC]";
            //myDt.Columns.Add(Mycolumn);
            DataTable myDt = WebHelper.bllSOURCEVIEW.GetQueryListForDropdown(me).Tables[0];

            this.ddlQueryName.DataSource = myDt;
            this.ddlQueryName.DataTextField = "NEWDesc";
            this.ddlQueryName.DataValueField = "ID";
            this.ddlQueryName.DataBind();
        }

        private void ShowTypeChanged()
        {
            if (this.ddlShowType.SelectedValue == "0")//all--->
            {
                this.lbgroupavg.BackColor = System.Drawing.Color.White;
                this.lbgroupcount.BackColor = System.Drawing.Color.White;
                this.lbgrouptotal.BackColor = System.Drawing.Color.White;
            }
            else
            {
                this.lbgroupavg.BackColor = System.Drawing.Color.Silver;
                this.lbgroupcount.BackColor = System.Drawing.Color.Silver;
                this.lbgrouptotal.BackColor = System.Drawing.Color.Silver;
            }
        }

        private void up(ListBox lb)
        {
            int[] selectedArray = lb.GetSelectedIndices();
            ListItem li = null;
            for (int i = 0; i < selectedArray.Length; i++)
            {
                if (selectedArray[i] != i)
                {
                    li = lb.Items[selectedArray[i]];
                    lb.Items.RemoveAt(selectedArray[i]);
                    lb.Items.Insert(selectedArray[i] - 1, li);
                }
            }
        }

        private void down(ListBox lb)
        {
            int[] selectedArray = lb.GetSelectedIndices();
            ListItem li = null;
            for (int i = selectedArray.Length - 1; i > -1; i--)
            {
                if (selectedArray[i] != lb.Items.Count - 1)
                {
                    li = lb.Items[selectedArray[i]];
                    lb.Items.RemoveAt(selectedArray[i]);
                    lb.Items.Insert(selectedArray[i] + 1, li);
                }
            }
        }

        private void delete(ListBox lb)
        {
            int[] selectedArray = lb.GetSelectedIndices();
            for (int i = selectedArray.Length - 1; i > -1; i--)
            {
                lb.Items.RemoveAt(selectedArray[i]);
            }
        }

        private bool ColumnTypeIsINT(string columnName)
        {
            CUSTOMRP.Model.SOURCEVIEW sv = QueryReport.Code.WebHelper.bllSOURCEVIEW.GetModel(Int32.Parse(this.ddlQueryName.SelectedValue));
            string columnType = CUSTOMRP.BLL.AppHelper.getColumnType(sv.ID, sv.TBLVIEWNAME, columnName, me).ToLower();
            return (columnType == "int") || (columnType == "decimal");
        }

        private void ddlQueryNameChange()
        {
            string[] colnames = null;
            CUSTOMRP.Model.SOURCEVIEW sv = WebHelper.bllSOURCEVIEW.GetModel(Int32.Parse(this.ddlQueryName.SelectedValue));
            List<CUSTOMRP.Model.SOURCEVIEWCOLUMN> svc = WebHelper.bllSOURCEVIEWCOLUMN.GetModelsForSourceView(sv.ID).OrderBy(x => x.DisplayName).ToList();
            colnames = svc.Select(x => x.DisplayName).ToArray();

            switch (sv.SourceType)
            {
                case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.View:
                case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.Table:
                    {
                        columninfos = CUSTOMRP.BLL.AppHelper.GetColumnInfoForTblView(me.DatabaseNAME, sv.TBLVIEWNAME);
                        if (colnames == null)
                        {
                            colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForTblView(me.DatabaseNAME, sv.TBLVIEWNAME);
                        }
                    }
                    break;
                case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.StoredProc:
                    {
                        columninfos = CUSTOMRP.BLL.AppHelper.GetColumnInfoForStoredProc(me.DatabaseNAME, sv.TBLVIEWNAME);
                        if (colnames == null)
                        {
                            colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForStoredProc(me.DatabaseNAME, sv.TBLVIEWNAME);
                        }
                    }
                    break;
            }
            // Filter result to only columns that is requested
            columninfos = columninfos.Where(x => colnames.Contains(x.ColName)).ToList();

            // ad-hoc add all columns to SOURCEVIEWCOLUMN
            WebHelper.bllSOURCEVIEWCOLUMN.AddColList(sv.ID, colnames);

            this.lbAllColumns.Items.Clear();

            //v1.0.0 Fai 2015.03.19 - Order by Column Name
            if (svc.Count == 0)
            {
                // do sorting
                columninfos = columninfos.OrderBy(p => p.ColName).ToList();
            }
            else
            {
                // patch displayname and do sorting
                columninfos = (from c in columninfos
                               join s in svc.Where(x => x.ColumnType == CUSTOMRP.Model.SOURCEVIEWCOLUMN.ColumnTypes.Normal) on c.ColName equals s.COLUMNNAME
                               orderby s.DisplayName
                               select new CUSTOMRP.Model.ColumnInfo()
                               {
                                   ColName = c.ColName,
                                   DisplayName = String.IsNullOrEmpty(s.DISPLAYNAME) ? c.ColName : s.DISPLAYNAME, // no need to show actual column name here if DisplayName is supplied
                                   DataType = c.DataType,
                               }).ToList();
            }

            foreach (CUSTOMRP.Model.ColumnInfo col in columninfos)
            {
                ListItem option = new ListItem(col.DisplayName, col.ColName);
                option.Attributes.Add("data-datatype", col.DataType);
                this.lbAllColumns.Items.Add(option);
            }

            foreach (CUSTOMRP.Model.FORMULAFIELD formula in formulaFields)
            {
                ListItem option = new ListItem(formula.FIELDNAME);
                this.lbFormula.Items.Add(option);
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

        private void SetFormulaControlNameForRender()
        {
            for (int i = 0; i < this.formulaFields.Count; i++)
            {
                this.formulaFields[i].SetControlID(String.Format("F_{0}", i));
            }
            // save change to session
            Session[strSessionKeyFormulaFields] = this.formulaFields;
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

            if (idx == this.formulaFields.Count)
            {
                CUSTOMRP.Model.FORMULAFIELD newfield = new CUSTOMRP.Model.FORMULAFIELD();
                newfield.FIELDNAME = "Field" + Convert.ToString(idx + 1);
                newfield.FUNCTION = 1; // Add
                newfield.Params.Add(new CUSTOMRP.Model.FORMULAPARAM()
                {
                    PARAMTYPE = 0,
                    VALUE1 = "1"
                });
                newfield.Params.Add(new CUSTOMRP.Model.FORMULAPARAM()
                {
                    PARAMTYPE = 0,
                    VALUE1 = "1"
                });
                formulaFields.Add(newfield);
            }

            this.SetFormulaControlNameForRender();

            CUSTOMRP.Model.FORMULAFIELD f = formulaFields[idx];
            DlgParam payload = new DlgParam()
            {
                Idx = idx,
                FieldName = f.FIELDNAME,
                RenderText = f.GetHtml,
                SQLText = f.GetText,
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
                if ((i != idx) && (formulaFields[i].FIELDNAME == fieldname))
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

            CUSTOMRP.Model.FORMULAFIELD f = formulaFields[idx];
            f.FIELDNAME = fieldname;

            AjaxReloadFormulaDlg();
        }

        private void AjaxParamAdd()
        {
            string strParamId = Request.Params["ParamId"];
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

            CUSTOMRP.Model.FORMULAFIELD f = formulaFields[idx];

            if (!f.AddParamter(strParamId))
            {
                AjaxShowError("Param Id not found!");
                return;
            }
            AjaxReloadFormulaDlg();
        }

        private void AjaxParamRemove()
        {
            string strParamId = Request.Params["ParamId"];
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

            CUSTOMRP.Model.FORMULAFIELD f = formulaFields[idx];

            if (!f.RemoveParam(strParamId))
            {
                AjaxShowError("Param Id not found!");
                return;
            }
            AjaxReloadFormulaDlg();
        }

        private void AjaxParamFuncChg()
        {
            string strParamId = Request.Params["ParamId"];
            string strNewFunc = Request.Params["newFunc"];
            int idx = 0;
            CUSTOMRP.Model.FORMULAFIELD.Functions newFunc = CUSTOMRP.Model.FORMULAFIELD.Functions.NONE;

            if (!Int32.TryParse(Request.Params["id"], out idx))
            {
                AjaxShowError("Invalid ID number!");
                return;
            }

            if (!Enum.TryParse<CUSTOMRP.Model.FORMULAFIELD.Functions>(strNewFunc, out newFunc))
            {
                AjaxShowError("Invalid function call!");
                return;
            }

            if (this.formulaFields.Count < (idx - 1))
            {
                AjaxShowError("Index out of range!");
                return;
            }

            CUSTOMRP.Model.FORMULAFIELD f = formulaFields[idx];

            if (!f.ChangeFunctionType(strParamId, newFunc))
            {
                AjaxShowError("Param Id not found!");
                return;
            }
            AjaxReloadFormulaDlg();
        }

        private void AjaxParamTextChg()
        {
            string strParamId = Request.Params["ParamId"];
            string newValue = Request.Params["newValue"];
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

            CUSTOMRP.Model.ColumnInfo col = this.columninfos.Where(x => x.ColName.ToLower() == newValue.ToLower() || x.DisplayName.ToLower() == newValue.ToLower()).FirstOrDefault();

            if (col != null) { newValue = col.ColName; }

            CUSTOMRP.Model.FORMULAFIELD f = formulaFields[idx];

            if (!f.ChangeParamText(strParamId, newValue, (col != null)))
            {
                AjaxShowError("Param Id not found!");
                return;
            }
            AjaxReloadFormulaDlg();
        }

        private void AjaxParamTypeChg()
        {
            string strParamId = Request.Params["ParamId"];
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

            CUSTOMRP.Model.FORMULAFIELD f = formulaFields[idx];

            if (!f.ChangeParamTypeToFormula(strParamId))
            {
                AjaxShowError("Param Id not found!");
                return;
            }
            AjaxReloadFormulaDlg();
        }

        private void AjaxUpdatelbformula()
        {
            string[] FieldNames = this.formulaFields.Select(x => x.FIELDNAME).ToArray();

            ReloadOptions payload = new ReloadOptions()
            {
                fields = FieldNames
            };

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ReloadOptions));
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

            this.formulaFields.RemoveAt(idx);

            this.SetFormulaControlNameForRender();

            AjaxUpdatelbformula();
        }

        private void AjaxPushSelectionToSession()
        {
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

        [Serializable, DataContract]
        public class DlgParam
        {
            [DataMember]
            public int Idx { get; set; }
            [DataMember]
            public string FieldName { get; set; }
            [DataMember]
            public string RenderText { get; set; }
            [DataMember]
            public string SQLText { get; set; }
        }

        [Serializable, DataContract]
        public class ReloadOptions
        {
            [DataMember]
            public string[] fields { get; set; }
        }
        */
    }
}