﻿using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;


/// <summary>
/// 其于OpenXml SDK写的帮助类
/// linson.20200226
/// </summary>

namespace Common
{
    public class MyExcelFile :IDisposable
    {
        private SpreadsheetDocument document = null;
        public DefaultCellStyle defaultCellStyle = new DefaultCellStyle(0,0,0);

        #region public function 为什么public 简单包一个静态方法?主要是因为想让实际方法设为static,减低耦合度,避免后期维护改的逻辑混乱.但又为了使用方便,用public 包一下, 非常好的一个策略.
        //open file and create
        public MyExcelFile(string filePath_load,out string errMsg)
        {
            errMsg = "";
            if (File.Exists(filePath_load) && Path.GetExtension(filePath_load)==".xlsx")
            {
                try
                {
                    document= openFile(filePath_load, out errMsg, out defaultCellStyle);
                }
                catch(Exception e)
                {
                    errMsg = e.Message;
                }
            }
            else
            {
                errMsg = "file is not exist,or extension is not xlsx.";
            }
        }
        public MyExcelFile(string filePath_create,bool overWirte,string defaultSheetName,out string myerrmsg)
        {
            myerrmsg = "";
            if (Path.GetExtension(filePath_create) != ".xlsx")
            {
                myerrmsg = "file extension is not xlsx.";
            }
            else
            {
                if(File.Exists(filePath_create) && overWirte==false)
                {
                    myerrmsg = "file is exist already";
                }
                else
                {
                    try
                    {
                        document = createXlsxExcelFile(filePath_create, defaultSheetName, out myerrmsg, out defaultCellStyle);
                    }
                    catch (Exception e)
                    {
                        myerrmsg = e.Message;
                    }
                }
            }
        }
        //new and update cell
        public bool SetOrUpdateCellValue(string sheetName, UInt32 rowNumber, UInt32 columnNumber, DataRow dataRow,UInt32 customStyle=0)
        {
            SheetData sheetData = getWorksheet( sheetName).Elements<SheetData>().First();
            return SetOrUpdateCellValue(sheetData, rowNumber, columnNumber, dataRow,defaultCellStyle,customStyle);
        }
        //new and update row
        public Row SetOrReplaceRow(string sheetName, UInt32 rowNumber, UInt32 columnNumber, DataRow dataRow)
        {
            SheetData sheetData = getWorksheet( sheetName).Elements<SheetData>().First();
            return SetOrReplaceRow(sheetData, rowNumber, columnNumber, dataRow, defaultCellStyle);
        }

        //new and uupdate rows with table
        public bool SetOrReplaceRows(string sheetName, UInt32 rowNumber, UInt32 columnNumber, DataTable dataTable)
        {
            bool res = false;
            if (dataTable != null)
            {
                uint writingGuard = rowNumber;
                //start to set cell value with column name.
                DataTable columnsTable = Common.MyExcelFile.GetColumnsNames(dataTable);
                SetOrReplaceRow(sheetName, writingGuard, columnNumber, columnsTable.Rows[0]);
                writingGuard++;
                //start to set cell value with datatable.
                if (dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        SetOrReplaceRow(sheetName, writingGuard, columnNumber, dataTable.Rows[i]);
                        writingGuard++;
                    }
                }
            }
            return res;
        }

        //remove row(just delete rows nodes)
        public bool RemoveRows(string sheetName, UInt32 startRowIndex, UInt32 endRowIndex)
        {
            bool res = false;
            IEnumerable<Row> rows = GetRangeRows(sheetName, startRowIndex, endRowIndex);
            if (rows != null)
            {
                foreach (Row r in rows.ToList())//删除不能用可碟带类型,因为需要当前来movenext.先换成集合把.
                {
                    r.Remove();
                }
                res = true;
            }
            return res;
        }
        public bool RemoveAllRows(string sheetName)
        {
            bool res = false;
            Worksheet worksheet = getWorksheet(sheetName);
            if (worksheet != null)
            {
                worksheet.Elements<SheetData>().First().RemoveAllChildren();
            }
            return res;
        }

