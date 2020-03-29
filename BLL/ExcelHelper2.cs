using Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CUSTOMRP.BLL
{
    public abstract class InsertSpecialPart_reportTotal
    {
        public static Common.IncOpenExcel FillSomeData(IncOpenExcel myExcelFile,  DataTable dataTable,string sheetName, List<int> columnsIndex,ref uint writtingRowNo)
        {
            if (myExcelFile!=null && dataTable != null && sheetName != null &&  columnsIndex != null && columnsIndex.Count > 0)
            {
                writtingRowNo = writtingRowNo + 1;
                for (int i = 0; i < columnsIndex.Count; i++)
                {
                    int columnIndex = columnsIndex[i];
                    if (columnIndex <= dataTable.Columns.Count - 1)
                    {
                        string tempTotal = GetTotal(dataTable, columnIndex);
                        myExcelFile.CreateOrUpdateCellAt(sheetName, writtingRowNo, (uint)(columnIndex+1+1), typeof(decimal), tempTotal);
                    }
                }
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