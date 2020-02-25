﻿using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

//todo 标签和对象 res=items.ElementAt(index).Text.InnerText;
//1.table  更新默认,更新指定起止行.
//2.table  新建立默认,更新指定起止行.
//3.得到指定row,column 的数据 .
//4.得到指定区域数据.

/// <summary>
/// 其于OpenXml SDK写的帮助类
/// </summary>

namespace Common
{
    public static class incOpenXml
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
            void onLoadDataTable(DataTable dt,SheetData sheetData, CellStyleIndexCollection indexs);

            void onUpdateDataTable(DataTable dt, SheetData sheetData, CellStyleIndexCollection indexs);
        }

        public class LoadData_WithGuard : ILoadData
        {
            public void onLoadDataTable(DataTable dt, SheetData sheetData, CellStyleIndexCollection indexs)
            {
                UInt32 rowIndex = 1;//rowindex must start from 1;
                List<string> startGuard = new List<string>() { "_reportData"};
                List<string> endGuard = new List<string>() { "_end" };
                CreateRow_String_NormalStyle(sheetData, rowIndex, startGuard, indexs);
                rowIndex++;
                CreateRow_String_TitleStyle(sheetData, rowIndex, GetColumnsNames(dt), indexs);
                rowIndex++;

                //loop to insert each row.
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CreateRow(sheetData, rowIndex, dt.Rows[i], indexs);
                    rowIndex++;
                }
                CreateRow_String_NormalStyle(sheetData, rowIndex, endGuard, indexs);
                rowIndex++;
            }

            public void onUpdateDataTable(DataTable dt, SheetData sheetData, CellStyleIndexCollection indexs)
            {
                sheetData.RemoveAllChildren();//clear all data.
                UInt32 rowIndex = 1;//rowindex must start from 1;

                //insert column name.
                CreateRow_String_TitleStyle(sheetData, rowIndex, GetColumnsNames(dt), indexs);
                rowIndex++;//虽然放到上一行语句更简洁,但放到这里更容易理解.

                //loop to insert each row.
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CreateRow(sheetData, rowIndex, dt.Rows[i], indexs);
                    rowIndex++;
                }
            }
        }

        public class LoadData_SimpleColumn : ILoadData
        {
            public void onLoadDataTable(DataTable dt, SheetData sheetData, CellStyleIndexCollection indexs)
            {
                UInt32 rowIndex = 1;//rowindex must start from 1;

                //insert column name.
                CreateRow_String_TitleStyle(sheetData, rowIndex, GetColumnsNames(dt), indexs);
                rowIndex++;//虽然放到上一行语句更简洁,但放到这里更容易理解.

                //loop to insert each row.
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CreateRow(sheetData, rowIndex, dt.Rows[i], indexs);
                    rowIndex++;
                }
            }

            public void onUpdateDataTable(DataTable dt, SheetData sheetData, CellStyleIndexCollection indexs)
            {
                sheetData.RemoveAllChildren();//clear all data.
                UInt32 rowIndex = 1;//rowindex must start from 1;

                //insert column name.
                CreateRow_String_TitleStyle(sheetData, rowIndex, GetColumnsNames(dt), indexs);
                rowIndex++;//虽然放到上一行语句更简洁,但放到这里更容易理解.

                //loop to insert each row.
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CreateRow(sheetData, rowIndex, dt.Rows[i], indexs);
                    rowIndex++;
                }
            }
        }
        #endregion

        #region private

        #region style

        private static void GenerateDeafultStyle(WorkbookPart workbookpart,out UInt32 titleStyleIndex,out UInt32 normalIndex,out UInt32 dateIndex)
        {
            WorkbookStylesPart stylepart = workbookpart.WorkbookStylesPart;

            if (workbookpart.WorkbookStylesPart==null)
            {
                stylepart = workbookpart.AddNewPart<WorkbookStylesPart>();
            }

            if(stylepart.Stylesheet==null)
            {
                stylepart.Stylesheet = new Stylesheet();
            }
            if(stylepart.Stylesheet.Fonts==null)
            {
                stylepart.Stylesheet.Fonts = new Fonts();
            }
            if(stylepart.Stylesheet.Fills==null)
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

            if(stylepart.Stylesheet.NumberingFormats==null)
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

        private static string getCellValue(Cell cell,WorkbookPart workbookPart)
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

        private static string getShareString(WorkbookPart workbookPart,int index)
        {
            string res = null;
            if(workbookPart.SharedStringTablePart!=null && workbookPart.SharedStringTablePart.SharedStringTable!=null)
            {
                IEnumerable<SharedStringItem> items = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>();
                if (items.Count() > index)
                {
                    res = items.ElementAt(index).Text.InnerText;
                }
            }
            return res;
        }

        private static bool SetPivotSource(WorkbookPart wbPart, string sheetName,string firstReference, string lastReference)
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

        private static void CreateRow_String_TitleStyle(SheetData thesheetData, UInt32Value rowIndex, List<string> data, CellStyleIndexCollection indexs)
        {
            Row theRow = new Row();
            theRow.RowIndex = rowIndex;
            thesheetData.Append(theRow);

            for (int i = 0; i < data.Count; i++)
            {
                Cell theCell = new Cell();
                theRow.InsertAt(theCell, i);
                theCell.StyleIndex = indexs.title;
                theCell.CellValue = new CellValue(data[i]);
                theCell.DataType = new EnumValue<CellValues>(CellValues.String);
            }
        }

        private static void CreateRow_String_NormalStyle(SheetData thesheetData, UInt32Value rowIndex, List<string> data, CellStyleIndexCollection indexs)
        {
            Row theRow = new Row();
            theRow.RowIndex = rowIndex;
            thesheetData.Append(theRow);

            for (int i = 0; i < data.Count; i++)
            {
                Cell theCell = new Cell();
                theRow.InsertAt(theCell, i);
                theCell.StyleIndex = indexs.StringStyle;
                theCell.CellValue = new CellValue(data[i]);
                theCell.DataType = new EnumValue<CellValues>(CellValues.String);
            }
        }

        private static void CreateRow(SheetData thesheetData, UInt32Value rowIndex, DataRow dataRow, CellStyleIndexCollection indexs)
        {
            Row theRow = new Row();
            theRow.RowIndex = rowIndex;
            thesheetData.Append(theRow);

            for (int i = 0; i < dataRow.Table.Columns.Count; i++)
            {
                Cell theCell = new Cell();
                theRow.InsertAt(theCell, i);
                
                CellValues theCellValue = GetValueType(dataRow.Table.Columns[i]);
                if(theCellValue == CellValues.Number)
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
            List<Type> number = new List<Type>{ typeof(Double), typeof(Decimal), typeof(Int32), typeof(int), typeof(Int16),typeof(Int64)};
            List<Type> date = new List<Type> { typeof(DateTime) };

            if (number.Contains(dataColumn.DataType))
            {
                return CellValues.Number;
            }
            else if(date.Contains(dataColumn.DataType))
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
            for(int i=0;i<dataTable.Columns.Count;i++)
            {
                res.Add(dataTable.Columns[i].ColumnName);
            }
            return res;
        }

        // By default, AutoSave = true, Editable = true, and Type = xlsx.
        private static bool CreateSpreadsheetWorkbook2(string filepath, string firstSheetName, DataTable dt, ILoadData implateLoadData, out string errMSG)
        {
            bool res = true;
            errMSG = "";
            try
            {
                //1.建立涉及到的目录2.sheetdata, worksheet.添加到目录.3.sheet,sheets,workbook添加sheets元素.
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                    WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();

                    SheetData sheetData = new SheetData();
                    Worksheet worksheet = new Worksheet();
                    worksheet.InsertAt<SheetData>(sheetData, 0);
                    worksheetPart.Worksheet = worksheet;


                    Sheet sheet = new Sheet()
                    {
                        Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name=firstSheetName
                    };
                    Sheets sheets = new Sheets();
                    sheets.Append(sheet);
                    Workbook workbook = new Workbook();
                    workbook.AppendChild<Sheets>(sheets);
                    workbookpart.Workbook = workbook;


                    UInt32 contentIndex, titleIndex, dateIndex;
                    GenerateDeafultStyle(workbookpart, out titleIndex, out contentIndex, out dateIndex);

                    if (implateLoadData != null)
                    {
                        CellStyleIndexCollection cellStyleIndex = new CellStyleIndexCollection(titleIndex, contentIndex, dateIndex, contentIndex);
                        implateLoadData.onLoadDataTable(dt, sheetData, cellStyleIndex);
                    }
                }
            }
            catch
            {

            }
            return res;
            }

        // By default, AutoSave = true, Editable = true, and Type = xlsx.
        private static bool CreateSpreadsheetWorkbook(string filepath, string firstSheetName, DataTable dt, ILoadData implateLoadData, out string errMSG)
        {
            bool res = true;
            errMSG = "";
            try
            {
                //1.建立文档2.添加WorkbookPart(xl目录?猜测)3.建立workbook.4.建立样式表5.建立WorksheetPart(wroksheet目录)
                //6.建立worksheet.7.建立sheetdata.
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                    workbookpart.Workbook = new Workbook();

                    UInt32 contentIndex, titleIndex, dateIndex;
                    GenerateDeafultStyle(workbookpart, out titleIndex, out contentIndex, out dateIndex);

                    //Worksheet
                    WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    // Add Sheets to the Workbook.
                    Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                    // Append a new worksheet and associate it with the workbook.
                    Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = firstSheetName };

                    sheets.Append(sheet);

                    if (implateLoadData != null)
                    {
                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                        CellStyleIndexCollection cellStyleIndex = new CellStyleIndexCollection(titleIndex, contentIndex, dateIndex, contentIndex);
                        implateLoadData.onLoadDataTable(dt, sheetData, cellStyleIndex);
                    }

                    workbookpart.Workbook.Save();
                }
            }
            catch (Exception ex)
            {

                res = false;
                errMSG = ex.Message;
            }

            return res;
        }

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

        private static bool UpdateSpreadsheetWorkbook(string filepath, string updateSheetName, DataTable dt, ILoadData implateLoadData, out string errMSG)
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

                        //load data
                        if (implateLoadData != null)
                        {
                            CellStyleIndexCollection cellStyleIndex = new CellStyleIndexCollection(titleIndex, contentIndex, dateIndex, contentIndex);
                            implateLoadData.onUpdateDataTable(dt, reportSheetData, cellStyleIndex);
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
        #endregion

        #region public
        public static bool GenerateXlsxExcel(DataTable rpdt, out string errMsg, string filePath,int row,int column)
        {
            bool res = false;
            ILoadData loadDataHandler = new LoadData_WithGuard();
            res = CreateSpreadsheetWorkbook(filePath, "Report", rpdt, loadDataHandler, out errMsg);
            return res;
        }

        public static bool UpdataData4XlsxExcel(DataTable rpdt, string dataSheetName, out string errMsg, string filePath)
        {
            bool res = false;
            ILoadData loadDataHandler = new LoadData_WithGuard();
            res = UpdateSpreadsheetWorkbook(filePath, dataSheetName, rpdt, loadDataHandler, out errMsg);
            return res;
        }

        public static bool SetPivotSource(string filePath, string SourcesheetName, string firstReference, string lastReference)
        {
            bool res = false;
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(filePath, true))
            {
                SetExcelAutoReflesh(document);
                res = SetPivotSource(document.WorkbookPart, SourcesheetName, firstReference, lastReference);
            }
            return res;
        }

        

        public static bool GenerateXlsxExcel(DataTable rpdt, out string errMsg, string filePath)
        {
            bool res = false;
            ILoadData loadDataHandler = new LoadData_WithGuard();//linson. 采用接口,来扩展变化,把变化抽离出来,而不是每次直接修改整体方法!
            res = CreateSpreadsheetWorkbook2(filePath, "Report", rpdt, loadDataHandler, out errMsg);
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