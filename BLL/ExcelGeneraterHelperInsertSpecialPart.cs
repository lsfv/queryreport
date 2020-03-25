using Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CUSTOMRP.BLL
{
    public abstract class InsertSpecialPart
    {
        protected Common.incOpenXml myExcelFile;
        protected ExcelReportInfo excelReportInfo;

        public InsertSpecialPart(ExcelReportInfo Info , Common.incOpenXml file)
        {
            excelReportInfo = Info;
            myExcelFile = file;
        }

        public abstract Common.incOpenXml FillSomeData();
    }

    public class InsertSpecialPart_Table : InsertSpecialPart
    {
        public InsertSpecialPart_Table(ExcelReportInfo Info, incOpenXml file) : base(Info, file)
        {
        }

        public override incOpenXml FillSomeData()
        {
            myExcelFile.SetOrReplaceRows(excelReportInfo.sheetName, excelReportInfo.writingRowIndex, 2, excelReportInfo.dataTable, excelReportInfo.tableStyle);
            excelReportInfo.writingRowIndex += (uint)excelReportInfo.dataTable.Rows.Count + 1;
            return myExcelFile;
        }
    }


    public class InsertSpecialPart_reportTotal : InsertSpecialPart
    {
        public InsertSpecialPart_reportTotal(ExcelReportInfo Info, incOpenXml file) : base(Info, file)
        {
        }

        public override incOpenXml FillSomeData()
        {
            if (excelReportInfo!=null && excelReportInfo.dataTable!=null && excelReportInfo.sheetName!=null && excelReportInfo.statistic_reportTotal!=null && excelReportInfo.statistic_reportTotal.columnsIndex!=null && excelReportInfo.statistic_reportTotal.columnsIndex.Count>0)
            {
                

                excelReportInfo.writingRowIndex++;//space
                DataRow totalRow = excelReportInfo.dataTable.NewRow();
                for (int i = 0; i < excelReportInfo.statistic_reportTotal.columnsIndex.Count; i++)
                {
                    int columnIndex = excelReportInfo.statistic_reportTotal.columnsIndex[i];
                    string tempTotal = GetTotal(excelReportInfo.dataTable, columnIndex);
                    if (columnIndex <= totalRow.Table.Columns.Count - 1)
                    {
                        totalRow[columnIndex] = tempTotal;
                    }
                }

                Dictionary<string, uint> rowStyle = null;
                try
                {
                    rowStyle = excelReportInfo.tableStyle[excelReportInfo.writingRowIndex];
                }
                catch
                { }

                myExcelFile.SetOrReplaceRow(excelReportInfo.sheetName, excelReportInfo.writingRowIndex, 2, totalRow, rowStyle);


                //string FirstColumn = myExcelFile.GetCellRealString(excelReportInfo.sheetName, excelReportInfo.writingRowIndex, 2);
                //if (string.IsNullOrWhiteSpace(FirstColumn))
                //{
                //    FirstColumn = excelReportInfo.statistic_reportTotal.statisitcType.ToString();
                //}
                //else
                //{
                //    FirstColumn = excelReportInfo.statistic_reportTotal.statisitcType.ToString() + ":" + FirstColumn;
                //}

                //DataTable table2 = Common.incOpenXml.GetDatatableSingleValue(FirstColumn);//todo need  reconstrct,data tabie is too heigh, use string?
                //myExcelFile.SetOrUpdateCellValue(excelReportInfo.sheetName, excelReportInfo.writingRowIndex, (uint)2, table2.Rows[0], myExcelFile.defaultCellStyle.blackIndex);


                excelReportInfo.writingRowIndex++;
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