        //search cell and row and worksheet
        public Cell GetCell(string sheetName, UInt32 rowNumber, UInt32 columnNumber)
        {
            SheetData sheetData = getWorksheet(sheetName).Elements<SheetData>().First();
            return GetACell(sheetData, rowNumber, columnNumber);
        }
        public string GetCellRealString(string sheetName, UInt32 rowNumber, UInt32 columnNumber)
        {
            Cell abc = GetCell(sheetName, rowNumber, columnNumber);
            return GetCellRealString(abc);
        }
        public string GetCellRealString(Cell cell)
        {
            if (cell != null)
            {
                if (cell.DataType != null)
                {
                    CellValues cellType = cell.DataType;
                    if (cellType == CellValues.SharedString)
                    {
                        return getShareString(int.Parse(cell.CellValue.Text));
                    }
                    else
                    {
                        return cell.CellValue.Text;
                    }
                }
                else
                {
                    return cell.CellValue==null?"":cell.CellValue.Text;
                }
                
            }
            else
            {
                return null;
            }
        }
        public IEnumerable<Row> GetRangeRows(string sheetName, UInt32 startRowIndex, UInt32 endRowIndex)
        {
            IEnumerable<Row> res = null;
            if (endRowIndex >= startRowIndex)
            {
                Worksheet worksheet = getWorksheet(sheetName);
                if (worksheet != null)
                {
                    var sheetdates = worksheet.Elements<SheetData>();
                    if (sheetdates.Count() > 0)
                    {
                        var sheetdata = sheetdates.First();
                        var rows = sheetdata.Elements<Row>().Where(x => x.RowIndex >= startRowIndex && x.RowIndex <= endRowIndex);
                        res = rows;
                    }
                }
            }
            return res;
        }
        public uint GetRowIndexFromAColumn(string sheetname ,string str, uint column)
        {
            uint res = 0;
            Worksheet worksheet = getWorksheet(sheetname);
            SheetData sheetData = getSheetDate(worksheet);
            IEnumerable<Row> rows = sheetData.Elements<Row>();

            foreach (Row row in rows)
            {
                if (row.Elements<Cell>() != null && row.Elements<Cell>().Count() > 0)
                {
                    Debug.Print(row.RowIndex);
                    var Cells = row.Elements<Cell>().Where(x => x.CellReference.Value == GetCellReference(column, row.RowIndex));
                    if (Cells!=null && Cells.Count()>0 &&GetCellRealString(Cells.First()) == str)
                    {
                        res = row.RowIndex;
                    }
                }
            }
            return res;
        }

        //other funtion
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
        public bool isValid()
        {
            return document != null;
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
            if(document!=null)
            {
                document.Close();
            }
        }
        public string getShareString(int index)
        {
            return getShareString(document, index);
        }
        public bool UpdateAllPivotSource(string SourcesheetName, string firstReference, string lastReference)
        {
            return UpdateAllPivotSource(document, SourcesheetName, firstReference, lastReference);
        }
        public static bool UpdateAllPivotSource(SpreadsheetDocument document, string SourcesheetName, string firstReference, string lastReference)
        {
            bool res = false;
            try
            {
                var pivottableCashes = document.WorkbookPart.PivotTableCacheDefinitionParts;
                foreach (PivotTableCacheDefinitionPart pivottablecachePart in pivottableCashes)
                {
                    pivottablecachePart.PivotCacheDefinition.CacheSource.RemoveAllChildren();
                    pivottablecachePart.PivotCacheDefinition.CacheSource.Append(new WorksheetSource()
                    {
                        Sheet = SourcesheetName,
                        Reference = new StringValue(firstReference + ":" + lastReference)
                    });
                }
                res = true;
            }
            catch
            {
                res = false;
            }
            return res;
        }
        public static string GetCellReference(UInt32 colIndex, UInt32 rowIndex)
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
        public static Dictionary<string, UInt32Value> getRowStyles(Row theRow)
        {
            Dictionary<string, UInt32Value> styles = new Dictionary<string, UInt32Value>();
            if (theRow != null)
            {
                var cells = theRow.Elements<Cell>();
                foreach (Cell cell in cells)
                {
                    styles.Add(cell.CellReference, cell.StyleIndex);
                }
            }
            return styles;
        }

        #endregion

        #region private



        private static Row SetOrReplaceRow(SheetData sheetData, UInt32 startRow,UInt32 startColumn, DataRow dataRow,DefaultCellStyle defaultCellStyle)
        {
            Row newRow = CreateRow(startRow, startColumn, dataRow, defaultCellStyle);
            //是否存在row,1.存在,删除.2.现在是否存在大于row,存在,在前插入.不存在.直接附加.
            var rows = sheetData.Elements<Row>().Where(x => x.RowIndex == startRow);
            if (rows.Count() > 0)
            {
                sheetData.RemoveChild(rows.First());
            }

            var biggerrows = sheetData.Elements<Row>().Where(x => x.RowIndex > startRow);
            if (biggerrows.Count() <= 0)
            {
                sheetData.Append(newRow);//todo:装载数据7/10的时间耗费在这里。需要优化！如果是大数据插入，应该创建大量row，再使用一次append或其他插入函数。
            }
            else
            {
                sheetData.InsertBefore(newRow, biggerrows.First());
            }
            return newRow;
        }

