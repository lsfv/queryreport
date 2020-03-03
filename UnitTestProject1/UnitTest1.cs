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

        public const string STRFIRST_SHEETNAME = "Report";
        public const string STRING_DATASTART = "_DATASTART";
        public const string STRING_DATAEND = "_DATAEND";

        [TestMethod]
        public void TestAll()
        {
            try
            {File.Delete(path);}
            catch
            { }
            CreateExcel();
            LoadFileAndCheck();
            TestWriteFile();
            CheckWriteFileResult();
            UpdateRow();
        }


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

        //测试保存的数据
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
                excelFile.SetOrReplaceRow(STRFIRST_SHEETNAME, 2, 4, dataTable.Rows[0]);

                DataTable dataTable2 = Common.incUnitTest.GetDatatable2();
                excelFile.SetOrReplaceRow(STRFIRST_SHEETNAME, 11, 2, dataTable2.Rows[0]);

                DataTable dataTable3 = Common.incUnitTest.GetDatatable2();
                excelFile.SetOrReplaceRow(STRFIRST_SHEETNAME, 1, 1, dataTable3.Rows[0]);
                excelFile.SetOrReplaceRow(STRFIRST_SHEETNAME, 2, 2, dataTable3.Rows[0]);
                excelFile.SetOrReplaceRow(STRFIRST_SHEETNAME, 3, 3, dataTable3.Rows[0]);
                excelFile.SetOrReplaceRow(STRFIRST_SHEETNAME, 4, 4, dataTable3.Rows[0]);
                excelFile.SetOrReplaceRow(STRFIRST_SHEETNAME, 5, 5, dataTable3.Rows[0]);
                excelFile.SetOrReplaceRow(STRFIRST_SHEETNAME, 15, 15, dataTable3.Rows[0]);
            }

            //unit test.
            using (Common.MyExcelFile excelFile = new Common.MyExcelFile(path, out errMSG))
            {
                Assert.AreEqual(excelFile.isValid(), true);

                Assert.AreEqual(excelFile.GetCellRealString(STRFIRST_SHEETNAME, 1, 2), "c++");
                Assert.AreEqual(excelFile.GetCellRealString(STRFIRST_SHEETNAME, 2, 3), "c++");
                Assert.AreEqual(excelFile.GetCellRealString(STRFIRST_SHEETNAME, 3, 4), "c++");
                Assert.AreEqual(excelFile.GetCellRealString(STRFIRST_SHEETNAME, 4, 5), "c++");
                Assert.AreEqual(excelFile.GetCellRealString(STRFIRST_SHEETNAME, 5, 6), "c++");
                Assert.AreEqual(excelFile.GetCellRealString(STRFIRST_SHEETNAME, 11, 3), "c++");
                Assert.AreEqual(excelFile.GetCellRealString(STRFIRST_SHEETNAME, 15, 16), "c++");
            }
        }


        private static void setGuard(uint startRow, uint endrow, Common.MyExcelFile myexcel)
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

        [TestMethod]
        public void templateTest_create()
        {
            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
            //先加载datatable.再修改cell. 1.null.2no reocrd 3.one record .4 two or more.
            if (dataTable != null)
            {
                string errMsg = "";
                using (Common.MyExcelFile myexcel = new Common.MyExcelFile(pathtemplate, true, STRFIRST_SHEETNAME, out errMsg))
                {
                    myexcel.SetOrReplaceRows(STRFIRST_SHEETNAME, 1, 2, dataTable);
                    if (dataTable != null)
                    {
                        setGuard(1, 1 + (UInt32)dataTable.Rows.Count, myexcel);
                    }
                    else
                    {
                        setGuard(1, 1, myexcel);
                    }
                }
            }
        }

        

        [TestMethod]
        public void templateTest_update()
        {
            //测试删除某行开始,并附加在某行.1.s=e=1,  2.s=1,e=2,  3.s=2,e=3.  4.s=1.e=4 .5.s=2,e=e;
            //删除起始行数据(包含起止).2更新后半段数据为调整后的行索引.3.填充现有数据.4.计算数据的range.并更新到pivotTable.
            //如果没有找到,删除所有数据,调用templateTest_create
           
            string errMsg = "";
            using (Common.MyExcelFile myexcel = new Common.MyExcelFile(pathtemplate,out errMsg))
            {
                UInt32 startRowIndex = myexcel.GetRowIndexFromAColumn(STRFIRST_SHEETNAME,STRING_DATASTART,1);
                UInt32 endRowIndex = myexcel.GetRowIndexFromAColumn(STRFIRST_SHEETNAME, STRING_DATAEND,1);

                if (startRowIndex ==0 &&  endRowIndex == 0)
                {
                    startRowIndex = myexcel.GetRowIndexFromAColumn(STRFIRST_SHEETNAME, STRING_DATASTART+ STRING_DATAEND, 1);
                    endRowIndex = startRowIndex;
                }

                DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(1);

                if (endRowIndex >= startRowIndex && startRowIndex > 0)
                {
                    IEnumerable<Row> rows_bottom = myexcel.GetRangeRows(STRFIRST_SHEETNAME, endRowIndex + 1, UInt32.MaxValue);
                    

                    //reomve pre data;
                    myexcel.RemoveRows(STRFIRST_SHEETNAME, startRowIndex, endRowIndex);
                    //update pre bottom data;
                    int deleteCount = (int)(endRowIndex - startRowIndex + 1);
                    int insertCount = dataTable==null?1: dataTable.Rows.Count + 1;
                    int offsetCount = insertCount - deleteCount;
                    updateRowIndexAndCellReference(rows_bottom, offsetCount);
                    //insert new data;
                    myexcel.SetOrReplaceRows(STRFIRST_SHEETNAME, startRowIndex, 2, dataTable);
                    if (dataTable != null)
                    {
                        setGuard(startRowIndex, startRowIndex + (UInt32)dataTable.Rows.Count, myexcel);
                    }
                    else
                    {
                        setGuard(startRowIndex, startRowIndex, myexcel);
                    }
                }
                else
                {
                    myexcel.RemoveAllRows(STRFIRST_SHEETNAME);

                    myexcel.SetOrReplaceRows(STRFIRST_SHEETNAME, 1, 2, dataTable);
                    if (dataTable != null)
                    {
                        setGuard(1, 1 + (UInt32)dataTable.Rows.Count, myexcel);
                    }
                    else
                    {
                        setGuard(1, 1, myexcel);
                    }
                }
            }
        }

        private void insertDataTable(Common.MyExcelFile myexcel,DataTable dataTable, UInt32 startRowIndex)
        {
            myexcel.SetOrReplaceRow(STRFIRST_SHEETNAME, startRowIndex, 2, Common.MyExcelFile.GetColumnsNames(dataTable).Rows[0]);

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                myexcel.SetOrReplaceRow(STRFIRST_SHEETNAME, startRowIndex + 1 + (uint)i, 2, dataTable.Rows[i]);
            }

            setGuard(startRowIndex, (uint)startRowIndex  + (uint)dataTable.Rows.Count,myexcel);
        }

        private void updateRowIndexAndCellReference(IEnumerable<Row> rows,int offsetIndex)
        {
            foreach (Row row in rows)
            {
                uint preIndex = row.RowIndex;
                row.RowIndex = (uint)(preIndex + offsetIndex);
                string preIndexStr = preIndex.ToString();
                string nowIndexStr = row.RowIndex.ToString();
                int preIndexStrLength = preIndexStr.Length;
                var allcells = row.Elements<Cell>();
                foreach (Cell cell in allcells)
                {
                    cell.CellReference = cell.CellReference.Value.Replace(preIndexStr, "") + nowIndexStr;
                }
            }
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

        //[TestMethod]
        //public void tempTest2()
        //{
        //    string errMsg = "";
        //    using (Common.MyExcelFile myexcel = new Common.MyExcelFile(pathtemplate, out errMsg))
        //    {
        //       var data= myexcel.getWorksheet(STRFIRST_SHEETNAME).Elements<SheetData>().First();

        //        var firstorw = data.Elements<Row>().First();
        //        Row newrow = new Row();
        //        data.InsertAfter(newrow, firstorw);
        //    }
        //}
    }
}