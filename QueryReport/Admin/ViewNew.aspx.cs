using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Text;
using QueryReport.Code;

namespace QueryReport.Admin
{
    public partial class ViewNew : LoginUserPage
    {
        private const string strSessionViewNew_myView = "__SESSION_VIEWNEW_MYVIEW";
        private const string strSessionViewNew_myColumns = "__SESSION_VIEWNEW_MYCOLUMNS";
        private const string strSessionViewNew_dbViewList = "__SESSION_VIEWNEW_DBVIEWLIST";
        private const string strSessionViewNew_strAllColumns = "__SESSION_VIEWNEW_STRALLCOLUMNS";

        private CUSTOMRP.Model.SOURCEVIEW myView = null;
        private List<CUSTOMRP.Model.SOURCEVIEWCOLUMN> myColumns = null;
        private List<TblView> dbViewList = null;

        private bool p_fSuppressRender = false;     // Whether to render page contents

        #region Event Handlers

        protected void Page_Init(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["id"]) == false)
            {
                if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_query, "Modify", me.LoginID) == false)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "ViewList.aspx");
                    Response.End();
                }

                if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_query, "Delete", me.LoginID))
                {
                    this.btnDelete.Visible = true;
                }

                if (!IsPostBack)
                {
                    int id = 0;

                    if (Request.QueryString["id"] != null)
                    {
                        if (!Int32.TryParse(Request.QueryString["id"], out id))
                        {
                            Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.parameter_error, "ViewList.aspx");
                            Response.End();
                            return;
                        }

                        myView = WebHelper.bllSOURCEVIEW.GetModel(me.ID, id);
                        if (myView == null)
                        {
                            Session.Remove(strSessionViewNew_myView);
                            Session.Remove(strSessionViewNew_myColumns);
                            Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.parameter_error, "ViewList.aspx");
                            Response.End();
                        }
                        else
                        {
                            Session[strSessionViewNew_myView] = myView;

                            myColumns = WebHelper.bllSOURCEVIEWCOLUMN.GetModelsForSourceView(me.ID, id, true).OrderBy(x => x.COLUMNNAME).ToList();
                            Session[strSessionViewNew_myColumns] = myColumns;
                        }
                    }
                }
                else
                {
                    myView = (CUSTOMRP.Model.SOURCEVIEW)Session[strSessionViewNew_myView];
                }
            }
            else
            {
                if (!IsPostBack)
                {
                    Session.Remove(strSessionViewNew_myView);
                    Session.Remove(strSessionViewNew_myColumns);
                    Session.Remove(strSessionViewNew_strAllColumns);
                }

                if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_query, "Add", me.LoginID) == false)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "ViewList.aspx");
                    Response.End();
                }
            }

            //?????
            //v1.8.2 Ben 2018.02.22 - Add to store actual columns name from DB - Begin
            if (!IsPostBack)
            {
                Session.Remove(strSessionViewNew_strAllColumns);
            }
            //v1.8.2 Ben 2018.02.22 - Add to store actual columns name from DB - End

            myColumns = (List<CUSTOMRP.Model.SOURCEVIEWCOLUMN>)Session[strSessionViewNew_myColumns] ?? new List<CUSTOMRP.Model.SOURCEVIEWCOLUMN>();
            dbViewList = (List<TblView>)Session[strSessionViewNew_dbViewList] ?? new List<TblView>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ProcessAjaxRequestParameters()) { return; } // stop further process if it's Ajax event

            if (!IsPostBack)
            {
                this.FillddlTblViewName();
                this.FillddlLevel();
                this.cbExcel.Checked = true;

                if (myView != null)
                {
                    this.txtuid.Text = myView.SOURCEVIEWNAME;
                    this.txtp1.Text = myView.DESC;
                    this.ddlTblViewName.SelectedValue = myView.TBLVIEWNAME;
                    this.ddlLevel.SelectedValue = myView.VIEWLEVEL.ToString();

                    if (myView.FORMATTYPE != null)
                    {
                        int[] valus = Common.Utils.getArray2N(myView.FORMATTYPE.Value);

                        if ((valus.ToList().Contains(0)))
                        {
                            this.cbExcel.Checked = true;
                        }
                        if ((valus.ToList().Contains(1)))
                        {
                            this.cbWord.Checked = true;
                        }
                    }
                }
            }

            this.FillgvFields();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.p_fSuppressRender)
            {
                base.Render(writer);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_query, "Delete", me.LoginID) == false)
            {
                //Common.JScript.Alert(AppNum.accesserror);
                //Common.JScript.GoHistory(-1);
                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "ViewList.aspx");
                Response.End();
            }

            if (myView != null)
            {
                if (WebHelper.bllReport.GetRecordCount(me.ID, " [SVID]='" + myView.ID.ToString() + "'") > 0)
                {
                    //Common.JScript.Alert(AppNum.cantdeletesv);
                    //Common.JScript.GoHistory(-1);
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.cantdeletesv, "ViewList.aspx");
                    Response.End();
                }
                else
                {
                    WebHelper.bllSOURCEVIEW.Delete(me.ID, myView.ID);
                    //v1.7.0 Ben 2017.08.21 - this session will store columns before update
                    Session.Remove(strSessionViewNew_myColumns);
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "viewlist.aspx");
                    Response.End();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string name = this.txtuid.Text.Trim();
            string description = this.txtp1.Text.Trim();
            string tblviewname = this.ddlTblViewName.SelectedValue;
            int sourcetype = dbViewList.Where(x => x.TblViewName == tblviewname).First().SourceType;
            decimal level = Decimal.Parse(this.ddlLevel.SelectedValue);

            List<int> kv = new List<int>();
            if (this.cbExcel.Checked)
            {
                kv.Add(0);
            }
            if (this.cbWord.Checked)
            {
                kv.Add(1);
            }
            long kvs = Common.Utils.getSum2N(kv.ToArray());

            if (myView != null)
            {
                myView.SOURCEVIEWNAME = name;
                myView.SOURCETYPE = sourcetype;
                myView.TBLVIEWNAME = tblviewname;
                myView.VIEWLEVEL = level;
                myView.DESC = description; 
                myView.FORMATTYPE = (int)kvs;
                WebHelper.bllSOURCEVIEW.Update(me.ID, myView);

                //v1.8.2 Ben 2018.02.22 - Remove columns not in actual - Begin
                /*
                //temp not delete
                if (Session[strSessionViewNew_strAllColumns] != null)
                {
                    var strAllColumns = (string[])Session[strSessionViewNew_strAllColumns];
                    //var delColumns= myColumns.Except(myColumns.Where(x => strAllColumns.Contains(x.COLUMNNAME))).ToArray();
                    //WebHelper.bllSOURCEVIEWCOLUMN.DeleteModelList(me.ID, delColumns);
                    //myColumns = myColumns.Where(x => strAllColumns.Contains(x.COLUMNNAME)).ToList();
                    / *
                    for (int i = 0; i < strAllColumns.Length; i++)
                    {
                        //Handle different same content but different case
                        if (myColumns.Any(x => x.COLUMNNAME.ToUpper() == strAllColumns[i].ToUpper() && x.COLUMNNAME != strAllColumns[i]))
                        {
                            myColumns.Where(x => x.COLUMNNAME.ToUpper() == strAllColumns[i].ToUpper()).ToList().ForEach(x => x.COLUMNNAME = strAllColumns[i]);
                        }
                    }
                    * /
                }
                */
                //v1.8.2 Ben 2018.02.22 - Remove columns not in actual - End

                //v1.8.2 Ben 2018.02.26 - Handle same name but different case - Begin
                //Same name but different case is only because once saved but modified column name later
                //and hence 2 records: 1. column in DB having ID. 2. new row not having ID and your modified in UI is in here
                //so update the latest 1. by 2. and remove 2. here
                var PossibleDuplicatedColumns = myColumns.Where(x => x.ID == 0).ToList();
                myColumns = myColumns.Except(PossibleDuplicatedColumns).ToList();
                for (var i = 0; i < PossibleDuplicatedColumns.Count(); i++)
                {
                    var modColumns = myColumns.Where(x => x.COLUMNNAME.ToUpper() == PossibleDuplicatedColumns[i].COLUMNNAME.ToUpper());
                    if (modColumns.Any())
                    {
                        var modColumn = myColumns.Where(x => x.COLUMNNAME.ToUpper() == PossibleDuplicatedColumns[i].COLUMNNAME.ToUpper()).OrderByDescending(x => x.ID).FirstOrDefault();
                        modColumn.HIDDEN = PossibleDuplicatedColumns[i].HIDDEN;
                        modColumn.COLUMNNAME = PossibleDuplicatedColumns[i].COLUMNNAME;
                        modColumn.DISPLAYNAME = PossibleDuplicatedColumns[i].DISPLAYNAME;
                    }
                    else
                        myColumns.Add(PossibleDuplicatedColumns[i]);
                }

                //v1.8.2 Ben 2018.02.26 - Handle same name but different case - End

                foreach (CUSTOMRP.Model.SOURCEVIEWCOLUMN item in myColumns)
                {
                    item.SVID = myView.ID;
                }
                //WebHelper.bllSOURCEVIEWCOLUMN.DeleteForSourceView(myView.ID);
                //v1.7.0 Ben 2017.08.21 - Below will not update Hidden columns so new function to update
                //WebHelper.bllSOURCEVIEWCOLUMN.UpdateModelList(me.ID, myColumns.ToArray());
                WebHelper.bllSOURCEVIEWCOLUMN.UpdateModelList(me.ID, myColumns.ToArray(),true);
            }
            else
            {
                myView = new CUSTOMRP.Model.SOURCEVIEW();
                myView.SOURCEVIEWNAME = name;
                myView.DATABASEID = Convert.ToInt32(me.DatabaseID);
                myView.SOURCETYPE = sourcetype;
                myView.TBLVIEWNAME = tblviewname;
                myView.AUDODATE = DateTime.Now;
                myView.VIEWLEVEL = level;
                myView.DESC = description;
                myView.FORMATTYPE = (int)kvs;
                myView.ID = WebHelper.bllSOURCEVIEW.Add(me.ID, myView);
                foreach (CUSTOMRP.Model.SOURCEVIEWCOLUMN item in myColumns)
                {
                    item.SVID = myView.ID;
                }
                //WebHelper.bllSOURCEVIEWCOLUMN.DeleteForSourceView(myView.ID);
                //WebHelper.bllSOURCEVIEWCOLUMN.AddColList(myView.ID, selected_arr);
                //v1.7.0 Ben 2017.08.21 - Below will not update Hidden columns so new function to update
                //WebHelper.bllSOURCEVIEWCOLUMN.UpdateModelList(me.ID, myColumns.ToArray());
                WebHelper.bllSOURCEVIEWCOLUMN.UpdateModelList(me.ID, myColumns.ToArray(), true);
            }

            Session.Remove(strSessionViewNew_myView); //work done
            //v1.7.0 Ben 2017.08.21 - this session will store columns before update
            //temp ignore it
            //Session.Remove(strSessionViewNew_myColumns);
            Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "viewlist.aspx");
            Response.End();
        }

        protected void excelCheckChanged(object sender, EventArgs e)
        {
            if (cbExcel.Checked == false && Request.QueryString.AllKeys.Contains("ID")) {
                string reportsInUse = CUSTOMRP.BLL.AppHelper.CheckSVInUse(me.ID, int.Parse(Request.QueryString["ID"]), 1);
                if (!string.IsNullOrEmpty(reportsInUse))
                {
                    this.lblJavascript.Text = string.Format("<script type=\"text/javascript\"> alert('{0} is in use by:\\n {1}Please delete these reports before deactivating Excel.') </script>", txtuid.Text, reportsInUse);
                    cbExcel.Checked = true;
                }
            }
        }

        protected void wordCheckChanged(object sender, EventArgs e)
        {
            if (cbExcel.Checked == false  && Request.QueryString.AllKeys.Contains("ID")) {
                string reportsInUse = CUSTOMRP.BLL.AppHelper.CheckSVInUse(me.ID, int.Parse(Request.QueryString["ID"]), 2);
                if (!string.IsNullOrEmpty(reportsInUse)) {
                    this.lblJavascript.Text = string.Format("<script type=\"text/javascript\"> alert('{0} is in use by:\\n {1}Please delete these reports before deactivating Word.') </script>", txtuid.Text, reportsInUse);
                    cbWord.Checked = true;
                }
               
            }
        }

        protected void ddlTblViewName_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.FillgvFields();
        }

        #endregion Event Handlers

        #region Private Methods

        private void FillddlTblViewName()
        {
            string strTblViewName = this.IsPostBack ? this.ddlTblViewName.SelectedValue :
                myView != null ? myView.TBLVIEWNAME :
                null;

            dbViewList.Clear();

            dbViewList.AddRange(CUSTOMRP.BLL.AppHelper.GetViewFromDB(me.ID, me.DatabaseNAME).Where(x => !x.Equals("v_Security")).Select(x => new TblView() { TblViewName = x, SourceType = 0 }).ToArray());
            dbViewList.AddRange(CUSTOMRP.BLL.AppHelper.GetStoredProcFromDB(me.ID, me.DatabaseNAME).Where(x => !x.ToLower().EndsWith("_columns")).Select(x => new TblView() { TblViewName = x, SourceType = 2 }).ToArray());

            Session[strSessionViewNew_dbViewList] = dbViewList;
            this.ddlTblViewName.DataSource = dbViewList.OrderBy(x => x.TblViewName).Select(x => new ListItem(x.TblViewName)).ToArray();
            this.ddlTblViewName.DataBind();

            if (strTblViewName != null)
            {
                this.ddlTblViewName.SelectedValue = strTblViewName;
            }

            if (myView != null)
            {
                myView.TBLVIEWNAME = this.ddlTblViewName.SelectedValue;
                myView.SOURCETYPE = dbViewList.Where(x => x.TblViewName == myView.TBLVIEWNAME).First().SourceType;
            }
        }

        private void FillddlLevel()
        {
            DataTable ViewLevel = WebHelper.bllviewLevel.GetList(me.ID, 1000000, "DATABASEID='" + me.DatabaseID + "'", "SLEVEL").Tables[0];

            this.ddlLevel.DataSource = ViewLevel;
            this.ddlLevel.DataTextField = "NAME";
            this.ddlLevel.DataValueField = "SLEVEL";
            this.ddlLevel.DataBind();
        }

        private void FillgvFields()
        {
            try
            {
                CUSTOMRP.Model.SOURCEVIEWCOLUMN svc = null;

                string tblviewname = this.ddlTblViewName.SelectedValue;
                int sourcetype = dbViewList.Where(x => x.TblViewName == tblviewname).First().SourceType;

                string[] strAllColumns = sourcetype == 2 ?
                    CUSTOMRP.BLL.AppHelper.GetColumnNamesForStoredProc(me.ID, me.DatabaseNAME, tblviewname).OrderBy(x => x).ToArray() :
                    CUSTOMRP.BLL.AppHelper.GetColumnNamesForTblView(me.ID, me.DatabaseNAME, tblviewname).OrderBy(x => x).ToArray();
                
                //v1.8.2 Ben 2018.02.22 - Add to store actual columns name from DB
                Session[strSessionViewNew_strAllColumns] = strAllColumns;

                TableRow tr = null;
                TableHeaderRow thead = null;
                TableFooterRow tfoot = null;
                TableHeaderCell th = null;
                TableCell td = null;
                HtmlGenericControl span = null;
                HtmlInputText txt = null;
                HtmlInputCheckBox cb = null;
                string safeColName = null;

                // refresh column data
                if ((myView != null) && (myView.TBLVIEWNAME == tblviewname))
                {
                    if (!IsPostBack)
                    {
                        //v1.7.0 Ben 2017.08.21 - Does not show hidden column in default and hence checkbox must be checked 
                        //myColumns = WebHelper.bllSOURCEVIEWCOLUMN.GetModelsForSourceView(me.ID, myView.ID).OrderBy(x => x.COLUMNNAME).ToList();
                        myColumns = WebHelper.bllSOURCEVIEWCOLUMN.GetModelsForSourceView(me.ID, myView.ID, true).OrderBy(x => x.COLUMNNAME).ToList();
                        Session[strSessionViewNew_myColumns] = myColumns;
                    }
                }

                gvFields.Rows.Clear();

                #region Header

                thead = new TableHeaderRow();
                thead.TableSection = TableRowSection.TableHeader;

                th = new TableHeaderCell();
                th.Text = "Field Name";
                thead.Cells.Add(th);

                th = new TableHeaderCell();
                th.Text = "Display Name";
                thead.Cells.Add(th);

                th = new TableHeaderCell();
                th.Text = "Visible";
                thead.Cells.Add(th);

                this.gvFields.Rows.Add(thead);

                #endregion

                #region Body

                foreach (string colname in strAllColumns)
                {
                    if (myColumns != null)
                    {
                        svc = myColumns.Where(x => x.COLUMNNAME == colname).OrderByDescending(x => x.ID).FirstOrDefault();
                    }

                    safeColName = HttpUtility.HtmlEncode(colname.Replace(' ', '_'));

                    tr = new TableRow();
                    tr.TableSection = TableRowSection.TableBody;
                    tr.Attributes.Add("data-colname", colname);

                    td = new TableCell();
                    span = new HtmlGenericControl("SPAN");
                    span.InnerHtml = colname;
                    td.Controls.Add(span);
                    tr.Cells.Add(td);

                    td = new TableCell();
                    td.Attributes.Add("data-fieldname", "DISPLAYNAME");
                    td.Attributes.Add("data-fieldvalue", (svc != null) ? svc.DISPLAYNAME : String.Empty);
                    txt = new HtmlInputText();
                    txt.ID = "txtDisplayName_" + safeColName;
                    txt.Attributes.Add("class", "form-inline input-sm form-control");
                    txt.Attributes.Add("placeholder", "Enter Display Name here...");
                    txt.Value = (svc != null) ? svc.DISPLAYNAME : String.Empty;
                    td.Controls.Add(txt);
                    //span = new HtmlGenericControl("SPAN");
                    //span.InnerHtml = (svc != null) ? svc.DISPLAYNAME : String.Empty;
                    //td.Controls.Add(span);
                    
                    tr.Cells.Add(td);

                    td = new TableCell();
                    td.Style.Add("text-align", "center");
                    td.Attributes.Add("data-fieldname", "HIDDEN");
                    
                    td.Attributes.Add("data-fieldvalue", ((svc == null) || (!svc.HIDDEN)) ? "true" : "false");
                    cb = new HtmlInputCheckBox();
                    cb.ID = "cbHidden_" + safeColName;
                    cb.Attributes.Add("class", "form-inline input-sm");
                    cb.Checked = ((svc == null) || (!svc.HIDDEN));
                    td.Controls.Add(cb);
                    //span = new HtmlGenericControl("SPAN");
                    //span.InnerHtml = ((svc == null) || (!svc.HIDDEN)) ? "True" : "False";
                    //td.Controls.Add(span);
                    tr.Cells.Add(td);

                    this.gvFields.Rows.Add(tr);
                }

                #endregion

                #region Footer

                tfoot = new TableFooterRow();
                tfoot.TableSection = TableRowSection.TableFooter;

                th = new TableHeaderCell();
                th.Text = "Field Name";
                tfoot.Cells.Add(th);

                th = new TableHeaderCell();
                th.Text = "Display Name";
                tfoot.Cells.Add(th);

                td = new TableCell();
                tfoot.Cells.Add(td);

                this.gvFields.Rows.Add(tfoot);

                #endregion
            }
            catch
            {
                // don't throw error here
            }
        }

        #endregion

        #region Ajax handling Methods

        private class SVCData
        {
            public string COLUMNNAME { get; set; }
            public string DISPLAYNAME { get; set; }
            public string HIDDEN { get; set; }
        }

        private class SVData
        {
            public SVCData[] items { get; set; }
        }

        private SVData GetSVData(string jsonpayload)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<SVData>(jsonpayload);
        }

        private void AjaxSaveData(SVData data)
        {
            CUSTOMRP.Model.SOURCEVIEWCOLUMN svc = null;
            foreach (SVCData r in data.items)
            {
                //v1.7.0 Ben 2017.08.21 - set the latest
                //svc = myColumns.Where(x => x.COLUMNNAME == r.COLUMNNAME).FirstOrDefault();
                svc = myColumns.Where(x => x.COLUMNNAME == r.COLUMNNAME).OrderByDescending(x => x.ID).FirstOrDefault();
                if (svc != null)
                {
                    svc.DISPLAYNAME = r.DISPLAYNAME;
                    svc.HIDDEN = Convert.ToBoolean(r.HIDDEN);
                }
                else
                {
                    svc = new CUSTOMRP.Model.SOURCEVIEWCOLUMN();
                    svc.COLUMNNAME = r.COLUMNNAME;
                    svc.DISPLAYNAME = r.DISPLAYNAME;
                    svc.COLUMNTYPE = 1;
                    svc.COLUMNCOMMENT = String.Empty;
                    svc.HIDDEN = Convert.ToBoolean(r.HIDDEN);
                    svc.DEFAULTDISPLAYNAME = String.Empty;

                    myColumns.Add(svc);
                }
            }

            Session[strSessionViewNew_myColumns] = myColumns;
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

        private bool ProcessAjaxRequestParameters()
        {
            bool result = false;

            string p_strAction = (Request.Params["action"] ?? String.Empty).ToLower();

            switch (p_strAction)
            {
                case "savedata":
                    {
                        string payload = Request.Params["payload"];
                        SVData data = null;
                        try
                        {
                            data = this.GetSVData(payload);
                        }
                        catch
                        {
                            AjaxShowError(AppNum.ErrorMsg.InvalidData);
                            return false;
                        }

                        //v1.2.0 - Cheong - 2016/12/29 - Add validation against duplicate display name or colliding column name
                        #region Display name validation
                        foreach (SVCData r in data.items)
                        {
                            if (!String.IsNullOrEmpty(r.DISPLAYNAME))
                            {
                                //v1.8.4 Ben 2018.05.23 - if duplicated one is hidden, allow save
                                //if (data.items.Where(x => x.COLUMNNAME == r.DISPLAYNAME).Count() > 0)
                                if (!Convert.ToBoolean(r.HIDDEN) && data.items.Where(x => x.COLUMNNAME == r.DISPLAYNAME && !Convert.ToBoolean(x.HIDDEN)).Count() > 0)
                                {
                                    AjaxShowError(String.Format(AppNum.ErrorMsg.DisplayNameSameAsColumnName, r.DISPLAYNAME));
                                    return true;
                                }
                                //v1.8.4 Ben 2018.05.23 - if duplicated one is hidden, allow save
                                //if (data.items.Where(x => x.DISPLAYNAME == r.DISPLAYNAME).Count() > 1)
                                if (!Convert.ToBoolean(r.HIDDEN) && data.items.Where(x => x.DISPLAYNAME == r.DISPLAYNAME && !Convert.ToBoolean(x.HIDDEN)).Count() > 1)
                                {
                                    AjaxShowError(String.Format(AppNum.ErrorMsg.DuplicateDisplayName, r.DISPLAYNAME));
                                    return true;
                                }
                            }
                        }
                        #endregion Display name validation

                        try
                        {
                            this.AjaxSaveData(data);
                            result = true;
                        }
#if DEBUG
                        catch (Exception ex)
                        {
                            AjaxShowError(ex.ToString());
                            return false;
                        }
#else
                        catch
                        {
                            AjaxShowError(AppNum.ErrorMsg.GeneralError);
                            return false;
                        }
#endif
                        AjaxShowError(String.Empty);    // OK
                    }
                    break;
            }
            return result;
        }

        #endregion
    }

    public class TblView
    {
        public string TblViewName { get; set; }
        public int SourceType { get; set; }
    }
}