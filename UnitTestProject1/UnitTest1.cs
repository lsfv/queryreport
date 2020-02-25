﻿using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.IO;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        string path = "C:/testfile/myexcelAAA.xlsx";

        [TestMethod]
        public void CreateExcel()
        {
            DataTable dataTable = Common.incUnitTest.GetDatatable();
            string errMSG = "";
            
            
            FileInfo fileInfo = new FileInfo(path);
            bool res= Common.incOpenXml.GenerateXlsxExcel(dataTable, out errMSG, path);
            Assert.AreEqual(res, true);
        }

        [TestMethod]
        public void UpdateByNewData()
        {
            string errMSG = "";
            DataTable dataTable2 = Common.incUnitTest.GetDatatable2();
            bool res = Common.incOpenXml.UpdataData4XlsxExcel(dataTable2, "Report", out errMSG, path);
            Console.Write(errMSG);
            Assert.AreEqual(res, true);
        }

        [TestMethod]
        public void UpdateVitoTableBySpecialRange()
        {
            bool res= Common.incOpenXml.SetPivotSource(path, "Report", "A1", "B2");
            Assert.AreEqual(res == true, true);
        }

        [TestMethod]
        public void tempTest()
        {
            Common.incOpenXml.testUnit(path);
        }
    }
}