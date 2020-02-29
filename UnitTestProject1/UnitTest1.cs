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
        string path = "C:/testfile/myexcelAAA.xlsx";
        public const string STRFIRST_SHEETNAME = "Report";

        [TestMethod]
        public void CreateExcel()
        {
            string errMSG = "";

            Common.MyExcelFile excelFile = new Common.MyExcelFile(path, true, STRFIRST_SHEETNAME,out errMSG);
            Assert.AreEqual(excelFile.isValid(), true);

            if (excelFile.isValid())
            {
                DataTable dt = Common.incUnitTest.GetOneValueDatatable("1:44");
                excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME,4, 4, dt.Rows[0]);
                DataTable dt2 = Common.incUnitTest.GetOneValueDatatable("2:33");
                excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 3, 3, dt2.Rows[0]);

                DataTable dt3 = Common.incUnitTest.GetOneValueDatatable("3:21");
                excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 2, 1, dt3.Rows[0], excelFile.defaultCellStyle.blackIndex);

                DataTable dt4 = Common.incUnitTest.GetOneValueDatatable("4:56");
                excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 5, 6, dt4.Rows[0]);

                DataTable dt5 = Common.incUnitTest.GetOneValueDatatable("5:29");
                excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 2, 9, dt5.Rows[0]);

                DataTable dt6 = Common.incUnitTest.GetOneValueDatatable("6:51");
                excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 5, 1, dt6.Rows[0]);

                DataTable dt7 = Common.incUnitTest.GetOneValueDatatable("7:51v2");
                excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 5, 1, dt7.Rows[0]);


                DataTable dt8 = Common.incUnitTest.GetOneValueDatatable("8:56v2");
                excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 5, 6, dt8.Rows[0]);

                DataTable dt9 = Common.incUnitTest.GetOnetimeValueDatatable();
                excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 5, 7, dt9.Rows[0]);

                excelFile.SaveAndClose();//必须手动关闭,释放资源,否则没有自动释放之前,无法打开.
            }

            unzip(path);
        }

        //打开之前生成的文档,测试数据正确,并测试文件的保存功能.
        [TestMethod]
        public void LoadFileAndCheck()
        {
            string errMSG = "";
            Common.MyExcelFile excelFile = new Common.MyExcelFile(path, out errMSG);
            Assert.AreEqual(excelFile.isValid(), true);
            
            Cell abc= excelFile.GetCell(STRFIRST_SHEETNAME, 5, 1);
            CellValues cellType = abc.DataType;
            Assert.AreNotEqual(abc,null);
            if (cellType == CellValues.SharedString)
            {
                Assert.AreEqual(excelFile.getShareString(int.Parse(abc.CellValue.Text)), "7:51v2");
            }
            else
            {
                Assert.AreEqual(abc.CellValue.Text, "7:51v2");
            }
            //Assert.AreEqual(excelFile.defaultCellStyle.normalIndex !=0,true);
            //Assert.AreEqual(excelFile.defaultCellStyle.blackIndex!=0, true);
            excelFile.SaveAndClose();
            unzip(path);
        }

        //写文件测试操作
        [TestMethod]
        public void TestWriteFile()
        {
            string errMSG = "";
            Common.MyExcelFile excelFile = new Common.MyExcelFile(path, out errMSG);
            Assert.AreEqual(excelFile.isValid(), true);

            DataTable dt6 = Common.incUnitTest.GetOneValueDatatable("openfile:10:1");
            excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 10, 1, dt6.Rows[0], excelFile.defaultCellStyle.blackIndex);

            DataTable dt7 = Common.incUnitTest.GetOneValueDatatable("openfile:5:1v3");
            excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 5, 1, dt6.Rows[0], excelFile.defaultCellStyle.blackIndex);

            excelFile.SaveAndClose();
            unzip(path);
        }

        //测试保存的数据和预设样式是否正确,测试2次1.直接测试2. 手工打开文档并保存. 测试原始和经过自动优化后是否都还有正确的数据和样式.
        //每次都打开看是否10:1,需要人工看是否是粗体.
        [TestMethod]
        public void CheckWriteFileResult()
        {
            string errMSG = "";
            
            using (Common.MyExcelFile excelFile = new Common.MyExcelFile(path, out errMSG))
            {
                Assert.AreEqual(excelFile.isValid(), true);

                Cell abc= excelFile.GetCell(STRFIRST_SHEETNAME, 10, 1);
                CellValues cellType = abc.DataType;
                if (cellType == CellValues.SharedString)
                {
                    Assert.AreEqual(excelFile.getShareString(int.Parse(abc.CellValue.Text)), "openfile:10:1");
                }
                else
                {
                    Assert.AreEqual(abc.CellValue.Text, "openfile:10:1");
                }
            }
        }


        //更新一整行,附加一整行,插入一整行.
        [TestMethod]
        public void UpdateRow()
        {
            string errMSG = "";
            using (Common.MyExcelFile excelFile = new Common.MyExcelFile(path, out errMSG))
            {
                Assert.AreEqual(excelFile.isValid(), true);

                DataTable dataTable = Common.incUnitTest.GetDatatable();
                excelFile.SetOrUpdateRow(STRFIRST_SHEETNAME, 2, 4, dataTable.Rows[0]);

                DataTable dataTable2 = Common.incUnitTest.GetDatatable2();
                excelFile.SetOrUpdateRow(STRFIRST_SHEETNAME, 11, 2, dataTable2.Rows[0]);

                DataTable dataTable3 = Common.incUnitTest.GetDatatable2();
                excelFile.SetOrUpdateRow(STRFIRST_SHEETNAME, 1, 1, dataTable3.Rows[0]);
                excelFile.SetOrUpdateRow(STRFIRST_SHEETNAME, 2, 2, dataTable3.Rows[0]);
                excelFile.SetOrUpdateRow(STRFIRST_SHEETNAME, 3, 3, dataTable3.Rows[0]);
                excelFile.SetOrUpdateRow(STRFIRST_SHEETNAME, 4, 4, dataTable3.Rows[0]);
                excelFile.SetOrUpdateRow(STRFIRST_SHEETNAME, 5, 5, dataTable3.Rows[0]);

                excelFile.SetOrUpdateRow(STRFIRST_SHEETNAME, 15, 15, dataTable3.Rows[0]);


                //todo check rows count is same, new row 's cell cout is datable's columns count.
            }

        }

        [TestMethod]
        public void UpdateByNewData()
        {
            
        }

        [TestMethod]
        public void tempTest()
        {
            List<string> fruits = 
                new List<string> { "apple", "passionfruit", "banana", "mango",
                                "orange", "blueberry", "grape", "strawberry" };
            var res= fruits.Where(x => x == "applea").FirstOrDefault();
            int i=3;
        }

        [TestMethod]
        public void unzipExcel()
        {
            string filepath = path;
            unzip(filepath);
        }

        private void unzip(string file)
        {
            string descpath = "C:/testfile/unzip" + DateTime.Now.ToFileTimeUtc();
            Common.ZipFloClass.UncompressFile(descpath, file, true);
        }

    }
}