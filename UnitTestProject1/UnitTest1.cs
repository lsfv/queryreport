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
        string pathtemplate = "C:/testfile/Template.xlsx";

        //public const string STRFIRST_SHEETNAME = "Report";
        //public const string STRING_DATASTART = "_DATASTART";
        //public const string STRING_DATAEND = "_DATAEND";



        //[TestMethod]
        //public void TestAll()
        //{
        //    try
        //    {File.Delete(path);}
        //    catch
        //    { }
        //    CreateExcel();
        //    LoadFileAndCheck();
        //    TestWriteFile();
        //    CheckWriteFileResult();
        //    UpdateRow();
        //}


        //[TestMethod]
        //public void CreateExcel()
        //{
        //    string errMSG = "";

        //    Common.MyExcelFile excelFile = new Common.MyExcelFile(path, true, STRFIRST_SHEETNAME,out errMSG);
        //    Assert.AreEqual(excelFile.isValid(), true);

        //    if (excelFile.isValid())
        //    {
        //        DataTable dt = Common.incUnitTest.GetOneValueDatatable("1:44");
        //        excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME,4, 4, dt.Rows[0]);
        //        DataTable dt2 = Common.incUnitTest.GetOneValueDatatable("2:33");
        //        excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 3, 3, dt2.Rows[0]);

        //        DataTable dt3 = Common.incUnitTest.GetOneValueDatatable("3:21");
        //        excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 2, 1, dt3.Rows[0], excelFile.defaultCellStyle.blackIndex);

        //        DataTable dt4 = Common.incUnitTest.GetOneValueDatatable("4:56");
        //        excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 5, 6, dt4.Rows[0]);

        //        DataTable dt5 = Common.incUnitTest.GetOneValueDatatable("5:29");
        //        excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 2, 9, dt5.Rows[0]);

        //        DataTable dt6 = Common.incUnitTest.GetOneValueDatatable("6:51");
        //        excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 5, 1, dt6.Rows[0]);

        //        DataTable dt7 = Common.incUnitTest.GetOneValueDatatable("7:51v2");
        //        excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 5, 1, dt7.Rows[0]);


        //        DataTable dt8 = Common.incUnitTest.GetOneValueDatatable("8:56v2");
        //        excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 5, 6, dt8.Rows[0]);

        //        DataTable dt9 = Common.incUnitTest.GetOnetimeValueDatatable();
        //        excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 5, 7, dt9.Rows[0]);

        //        excelFile.SaveAndClose();//必须手动关闭,释放资源,否则没有自动释放之前,无法打开.
        //    }

        //    unzip(path);
        //}

        ////打开之前生成的文档,测试数据正确,并测试文件的保存功能.
        //[TestMethod]
        //public void LoadFileAndCheck()
        //{
        //    string errMSG = "";
        //    Common.MyExcelFile excelFile = new Common.MyExcelFile(path, out errMSG);
        //    Assert.AreEqual(excelFile.isValid(), true);

        //    Cell abc= excelFile.GetCell(STRFIRST_SHEETNAME, 5, 1);
        //    CellValues cellType = abc.DataType;
        //    Assert.AreNotEqual(abc,null);
        //    if (cellType == CellValues.SharedString)
        //    {
        //        Assert.AreEqual(excelFile.getShareString(int.Parse(abc.CellValue.Text)), "7:51v2");
        //    }
        //    else
        //    {
        //        Assert.AreEqual(abc.CellValue.Text, "7:51v2");
        //    }
        //    //Assert.AreEqual(excelFile.defaultCellStyle.normalIndex !=0,true);
        //    //Assert.AreEqual(excelFile.defaultCellStyle.blackIndex!=0, true);
        //    excelFile.SaveAndClose();
        //    unzip(path);
        //}

        ////写文件测试操作
        //[TestMethod]
        //public void TestWriteFile()
        //{
        //    string errMSG = "";
        //    Common.MyExcelFile excelFile = new Common.MyExcelFile(path, out errMSG);
        //    Assert.AreEqual(excelFile.isValid(), true);

        //    DataTable dt6 = Common.incUnitTest.GetOneValueDatatable("openfile:10:1");
        //    excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 10, 1, dt6.Rows[0], excelFile.defaultCellStyle.blackIndex);

        //    DataTable dt7 = Common.incUnitTest.GetOneValueDatatable("openfile:5:1v3");
        //    excelFile.SetOrUpdateCellValue(STRFIRST_SHEETNAME, 5, 1, dt6.Rows[0], excelFile.defaultCellStyle.blackIndex);

        //    excelFile.SaveAndClose();
        //    unzip(path);
        //}

        ////测试保存的数据
        //[TestMethod]
        //public void CheckWriteFileResult()
        //{
        //    string errMSG = "";

        //    using (Common.MyExcelFile excelFile = new Common.MyExcelFile(path, out errMSG))
        //    {
        //        Assert.AreEqual(excelFile.isValid(), true);

        //        Cell abc= excelFile.GetCell(STRFIRST_SHEETNAME, 10, 1);
        //        CellValues cellType = abc.DataType;
        //        if (cellType == CellValues.SharedString)
        //        {
        //            Assert.AreEqual(excelFile.getShareString(int.Parse(abc.CellValue.Text)), "openfile:10:1");
        //        }
        //        else
        //        {
        //            Assert.AreEqual(abc.CellValue.Text, "openfile:10:1");
        //        }
        //    }
        //}


        ////更新一整行,附加一整行,插入一整行.
        //[TestMethod]
        //public void UpdateRow()
        //{
        //    string errMSG = "";
        //    using (Common.MyExcelFile excelFile = new Common.MyExcelFile(path, out errMSG))
        //    {
        //        Assert.AreEqual(excelFile.isValid(), true);

        //        DataTable dataTable = Common.incUnitTest.GetDatatable();
        //        excelFile.SetOrReplaceRow(STRFIRST_SHEETNAME, 2, 4, dataTable.Rows[0]);

        //        DataTable dataTable2 = Common.incUnitTest.GetDatatable2();
        //        excelFile.SetOrReplaceRow(STRFIRST_SHEETNAME, 11, 2, dataTable2.Rows[0]);

        //        DataTable dataTable3 = Common.incUnitTest.GetDatatable2();
        //        excelFile.SetOrReplaceRow(STRFIRST_SHEETNAME, 1, 1, dataTable3.Rows[0]);
        //        excelFile.SetOrReplaceRow(STRFIRST_SHEETNAME, 2, 2, dataTable3.Rows[0]);
        //        excelFile.SetOrReplaceRow(STRFIRST_SHEETNAME, 3, 3, dataTable3.Rows[0]);
        //        excelFile.SetOrReplaceRow(STRFIRST_SHEETNAME, 4, 4, dataTable3.Rows[0]);
        //        excelFile.SetOrReplaceRow(STRFIRST_SHEETNAME, 5, 5, dataTable3.Rows[0]);
        //        excelFile.SetOrReplaceRow(STRFIRST_SHEETNAME, 15, 15, dataTable3.Rows[0]);
        //    }

        //    //unit test.
        //    using (Common.MyExcelFile excelFile = new Common.MyExcelFile(path, out errMSG))
        //    {
        //        Assert.AreEqual(excelFile.isValid(), true);

        //        Assert.AreEqual(excelFile.GetCellRealString(STRFIRST_SHEETNAME, 1, 2), "c++");
        //        Assert.AreEqual(excelFile.GetCellRealString(STRFIRST_SHEETNAME, 2, 3), "c++");
        //        Assert.AreEqual(excelFile.GetCellRealString(STRFIRST_SHEETNAME, 3, 4), "c++");
        //        Assert.AreEqual(excelFile.GetCellRealString(STRFIRST_SHEETNAME, 4, 5), "c++");
        //        Assert.AreEqual(excelFile.GetCellRealString(STRFIRST_SHEETNAME, 5, 6), "c++");
        //        Assert.AreEqual(excelFile.GetCellRealString(STRFIRST_SHEETNAME, 11, 3), "c++");
        //        Assert.AreEqual(excelFile.GetCellRealString(STRFIRST_SHEETNAME, 15, 16), "c++");
        //    }
        //}

        private int record1 = 0;
        private int record2 = -1;

        [TestMethod]
        public void templateTest()
        {
            int[] r1 = { 0, 1, 2, 1000 };

            int[] r2 = {-1, 0, 1, 2, 1000 };

            for (int i = 0; i < r1.Length; i++)
            {
                for (int j = 0; j < r2.Length; j++)
                {
                    record1 = r1[i];
                    record2 = r2[j];
                    templateTest_create();
                    templateTest_update();
                }
            }
        }


        //null:nothing .0:空数据，只有字段名，start .end 在一起  1: two records. 2 :three records. 1000:1001 records
        [TestMethod]
        public void templateTest_create()
        {
            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(record1);
            string errmsg;
            CUSTOMRP.BLL.TemplateManager.GenerateXlsxExcel(dataTable, out errmsg,pathtemplate, CUSTOMRP.BLL.TemplateManager.STRFIRST_SHEETNAME);
        }

        //pre :0 ,1,2,1000.
        //update->null ,0,1,2, 1000.
        [TestMethod]
        public void templateTest_update()
        {
            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(record2);
            string errmsg;
            CUSTOMRP.BLL.TemplateManager.UpdataData4XlsxExcel(dataTable, CUSTOMRP.BLL.TemplateManager.STRFIRST_SHEETNAME, out errmsg, pathtemplate);
        }

    
        [TestMethod]
        public void tempTest()
        {
            List<string> fruits = 
                new List<string> { "apple", "passionfruit", "banana", "mango",
                                "orange", "blueberry", "grape", "strawberry" };
            var res = fruits.Where(x => x == "applea").FirstOrDefault();
        }

        private void unzip(string file)
        {
            string descpath = "C:/testfile/unzip" + DateTime.Now.ToFileTimeUtc();
            Common.ZipFloClass.UncompressFile(descpath, file, true);
        }

        [TestMethod]
        public void unzipExcel()
        {
            string filepath = pathtemplate;
            unzip(filepath);
        }
    }
}