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
                    using (Common.IncOpenExcel incOpenExcel = new Common.IncOpenExcel(files[i], "Report",true))
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

            using (Common.IncOpenExcel incOpenExcel = new Common.IncOpenExcel(path, "Report", true))
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
            //null,ok.   position 0,0 [-1,-1],[2,3] ,[1,1]
            string path = "C:\\testfile\\testrowdata.xlsx";
            try
            {
                using (Common.IncOpenExcel incOpenExcel = new Common.IncOpenExcel(path, "Report", true))
                {
                    Assert.AreEqual(incOpenExcel.IsValidate(), true);
                    DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
                    Dictionary<uint, uint> style = new Dictionary<uint, uint>();
                    style.Add(5,1);

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
                using (Common.IncOpenExcel incOpenExcel = new Common.IncOpenExcel(path, "Report", true))
                {
                    Assert.AreEqual(incOpenExcel.IsValidate(), true);

                    DataTable dataTable = null;// Common.incUnitTest.GetDatatableCustomCount(5);

                    //excelh CreateOrUpdateRowsAt(incOpenExcel, "Report", dataTable, 2, 2,null);

                    dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
                    //incOpenExcel.CreateOrUpdateRowsAt("Report", dataTable, 2, 2, null);

                    dataTable = Common.incUnitTest.GetDatatableCustomCount(2);
                    //incOpenExcel.CreateOrUpdateRowsAt("Report", dataTable, 3, 1, null);
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
            bool res = CUSTOMRP.BLL.ExcelHelper.CreateReport(bllpath, dataTable, null);
            Assert.AreEqual(res, true);
        }


        [TestMethod]
        public void CreateOrUpdateRowsAt_datanull()
        {
            string bllpath = "C:\\testfile\\bll_template_datanull.xlsx";
            DataTable dataTable = null;
            bool res = CUSTOMRP.BLL.ExcelHelper.CreateReport(bllpath, dataTable, null);
            Assert.AreEqual(res, true);
        }


        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreateOrUpdateRowsAt_pathnull()
        {
            string bllpath = null;
            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
            bool res = CUSTOMRP.BLL.ExcelHelper.CreateReport(bllpath, dataTable, null);
        }


        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreateOrUpdateRowsAt_pathempty()
        {

            string bllpath = "";
            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
            bool res = CUSTOMRP.BLL.ExcelHelper.CreateReport(bllpath, dataTable, null);
            Assert.AreEqual(res, true);

            res = CUSTOMRP.BLL.ExcelHelper.CreateReport(null, dataTable, null);
        }


        //测试样式.
        [TestMethod]
        public void CreateOrUpdateRowsAt_Remove()
        {
            string bllpath = "C:\\testfile\\bll_templateMoveAt.xlsx";
            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
            move(bllpath, dataTable);
        }
        

        //不是立即移动，而是得到xml，再附加文件的末尾，并附带一个移动到的行编号
        public static bool move(string path, DataTable dataTable)
        {
            bool res = false;

            using (Common.IncOpenExcel incOpenExcel = new Common.IncOpenExcel(path, SHEETNAME, true))
            {
                //incOpenExcel.CreateOrUpdateRowsAt(SHEETNAME, dataTable, 1, 2, null);

                List<string> rowsXmls = incOpenExcel.GetRowsXml(SHEETNAME, 2, 4);
                incOpenExcel.DeleteRows(SHEETNAME, 2, 4);
                incOpenExcel.MoveRows(SHEETNAME, rowsXmls, 20);
            }
            return res;
        }


        //测试更新cell.
        [TestMethod]
        public void T10_CreateOrUpdateCell()
        {
            string path = "C:\\testfile\\bll_templateUpdateCell.xlsx";

            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
            using (Common.IncOpenExcel incOpenExcel = new Common.IncOpenExcel(path, SHEETNAME, true))
            {
                //incOpenExcel.CreateOrUpdateRowsAt(SHEETNAME, dataTable, 1, 2,null);
                incOpenExcel.CreateOrUpdateCellAt(SHEETNAME, 1, 1, typeof(string), "_start");
            }

        }

        //测试 datable 带 flag
        [TestMethod]
        public void T1101_CreateTable_Flag()
        {
            string path = "C:\\testfile\\CreateTable1.xlsx";
            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
            CUSTOMRP.BLL.ExcelHelper.CreateReport(path, dataTable, null);
        }
        [TestMethod]
        public void T1102_CreateTable_Flag()
        {
            string path = "C:\\testfile\\CreateTable2.xlsx";
            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(0);
            CUSTOMRP.BLL.ExcelHelper.CreateReport(path, dataTable, null);
        }
        [TestMethod]
        public void T1103_CreateTable_Flag()
        {
            string path = "C:\\testfile\\CreateTable3.xlsx";
            DataTable dataTable = null;
            CUSTOMRP.BLL.ExcelHelper.CreateReport(path, dataTable, null);
        }

        [TestMethod]
        public void T1104_CreateTable_Flag()
        {
            string path = "C:\\testfile\\CreateTable3.xlsx";
            DataTable dataTable = null;
            CUSTOMRP.BLL.ExcelHelper.CreateReport(path, dataTable, null);
        }

        [TestMethod]
        public void T1201_UpdateTabel()
        {
            string path = "C:\\testfile\\updateTabel1.xlsx";
            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
            CUSTOMRP.BLL.ExcelHelper.CreateReport(path, dataTable,null);

            dataTable = null;
            CUSTOMRP.BLL.ExcelHelper.UpdateReport(path, dataTable, null);
        }


        [TestMethod]
        public void T1202_UpdateTabel()
        {
            string path = "C:\\testfile\\updateTabel2.xlsx";
            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
            CUSTOMRP.BLL.ExcelHelper.CreateReport(path, dataTable, null);

            dataTable = Common.incUnitTest.GetDatatableCustomCount(0);
            CUSTOMRP.BLL.ExcelHelper.UpdateReport(path, dataTable, null);
        }


        [TestMethod]
        public void T1203_UpdateTabel()
        {
            string path = "C:\\testfile\\updateTabel3.xlsx";
            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
            CUSTOMRP.BLL.ExcelHelper.CreateReport(path, dataTable, null);

            dataTable = Common.incUnitTest.GetDatatableCustomCount(1);
            CUSTOMRP.BLL.ExcelHelper.UpdateReport(path, dataTable, null);
        }


        [TestMethod]
        public void T1204_UpdateTabel()
        {
            string path = "C:\\testfile\\updateTabel4.xlsx";
            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
            CUSTOMRP.BLL.ExcelHelper.CreateReport(path, dataTable, null);

            dataTable = Common.incUnitTest.GetDatatableCustomCount(10);
            CUSTOMRP.BLL.ExcelHelper.UpdateReport(path, dataTable, null);
        }


        [TestMethod]
        public void T1205_UpdateTabel()
        {
            string path = "C:\\testfile\\updateTabel5.xlsx";

            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
            List<int> totalIndex = new List<int>();
            totalIndex.Add(3);
            CUSTOMRP.BLL.ExcelHelper.CreateReport(path, dataTable, totalIndex);

            dataTable = Common.incUnitTest.GetDatatableCustomCount(10);
            CUSTOMRP.BLL.ExcelHelper.UpdateReport(path, dataTable, null);


            List<int> totalIndex2 = new List<int>();
            totalIndex2.Add(0);
            totalIndex2.Add(1);
            dataTable = Common.incUnitTest.GetDatatableCustomCount(2);
            CUSTOMRP.BLL.ExcelHelper.UpdateReport(path, dataTable, totalIndex2);

            dataTable = Common.incUnitTest.GetDatatableCustomCount(0);
            CUSTOMRP.BLL.ExcelHelper.UpdateReport(path, dataTable, null);

            dataTable = null;
            CUSTOMRP.BLL.ExcelHelper.UpdateReport(path, dataTable, null);

            List<int> totalIndex3 = new List<int>();
            totalIndex3.Add(3);
            dataTable = Common.incUnitTest.GetDatatableCustomCount(3);
            CUSTOMRP.BLL.ExcelHelper.UpdateReport(path, dataTable, totalIndex3);
        }

        [TestMethod]
        public void T1301_GetRowColumn()
        {
            uint r, c;
            Ref2RC("A2", out r, out c);
            Debug.Print(r + "." + c);

            Ref2RC("z2", out r, out c);
            Debug.Print(r + "." + c);

            Ref2RC("bc4", out r, out c);
            Debug.Print(r + "." + c);
        }

        public void Ref2RC(string refa,out uint rowNo,out uint columnNo)
        {
            rowNo = 0;
            columnNo = 0;
            refa = refa.ToUpper();
            Dictionary<char, int> maping = new Dictionary<char, int>();
            maping.Add('A', 1); maping.Add('B', 2); maping.Add('C', 3); maping.Add('D', 4);
            maping.Add('E', 5); maping.Add('F', 6); maping.Add('G', 7); maping.Add('H', 8);
            maping.Add('I', 9); maping.Add('J', 10); maping.Add('K', 11); maping.Add('L', 12);
            maping.Add('M', 13); maping.Add('N', 14); maping.Add('O', 15); maping.Add('P', 16);
            maping.Add('Q', 17); maping.Add('R', 18); maping.Add('S', 19); maping.Add('T', 20);
            maping.Add('U', 21); maping.Add('V', 22); maping.Add('W', 23); maping.Add('X', 24);
            maping.Add('Y', 25); maping.Add('Z', 26);

            List<char> chars = refa.ToList<char>();
            List<char> columns = new List<char>();
            List<char> rows = new List<char>();
            foreach (char c in chars)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    columns.Add(c);
                }
                else if (c >= '0' && c <= '9')
                {
                    rows.Add(c);
                }
            }
            if (columns.Count == 2)
            {
                columnNo = (uint)(26 * maping[columns[0]] + maping[columns[1]]);
            }
            else if (columns.Count == 1)
            {
                columnNo = (uint)maping[columns[0]];
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void T1401_CreateTabel()
        {
            CUSTOMRP.BLL.ExcelHelper.CreateReport(null, null, null);
        }

        [TestMethod]
        public void T1410_UpdateTabel()
        {
            string path = "C:\\testfile\\updateTabel10.xlsx";

            DataTable dataTable = Common.incUnitTest.GetDatatableCustomCount(2);
            List<int> totalIndex = null;
            CUSTOMRP.BLL.ExcelHelper.CreateReport(path, dataTable, totalIndex);

            dataTable = Common.incUnitTest.GetDatatableCustomCount(3,30);
            totalIndex = new List<int>();
            CUSTOMRP.BLL.ExcelHelper.UpdateReport(path, dataTable, totalIndex);

            dataTable = Common.incUnitTest.GetDatatableCustomCount(5,4);
            totalIndex = new List<int>();
            totalIndex.Add(1);
            totalIndex.Add(3);
            CUSTOMRP.BLL.ExcelHelper.UpdateReport(path, dataTable, totalIndex);


            dataTable = Common.incUnitTest.GetDatatableCustomCount(1);
            totalIndex = new List<int>();
            totalIndex.Add(0);
            CUSTOMRP.BLL.ExcelHelper.UpdateReport(path, dataTable, totalIndex);


            dataTable = Common.incUnitTest.GetDatatableCustomCount(0);
            totalIndex = new List<int>();
            totalIndex.Add(1);
            CUSTOMRP.BLL.ExcelHelper.UpdateReport(path, dataTable, totalIndex);

            dataTable = null;
            totalIndex = new List<int>();
            totalIndex.Add(0);
            CUSTOMRP.BLL.ExcelHelper.UpdateReport(path, dataTable, totalIndex);

            dataTable = Common.incUnitTest.GetDatatableCustomCount(5);
            totalIndex = new List<int>();
            totalIndex.Add(1);
            totalIndex.Add(2);
            CUSTOMRP.BLL.ExcelHelper.UpdateReport(path, dataTable, totalIndex);

            dataTable = Common.incUnitTest.GetDatatableCustomCount(10);
            totalIndex = null;
            CUSTOMRP.BLL.ExcelHelper.UpdateReport(path, dataTable, totalIndex);
        }


        [TestMethod]
        public void unzipExcel()
        {
            string filepath = "C:\\testfile\\updateTabel5.xlsx";
            unzip(filepath);
        }


        private void unzip(string file)
        {
            string descpath = "C:/testfile/unzip"+Path.GetFileName(file) + DateTime.Now.ToFileTimeUtc();
            Common.ZipFloClass.UncompressFile(descpath, file, true);
        }
    }
}