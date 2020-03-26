﻿using System;
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
                        Assert.AreEqual(incOpenExcel.IsRight(), true);
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
            try
            {
                using (Common.IncOpenExcel incOpenExcel = new Common.IncOpenExcel(path, "Report"))
                {
                    Assert.AreEqual(incOpenExcel.IsRight(), true);

                    incOpenExcel.CreateOrUpdateRowAt("Report", null, 4, 1, null);
                    incOpenExcel.CreateOrUpdateRowAt("Report", null, 1, 1, null);
                    incOpenExcel.CreateOrUpdateRowAt("Report", null, 2, 1, null);
                    incOpenExcel.CreateOrUpdateRowAt("Report", null, 3, 1, null);
                    incOpenExcel.CreateOrUpdateRowAt("Report", null, 1, 1, null);
                    incOpenExcel.CreateOrUpdateRowAt("Report", null, 5, 1, null);

                    
                }
                
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
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
                    Assert.AreEqual(incOpenExcel.IsRight(), true);

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
                    Assert.AreEqual(incOpenExcel.IsRight(), true);

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
        public void CreateOrUpdateRowsAt2()
        {

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