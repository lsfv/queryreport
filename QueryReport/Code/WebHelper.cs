using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace QueryReport.Code
{
    /// <summary>
    /// 程序中编程，测试等相关的帮助类
    /// </summary>
    public abstract class WebHelper
    {
        public static CUSTOMRP.BLL.SOURCEVIEW bllSOURCEVIEW = new CUSTOMRP.BLL.SOURCEVIEW();
        public static CUSTOMRP.BLL.SensitivityLevel bllviewLevel = new CUSTOMRP.BLL.SensitivityLevel();
        public static CUSTOMRP.BLL.RPCATEGORY bllcategory = new CUSTOMRP.BLL.RPCATEGORY();
        public static CUSTOMRP.BLL.COMMON bllCommon = new CUSTOMRP.BLL.COMMON();
        public static CUSTOMRP.BLL.REPORTGROUP bllrpGroup = new CUSTOMRP.BLL.REPORTGROUP();
        public static CUSTOMRP.BLL.REPORT bllReport = new CUSTOMRP.BLL.REPORT();
        //public static CUSTOMRP.BLL.REPORTCOLUMN bllReportColumn = new CUSTOMRP.BLL.REPORTCOLUMN();
        public static CUSTOMRP.BLL.USER bllUSER = new CUSTOMRP.BLL.USER();
        //public static CUSTOMRP.BLL.V_DATABASE bllV_DATABASE = new CUSTOMRP.BLL.V_DATABASE();
        public static CUSTOMRP.BLL.USERGROUP bllUserGroup = new CUSTOMRP.BLL.USERGROUP();
        public static CUSTOMRP.BLL.GROUPRIGHT bllGroupRight = new CUSTOMRP.BLL.GROUPRIGHT();
        public static CUSTOMRP.BLL.DATABASE bllCompany = new CUSTOMRP.BLL.DATABASE();
        public static CUSTOMRP.BLL.WORDTEMPLATE bllWORDTEMPLATE = new CUSTOMRP.BLL.WORDTEMPLATE();
        public static CUSTOMRP.BLL.SOURCEVIEWCOLUMN bllSOURCEVIEWCOLUMN = new CUSTOMRP.BLL.SOURCEVIEWCOLUMN();
        public static CUSTOMRP.BLL.QUERYPARAMS bllQUERYPARAMS = new CUSTOMRP.BLL.QUERYPARAMS();
        public static CUSTOMRP.BLL.WORDFILE bllWordFile = new CUSTOMRP.BLL.WORDFILE();
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRight">eg add,delete,</param>
        /// <param name="function">eg add</param>
        /// <returns></returns>
        public static bool checkRight(string userRight, string function)
        {
            if (("," + userRight + ",").Contains("," + function + ",") == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        

        public static string changetext(string a)
        {
            a.Replace(Environment.NewLine.ToString(), "");
            string[] list = a.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Length; i++)
            {
                sb.Append(list[i].Split('=')[1] + "=" + list[i].Split('=')[0].Replace(Environment.NewLine.ToString(), "") + ";");
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 19820101 转为1982-01-01
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string stringToDateString(string time)
        {
            return time.Insert(4, "-").Insert(7, "-");
        }

        /// <summary>
        /// 当前时间转为如:1982-01-01
        /// </summary>
        /// <returns></returns>
        public static string nowToString()
        {
            //v1.0.0 - Cheong - 2015/02/13
            // There's two problem in this function
            // 1) Don't use DateTime.Now in .NETv4 as it's know to cause OverflowException when timezone has DST.
            // 2) Calling Now multiple times can result Going-Back-in-time impossible bug on last second of the year.
            //return DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "-" + DateTime.Now.Day.ToString().PadLeft(2, '0');
            return DateTime.Today.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        }

        public static int[] getColumnIndexByColumnName_OrderbyIndex(string[] ColumnName, DataColumnCollection Columns)
        {
            int[] Result = new int[ColumnName.Length];
            for (int i = 0; i < ColumnName.Length; i++)
            {
                foreach (DataColumn dc in Columns)
                {
                    if (ColumnName[i] == dc.ColumnName)
                    {
                        Result[i] = dc.Ordinal;
                    }
                }
            }

            Result = Common.Utils.intOrder(Result);
            return Result;
        }

        public static int[] getColumnIndexByColumnName(string[] ColumnName, DataColumnCollection Columns)
        {
            int[] Result = new int[ColumnName.Length];
            for (int i = 0; i < ColumnName.Length; i++)
            {
                foreach (DataColumn dc in Columns)
                {
                    if (ColumnName[i] == dc.ColumnName)
                    {
                        Result[i] = dc.Ordinal;
                    }
                }
            }

            return Result;
        }


        public static string GetAlertJS(string msg)
        {
            if (msg == null) { msg = ""; }
            return String.Format("<script type=\"text/javascript\">alert(\"{0}\");</script>", msg);
        }

        public static string GetJSModelShow()
        {
            string js = "<script>$(function() {$('#templatemgt').modal('show')});</script >";
            return js;
        }

    }
}