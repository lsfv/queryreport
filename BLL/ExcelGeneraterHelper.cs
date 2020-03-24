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
    public abstract class ExcelGeneraterHelper
    {
        public readonly static string STRING_DATASTART = "_DATASTART";
        public readonly static string STRING_DATAEND = "_DATAEND";


        #region public function
        public static bool GenerateXlsxExcel(string filePath, ExcelReportInfo excelReportInfo)
        {
            bool res = false;
            string errMsg;
            if (excelReportInfo.dataTable != null && excelReportInfo.sheetName!=null)
            {
                using (Common.incOpenXml myexcel = new Common.incOpenXml(filePath, true, excelReportInfo.sheetName, out errMsg))
                {
                    res= InsertDataToReprt(myexcel, excelReportInfo);
                }
            }
            return res;
        }


        public static bool UpdataData4XlsxExcel(string filePath, ExcelReportInfo excelReportInfo)
        {
            //0. find range of data  1.remove pre date and bottom 2.insert data 3 append pre bottom 4.update pivotalbe.
            bool res = false;
            string errMsg;
            if (excelReportInfo.dataTable != null && excelReportInfo.sheetName != null)
            {
                using (Common.incOpenXml myexcel = new Common.incOpenXml(filePath, out errMsg))
                {
                    uint startRowIndex, endRowIndex;
                    FindDataRange(myexcel, out startRowIndex, out endRowIndex, excelReportInfo.sheetName);
                    

                    if (endRowIndex >= startRowIndex && startRowIndex > 0)//when it  has  'data_start' and 'data_end'
                    {
                        excelReportInfo.startRowIndex = startRowIndex;
                        excelReportInfo.writingRowIndex = startRowIndex;
                        //get pre styles of data.
                        Dictionary<uint, Dictionary<string, uint>> tableStyle = GetTableDataStyles(myexcel, startRowIndex, endRowIndex, (UInt32)excelReportInfo.dataTable.Rows.Count, excelReportInfo.sheetName);
                        excelReportInfo.tableStyle = tableStyle;
                        //get pre buttom rows and save.
                        IEnumerable<Row> rows_bottom = myexcel.GetRangeRows(excelReportInfo.sheetName, endRowIndex + 1, UInt32.MaxValue);
                        List<string> bottomRows = new List<string>();
                        foreach (Row row in rows_bottom)
                        {
                            bottomRows.Add(row.OuterXml);
                        }

                        //reomve all rows begin from data
                        myexcel.RemoveRows(excelReportInfo.sheetName, startRowIndex, UInt32.MaxValue);

                        //insert new data;
                        InsertDataToReprt(myexcel, excelReportInfo);

                        //append pre bottom
                        foreach (string strRow in bottomRows)
                        {
                            Row newRow = new Row(strRow);
                            myexcel.AppendRowAndupdateRowReference(excelReportInfo.sheetName, newRow, excelReportInfo.writingRowIndex, null);
                            excelReportInfo.writingRowIndex++;
                        }

                        //update pivotTable
                        //todo 不能添加group信息.否则透视表就无用了. 
                        string startRef = Common.incOpenXml.GetCellReference((uint)2, startRowIndex);
                        string endRef = Common.incOpenXml.GetCellReference((uint)2 + (uint)excelReportInfo.dataTable.Columns.Count - 1, startRowIndex + (uint)excelReportInfo.dataTable.Rows.Count);
                        myexcel.UpdateAllPivotSource(excelReportInfo.sheetName, startRef, endRef);
                    }
                    else//when it does't has  'data_start' and 'data_end'
                    {
                        excelReportInfo.startRowIndex = 1;
                        excelReportInfo.writingRowIndex = 1;
                        myexcel.RemoveAllRows(excelReportInfo.sheetName);
                        InsertDataToReprt(myexcel, excelReportInfo);

                        //update pivotTable
                        string startRef = Common.incOpenXml.GetCellReference((uint)2, 1);
                        string endRef = Common.incOpenXml.GetCellReference((uint)2 + (uint)excelReportInfo.dataTable.Columns.Count - 1, (uint)excelReportInfo.dataTable.Rows.Count + 1);
                        myexcel.UpdateAllPivotSource(excelReportInfo.sheetName, startRef, endRef);
                    }
                    res = true;
                }

            }
            return res;
        }

        #endregion

        #region private function
        private static bool InsertDataToReprt(incOpenXml myexcel, ExcelReportInfo excelReportInfo)
        {
            if (excelReportInfo.dataTable != null)
            {
                List<InsertSpecialPart> fillReportList = SetupInsertSteps(myexcel, excelReportInfo);

                foreach (InsertSpecialPart fillReport_node in fillReportList)
                {
                    fillReport_node.FillSomeData();
                }
            }
            uint startGuard = excelReportInfo.startRowIndex;
            uint endGuard = excelReportInfo.startRowIndex == excelReportInfo.writingRowIndex ? startGuard : excelReportInfo.writingRowIndex - 1;
            setGuard(startGuard, endGuard, myexcel,excelReportInfo.sheetName);
            return true;
        }

        //把所有的修改集中到这个配置方法，并且采用可扩展方式（建立新类）来完成功能。是否需要改成装饰模式 或者指责链模式?
        private static List<InsertSpecialPart> SetupInsertSteps(Common.incOpenXml file, ExcelReportInfo excelReportInfo)
        {
            //1.插入数据。2插入subtotal.
            List<InsertSpecialPart> list = new List<InsertSpecialPart>();
            InsertSpecialPart_Table fillReport_Table = new InsertSpecialPart_Table(excelReportInfo,file);
            list.Add(fillReport_Table);

            //StatisticFuntion_subTotal fillReport_SubTotal = new StatisticFuntion_subTotal(file, argument, baseArgument);
            //list.Add(fillReport_SubTotal);
            return list;
        }


        private static void setGuard(uint startRow, uint endrow, Common.incOpenXml myexcel, string sheetName)
        {
            if (startRow == endrow)
            {
                DataTable dtt = Common.incUnitTest.GetOneValueDatatable(STRING_DATASTART + STRING_DATAEND);
                myexcel.SetOrUpdateCellValue(sheetName, startRow, 1, dtt.Rows[0]);
            }
            else
            {
                DataTable dtt = Common.incUnitTest.GetOneValueDatatable(STRING_DATASTART);
                myexcel.SetOrUpdateCellValue(sheetName, startRow, 1, dtt.Rows[0]);
                DataTable dtt2 = Common.incUnitTest.GetOneValueDatatable(STRING_DATAEND);
                myexcel.SetOrUpdateCellValue(sheetName, endrow, 1, dtt2.Rows[0]);
            }
        }

        private static void FindDataRange(Common.incOpenXml myexcel, out uint startRowIndex, out uint endRowIndex,string sheetName)
        {
            startRowIndex = myexcel.GetRowIndexFromAColumn(sheetName, STRING_DATASTART, 1);
            endRowIndex = myexcel.GetRowIndexFromAColumn(sheetName, STRING_DATAEND, 1);
            if (startRowIndex == 0 && endRowIndex == 0)
            {
                startRowIndex = myexcel.GetRowIndexFromAColumn(sheetName, STRING_DATASTART + STRING_DATAEND, 1);
                endRowIndex = startRowIndex;
            }
        }

        //现在只获得了表头的样式,本意是根据现有样式,来推导完成整个新表格的样式.应该是不可能了,因为中间包含很多统计信息,无法在excel自定义信息了,所以无法推导.
        private static Dictionary<uint, Dictionary<string, uint>> GetTableDataStyles(Common.incOpenXml myexcel, uint startRowIndex, uint lastRow, uint newDataRowCount,string sheetName)
        {
            Dictionary<uint, Dictionary<string, uint>> tableStyle = new Dictionary<uint, Dictionary<string, uint>>();

            if (startRowIndex >= 0)
            {
                Row titleRow = myexcel.GetRow(sheetName, startRowIndex);
                if (titleRow != null)
                {
                    Dictionary<string, uint> titleStyles = Common.incOpenXml.getRowStyles(titleRow, startRowIndex);
                    tableStyle.Add(startRowIndex, titleStyles);
                }
            }
            return tableStyle;
        }

        #endregion
    }


    #region ExcelReportInfo
    public class ExcelReportInfo
    {
        public DataTable dataTable;
        public string sheetName;
        public uint startRowIndex;
        public Dictionary<uint, Dictionary<string, uint>> tableStyle;
        public uint writingRowIndex;

        public StatisticColumns statistic_reportTotal;
        public StatisticColumns statistic_reportAVG;
        public StatisticColumns statistic_reportCount;

        public StatisticColumns statistic_groupTotal;
        public StatisticColumns statistic_groupAVG;
        public StatisticColumns statistic_groupCount;

        public ExcelReportInfo(DataTable dataTable,  uint startRowIndex, Dictionary<uint, Dictionary<string, uint>> tableStyle,StatisticColumns statistic_reportTotal, StatisticColumns statistic_reportAVG, StatisticColumns statistic_reportCount, StatisticColumns statistic_groupTotal, StatisticColumns statistic_groupAVG, StatisticColumns statistic_groupCount)
        {
            this.dataTable = dataTable;
            this.sheetName = "Report";
            this.startRowIndex = startRowIndex;
            this.tableStyle = tableStyle;
            this.writingRowIndex = startRowIndex;
            this.statistic_reportTotal = statistic_reportTotal;
            this.statistic_reportAVG = statistic_reportAVG;
            this.statistic_reportCount = statistic_reportCount;
            this.statistic_groupTotal = statistic_groupTotal;
            this.statistic_groupAVG = statistic_groupAVG;
            this.statistic_groupCount = statistic_groupCount;
        }
    }

    public enum Enum_StatisitcType
    {
        reportTotal,
        reportAVG,
        reportCount,
        groupTotal,
        groupAVG,
        groupCount
    }

    public class StatisticColumns
    {
        public Enum_StatisitcType statisitcType;
        public IList<int> columnsIndex;

        public StatisticColumns(Enum_StatisitcType type, IList<int> columns)
        {
            statisitcType = type;
            columnsIndex = columns;
        }
    }
    #endregion
}