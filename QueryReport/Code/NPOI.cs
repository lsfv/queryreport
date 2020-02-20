using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;

namespace QueryReport.Code
{
    public abstract class NPOIHelper
    {
        public const int TrailingBlank = 2;

        public struct subTotal
        {
            public int ColumnIndex { get; set; }
            public decimal total { get; set; }
        }
        public struct subAvg
        {
            public int ColumnIndex { get; set; }
            public decimal total { get; set; }
        }
        public struct subCount
        {
            public int ColumnIndex { get; set; }
            public int count { get; set; }
        }
        public class ColumnStyle
        {
            public XSSFCellStyle csHeader { get; set; }
            public XSSFCellStyle csBody { get; set; }
            public XSSFCellStyle csBodyMinor { get; set; }
            public XSSFCellStyle csSubTol { get; set; }
            public XSSFCellStyle csSubTol2 { get; set; }
            public XSSFCellStyle csSubCnt { get; set; }
            public XSSFCellStyle csSubCnt2 { get; set; }
            public XSSFCellStyle csTol { get; set; }
            public XSSFCellStyle csCnt { get; set; }
        }
        public class ColumnSetting
        {
            public int? FONT_SIZE { get; set; }
            public bool FONT_BOLD { get; set; }
            public bool FONT_ITALIC { get; set; }
            public int HORIZONTAL_TEXT_ALIGN { get; set; }
            public string CELL_FORMAT { get; set; }
            public string BACKGROUND_COLOR { get; set; }
            public string FONT_COLOR { get; set; }
            //public bool IS_ASCENDING { get; set; }
            //public int SORT_SEQUENCE { get; set; }
            public ColumnSetting(int? FONT_SIZE, bool FONT_BOLD, bool FONT_ITALIC, int HORIZONTAL_TEXT_ALIGN, string CELL_FORMAT, string BACKGROUND_COLOR, string FONT_COLOR)//, bool IS_ASCENDING, int SORT_SEQUENCE)
            {
                this.FONT_SIZE = FONT_SIZE;
                this.FONT_BOLD = FONT_BOLD;
                this.FONT_ITALIC = FONT_ITALIC;
                this.HORIZONTAL_TEXT_ALIGN = HORIZONTAL_TEXT_ALIGN;
                this.CELL_FORMAT = CELL_FORMAT;
                this.BACKGROUND_COLOR = BACKGROUND_COLOR;
                this.FONT_COLOR = FONT_COLOR;
                //this.IS_ASCENDING = IS_ASCENDING;
                //this.SORT_SEQUENCE = SORT_SEQUENCE;
            }
        }

