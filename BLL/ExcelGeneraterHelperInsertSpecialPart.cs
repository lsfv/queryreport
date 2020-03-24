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
}