        private static Row CreateRow(uint startRow, uint startColumn, DataRow dataRow, DefaultCellStyle defaultCellStyle,IList<uint> allStyles=null)
        {
            Row newRow = new Row();
            newRow.RowIndex = startRow;
            for (UInt32 i = 0; i < dataRow.Table.Columns.Count; i++)
            {
                Cell newCell = new Cell();
                newCell.CellReference = GetCellReference(startColumn + i, startRow);

                CellValues celltype = GetValueType(dataRow.Table.Columns[(int)i].DataType);

                newCell.DataType = new EnumValue<CellValues>(celltype);

                if (celltype == CellValues.Number)
                {
                    newCell.StyleIndex = defaultCellStyle.normalIndex;
                    newCell.CellValue = new CellValue(dataRow[(int)i].ToString());
                }
                else if (celltype == CellValues.Date)
                {
                    newCell.StyleIndex = defaultCellStyle.dateTimeIndex;
                    if (dataRow[(int)i].ToString() == "")
                    {
                        newCell.CellValue = new CellValue("");
                    }
                    else
                    {
                        newCell.CellValue = new CellValue(((DateTime)dataRow[(int)i]));
                    }
                }
                else
                {
                    newCell.StyleIndex = defaultCellStyle.normalIndex;
                    newCell.CellValue = new CellValue(dataRow[(int)i].ToString());
                }

                //if there is custom style,we need overwirte styleid
                if (allStyles != null && allStyles.Count - 1 >= i)
                {
                    newCell.StyleIndex = allStyles[(int)i];
                }
                newRow.Append(newCell);
            }

            return newRow;
        }

