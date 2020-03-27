using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CUSTOMRP.BLL
{
    public abstract class ExcelHelper
    {
        private static readonly string SHEETNAME = "Report";

        public static bool CreateReport(string path, DataTable dataTable)
        {

            using (Common.IncOpenExcel incOpenExcel = new Common.IncOpenExcel(path, SHEETNAME))
            {
                incOpenExcel.CreateOrUpdateRowsAt(SHEETNAME, dataTable, 1, 2, null);
            }



            return true;
        }




    }
}