        /// <summary>
        /// Generate Excel workbook from datatable
        /// </summary>
        /// <param name="dt">DataTable containing data to write into workbook</param>
        /// <param name="rptTitle">Report title located at row 2</param>
        /// <param name="ExtendedFields">Splitted string of CUSTOMRP.Model.REPORT.EXTENDFIELD</param>
        /// <param name="rowMore3">Query criteria or report header depending on mode.</param>
        /// <param name="content"></param>
        /// <param name="avg"></param>
        /// <param name="sum"></param>
        /// <param name="group"></param>
        /// <param name="subtotal"></param>
        /// <param name="subavg"></param>
        /// <param name="SubCount"></param>
        /// <param name="Count"></param>
        /// <param name="columnWidths">Number of characters to be displayed in column</param>
        /// <param name="print_orientation">Page orientation. Default: NotSet</param>
        /// <param name="print_fitToPage">Number of page to scale to fit when printing. When -1 it's not set. Default: -1</param>
        /// <param name="reportFooter">Report footer to be appended to the page</param>
        /// <returns></returns>
        public static XSSFWorkbook GetWorkbookFromDt(System.Data.DataTable dt, string rptTitle, string[] ExtendedFields,
            List<string> rptHeader, List<string> rpcr, Dictionary<string, string> content,
            List<string> avg, List<string> sum, List<string> group, List<string> subtotal, List<string> subavg, List<string> SubCount, List<string> Count,
            Dictionary<string, decimal> columnWidths = null, CUSTOMRP.Model.REPORT.Orientation print_orientation = CUSTOMRP.Model.REPORT.Orientation.NotSet, short print_fitToPage = -1,
            List<string> reportFooter = null, Dictionary<string, ColumnSetting> colSettings = null, string fontFamily = null, List<decimal> indentWidths = null, string subcountLabel = "Sub Count",
            List<string> sortonCols = null, List<bool> isAscending = null, List<int> seq = null)
        {
            bool showChangeOnly = ExtendedFields[CUSTOMRP.Model.REPORT.EXTENDFIELDs.ReportType] == CUSTOMRP.Model.REPORT.ExtReportType.ChangeOnly;
            bool hideHeaders = ExtendedFields[CUSTOMRP.Model.REPORT.EXTENDFIELDs.ReportType] == CUSTOMRP.Model.REPORT.ExtReportType.DataExport;
            bool hideCriteria = ExtendedFields[CUSTOMRP.Model.REPORT.EXTENDFIELDs.HideCriteria] == "1";
            DateTime emptyDateTime = new DateTime(1900, 01, 01);

            XSSFWorkbook XSSFworkbook = null;
            XSSFworkbook = InitializeWorkbook("DW Query Report", "Excel Report");

            XSSFSheet sheet = (XSSFSheet)XSSFworkbook.CreateSheet("Report");

            int columnsCount = dt.Columns.Count;

            XSSFCellStyle csDefault = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            csDefault.VerticalAlignment = VerticalAlignment.Top;
            if (fontFamily != null)
            {
                XSSFFont fontDefault = (XSSFFont)XSSFworkbook.CreateFont();
                fontDefault.FontName = fontFamily;
                csDefault.SetFont(fontDefault);
            };

            #region Report header styles

            XSSFCellStyle cscompay = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            XSSFCellStyle cstitle = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();

            cscompay.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Top;
            cscompay.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
            cstitle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Top;

            XSSFFont fontCOMPANY = (XSSFFont)XSSFworkbook.CreateFont();
            XSSFFont fontTITLE = (XSSFFont)XSSFworkbook.CreateFont();
            if (fontFamily != null)
            {
                fontCOMPANY.FontName = fontFamily;
                fontTITLE.FontName = fontFamily;
            };

            fontCOMPANY.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            fontCOMPANY.FontHeightInPoints = 12;

            fontTITLE.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            fontTITLE.FontHeightInPoints = 16;

            cscompay.SetFont(fontCOMPANY);
            cstitle.SetFont(fontTITLE);

            #endregion Report header styles

            XSSFCell title;
            int CriterialColumnsIndex = 0;

            if (!hideHeaders)
            {
                #region Company Name

                IRow row0 = sheet.CreateRow(0);
                XSSFCell com = (XSSFCell)row0.CreateCell(0);
                com.SetCellValue(QueryReport.Code.AppNum.companyName);
                com.CellStyle = cscompay;
                CriterialColumnsIndex++;

                #endregion Company Name

                #region Report title

                IRow row1 = sheet.CreateRow(1);
                title = (XSSFCell)row1.CreateCell(0);
                title.SetCellValue(rptTitle);
                title.CellStyle = cstitle;
                CriterialColumnsIndex++;

                #endregion Report Title

                #region Insert report header
                if (rptHeader != null && rptHeader.Count > 0)
                {
                    CriterialColumnsIndex++;
                    foreach (String sr in rptHeader)
                    {
                        IRow row = sheet.CreateRow(CriterialColumnsIndex);
                        for (int i = 0; i < sr.Split('|').Count(); i++)
                        {
                            XSSFCell cell = (XSSFCell)row.CreateCell(i);
                            cell.SetCellValue(sr.Split('|')[i]);
                            cell.CellStyle = csDefault;
                        };
                        CriterialColumnsIndex++;
                    }
                }
                #endregion Insert report header

                #region Insert criterial columns
                //v1.1.0 - Cheong - 2016/06/01 - Hide criteria text

                //v1.8.8 Alex 2018.10.05 Offset 2 columns
                if ((!hideCriteria) && (rpcr.Count > 0))
                {
                    CriterialColumnsIndex++;
                    foreach (String sr in rpcr)
                    {
                        IRow row = sheet.CreateRow(CriterialColumnsIndex);
                        XSSFCell cell = (XSSFCell)row.CreateCell(0);
                        cell.SetCellValue(sr);
                        cell.CellStyle = csDefault;
                        CriterialColumnsIndex++;
                    }
                }
                #endregion Insert criterial columns

                #region Print on Date
                CriterialColumnsIndex++;
                IRow rowDate = sheet.CreateRow(CriterialColumnsIndex);
                XSSFCell celldate = (XSSFCell)rowDate.CreateCell(0);
                //celldate.SetCellValue("Print on :" + DateTime.Now.ToString("yyyy-MM-dd"));
                celldate.SetCellValue("Print on : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //celldate.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;            
                celldate.CellStyle = csDefault;
                #endregion Print on Date

                // Skip 2 more lines
                CriterialColumnsIndex = CriterialColumnsIndex + 2;
            }

            #region Main report styles

            XSSFFont fontBold = (XSSFFont)XSSFworkbook.CreateFont();
            fontBold.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            if (fontFamily != null) fontBold.FontName = fontFamily;

            XSSFFont fontBoldUnderlined = (XSSFFont)XSSFworkbook.CreateFont();
            fontBoldUnderlined.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            fontBoldUnderlined.Underline = NPOI.SS.UserModel.FontUnderlineType.Single;
            if (fontFamily != null) fontBoldUnderlined.FontName = fontFamily;

            XSSFCellStyle csBold = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            csBold.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Top;
            csBold.SetFont(fontBold);

            XSSFCellStyle csBoldOnly = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            csBoldOnly.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Top;
            csBoldOnly.SetFont(fontBold);

            //XSSFCellStyle csBoldBtmLine = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            //csBoldBtmLine.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Top;
            //csBoldBtmLine.SetFont(fontBold);
            //csBoldBtmLine.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            //csBoldBtmLine.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

            XSSFCellStyle csBoldBtmLineOnly = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            csBoldBtmLineOnly.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Top;
            csBoldBtmLineOnly.SetFont(fontBold);
            csBoldBtmLineOnly.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            csBoldBtmLineOnly.BorderTop = NPOI.SS.UserModel.BorderStyle.None;

            XSSFCellStyle csBoldTopLine = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            csBoldTopLine.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Top;
            csBoldTopLine.SetFont(fontBold);
            csBoldTopLine.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            csBoldTopLine.BorderBottom = NPOI.SS.UserModel.BorderStyle.None;

            XSSFCellStyle groupheaderStyle = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            groupheaderStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Top;
            groupheaderStyle.SetFont(fontBoldUnderlined);
            if (showChangeOnly)    // show data change only
            {
                groupheaderStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            }

            //v1.2.0 Kim 2016.10.25 add column settings
            // construct the dict of style for each col
            Dictionary<string, ColumnStyle> columnStyles = new Dictionary<string, ColumnStyle> { };
            IDataFormat iDataFormat = XSSFworkbook.CreateDataFormat();
            for (int m = 0; m < columnsCount; m++)
            {
                ColumnStyle columnStyle = new ColumnStyle();
                String columnName = dt.Columns[m].ColumnName.Trim();
                ////v1.8.8 Alex 2018.10.24 Sometimes column name is not updated - Begin
                //if (!colSettings.ContainsKey(columnName)) continue;
                ////v1.8.8 Alex 2018.10.24 Sometimes column name is not updated - End
                ColumnSetting colSetting = colSettings[columnName];
                // get Horizontal Align
                Type columnDataType = dt.Columns[m].DataType;
                bool isNumColumn = columnDataType == typeof(Double) || columnDataType == typeof(Decimal) || columnDataType == typeof(Int32);
                HorizontalAlignment eHorizontalAlign = HorizontalAlignment.General;
                if (colSetting.HORIZONTAL_TEXT_ALIGN != (int)HorizontalAlignment.General)
                {
                    eHorizontalAlign = (HorizontalAlignment)colSetting.HORIZONTAL_TEXT_ALIGN;
                }
                else if (isNumColumn)
                {
                    eHorizontalAlign = HorizontalAlignment.Right;
                }
                else
                {
                    eHorizontalAlign = HorizontalAlignment.General;
                };
                // get format
                short? dataFormat = null;
                if (!string.IsNullOrEmpty(colSetting.CELL_FORMAT))
                {
                    dataFormat = iDataFormat.GetFormat(colSetting.CELL_FORMAT);
                }
                else
                {
                    if (columnDataType == typeof(DateTime))
                    {
                        var hvDate = false;
                        var hvTime = false;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (!Convert.IsDBNull(row[m]) && (DateTime)row[m] != emptyDateTime)
                            {
                                string datetimeStr = ((DateTime)row[m]).ToString("yyyy-MM-dd HH:mm:ss");
                                if (!datetimeStr.StartsWith("1900-01-01")) hvDate = true;
                                if (!datetimeStr.EndsWith("00:00:00")) hvTime = true;
                                if (hvDate && hvTime) break;
                            };
                        };
                        if (hvDate && !hvTime)
                        {
                            dataFormat = iDataFormat.GetFormat("yyyy-MM-dd");
                        }
                        else if (!hvDate && hvTime)
                        {
                            dataFormat = iDataFormat.GetFormat("HH:mm:ss");
                        }
                        else
                        {
                            dataFormat = iDataFormat.GetFormat("yyyy-MM-dd HH:mm:ss");
                        };
                    };
                };
                // get bg color
                XSSFColor bgColor = null;
                if (!string.IsNullOrEmpty(colSetting.BACKGROUND_COLOR))
                {
                    ColorConverter cc = new ColorConverter();
                    Color color = (Color)cc.ConvertFromString(colSetting.BACKGROUND_COLOR);
                    bgColor = new XSSFColor();
                    bgColor.SetRgb(new byte[3] { color.R, color.G, color.B });
                }
                // get color
                XSSFColor fontColor = null;
                if (!string.IsNullOrEmpty(colSetting.FONT_COLOR))
                {
                    ColorConverter cc = new ColorConverter();
                    Color color = (Color)cc.ConvertFromString(colSetting.FONT_COLOR);
                    fontColor = new XSSFColor();
                    fontColor.SetRgb(new byte[3] { color.R, color.G, color.B });
                }

                //Header
                XSSFCellStyle csHeader = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
                csHeader.SetFont(fontBold);
                csHeader.VerticalAlignment = VerticalAlignment.Top;
                // v1.2.0 Kim 2016.12.07 not to wrap if width set to 0
                csHeader.WrapText = columnWidths != null && columnWidths[dt.Columns[m].ColumnName.Trim()] == 0 ? false : true;
                csHeader.Alignment = eHorizontalAlign;
                csHeader.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                csHeader.BorderTop = NPOI.SS.UserModel.BorderStyle.None;
                columnStyle.csHeader = csHeader;
                //Body
                XSSFFont fontBody = (XSSFFont)XSSFworkbook.CreateFont();
                if (colSetting.FONT_SIZE.HasValue) fontBody.FontHeightInPoints = (short)colSetting.FONT_SIZE.Value;
                if (colSetting.FONT_BOLD) fontBody.IsBold = true;
                if (colSetting.FONT_ITALIC) fontBody.IsItalic = true;
                if (fontFamily != null) fontBody.FontName = fontFamily;
                if (fontColor != null) fontBody.SetColor(fontColor);
                XSSFCellStyle csBody = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
                csBody.SetFont(fontBody);
                csBody.Alignment = eHorizontalAlign;
                if (dataFormat != null) csBody.DataFormat = dataFormat.Value;
                if (bgColor != null)
                {
                    csBody.FillForegroundXSSFColor = bgColor;
                    csBody.FillPattern = FillPattern.SolidForeground;
                }
                csBody.VerticalAlignment = VerticalAlignment.Top;
                // v1.2.0 Kim 2016.12.07 not to wrap if width set to 0
                csBody.WrapText = columnWidths != null && columnWidths[dt.Columns[m].ColumnName.Trim()] == 0 ? false : true;
                columnStyle.csBody = csBody;
                //BodyMinor
                XSSFFont fontBodyMinor = (XSSFFont)XSSFworkbook.CreateFont();
                if (colSetting.FONT_SIZE.HasValue) fontBodyMinor.FontHeightInPoints = (short)colSetting.FONT_SIZE.Value;
                if (colSetting.FONT_BOLD) fontBodyMinor.IsBold = true;
                if (colSetting.FONT_ITALIC) fontBodyMinor.IsItalic = true;
                if (fontFamily != null) fontBodyMinor.FontName = fontFamily;
                fontBodyMinor.Color = IndexedColors.Grey25Percent.Index;

                XSSFCellStyle csBodyMinor = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
                csBodyMinor.SetFont(fontBodyMinor);
                csBodyMinor.Alignment = eHorizontalAlign;
                if (dataFormat != null) csBodyMinor.DataFormat = dataFormat.Value;
                if (bgColor != null)
                {
                    csBodyMinor.FillForegroundXSSFColor = bgColor;
                    csBodyMinor.FillPattern = FillPattern.SolidForeground;
                }
                csBodyMinor.VerticalAlignment = VerticalAlignment.Top;
                // v1.2.0 Kim 2016.12.07 not to wrap if width set to 0
                csBodyMinor.WrapText = columnWidths != null && columnWidths[dt.Columns[m].ColumnName.Trim()] == 0 ? false : true;
                columnStyle.csBodyMinor = csBodyMinor;

                XSSFFont fontTolCnt = (XSSFFont)XSSFworkbook.CreateFont();
                if (colSetting.FONT_SIZE.HasValue) fontTolCnt.FontHeightInPoints = (short)colSetting.FONT_SIZE.Value;
                fontTolCnt.IsBold = true;
                if (colSetting.FONT_ITALIC) fontTolCnt.IsItalic = true;
                if (fontFamily != null) fontTolCnt.FontName = fontFamily;
                if (fontColor != null) fontBody.SetColor(fontColor);
                //SubTol
                XSSFCellStyle csSubTol = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
                csSubTol.SetFont(fontTolCnt);
                csSubTol.VerticalAlignment = VerticalAlignment.Top;
                csSubTol.Alignment = eHorizontalAlign;
                if (dataFormat != null) csSubTol.DataFormat = dataFormat.Value;
                if (bgColor != null)
                {
                    csSubTol.FillForegroundXSSFColor = bgColor;
                    csSubTol.FillPattern = FillPattern.SolidForeground;
                }
                csSubTol.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin; //@@@
                csSubTol.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin; //@@@
                columnStyle.csSubTol = csSubTol;
                //SubTol2
                XSSFCellStyle csSubTol2 = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
                csSubTol2.SetFont(fontTolCnt);
                csSubTol2.VerticalAlignment = VerticalAlignment.Top;
                csSubTol2.Alignment = eHorizontalAlign;
                if (dataFormat != null) csSubTol2.DataFormat = dataFormat.Value;
                if (bgColor != null)
                {
                    csSubTol2.FillForegroundXSSFColor = bgColor;
                    csSubTol2.FillPattern = FillPattern.SolidForeground;
                }
                csSubTol2.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin; //@@@
                csSubTol2.BorderTop = NPOI.SS.UserModel.BorderStyle.None; //@@@
                columnStyle.csSubTol2 = csSubTol2;
                //SubCnt
                XSSFCellStyle csSubCnt = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
                csSubCnt.SetFont(fontTolCnt);
                csSubCnt.VerticalAlignment = VerticalAlignment.Top;
                csSubCnt.Alignment = eHorizontalAlign;
                if (bgColor != null)
                {
                    csSubCnt.FillForegroundXSSFColor = bgColor;
                    csSubCnt.FillPattern = FillPattern.SolidForeground;
                }
                csSubCnt.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin; //@@@
                csSubCnt.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin; //@@@
                columnStyle.csSubCnt = csSubCnt;
                //SubCnt2
                XSSFCellStyle csSubCnt2 = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
                csSubCnt2.SetFont(fontTolCnt);
                csSubCnt2.VerticalAlignment = VerticalAlignment.Top;
                csSubCnt2.Alignment = eHorizontalAlign;
                if (bgColor != null)
                {
                    csSubCnt2.FillForegroundXSSFColor = bgColor;
                    csSubCnt2.FillPattern = FillPattern.SolidForeground;
                }
                csSubCnt2.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin; //@@@
                csSubCnt2.BorderTop = NPOI.SS.UserModel.BorderStyle.None; //@@@
                columnStyle.csSubCnt2 = csSubCnt2;
                //Tol
                XSSFCellStyle csTol = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
                csTol.SetFont(fontTolCnt);
                csTol.BorderBottom = NPOI.SS.UserModel.BorderStyle.Double;
                csTol.VerticalAlignment = VerticalAlignment.Top;
                csTol.Alignment = eHorizontalAlign;
                if (dataFormat != null) csTol.DataFormat = dataFormat.Value;
                if (bgColor != null)
                {
                    csTol.FillForegroundXSSFColor = bgColor;
                    csTol.FillPattern = FillPattern.SolidForeground;
                }
                columnStyle.csTol = csTol;
                //Cnt
                XSSFCellStyle csCnt = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
                csCnt.SetFont(fontTolCnt);
                csCnt.BorderBottom = NPOI.SS.UserModel.BorderStyle.Double;
                csCnt.VerticalAlignment = VerticalAlignment.Top;
                csCnt.Alignment = eHorizontalAlign;
                if (bgColor != null)
                {
                    csCnt.FillForegroundXSSFColor = bgColor;
                    csCnt.FillPattern = FillPattern.SolidForeground;
                }
                columnStyle.csCnt = csCnt;

                columnStyles.Add(columnName, columnStyle);
            }

            #region old code
            //XSSFPalette palette = XSSFworkbook.GetCustomPalette();
            //XSSFColor groupheadercolor = palette.FindSimilarColor(0xC0, 0xC0, 0xC0);

            //XSSFCellStyle csgroupheader = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            //csgroupheader.FillForegroundColor = 15; // groupheadercolor.GetIndex();

            //XSSFFont whitefont = (XSSFFont)XSSFworkbook.CreateFont();
            //whitefont.Boldweight = (short)NPOI.SS.UserModel.FONT_BOLDWeight.Bold;
            //whitefont.Color = NPOI.SS.UserModel.IndexedColors.White.Index; // white
            //XSSFFont greyfont = (XSSFFont)XSSFworkbook.CreateFont();
            //greyfont.Boldweight = (short)NPOI.SS.UserModel.FONT_BOLDWeight.Bold;
            //greyfont.Color = NPOI.SS.UserModel.IndexedColors.Grey25Percent.Index; // white

            //XSSFCellStyle csRight = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            //csRight.SetFont(font1);
            //csRight.Alignment = HorizontalAlignment.Right;

            ////v1.0.0 - Cheong - 2016/03/31 - Add underline to column header
            //XSSFCellStyle cs_ul = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            //cs_ul.SetFont(fontBold);
            //cs_ul.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            ////v1.2.0 - Cheong - 2016/09/30 - Allow column header to have multiple line
            //cs_ul.VerticalAlignment = VerticalAlignment.Top;
            //cs_ul.WrapText = true;

            ////v1.0.0 - Cheong - 2016/03/31 - Add underline to column header
            //XSSFCellStyle csRight_ul = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            //csRight_ul.SetFont(fontBold);
            //csRight_ul.Alignment = HorizontalAlignment.Right;
            //csRight_ul.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            ////csRight_ul
            //csRight_ul.VerticalAlignment = VerticalAlignment.Top;
            //csRight_ul.WrapText = true;

            //XSSFCellStyle subtotalcellLeftStyle = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            //subtotalcellLeftStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            //subtotalcellLeftStyle.SetFont(fontBold);
            //subtotalcellLeftStyle.Alignment = HorizontalAlignment.Left;

            //XSSFCellStyle subtotalcellRightStyle = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            //subtotalcellRightStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            //subtotalcellRightStyle.SetFont(fontBold);
            //subtotalcellRightStyle.Alignment = HorizontalAlignment.Right;

            //XSSFCellStyle whitecellStyle = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            //whitecellStyle.SetFont(whitefont);
            //whitecellStyle.VerticalAlignment = VerticalAlignment.Top;
            //XSSFCellStyle greycellStyle = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            //greycellStyle.SetFont(greyfont);
            //greycellStyle.VerticalAlignment = VerticalAlignment.Top;
            //greycellStyle.WrapText = true;
            //XSSFCellStyle generalcellStyle = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            //generalcellStyle.VerticalAlignment = VerticalAlignment.Top;
            //generalcellStyle.WrapText = true;

            //XSSFCellStyle reporttotalLeftstyle = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            ////subtotalstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin; // do not set this, when setting it the style will merge with cellstyle
            //reporttotalLeftstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Double;
            //reporttotalLeftstyle.SetFont(fontBold);
            //reporttotalLeftstyle.Alignment = HorizontalAlignment.Left;

            //XSSFCellStyle reporttotalRightstyle = (XSSFCellStyle)XSSFworkbook.CreateCellStyle();
            ////subtotalstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin; // do not set this, when setting it the style will merge with cellstyle
            //reporttotalRightstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Double;
            //reporttotalRightstyle.SetFont(fontBold);
            //reporttotalRightstyle.Alignment = HorizontalAlignment.Right;
            #endregion old code

            #endregion Main report styles

            IRow rowColumnName = sheet.CreateRow(CriterialColumnsIndex);
            int rowIndex = CriterialColumnsIndex;

            //start set value to each cell.
            int[] Subsum_Index = WebHelper.getColumnIndexByColumnName_OrderbyIndex(subtotal.ToArray(), dt.Columns);
            int[] Subavg_Index = WebHelper.getColumnIndexByColumnName_OrderbyIndex(subavg.ToArray(), dt.Columns);
            int[] Subcount_Index = WebHelper.getColumnIndexByColumnName_OrderbyIndex(SubCount.ToArray(), dt.Columns);
            int[] sum_Index = WebHelper.getColumnIndexByColumnName_OrderbyIndex(sum.ToArray(), dt.Columns);
            int[] avg_Index = WebHelper.getColumnIndexByColumnName_OrderbyIndex(avg.ToArray(), dt.Columns);
            int[] count_Index = WebHelper.getColumnIndexByColumnName_OrderbyIndex(Count.ToArray(), dt.Columns);
            int[] groupIndex = WebHelper.getColumnIndexByColumnName(group.ToArray(), dt.Columns);
            int[] sortIndex = WebHelper.getColumnIndexByColumnName(sortonCols.ToArray(), dt.Columns);

            subTotal[] subTotalInfo = new subTotal[Subsum_Index.Length];
            subAvg[] subAvginfo = new subAvg[Subavg_Index.Length];
            subCount[] subcountinfo = new subCount[Subcount_Index.Length];

            int offset = groupIndex.Length, past_off = 0;
            //int[,] sumCnt = new int[offset, Subsum_Index.Length];
            int[] sumCnt = new int[offset];
            int[,] avgCnt = new int[offset, Subavg_Index.Length];
            decimal[,] sums = new decimal[offset, Subsum_Index.Length];
            decimal[,] avgs = new decimal[offset, Subavg_Index.Length];

            //Column header
            for (int m = 0; m < offset; m++)
            {
                ICell cell = rowColumnName.CreateCell(m);
                cell.CellStyle = csBoldBtmLineOnly;
            }
            csBold.BorderBottom = NPOI.SS.UserModel.BorderStyle.None;    // Reset border settings
            for (int m = 0; m < columnsCount; m++)
            {
                DataColumn dc = dt.Columns[m];
                String columnName = dt.Columns[m].ColumnName.Trim();
                title = (XSSFCell)rowColumnName.CreateCell(m + offset); // groupIndex.Lengthreplace 2 by # of groups
                //v1.0.0 - Cheong - 2015/05/27 - Replace with displayname
                //title.SetCellValue((dc.ColumnName).Trim());
                if (! groupIndex.Contains(m))
                title.SetCellValue(content[dc.ColumnName.Trim()].Replace("\\n", "\r\n"));
                //v1.2.0 Kim 2016.10.25 add column settings
                ////v1.0.0 - Cheong - 2016/03/21 - Align right for column headers of numeric fields
                //if ((dt.Columns[m].DataType == typeof(Double)) || (dt.Columns[m].DataType == typeof(Decimal)) || (dt.Columns[m].DataType == typeof(Int32)))
                //{
                //    title.CellStyle = csRight_ul;
                //}
                //else
                //{
                //    title.CellStyle = cs_ul;
                //}
                title.CellStyle = columnStyles[columnName].csHeader;
            }


            for (int x = 0; x < subTotalInfo.Length; x++)
            {
                subTotalInfo[x].ColumnIndex = Subsum_Index[x];
            }
            for (int y = 0; y < subAvginfo.Length; y++)
            {
                subAvginfo[y].ColumnIndex = Subavg_Index[y];
            }

            Type[] numeric = new Type[3] { typeof(Double), typeof(Decimal), typeof(Int32) };
            // v1.8.8 Alex 2018.10.11 - Pre-sort DataTable - Begin
            if (dt.Rows.Count > 0) //&& (!colSettings.All(x => x.Value.SORT_SEQUENCE == 100)))   // Alex - if all sort sequences are 100, the list will not be sorted.
            {

                OrderedEnumerableRowCollection<DataRow> tmp = null;
                //bool grouped = false;
                // 1. Sort columns which belong to a group (only if the user specified grouping).
                if (sortIndex.Length > 0)
                {
                    DateTime tpdatetime;
                    //grouped = true;
                    if (numeric.Contains(dt.Columns[sortIndex[0]].DataType))
                    {
                        tmp = isAscending[0] ? dt.AsEnumerable().OrderBy(f => Convert.ToDouble(f[sortIndex[0]]))
                            : dt.AsEnumerable().OrderByDescending(f => Convert.ToDouble(f[sortIndex[0]]));
                    }
                    else if (dt.Columns[sortIndex[0]].DataType == typeof(DateTime))
                    {
                        tmp = isAscending[0] ? dt.AsEnumerable().OrderBy(f => DateTime.TryParse(Convert.ToString(f[sortIndex[0]]), out tpdatetime) ? tpdatetime : DateTime.MinValue )
                            : dt.AsEnumerable().OrderByDescending(f => DateTime.TryParse(Convert.ToString(f[sortIndex[0]]), out tpdatetime) ? tpdatetime : DateTime.MinValue );
                    }
                    else
                    {
                        tmp = isAscending[0] ? dt.AsEnumerable().OrderBy(f => Convert.ToString(f[sortIndex[0]]))
                            : dt.AsEnumerable().OrderByDescending(f => Convert.ToString(f[sortIndex[0]]));
                    }
                    for (int z = 1; z < sortIndex.Count(); z++)
                    {

                        if (isAscending[z])
                        {
                            var thisI = z;
                            tmp = numeric.Contains(dt.Columns[sortIndex[thisI]].DataType) ? tmp.ThenBy(f => f[sortIndex[thisI]] is DBNull ? 0d : Convert.ToDouble(f[sortIndex[thisI]]))
                                : (dt.Columns[sortIndex[thisI]].DataType == typeof(DateTime) ? tmp.ThenBy(f => DateTime.TryParse(Convert.ToString(f[sortIndex[thisI]]), out tpdatetime) ? tpdatetime : DateTime.MinValue) : tmp.ThenBy(f => Convert.ToString(f[sortIndex[thisI]])));
                        }
                        else
                        {
                            var thisI = z;
                            tmp = numeric.Contains(dt.Columns[sortIndex[thisI]].DataType) ? tmp.ThenByDescending(f => f[sortIndex[thisI]] is DBNull ? 0d : Convert.ToDouble(f[sortIndex[thisI]]))
                                : (dt.Columns[sortIndex[thisI]].DataType == typeof(DateTime) ? tmp.ThenByDescending(f => DateTime.TryParse(Convert.ToString(f[sortIndex[thisI]]), out tpdatetime) ? tpdatetime : DateTime.MinValue) : tmp.ThenByDescending(f => Convert.ToString(f[sortIndex[thisI]])));
                        }
                    }
                    dt = tmp.CopyToDataTable();
                }
                //// 2. Sort columns which do not belong to a group
                //// Sort sequence only affects non-group columns
                //var sorted = colSettings.Select(x => x.Value.SORT_SEQUENCE).OrderBy(x => x).ToList();
                //var orig = colSettings.Select(x => x.Value.SORT_SEQUENCE).ToList();

                //int[] real_sequence = new int[colSettings.Count];
                //for (int x = 0; x < colSettings.Count; x++)
                //{
                //    var i = orig.IndexOf(sorted[x]);
                //    real_sequence[x] = i;
                //    orig[i] = int.MinValue;
                //    //var i = sorted.IndexOf(colSettings.ElementAt(x).Value.SORT_SEQUENCE);
                //    //real_sequence[x] = i;
                //    //sorted[i] = int.MinValue;
                //}


                //for (int z = 0; z < dt.Columns.Count; z++)
                //{
                //    if (groupIndex.Contains(real_sequence[z])) continue;
                //    if (!grouped && z == 0)
                //    {
                //        if (numeric.Contains(dt.Columns[real_sequence[0]].DataType))
                //        {

                //            tmp = colSettings[colSettings.Keys.ElementAt(real_sequence[0])].IS_ASCENDING ? dt.AsEnumerable().OrderBy(f => Convert.IsDBNull(f[real_sequence[0]]) ? 0 : Convert.ToDouble(f[real_sequence[0]])) : dt.AsEnumerable().OrderByDescending(f => Convert.IsDBNull(f[real_sequence[0]]) ? 0 : Convert.ToDouble(f[real_sequence[0]]));
                //        }
                //        else
                //        {
                //            tmp = colSettings[colSettings.Keys.ElementAt(real_sequence[0])].IS_ASCENDING ? dt.AsEnumerable().OrderBy(f => Convert.IsDBNull(f[real_sequence[0]]) ? "0" : Convert.ToString(f[real_sequence[0]])) : dt.AsEnumerable().OrderByDescending(f => Convert.IsDBNull(f[real_sequence[0]]) ? "0" : Convert.ToString(f[real_sequence[0]]));
                //        }
                //    }
                //    else
                //    {
                //        var thisI = real_sequence[z];
                //        if (colSettings[colSettings.Keys.ElementAt(real_sequence[z])].IS_ASCENDING)      // colSettings[colSettings.Keys.ElementAt(z)].IS_ASCENDING
                //        {
                //            tmp = numeric.Contains(dt.Columns[thisI].DataType) ? tmp.ThenBy(f => Convert.IsDBNull(f[thisI]) ? 0 : Convert.ToDouble(f[thisI])) : tmp.ThenBy(f => Convert.IsDBNull(f[thisI]) ? "0" : Convert.ToString(f[thisI]));
                //        }
                //        else
                //        {
                //            //throw new Exception("Descending");
                //            tmp = numeric.Contains(dt.Columns[thisI].DataType) ? tmp.ThenByDescending(f => Convert.IsDBNull(f[thisI]) ? 0 : Convert.ToDouble(f[thisI])) : tmp.ThenByDescending(f => Convert.IsDBNull(f[thisI]) ? "0" : Convert.ToString(f[thisI]));
                //        }
                //    }
                //}
            }
            //for (int z = 0; z < dt.Columns.Count; z++)
            //{
            //    if (groupIndex.Contains(z)) continue;
            //    if (!grouped && z == 0)
            //    {
            //        tmp = colSettings[colSettings.Keys.ElementAt(0)].IS_ASCENDING ? dt.AsEnumerable().OrderBy(f => Convert.ToString(f[0])) : dt.AsEnumerable().OrderByDescending(f => Convert.ToString(f[0]));
            //    }
            //    else
            //    {
            //        if (colSettings[colSettings.Keys.ElementAt(z)].IS_ASCENDING)      // colSettings[colSettings.Keys.ElementAt(z)].IS_ASCENDING
            //        {
            //            var thisI = z;
            //            tmp = tmp.ThenBy(f => Convert.ToString(f[thisI]));
            //        }
            //        else
            //        {
            //            //throw new Exception("Descending");
            //            var thisI = z;
            //            tmp = tmp.ThenByDescending(f => Convert.ToString(f[thisI]));
            //        }
            //    }
            //}
            //dt = tmp.CopyToDataTable();

            //dt = dt.AsEnumerable().OrderByDescending(f => Convert.ToString(f[groupIndex[0]])).ThenByDescending(f => Convert.ToString(f[groupIndex[1]]))
            //    .ThenByDescending(f => Convert.ToString(f[groupIndex[2]])).ThenByDescending(f => Convert.ToString(f[groupIndex[3]])).CopyToDataTable();
            
            // v1.8.8 Alex 2018.10.11 - Pre-sort DataTable - End

            int tempRowsCount = 0;
            int off = 0;
            bool GroupBegin = false;
            bool groupIsEmp = false;    // v1.8.8 Alex 2018.10.26 - Adjustment for Chinese Women - If group content is empty string, don't show header
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                rowIndex++;

                IRow row = sheet.CreateRow(rowIndex);

                //if (showType == "0")
                //{
                if (groupIndex.Length > 0)
                {
                    #region group header

                    GroupBegin = false;

                    if (i == 0)
                    {
                        GroupBegin = true;
                    }
                    else
                    {
                        past_off = off;
                        off = 0;
                        for (int g_i = 0; g_i < groupIndex.Count(); g_i++)
                        {
                            if (dt.Rows[i][groupIndex[g_i]].ToString() == dt.Rows[i - 1][groupIndex[g_i]].ToString())
                            {
                                if (g_i < groupIndex.Count() - 1) off++;
                            }
                            else
                            {
                                GroupBegin = true;
                                break;
                            }
                        }
                        if (GroupBegin)
                        {
                            // v1.8.8 2018.10.09 Alex - Tree Subtotal, SubAvg, Subcount
                            if (past_off >= off)
                            {
                                for (int x = past_off; x >= off + 1; x--)
                                {

                                    #region sub total

                                    if (Subsum_Index.Length > 0)
                                    {
                                        //rowIndex++;
                                        IRow subTotalrow = sheet.CreateRow(rowIndex);
                                        //for (int j = (x - 1 >= 0) ? (x - 1) : 0; j < columnsCount + offset; j++)
                                        //{
                                        //    // Add empty cells
                                        //    ICell cell = subTotalrow.CreateCell(j);
                                        //    cell.CellStyle = csBold;
                                        //    cell.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                                        //}

                                        for (int j = 0; j < subTotalInfo.Length; j++)
                                        {
                                            int subtotalColumnIndex = subTotalInfo[j].ColumnIndex;
                                            ICell subTotal = subTotalrow.CreateCell(subtotalColumnIndex + offset);
                                            if (j == 0)
                                            {
                                                if (subtotalColumnIndex + offset == 0)
                                                {
                                                    subTotal.SetCellValue((sums[x - 1, j]).ToString());
                                                }
                                                else
                                                {
                                                    ICell subTotalTitle = subTotalrow.CreateCell(offset - 1);
                                                    //-->subTotalTitle.SetCellValue("Total");
                                                    //subTotalTitle.CellStyle = csBold;
                                                    //subTotalTitle.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                                                    //subTotal.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                                                    if (dt.Columns[j].DataType == typeof(Int32))
                                                    {
                                                        subTotal.SetCellValue(Convert.ToInt32(sums[x - 1, j]));
                                                    }
                                                    else
                                                    {
                                                        subTotal.SetCellValue(Convert.ToDouble(sums[x - 1, j]));
                                                    }

                                                    subTotalTitle.CellStyle = csBoldOnly;     //@@@
                                                }
                                            }
                                            else
                                            {
                                                if (dt.Columns[j].DataType == typeof(Int32))
                                                {
                                                    subTotal.SetCellValue(Convert.ToInt32(sums[x - 1, j]));
                                                }
                                                else
                                                {
                                                    subTotal.SetCellValue(Convert.ToDouble(sums[x - 1, j]));
                                                }
                                            }
                                            String columnName = dt.Columns[subtotalColumnIndex].ColumnName.Trim();
                                            subTotal.CellStyle = columnStyles[columnName].csSubTol2;
                                            subTotalInfo[j].total = 0;

                                            //subTotal.CellStyle = csBoldBtmLine;     //@@@
                                        }
                                    }

                                    #endregion

                                    #region sub count
                                    if (Subcount_Index.Length > 0)
                                    {
                                        rowIndex++;
                                        IRow subCountrow = sheet.CreateRow(rowIndex);
                                        //for (int j = (x - 1 >= 0) ? (x - 1) : 0; j < columnsCount + offset; j++)
                                        //{
                                        //    ICell cell = subCountrow.CreateCell(j);
                                        //    cell.CellStyle = csBold;
                                        //    cell.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                                        //}

                                        for (int j = 0; j < Subcount_Index.Length; j++)
                                        {
                                            ICell subCount = subCountrow.CreateCell(Subcount_Index[j] + offset);
                                            if (j == 0)//first column.
                                            {
                                                if (Subcount_Index[j] == 0)
                                                {
                                                    subCount.SetCellValue(sumCnt[x - 1]);
                                                }
                                                else
                                                {
                                                    ICell subCountTitle = subCountrow.CreateCell(offset - 1);
                                                    //-->subCountTitle.SetCellValue("Count");
                                                    //subCountTitle.CellStyle = csBold;
                                                    //subCount.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                                                    subCount.SetCellValue(sumCnt[x - 1]);
                                                    subCountTitle.CellStyle = csBoldOnly;     //@@@
                                                }
                                            }
                                            else
                                            {
                                                subCount.SetCellValue(tempRowsCount);
                                            }
                                            String columnName = dt.Columns[Subcount_Index[j]].ColumnName.Trim();
                                            subCount.CellStyle = columnStyles[columnName].csSubCnt2;

                                            //subCount.CellStyle = csBoldBtmLine;     //@@@
                                        }
                                    }
                                    #endregion

                                    #region sub avg

                                    if (Subavg_Index.Length > 0)
                                    {
                                        rowIndex++;
                                        IRow subAvgrow = sheet.CreateRow(rowIndex);
                                        for (int j = (x - 1 >= 0) ? (x - 1) : 0; j < columnsCount + offset; j++)
                                        {
                                            ICell cell = subAvgrow.CreateCell(j);
                                            //cell.CellStyle = csBold;
                                            //cell.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                                        }

                                        for (int j = 0; j < Subavg_Index.Length; j++)
                                        {
                                            ICell subTotal = subAvgrow.CreateCell(subAvginfo[j].ColumnIndex + offset);
                                            if (j == 0)
                                            {
                                                if (subAvginfo[j].ColumnIndex == 0)
                                                {
                                                    if (tempRowsCount != 0)
                                                    {
                                                        subTotal.SetCellValue(Convert.ToDouble((avgCnt[x - 1, j] == 0) ? "0" : (avgs[x - 1, j] / avgCnt[x - 1, j]).ToString("0.##")));
                                                        //subTotal.SetCellValue("Avg:" + Convert.ToDouble((avgs[x, j] / avgCnt[x, j]).ToString("0.##")));
                                                    }
                                                    else
                                                    {
                                                        subTotal.SetCellValue("0");
                                                    }

                                                }
                                                else
                                                {
                                                    ICell subTotalTitle = subAvgrow.CreateCell(offset - 1); //(x - 1 >= 0) ? (x - 1) : 0
                                                    //-->subTotalTitle.SetCellValue("Avg");
                                                    //subTotalTitle.CellStyle = csBold;
                                                    //subTotal.CellStyle = csBold;
                                                    if (tempRowsCount != 0)
                                                    {
                                                        subTotal.SetCellValue(Convert.ToDouble((avgCnt[x - 1, j] == 0) ? "0" : (avgs[x - 1, j] / avgCnt[x - 1, j]).ToString("0.##")));
                                                    }
                                                    else
                                                    {
                                                        subTotal.SetCellValue("0");
                                                    }
                                                    subTotalTitle.CellStyle = csBoldOnly;     //@@@

                                                }
                                            }
                                            else
                                            {
                                                if (tempRowsCount != 0)
                                                {
                                                    subTotal.SetCellValue(Convert.ToDouble((avgCnt[x - 1, j] == 0) ? "0" : (avgs[x - 1, j] / avgCnt[x - 1, j]).ToString("0.##")));
                                                }
                                                else
                                                {
                                                    subTotal.SetCellValue("0");
                                                }
                                            }

                                            String columnName = dt.Columns[subAvginfo[j].ColumnIndex].ColumnName.Trim();
                                            subTotal.CellStyle = columnStyles[columnName].csSubTol2;
                                            //subTotal.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;


                                            subAvginfo[j].total = 0;

                                            //subTotal.CellStyle = csBoldBtmLine;     //@@@
                                        }
                                    }
                                    #endregion

                                    rowIndex++;
                                }

                                for (int x = off; x < offset; x++) // subAvginfo[j].ColumnIndex    1 -> 3
                                {
                                    for (int o = 0; o < Subsum_Index.Length; o++)
                                    {
                                        sumCnt[x] = 0;//sumCnt[x, o] = 0;
                                        sums[x, o] = 0m;
                                    }
                                    for (int o = 0; o < Subavg_Index.Length; o++)
                                    {
                                        avgCnt[x, o] = 0;
                                        avgs[x, o] = 0m;
                                    }
                                }
                                //rowIndex++;
                            }
                        }

                    }

                    if (GroupBegin)
                    {
                        //v1.0.0 - Cheong - 2016/03/18 - Insert new row before each group (except the first one)
                        if (i != 0)
                        {
                            if (off==0) rowIndex++;
                            row = sheet.CreateRow(rowIndex);
                        }

                        tempRowsCount = 0;

                        //StringBuilder sb = new StringBuilder();
                        String str;
                        //v1.0.0 - Cheong - 2015/05/27 - Show displayname instead
                        //sb.AppendFormat("{0} : {1}", group[0], dt.Rows[i][groupIndex[0]]);
                        //v1.0.0 - Cheong - 2015/06/02 - Standardize date format on output
                        /*
                        sb.AppendFormat("{0} : {1}", content[group[0]], dt.Rows[i][groupIndex[0]]);
                        for (int g_i = 1; g_i < groupIndex.Length; g_i++)
                        {
                            //sb.AppendFormat("     {0} : {1}", group[g_i], dt.Rows[i][groupIndex[g_i]]);
                            sb.AppendFormat("     {0} : {1}", content[group[g_i]], dt.Rows[i][groupIndex[g_i]]);
                        }
                         * */

                        for (int g_i = 0; g_i < offset; g_i++)
                        {
                            //if ((g_i == 0) && (groupIndex[g_i] != 0))
                            //{
                            //    continue;
                            //}
                            if (dt.Rows[i][groupIndex[g_i]].ToString() == string.Empty)
                            {
                                groupIsEmp = true;
                            }
                            else if (g_i >= off)
                            {
                                if (!Convert.IsDBNull(dt.Rows[i][groupIndex[g_i]]))
                                {
                                    if (dt.Columns[groupIndex[g_i]].DataType == typeof(DateTime))
                                    {
                                        if ((DateTime)dt.Rows[i][groupIndex[g_i]] != emptyDateTime)
                                        {

                                            string l_strDateTime = ((DateTime)dt.Rows[i][groupIndex[g_i]]).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                            l_strDateTime = l_strDateTime.Replace("1900-01-01", "").Trim();
                                            l_strDateTime = l_strDateTime.Replace("00:00:00", "").Trim();

                                            str = string.Format("{0} : {1}", content[dt.Columns[groupIndex[g_i]].ColumnName], l_strDateTime);
                                            //row.CreateCell(j).SetCellValue(((DateTime)dt.Rows[i][j]).ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture));
                                        }
                                        else
                                        {
                                            str = string.Format("{0} : {1}", content[dt.Columns[groupIndex[g_i]].ColumnName], String.Empty);
                                        };
                                    }
                                    else
                                    {
                                        str = string.Format("{0} : {1}", content[dt.Columns[groupIndex[g_i]].ColumnName], dt.Rows[i][groupIndex[g_i]]);
                                    }
                                }
                                else
                                {
                                    str = string.Format("{0} : {1}", content[dt.Columns[groupIndex[g_i]].ColumnName], String.Empty);
                                }
                                #region AddCell

                                ICell cell = row.CreateCell(g_i);
                                //v1.0.0 - Cheong - 2016/03/18 - Change group header style to add top border
                                //cell.CellStyle = cs;

                                cell.CellStyle = groupheaderStyle;
                                cell.SetCellValue(str);

                                for (int j = g_i + 1; j < columnsCount; j++)
                                {
                                    // Add empty cells
                                    cell = row.CreateCell(j);
                                    cell.CellStyle = groupheaderStyle;

                                }

                                //shift row
                                rowIndex++;
                                row = sheet.CreateRow(rowIndex);

                                //// Merge the group cell and empty cells
                                //NPOI.SS.Util.CellRangeAddress cra = new NPOI.SS.Util.CellRangeAddress(rowIndex, rowIndex, g_i, dt.Columns.Count - 1);
                                //sheet.AddMergedRegion(cra);
                            }

                        }
                                #endregion

                    }

                    #endregion
                }


                for (int j = 0; j < columnsCount; j++)
                {
                    if (groupIndex.Contains(j)) continue;
                    ICell cell = row.CreateCell(j + offset);
                    //v1.2.0 Kim 2016.10.25 add column settings
                    //cell.CellStyle = generalcellStyle;
                    String columnName = dt.Columns[j].ColumnName.Trim();
                    //v1.0.0 - Cheong - 2016/03/18 - Alter Show changed data only logic
                    if (showChangeOnly && (i != 0) && (dt.Rows[i][j].Equals(dt.Rows[i - 1][j]) && (dt.Rows[i][0].Equals(dt.Rows[i - 1][0]))))
                    {
                        //v1.2.0 Kim 2016.10.25 add column settings
                        //cell.CellStyle = greycellStyle;
                        cell.CellStyle = columnStyles[columnName].csBodyMinor;
                    }
                    else
                    {
                        cell.CellStyle = columnStyles[columnName].csBody;
                    };

                    if (!Convert.IsDBNull(dt.Rows[i][j]))
                    {
                        if (dt.Columns[j].DataType == typeof(DateTime))
                        {
                            //v1.2.0 Kim 2016.10.25 set date instead of set string to cell
                            //string l_strDateTime = ((DateTime)dt.Rows[i][j]).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                            //l_strDateTime = l_strDateTime.Replace("1900-01-01", "").Trim();
                            //l_strDateTime = l_strDateTime.Replace("00:00:00", "").Trim();
                            //cell.SetCellValue(l_strDateTime);
                            ////row.CreateCell(j).SetCellValue(((DateTime)dt.Rows[i][j]).ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture));
                            if ((DateTime)dt.Rows[i][j] != emptyDateTime) cell.SetCellValue((DateTime)dt.Rows[i][j]);

                        }
                        else if ((dt.Columns[j].DataType == typeof(Double)) || (dt.Columns[j].DataType == typeof(Decimal)) || (dt.Columns[j].DataType == typeof(Int32)))
                        {
                            cell.SetCellValue(Convert.ToDouble(dt.Rows[i][j]));
                        }
                        else
                        {
                            cell.SetCellValue(dt.Rows[i][j].ToString());
                        }
                    }
                    else
                    {
                        cell.SetCellValue(String.Empty);
                    }
                }
                //whether need sub .
                if (offset > 0)
                {
                    #region Grouping

                    tempRowsCount++;
                    //int real_off = off;
                    //if (((past_off != off) && (off == 0)) || (i == 0))
                    //{
                    //    real_off = offset - 1;
                    //}
                    //sumCnt[real_off, j]++;
                    for (int x = 0; x < offset; x++)
                    {
                        sumCnt[x]++;
                    }

                    bool SubTotal = false;
                    decimal dectemp;

                    if (i == dt.Rows.Count - 1)
                    {
                        SubTotal = true;
                    }
                    else
                    {
                        for (int g_i = offset - 1; g_i >= 0; g_i--)
                        {
                            if (dt.Rows[i][groupIndex[g_i]].ToString() != dt.Rows[i + 1][groupIndex[g_i]].ToString())
                            {
                                SubTotal = true;
                                break;
                            }
                        }
                    }

                    for (int j = 0; j < subTotalInfo.Length; j++)
                    {
                        //v1.1.0 - Cheong - 2016/05/27 - Do not throw exception when attempt to sum a field that can have null value
                        //subTotalInfo[j].total += Decimal.Parse(dt.Rows[i][subTotalInfo[j].ColumnIndex].ToString());
                        if (Decimal.TryParse(dt.Rows[i][subTotalInfo[j].ColumnIndex].ToString(), out dectemp))
                        {
                            subTotalInfo[j].total += dectemp;
                            // Alex - Below
                            for (int x = 0; x < offset; x++)
                            {
                                sums[x, j] += dectemp; //sums[real_off, j] += dectemp;
                            }
                        }
                    }
                    for (int j = 0; j < subAvginfo.Length; j++)
                    {
                        //subAvginfo[j].total += Decimal.Parse(dt.Rows[i][subAvginfo[j].ColumnIndex].ToString());
                        if (Decimal.TryParse(dt.Rows[i][subAvginfo[j].ColumnIndex].ToString(), out dectemp))
                        {
                            // TBD:
                            subAvginfo[j].total += dectemp;
                            // Alex - Below
                            for (int x = 0; x < offset; x++)
                            {
                                avgs[x, j] += dectemp; //avgs[real_off, j] += dectemp;
                                avgCnt[x, j]++; //avgCnt[real_off, j]++;
                            }
                        }
                    }

                    // Alex TODO TODO TODO TODO TODO: Add empty cells with the sub* style
                    // "Total:" (w/ colon) when there are no groups
                    // "Total" is always totalCell = (# of groups - 1)  >= 0 ? 0 : (# of groups - 1).
                    // Bold and top border from totalCell to totalCell + # of content

                    bool firstAggregate = true;
                    IRow borderTopRow = null;

                    if (SubTotal && ((Subsum_Index.Length > 0) || (Subcount_Index.Length > 0) || (Subavg_Index.Length > 0)))
                    {
                        rowIndex++;
                        borderTopRow = sheet.CreateRow(rowIndex);
                        for (int j = 0; j < offset + columnsCount; j++)
                        {
                            ICell cell = borderTopRow.CreateCell(j);
                            cell.CellStyle = csBoldTopLine;
                        }
                    }

                    #region sub total

                    if (SubTotal && (Subsum_Index.Length > 0))
                    {

                        for (int j = 0; j < subTotalInfo.Length; j++)
                        {
                            int subtotalColumnIndex = subTotalInfo[j].ColumnIndex;
                            ICell subTotal = borderTopRow.CreateCell(subtotalColumnIndex + offset);
                            if (j == 0)
                            {
                                if (subtotalColumnIndex + offset == 0)
                                {
                                    subTotal.SetCellValue("Sub Total:" + (subTotalInfo[j].total).ToString());
                                }
                                else
                                {
                                    ICell subTotalTitle = borderTopRow.CreateCell((offset - 1) >= 0 ? (offset - 1) : 0);
                                    subTotalTitle.SetCellValue("Sub Total");
                                    //subTotalTitle.CellStyle = csBold;
                                    //subTotalTitle.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                                    //subTotal.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                                    if (dt.Columns[j].DataType == typeof(Int32))
                                    {
                                        subTotal.SetCellValue(Convert.ToInt32(subTotalInfo[j].total));
                                    }
                                    else
                                    {
                                        subTotal.SetCellValue(Convert.ToDouble(subTotalInfo[j].total));
                                    }
                                    subTotalTitle.CellStyle = csBoldTopLine;
                                }
                            }
                            else
                            {
                                if (dt.Columns[j].DataType == typeof(Int32))
                                {
                                    subTotal.SetCellValue(Convert.ToInt32(subTotalInfo[j].total));
                                }
                                else
                                {
                                    subTotal.SetCellValue(Convert.ToDouble(subTotalInfo[j].total));
                                }
                            }
                            //v1.2.0 Kim 2016.10.25 add column settings
                            ////v1.0.0 - Cheong - 2016/03/21 - Align aggreate fields of columns
                            //if ((dt.Columns[subTotalInfo[j].ColumnIndex].DataType == typeof(Double)) || (dt.Columns[subTotalInfo[j].ColumnIndex].DataType == typeof(Decimal)) || (dt.Columns[subTotalInfo[j].ColumnIndex].DataType == typeof(Int32)))
                            //{
                            //    subTotal.CellStyle = subtotalcellRightStyle;
                            //}
                            //else
                            //{
                            //    subTotal.CellStyle = subtotalcellLeftStyle;
                            //}
                            String columnName = dt.Columns[subtotalColumnIndex].ColumnName.Trim();
                            subTotal.CellStyle = columnStyles[columnName].csSubTol;
                            //subTotal.CellStyle = csBoldBtmLine;
                            subTotalInfo[j].total = 0;
                        }
                        firstAggregate = false;
                        rowIndex++;
                        borderTopRow = sheet.CreateRow(rowIndex);
                    }

                    #endregion

                    #region sub count
                    if (SubTotal && (Subcount_Index.Length > 0))
                    {
                        if (groupIsEmp)
                        {
                            groupIsEmp = false;
                        } else {
                            for (int j = 0; j < Subcount_Index.Length; j++)
                            {
                                ICell subCount = borderTopRow.CreateCell(Subcount_Index[j] + offset);
                                if (j == 0)//first column.
                                {
                                    if (Subcount_Index[j] == 0)
                                    {
                                        subCount.SetCellValue(subcountLabel + ":" + tempRowsCount.ToString());

                                    }
                                    else
                                    {
                                        ICell subCountTitle = borderTopRow.CreateCell((offset - 1) >= 0 ? (offset - 1) : 0);
                                        subCountTitle.SetCellValue(subcountLabel);
                                        //subCountTitle.CellStyle = csBold;
                                        //subCount.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                                        subCount.SetCellValue(tempRowsCount);
                                        subCountTitle.CellStyle = firstAggregate ? csBoldTopLine : csBoldOnly;
                                    }
                                }
                                else
                                {
                                    subCount.SetCellValue(tempRowsCount);
                                }
                                //v1.2.0 Kim 2016.10.25 add column settings
                                ////v1.0.0 - Cheong - 2016/03/21 - Align aggreate fields of columns
                                //if ((dt.Columns[Subcount_Index[j]].DataType == typeof(Double)) || (dt.Columns[Subcount_Index[j]].DataType == typeof(Decimal)) || (dt.Columns[Subcount_Index[j]].DataType == typeof(Int32)))
                                //{
                                //    subCount.CellStyle = subtotalcellRightStyle;
                                //}
                                //else
                                //{
                                //    subCount.CellStyle = subtotalcellLeftStyle;
                                //}
                                String columnName = dt.Columns[Subcount_Index[j]].ColumnName.Trim();
                                subCount.CellStyle = firstAggregate ? columnStyles[columnName].csSubCnt : columnStyles[columnName].csSubCnt2;
                                //subCount.CellStyle = csBoldBtmLine;
                            }
                            firstAggregate = false;
                            rowIndex++;
                            borderTopRow = sheet.CreateRow(rowIndex);
                        }
                    }

                    #endregion

                    #region sub avg

                    if (SubTotal && (Subavg_Index.Length > 0))
                    {
                        for (int j = 0; j < Subavg_Index.Length; j++)
                        {
                            ICell subTotal = borderTopRow.CreateCell(subAvginfo[j].ColumnIndex + offset);
                            if (j == 0)
                            {
                                if (subAvginfo[j].ColumnIndex == 0)
                                {
                                    if (tempRowsCount != 0)
                                    {
                                        subTotal.SetCellValue("Sub Avg:" + Convert.ToDouble((subAvginfo[j].total / tempRowsCount).ToString("0.##")));
                                    }
                                    else
                                    {
                                        subTotal.SetCellValue("Sub Avg:0");
                                    }

                                }
                                else
                                {
                                    ICell subTotalTitle = borderTopRow.CreateCell((offset - 1) >= 0 ? (offset - 1) : 0);
                                    subTotalTitle.SetCellValue("Sub Avg");
                                    //subTotalTitle.CellStyle = csBold;
                                    //subTotal.CellStyle = csBold;
                                    if (tempRowsCount != 0)
                                    {
                                        subTotal.SetCellValue(Convert.ToDouble((subAvginfo[j].total / tempRowsCount).ToString("0.##")));
                                    }
                                    else
                                    {
                                        subTotal.SetCellValue("Sub Avg:0");
                                    }
                                    subTotalTitle.CellStyle = firstAggregate ? csBoldTopLine : csBoldOnly;
                                }
                            }
                            else
                            {
                                if (tempRowsCount != 0)
                                {
                                    subTotal.SetCellValue(Convert.ToDouble((subAvginfo[j].total / tempRowsCount).ToString("0.##")));
                                }
                                else
                                {
                                    subTotal.SetCellValue("Sub Avg:0");
                                }
                            }
                            //v1.2.0 Kim 2016.10.25 add column settings
                            ////v1.0.0 - Cheong - 2016/03/21 - Align aggreate fields of columns
                            //if ((dt.Columns[subAvginfo[j].ColumnIndex].DataType == typeof(Double)) || (dt.Columns[subAvginfo[j].ColumnIndex].DataType == typeof(Decimal)) || (dt.Columns[subAvginfo[j].ColumnIndex].DataType == typeof(Int32)))
                            //{
                            //    subTotal.CellStyle = subtotalcellRightStyle;
                            //}
                            //else
                            //{
                            //    subTotal.CellStyle = subtotalcellLeftStyle;
                            //}
                            String columnName = dt.Columns[subAvginfo[j].ColumnIndex].ColumnName.Trim();
                            subTotal.CellStyle = firstAggregate ? columnStyles[columnName].csSubTol : columnStyles[columnName].csSubTol2;
                            //subTotal.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                            subAvginfo[j].total = 0;
                            //subTotal.CellStyle = csBoldBtmLine;
                        }
                        rowIndex++;
                        borderTopRow = sheet.CreateRow(rowIndex);
                    }
                    //if (SubTotal && ((Subsum_Index.Length > 0) || (Subcount_Index.Length > 0) || (Subavg_Index.Length > 0))) rowIndex--;
                    #endregion

                    #endregion
                }
                //}
                #region TBD - old code
                /*
                else if (showType == "1")
                {
                    bool hidden = false;
                    if (groupIndex.Length > 0 && i > 0)
                    {
                        int sameCount = 0;
                        for (int g_i = 0; g_i < groupIndex.Count(); g_i++)
                        {
                            if (dt.Rows[i][groupIndex[g_i]].ToString() == dt.Rows[i - 1][groupIndex[g_i]].ToString())
                            {
                                sameCount++;
                            }
                        }
                        if (sameCount == groupIndex.Length)
                        {
                            hidden = true;
                        }
                    }

                    for (int j = 0; j < columnsCount; j++)
                    {
                        ICell cell = row.CreateCell(j);
                        if (!Convert.IsDBNull(dt.Rows[i][j]))
                        {
                            //v1.0.0 - Cheong - 2016/03/18 - Still need to set the content, just change the font color to white
                            //v1.0.0 - Cheong - 2016/03/17 - In "Show changed data only" mode, if the first column value is different, show it.
                            //if (((i != 0) && (dt.Rows[i][j].Equals(dt.Rows[i - 1][j]))) || hidden)
                            if (((i != 0) && (dt.Rows[i][j].Equals(dt.Rows[i - 1][j]) && (dt.Rows[i][0].Equals(dt.Rows[i - 1][0])))) || hidden)
                            {
                                //cell.SetCellValue(String.Empty);
                                cell.CellStyle = whitecellStyle;
                            }

                            {
                                //v1.0.0 - Cheong - 2016/03/15 - Add datatype handling to "Show changed data only"
                                //cell.SetCellValue(dt.Rows[i][j].ToString());
                                if (dt.Columns[j].DataType == typeof(DateTime))
                                {
                                    string l_strDateTime = ((DateTime)dt.Rows[i][j]).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                    l_strDateTime = l_strDateTime.Replace("1900-01-01", "").Trim();
                                    l_strDateTime = l_strDateTime.Replace("00:00:00", "").Trim();

                                    cell.SetCellValue(l_strDateTime);
                                    //row.CreateCell(j).SetCellValue(((DateTime)dt.Rows[i][j]).ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture));
                                }
                                else if ((dt.Columns[j].DataType == typeof(Double)) || (dt.Columns[j].DataType == typeof(Decimal)) || (dt.Columns[j].DataType == typeof(Int32)))
                                {
                                    cell.SetCellValue(Convert.ToDouble(dt.Rows[i][j]));
                                }
                                else
                                {
                                    cell.SetCellValue(dt.Rows[i][j].ToString());
                                }
                            }
                        }
                        else
                        {
                            cell.SetCellValue(String.Empty);
                        }
                    }
                }
                */
                #endregion TBD - old code
            }

            // v1.8.8 Alex 2018.10.09 - Subtotal, count, avg for the last subgroup - Begin

            rowIndex++;
            for (int x = off; x >= 1; x--)
            {
                #region sub total

                if (Subsum_Index.Length > 0)
                {
                    //rowIndex++;
                    IRow subTotalrow = sheet.CreateRow(rowIndex);

                    for (int j = 0; j < subTotalInfo.Length; j++)
                    {
                        int subtotalColumnIndex = subTotalInfo[j].ColumnIndex;
                        ICell subTotal = subTotalrow.CreateCell(subtotalColumnIndex + offset);
                        if (j == 0)
                        {
                            if (subtotalColumnIndex + offset == 0)
                            {
                                subTotal.SetCellValue((sums[x - 1, j]).ToString());
                            }
                            else
                            {
                                ICell subTotalTitle = subTotalrow.CreateCell((offset - 1) >= 0 ? (offset - 1) : 0);
                                //-->subTotalTitle.SetCellValue("Total");
                                //subTotalTitle.CellStyle = csBold;
                                //subTotalTitle.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                                //subTotal.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                                if (dt.Columns[j].DataType == typeof(Int32))
                                {
                                    subTotal.SetCellValue(Convert.ToInt32(sums[x - 1, j]));
                                }
                                else
                                {
                                    subTotal.SetCellValue(Convert.ToDouble(sums[x - 1, j]));
                                }
                                subTotalTitle.CellStyle = csBoldOnly;
                            }
                        }
                        else
                        {
                            if (dt.Columns[j].DataType == typeof(Int32))
                            {
                                subTotal.SetCellValue(Convert.ToInt32(sums[x - 1, j]));
                            }
                            else
                            {
                                subTotal.SetCellValue(Convert.ToDouble(sums[x - 1, j]));
                            }
                        }
                        String columnName = dt.Columns[subtotalColumnIndex].ColumnName.Trim();
                        subTotal.CellStyle = columnStyles[columnName].csSubTol2;
                        subTotalInfo[j].total = 0;
                        //subTotal.CellStyle = csBoldBtmLine;
                    }
                }

                #endregion

                #region sub count
                if (Subcount_Index.Length > 0)
                {
                    rowIndex++;
                    IRow subCountrow = sheet.CreateRow(rowIndex);

                    for (int j = 0; j < Subcount_Index.Length; j++)
                    {
                        ICell subCount = subCountrow.CreateCell(Subcount_Index[j] + offset);
                        if (j == 0)//first column.
                        {
                            if (Subcount_Index[j] == 0)
                            {
                                subCount.SetCellValue(sumCnt[x - 1]);

                            }
                            else
                            {
                                ICell subCountTitle = subCountrow.CreateCell((offset - 1) >= 0 ? (offset - 1) : 0);
                                //-->subCountTitle.SetCellValue("Count");
                                //subCountTitle.CellStyle = csBold;
                                //subCount.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                                subCount.SetCellValue(sumCnt[x - 1]);
                                subCountTitle.CellStyle = csBoldOnly;
                            }
                        }
                        else
                        {
                            subCount.SetCellValue(tempRowsCount);
                        }
                        String columnName = dt.Columns[Subcount_Index[j]].ColumnName.Trim();
                        subCount.CellStyle = columnStyles[columnName].csSubCnt2;
                        //subCount.CellStyle = csBoldBtmLine;
                    }
                }
                #endregion

                #region sub avg

                if (Subavg_Index.Length > 0)
                {
                    rowIndex++;
                    IRow subAvgrow = sheet.CreateRow(rowIndex);

                    for (int j = 0; j < Subavg_Index.Length; j++)
                    {
                        ICell subTotal = subAvgrow.CreateCell(subAvginfo[j].ColumnIndex + offset);
                        if (j == 0)
                        {
                            if (subAvginfo[j].ColumnIndex == 0)
                            {
                                if (tempRowsCount != 0)
                                {
                                    subTotal.SetCellValue(Convert.ToDouble((avgCnt[x - 1, j] == 0) ? "0" : (avgs[x - 1, j] / avgCnt[x - 1, j]).ToString("0.##")));
                                    //subTotal.SetCellValue("Avg:" + Convert.ToDouble((avgs[x, j] / avgCnt[x, j]).ToString("0.##")));
                                }
                                else
                                {
                                    subTotal.SetCellValue("0");
                                }

                            }
                            else
                            {
                                ICell subTotalTitle = subAvgrow.CreateCell((offset - 1) >= 0 ? (offset - 1) : 0);
                                //-->subTotalTitle.SetCellValue("Avg");
                                //subTotalTitle.CellStyle = csBold;
                                //subTotal.CellStyle = csBold;
                                if (tempRowsCount != 0)
                                {
                                    subTotal.SetCellValue(Convert.ToDouble((avgCnt[x - 1, j] == 0) ? "0" : (avgs[x - 1, j] / avgCnt[x - 1, j]).ToString("0.##")));
                                }
                                else
                                {
                                    subTotal.SetCellValue("0");
                                }
                                subTotalTitle.CellStyle = csBoldOnly;
                            }
                        }
                        else
                        {
                            if (tempRowsCount != 0)
                            {
                                subTotal.SetCellValue(Convert.ToDouble((avgCnt[x - 1, j] == 0) ? "0" : (avgs[x - 1, j] / avgCnt[x - 1, j]).ToString("0.##")));
                            }
                            else
                            {
                                subTotal.SetCellValue("0");
                            }
                        }

                        String columnName = dt.Columns[subAvginfo[j].ColumnIndex].ColumnName.Trim();
                        subTotal.CellStyle = columnStyles[columnName].csSubTol2;
                        //subTotal.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                        subAvginfo[j].total = 0;
                        //subTotal.CellStyle = csBoldBtmLine;
                    }
                }
                #endregion

                rowIndex++;
            }

            // v1.8.8 Alex 2018.10.09 - Subtotal, count, avg for the last subgroup - End

            #region rp sum,avg,count

            IList<decimal> sum_result = new List<decimal>(sum_Index.Length);
            IList<decimal> avg_result = new List<decimal>(avg_Index.Length);

            for (int j = 0; j < sum_Index.Length; j++)
            {
                sum_result.Add(0);
            }

            for (int j = 0; j < avg_Index.Length; j++)
            {
                avg_result.Add(0);
            }

            //v1.8.8 Alex 2018.11.05 - Ignore values that cannot be parsed - Begin
            decimal parse;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < sum_Index.Length; j++)
                {
                    sum_result[j] += !Convert.IsDBNull(dt.Rows[i][sum_Index[j]]) ? (Decimal.TryParse(dt.Rows[i][sum_Index[j]].ToString(), out parse) ? parse : 0M) : 0M;
                }

