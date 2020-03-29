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
                writtingRowNo++;
                int minIndex = columnsIndex.Min();
                minIndex = minIndex == 0 ? 0 : minIndex - 1;
                for (int i = 0; i < columnsIndex.Count; i++)
                {
                    int columnIndex = columnsIndex[i];
                    if (columnIndex <= dataTable.Columns.Count - 1)
                    {
                        string tempTotal = GetTotal(dataTable, columnIndex);
                        if (columnIndex == minIndex)
                        {
                            tempTotal = "Total " + tempTotal;
                            myExcelFile.CreateOrUpdateCellAt(sheetName, writtingRowNo, (uint)(columnIndex + 1 + 1), typeof(string), tempTotal);//index->no:+1. flag:+1
                        }
                        else
                        {
                            myExcelFile.CreateOrUpdateCellAt(sheetName, writtingRowNo, (uint)(columnIndex + 1 + 1), typeof(decimal), tempTotal);
                        }
                    }
                }

                if (!columnsIndex.Contains(minIndex))
                {
                    myExcelFile.CreateOrUpdateCellAt(sheetName, writtingRowNo, (uint)minIndex+1+1, typeof(string), "Total");
                }
                writtingRowNo++;
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
                return resd.ToString();
            }
            catch {
                return "";
            }
            
        }
    }
}