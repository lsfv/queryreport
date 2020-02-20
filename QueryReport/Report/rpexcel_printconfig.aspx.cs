using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using QueryReport.Code;

namespace QueryReport.Report
{
    public partial class rpexcel_printconfig : LoginUserPage
    {
        private ReportParameterContainer container = null;
        private CUSTOMRP.Model.REPORT myReport = null;
        private List<CUSTOMRP.Model.ColumnInfo> columninfos = null;

        #region Event Handlers

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2241:Provide correct arguments to formatting methods")]
        protected void Page_Init(object sender, EventArgs e)
        {
            this.lblJavascript.Text = String.Empty;
            if (Session[rpexcel.strSessionKeyMyReport] != null)
            {
                myReport = (CUSTOMRP.Model.REPORT)Session[rpexcel.strSessionKeyMyReport];
            }

            if (myReport == null)
            {
                string querystring = Request.UrlReferrer.Query;
                if (!String.IsNullOrEmpty(querystring))
                {
                    string[] queries = querystring.Substring(1).Split('&');
                    foreach (string query in queries)
                    {
                        if (query.StartsWith("id"))
                        {
                            int id = Int32.Parse(query.Split('=')[1]);
                            myReport = WebHelper.bllReport.GetModel(me.ID, id);

                            if (myReport == null)
                            {
                                this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert{\"{0}\"};window.close();</script>", AppNum.ErrorMsg.parameter_error);
                                HttpContext.Current.ApplicationInstance.CompleteRequest();
                                return;
                            }

                            Session[rpexcel.strSessionKeyMyReport] = myReport;
                        }
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
                this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert{\"{0}\"};window.close();</script>", AppNum.ErrorMsg.parameter_error);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.myReport != null)
            {
                this.ddl_print_orientation.Value = Convert.ToString(myReport.PRINT_ORIENTATION);
                this.txt_print_fittopage.Value = Convert.ToString(myReport.PRINT_FITTOPAGE);
                this.txtReportHeader.Text = myReport.REPORT_HEADER;
                this.txtReportFooter.Text = myReport.REPORT_FOOTER;
                this.txtSubCntLbl.Value = myReport.SUBCOUNT_LABEL;
                this.txtFontFamily.Value = myReport.FONT_FAMILY;
                this.chkPdfGridLines.Checked = myReport.PDF_GRID_LINES;
            }
            this.FillgvReportColumns();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string containername = this.btnSave.UniqueID.Replace(this.btnSave.ID, "");

            #region update setting to myReport

            if (!String.IsNullOrWhiteSpace(Request.Params[this.ddl_print_orientation.UniqueID]))
            {
                myReport.PRINT_ORIENTATION = Convert.ToInt32(Request.Params[this.ddl_print_orientation.UniqueID]);
            }
            if (!String.IsNullOrWhiteSpace(Request.Params[this.txt_print_fittopage.UniqueID]))
            {
                myReport.PRINT_FITTOPAGE = Convert.ToInt16(Request.Params[this.txt_print_fittopage.UniqueID]);
            }
            myReport.REPORT_HEADER = ProcessMultilineText(Request.Params[this.txtReportHeader.UniqueID]);
            myReport.REPORT_FOOTER = ProcessMultilineText(Request.Params[this.txtReportFooter.UniqueID]);
            myReport.SUBCOUNT_LABEL = Request.Params[this.txtSubCntLbl.UniqueID];
            myReport.FONT_FAMILY = Request.Params[this.txtFontFamily.UniqueID];
            myReport.PDF_GRID_LINES = Request.Params.AllKeys.Contains(this.chkPdfGridLines.UniqueID) ? (Request.Params[this.chkPdfGridLines.UniqueID].Equals("on") ? true : false) : false;

            CUSTOMRP.Model.REPORTCOLUMN[] contents = myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content).ToArray();
            string target;
            var regexColorCode = new System.Text.RegularExpressions.Regex("^#[a-fA-F0-9]{6}$");
            for (int i = 0; i < contents.Length; i++)
            {
                if (!String.IsNullOrWhiteSpace(Request.Params[String.Format("{0}hid_colname_{1}", containername, i)]))
                {
                    target = Request.Params[String.Format("{0}hid_colname_{1}", containername, i)].Trim();
                    int target_coltype;
                    if (!Int32.TryParse(Request.Params[String.Format("{0}hid_coltype_{1}", containername, i)].Trim(), out target_coltype)) { target_coltype = 1; }
                    CUSTOMRP.Model.REPORTCOLUMN content = myReport.ReportColumns.Where(x => (x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content)
                        && ((target_coltype == 1 ? x.COLUMNNAME : x.DISPLAYNAME) == target)).FirstOrDefault();
                    if (content != null)
                    {
                        if (!String.IsNullOrWhiteSpace(Request.Params[String.Format("{0}txt_displayname_{1}", containername, i)]))
                        {
                            content.DISPLAYNAME = Request.Params[String.Format("{0}txt_displayname_{1}", containername, i)].Trim();
                        }

                        //// v1.8.8 Alex 2018.10.11 - Add Sort Order - Begin
                        //string isAscendingStr = Request.Params[String.Format("{0}select_sortorder_{1}", containername, i)];
                        //content.IS_ASCENDING = Convert.ToBoolean(int.Parse(isAscendingStr));
                        //// v1.8.8 Alex 2018.10.11 - Add Sort Order - End

                        //// v1.8.8 Alex 2018.10.12 - Add Sort Sequence - Begin
                        //if (!String.IsNullOrWhiteSpace(Request.Params[String.Format("{0}txt_sortsequence_{1}", containername, i)]))
                        //{
                        //    try
                        //    {
                        //        content.SORT_SEQUENCE = int.Parse(Request.Params[String.Format("{0}txt_sortsequence_{1}", containername, i)]);
                        //    }
                        //    catch
                        //    {
                        //        this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert('{0}');</script>", String.Format("Excel sort sequence must be an integer.", 255 - NPOIHelper.TrailingBlank));
                        //        return;
                        //    }
                        //}
                        //else
                        //{
                        //    this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert('{0}');</script>", String.Format("Excel sort sequence cannot be empty.", 255 - NPOIHelper.TrailingBlank));
                        //    return;
                        //}
                        //// v1.8.8 Alex 2018.10.12 - Add Sort Sequence - End


                        if (!String.IsNullOrWhiteSpace(Request.Params[String.Format("{0}txt_width_{1}", containername, i)]))
                        {
                            if ("auto".Equals((Request.Params[String.Format("{0}txt_width_{1}", containername, i)] ?? String.Empty).ToLower()))
                            {
                                content.EXCEL_COLWIDTH = -1;
                            }
                            else
                            {
                                try
                                {
                                    content.EXCEL_COLWIDTH = Decimal.Parse(Request.Params[String.Format("{0}txt_width_{1}", containername, i)]);
                                }
                                catch
                                {
                                    this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert('{0}');</script>", String.Format("Excel column width must be less than or equal {0} characters.", 255 - NPOIHelper.TrailingBlank));
                                    return;
                                }
                            }
                        }
                        string backgroundColorStr = Request.Params[String.Format("{0}txt_backgroundColor_{1}", containername, i)];
                        if (!String.IsNullOrEmpty(backgroundColorStr))
                        {
                            if (regexColorCode.IsMatch(backgroundColorStr.Trim()))
                            {
                                content.BACKGROUND_COLOR = backgroundColorStr;
                            }
                            else
                            {
                                this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert('{0}');</script>", "Invalid color code '" + backgroundColorStr + "'.");
                                return;
                            }
                        }
                        else
                        {
                            content.BACKGROUND_COLOR = "";
                        };
                        string fontColorStr = Request.Params[String.Format("{0}txt_fontColor_{1}", containername, i)];
                        if (!String.IsNullOrEmpty(fontColorStr))
                        {
                            if (regexColorCode.IsMatch(fontColorStr.Trim()))
                            {
                                content.FONT_COLOR = fontColorStr;
                            }
                            else
                            {
                                this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert('{0}');</script>", "Invalid color code '" + fontColorStr + "'.");
                                return;
                            }
                        }
                        else
                        {
                            content.FONT_COLOR = "";
                        };
                        string fontSizeStr = Request.Params[String.Format("{0}txt_fontSize_{1}", containername, i)];
                        if (!String.IsNullOrEmpty(fontSizeStr))
                        {
                            try
                            {
                                content.FONT_SIZE = int.Parse(fontSizeStr);
                            }
                            catch
                            {
                                this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert('{0}');</script>", "Invalid font size '" + fontSizeStr + "'.");
                                return;
                            }
                        }
                        else
                        {
                            content.FONT_SIZE = null;
                        };

                        string fontBoldStr = Request.Params[String.Format("{0}chk_fontBold_{1}", containername, i)];
                        if (!String.IsNullOrEmpty(fontBoldStr))
                        {
                            content.FONT_BOLD = true;
                        }
                        else
                        {
                            content.FONT_BOLD = false;
                        };

                        string fontItalicStr = Request.Params[String.Format("{0}chk_fontItalic_{1}", containername, i)];
                        if (!String.IsNullOrEmpty(fontItalicStr))
                        {
                            content.FONT_ITALIC = true;
                        }
                        else
                        {
                            content.FONT_ITALIC = false;
                        };

                        string horizontalTextAlignStr = Request.Params[String.Format("{0}select_horizontalTextAlign_{1}", containername, i)];
                        content.HORIZONTAL_TEXT_ALIGN = int.Parse(horizontalTextAlignStr);

                        string cellFormatStr = Request.Params[String.Format("{0}txt_cellFormat_{1}", containername, i)];
                        content.CELL_FORMAT = cellFormatStr;
                    }
                }
            }

            #endregion update setting to myReport

            // v1.8.8 Alex 2018.10.22 - Group Indent Column Width - Begin
            for (int i = 0; i < myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Group).Count(); i++)
            {
                if (!String.IsNullOrWhiteSpace(Request.Params[String.Format("{0}hid_colname_grp_{1}", containername, i)]))
                {
                    target = Request.Params[String.Format("{0}hid_colname_grp_{1}", containername, i)].Substring(8);
                    int target_coltype;
                    if (!Int32.TryParse(Request.Params[String.Format("{0}hid_coltype_grp_{1}", containername, i)].Trim(), out target_coltype)) { target_coltype = 1; }
                    CUSTOMRP.Model.REPORTCOLUMN content_group = myReport.ReportColumns.Where(x => (x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Group)
                        && ((target_coltype == 1 ? x.COLUMNNAME : x.DISPLAYNAME) == target)).FirstOrDefault();



                    if (!String.IsNullOrWhiteSpace(Request.Params[String.Format("{0}txt_width_grp_{1}", containername, i)]))
                    {
                        if ("auto".Equals((Request.Params[String.Format("{0}txt_width_grp_{1}", containername, i)] ?? String.Empty).ToLower()))
                        {
                            content_group.EXCEL_COLWIDTH = -1;
                        }
                        else
                        {
                            try
                            {
                                content_group.EXCEL_COLWIDTH = Decimal.Parse(Request.Params[String.Format("{0}txt_width_grp_{1}", containername, i)]);
                            }
                            catch
                            {
                                this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert('{0}');</script>", String.Format("Excel column width must be less than or equal {0} characters.", 255 - NPOIHelper.TrailingBlank));
                                return;
                            }
                        }
                    }
                }
            }
            // v1.8.8 Alex 2018.10.22 - Group Indent Column Width - End

            #region Update container content for displayname

            for (int i = 0; i < container.contentColumn.Length; i++)
            {
                if (!String.IsNullOrWhiteSpace(Request.Params[String.Format("{0}hid_colname_{1}", containername, i)]))
                {
                    target = Request.Params[String.Format("{0}hid_colname_{1}", containername, i)].Trim();
                    int target_coltype;
                    if (!Int32.TryParse(Request.Params[String.Format("{0}hid_coltype_{1}", containername, i)].Trim(), out target_coltype)) { target_coltype = 1; }
                    Fields content = container.contentColumn.Where(x => ((target_coltype == 1 ? x.ColumnName : x.DisplayName) == target)).FirstOrDefault();
                    if (content != null)
                    {
                        if (!String.IsNullOrWhiteSpace(Request.Params[String.Format("{0}txt_displayname_{1}", containername, i)]))
                        {
                            content.DisplayName = Request.Params[String.Format("{0}txt_displayname_{1}", containername, i)].Trim();
                        }
                    }
                }
            }

            #endregion

            Session[rpexcel.strSessionKeyReportParameterContainer] = container;
            Session[rpexcel.strSessionKeyMyReport] = myReport;
            this.lblJavascript.Text = "<script type=\"text/javascript\">closeMe();</script>";
        }