                for (int j = 0; j < avg_Index.Length; j++)
                {
                    avg_result[j] += !Convert.IsDBNull(dt.Rows[i][avg_Index[j]]) ? (Decimal.TryParse(dt.Rows[i][sum_Index[j]].ToString(), out parse) ? parse : 0M) : 0M;
                }
            }
            //v1.8.8 Alex 2018.11.05 - Ignore values that cannot be parsed - End
            rowIndex ++;

            #region Total

            if (sum_Index.Length > 0)
            {
                rowIndex++;

                IRow sumrow = sheet.CreateRow(rowIndex);

                for (int i = 0; i < sum_Index.Length; i++)
                {
                    XSSFCell sumCell = (XSSFCell)sumrow.CreateCell(sum_Index[i] + offset);
                    if (i == 0)
                    {
                        if (sum_Index[i] == 0)                   //first column or non-grouped
                        {
                            sumCell.SetCellValue("Total:" + sum_result[i].ToString());

                        }
                        else
                        {
                            XSSFCell sumCelltitle = (XSSFCell)sumrow.CreateCell((offset - 1) >= 0 ? (offset - 1) : 0); //sum_Index[i] + offset - 1
                            sumCelltitle.SetCellValue("Total");
                            sumCelltitle.CellStyle = csBold;
                            sumCell.SetCellValue(Convert.ToDouble(sum_result[i]));
                        }
                    }
                    else
                    {
                        sumCell.SetCellValue(Convert.ToDouble(sum_result[i]));
                    }
                    String columnName = dt.Columns[sum_Index[i]].ColumnName.Trim();
                    sumCell.CellStyle = columnStyles[columnName].csTol;

                }
            }

