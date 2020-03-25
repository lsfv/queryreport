using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CreateReportTest()
        {
            //不同的数据:null,empty,1,2,3,5,100
            List<DataTable> dataTables = new List<DataTable>();
            dataTables.Add(null);
            dataTables.Add(Common.incUnitTest.GetDatatableCustomCount(0));
            dataTables.Add(Common.incUnitTest.GetDatatableCustomCount(1));
            dataTables.Add(Common.incUnitTest.GetDatatableCustomCount(2));
            dataTables.Add(Common.incUnitTest.GetDatatableCustomCount(3));
            dataTables.Add(Common.incUnitTest.GetDatatableCustomCount(5));
            dataTables.Add(Common.incUnitTest.GetDatatableCustomCount(100));

            for(int i=0;i<dataTables.Count();i++)
            {
                CUSTOMRP.BLL.ExcelReportInfo reportInfo = new CUSTOMRP.BLL.ExcelReportInfo(dataTables[i], (uint)i+1, null, null, null, null, null, null, null);
                string path = "C:/testfile/Template"+i.ToString()+".xlsx";
                CUSTOMRP.BLL.ExcelGeneraterHelper.GenerateXlsxExcel(path, reportInfo);
            }
        }

        [TestMethod]
        public void UpdateReportTest()
        {
            //更新不同的种类.  empty->2.  1->3.  2->1  3->0,5->null,100->100s

            List<DataTable> dataTables = new List<DataTable>();
            dataTables.Add(Common.incUnitTest.GetDatatableCustomCount(2));
            dataTables.Add(Common.incUnitTest.GetDatatableCustomCount(3));
            dataTables.Add(Common.incUnitTest.GetDatatableCustomCount(1));
            dataTables.Add(Common.incUnitTest.GetDatatableCustomCount(0));
            dataTables.Add(null);
            dataTables.Add(Common.incUnitTest.GetDatatableCustomCount(100));
            for (int i = 0; i < dataTables.Count(); i++)
            {
                CUSTOMRP.BLL.ExcelReportInfo reportInfo = new CUSTOMRP.BLL.ExcelReportInfo(dataTables[i], (uint)i+1, null, null, null, null, null, null, null);
                string path = "";
                if (i == 0)
                {
                    path = "C:/testfile/Template1.xlsx";
                }
                if (i == 1)
                {
                    path = "C:/testfile/Template2.xlsx";
                }
                if (i == 2)
                {
                    path = "C:/testfile/Template3.xlsx";
                }
                if (i == 3)
                {
                    path = "C:/testfile/Template4.xlsx";
                }
                if (i == 4)
                {
                    path = "C:/testfile/Template5.xlsx";
                }
                if (i == 5)
                {
                    path = "C:/testfile/Template6.xlsx";
                }
                CUSTOMRP.BLL.ExcelGeneraterHelper.UpdataData4XlsxExcel(path, reportInfo);
            }
        }

        [TestMethod]
        public void PivotTableTest1()
        {
            //add pivotable in  1.xlsm:
            //2->8
            //8->3
            //3->0
            //0->null

            DataTable dataTable1 = Common.incUnitTest.GetDatatableCustomCount(8);
            CUSTOMRP.BLL.ExcelReportInfo reportInfo = new CUSTOMRP.BLL.ExcelReportInfo(dataTable1, 100, null, null, null, null, null, null, null);
            string path = "C:/testfile/Template1.xlsx";
            CUSTOMRP.BLL.ExcelGeneraterHelper.UpdataData4XlsxExcel(path, reportInfo);
        }

        [TestMethod]
        public void PivotTableTest2()
        {
            //1.xlsm:
            //2->8
            //8->3
            //3->0
            //0->null

            DataTable dataTable1 = Common.incUnitTest.GetDatatableCustomCount(3);
            CUSTOMRP.BLL.ExcelReportInfo reportInfo = new CUSTOMRP.BLL.ExcelReportInfo(dataTable1, 100, null, null, null, null, null, null, null);
            string path = "C:/testfile/Template1.xlsx";
            CUSTOMRP.BLL.ExcelGeneraterHelper.UpdataData4XlsxExcel(path, reportInfo);
        }

        [TestMethod]
        public void PivotTableTest3()
        {
            //1.xlsm:
            //2->8
            //8->3
            //3->0
            //0->null

            DataTable dataTable1 = Common.incUnitTest.GetDatatableCustomCount(0);
            CUSTOMRP.BLL.ExcelReportInfo reportInfo = new CUSTOMRP.BLL.ExcelReportInfo(dataTable1, 100, null, null, null, null, null, null, null);
            string path = "C:/testfile/Template1.xlsx";
            CUSTOMRP.BLL.ExcelGeneraterHelper.UpdataData4XlsxExcel(path, reportInfo);
        }

        [TestMethod]
        public void PivotTableTest4()
        {
            //1.xlsm:
            //2->8
            //8->3
            //3->0
            //0->null

            DataTable dataTable1 = null;
            CUSTOMRP.BLL.ExcelReportInfo reportInfo = new CUSTOMRP.BLL.ExcelReportInfo(dataTable1, 100, null, null, null, null, null, null, null);
            string path = "C:/testfile/Template1.xlsx";
            CUSTOMRP.BLL.ExcelGeneraterHelper.UpdataData4XlsxExcel(path, reportInfo);
        }

        [TestMethod]
        public void styleTest()
        {
            //add style on  2.xlsm
            DataTable dataTable1 = Common.incUnitTest.GetDatatableCustomCount(15);
            CUSTOMRP.BLL.ExcelReportInfo reportInfo = new CUSTOMRP.BLL.ExcelReportInfo(dataTable1, 100, null, null, null, null, null, null, null);
            string path = "C:/testfile/Template2.xlsx";
            CUSTOMRP.BLL.ExcelGeneraterHelper.UpdataData4XlsxExcel(path, reportInfo);
        }

        [TestMethod]
        public void TotalTest1()
        {
            //1.zero record. 2. one record. 3. a error record 4. a over index record.
            DataTable dataTable1 = Common.incUnitTest.GetDatatableCustomCount(15);
            List<int> columns = new List<int>();

            CUSTOMRP.BLL.StatisticColumns totalColumns = new CUSTOMRP.BLL.StatisticColumns(CUSTOMRP.BLL.Enum_StatisitcType.Total, columns);
            CUSTOMRP.BLL.ExcelReportInfo reportInfo = new CUSTOMRP.BLL.ExcelReportInfo(dataTable1, 1, null, totalColumns, null, null, null, null, null);
            string path = "C:/testfile/TemplateTotal1.xlsx";
            CUSTOMRP.BLL.ExcelGeneraterHelper.GenerateXlsxExcel(path, reportInfo);
        }

        [TestMethod]
        public void TotalTest2()
        {
            //1.zero record. 2. one record. 3. a error record 4. a over index record.
            DataTable dataTable1 = Common.incUnitTest.GetDatatableCustomCount(15);
            List<int> columns = new List<int>();
            columns.Add(0);

            CUSTOMRP.BLL.StatisticColumns totalColumns = new CUSTOMRP.BLL.StatisticColumns(CUSTOMRP.BLL.Enum_StatisitcType.Total, columns);
            CUSTOMRP.BLL.ExcelReportInfo reportInfo = new CUSTOMRP.BLL.ExcelReportInfo(dataTable1, 1, null, totalColumns, null, null, null, null, null);
            string path = "C:/testfile/TemplateTotal2.xlsx";
            CUSTOMRP.BLL.ExcelGeneraterHelper.GenerateXlsxExcel(path, reportInfo);
        }

        [TestMethod]
        public void TotalTest3()
        {
            //1.zero record. 2. one record. 3. a error record 4. a over index record.
            DataTable dataTable1 = Common.incUnitTest.GetDatatableCustomCount(15);
            List<int> columns = new List<int>();
            columns.Add(1);

            CUSTOMRP.BLL.StatisticColumns totalColumns = new CUSTOMRP.BLL.StatisticColumns(CUSTOMRP.BLL.Enum_StatisitcType.Total, columns);
            CUSTOMRP.BLL.ExcelReportInfo reportInfo = new CUSTOMRP.BLL.ExcelReportInfo(dataTable1, 1, null, totalColumns, null, null, null, null, null);
            string path = "C:/testfile/TemplateTotal3.xlsx";
            CUSTOMRP.BLL.ExcelGeneraterHelper.GenerateXlsxExcel(path, reportInfo);
        }

        [TestMethod]
        public void TotalTest4()
        {
            //1.zero record. 2. one record. 3. a error record 4. a over index record.
            DataTable dataTable1 = Common.incUnitTest.GetDatatableCustomCount(15);
            List<int> columns = new List<int>();
            columns.Add(0);
            columns.Add(10);
            CUSTOMRP.BLL.StatisticColumns totalColumns = new CUSTOMRP.BLL.StatisticColumns(CUSTOMRP.BLL.Enum_StatisitcType.Total, columns);
            CUSTOMRP.BLL.ExcelReportInfo reportInfo = new CUSTOMRP.BLL.ExcelReportInfo(dataTable1, 1, null, totalColumns, null, null, null, null, null);
            string path = "C:/testfile/TemplateTotal4.xlsx";
            CUSTOMRP.BLL.ExcelGeneraterHelper.GenerateXlsxExcel(path, reportInfo);
        }

        #region old
        ////null:nothing .0:空数据，只有字段名，start .end 在一起  1: two records. 2 :three records. 1000:1001 records
        //[TestMethod]
        //public void templateTest_create(DataTable dataTable )
        //{
        //    string errmsg;
        //    CUSTOMRP.BLL.ExcelGeneraterHelper.ReportStatisticInfo reportStatisticInfo = new CUSTOMRP.BLL.ExcelGeneraterHelper.ReportStatisticInfo();

        //    List<int> TotalIndex= new List<int>();
        //    TotalIndex.Add(15);
        //    reportStatisticInfo.Statistics_total = new CUSTOMRP.BLL.ExcelGeneraterHelper.ReportStatisitc_Total(TotalIndex);

        //    CUSTOMRP.BLL.ExcelGeneraterHelper.GenerateXlsxExcel(dataTable, out errmsg,pathtemplate, reportStatisticInfo);
        //}

        ////null:nothing .0:空数据，只有字段名，start .end 在一起  1: two records. 2 :three records. 1000:1001 records
        //[TestMethod]
        //public void templateTest_create2()
        //{
        //    DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(record1);
        //    string errmsg;
        //    CUSTOMRP.BLL.ExcelGeneraterHelper.ReportStatisticInfo reportStatisticInfo = new CUSTOMRP.BLL.ExcelGeneraterHelper.ReportStatisticInfo();

        //    List<int> indexs = new List<int>();
        //    reportStatisticInfo.Statistics_total = new CUSTOMRP.BLL.ExcelGeneraterHelper.ReportStatisitc_Total(indexs);

        //    CUSTOMRP.BLL.ExcelGeneraterHelper.GenerateXlsxExcel(dataTable, out errmsg, pathtemplate, reportStatisticInfo);
        //}

        ////pre :0 ,1,2,1000.
        ////update->null ,0,1,2, 1000.
        ////todo 1. format style.   2.total   3.pic  4.group.
        //[TestMethod]
        //public void templateTest_update()
        //{
        //    DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(record2);
        //    string errmsg;
        //    CUSTOMRP.BLL.ExcelGeneraterHelper.ReportStatisticInfo reportArgument = new CUSTOMRP.BLL.ExcelGeneraterHelper.ReportStatisticInfo();

        //    List<int> indexs = new List<int>();
        //    indexs.Add(0);
        //    indexs.Add(3);
        //    reportArgument.Statistics_total = new CUSTOMRP.BLL.ExcelGeneraterHelper.ReportStatisitc_Total(indexs);

        //    CUSTOMRP.BLL.ExcelGeneraterHelper.UpdataData4XlsxExcel(dataTable, out errmsg, pathtemplate, reportArgument);
        //}


        //[TestMethod]
        //public void tempTest()
        //{
        //    List<string> fruits = 
        //        new List<string> { "apple", "passionfruit", "banana", "mango",
        //                        "orange", "blueberry", "grape", "strawberry" };
        //    var res = fruits.Where(x => x == "applea").FirstOrDefault();
        //}


        //private void unzip(string file)
        //{
        //    string descpath = "C:/testfile/unzip" + DateTime.Now.ToFileTimeUtc();
        //    Common.ZipFloClass.UncompressFile(descpath, file, true);
        //}


        //[TestMethod]
        //public void unzipExcel()
        //{
        //    string filepath = pathtemplate;
        //    unzip(filepath);
        //}


        //[TestMethod]
        //public void temp_dictonary()
        //{

        //    Row row = new Row();
        //    row.ToString();

        //}

        //private string RemoveLastNumber(string str)
        //{
        //    while (true)
        //    {
        //        if (string.IsNullOrWhiteSpace(str) || str.Last<char>() >= 'A')
        //        {
        //            break;
        //        }
        //        else
        //        {
        //            str = str.Remove(str.Length - 1);
        //        }
        //    }
        //    return str;
        //}


        //public static string GetCellReference_Columnname(UInt32 colIndex)
        //{
        //    UInt32 dividend = colIndex;
        //    string columnName = String.Empty;
        //    UInt32 modifier;

        //    while (dividend > 0)
        //    {
        //        modifier = (dividend - 1) % 26;
        //        columnName = Convert.ToChar(65 + modifier).ToString() + columnName;
        //        dividend = (UInt32)((dividend - modifier) / 26);
        //    }
        //    return columnName;
        //}
        #endregion
    }
}