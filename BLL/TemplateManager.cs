using Common;
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
    //TemplateManager 是一个报表生成的帮助类，是上层逻辑类。而 incopenxml.cs是一个基础类库，不要随意加方法,要保持基础通用可移植性。
    public abstract class TemplateManager
    {
        public const string STRFIRST_SHEETNAME = "Report";
        public const string STRING_DATASTART = "_DATASTART";
        public const string STRING_DATAEND = "_DATAEND";


        #region public
        public static bool GenerateXlsxExcel(DataTable dataTable, out string errMsg, string filePath, ReportArgument reportArgument)
        {
            bool res = false;
            errMsg = "";
            if (dataTable != null)
            {
                using (Common.MyExcelFile myexcel = new Common.MyExcelFile(filePath, true, STRFIRST_SHEETNAME, out errMsg))
                {
                    ReportBaseArgument baseArgument = new ReportBaseArgument(1, STRFIRST_SHEETNAME, dataTable, null);
                    InsertDataWithReprtArgument(myexcel, reportArgument, baseArgument);
                    res = true;
                }
            }
            return res;
        }


        public static bool UpdataData4XlsxExcel(DataTable dataTable, out string errMsg, string filePath, ReportArgument reportArgument)
        {
            //0. find range of data  1.remove pre date and bottom 2.insert data 3 append pre bottom 4.update pivotalbe.
            bool res = true;
            using (Common.MyExcelFile myexcel = new Common.MyExcelFile(filePath, out errMsg))
            {
                uint startRowIndex, endRowIndex;
                FindDataRange(myexcel, out startRowIndex, out endRowIndex);

                if (endRowIndex >= startRowIndex && startRowIndex > 0)//when it  has  'data_start' and 'data_end'
                {
                    //get pre styles of data.
                    Dictionary<uint, Dictionary<string, uint>> tableStyle = GetTableDataStyles(myexcel, startRowIndex, endRowIndex, (UInt32)dataTable.Rows.Count);
                    //get pre buttom rows and save.
                    IEnumerable<Row> rows_bottom = myexcel.GetRangeRows(STRFIRST_SHEETNAME, endRowIndex + 1, UInt32.MaxValue);
                    List<string> bottomRows = new List<string>();
                    foreach (Row row in rows_bottom)
                    {
                        bottomRows.Add(row.OuterXml);
                    }
                   
                    //reomve all rows begin from data
                    myexcel.RemoveRows(STRFIRST_SHEETNAME, startRowIndex, UInt32.MaxValue);

                    //insert new data;
                    ReportBaseArgument baseArgument = new ReportBaseArgument(startRowIndex, STRFIRST_SHEETNAME, dataTable, tableStyle);
                    InsertDataWithReprtArgument(myexcel,reportArgument,baseArgument);

                    //append pre bottom
                    foreach (string strRow in bottomRows)
                    {
                        Row newRow = new Row(strRow);
                        myexcel.AppendRowAndupdateRowReference(STRFIRST_SHEETNAME, newRow, baseArgument.writingRowIndex, null);
                        baseArgument.writingRowIndex++;
                    }

                    //update pivotTable
                    string startRef = Common.MyExcelFile.GetCellReference((uint)2, startRowIndex);
                    string endRef = Common.MyExcelFile.GetCellReference((uint)2 + (uint)dataTable.Columns.Count - 1, startRowIndex + (uint)dataTable.Rows.Count);
                    myexcel.UpdateAllPivotSource(STRFIRST_SHEETNAME, startRef, endRef);
                }
                else//when it does't has  'data_start' and 'data_end'
                {
                    myexcel.RemoveAllRows(STRFIRST_SHEETNAME);
                    ReportBaseArgument baseArgument = new ReportBaseArgument(1, STRFIRST_SHEETNAME, dataTable, null);
                    InsertDataWithReprtArgument(myexcel, reportArgument, baseArgument);

                    //update pivotTable
                    string startRef = Common.MyExcelFile.GetCellReference((uint)2, 1);
                    string endRef = Common.MyExcelFile.GetCellReference((uint)2 + (uint)dataTable.Columns.Count - 1, (uint)dataTable.Rows.Count + 1);
                    myexcel.UpdateAllPivotSource(STRFIRST_SHEETNAME, startRef, endRef);
                }
            }
            return res;
        }
        #endregion



        #region private
        private static void InsertDataWithReprtArgument(Common.MyExcelFile myexcel,ReportArgument reportArgument,ReportBaseArgument baseArgument)
        {
            if (baseArgument.dataTable != null)
            {
                List<FillReport> fillReportList = new List<FillReport>();

                ConfigNodes_fillReport(fillReportList,myexcel,reportArgument,baseArgument);
                foreach (FillReport fillReport_node in fillReportList)
                {
                    fillReport_node.FillSomeData();
                }
            }
            uint startGuard = baseArgument.startRowIndex;
            uint endGuard = baseArgument.startRowIndex == baseArgument.writingRowIndex ? startGuard : baseArgument.writingRowIndex - 1;
            setGuard(startGuard, endGuard, myexcel);
        }

        //把所有的修改集中到这个配置方法，并且采用可扩展方式（建立新类）来完成功能。是否需要改成装饰模式 或者指责链模式?
        private static void ConfigNodes_fillReport(List<FillReport> list, Common.MyExcelFile file, ReportArgument argument, ReportBaseArgument baseArgument)
        {
            //1.插入数据。2插入subtotal.

            FillReport_Table fillReport_Table = new FillReport_Table(file, argument, baseArgument);
            FillReport_subTotal fillReport_SubTotal = new FillReport_subTotal(file, argument, baseArgument);
            list.Add(fillReport_Table);
            list.Add(fillReport_SubTotal);
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

        #region innclass
        public class ReportBaseArgument
        {
            public uint startRowIndex;
            public uint writingRowIndex;
            public string writingSheetName;
            public DataTable dataTable;
            public Dictionary<uint, Dictionary<string, uint>> tableStyle;

            public ReportBaseArgument(uint _startIndex, string _name,DataTable _table, Dictionary<uint, Dictionary<string, uint>> _style)
            {
                startRowIndex = _startIndex;
                writingRowIndex = _startIndex;
                writingSheetName = _name;
                dataTable = _table;
                tableStyle = _style;
            }
        }

        public class ReportArgument
        {
            public ReportArgument_ReportStatistics Statistics_total;
        }

        public abstract class ReportArgument_ReportStatistics
        {
            public string name;
            public IList<int> columnsIndex;
        }

        public class ReportArgument_ReportStatistics_total:ReportArgument_ReportStatistics
        {
            private ReportArgument_ReportStatistics_total() { }
            public ReportArgument_ReportStatistics_total(IList<int>  indexs)
            {
                name = "Total";
                columnsIndex = indexs;
            }
        }

        public class ReportArgument_ReportStatistics_Avg : ReportArgument_ReportStatistics
        {
            private ReportArgument_ReportStatistics_Avg() { }
            public ReportArgument_ReportStatistics_Avg(IList<int> indexs)
            {
                name = "AVG";
                columnsIndex = indexs;
            }
        }

        public class ReportArgument_ReportStatistics_count : ReportArgument_ReportStatistics
        {
            private ReportArgument_ReportStatistics_count() { }
            public ReportArgument_ReportStatistics_count(IList<int> indexs)
            {
                name = "count";
                columnsIndex = indexs;
            }
        }
        #endregion
    }
}