            #endregion Total

            #region Count

            if (count_Index.Length > 0)
            {
                rowIndex++;

                IRow Countrow = sheet.CreateRow(rowIndex);

                for (int i = 0; i < count_Index.Length; i++)
                {
                    XSSFCell countCell = (XSSFCell)Countrow.CreateCell(count_Index[i] + offset);
                    if (i == 0)//first column
                    {
                        if (count_Index[i] == 0) //first column 's position is 0
                        {
                            countCell.SetCellValue("Count:" + dt.Rows.Count);

                        }
                        else
                        {
                            XSSFCell CountCelltitle = (XSSFCell)Countrow.CreateCell((offset - 1) >= 0 ? (offset - 1) : 0);  // count_Index[i] + offset - 1
                            CountCelltitle.SetCellValue("Count");
                            CountCelltitle.CellStyle = csBold;

                            countCell.SetCellValue(dt.Rows.Count);
                        }
                    }
                    else
                    {
                        countCell.SetCellValue(dt.Rows.Count);

                    }
                    //v1.2.0 Kim 2016.10.25 add column settings
                    ////v1.0.0 - Cheong - 2016/03/21 - Align aggreate fields of columns
                    //if ((dt.Columns[count_Index[i]].DataType == typeof(Double)) || (dt.Columns[count_Index[i]].DataType == typeof(Decimal)) || (dt.Columns[count_Index[i]].DataType == typeof(Int32)))
                    //{
                    //    countCell.CellStyle = reporttotalRightstyle;
                    //}
                    //else
                    //{
                    //    countCell.CellStyle = reporttotalLeftstyle;
                    //}
                    String columnName = dt.Columns[count_Index[i]].ColumnName.Trim();
                    countCell.CellStyle = columnStyles[columnName].csCnt;
                }
            }

