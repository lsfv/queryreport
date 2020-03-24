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
    public abstract class FillReport
    {
        protected Common.MyExcelFile myExcelFile;
        protected TemplateManager.ReportBaseArgument reportBaseArgument;
        protected TemplateManager.ReportArgument reportArgument;

        public FillReport(Common.MyExcelFile file, TemplateManager.ReportArgument argument, TemplateManager.ReportBaseArgument BaseArgument)
        {
            myExcelFile = file;
            reportArgument = argument;
            reportBaseArgument = BaseArgument;
        }

        public abstract Common.MyExcelFile FillSomeData();
    }

    public class FillReport_Table : FillReport
    {
        public FillReport_Table(MyExcelFile file, TemplateManager.ReportArgument argument, TemplateManager.ReportBaseArgument BaseArgument) : base(file, argument, BaseArgument)
        { }

        public override MyExcelFile FillSomeData()
        {
            myExcelFile.SetOrReplaceRows(reportBaseArgument.writingSheetName, reportBaseArgument.writingRowIndex, 2, reportBaseArgument.dataTable, reportBaseArgument.tableStyle);
            reportBaseArgument.writingRowIndex += (uint)reportBaseArgument.dataTable.Rows.Count + 1;
            return myExcelFile;
        }
    }

    public class FillReport_subTotal : FillReport
    {
        public FillReport_subTotal(MyExcelFile file, TemplateManager.ReportArgument argument, TemplateManager.ReportBaseArgument BaseArgument) : base(file, argument, BaseArgument)
        { }

        public override MyExcelFile FillSomeData()
        {
            if (reportArgument != null && reportArgument.columnsIndex_total != null)
            {
                reportBaseArgument.writingRowIndex++;//space
                for (int i = 0; i < reportArgument.columnsIndex_total.Count; i++)
                {
                    int columnIndex = reportArgument.columnsIndex_total[i];
                    string tempTotal = GetTotal(reportBaseArgument.dataTable, columnIndex);
                    DataTable table = Common.MyExcelFile.GetDatatableSingleValue(tempTotal);//todo need  reconstrct,data tabie is too heigh, use string?
                    myExcelFile.SetOrUpdateCellValue(reportBaseArgument.writingSheetName, reportBaseArgument.writingRowIndex, (uint)columnIndex + 1 + 1, table.Rows[0], myExcelFile.defaultCellStyle.blackIndex);
                }

                string totolFlag = myExcelFile.GetCellRealString(reportBaseArgument.writingSheetName, reportBaseArgument.writingRowIndex, 2);
                if (string.IsNullOrWhiteSpace(totolFlag))
                {
                    totolFlag = "Total";
                }
                else
                {
                    totolFlag = "Total:" + totolFlag;
                }
                DataTable table2 = Common.MyExcelFile.GetDatatableSingleValue(totolFlag);//todo need  reconstrct,data tabie is too heigh, use string?
                myExcelFile.SetOrUpdateCellValue(reportBaseArgument.writingSheetName, reportBaseArgument.writingRowIndex, (uint)2, table2.Rows[0], myExcelFile.defaultCellStyle.blackIndex);


                reportBaseArgument.writingRowIndex++;
            }
            return myExcelFile;
        }

        public MyExcelFile FillReportBY(IList<int> columnsIndex,int type)
        {

            if (columnsIndex != null)
            {
                reportBaseArgument.writingRowIndex++;//space
                for (int i = 0; i < reportArgument.columnsIndex_total.Count; i++)
                {
                    int columnIndex = reportArgument.columnsIndex_total[i];
                    string tempValue = "";//todo if GetTotal(reportBaseArgument.dataTable, columnIndex);
                    DataTable table = Common.MyExcelFile.GetDatatableSingleValue(tempValue);//todo need  reconstrct,data tabie is too heigh, use string?
                    myExcelFile.SetOrUpdateCellValue(reportBaseArgument.writingSheetName, reportBaseArgument.writingRowIndex, (uint)columnIndex + 1 + 1, table.Rows[0], myExcelFile.defaultCellStyle.blackIndex);
                }

                string totolFlag = myExcelFile.GetCellRealString(reportBaseArgument.writingSheetName, reportBaseArgument.writingRowIndex, 2);
                if (string.IsNullOrWhiteSpace(totolFlag))
                {
                    totolFlag = "";//todo if "Total";
                }
                else
                {
                    totolFlag = "";//todo if "Total:" + totolFlag;
                }
                DataTable table2 = Common.MyExcelFile.GetDatatableSingleValue(totolFlag);//todo need  reconstrct,data tabie is too heigh, use string?
                myExcelFile.SetOrUpdateCellValue(reportBaseArgument.writingSheetName, reportBaseArgument.writingRowIndex, (uint)2, table2.Rows[0], myExcelFile.defaultCellStyle.blackIndex);


                reportBaseArgument.writingRowIndex++;
            }
            return myExcelFile;
        }

        public static string GetTotal(DataTable table, int columnIndex)
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
