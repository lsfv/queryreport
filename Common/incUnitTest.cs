using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Common
{
    public abstract class incUnitTest
    {
        public static void SetupColumm(DataTable databable_books)
        {
            DataColumn tempDataColumn = null;
            tempDataColumn = databable_books.Columns.Add("ID", Type.GetType("System.Int32"));
            tempDataColumn.AutoIncrement = true;//自动增加
            tempDataColumn.AutoIncrementSeed = 1;//起始为1
            tempDataColumn.AutoIncrementStep = 1;//步长为1
            tempDataColumn.AllowDBNull = false;//


            databable_books.Columns.Add("Product", Type.GetType("System.String"));
            databable_books.Columns.Add("Date", Type.GetType("System.DateTime"));
            databable_books.Columns.Add("BookID", Type.GetType("System.Int32"));
            databable_books.Columns.Add("Description", Type.GetType("System.String"));
        }

        public static DataTable GetOneValueDatatable()
        {
            DataTable databable_books = new DataTable("Books");

       

            databable_books.Columns.Add("Product", Type.GetType("System.String"));


            DataRow newRow;
            newRow = databable_books.NewRow();
            newRow["Product"] = "c++";
            databable_books.Rows.Add(newRow);


            return databable_books;
        }

        public static DataTable GetDatatable()
        {
            DataTable databable_books = new DataTable("Books");

            SetupColumm(databable_books);

            DataRow newRow;
            newRow = databable_books.NewRow();
            newRow["Product"] = "c++";
            newRow["BookID"] = 1;
            newRow["Date"] = new DateTime(1922,11,15);
            newRow["Description"] = "我很喜欢";
            databable_books.Rows.Add(newRow);

            newRow = databable_books.NewRow();
            newRow["Product"] = "c#";
            newRow["BookID"] = 2;
            newRow["Date"] = new DateTime(1922, 11, 16);
            newRow["Description"] = "very nice";
            databable_books.Rows.Add(newRow);

            newRow = databable_books.NewRow();
            newRow["Product"] = "java";
            newRow["BookID"] = 3;
            newRow["Date"] = new DateTime(1922, 11, 17);
            newRow["Description"] = "good id";
            databable_books.Rows.Add(newRow);

            return databable_books;
        }

        public static DataTable GetDatatable2()
        {
            DataTable databable_books = new DataTable("Books");


            SetupColumm(databable_books);

            DataRow newRow;
            newRow = databable_books.NewRow();
            newRow["Product"] = "c++";
            newRow["BookID"] = 1;
            newRow["Date"] = new DateTime(1922, 11, 17);
            newRow["Description"] = "我很喜欢1";
            databable_books.Rows.Add(newRow);

            newRow = databable_books.NewRow();
            newRow["Product"] = "c#";
            newRow["BookID"] = 2;
            newRow["Date"] = new DateTime(1922, 11, 17);
            newRow["Description"] = "very nice1";
            databable_books.Rows.Add(newRow);

            newRow = databable_books.NewRow();
            newRow["Product"] = "java";
            newRow["BookID"] = 3;
            newRow["Date"] = new DateTime(1922, 11, 17);
            newRow["Description"] = "good id1";
            databable_books.Rows.Add(newRow);

            newRow = databable_books.NewRow();
            newRow["Product"] = "js";
            newRow["BookID"] = 4;
            newRow["Date"] = new DateTime(1922, 11, 17);
            newRow["Description"] = "good id1";
            databable_books.Rows.Add(newRow);

            return databable_books;
        }
    }
}