            #endregion Count

            #region Avg

            if (avg_Index.Length > 0)
            {
                rowIndex++;

                IRow avgrow = sheet.CreateRow(rowIndex);

                for (int i = 0; i < avg_Index.Length; i++)
                {
                    XSSFCell avgCell = (XSSFCell)avgrow.CreateCell(avg_Index[i] + offset);
                    if (i == 0)
                    {
                        if (avg_Index[i] == 0)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                avgCell.SetCellValue("Avg:" + Convert.ToDouble((avg_result[i] / dt.Rows.Count).ToString("0.##")));
                            }
                        }
                        else
                        {
                            XSSFCell avgCellTitle = (XSSFCell)avgrow.CreateCell((offset - 1) >= 0 ? (offset - 1) : 0); // avg_Index[i] + offset - 1
                            avgCellTitle.SetCellValue("Avg");
                            avgCellTitle.CellStyle = csBold;

                            if (dt.Rows.Count > 0)
                            {
                                avgCell.SetCellValue(Convert.ToDouble((avg_result[i] / dt.Rows.Count).ToString("0.##")));
                            }
                        }
                    }
                    else
                    {
                        if (dt.Rows.Count > 0)
                        {
                            avgCell.SetCellValue(Convert.ToDouble((avg_result[i] / dt.Rows.Count).ToString("0.##")));
                        }
                    }
                    //v1.2.0 Kim 2016.10.25 add column settings
                    ////v1.0.0 - Cheong - 2016/03/21 - Align aggreate fields of columns
                    //if ((dt.Columns[avg_Index[i]].DataType == typeof(Double)) || (dt.Columns[avg_Index[i]].DataType == typeof(Decimal)) || (dt.Columns[avg_Index[i]].DataType == typeof(Int32)))
                    //{
                    //    avgCell.CellStyle = reporttotalRightstyle;
                    //}
                    //else
                    //{
                    //    avgCell.CellStyle = reporttotalLeftstyle;
                    //}
                    String columnName = dt.Columns[avg_Index[i]].ColumnName.Trim();
                    avgCell.CellStyle = columnStyles[columnName].csTol;
                }
            }

            #endregion Avg

            #endregion rp sum,avg,count

            #region Report footer

            if (reportFooter != null)
            {
                rowIndex++; // skip one more row

                IRow rowFooter = null;
                ICell cell = null;
                for (int i = 0; i < reportFooter.Count; i++)
                {
                    rowFooter = sheet.CreateRow(rowIndex + i);
                    for (int j = 0; j < reportFooter[i].Split('|').Count(); j++)
                    {
                        cell = rowFooter.CreateCell(j);
                        cell.SetCellValue(reportFooter[i].Split('|')[j]);
                        cell.CellStyle = csDefault;
                    };
                }
                rowIndex += reportFooter.Count;
            }

            #endregion Report footer

            #region Set column widths
            //v1.2.0 Kim 2016.10.26 charWidth increase as column setting have 'bold' text
            //var charWidth = 256;
            var charWidth = 290;

            if (columnWidths == null)
            {
                //v1.6.3 Fai 2015.04.02 - Performance Tuning for AutoSizeColumn - Begin
                //列宽自适应，只对英文和数字有效
                //for (int i = 0; i <= columnsCount; i++)
                //{
                //    sheet.AutoSizeColumn(i, false);
                //}
                for (int i = 0; i < columnsCount; i++)
                {
                    //
                    int l_intMaxLength = dt.Columns[i].ColumnName.Length;
                    //dt.Rows.OfType<DataRow>().ToList()
                    //    .ForEach(p => 
                    //        { 
                    //            l_intMaxLength = 
                    //                Convert.ToString(p.ItemArray[i]).Length > l_intMaxLength ? 
                    //                    Convert.ToString(p.ItemArray[i]).Length : 
                    //                    l_intMaxLength; 
                    //        });

                    dt.Rows.OfType<DataRow>().ToList()
                        .ForEach(p =>
                            {
                                l_intMaxLength =
                                    Convert.ToString(
                                            p.ItemArray[i].GetType() == typeof(DateTime) && ((DateTime)p.ItemArray[i]).ToString("HH:mm:ss") == "00:00:00" ?
                                            ((DateTime)p.ItemArray[i]).ToString("yyyy-MM-dd") :
                                            Convert.ToString(p.ItemArray[i])).Length > l_intMaxLength ?
                                        Convert.ToString(
                                            p.ItemArray[i].GetType() == typeof(DateTime) && ((DateTime)p.ItemArray[i]).ToString("HH:mm:ss") == "00:00:00" ?
                                            ((DateTime)p.ItemArray[i]).ToString("yyyy-MM-dd") :
                                            Convert.ToString(p.ItemArray[i])).Length :
                                        l_intMaxLength;
                            });

                    //v1.0.0 - Cheong - 2015/12/30 - Checked this is a bug in NPOI.
                    //http://apache-poi.1045710.n5.nabble.com/autoSizeColumn-setColumnWidth-passed-255-td4759807.html
                    //Need to update the NPOI version from 2.1.3.1 to 3.8rc4+ to fix it.
                    //So for now will try to fix it with workaround
                    //sheet.SetColumnWidth(i, (int)(l_intMaxLength * 1.14388) * 256);
                    //sheet.SetColumnWidth(i, (int)(l_intMaxLength * 1.2) * 256);

                    if (l_intMaxLength < (255 - TrailingBlank))
                    {
                        sheet.SetColumnWidth(i + offset, (int)((l_intMaxLength + TrailingBlank) * charWidth));
                    }
                    else
                    {
                        sheet.SetColumnWidth(i + offset, 65280);
                    }
                }
                //v1.6.3 Fai 2015.04.02 - Performance Tuning for AutoSizeColumn - End
            }
            else
            {
                // v1.8.8 Alex 2018.10.22 - Add column width for empty columns when grouped

                ////////////////
                if ((indentWidths != null) && (indentWidths.Count() > 0))
                {
                    for (int i = 0; i < offset - 1; i++)
                    {
                        if (indentWidths[i] != -1)
                        {
                            sheet.SetColumnWidth(i, (int)(indentWidths[i] * charWidth));
                        }
                        else
                        {
                            sheet.SetColumnWidth(i, (int)(0.1 * charWidth)); //2
                        }
                    }
                    if (offset - 1 >= 0)
                    {
                        if (indentWidths[indentWidths.Count - 1] != -1)
                        {
                            sheet.SetColumnWidth(offset - 1, (int)(indentWidths[offset - 1] * charWidth));
                        }
                        else {
                            if ((subtotal.Count() > 0) || (subavg.Count() > 0) || (SubCount.Count() > 0))
                            {
                                sheet.SetColumnWidth(offset - 1, (int)(0.1 * charWidth));    //9
                            }
                            else
                            {
                                sheet.SetColumnWidth(offset - 1, (int)(0.1 * charWidth));    //2
                            }
                        }
                    }

                    //for (int i = 0; i < offset - 1; i++)
                    //{
                    //    sheet.SetColumnWidth(i, 2 * charWidth);
                    //}
                    //if (offset - 1 >= 0)
                    //{
                    //    if ((subtotal.Count() > 0) || (subavg.Count() > 0) || (SubCount.Count() > 0))
                    //    {
                    //        sheet.SetColumnWidth(offset - 1, 5 * charWidth);
                    //    }
                    //    else
                    //    {
                    //        sheet.SetColumnWidth(offset - 1, 2 * charWidth);
                    //    }
                    //}
                }
                //v1.0.0 - Cheong - 2016/01/21 - Add code for column width setup - Begin
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (columnWidths[dt.Columns[i].ColumnName.Trim()] > -1)
                    {
                        //v1.0.0 - Cheong - 2016/03/18 - If column width set to 0, set it to 1 in Excel to hide the column
                        if (columnWidths[dt.Columns[i].ColumnName.Trim()] == 0)
                        {
                            if (i == 0)
                            {
                                sheet.SetColumnWidth(i + offset, 50); // Should not be 0 as the Company title cannot be displayed
                            }
                            else
                            {
                                sheet.SetColumnWidth(i + offset, 0);
                            }
                        }
                        else if (columnWidths[dt.Columns[i].ColumnName.Trim()] < (255 - TrailingBlank))
                        {
                            sheet.SetColumnWidth(i + offset, (int)Math.Truncate((columnWidths[dt.Columns[i].ColumnName.Trim()] + TrailingBlank) * charWidth));
                        }
                        else
                        {
                            //sheet.SetColumnWidth(i, 255 * charWidth);
                            sheet.SetColumnWidth(i + offset, 65280);
                        }
                    }
                    else
                    {
                        // for columns not set excel_width, use default handling
                        int l_intMaxLength = dt.Columns[i].ColumnName.Length;

                        dt.Rows.OfType<DataRow>().ToList().ForEach(p =>
                            {
                                l_intMaxLength =
                                    Convert.ToString(
                                            p.ItemArray[i].GetType() == typeof(DateTime) && ((DateTime)p.ItemArray[i]).ToString("HH:mm:ss") == "00:00:00" ?
                                            ((DateTime)p.ItemArray[i]).ToString("yyyy-MM-dd") :
                                            Convert.ToString(p.ItemArray[i])).Length > l_intMaxLength ?
                                        Convert.ToString(
                                            p.ItemArray[i].GetType() == typeof(DateTime) && ((DateTime)p.ItemArray[i]).ToString("HH:mm:ss") == "00:00:00" ?
                                            ((DateTime)p.ItemArray[i]).ToString("yyyy-MM-dd") :
                                            Convert.ToString(p.ItemArray[i])).Length :
                                        l_intMaxLength;
                            });

                        //v1.0.0 - Cheong - 2015/12/30 - Checked this is a bug in NPOI.
                        //http://apache-poi.1045710.n5.nabble.com/autoSizeColumn-setColumnWidth-passed-255-td4759807.html
                        //Need to update the NPOI version from 2.1.3.1 to 3.8rc4+ to fix it.
                        //So for now will try to fix it with workaround
                        //sheet.SetColumnWidth(i, (int)(l_intMaxLength * 1.14388) * 256);
                        //sheet.SetColumnWidth(i, (int)(l_intMaxLength * 1.2) * 256);

                        //v1.8.4 Ben 2018.04.18 - It's max 65280 and now l_intMaxLength is 230, TrailingBlank is 2, charWidth is 290 which is already larger than 65280 so check 
                        //if (l_intMaxLength < (255 - TrailingBlank))
                        if ((l_intMaxLength + TrailingBlank) * charWidth <= 65280)
                        {
                            sheet.SetColumnWidth(i + offset, (int)((l_intMaxLength + TrailingBlank) * charWidth));
                        }
                        else
                        {
                            //sheet.SetColumnWidth(i, 255 * charWidth);
                            sheet.SetColumnWidth(i + offset, 65280);
                        }
                    }
                }
                //v1.0.0 - Cheong - 2016/01/21 - Add code for column width setup - End
            }

            #endregion Set column widths

            for (int x = 0; x < groupIndex.Count(); x++)
            {
                sheet.SetColumnWidth(offset + groupIndex[x], (int) (0.2 * charWidth));
            }

            //v1.0.0 - Cheong - 2016/01/21 - Add code for page setup - Begin
            switch (print_orientation)
            {
                case CUSTOMRP.Model.REPORT.Orientation.Portrait:
                    {
                        sheet.PrintSetup.Landscape = false;
                    }
                    break;
                case CUSTOMRP.Model.REPORT.Orientation.Landscape:
                    {
                        sheet.PrintSetup.Landscape = true;
                    }
                    break;
            }

            if (print_fitToPage > 0)
            {
                sheet.FitToPage = true;
                sheet.PrintSetup.FitWidth = print_fitToPage;
                sheet.PrintSetup.FitHeight = 0;
            }
            //v1.0.0 - Cheong - 2016/01/21 - Add code for page setup - End

            //v1.0.0 - Cheong - 2016/02/25 - Add repeating column header
            XSSFName PrintTitle = (XSSFName)XSSFworkbook.CreateName();
            PrintTitle.NameName = "Print_Titles";
            PrintTitle.RefersToFormula = String.Format("Report!${0}:${0}", CriterialColumnsIndex + 1);
            PrintTitle.SheetIndex = 0;

            //v1.0.0 - Cheong - 2016/02/25 - Test page footer
            sheet.OddFooter.Left = String.Format("{0} P.&P/&N", rptTitle);

            return XSSFworkbook;
        }

        private static XSSFWorkbook InitializeWorkbook(string author, string subject)
        {
            XSSFWorkbook XSSFworkbook = new XSSFWorkbook();

            //create a entry of DocumentSummaryInformation
            NPOI.POIXMLProperties xmlProps = XSSFworkbook.GetProperties();
            NPOI.CoreProperties coreProps = xmlProps.CoreProperties;

            coreProps.Creator = author;
            coreProps.Subject = subject;

            return XSSFworkbook;
        }

        public static XSSFWorkbook GetWorkbookAsMailMergeDataSource(System.Data.DataTable dt)
        {
            XSSFWorkbook XSSFworkbook = null;
            XSSFworkbook = InitializeWorkbook("DW Query Report", "MailMerge DataSource");

            XSSFSheet sheet1 = (XSSFSheet)XSSFworkbook.CreateSheet("Sheet1");
            int columnsCount = dt.Columns.Count;

            #region caption row
            IRow row = sheet1.CreateRow(0);
            ICell cell = null;
            for (int m = 0; m < columnsCount; m++)
            {
                DataColumn dc = dt.Columns[m];
                cell = (XSSFCell)row.CreateCell(m);
                cell.SetCellValue(MailMerge.GetSafeFieldName(dc.ColumnName));
            }
            #endregion

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < columnsCount; j++)
                {
                    cell = (XSSFCell)row.CreateCell(j);
                    cell.SetCellValue(AppHelper.FormatData(dt.Rows[i][j]));
                }
            }

            for (int i = 0; i <= columnsCount; i++)
            {
                sheet1.AutoSizeColumn(i);
            }

            return XSSFworkbook;
        }

        public static Dictionary<string, string> GetHorizontalTextAligns()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (HorizontalAlignment eHorAlign in Enum.GetValues(typeof(HorizontalAlignment)))
            {
                dict.Add(((int)eHorAlign).ToString(), eHorAlign.ToString());
            }
            return dict;
        }

    }
}