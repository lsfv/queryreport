using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace Common
{
    public class IncOpenExcel:IDisposable
    {
        public readonly static string error_filetype = "it is not .xlsx";
        public readonly static string error_empty = "it is null or empty";

        private SpreadsheetDocument document = null;
        private DefaultCellStyle defaultCellStyle = null;

        public bool IsRight()
        {
            return document == null ? false : true;
        }
        #region public
        #region file
        public IncOpenExcel(string newFilePath, string sheetName)
        {
            if (!string.IsNullOrWhiteSpace(newFilePath))
            {
                if (Path.GetExtension(newFilePath) == ".xlsx")
                {
                    if (File.Exists(newFilePath))
                    {
                        File.Delete(newFilePath);
                        document = CreateFile(newFilePath, sheetName, out defaultCellStyle);
                    }
                    else
                    {
                        document = CreateFile(newFilePath, sheetName, out defaultCellStyle);
                    }
                }
                else
                {
                    throw new Exception(error_filetype);
                }
            }
            else
            {
                throw new Exception(error_empty);
            }
        }
        #endregion

        #region row
        public bool CreateOrUpdateRowsAt(string sheetName, DataTable dataTable, uint rowNo, uint columnNo, Dictionary<int, Dictionary<int, uint>> rowsEachColumnStyle = null)
        {
            bool res = false;
            Worksheet worksheet = getWorksheet(sheetName);
            SheetData sheetData = worksheet == null ? null : worksheet.Elements<SheetData>().First();
            if (sheetData != null)
            {
                if (dataTable != null)
                {
                    int failcount = 0;
                    for(int i=0;i<dataTable.Rows.Count;i++)
                    {
                        bool tempres = false;
                        if (rowsEachColumnStyle != null && rowsEachColumnStyle.Keys.Contains(i))
                        {
                            tempres = IncOpenExcel.CreateOrUpdateRowAt(sheetData, dataTable.Rows[i], rowNo+(uint)i, columnNo, defaultCellStyle, rowsEachColumnStyle[i]);
                        }
                        else
                        {
                            tempres = IncOpenExcel.CreateOrUpdateRowAt(sheetData, dataTable.Rows[i], rowNo+ (uint)i, columnNo, defaultCellStyle,null);
                        }
                        if (!tempres) { failcount++; }
                    }
                    if (failcount == 0) { res = true; }
                }
            }
            return res;
        }

        public bool CreateOrUpdateRowAt(string sheetName, DataRow dataRow, uint rowNo, uint columnNo, Dictionary<int, uint> eachColumnStyle = null)
        {
            bool res = false;
            Worksheet worksheet = getWorksheet(sheetName);
            SheetData sheetData = worksheet == null?null: worksheet.Elements<SheetData>().First();
            if (sheetData != null)
            {
                res=IncOpenExcel.CreateOrUpdateRowAt(sheetData, dataRow, rowNo, columnNo, defaultCellStyle, eachColumnStyle);
            }
            return res;
        }
        #endregion

        #region cell
        #endregion

        #region other
        public Worksheet getWorksheet(string sheetname)
        {
            Worksheet res = null;
            IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == sheetname);
            if (sheets.Count() == 0)
            {
                res = null;
            }
            else
            {
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(relationshipId);
                res = worksheetPart.Worksheet;
            }
            return res;
        }
        public SheetData getSheetDate(Worksheet worksheet)
        {
            SheetData res = null;
            if (worksheet != null)
            {
                var sds = worksheet.Elements<SheetData>();
                if (sds != null && sds.Count() > 0)
                {
                    res = sds.First();
                }
            }
            return res;
        }

        public void SaveAndClose()
        {
            if (document != null)
            {
                document.Close();
            }
        }
        public void Dispose()
        {
            if (document != null)
            {
                document.Close();
            }
        }
        #endregion
        #endregion 


        #region private
        #region file
        private static SpreadsheetDocument CreateFile(string newFilePath, string firstSheetName, out DefaultCellStyle defaultCellStyle)
        {
            SpreadsheetDocument spreadsheetDocument = null;
            defaultCellStyle = null;
            try
            {
                //建立xlsx文件
                spreadsheetDocument = SpreadsheetDocument.Create(newFilePath, SpreadsheetDocumentType.Workbook, true);

                //建立xl,worksheets目录(会默认生成0字节的workbook和worksheet,以及2个res文档)
                WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();

                //建立workbook文档,设定模式的worksheet (sheet的3个属性必须填写,特别的是name是这里设定,有点不符合常见的抽象思维)
                Workbook workbook = new Workbook();
                Sheets sheets = new Sheets();
                Sheet sheet = new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = firstSheetName
                };
                sheets.Append(sheet);
                workbook.AppendChild<Sheets>(sheets);
                workbookpart.Workbook = workbook;

                //建立默认的worksheet文档(可以先workbook,后worksheet)
                SheetData sheetData = new SheetData();
                Worksheet worksheet = new Worksheet();
                worksheet.Append(sheetData);
                worksheetPart.Worksheet = worksheet;//给默认的worksheet赋值,否则0字节.

                //建立样式文件
                createDeafultStyle(workbookpart, out defaultCellStyle);
                spreadsheetDocument.Save();
            }
            catch (Exception e)
            {
                spreadsheetDocument = null;
                defaultCellStyle = null;
                throw e;
            }
            return spreadsheetDocument;
        }
        #endregion

        #region row
        private static bool CreateOrUpdateRowAt(SheetData sheetData, DataRow dataRow, uint rowNo, uint columnNo, DefaultCellStyle defaultCellStyle, Dictionary<int, uint> eachColumnStyle)
        {
            bool res = false;
            //create row and cell. append or insert.
            if (sheetData != null && rowNo > 0 && columnNo > 0 && defaultCellStyle != null)
            {
                Row newRow = CreateRow(rowNo,columnNo,dataRow,defaultCellStyle, eachColumnStyle);
                //是否存在row,1.存在,删除.2.现在是否存在大于row,存在,在前插入.不存在.直接附加.
                var rows = sheetData.Elements<Row>().Where(x => x.RowIndex == rowNo);
                if (rows.Count() > 0)
                {
                    sheetData.RemoveChild(rows.First());
                }

                var biggerrows = sheetData.Elements<Row>().Where(x => x.RowIndex > rowNo);
                if (biggerrows.Count() <= 0)
                {
                    sheetData.Append(newRow);//todo:装载数据7/10的时间耗费在这里。需要优化！如果是大数据插入，应该创建大量row，再使用一次append或其他插入函数。
                }
                else
                {
                    sheetData.InsertBefore(newRow, biggerrows.First());
                }
                res = true;
            }
            else
            {
                throw new Exception("create row argument error.");
            }
            return res;
        }

        //Dictionary<int, uint> 表格列的索引(from zero)和样式编号
        private static Row CreateRow(uint rowNo, uint columnNo, DataRow dataRow, DefaultCellStyle defaultCellStyle, Dictionary<int, uint> eachColumnStyle = null)
        {
            Row newRow = new Row();
            newRow.RowIndex = rowNo;

            if (dataRow != null)
            {
                for (uint i = 0; i < dataRow.Table.Columns.Count; i++)
                {
                    Cell newCell = null;
                    string cellref = GetCellReference(rowNo, columnNo + i);
                    Type cellType = dataRow.Table.Columns[(int)i].DataType;
                    object cellValue = dataRow[(int)i];
                    if (eachColumnStyle != null && eachColumnStyle.Keys.Contains((int)i))
                    {
                        newCell = createCell(cellref, cellType, cellValue, defaultCellStyle, eachColumnStyle[(int)i]);
                    }
                    else
                    {
                        newCell = createCell(cellref, cellType, cellValue, defaultCellStyle);
                    }
                    if (newCell != null)
                    {
                        newRow.Append(newCell);
                    }
                }
            }

            return newRow;
        }
        #endregion

        #region cell
        private static Cell createCell(string reference, Type type, object value, DefaultCellStyle defaultCellStyle, uint customStyle = 0)
        {
            Cell theCell = null;
            try
            {
                theCell = new Cell();
                theCell.CellReference = reference;

                CellValues theCellValue = GetValueType(type);
                theCell.DataType = new EnumValue<CellValues>(theCellValue);

                if (theCellValue == CellValues.Date)
                {
                    if (value.ToString() != "")
                    {
                        theCell.CellValue = new CellValue(((DateTime)value));
                    }
                    else
                    {
                        theCell.CellValue = new CellValue("");
                    }
                    if (customStyle == 0)
                    {
                        theCell.StyleIndex = defaultCellStyle.dateTimeIndex;
                    }
                    else
                    {
                        theCell.StyleIndex = customStyle;
                    }
                }
                else
                {
                    theCell.CellValue = new CellValue(value.ToString());
                    if (customStyle == 0)
                    {
                        theCell.StyleIndex = defaultCellStyle.normalIndex;
                    }
                    else
                    {
                        theCell.StyleIndex = customStyle;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return theCell;
        }
        #endregion

        #region other
        private static void SetExcelPivotTableCacheAutoReflesh(SpreadsheetDocument document)
        {
            try
            {
                WorkbookPart wbPart = document.WorkbookPart;
                var pivottableCashes = wbPart.PivotTableCacheDefinitionParts;
                foreach (PivotTableCacheDefinitionPart pivottablecachePart in pivottableCashes)
                {
                    pivottablecachePart.PivotCacheDefinition.RefreshOnLoad = true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string GetCellReference( UInt32 rowIndex, UInt32 colIndex)
        {
            UInt32 dividend = colIndex;
            string columnName = String.Empty;
            UInt32 modifier;

            while (dividend > 0)
            {
                modifier = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modifier).ToString() + columnName;
                dividend = (UInt32)((dividend - modifier) / 26);
            }

            return columnName + rowIndex.ToString();
        }
        private static CellValues GetValueType(Type type)
        {
            List<Type> number = new List<Type> { typeof(Double), typeof(Decimal), typeof(Int32), typeof(int), typeof(Int16), typeof(Int64) };
            List<Type> date = new List<Type> { typeof(DateTime) };

            if (number.Contains(type))
            {
                return CellValues.Number;
            }
            else if (date.Contains(type))
            {
                return CellValues.Date;
            }
            else
            {
                return CellValues.String;
            }
        }
        #endregion

        #region font ,cellformat
        //建立一个字体
        private static UInt32Value createFont(Stylesheet styleSheet, string fontName, Nullable<double> fontSize, bool isBold, System.Drawing.Color foreColor)
        {
            if (styleSheet.Fonts.Count == null)
            {
                styleSheet.Fonts.Count = (UInt32Value)0;
            }
            Font font = new Font();
            if (!string.IsNullOrEmpty(fontName))
            {
                FontName name = new FontName()
                {
                    Val = fontName
                };
                font.Append(name);
            }

            if (fontSize.HasValue)
            {
                FontSize size = new FontSize()
                {
                    Val = fontSize.Value
                };
                font.Append(size);
            }

            if (isBold == true)
            {
                Bold bold = new Bold();
                font.Append(bold);
            }

            if (foreColor != null)
            {
                Color color = new Color()
                {
                    Rgb = new HexBinaryValue()
                    {
                        Value =
                            System.Drawing.ColorTranslator.ToHtml(
                                System.Drawing.Color.FromArgb(
                                    foreColor.A,
                                    foreColor.R,
                                    foreColor.G,
                                    foreColor.B)).Replace("#", "")
                    }
                };
                font.Append(color);
            }
            styleSheet.Fonts.Append(font);
            UInt32Value result = styleSheet.Fonts.Count;
            styleSheet.Fonts.Count++;
            return result;
        }
        //创建一个单元格样式
        private static UInt32Value createCellFormat(Stylesheet styleSheet, UInt32Value borderid, UInt32Value fontIndex, UInt32Value fillIndex, UInt32Value numberFormatId)
        {
            if (styleSheet.CellFormats.Count == null)
            {
                styleSheet.CellFormats.Count = (UInt32Value)0;
            }
            CellFormat cellFormat = new CellFormat();
            cellFormat.BorderId = 0;
            if (fontIndex != null)
            {
                cellFormat.ApplyFont = true;
                cellFormat.FontId = fontIndex;
            }
            if (fillIndex != null)
            {
                cellFormat.FillId = fillIndex;
                cellFormat.ApplyFill = true;
            }
            if (numberFormatId != null)
            {
                cellFormat.NumberFormatId = numberFormatId;
                cellFormat.ApplyNumberFormat = true;
            }

            styleSheet.CellFormats.Append(cellFormat);
            UInt32Value result = styleSheet.CellFormats.Count;
            styleSheet.CellFormats.Count++;
            return result;
        }
        //建立一个最小样式表.
        private static void createDeafultStyle(WorkbookPart workbookpart, out DefaultCellStyle cellStyle)
        {

            cellStyle = new DefaultCellStyle(0, 0, 0);
            if (workbookpart != null)
            {
                //1.建立必要的文件和其根节点.
                if (workbookpart.WorkbookStylesPart == null)
                {
                    workbookpart.AddNewPart<WorkbookStylesPart>();
                }
                if (workbookpart.WorkbookStylesPart.Stylesheet == null)
                {
                    workbookpart.WorkbookStylesPart.Stylesheet = new Stylesheet();
                }
                if (workbookpart.WorkbookStylesPart.Stylesheet.Fonts == null)
                {
                    workbookpart.WorkbookStylesPart.Stylesheet.Fonts = new Fonts();
                }
                if (workbookpart.WorkbookStylesPart.Stylesheet.Fills == null)
                {
                    workbookpart.WorkbookStylesPart.Stylesheet.Fills = new Fills(new Fill(new PatternFill() { PatternType = PatternValues.None }), new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }));
                }
                if (workbookpart.WorkbookStylesPart.Stylesheet.Borders == null)
                {
                    workbookpart.WorkbookStylesPart.Stylesheet.Borders = new Borders();
                    workbookpart.WorkbookStylesPart.Stylesheet.Borders.Append(new Border(new RightBorder(), new TopBorder(), new BottomBorder(), new DiagonalBorder()));
                }
                if (workbookpart.WorkbookStylesPart.Stylesheet.CellFormats == null)
                {
                    workbookpart.WorkbookStylesPart.Stylesheet.CellFormats = new CellFormats();
                }
                if (workbookpart.WorkbookStylesPart.Stylesheet.NumberingFormats == null)
                {
                    workbookpart.WorkbookStylesPart.Stylesheet.NumberingFormats = new NumberingFormats();
                }

                //自定义字体
                UInt32 defaultFont = createFont(workbookpart.WorkbookStylesPart.Stylesheet, "Microsoft YaHei", (double)11, false, System.Drawing.Color.Black);
                UInt32 BoldFont = createFont(workbookpart.WorkbookStylesPart.Stylesheet, "Microsoft YaHei", (double)11, true, System.Drawing.Color.Black);
                //自定义数字格式,时间格式
                UInt32 dateDeafult = 240;
                var numberFormatDate_index = new NumberingFormat();
                numberFormatDate_index.FormatCode = new StringValue("yyyy-mm-dd");
                numberFormatDate_index.NumberFormatId = dateDeafult;//随意定义100~200之间.
                workbookpart.WorkbookStylesPart.Stylesheet.NumberingFormats.InsertAt(numberFormatDate_index, workbookpart.WorkbookStylesPart.Stylesheet.NumberingFormats.Count());
                //自定义最终给用户的单元格式.
                //todo:这里应该扩展机会会大.需要重构
                cellStyle.normalIndex = createCellFormat(workbookpart.WorkbookStylesPart.Stylesheet, null, defaultFont, null, null);
                cellStyle.blackIndex = createCellFormat(workbookpart.WorkbookStylesPart.Stylesheet, null, BoldFont, null, null);
                cellStyle.dateTimeIndex = createCellFormat(workbookpart.WorkbookStylesPart.Stylesheet, null, defaultFont, null, dateDeafult);
                workbookpart.WorkbookStylesPart.Stylesheet.Save();
            }
        }

        
        #endregion
        #endregion

        #region innerclass
        public class DefaultCellStyle
        {
            public UInt32 normalIndex { get; set; }
            public UInt32 dateTimeIndex { get; set; }
            public UInt32 blackIndex { get; set; }

            public DefaultCellStyle(UInt32 _normalIndex, UInt32 _dateTimeIndex, UInt32 _blackIndex)
            {
                normalIndex = _normalIndex;
                dateTimeIndex = _dateTimeIndex;
                blackIndex = _blackIndex;
            }
        }
        #endregion
    }
}