using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CUSTOMRP.BLL
{
    public abstract class TemplateManager
    {
        public const string STRFIRST_SHEETNAME = "Report";
        public const string STRING_DATASTART = "_DATASTART";
        public const string STRING_DATAEND = "_DATAEND";



        public static bool GenerateXlsxExcel(DataTable dataTable, out string errMsg, string filePath, ReportArgument reportArgument)
        {
            bool res = false;
            errMsg = "";
            if (dataTable != null)
            {
                using (Common.MyExcelFile myexcel = new Common.MyExcelFile(filePath, true, STRFIRST_SHEETNAME, out errMsg))
                {
                    InsertData(STRFIRST_SHEETNAME, dataTable, myexcel, 1, null, reportArgument);
                    res = true;
                }
            }
            return res;
        }



        public static bool UpdataData4XlsxExcel(DataTable dataTable, out string errMsg, string filePath, ReportArgument reportArgument)
        {
            //0. find range of data  1.remove pre date 2. reset range for ready to insert data  3.put newdata.
            bool res = true;
            using (Common.MyExcelFile myexcel = new Common.MyExcelFile(filePath, out errMsg))
            {
                uint startRowIndex, endRowIndex;
                FindDataRange(myexcel, out startRowIndex, out endRowIndex);

                if (endRowIndex >= startRowIndex && startRowIndex > 0)//when it  has  'data_start' and 'data_end'
                {
                    //get pre styles of data.
                    Dictionary<uint, Dictionary<string, uint>> tableStyle = GetTableDataStyles(myexcel, startRowIndex, endRowIndex, (UInt32)dataTable.Rows.Count);
                    IEnumerable<Row> rows_bottom = myexcel.GetRangeRows(STRFIRST_SHEETNAME, endRowIndex + 1, UInt32.MaxValue);

                    //reomve pre data;
                    myexcel.RemoveRows(STRFIRST_SHEETNAME, startRowIndex, endRowIndex);

                    //reset range for inserting data
                    int deleteCount = (int)(endRowIndex - startRowIndex + 1);
                    int insertCount = dataTable == null ? 1 : dataTable.Rows.Count + 1;
                    int offsetCount = insertCount - deleteCount;
                    Common.MyExcelFile.updateRowIndexAndCellReference(rows_bottom, offsetCount);

                    //insert new data;
                    InsertData(STRFIRST_SHEETNAME, dataTable, myexcel, startRowIndex, tableStyle, reportArgument);

                    //update pivotTable
                    string startRef = Common.MyExcelFile.GetCellReference((uint)2, startRowIndex);
                    string endRef = Common.MyExcelFile.GetCellReference((uint)2 + (uint)dataTable.Columns.Count - 1, startRowIndex + (uint)dataTable.Rows.Count);
                    myexcel.UpdateAllPivotSource(STRFIRST_SHEETNAME, startRef, endRef);
                }
                else//when it does't has  'data_start' and 'data_end'
                {
                    myexcel.RemoveAllRows(STRFIRST_SHEETNAME);
                    InsertData(STRFIRST_SHEETNAME, dataTable, myexcel, 1, null, null);

                    //update pivotTable
                    string startRef = Common.MyExcelFile.GetCellReference((uint)2, 1);
                    string endRef = Common.MyExcelFile.GetCellReference((uint)2 + (uint)dataTable.Columns.Count - 1, (uint)dataTable.Rows.Count + 1);
                    myexcel.UpdateAllPivotSource(STRFIRST_SHEETNAME, startRef, endRef);
                }

            }
            return res;
        }

        #region private

        private static void InsertData(string sheetname, DataTable dataTable, Common.MyExcelFile myexcel, uint startRowIndex, Dictionary<uint, Dictionary<string, uint>> tableStyle, ReportArgument reportArgument)
        {
            myexcel.SetOrReplaceRows(sheetname, startRowIndex, 2, dataTable, tableStyle);
            if (dataTable != null)
            {
                setGuard(startRowIndex, startRowIndex + (UInt32)dataTable.Rows.Count, myexcel);
            }
            else
            {
                setGuard(startRowIndex, startRowIndex, myexcel);
            }
        }

        private static void FindDataRange(Common.MyExcelFile myexcel, out uint startRowIndex, out uint endRowIndex)
        {
            startRowIndex = myexcel.GetRowIndexFromAColumn(STRFIRST_SHEETNAME, STRING_DATASTART, 1);
            endRowIndex = myexcel.GetRowIndexFromAColumn(STRFIRST_SHEETNAME, STRING_DATAEND, 1);
            if (startRowIndex == 0 && endRowIndex == 0)
            {
                startRowIndex = myexcel.GetRowIndexFromAColumn(STRFIRST_SHEETNAME, STRING_DATASTART + STRING_DATAEND, 1);
                endRowIndex = startRowIndex;
            }
        }


        private static void setGuard(uint startRow, uint endrow, Common.MyExcelFile myexcel)
        {
            if (startRow == endrow)
            {
                DataTable dtt = Common.incUnitTest.GetOneValueDatatable(STRING_DATASTART + STRING_DATAEND);
                myexcel.SetOrUpdateCellValue(STRFIRST_SHEETNAME, startRow, 1, dtt.Rows[0]);
            }
            else
            {
                DataTable dtt = Common.incUnitTest.GetOneValueDatatable(STRING_DATASTART);
                myexcel.SetOrUpdateCellValue(STRFIRST_SHEETNAME, startRow, 1, dtt.Rows[0]);
                DataTable dtt2 = Common.incUnitTest.GetOneValueDatatable(STRING_DATAEND);
                myexcel.SetOrUpdateCellValue(STRFIRST_SHEETNAME, endrow, 1, dtt2.Rows[0]);
            }
        }


        //todo remove colume
        private static Dictionary<uint, Dictionary<string, uint>> GetTableDataStyles(Common.MyExcelFile myexcel, uint startRowIndex, uint lastRow, uint newDataRowCount)
        {
            //
            Dictionary<uint, Dictionary<string, uint>> tableStyle = new Dictionary<uint, Dictionary<string, uint>>();

            if (startRowIndex >= 0)
            {
                Row titleRow = myexcel.GetRow(STRFIRST_SHEETNAME, startRowIndex);
                if (titleRow != null)
                {
                    Dictionary<string, uint> titleStyles = Common.MyExcelFile.getRowStyles(titleRow, startRowIndex);
                    tableStyle.Add(startRowIndex, titleStyles);
                }
            }
            return tableStyle;
        }
        #endregion


        //inner class
        public class ReportArgument
        {
            IList<int> columnsIndex_total;
        }
    }
}