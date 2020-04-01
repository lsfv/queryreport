using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CUSTOMRP.BLL
{
    //todo 最后需要 看下GetCellReference 是否是row ,column.
    public abstract class ExcelHelper
    {
        private static readonly string SHEETNAME = "Report";
        private static readonly string STRING_DATASTART = "_DATASTART";
        private static readonly string STRING_DATAEND = "_DATAEND";

        public static bool CreateReport(string path, DataTable dataTable, List<int> totalColumn)
        {
            using (Common.IncOpenExcel incOpenExcel = new Common.IncOpenExcel(path, SHEETNAME,true))
            {
                if (dataTable != null)
                {
                    //init variable
                    uint startRowNo = 1;
                    uint startColumnNo = 2;
                    uint writingRowNo = startRowNo;
                    bool hasTotal = totalColumn == null ? false : totalColumn.Count > 0 ? true : false;

                    //insert databable
                    CreateOrUpdateRowsAt(incOpenExcel, SHEETNAME, incOpenExcel.defaultCellStyle, dataTable, startRowNo, startColumnNo, null);
                    writingRowNo += (uint)dataTable.Rows.Count+1;

                    //insert total
                    InsertSpecialPart_reportTotal.FillSomeData(incOpenExcel, dataTable, SHEETNAME, totalColumn, ref writingRowNo,incOpenExcel.defaultCellStyle);

                    //insert dataFlag
                    writingRowNo--;
                    SetDataFlag(startRowNo, writingRowNo, incOpenExcel, SHEETNAME);
                }
            }
            return true;
        }


        public static bool UpdateReport(string path, DataTable dataTable, List<int> totalColumn)
        {
            using (Common.IncOpenExcel incOpenExcel = new Common.IncOpenExcel(path, SHEETNAME, false))
            {
                if (dataTable != null)
                {
                    //0.get style of report 1.save bottom  2.delete all from data  3.append data 4. get offset and change boxxom 5.append bottom
                    ////init variable
                    uint startRowNo, endRowNo, writingRowNo;
                    uint startColumnNo = 2;
                    GetDataFlag(incOpenExcel, out startRowNo, out endRowNo, SHEETNAME);
                    writingRowNo = startRowNo;
                    bool hasTotal = totalColumn == null ? false : totalColumn.Count > 0 ? true : false;
                    Dictionary<uint, uint> titleStyle = Common.IncOpenExcel.getRowStyles(incOpenExcel.GetRow(SHEETNAME, startRowNo));
                    
                    //delete predata
                    List<string> bottomXmls = incOpenExcel.GetRowsXml(SHEETNAME, endRowNo + 1, uint.MaxValue);
                    incOpenExcel.DeleteRows(SHEETNAME, startRowNo, uint.MaxValue);

                    

                    //insert databable
                    CreateOrUpdateRowsAt(incOpenExcel, SHEETNAME, incOpenExcel.defaultCellStyle, dataTable, startRowNo, startColumnNo, titleStyle);
                    writingRowNo += (uint)dataTable.Rows.Count + 1;

                    //insert total
                    InsertSpecialPart_reportTotal.FillSomeData(incOpenExcel, dataTable, SHEETNAME, totalColumn, ref writingRowNo, incOpenExcel.defaultCellStyle);

                    //insert dataFlag
                    writingRowNo--;
                    SetDataFlag(startRowNo, writingRowNo, incOpenExcel, SHEETNAME);

                    //append bottom
                    writingRowNo++;
                    incOpenExcel.MoveRows(SHEETNAME, bottomXmls, writingRowNo);

                    //clear value which contain formule
                    incOpenExcel.ClearFormulaCache(SHEETNAME);
                }
            }
            return true;
        }

        private static bool CreateOrUpdateRowsAt(Common.IncOpenExcel excelFile,string sheetName, Common.IncOpenExcel.DefaultCellStyle defaultCellStyle, DataTable dataTable, uint rowNo, uint columnNo, Dictionary<uint, uint> titleStyle)
        {
            if (excelFile!=null && sheetName != null && dataTable != null && defaultCellStyle != null)
            {
                DataTable columnsTable = Common.IncOpenExcel.GetColumnsNames(dataTable);

                if (titleStyle == null)
                {
                    titleStyle = Common.IncOpenExcel.getRowStyles(columnsTable.Columns, columnNo, 3, defaultCellStyle);//todo blacked here
                }
                excelFile.CreateOrUpdateRowAt(sheetName, columnsTable.Rows[0], rowNo, columnNo, titleStyle);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Dictionary<uint, uint> tempRowStyle = Common.IncOpenExcel.getRowStyles(dataTable.Columns, columnNo, 2, defaultCellStyle);
                    excelFile.CreateOrUpdateRowAt(sheetName, dataTable.Rows[i], rowNo + (uint)(i + 1), columnNo, tempRowStyle);
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

    }
}