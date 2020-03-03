using DocumentFormat.OpenXml;
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

        #region public function 为什么public 简单包一个静态方法?主要是因为想让主要方法设为static,减低耦合度.而为了使用方便,又用public 包一下,方便使用. 非常好的一个策略.
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
        public Row InsertRowAfter(string sheetName,  UInt32 columnNumber, DataRow dataRow, Row guradRow)
        {
            //var sheetDatas = getWorksheet(document, sheetName).Elements<SheetData>();
            //if (sheetDatas != null)
            //{
            //    var sheetdata = sheetDatas.First();
            //}
            return null;
        }

        //new and uupdate rows with table
        public bool SetOrReplaceRows(string sheetName, UInt32 rowNumber, UInt32 columnNumber, DataTable dataTable)
        {
            bool res = false;
            if (dataTable != null)
            {
                uint writingGuard = rowNumber;
                //start to set column name.
                DataTable columnsTable = Common.MyExcelFile.GetColumnsNames(dataTable);
                SetOrReplaceRow(sheetName, writingGuard, columnNumber, columnsTable.Rows[0]);
                writingGuard++;
                //start to set datatable
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
        #endregion

        #region private
        private static Row SetOrReplaceRow(SheetData sheetData, UInt32 startRow,UInt32 startColumn, DataRow dataRow,DefaultCellStyle defaultCellStyle)
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
                    newCell.CellValue = new CellValue(((DateTime)dataRow[(int)i]));
                }
                else
                {
                    newCell.StyleIndex = defaultCellStyle.normalIndex;
                    newCell.CellValue = new CellValue(dataRow[(int)i].ToString());
                }
                newRow.Append(newCell);
            }
            //是否存在row,1.存在,删除.2.现在是否存在大于row,存在,在前插入.不存在.直接附加.
            var rows = sheetData.Elements<Row>().Where(x => x.RowIndex == startRow);
            if (rows.Count()>0)
            {
                sheetData.RemoveChild(rows.First());
            }

            var biggerrows = sheetData.Elements<Row>().Where(x => x.RowIndex > startRow);
            if (biggerrows.Count()<=0)
            {
                sheetData.Append(newRow);//todo:装载数据7/10的时间耗费在这里。需要优化！如果是大数据插入，应该创建大量row，再使用一次append或其他插入函数。
            }
            else
            {
                sheetData.InsertBefore(newRow, biggerrows.First());
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
                errMsg = e.Message;//todo:learn try catch.
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
                    theCell.CellValue = new CellValue(((DateTime)value));
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
                errMSG = e.Message;
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

    public static class nouse
    {
     
                #region strategy pattern
                public class CellStyleIndexCollection
                {
                    public UInt32 title;
                    public UInt32 Number;
                    public UInt32 dateTime;
                    public UInt32 StringStyle;

                    public CellStyleIndexCollection(uint title, uint number, uint dateTime, uint _StringStyle)
                    {
                        this.title = title;
                        Number = number;
                        this.dateTime = dateTime;
                        StringStyle = _StringStyle;
                    }
                }

                public interface ILoadData
                {
                    void onClearAndLoadDataTable(DataTable dt, SheetData sheetData, CellStyleIndexCollection indexs, int row = 1, int column = 1);
                }

                public class LoadData_WithGuard : ILoadData
                {
                    public void onClearAndLoadDataTable(DataTable dt, SheetData sheetData, CellStyleIndexCollection indexs, int row = 1, int column = 1)
                    {
                        UInt32 rowIndex = (UInt32)row;
                        List<string> startGuard = new List<string>() { "_reportData" };
                        List<string> endGuard = new List<string>() { "_end" };
                        CreateRow_String_NormalStyle(sheetData, rowIndex, startGuard, indexs);
                        rowIndex++;
                        CreateRow_String_TitleStyle(sheetData, rowIndex, GetColumnsNames(dt), indexs, column);
                        rowIndex++;

                        //loop to insert each row.
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            CreateRow(sheetData, rowIndex, dt.Rows[i], indexs, column);
                            rowIndex++;
                        }
                        CreateRow_String_NormalStyle(sheetData, rowIndex, endGuard, indexs);
                        rowIndex++;
                    }
                }

                #endregion

                #region private

                #region style

                private static void GenerateDeafultStyle(WorkbookPart workbookpart, out UInt32 titleStyleIndex, out UInt32 normalIndex, out UInt32 dateIndex)
                {
                    WorkbookStylesPart stylepart = workbookpart.WorkbookStylesPart;

                    if (workbookpart.WorkbookStylesPart == null)
                    {
                        stylepart = workbookpart.AddNewPart<WorkbookStylesPart>();
                    }

                    if (stylepart.Stylesheet == null)
                    {
                        stylepart.Stylesheet = new Stylesheet();
                    }
                    if (stylepart.Stylesheet.Fonts == null)
                    {
                        stylepart.Stylesheet.Fonts = new Fonts();
                    }
                    if (stylepart.Stylesheet.Fills == null)
                    {
                        stylepart.Stylesheet.Fills = new Fills(
                        new Fill(new PatternFill() { PatternType = PatternValues.None }),
                        new Fill(new PatternFill() { PatternType = PatternValues.Gray125 })
                        );
                    }

                    if (stylepart.Stylesheet.Borders == null)
                    {
                        stylepart.Stylesheet.Borders = new Borders(
                        new Border(
                        new RightBorder(),
                        new TopBorder(),
                        new BottomBorder(),
                        new DiagonalBorder())
                        );
                    }

                    if (stylepart.Stylesheet.CellFormats == null)
                    {
                        stylepart.Stylesheet.CellFormats = new CellFormats();
                    }

                    if (stylepart.Stylesheet.NumberingFormats == null)
                    {
                        stylepart.Stylesheet.NumberingFormats = new NumberingFormats();
                    }


                    Stylesheet styleSheet = stylepart.Stylesheet;
                    var fontIndex = createFont(styleSheet, "Microsoft YaHei", (double)11, false, System.Drawing.Color.Black);
                    var numberFormatDate_index = new NumberingFormat();
                    numberFormatDate_index.FormatCode = new StringValue("yyyy-mm-dd");
                    numberFormatDate_index.NumberFormatId = 240;

                    stylepart.Stylesheet.NumberingFormats.InsertAt(numberFormatDate_index, stylepart.Stylesheet.NumberingFormats.Count());


                    var fontStyleIndex = createCellFormat(styleSheet, fontIndex, null, null);

                    var fillIndex = createFill(styleSheet, System.Drawing.Color.White);
                    var fillRedStyleIndex = createCellFormat(styleSheet, fontIndex, fillIndex, null);

                    var fontBlackIndex = createFont(styleSheet, "Microsoft YaHei", (double)11, true, System.Drawing.Color.Black);
                    var fontBlackStyleIndex = createCellFormat(styleSheet, fontBlackIndex, null, null);

                    var defaultDateStyleIndex = createCellFormat(styleSheet, fontIndex, null, numberFormatDate_index.NumberFormatId);//时间格式 14:yyyy/mm/dd 

                    titleStyleIndex = fontBlackStyleIndex;
                    normalIndex = fontStyleIndex;
                    dateIndex = defaultDateStyleIndex;

                    stylepart.Stylesheet.Save();
                }

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

                private static UInt32Value createFill(Stylesheet styleSheet, System.Drawing.Color fillColor)
                {
                    if (styleSheet.Fills.Count == null)
                    {
                        styleSheet.Fills.Count = (UInt32Value)2;

                    }

                    Fill fill = new Fill(
                        new PatternFill(
                             new ForegroundColor()
                             {
                                 Rgb = new HexBinaryValue()
                                 {
                                     Value = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(fillColor.A, fillColor.R, fillColor.G, fillColor.B)).Replace("#", "")
                                 }
                             })
                        {
                            PatternType = PatternValues.Solid
                        }
                    );
                    styleSheet.Fills.Append(fill);

                    UInt32Value result = styleSheet.Fills.Count;

                    styleSheet.Fills.Count++;

                    return result;
                }

                //创建一个单元格样式
                private static UInt32Value createCellFormat(Stylesheet styleSheet, UInt32Value fontIndex, UInt32Value fillIndex, UInt32Value numberFormatId)
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
                        cellFormat.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                    }

                    styleSheet.CellFormats.Append(cellFormat);
                    UInt32Value result = styleSheet.CellFormats.Count;
                    styleSheet.CellFormats.Count++;
                    return result;
                }

                #endregion

                //private static int findFirstRow(SheetData sheetData, string target)
                //{
                //    int res = -1;
                //    if (sheetData.GetEnumerator().GetType() == typeof(IEnumerator<Row>))
                //    {
                //        IEnumerator<Row> items = (IEnumerator<Row>)sheetData.GetEnumerator();
                //        while(items.Current.GetFirstChild<Cell>().)
                //    }
                //    else
                //    {
                //        throw new Exception("error type,type of child is't row");
                //    }

                //    return res;
                //}

                private static string getCellValue(Cell cell, WorkbookPart workbookPart)
                {
                    string res = null;
                    try
                    {
                        if (cell.DataType == CellValues.String)
                        {
                            res = cell.CellValue.ToString();
                        }
                        else if (cell.DataType == CellValues.SharedString)
                        {
                            res = getShareString(workbookPart, int.Parse(cell.InnerText));
                        }
                    }
                    catch
                    {//do noting just res=null
                    }
                    return res;
                }

                private static string getShareString(WorkbookPart workbookPart, int index)
                {
                    string res = null;
                    if (workbookPart.SharedStringTablePart != null && workbookPart.SharedStringTablePart.SharedStringTable != null)
                    {
                        IEnumerable<SharedStringItem> items = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>();
                        if (items.Count() > index)
                        {
                            res = items.ElementAt(index).Text.InnerText;
                        }
                    }
                    return res;
                }

                private static bool SetPivotSource(WorkbookPart wbPart, string sheetName, string firstReference, string lastReference)
                {
                    bool res = false;
                    try
                    {
                        var pivottableCashes = wbPart.PivotTableCacheDefinitionParts;
                        foreach (PivotTableCacheDefinitionPart pivottablecachePart in pivottableCashes)
                        {
                            pivottablecachePart.PivotCacheDefinition.CacheSource.RemoveAllChildren();
                            pivottablecachePart.PivotCacheDefinition.CacheSource.Append(new WorksheetSource()
                            {
                                Sheet = sheetName,
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

                private static void CreateRow_String_TitleStyle(SheetData thesheetData, UInt32Value rowIndex, List<string> data, CellStyleIndexCollection indexs, int column = 1)
                {
                    Row theRow = new Row();
                    theRow.RowIndex = rowIndex;
                    thesheetData.Append(theRow);

                    for (int i = 0; i < column - 1; i++)
                    {
                        Cell theCell = new Cell();
                        theRow.InsertAt(theCell, i);
                    }

                    for (int i = 0; i < data.Count; i++)
                    {
                        Cell theCell = new Cell();
                        theRow.InsertAt(theCell, i + (column - 1));

                        theCell.StyleIndex = indexs.title;
                        theCell.CellValue = new CellValue(data[i]);
                        theCell.DataType = new EnumValue<CellValues>(CellValues.String);

                    }
                }

                private static void CreateRow_String_NormalStyle(SheetData thesheetData, UInt32Value rowIndex, List<string> data, CellStyleIndexCollection indexs, int column = 1)
                {
                    Row theRow = new Row();
                    theRow.RowIndex = rowIndex;
                    thesheetData.Append(theRow);

                    for (int i = 0; i < column - 1; i++)
                    {
                        Cell theCell = new Cell();
                        theRow.InsertAt(theCell, i);
                    }

                    for (int i = 0; i < data.Count; i++)
                    {
                        Cell theCell = new Cell();
                        theRow.InsertAt(theCell, i + (column - 1));

                        theCell.StyleIndex = indexs.StringStyle;
                        theCell.CellValue = new CellValue(data[i]);
                        theCell.DataType = new EnumValue<CellValues>(CellValues.String);

                    }
                }

                private static void CreateRow(SheetData thesheetData, UInt32Value rowIndex, DataRow dataRow, CellStyleIndexCollection indexs, int column = 1)
                {
                    Row theRow = new Row();
                    theRow.RowIndex = rowIndex;
                    thesheetData.Append(theRow);

                    for (int i = 0; i < column - 1; i++)
                    {
                        Cell theCell = new Cell();
                        theRow.InsertAt(theCell, i);
                    }

                    for (int i = 0; i < dataRow.Table.Columns.Count; i++)
                    {
                        Cell theCell = new Cell();
                        theRow.InsertAt(theCell, i + (column - 1));

                        CellValues theCellValue = GetValueType(dataRow.Table.Columns[i]);
                        if (theCellValue == CellValues.Number)
                        {
                            theCell.StyleIndex = indexs.Number;
                            theCell.CellValue = new CellValue(dataRow[i].ToString());
                            theCell.DataType = new EnumValue<CellValues>(theCellValue);
                        }
                        else if (theCellValue == CellValues.Date)
                        {
                            theCell.StyleIndex = indexs.dateTime;
                            theCell.CellValue = new CellValue(((DateTime)dataRow[i]));
                            theCell.DataType = new EnumValue<CellValues>(theCellValue);
                        }
                        else
                        {
                            theCell.StyleIndex = indexs.StringStyle;
                            theCell.CellValue = new CellValue(dataRow[i].ToString());
                            theCell.DataType = new EnumValue<CellValues>(theCellValue);
                        }

                    }
                }

                private static CellValues GetValueType(DataColumn dataColumn)
                {
                    List<Type> number = new List<Type> { typeof(Double), typeof(Decimal), typeof(Int32), typeof(int), typeof(Int16), typeof(Int64) };
                    List<Type> date = new List<Type> { typeof(DateTime) };

                    if (number.Contains(dataColumn.DataType))
                    {
                        return CellValues.Number;
                    }
                    else if (date.Contains(dataColumn.DataType))
                    {
                        return CellValues.Date;
                    }
                    else
                    {
                        return CellValues.String;
                    }
                }

                private static List<string> GetColumnsNames(DataTable dataTable)
                {
                    List<string> res = new List<string>();
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        res.Add(dataTable.Columns[i].ColumnName);
                    }
                    return res;
                }

                // By default, AutoSave = true, Editable = true, and Type = xlsx.
                private static bool CreateSpreadsheetWorkbook2(string filepath, string firstSheetName, DataTable dt, ILoadData implateLoadData, out string errMSG, int row, int column)
                {
                    bool res = true;
                    errMSG = "";
                    try
                    {
                        //1.建立涉及到的目录2.sheetdata, worksheet.最后把worksheet添加到目录.3.sheet,sheets,最后把sheets 元素添加到workbook.
                        using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook))
                        {
                            //建立xl,worksheets目录
                            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                            //建立Worksheet各种元素,最后建立Worksheet文件
                            SheetData sheetData = new SheetData();
                            Worksheet worksheet = new Worksheet();
                            worksheet.InsertAt<SheetData>(sheetData, 0);
                            worksheetPart.Worksheet = worksheet;
                            //建立Workbook各种数据,主要是sheet记录,最后建立Workbook文件
                            Sheet sheet = new Sheet()
                            {
                                Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                                SheetId = 1,
                                Name = firstSheetName
                            };
                            Sheets sheets = new Sheets();
                            sheets.Append(sheet);
                            Workbook workbook = new Workbook();
                            workbook.AppendChild<Sheets>(sheets);
                            workbookpart.Workbook = workbook;

                            //建立或插入样式元素.
                            UInt32 contentIndex, titleIndex, dateIndex;
                            GenerateDeafultStyle(workbookpart, out titleIndex, out contentIndex, out dateIndex);
                            CellStyleIndexCollection cellStyleIndex = new CellStyleIndexCollection(titleIndex, contentIndex, dateIndex, contentIndex);

                            //执行数据加载
                            if (implateLoadData != null)
                            {
                                //implateLoadData.onLoadDataTable(dt, sheetData, cellStyleIndex, row, column);
                            }
                        }
                    }
                    catch
                    {

                    }
                    return res;
                }

                //// By default, AutoSave = true, Editable = true, and Type = xlsx.
                //private static bool CreateSpreadsheetWorkbook(string filepath, string firstSheetName, DataTable dt, ILoadData implateLoadData, out string errMSG)
                //{
                //    bool res = true;
                //    errMSG = "";
                //    try
                //    {
                //        //1.建立文档2.添加WorkbookPart(xl目录?猜测)3.建立workbook.4.建立样式表5.建立WorksheetPart(wroksheet目录)
                //        //6.建立worksheet.7.建立sheetdata.
                //        using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook))
                //        {
                //            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                //            workbookpart.Workbook = new Workbook();

                //            UInt32 contentIndex, titleIndex, dateIndex;
                //            GenerateDeafultStyle(workbookpart, out titleIndex, out contentIndex, out dateIndex);

                //            //Worksheet
                //            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                //            worksheetPart.Worksheet = new Worksheet(new SheetData());

                //            // Add Sheets to the Workbook.
                //            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                //            // Append a new worksheet and associate it with the workbook.
                //            Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = firstSheetName };

                //            sheets.Append(sheet);

                //            if (implateLoadData != null)
                //            {
                //                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                //                CellStyleIndexCollection cellStyleIndex = new CellStyleIndexCollection(titleIndex, contentIndex, dateIndex, contentIndex);
                //                implateLoadData.onLoadDataTable(dt, sheetData, cellStyleIndex);
                //            }

                //            workbookpart.Workbook.Save();
                //        }
                //    }
                //    catch (Exception ex)
                //    {

                //        res = false;
                //        errMSG = ex.Message;
                //    }

                //    return res;
                //}

                private static Dictionary<String, OpenXmlPart> BuildUriPartDictionary(SpreadsheetDocument document)
                {
                    var uriPartDictionary = new Dictionary<String, OpenXmlPart>();
                    var queue = new Queue<OpenXmlPartContainer>();
                    queue.Enqueue(document);
                    while (queue.Count > 0)
                    {
                        foreach (var part in queue.Dequeue().Parts.Where(part => !uriPartDictionary.Keys.Contains(part.OpenXmlPart.Uri.ToString())))
                        {
                            uriPartDictionary.Add(part.OpenXmlPart.Uri.ToString(), part.OpenXmlPart);
                            queue.Enqueue(part.OpenXmlPart);
                        }
                    }
                    return uriPartDictionary;
                }

                private static bool UpdateSpreadsheetWorkbook(string filepath, string updateSheetName, DataTable dt, ILoadData implateLoadData, out string errMSG, int row = 1, int column = 1)
                {
                    bool res = true;
                    errMSG = "";

                    try
                    {
                        using (SpreadsheetDocument document = SpreadsheetDocument.Open(filepath, true))
                        {
                            SetExcelAutoReflesh(document);

                            IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == updateSheetName);
                            if (sheets.Count() == 0)
                            {
                                res = false;
                                errMSG = "can't find 'report' sheet.";
                            }
                            else
                            {
                                string relationshipId = sheets.First().Id.Value;
                                WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(relationshipId);
                                Worksheet theSheet = worksheetPart.Worksheet;
                                SheetData reportSheetData = theSheet.GetFirstChild<SheetData>();

                                UInt32 contentIndex, titleIndex, dateIndex;
                                GenerateDeafultStyle(document.WorkbookPart, out titleIndex, out contentIndex, out dateIndex);
                                CellStyleIndexCollection cellStyleIndex = new CellStyleIndexCollection(titleIndex, contentIndex, dateIndex, contentIndex);

                                //load data
                                if (implateLoadData != null)
                                {
                                    //implateLoadData.onUpdateDataTable(dt, reportSheetData, cellStyleIndex, row, column);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        res = false;
                        errMSG = ex.Message;
                    }


                    return res;
                }


                private static void SetExcelAutoReflesh(SpreadsheetDocument document)
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

                private static bool SetPivotSource(string filePath, string SourcesheetName, string firstReference, string lastReference)
                {
                    bool res = false;
                    using (SpreadsheetDocument document = SpreadsheetDocument.Open(filePath, true))
                    {
                        SetExcelAutoReflesh(document);
                        res = SetPivotSource(document.WorkbookPart, SourcesheetName, firstReference, lastReference);
                    }
                    return res;
                }
                #endregion

                #region public
                public static bool GenerateXlsxExcel(DataTable rpdt, out string errMsg, string filePath, int row = 1, int column = 1)
                {
                    bool res = false;
                    ILoadData loadDataHandler = new LoadData_WithGuard();
                    res = CreateSpreadsheetWorkbook2(filePath, "Report", rpdt, loadDataHandler, out errMsg, row, column);
                    return res;
                }



                public static bool UpdataData4XlsxExcel(DataTable rpdt, string dataSheetName, out string errMsg, string filePath, int row = 1, int column = 1)
                {
                    bool res = false;
                    ILoadData loadDataHandler = new LoadData_WithGuard();
                    res = UpdateSpreadsheetWorkbook(filePath, dataSheetName, rpdt, loadDataHandler, out errMsg, row, column);
                    return res;
                }

                public static void testUnit(string filePath)
                {
                    try
                    {
                        using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filePath, true))
                        {
                            string a = getShareString(spreadsheetDocument.WorkbookPart, 1);
                            Debug.WriteLine(a);

                            //bool res = Common.incOpenXml.SetPivotSource(filePath, "Report", "A1", "B2");

                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                #endregion

                #region nouse
                //public static bool XLDeleteSheet(string fileName, string sheetToDelete)
                //{
                //    bool returnValue = false;
                //    using (SpreadsheetDocument xlDoc = SpreadsheetDocument.Open(fileName, true))
                //    {
                //        XmlDocument doc = new XmlDocument();
                //        doc.Load(xlDoc.WorkbookPart.GetStream());

                //        XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
                //        nsManager.AddNamespace("d", doc.DocumentElement.NamespaceURI);

                //        string searchString = string.Format("//d:sheet[@name='{0}']", sheetToDelete);
                //        XmlNode node = doc.SelectSingleNode(searchString, nsManager);
                //        if (node != null)
                //        {
                //            XmlAttribute relationAttribute = node.Attributes["r:id"];
                //            if (relationAttribute != null)
                //            {
                //                string relId = relationAttribute.Value;
                //                xlDoc.WorkbookPart.DeletePart(relId);
                //                node.ParentNode.RemoveChild(node);
                //                doc.Save(xlDoc.WorkbookPart.GetStream(FileMode.Create));
                //                returnValue = true;
                //            }
                //        }
                //    }
                //    return returnValue;
                //}

                //public static Worksheet GetWorksheet(this SpreadsheetDocument document, string sheetName = null)
                //{
                //    var sheets = document.WorkbookPart.Workbook.Descendants<Sheet>();
                //    var sheet = (sheetName == null
                //                     ? sheets.FirstOrDefault()
                //                     : sheets.FirstOrDefault(s => s.Name == sheetName)) ?? sheets.FirstOrDefault();

                //    var worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheet.Id);
                //    return worksheetPart.Worksheet;
                //}

                //public static SheetData GetFirstSheetData(this SpreadsheetDocument document, string sheetName = null)
                //{
                //    return document.GetWorksheet(sheetName).GetFirstChild<SheetData>();
                //}

                //public static SheetData GetFirstSheetData(this Worksheet worksheet)
                //{
                //    return worksheet.GetFirstChild<SheetData>();
                //}

                //public static SharedStringTablePart GetSharedStringTable(this SpreadsheetDocument document)
                //{
                //    var sharedStringTable = document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                //    return sharedStringTable;
                //}

                //public static void UpdateCellText(this SheetData sheetData, string cellName, string cellText)
                //{
                //    var cell = sheetData.GetCell(cellName);
                //    if (cell == null)
                //    {
                //        return;
                //    }
                //    cell.UpdateCellText(cellText);
                //}

                //public static void SetCellValue(this SheetData sheetData, string cellName, object cellText = null)
                //{
                //    SetCellValue(sheetData, cellName, cellText ?? string.Empty, CellStyleIndex);
                //}

                //private static void CreateCell(this Row row, string cellName, object cellText, uint cellStyleIndex)
                //{
                //    var refCell =
                //        row.Elements<Cell>()
                //        .FirstOrDefault(
                //            cell =>
                //            string.Compare(cell.CellReference.Value, cellName, StringComparison.OrdinalIgnoreCase) > 0);
                //    var resultCell = new Cell { CellReference = cellName };
                //    resultCell.UpdateCell(cellText, cellStyleIndex);
                //    row.InsertBefore(resultCell, refCell);
                //} 
                //private static void SetCellValue(this SheetData sheetData, string cellName, object cellText, uint cellStyleIndex)
                //{
                //    uint rowIndex = GetRowIndex(cellName);
                //    var row = sheetData.GetRow(rowIndex);
                //    if (row == null)
                //    {
                //        row = new Row { RowIndex = rowIndex };
                //        row.CreateCell(cellName, cellText, cellStyleIndex);
                //        sheetData.Append(row);
                //    }
                //    else
                //    {
                //        var cell = row.GetCell(cellName);
                //        if (cell == null)
                //        {
                //            row.CreateCell(cellName, cellText, cellStyleIndex);
                //        }
                //        else
                //        {
                //            cell.UpdateCell(cellText, cellStyleIndex);
                //        }
                //    }
                //}     
                //public static int GetRowsCount(this IEnumerable<Row> rows)
                //{
                //    return rows.GroupBy(x => x.RowIndex.Value).Count();
                //}

                //public static IList<Cell> GetCells(this IEnumerable<Row> rows, int rowIndex)
                //{
                //    return rows.Where(row => row.RowIndex.Value == rowIndex).SelectMany(row => row.Elements<Cell>()).ToList();
                //}

                //public static string GetCellValue(this IEnumerable<Cell> cells, string cellName, SharedStringTablePart stringTablePart)
                //{
                //    if (cells == null)
                //    {
                //        throw new ArgumentNullException("cells");
                //    }
                //    if (cellName == null)
                //    {
                //        throw new ArgumentNullException("cellName");
                //    }
                //    var cell = (from item in cells where item.CellReference == cellName select item).FirstOrDefault();
                //    if (cell == null)
                //    {
                //        return string.Empty;
                //    }
                //    if (cell.ChildElements.Count == 0)
                //    {
                //        return string.Empty;
                //    }
                //    var value = cell.CellValue.InnerText;
                //    if (cell.DataType == null)
                //    {
                //        return value;
                //    }
                //    switch (cell.DataType.Value)
                //    {
                //        case CellValues.SharedString:
                //            if (stringTablePart != null)
                //            {
                //                value = stringTablePart.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                //            }
                //            break;
                //        case CellValues.Boolean:
                //            value = value == "0" ? "FALSE" : "TRUE";
                //            break;
                //    }
                //    return value;
                //}

                //public static string ValidateDocument(this SpreadsheetDocument document)
                //{
                //    var msg = new StringBuilder();
                //    try
                //    {
                //        var validator = new OpenXmlValidator();
                //        int count = 0;
                //        foreach (ValidationErrorInfo error in validator.Validate(document))
                //        {
                //            count++;
                //            msg.Append("\nError " + count)
                //               .Append("\nDescription: " + error.Description)
                //               .Append("\nErrorType: " + error.ErrorType)
                //               .Append("\nNode: " + error.Node)
                //               .Append("\nPath: " + error.Path.XPath)
                //               .Append("\nPart: " + error.Part.Uri)
                //               .Append("\n-------------------------------------------");
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        msg.Append(ex.Message);
                //    }

                //    return msg.ToString();
                //}



                //private static uint GetRowIndex(string cellName)
                //{
                //    var regex = new Regex(@"\d+");
                //    var match = regex.Match(cellName);
                //    return uint.Parse(match.Value);
                //}

                //private static CellValues GetCellDataType(object cellText)
                //{
                //    var type = cellText.GetType();
                //    switch (type.Name)
                //    {
                //        case "Int32":
                //        case "Decimal":
                //        case "Double":
                //        case "Int64":
                //            return CellValues.Number;
                //        case "String":
                //            return CellValues.String;
                //        ////            case "DateTime":
                //        ////                return CellValues.Date;
                //        default:
                //            return CellValues.String;
                //    }
                //}

                //private static void UpdateCell(this Cell cell, object cellText, uint cellStyleIndex)
                //{
                //    cell.UpdateCellText(cellText);
                //    cell.StyleIndex = cellStyleIndex;
                //}

                //private static void UpdateCellText(this Cell cell, object cellText)
                //{
                //    cell.DataType = GetCellDataType(cellText);
                //    cell.CellValue = cell.CellValue ?? new CellValue();
                //    cell.CellValue.Text = cellText.ToString();
                //}

                //private static Row GetRow(this SheetData sheetData, long rowIndex)
                //{
                //    return sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
                //}

                //private static Cell GetCell(this Row row, string cellName)
                //{
                //    return row.Elements<Cell>().FirstOrDefault(c => c.CellReference.Value == cellName);
                //}

                //private static Cell GetCell(this SheetData sheetData, string cellName)
                //{
                //    return sheetData.Descendants<Cell>().FirstOrDefault(c => c.CellReference.Value == cellName);
                //}
                #endregion
            }
}