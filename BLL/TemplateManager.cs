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
    public abstract class TemplateManager
    {
        public const string STRFIRST_SHEETNAME = "Report";
        public const string STRING_DATASTART = "_DATASTART";
        public const string STRING_DATAEND = "_DATAEND";


        public static bool GenerateXlsxExcel(DataTable dataTable, out string errMsg, string filePath)
        {
            bool res = false;
            errMsg = "";
            if (dataTable != null)
            {
                using (Common.MyExcelFile myexcel = new Common.MyExcelFile(filePath, true, STRFIRST_SHEETNAME, out errMsg))
                {
                    myexcel.SetOrReplaceRows(STRFIRST_SHEETNAME, 1, 2, dataTable,null); 
                    if (dataTable != null)
                    {
                        setGuard(1, 1 + (UInt32)dataTable.Rows.Count, myexcel);
                    }
                    else
                    {
                        setGuard(1, 1, myexcel);
                    }
                    res = true;
                }
            }
            return res;
        }


        public static bool UpdataData4XlsxExcel(DataTable dataTable,  out string errMsg, string filePath)
        {
            bool res = false;
            using (Common.MyExcelFile myexcel = new Common.MyExcelFile(filePath, out errMsg))
            {
                UInt32 startRowIndex = myexcel.GetRowIndexFromAColumn(STRFIRST_SHEETNAME, STRING_DATASTART, 1);
                UInt32 endRowIndex = myexcel.GetRowIndexFromAColumn(STRFIRST_SHEETNAME, STRING_DATAEND, 1);

                if (startRowIndex == 0 && endRowIndex == 0)// no data .just has column name.
                {
                    startRowIndex = myexcel.GetRowIndexFromAColumn(STRFIRST_SHEETNAME, STRING_DATASTART + STRING_DATAEND, 1);
                    endRowIndex = startRowIndex;
                }

                if (endRowIndex >= startRowIndex && startRowIndex > 0)//it is a right sheet,and has data.
                {
                    //get data's format style first.
                    Dictionary<uint, Dictionary<string, uint>> tableStyle = new Dictionary<uint, Dictionary<string, uint>>();

                    var titleRows = myexcel.GetRangeRows(STRFIRST_SHEETNAME, startRowIndex, startRowIndex);
                    if (titleRows != null && titleRows.Count() > 0)
                    {
                        Dictionary<string, uint> titleStyles = Common.MyExcelFile.getRowStyles(titleRows.First());
                        tableStyle.Add(startRowIndex,titleStyles);
                    }
                    foreach (string key in tableStyle[startRowIndex].Keys)
                    {
                        Debug.WriteLine("list title sytle:"+key + "." + tableStyle[startRowIndex][key]);
                    }
                    

                    IEnumerable<Row> rows_bottom = myexcel.GetRangeRows(STRFIRST_SHEETNAME, endRowIndex + 1, UInt32.MaxValue);

                    //reomve pre data;
                    myexcel.RemoveRows(STRFIRST_SHEETNAME, startRowIndex, endRowIndex);
                    //update pre bottom data;
                    int deleteCount = (int)(endRowIndex - startRowIndex + 1);
                    int insertCount = dataTable == null ? 1 : dataTable.Rows.Count + 1;
                    int offsetCount = insertCount - deleteCount;
                    Common.MyExcelFile.updateRowIndexAndCellReference(rows_bottom, offsetCount);
                    //insert new data;
                    myexcel.SetOrReplaceRows(STRFIRST_SHEETNAME, startRowIndex, 2, dataTable,tableStyle);
                    if (dataTable != null)
                    {
                        setGuard(startRowIndex, startRowIndex + (UInt32)dataTable.Rows.Count, myexcel);
                    }
                    else
                    {
                        setGuard(startRowIndex, startRowIndex, myexcel);
                    }
                    res = true;

                    //update pivotTable
                    string startRef = Common.MyExcelFile.GetCellReference((uint)2, startRowIndex);
                    string endRef = Common.MyExcelFile.GetCellReference((uint)2+(uint)dataTable.Columns.Count-1, startRowIndex + (uint)dataTable.Rows.Count);
                    Debug.Print(startRef + ".ref." + endRef);
                    myexcel.UpdateAllPivotSource(STRFIRST_SHEETNAME, startRef,endRef);
                }
                else//it is a error format.
                {
                    myexcel.RemoveAllRows(STRFIRST_SHEETNAME);
                    myexcel.SetOrReplaceRows(STRFIRST_SHEETNAME, 1, 2, dataTable,null);
                    if (dataTable != null)
                    {
                        setGuard(1, 1 + (UInt32)dataTable.Rows.Count, myexcel);
                    }
                    else
                    {
                        setGuard(1, 1, myexcel);
                    }

                    //update pivotTable
                    string startRef = Common.MyExcelFile.GetCellReference((uint)2, 1);
                    string endRef = Common.MyExcelFile.GetCellReference((uint)2 + (uint)dataTable.Columns.Count - 1,  (uint)dataTable.Rows.Count+1);
                    Debug.Print(startRef + ".ref." + endRef);
                    myexcel.UpdateAllPivotSource(STRFIRST_SHEETNAME, startRef, endRef);

                    res = true;
                }

                
            }
            return res;
        }


        public static void setGuard(uint startRow, uint endrow, Common.MyExcelFile myexcel)
        {
            if (startRow == endrow)
            {
                DataTable dtt = Common.incUnitTest.GetOneValueDatatable(STRING_DATASTART + STRING_DATAEND);
                myexcel.SetOrUpdateCellValue(STRFIRST_SHEETNAME, startRow, 1, dtt.Rows[0]);
            }
            else
            {
                DataTable dtt = Common.incUnitTest.GetOneValueDatatable(STRING_DATASTART);
                myexcel.SetOrUpdateCellValue(STRFIRST_SHEETNAME, startRow, 1, dtt.Rows[0]);
                DataTable dtt2 = Common.incUnitTest.GetOneValueDatatable(STRING_DATAEND);
                myexcel.SetOrUpdateCellValue(STRFIRST_SHEETNAME, endrow, 1, dtt2.Rows[0]);
            }
        }



        ////并没有使用，如果性能不行，再考虑使用。
        //private static void insertDataTable(Common.MyExcelFile myexcel, DataTable dataTable, UInt32 startRowIndex,string sheetName)
        //{
        //    myexcel.SetOrReplaceRow(sheetName, startRowIndex, 2, Common.MyExcelFile.GetColumnsNames(dataTable).Rows[0]);

        //    for (int i = 0; i < dataTable.Rows.Count; i++)
        //    {
        //        myexcel.SetOrReplaceRow(sheetName, startRowIndex + 1 + (uint)i, 2, dataTable.Rows[i]);
        //    }

        //    setGuard(startRowIndex, (uint)startRowIndex + (uint)dataTable.Rows.Count, myexcel);
        //}
        
    }
}