        private string ProcessMultilineText(string input)
        {
            return input == null ? null : input.Replace("\r\n", "\n").TrimEnd();
        }
        #endregion

        private void FillgvReportColumns()
        {
            TableRow tr = null;
            TableHeaderCell th = null;
            TableCell td = null;
            HtmlInputText txt = null;
            HtmlGenericControl span = null;
            HtmlInputHidden hid = null;
            HtmlSelect select = null;
            string temp = "";
            Dictionary<string, string> dictHorAligndict = NPOIHelper.GetHorizontalTextAligns();

            gvReportColumns.Rows.Clear();
            #region header
            tr = new TableRow();
            tr.TableSection = TableRowSection.TableHeader;
            th = new TableHeaderCell();
            th.Text = "Column Name";
            tr.Cells.Add(th);

            th = new TableHeaderCell();
            th.Text = "Display Name";
            tr.Cells.Add(th);

            ////v1.8.8 Alex 2018.10.10 - Add Sort Order - Begin
            //th = new TableHeaderCell();
            //th.Text = "Sort Order";
            //tr.Cells.Add(th);
            ////v1.8.8 Alex 2018.10.10 - Add Sort Order - End

            ////v1.8.8 Alex 2018.10.12 - Add Sort Sequence - Begin
            //th = new TableHeaderCell();
            //th.Text = "Sort Sequence";
            //tr.Cells.Add(th);
            ////v1.8.8 Alex 2018.10.12 - Add Sort Sequence - End

            th = new TableHeaderCell();
            th.Text = "Column Width (in characters)";
            tr.Cells.Add(th);

            th = new TableHeaderCell();
            th.Text = "Background Color";
            tr.Cells.Add(th);

            th = new TableHeaderCell();
            th.Text = "Font Color";
            tr.Cells.Add(th);

            th = new TableHeaderCell();
            th.Text = "Font Size";
            tr.Cells.Add(th);

            th = new TableHeaderCell();
            th.Text = "Bold";
            tr.Cells.Add(th);

            th = new TableHeaderCell();
            th.Text = "Italic";
            tr.Cells.Add(th);

            th = new TableHeaderCell();
            th.Text = "Horizontal Text Align";
            tr.Cells.Add(th);

            th = new TableHeaderCell();
            th.Text = "Formatting";
            tr.Cells.Add(th);

            gvReportColumns.Rows.Add(tr);
            #endregion header

            //myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Group).ToDictionary(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DisplayName, y => y.DisplayName);
            var groupNames = container.groupColumn.Select(x => x.ColumnName).ToList();
            if ((myReport != null) && (myReport.ReportColumns != null))
            {
                CUSTOMRP.Model.REPORTCOLUMN[] contents = myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content).ToArray();
                CUSTOMRP.Model.REPORTCOLUMN[] contents_grps = myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Group).ToArray();

