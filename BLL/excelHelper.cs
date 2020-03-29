using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CUSTOMRP.BLL
{
    //todo 最后需要 看下GetCellReference 是否是row ,column.
    //todo total,
    public abstract class ExcelHelper
    {
        private static readonly string SHEETNAME = "Report";
        private static readonly string STRING_DATASTART = "_DATASTART";
        private static readonly string STRING_DATAEND = "_DATAEND";

        public static bool CreateReport(string path, DataTable dataTable)
        {
            using (Common.IncOpenExcel incOpenExcel = new Common.IncOpenExcel(path, SHEETNAME,true))
            {
                if (dataTable != null)
                {
                    uint startRowNo = 1;
                    uint startColumnNo = 2;
                    uint writingRowNo = startRowNo;
                    incOpenExcel.CreateOrUpdateRowsAt(SHEETNAME, dataTable, startRowNo, startColumnNo, null);
                    writingRowNo += (uint)dataTable.Rows.Count;
                    List<int> ll = new List<int>();
                    ll.Add(0);
                    ll.Add(1);
                    ll.Add(2);
                    ll.Add(3);
                    ll.Add(5);
                    ll.Add(15);
                    InsertSpecialPart_reportTotal.FillSomeData(incOpenExcel, dataTable, SHEETNAME, ll,ref writingRowNo);
                    writingRowNo++;
                    SetDataFlag(startRowNo, writingRowNo, incOpenExcel, SHEETNAME);
                }
            }
            return true;
        }


        public static bool UpdateReport(string path, DataTable dataTable)
        {
            using (Common.IncOpenExcel incOpenExcel = new Common.IncOpenExcel(path, SHEETNAME, false))
            {
                if (dataTable != null)
                {
                    //0.get style of report 1.save bottom  2.delete all from data  3.append data 4. get offset and change boxxom 5.append bottom
                    uint startRowNo, endRowNo;
                    GetDataFlag(incOpenExcel, out startRowNo, out endRowNo, SHEETNAME);

                    Dictionary<uint, uint> titleStyle = Common.IncOpenExcel.getRowStyles(incOpenExcel.GetRow(SHEETNAME, startRowNo));
                    Dictionary<uint, Dictionary<uint, uint>> reportStyle = new Dictionary<uint, Dictionary<uint, uint>>();
                    reportStyle.Add(startRowNo, titleStyle);

                    List<string> bottomXmls = incOpenExcel.GetRowsXml(SHEETNAME, endRowNo + 1, uint.MaxValue);
                    incOpenExcel.DeleteRows(SHEETNAME, startRowNo, uint.MaxValue);
                    incOpenExcel.CreateOrUpdateRowsAt(SHEETNAME, dataTable, startRowNo, 2, reportStyle);
                    int offset = dataTable.Rows.Count + 1 - (int)(endRowNo - startRowNo + 1);
                    incOpenExcel.MoveRows(SHEETNAME, bottomXmls, offset);
                    SetDataFlag(startRowNo, startRowNo + (uint)dataTable.Rows.Count, incOpenExcel, SHEETNAME);
                }
            }
            return true;
        }

        private static void SetDataFlag(uint startRow, uint endrow, Common.IncOpenExcel myexcel, string sheetName)
        {
            if (startRow == endrow)
            {
                myexcel.CreateOrUpdateCellAt(sheetName, startRow, 1, typeof(string), STRING_DATASTART+ STRING_DATAEND);
            }
            else
            {
                myexcel.CreateOrUpdateCellAt(sheetName, startRow, 1, typeof(string),STRING_DATASTART);
                myexcel.CreateOrUpdateCellAt(sheetName, endrow, 1, typeof(string), STRING_DATAEND);
            }
        }

        private static void GetDataFlag(Common.IncOpenExcel myexcel, out uint startRowNo, out uint endRowNo, string sheetName)
        {
            startRowNo = myexcel.FindStringInColumn(sheetName, STRING_DATASTART, 1);
            endRowNo = myexcel.FindStringInColumn(sheetName, STRING_DATAEND, 1);
            if (startRowNo == 0 && endRowNo == 0)//can't find.
            {
                startRowNo = endRowNo =myexcel.FindStringInColumn(sheetName, STRING_DATASTART + STRING_DATAEND, 1);
            }
        }

        private static Dictionary<uint, Dictionary<uint, uint>> getRerportStyle(Common.IncOpenExcel myexcel, DataTable dataTable)
        {
            Dictionary<uint, Dictionary<uint, uint>> res = new Dictionary<uint, Dictionary<uint, uint>>();



            return res;
        }
    }
}