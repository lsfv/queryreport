using Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CUSTOMRP.BLL
{
    public abstract class StatisticFuntion
    {
        protected Common.incOpenXml myExcelFile;
        protected ExcelGeneraterHelper.ReportBaseInfo reportBaseArgument;
        protected ExcelGeneraterHelper.ReportStatisticInfo reportArgument;

        public StatisticFuntion(Common.incOpenXml file, ExcelGeneraterHelper.ReportStatisticInfo argument, ExcelGeneraterHelper.ReportBaseInfo BaseArgument)
        {
            myExcelFile = file;
            reportArgument = argument;
            reportBaseArgument = BaseArgument;
        }

        public abstract Common.incOpenXml FillSomeData();

    }

    public class StatisticFuntion_Table : StatisticFuntion
    {
        public StatisticFuntion_Table(incOpenXml file, ExcelGeneraterHelper.ReportStatisticInfo argument, ExcelGeneraterHelper.ReportBaseInfo BaseArgument) : base(file, argument, BaseArgument)
        { }

        public override incOpenXml FillSomeData()
        {
            myExcelFile.SetOrReplaceRows(reportBaseArgument.sheetName_data, reportBaseArgument.writingRowIndex, 2, reportBaseArgument.dataTable, reportBaseArgument.tableStyle);
            reportBaseArgument.writingRowIndex += (uint)reportBaseArgument.dataTable.Rows.Count + 1;
            return myExcelFile;
        }
    }

    public class StatisticFuntion_subTotal : StatisticFuntion
    {
        public StatisticFuntion_subTotal(incOpenXml file, ExcelGeneraterHelper.ReportStatisticInfo argument, ExcelGeneraterHelper.ReportBaseInfo BaseArgument) : base(file, argument, BaseArgument)
        { }

        public override incOpenXml FillSomeData()
        {
            if (reportArgument != null && reportArgument.Statistics_total != null && reportArgument.Statistics_total.columnsIndex.Count()>0)
            {
                reportBaseArgument.writingRowIndex++;//space
                for (int i = 0; i < reportArgument.Statistics_total.columnsIndex.Count; i++)
                {
                    int columnIndex = reportArgument.Statistics_total.columnsIndex[i];
                    string tempTotal = GetTotal(reportBaseArgument.dataTable, columnIndex);
                    DataTable table = Common.incOpenXml.GetDatatableSingleValue(tempTotal);//todo need  reconstrct,data tabie is too heigh, use string?
                    myExcelFile.SetOrUpdateCellValue(reportBaseArgument.sheetName_data, reportBaseArgument.writingRowIndex, (uint)columnIndex + 1 + 1, table.Rows[0], myExcelFile.defaultCellStyle.blackIndex);
                }

                string FirstColumn = myExcelFile.GetCellRealString(reportBaseArgument.sheetName_data, reportBaseArgument.writingRowIndex, 2);
                if (string.IsNullOrWhiteSpace(FirstColumn))
                {
                    FirstColumn = reportArgument.Statistics_total.name;
                }
                else
                {
                    FirstColumn = reportArgument.Statistics_total.name + ":" + FirstColumn;
                }

                DataTable table2 = Common.incOpenXml.GetDatatableSingleValue(FirstColumn);//todo need  reconstrct,data tabie is too heigh, use string?
                myExcelFile.SetOrUpdateCellValue(reportBaseArgument.sheetName_data, reportBaseArgument.writingRowIndex, (uint)2, table2.Rows[0], myExcelFile.defaultCellStyle.blackIndex);


                reportBaseArgument.writingRowIndex++;
            }
            return myExcelFile;
        }

        public string GetTotal(DataTable table, int columnIndex)
        {
            decimal resd = 0;
            try
            {
                foreach (DataRow row in table.Rows)
                {
                    resd += decimal.Parse(row[columnIndex].ToString());
                }
            }
            catch { }
            return resd.ToString();
        }

    }



    public class StatisticFuntion_Avg : StatisticFuntion
    {
        public StatisticFuntion_Avg(incOpenXml file, ExcelGeneraterHelper.ReportStatisticInfo argument, ExcelGeneraterHelper.ReportBaseInfo BaseArgument) : base(file, argument, BaseArgument)
        { }

        public override incOpenXml FillSomeData()
        {
            if (reportArgument != null && reportArgument.Statistics_total != null)
            {
                reportBaseArgument.writingRowIndex++;//space
                for (int i = 0; i < reportArgument.Statistics_total.columnsIndex.Count; i++)
                {
                    int columnIndex = reportArgument.Statistics_total.columnsIndex[i];
                    string tempTotal = GetTotal(reportBaseArgument.dataTable, columnIndex);
                    DataTable table = Common.incOpenXml.GetDatatableSingleValue(tempTotal);//todo need  reconstrct,data tabie is too heigh, use string?
                    myExcelFile.SetOrUpdateCellValue(reportBaseArgument.sheetName_data, reportBaseArgument.writingRowIndex, (uint)columnIndex + 1 + 1, table.Rows[0], myExcelFile.defaultCellStyle.blackIndex);
                }

                string FirstColumn = myExcelFile.GetCellRealString(reportBaseArgument.sheetName_data, reportBaseArgument.writingRowIndex, 2);
                if (string.IsNullOrWhiteSpace(FirstColumn))
                {
                    FirstColumn = reportArgument.Statistics_total.name;
                }
                else
                {
                    FirstColumn = reportArgument.Statistics_total.name + ":" + FirstColumn;
                }

                DataTable table2 = Common.incOpenXml.GetDatatableSingleValue(FirstColumn);//todo need  reconstrct,data tabie is too heigh, use string?
                myExcelFile.SetOrUpdateCellValue(reportBaseArgument.sheetName_data, reportBaseArgument.writingRowIndex, (uint)2, table2.Rows[0], myExcelFile.defaultCellStyle.blackIndex);


                reportBaseArgument.writingRowIndex++;
            }
            return myExcelFile;
        }

        public string GetTotal(DataTable table, int columnIndex)
        {
            decimal resd = 0;
            try
            {
                foreach (DataRow row in table.Rows)
                {
                    resd += decimal.Parse(row[columnIndex].ToString());
                }
            }
            catch { }
            return resd.ToString();
        }

    }


}