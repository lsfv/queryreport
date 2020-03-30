using Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CUSTOMRP.BLL
{
    public abstract class InsertSpecialPart
    {
        public abstract Common.IncOpenExcel FillSomeData();
    }

    public abstract class InsertSpecialPart_datatable : InsertSpecialPart
    {

    }

    public abstract class InsertSpecialPart_reportTotal:InsertSpecialPart
    {
        public static Common.IncOpenExcel FillSomeData(IncOpenExcel myExcelFile, DataTable dataTable, string sheetName, List<int> columnsIndex, ref uint writtingRowNo,Common.IncOpenExcel.DefaultCellStyle defaultCellStyle)
        {
            if (myExcelFile != null && dataTable != null && sheetName != null && columnsIndex != null && columnsIndex.Count > 0)
            {
                //建立空表，填充对应字段值。create table
                DataTable totalTable = CreateTable(dataTable, columnsIndex);

                //得到样式,并fill report
                Dictionary<uint,uint> rowstyle= Common.IncOpenExcel.getRowStyles(totalTable.Columns, 2, 1, defaultCellStyle);
                myExcelFile.CreateOrUpdateRowAt(sheetName, totalTable.Rows[0], writtingRowNo, 2, rowstyle);
                writtingRowNo++;

                myExcelFile.CreateOrUpdateRowAt(sheetName, totalTable.Rows[1], writtingRowNo, 2, rowstyle);//todo blacked style
                writtingRowNo++;
            }
            return myExcelFile;
        }

        private static DataTable CreateTable(DataTable dataTable, List<int> columnsIndex)
        {
            DataTable totalTable = Common.incUnitTest.GetStringDatatableCustomCount(dataTable.Columns.Count);
            totalTable.Rows.Add(totalTable.NewRow());
            totalTable.Rows.Add(totalTable.NewRow());

            int TotalIndex = columnsIndex.Min() == 0 ? 0 : columnsIndex.Min() - 1;
            for (int i = 0; i < columnsIndex.Count; i++)
            {
                int columnIndex = columnsIndex[i];
                if (columnIndex <= dataTable.Columns.Count - 1)
                {
                    string tempTotal = GetTotal(dataTable, columnIndex);
                    totalTable.Rows[1][columnIndex] = tempTotal;
                }
            }
            totalTable.Rows[1][TotalIndex] = totalTable.Rows[1][TotalIndex].ToString() == "" ? "Total" : "Total " + totalTable.Rows[1][TotalIndex].ToString();
            return totalTable;
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