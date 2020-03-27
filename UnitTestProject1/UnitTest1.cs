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
        private static readonly string SHEETNAME = "Report";

        //创建测试,期望错误的都有异常出现.否则都可以建立document成员变量.
        [TestMethod]
        public void CreateFile()
        {
            //null,"", "abc"  "avadf\fdsa.word" "aa.xlsx"
            string[] files = { null, "", "aa", "C:\\testfile\\empty1.world", "C:\\testfileaaaaa\\empty1.world", "C:\\testfile\\empty1.xlsx" , "C:\\testfile\\empty1.xlsx" };
            for (int i = 0; i < files.Count(); i++)
            {
                try
                {
                    using (Common.IncOpenExcel incOpenExcel = new Common.IncOpenExcel(files[i], "Report"))
                    {
                        Assert.AreEqual(incOpenExcel.IsValidate(), true);
                    }
                }
                catch(Exception e)
                {
                    Debug.Print(e.ToString());
                }
            }
        }

        [TestMethod]
        public void CreateOrUpdateRowAt()
        {
            //null,ok.
            //position 0,0 [-1,-1],[2,3] ,[1,1]

            string path = "C:\\testfile\\testjustrow.xlsx";

            using (Common.IncOpenExcel incOpenExcel = new Common.IncOpenExcel(path, "Report"))
            {
                Assert.AreEqual(incOpenExcel.IsValidate(), true);

                incOpenExcel.CreateOrUpdateRowAt("Report", null, 4, 1, null);
                incOpenExcel.CreateOrUpdateRowAt("Report", null, 1, 1, null);
                incOpenExcel.CreateOrUpdateRowAt("Report", null, 2, 1, null);
                incOpenExcel.CreateOrUpdateRowAt("Report", null, 3, 1, null);
                incOpenExcel.CreateOrUpdateRowAt("Report", null, 1, 1, null);
                incOpenExcel.CreateOrUpdateRowAt("Report", null, 5, 1, null);
            }
            unzip(path);
        }


        [TestMethod]
        public void CreateOrUpdateRowAt2()
        {
            //null,ok.
            //position 0,0 [-1,-1],[2,3] ,[1,1]

            string path = "C:\\testfile\\testrowdata.xlsx";
            try
            {
                using (Common.IncOpenExcel incOpenExcel = new Common.IncOpenExcel(path, "Report"))
                {
                    Assert.AreEqual(incOpenExcel.IsValidate(), true);

                    DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(5);

                    Dictionary<int, uint> style = new Dictionary<int, uint>();
                    style.Add(2, 1);

                    incOpenExcel.CreateOrUpdateRowAt("Report", dataTable.Rows[0], 4, 4, style);
                    incOpenExcel.CreateOrUpdateRowAt("Report", null, 1, 1, null);
                    incOpenExcel.CreateOrUpdateRowAt("Report", dataTable.Rows[1], 2, 2, null);
                    incOpenExcel.CreateOrUpdateRowAt("Report", dataTable.Rows[2], 3, 3, null);
                    incOpenExcel.CreateOrUpdateRowAt("Report", dataTable.Rows[3], 1, 1, null);
                    incOpenExcel.CreateOrUpdateRowAt("Report", dataTable.Rows[4], 2, 2, null);
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
        }


        [TestMethod]
        public void CreateOrUpdateRowsAt1()
        {
            // table null, 1, 5,2 replace 5.
            string path = "C:\\testfile\\testtabledata1.xlsx";
            try
            {
                using (Common.IncOpenExcel incOpenExcel = new Common.IncOpenExcel(path, "Report"))
                {
                    Assert.AreEqual(incOpenExcel.IsValidate(), true);

                    DataTable dataTable = null;// Common.incUnitTest.GetDatatableCustomCount(5);

                    incOpenExcel.CreateOrUpdateRowsAt("Report", dataTable, 2, 2, null);

                    dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
                    incOpenExcel.CreateOrUpdateRowsAt("Report", dataTable, 2, 2, null);

                    dataTable = Common.incUnitTest.GetDatatableCustomCount(2);
                    incOpenExcel.CreateOrUpdateRowsAt("Report", dataTable, 3, 1, null);
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
        }


        //测试样式.
        [TestMethod]
        public void CreateOrUpdateRowsAt_ok()
        {
            string bllpath = "C:\\testfile\\bll_templateok.xlsx";
            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
            bool res = CUSTOMRP.BLL.ExcelHelper.CreateReport(bllpath, dataTable);
            Assert.AreEqual(res, true);
        }

        [TestMethod]
        public void CreateOrUpdateRowsAt_datanull()
        {
            string bllpath = "C:\\testfile\\bll_template_datanull.xlsx";
            DataTable dataTable = null;
            bool res = CUSTOMRP.BLL.ExcelHelper.CreateReport(bllpath, dataTable);
            Assert.AreEqual(res, true);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreateOrUpdateRowsAt_pathnull()
        {
            string bllpath = null;
            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
            bool res = CUSTOMRP.BLL.ExcelHelper.CreateReport(bllpath, dataTable);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreateOrUpdateRowsAt_pathempty()
        {

            string bllpath = "";
            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
            bool res = CUSTOMRP.BLL.ExcelHelper.CreateReport(bllpath, dataTable);
            Assert.AreEqual(res, true);

            res = CUSTOMRP.BLL.ExcelHelper.CreateReport(null, dataTable);
        }


        //测试样式.
        [TestMethod]
        public void CreateOrUpdateRowsAt_Remove()
        {
            string bllpath = "C:\\testfile\\bll_templatefixremove.xlsx";
            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(5);

            move(bllpath, dataTable);


        }
        

        //不是立即移动，而是得到xml，再附加文件的末尾，并附带一个移动到的行编号
        public static bool move(string path, DataTable dataTable)
        {
            bool res = false;

            using (Common.IncOpenExcel incOpenExcel = new Common.IncOpenExcel(path, SHEETNAME))
            {
                incOpenExcel.CreateOrUpdateRowsAt(SHEETNAME, dataTable, 1, 2, null);

                List<string> rowsXmls= incOpenExcel.GetRowsXml(SHEETNAME, 2, 4);
                //incOpenExcel.AppendRowsXml()
                //getRows
                //AppendAt(xmlrows,appendIndex)
            }



            return res;
        }


        //todo 加载也要测试非异常,有文档变量
        [TestMethod]
        public void unzipExcel()
        {
            string filepath = "C:\\testfile\\testrow.xlsx";
            unzip(filepath);
        }

        private void unzip(string file)
        {
            string descpath = "C:/testfile/unzip"+Path.GetFileName(file) + DateTime.Now.ToFileTimeUtc();
            Common.ZipFloClass.UncompressFile(descpath, file, true);
        }


       
    }
}