                for (int i = 0; i < contents.Length; i++)
                {
                    tr = new TableRow();

                    td = new TableCell();
                    span = new HtmlGenericControl();
                    span.InnerHtml = contents[i].ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? contents[i].COLUMNNAME : contents[i].DISPLAYNAME;
                    td.Controls.Add(span);
                    hid = new HtmlInputHidden();
                    hid.ID = String.Format("hid_colname_{0}", i);
                    hid.Value = contents[i].ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? contents[i].COLUMNNAME : contents[i].DISPLAYNAME;
                    td.Controls.Add(hid);
                    hid = new HtmlInputHidden();
                    hid.ID = String.Format("hid_coltype_{0}", i);
                    hid.Value = Convert.ToString(contents[i].COLUMNTYPE);
                    td.Controls.Add(hid);
                    tr.Cells.Add(td);

                    td = new TableCell();
                    txt = new HtmlInputText();
                    txt.ID = String.Format("txt_displayname_{0}", i);
                    txt.Value = Convert.ToString(contents[i].DISPLAYNAME);
                    txt.Attributes.Add("class", "form-control-smallinline");
                    td.Controls.Add(txt);
                    tr.Cells.Add(td);

                    ////v1.8.8 Alex 2018.10.10 - Add Sort Order - Begin
                    //td = new TableCell();
                    //select = new HtmlSelect();
                    //select.Items.Add(new ListItem("Ascending", "1"));
                    //select.Items.Add(new ListItem("Descending", "0"));
                    //select.ID = String.Format("select_sortorder_{0}", i);
                    //select.Attributes.Add("class", "form-control-smallinline");
                    //select.Value = contents[i].IS_ASCENDING ? "1" : "0";
                    //td.Controls.Add(select);
                    //tr.Cells.Add(td);
                    ////v1.8.8 Alex 2018.10.10 - Add Sort Order - End

                    ////v1.8.8 Alex 2018.10.12 - Add Sort Sequence - Begin
                    //td = new TableCell();
                    //txt = new HtmlInputText();
                    //txt.ID = String.Format("txt_sortsequence_{0}", i);
                    //txt.Value = Convert.ToString(contents[i].SORT_SEQUENCE);
                    //txt.Attributes.Add("class", "form-control-smallinline");
                    //txt.Style.Add("width", "50px");
                    //td.Controls.Add(txt);
                    //tr.Cells.Add(td);
                    ////v1.8.8 Alex 2018.10.12 - Add Sort Sequence - End

                    td = new TableCell();
                    txt = new HtmlInputText();
                    txt.ID = String.Format("txt_width_{0}", i);
                    temp = Convert.ToString(contents[i].EXCEL_COLWIDTH);

                    if (temp.Contains("."))
                    {
                        txt.Value = temp.TrimEnd('0').TrimEnd('.');
                    }
                    else
                    {
                        txt.Value = temp;
                    }
                    //v1.0.0 - Cheong - 2016/02/25 - Show "Auto" for "-1" in col width
                    if ("-1" == txt.Value)
                    {
                        txt.Value = "Auto";
                    }
                    txt.Attributes.Add("class", "form-control-smallinline");
                    td.Controls.Add(txt);
                    tr.Cells.Add(td);

                    td = new TableCell();
                    txt = new HtmlInputText();
                    txt.ID = String.Format("txt_backgroundColor_{0}", i);
                    txt.Attributes.Add("class", "form-control-smallinline cp");
                    txt.Attributes.Add("data-isColorPicker", "true");
                    txt.Style.Add("width", "80px");
                    txt.Value = contents[i].BACKGROUND_COLOR;
                    td.Controls.Add(txt);
                    tr.Cells.Add(td);

                    td = new TableCell();
                    txt = new HtmlInputText();
                    txt.ID = String.Format("txt_fontColor_{0}", i);
                    txt.Attributes.Add("class", "form-control-smallinline cp");
                    txt.Attributes.Add("data-isColorPicker", "true");
                    txt.Style.Add("width", "80px");
                    txt.Value = contents[i].FONT_COLOR;
                    td.Controls.Add(txt);
                    tr.Cells.Add(td);

                    td = new TableCell();
                    txt = new HtmlInputText();
                    txt.ID = String.Format("txt_fontSize_{0}", i);
                    txt.Attributes.Add("class", "form-control-smallinline");
                    txt.Attributes.Add("type", "number");
                    txt.Attributes.Add("min", "1");
                    txt.Attributes.Add("max", "409");
                    txt.Style.Add("width", "60px");
                    if (contents[i].FONT_SIZE.HasValue) txt.Value = contents[i].FONT_SIZE.Value.ToString();
                    td.Controls.Add(txt);
                    tr.Cells.Add(td);

                    td = new TableCell();
                    txt = new HtmlInputText();
                    txt.ID = String.Format("chk_fontBold_{0}", i);
                    txt.Attributes.Add("class", "form-control-smallinline");
                    txt.Attributes.Add("type", "checkbox");
                    txt.Attributes.Add("value", "1");
                    if (contents[i].FONT_BOLD) txt.Attributes.Add("checked", "checked");
                    td.Controls.Add(txt);
                    tr.Cells.Add(td);

                    td = new TableCell();
                    txt = new HtmlInputText();
                    txt.ID = String.Format("chk_fontItalic_{0}", i);
                    txt.Attributes.Add("class", "form-control-smallinline");
                    txt.Attributes.Add("type", "checkbox");
                    txt.Attributes.Add("value", "1");
                    if (contents[i].FONT_ITALIC) txt.Attributes.Add("checked", "checked");
                    td.Controls.Add(txt);
                    tr.Cells.Add(td);

                    td = new TableCell();
                    select = new HtmlSelect();
                    foreach (var o in dictHorAligndict)
                    {
                        select.Items.Add(new ListItem(o.Value, o.Key));
                    };
                    select.ID = String.Format("select_horizontalTextAlign_{0}", i);
                    select.Attributes.Add("class", "form-control-smallinline");
                    select.Value = contents[i].HORIZONTAL_TEXT_ALIGN.ToString();
                    td.Controls.Add(select);
                    tr.Cells.Add(td);

                    td = new TableCell();
                    txt = new HtmlInputText();
                    txt.ID = String.Format("txt_cellFormat_{0}", i);
                    txt.Attributes.Add("class", "form-control-smallinline");
                    txt.Style.Add("width", "300px");
                    txt.Value = contents[i].CELL_FORMAT;
                    td.Controls.Add(txt);
                    tr.Cells.Add(td);

                    gvReportColumns.Rows.Add(tr);
                }
                //v1.8.8 Alex 2018.10.22 Add Group Width (e.g. Column A,B when grouped by two columns) - Begin
                for (var i = 0; i < contents_grps.Count(); i++)
                {
                    tr = new TableRow();
                    for (int j = 0; j < 10; j++)
                    {
                        td = new TableCell();
                        tr.Cells.Add(td);
                    }

                    td = new TableCell();
                    span = new HtmlGenericControl();
                    span.InnerHtml = "Group - " + groupNames[i];
                    td.Controls.Add(span);
                    hid = new HtmlInputHidden();
                    hid.ID = String.Format("hid_colname_grp_{0}", i);
                    hid.Value = "Group - " + groupNames[i];
                    td.Controls.Add(hid);
                    hid = new HtmlInputHidden();
                    hid.ID = String.Format("hid_coltype_grp_{0}", i);
                    hid.Value = "Group - " + groupNames[i];
                    td.Controls.Add(hid);
                    tr.Cells.AddAt(0, td);



                    td = new TableCell();
                    txt = new HtmlInputText();
                    txt.ID = String.Format("txt_width_grp_{0}", i);
                    temp = Convert.ToString(contents_grps[i].EXCEL_COLWIDTH);

                    if (temp.Contains("."))
                    {
                        txt.Value = temp.TrimEnd('0').TrimEnd('.');
                    }
                    else
                    {
                        txt.Value = temp;
                    }
                    //v1.0.0 - Cheong - 2016/02/25 - Show "Auto" for "-1" in col width
                    if ("-1" == txt.Value)
                    {
                        txt.Value = "Auto";
                    }
                    txt.Attributes.Add("class", "form-control-smallinline");
                    td.Controls.Add(txt);
                    tr.Cells.AddAt(2, td);

                    gvReportColumns.Rows.Add(tr);
                }
                //v1.8.8 Alex 2018.10.22 Add Group Width (e.g. Column A,B when grouped by two columns) - End
            }

        }
    }
}