        private static bool SetOrUpdateCellValue(SheetData sheetData, UInt32 rowNumber, UInt32 columnNumber, DataRow dataRow, DefaultCellStyle defaultCellStyle, UInt32 customStyle = 0)
        {
            bool res = false;

            if ((dataRow.Table.Columns.Count == 1))
            {
                //1.是否存在Row,存在跟新,不存在建立新Row.2.是否有比这个Row更大的Row,有插入大Row之前.否则直接附加在sheetdata后面.
                string cellRefrence = GetCellReference(columnNumber, rowNumber);

                IEnumerable<Row> equalOrbiggerRows = sheetData.Elements<Row>().Where(x => x.RowIndex >= rowNumber);
                Row equalRow = null, biggerRow = null;
                if (equalOrbiggerRows != null && equalOrbiggerRows.Count() > 0 && equalOrbiggerRows.First().RowIndex == rowNumber)
                {
                    equalRow = equalOrbiggerRows.First();
                }
                else if (equalOrbiggerRows != null && equalOrbiggerRows.Count() > 0 && equalOrbiggerRows.First().RowIndex > rowNumber)
                {
                    biggerRow = equalOrbiggerRows.First();
                }

                if (equalRow != null)
                {
                    //1.是否存在cell,存在跟新,不存在建立新cell.2.是否有比这个cell更大的cell,有插入大cell之前.否则直接附加在Row后面.

                    IEnumerable<Cell> equalOrbiggerCells = equalRow.Elements<Cell>().Where(x => x.CellReference >= new StringValue(cellRefrence));

                    Cell equalCell = null;
                    Cell biggerCell = null;
                    if (equalOrbiggerCells != null && equalOrbiggerCells.Count() > 0 && equalOrbiggerCells.First().CellReference == cellRefrence)
                    {
                        equalCell = equalOrbiggerCells.First();
                    }
                    else if (equalOrbiggerCells != null && equalOrbiggerCells.Count() > 0 && equalOrbiggerCells.First().CellReference > new StringValue(cellRefrence))
                    {
                        biggerCell = equalOrbiggerCells.First();
                    }

                    Cell newCell = createCell(cellRefrence, dataRow.Table.Columns[0].DataType, dataRow[0], defaultCellStyle, out Exception tempExc, customStyle);
                    if (equalCell != null)
                    {
                        equalOrbiggerRows.First().ReplaceChild(newCell, equalCell);
                    }
                    else
                    {
                        if (biggerCell != null)
                        {
                            equalOrbiggerRows.First().InsertBefore(newCell, biggerCell);
                        }
                        else
                        {
                            equalOrbiggerRows.First().Append(newCell);
                        }
                    }
                    res = true;
                }
                else
                {
                    Row newrow = new Row();
                    newrow.RowIndex = rowNumber;

                    Cell theCell = createCell(cellRefrence, dataRow.Table.Columns[0].DataType, dataRow[0], defaultCellStyle, out Exception tempExc, customStyle);
                    if (theCell != null)
                    {
                        newrow.Append(theCell);
                    }

                    if (biggerRow != null)
                    {
                        sheetData.InsertBefore(newrow, equalOrbiggerRows.First());//插入的行不是最大的,插到比它大的前面.
                    }
                    else
                    {
                        sheetData.Append(newrow); ;//插入的行是最大的,直接附加到最后
                    }
                    res = true;
                }
            }
            return res;
        }
        private static Cell GetACell(SheetData sheetData, UInt32 rowNumber, UInt32 columnNumber)
        {
            Cell res = null;
            var rows = sheetData.Elements<Row>().Where(x => x.RowIndex == rowNumber);
            if (rows != null)
            {
                var row = rows.First();
                var cells = row.Elements<Cell>().Where(x => x.CellReference == GetCellReference(columnNumber, rowNumber));
                if (cells != null && cells.Count()>0)
                {
                    res = cells.First();
                }
            }
            return res;
        }
        private static SpreadsheetDocument openFile(string filepath,out string errMsg,out DefaultCellStyle defaultCellStyle)
        {
            SpreadsheetDocument res = null;
            defaultCellStyle = null;
            errMsg = "";
            try
            {
                res = SpreadsheetDocument.Open(filepath, true);
                SetExcelPivotTableCacheAutoReflesh(res, out errMsg);
                createDeafultStyle(res.WorkbookPart, out defaultCellStyle);
            }
            catch(Exception e)
            {
                res = null;
                errMsg = "open error:" + e.Message;
            }
            return res;
        }
        private static void SetExcelPivotTableCacheAutoReflesh(SpreadsheetDocument document,out string errMsg)
        {
            errMsg = "";
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
                //一般原则是做回撤补救措施和继续抛出错误.
                //除非是无修改数据的try块,并且自己可以处理.
                errMsg = e.ToString();
                throw e;
            }
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
        //todo:还是需要去掉customStyle.因为可以修改成员变量的值来达到目的.更简化.
        private static Cell createCell(string reference,Type type,object value,DefaultCellStyle defaultCellStyle,out Exception exc,UInt32 customStyle=0)
        {
            Cell theCell = null;
            exc = null;
            try
            {
                theCell = new Cell();
                theCell.CellReference = reference;

                CellValues theCellValue = GetValueType(type);
                theCell.DataType = new EnumValue<CellValues>(theCellValue);

                if (theCellValue == CellValues.Date)
                {
                    theCell.StyleIndex = defaultCellStyle.dateTimeIndex;
                    if (value.ToString() != "")
                    {
                        theCell.CellValue = new CellValue(((DateTime)value));
                    }
                    else
                    {
                        theCell.CellValue = new CellValue("");
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
            catch(Exception e)
            {
                exc = e;
            }
            return theCell;
        }
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
        private static UInt32Value createCellFormat(Stylesheet styleSheet, UInt32Value borderid,  UInt32Value fontIndex, UInt32Value fillIndex, UInt32Value numberFormatId)
        {
            if (styleSheet.CellFormats.Count == null)
            {
                styleSheet.CellFormats.Count = (UInt32Value)0;
            }
            CellFormat cellFormat = new CellFormat();
            cellFormat.BorderId = 0;
            //if (borderid == null)
            //{
            //    cellFormat.BorderId = 0;
            //    cellFormat.ApplyBorder = true;
            //}
            //else
            //{
            //    cellFormat.BorderId = borderid;
            //    cellFormat.ApplyBorder = true;
            //}
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
            }
        }
        private static SpreadsheetDocument createXlsxExcelFile(string filepath, string firstSheetName, out string errMSG,out DefaultCellStyle defaultCellStyle)
        {
            SpreadsheetDocument spreadsheetDocument = null;
            defaultCellStyle = null;
            errMSG = "";
            try
            {
                //建立xlsx文件
                spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook, true);

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
                errMSG = e.ToString();
                throw e;
            }
            return spreadsheetDocument;
        }
        private static string getShareString(SpreadsheetDocument documet, int index)
        {
            string res = null;
            if (documet != null && documet.WorkbookPart.SharedStringTablePart != null && documet.WorkbookPart.SharedStringTablePart.SharedStringTable != null)
            {
                IEnumerable<SharedStringItem> items = documet.WorkbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>();
                if (items.Count() > index)
                {
                    res = items.ElementAt(index).Text.InnerText;
                }
            }
            return res;
        }
        public static DataTable GetColumnsNames(DataTable dataTable)
        {
            DataTable databable_books = new DataTable("ColumnsNames");
            DataRow newRow = databable_books.NewRow();
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                databable_books.Columns.Add("column"+i, Type.GetType("System.String"));
                
                newRow["column" + i] = dataTable.Columns[i].ColumnName;
            }
            databable_books.Rows.Add(newRow);
            return databable_books;
        }
        #endregion

        #region innerclass
        public class DefaultCellStyle
        {
            public enum ENUM_CellStyle
            {
                normal,
                dateTime,
                black
            }

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