using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CUSTOMRP.Model;

namespace CUSTOMRP.BLL
{
    public abstract class AppHelper
    {
        private static CUSTOMRP.BLL.COMMON bllcommon = new COMMON();
        private static CUSTOMRP.BLL.SOURCEVIEW bllSOURCEVIEW = new SOURCEVIEW();

        private static Type[] decimalTypes = new Type[] { typeof(Decimal), typeof(Double), typeof(Single) };
        private static Type[] intTypes = new Type[] { typeof(Byte), typeof(Int16), typeof(Int32), typeof(Int64), typeof(SByte), typeof(UInt16), typeof(UInt32), typeof(UInt64) };

        // Common - Begin
        public enum ChartType
        {
            Pie = 0,
            Donut = 1,
            Bar = 2
            , Line = 3
        }

        public class QueryParamsObject
        {
            public string ParamName { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
            public string SqlType { get; set; }
        }


        //public class QueryParamsEmbedded
        //{
        //    public string ParamName { get; set; }
        //    public string SqlType { get; set; }
        //    public string Value { get; set; }
        //    public string[] EnumValues { get; set; }
        //}

        public class DashboardNameTypeObject
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }
        // Common - End

        // MODEL - Begin
        public sealed class t_QueryReportDashboard
        {
            //private int _ID;
            //private string _Title;
            //private int _Type;
            //private string _View;
            //private string _GroupBy;

            public int ID { get; set; }
            public string Title { get; set; }
            public int Type { get; set; }
            public string View { get; set; }
            public string GroupBy { get; set; }
            public int AggregateType { get; set; }
            public string AggregateColumn { get; set; }

        }
        // MODEL - End


        // Alex - Should return error msg (if any) instead of void - Begin
        public static int AddNewChart(int UserID, string DBName, t_QueryReportDashboard newChart)
        {
            string query = string.Format("INSERT INTO {0}.dbo.DASHBOARD ([Title], [Type], [View], [GroupBy], [AggregateType], [AggregateColumn], [UserID]) VALUES (@Title, @Type, @View, @GroupBy, @AggregateType, @AggregateColumn, @UserID); select @@IDENTITY", DBName);
            SqlParameter[] parameters = {
                new SqlParameter("@Title", SqlDbType.NVarChar,-1),
                new SqlParameter("@Type", SqlDbType.TinyInt),
                new SqlParameter("@View", SqlDbType.VarChar, 100),
                new SqlParameter("@GroupBy", SqlDbType.VarChar, 100),
                new SqlParameter("@AggregateType", SqlDbType.TinyInt),
                new SqlParameter("@AggregateColumn", SqlDbType.VarChar, 100),
                new SqlParameter("@UserID", SqlDbType.Int)
            };
            parameters[0].Value = newChart.Title;
            parameters[1].Value = newChart.Type;
            parameters[2].Value = newChart.View;
            parameters[3].Value = newChart.GroupBy;
            parameters[4].Value = newChart.AggregateType;
            parameters[5].Value = newChart.AggregateColumn;
            parameters[6].Value = UserID;

            object obj = CUSTOMRP.DAL.DbHelperSQL.GetSingle(UserID, query, parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        public static void DeleteChart(int UserID, string DBName, int ID)
        {
            string query = string.Format("DELETE FROM {0}.dbo.DASHBOARD WHERE ID = @ID;", DBName);
            var id_param = new SqlParameter("@ID", SqlDbType.Int);
            id_param.Value = ID;
            CUSTOMRP.DAL.DbHelperSQL.ExecuteSql(UserID, query, id_param);
            //if (obj == null)
            //{
            //    return 0;
            //}
            //else
            //{
            //    return Convert.ToInt32(obj);
            //}
        }


        public static int CopyReport(int UserID, int RPID) {
            return (int)bllcommon.GetSingle(UserID, string.Format("dbo.sp_DuplicateReport {0}", RPID));
        }

        public static List<QueryParamsObject> GetQueryParams(int UserID, string DBName, string view)
        {
            DataTable dt = bllcommon.query(UserID, string.Format("use {1}; select s.name as param_name, t.name as sql_type from {1}.sys.parameters s " +
            "inner join {1}.sys.types t on s.user_type_id=t.user_type_id " +
            "where s.object_id = object_id('{0}') and left(s.name,12) = '@QueryParam_'", view, DBName));
            var convertedList = (from rw in dt.AsEnumerable()
                                 select new QueryParamsObject()
                                 {
                                     ParamName = Convert.ToString(rw["param_name"]),
                                     SqlType = Convert.ToString(rw["sql_type"])
                                 }).ToList();
            return convertedList;
        }

        public static String[] ParamGetEnumValues(int UserID, string DBName, string sourceName, int sourceType, string columnName)
        {
            try
            {
                switch (sourceType)
                {
                    case 0:             // View
                        DataTable dt = bllcommon.query(UserID, string.Format("use {2}; declare @sql nvarchar(max);" +
                        "set @sql=N'select distinct [{1}] as r from [qreport].[{0}] ORDER BY r'; EXEC(@sql)",
                        sourceName, columnName, DBName));
                        var convertedList = (from rw in dt.AsEnumerable()
                                             select Convert.ToString(rw["r"])).ToArray();
                        return convertedList;
                    case 2:             // Stored procedure
                                        // Slow, but there is no better way than this
                        string[] colnames = GetColumnNamesForStoredProc(UserID, DBName, sourceName);    // Needs the whole, unmodified schema
                        StringBuilder sb = new StringBuilder(string.Format("use {0}; DECLARE @Schema TABLE (", DBName));
                        for (int x = 0; x < colnames.Length; x++)
                        {
                            sb.Append(string.Format("[{0}] NVARCHAR(MAX){1}", colnames[x], x == colnames.Length - 1 ? String.Empty : ",")); // Always cast to NVARCHAR(MAX)
                        }
                        sb.Append(string.Format("); INSERT INTO @Schema EXEC [qreport].[{0}]; SELECT DISTINCT [{1}] AS r FROM @Schema ORDER BY r", sourceName, columnName));
                        DataTable dt_2 = bllcommon.query(UserID, sb.ToString());
                        var convertedList_2 = (from rw in dt_2.AsEnumerable() select Convert.ToString(rw["r"])).ToArray();
                        return convertedList_2;
                    default:
                        return new string[] { };
                }
            }
            catch (Exception ex)
            {
                Common.JScript.AlertAndRedirect("Browse values is not available for this query. Please input values manually."+ex.Message, "rpexcel2.aspx"); //++
                return new string[] { };
            }
        }

        public static String[] QueryParamGetEnumValues(int UserID, string DBName, string tableOrViewObjectID, string columnName)
        {
            DataTable dt = bllcommon.query(UserID, string.Format("use {2}; declare @sql nvarchar(max);" +
            "set @sql=N'select distinct [{1}] as r from ' + OBJECT_SCHEMA_NAME({0}) + N'.' + OBJECT_NAME({0}) + N' ORDER BY r'; EXEC(@sql)",
            tableOrViewObjectID, columnName, DBName));
            var convertedList = (from rw in dt.AsEnumerable()
                                 select Convert.ToString(rw["r"])).ToArray();
            return convertedList;
        }


        public static List<t_QueryReportDashboard> GetCharts(int UserID, string DBName)
        {
            DataTable dt = bllcommon.query(UserID, string.Format("SELECT * FROM {0}.dbo.DASHBOARD WHERE UserID = {1}", DBName, UserID));
            var convertedList = (from rw in dt.AsEnumerable()
                                 select new t_QueryReportDashboard()
                                 {
                                     ID = Convert.ToInt32(rw["ID"]),
                                     Title = Convert.ToString(rw["Title"]),
                                     Type = Convert.ToInt32(rw["Type"]),
                                     View = Convert.ToString(rw["View"]),
                                     GroupBy = Convert.ToString(rw["GroupBy"]),
                                     AggregateType = Convert.ToInt32(rw["AggregateType"]),
                                     AggregateColumn = Convert.ToString(rw["AggregateColumn"])
                                 }).ToList();
            return convertedList;
        }

        public static bool DashbordDistinctCount(int UserID, string DBName, string tableOrViewName, string groupByName)
        {
            DataTable dt = bllcommon.query(UserID, string.Format("SELECT COUNT(DISTINCT [{1}]) AS c FROM {2}.qreport.{0}", tableOrViewName, groupByName, DBName));
            if (Convert.ToInt32(dt.Rows[0][0]) > 20)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static DataTable DashboardAggregation(int UserID, string DBName, string tableOrViewName, string groupByName, int aggregateType, string aggregateColumn)
        {
            string op;
            switch (aggregateType)
            {
                case 0:
                    op = "COUNT";
                    aggregateColumn = groupByName;
                    break;
                case 1:
                    op = "SUM";
                    break;
                case 2:
                    op = "AVG";
                    break;
                case 3:
                    op = "MAX";
                    break;
                case 4:
                    op = "MIN";
                    break;
                default:
                    op = "COUNT";
                    break;
            }
            DataTable result = bllcommon.query(UserID, string.Format("SELECT [{1}], {3}([{4}]) FROM {2}.qreport.{0} GROUP BY [{1}]", tableOrViewName, groupByName, DBName, op, aggregateColumn));
            result.TableName = tableOrViewName;
            return result;
        }

        // groupByName is now dateTimeColumn, aggregateColumn is now countColumn
        public static DataTable DashboardDateTimeAggregation(int UserID, string DBName, string tableOrViewName, string dateTimeColumn, string countColumn)
        {
            DataTable result = bllcommon.query(UserID, string.Format("SELECT [{1}], [{2}] FROM {3}.qreport.{0} GROUP BY [{1}]", tableOrViewName, dateTimeColumn, countColumn, DBName));
            result.TableName = tableOrViewName;
            return result;
        }
        public static string[] GetQueryReportViewNames(int UserID, string DBName)
        {
            DataTable dt = bllcommon.query(UserID, string.Format("USE {0}; SELECT v.TABLE_SCHEMA + '.' + v.TABLE_NAME AS a FROM {0}.INFORMATION_SCHEMA.VIEWS v WHERE v.TABLE_SCHEMA = 'qreport' ORDER BY a ASC", DBName));
            var result = dt.AsEnumerable()
                           .Select(r => r.Field<string>("a"))
                           .ToArray();
            return result;
        }

        public static Dictionary<string, string> GetColumnNamesByView(int UserID, string DBName, String view)
        {
            DataTable dt = bllcommon.query(UserID, string.Format("SELECT DISTINCT COLUMN_NAME, DATA_TYPE FROM {0}.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = STUFF('{1}',1,8,'')", DBName, view));
            var convertedList = dt.AsEnumerable().ToDictionary<DataRow, string, string>(rw => Convert.ToString(rw["COLUMN_NAME"]),
                                                                                        rw => Convert.ToString(rw["DATA_TYPE"]));
            return convertedList;
        }

        public static bool ReportCheckFieldsAreNumeric(int UserID, string DBName, String view, List<string> fields)
        {
            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < fields.Count; i++)
            {
                sb.Append("'");
                sb.Append(fields[i]);
                sb.Append("'");
                if (i == fields.Count - 1)
                {
                    sb.Append(",");
                }
            }
            DataTable dt = bllcommon.query(UserID, string.Format("SELECT CASE WHEN( SELECT COUNT(DISTINCT COLUMN_NAME) FROM {0}.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{1}' AND COLUMN_NAME IN ({2}) AND DATA_TYPE IN ('smallint', 'int', 'float', 'bigint')) = ( SELECT COUNT(DISTINCT COLUMN_NAME) FROM {0}.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{1}' AND (COLUMN_NAME IN ({2})) ) THEN 1 ELSE 0 END AS EqualOrNot"
                , DBName, view, sb.ToString()));
            return Convert.ToBoolean(dt.Rows[0][0]);
        }

        // Alex - Should return error msg (if any) instead of void - End

        public static string[] GetViewFromDB(int UserID, string DBName)
        {
            List<string> result = new List<string>();
            DataTable dt = bllcommon.query(UserID, String.Format("SELECT v.[TABLE_NAME] FROM [{0}].[INFORMATION_SCHEMA].[VIEWS] v WHERE v.[TABLE_SCHEMA] = 'qreport'", DBName));
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(dr[0] as string);
            }
            return result.ToArray();
        }

        public static string[] GetStoredProcFromDB(int UserID, string DBName)
        {
            List<string> result = new List<string>();
            DataTable dt = bllcommon.query(UserID, String.Format("SELECT r.[SPECIFIC_NAME] FROM [{0}].[INFORMATION_SCHEMA].[ROUTINES] r WHERE r.[ROUTINE_TYPE] = 'PROCEDURE' AND r.[SPECIFIC_SCHEMA] = 'qreport'", DBName));
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(dr[0] as string);
            }
            return result.ToArray();
        }

        public static string CheckSVInUse(int UserID, int svid, int type){
            StringBuilder builder = new StringBuilder();
            DataTable dt =  bllcommon.query(UserID, string.Format("SELECT [REPORTNAME] FROM [REPORT] WHERE SVID = {0} AND TYPE = {1}", svid, type));
            foreach (DataRow dr in dt.Rows)
            { 
                builder.Append("• ");
                builder.Append(dr[0] as string);
                builder.Append("\\n");
            }
            return builder.ToString();
        }

        public static string[] GetColumnNamesForTblView(int UserID, string DBName, string tblviewname)
        {
            List<string> result = new List<string>();
            DataTable dt = bllcommon.query(UserID, String.Format("SELECT c.[COLUMN_NAME] FROM [{0}].[INFORMATION_SCHEMA].[COLUMNS] c WHERE c.[TABLE_SCHEMA] = 'qreport' AND c.[TABLE_NAME] = '{1}' ORDER BY c.[ORDINAL_POSITION]", DBName, tblviewname));
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(dr[0] as string);
            }
            return result.ToArray();
        }

        public static string[] GetColumnNamesForStoredProc(int UserID, string DBName, string spName)
        {
            // use old method if "_columns" SP does not exist
            if (0 == Convert.ToInt32(bllcommon.GetSingle(UserID, String.Format("SELECT COUNT(*) FROM [{0}].[INFORMATION_SCHEMA].[ROUTINES] WHERE [SPECIFIC_SCHEMA] = 'qreport' AND [SPECIFIC_NAME] = '{1}' + '_columns'", DBName, spName))))
            {
                return GetColumnNamesForStoredProc_Internal(UserID, DBName, spName);
            }

            // SP exists
            List<string> result = new List<string>();
            DataTable dt = bllcommon.RunSP(UserID, String.Format("[{0}].[qreport].[{1}]", DBName, spName + "_columns"), new SqlParameter[0]);
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(dr["ColumnName"] as string);
            }
            return result.ToArray();
        }

        private static string[] GetColumnNamesForStoredProc_Internal(int UserID, string DBName, string spName)
        {
            List<string> result = new List<string>();

            //v1.0.0 Fai 2015.04.02 - SQLParam from 2000 > Max
            DataTable dt = bllcommon.RunSP(UserID, String.Format("[{0}].[qreport].[{1}]", DBName, spName), new SqlParameter[2]{
               new SqlParameter(){ ParameterName = "@cols", SqlDbType = SqlDbType.NVarChar, Size = -1, Value = "*"},
               new SqlParameter(){ ParameterName = "@cmd", SqlDbType = SqlDbType.NVarChar, Size = -1, Value = "AND 1 = 2"},
            });
            foreach (DataColumn col in dt.Columns)
            {
                result.Add(col.ColumnName as string);
            }
            return result.ToArray();
        }

        public static List<ColumnInfo> GetColumnInfoForTblView(int UserID, string DBName, string tblviewname)
        {
            List<ColumnInfo> result = new List<ColumnInfo>();
            DataTable dt = bllcommon.query(UserID, String.Format("SELECT ColName = c.[COLUMN_NAME],"
                + " DataType = CASE c.DATA_TYPE"
                + "   WHEN 'bit' THEN 'Int'"
                + "   WHEN 'int' THEN 'Int'"
                + "   WHEN 'smallint' THEN 'Int'"
                + "   WHEN 'tinyint' THEN 'Int'"
                + "   WHEN 'datetime' THEN 'DateTime'"
                + "   WHEN 'smalldatetime' THEN 'DateTime'"
                + "   WHEN 'float' THEN 'Decimal'"
                + "   WHEN 'real' THEN 'Decimal'"
                + "   WHEN 'numeric' THEN 'Decimal'"
                + "   ELSE 'String'"
                + "   END"
                + " FROM [{0}].[INFORMATION_SCHEMA].[COLUMNS] c WHERE c.[TABLE_SCHEMA] = 'qreport' AND c.[TABLE_NAME] = '{1}' ORDER BY c.[ORDINAL_POSITION]", DBName, tblviewname));

            foreach (DataRow dr in dt.Rows)
            {
                result.Add(new ColumnInfo()
                {
                    ColName = dr["ColName"] as string,
                    DisplayName = dr["ColName"] as string,
                    DataType = dr["DataType"] as string,
                });
            }

            return result;
        }


        public static List<ColumnInfo> GetColumnInfoForStoredProc(int UserID, string DBName, int svid)
        {
            // v1.8.8 Alex 2019.03.18 - Not refreshed
            List<ColumnInfo> result = new List<ColumnInfo>();

            DataTable dt = bllcommon.query(UserID, string.Format("SELECT * FROM SOURCEVIEWCOLUMN WHERE SVID = {0}", svid));
            var convertedList = (from rw in dt.AsEnumerable()
                                 select new ColumnInfo()
                                 {
                                     ColName = Convert.ToString(rw["COLUMNNAME"]),
                                     DisplayName = Convert.ToString(rw["DISPLAYNAME"]),
                                     DataType = Convert.ToString(rw["DATA_TYPE"])
                                 }).ToList();
            return convertedList;
        }

        // v1.8.8 Alex 2019.03.18 - Refresh only
        public static List<ColumnInfo> GetColumnInfoForStoredProcRefresh(int UserID, string DBName, string spName, bool IgnoreTable = true)
        {
            // v1.8.8 Alex 2019.03.18 - Refresh only
            // use old method if "_columns" SP does not exist
            if (0 == Convert.ToInt32(bllcommon.GetSingle(UserID, String.Format("SELECT COUNT(*) FROM [{0}].[INFORMATION_SCHEMA].[ROUTINES] WHERE [SPECIFIC_SCHEMA] = 'qreport' AND [SPECIFIC_NAME] = '{1}' + '_columns'", DBName, spName))))
            {
                return GetColumnInfoForStoredProc_Internal(UserID, DBName, spName);
            }

            // SP exists
            List<string> columns = new List<string>();
            DataTable dt = bllcommon.RunSP(UserID, String.Format("[{0}].[qreport].[{1}]", DBName, spName + "_columns"), new SqlParameter[0]);
            foreach (DataRow dr in dt.Rows)
            {
                //v1.0.0 - Cheong - 2016/03/29
                if (!IgnoreTable || (!"TABLE".Equals(dr["DataType"])))
                {
                    columns.Add(String.Format("[{0}]", dr["ColumnName"] as string));
                }
            }
            return GetColumnInfoForStoredProc_Internal(UserID, DBName, spName, String.Join(",", columns));
        }

        private static List<ColumnInfo> GetColumnInfoForStoredProc_Internal(int UserID, string DBName, string spName, string columns = "*")
        {
            List<ColumnInfo> result = new List<ColumnInfo>();

            //v1.0.0 Fai 2015.04.02 - SQLParam from 2000 > Max
            DataTable dt = bllcommon.RunSP(UserID, String.Format("[{0}].[qreport].[{1}]", DBName, spName), new SqlParameter[2]{
               new SqlParameter(){ ParameterName = "@cols", SqlDbType = SqlDbType.NVarChar, Size = -1, Value = columns},
               new SqlParameter(){ ParameterName = "@cmd", SqlDbType = SqlDbType.NVarChar, Size = -1, Value = "AND 1 = 2"},
            });

            foreach (DataColumn col in dt.Columns)
            {
                result.Add(new ColumnInfo()
                {
                    ColName = col.ColumnName as string,
                    DisplayName = col.ColumnName as string,
                    DataType =
                        intTypes.Contains(col.DataType) ? "Int" :
                        decimalTypes.Contains(col.DataType) ? "Decimal" :
                        col.DataType == typeof(DateTime) ? "DateTime" :
                        "String"
                });
            }
            return result;
        }

        private static string GetSqlForTblView(int UserID, string DBName, string tblviewname)
        {
            string[] cols = GetColumnNamesForTblView(UserID, DBName, tblviewname);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT [{0}]", cols[0]);
            for (int i = 1; i < cols.Length; i++)
            {
                sb.AppendFormat(", [{0}]", cols[i]);
            }
            sb.AppendFormat(" FROM [{0}].[qreport].[{1}]", DBName, tblviewname);
            return sb.ToString();
        }

        public static DataTable getSecurityForUser(int UserID, string DBName, string username)
        {
            DataTable dt = bllcommon.SafeQuery(UserID, String.Format("SELECT * FROM {0} WHERE UserName = '{1}'", String.Format("[{0}].[qreport].[v_Security]", DBName), username));
            return dt;
        }

        public static DataTable getDataForReport(int UserID, ref List<string> comments, ref List<string> avgs, ref List<string> sums, ref List<string> groups,
            ref List<string> subtotal, ref List<string> subavg, ref  List<string> subcount, ref List<string> counts, CUSTOMRP.Model.LoginUser me,
            int rpid, bool f_distinct, ReportCriteria[] rpParam = null, List<SqlParameter> queryParams = null)  // version used in rpembedded
        {
            DataTable dt = null;
            CUSTOMRP.BLL.REPORT bllReport = new CUSTOMRP.BLL.REPORT();
            CUSTOMRP.Model.REPORT report = bllReport.GetModel(UserID, rpid);

            if (report == null) { return dt; }

            CUSTOMRP.BLL.SOURCEVIEW bllSourceView = new CUSTOMRP.BLL.SOURCEVIEW();
            CUSTOMRP.Model.SOURCEVIEW sv = bllSourceView.GetModel(UserID, report.SVID);

            #region get column names

            string[] colnames = null;
            switch (sv.SourceType)
            {
                case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.View:
                case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.Table:
                    {
                        colnames = AppHelper.GetColumnNamesForTblView(me.ID, me.DatabaseNAME, sv.TBLVIEWNAME);
                    }
                    break;
                case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.StoredProc:
                    {
                        colnames = AppHelper.GetColumnNamesForStoredProc(me.ID, me.DatabaseNAME, sv.TBLVIEWNAME);
                    }
                    break;
            }

            #endregion

            //CUSTOMRP.BLL.REPORTCOLUMN bllReportColumn = new CUSTOMRP.BLL.REPORTCOLUMN();
            //v1.0.0 - Cheong - 2015/03/31 - Filter missing columns from list when running reports
            //v1.0.0 - Cheong - 2015/05/29 - Do not filter out formulas
            //List<CUSTOMRP.Model.REPORTCOLUMN> sclist = bllReportColumn.getCriteriaColumnModelList(rpid).Where(x => colnames.Contains(x.COLUMNNAME)).ToList();
            List<CUSTOMRP.Model.REPORTCOLUMN> sclist = bllReport.getCriteriaColumnModelList(UserID, rpid).Where(x => colnames.Contains(x.COLUMNNAME) || x.ColumnType == Model.REPORTCOLUMN.ColumnTypes.Formula).ToList();

            #region Original GetDataForReport

            List<CUSTOMRP.Model.REPORTCOLUMN> selectColumns = sclist.Where(x => x.COLUMNFUNC == 1).ToList();
            List<CUSTOMRP.Model.REPORTCOLUMN> criterialColumns = sclist.Where(x => x.COLUMNFUNC == 2).ToList();
            List<CUSTOMRP.Model.REPORTCOLUMN> sortColumns = sclist.Where(x => x.COLUMNFUNC == 3).ToList();
            List<CUSTOMRP.Model.REPORTCOLUMN> avgColumns = sclist.Where(x => x.COLUMNFUNC == 4).ToList();
            List<CUSTOMRP.Model.REPORTCOLUMN> sumColumns = sclist.Where(x => x.COLUMNFUNC == 5).ToList();
            List<CUSTOMRP.Model.REPORTCOLUMN> groupColumns = sclist.Where(x => x.COLUMNFUNC == 6).ToList();
            List<CUSTOMRP.Model.REPORTCOLUMN> subtotalColumns = sclist.Where(x => x.COLUMNFUNC == 7).ToList();
            List<CUSTOMRP.Model.REPORTCOLUMN> subavgColumns = sclist.Where(x => x.COLUMNFUNC == 8).ToList();
            List<CUSTOMRP.Model.REPORTCOLUMN> subcountColumns = sclist.Where(x => x.COLUMNFUNC == 9).ToList();
            List<CUSTOMRP.Model.REPORTCOLUMN> countsColumns = sclist.Where(x => x.COLUMNFUNC == 10).ToList();

            if (rpParam != null)
            {
                foreach (ReportCriteria sl in rpParam)
                {
                    if (!string.IsNullOrEmpty(sl.CRITERIA1))
                    {
                        //v1.0.0 Fai 2015.04.22 - Use StartWith instead of Replace, it will affect Between Operator - Begin
                        //comments.Add(sl.CRITERIA1.Replace(" and ", ""));
                        comments.Add(sl.CRITERIA1.StartsWith(" and ") ? sl.CRITERIA1.Substring(5, sl.CRITERIA1.Length - 5) : sl.CRITERIA1);
                        //v1.0.0 Fai 2015.04.22 - Use StartWith instead of Replace, it will affect Between Operator - End
                    };
                }
            }
            else
            {
                foreach (CUSTOMRP.Model.REPORTCOLUMN sl in criterialColumns)
                {
                    if (!string.IsNullOrEmpty(sl.CRITERIA1))
                    {
                        //v1.0.0 Fai 2015.04.22 - Use StartWith instead of Replace, it will affect Between Operator - Begin
                        //comments.Add(sl.CRITERIA1.Replace(" and ", ""));
                        comments.Add(sl.CRITERIA1.StartsWith(" and ") ? sl.CRITERIA1.Substring(5, sl.CRITERIA1.Length - 5) : sl.CRITERIA1);
                        //v1.0.0 Fai 2015.04.22 - Use StartWith instead of Replace, it will affect Between Operator - End
                    };
                }
            }

            foreach (CUSTOMRP.Model.REPORTCOLUMN sl in avgColumns)
            {
                avgs.Add(sl.COLUMNNAME);
            }
            foreach (CUSTOMRP.Model.REPORTCOLUMN sl in sumColumns)
            {
                sums.Add(sl.COLUMNNAME);
            }
            foreach (CUSTOMRP.Model.REPORTCOLUMN sl in groupColumns)
            {
                groups.Add(sl.COLUMNNAME);
            }
            foreach (CUSTOMRP.Model.REPORTCOLUMN sl in subtotalColumns)
            {
                subtotal.Add(sl.COLUMNNAME);
            }
            foreach (CUSTOMRP.Model.REPORTCOLUMN sl in subavgColumns)
            {
                subavg.Add(sl.COLUMNNAME);
            }
            foreach (CUSTOMRP.Model.REPORTCOLUMN sl in subcountColumns)
            {
                subcount.Add(sl.COLUMNNAME);
            }
            foreach (CUSTOMRP.Model.REPORTCOLUMN sl in countsColumns)
            {
                counts.Add(sl.COLUMNNAME);
            }

            #endregion

            if (selectColumns.Count == 0) { return dt; }    // null

            //v1.0.0 - Cheong - 2015/05/29 - Allow the use of formula
            //StringBuilder colsString = new StringBuilder();
            //colsString.AppendFormat("[{0}]", selectColumns[0].COLUMNNAME);
            //for (int i = 1; i < selectColumns.Count; i++)
            //{
            //    colsString.AppendFormat(", [{0}]", selectColumns[i].COLUMNNAME);
            //}
            StringBuilder colsString = new StringBuilder((f_distinct ? "DISTINCT " : String.Empty) + String.Join(", ", selectColumns.Select(x => x.SelectStatement).ToArray()));

            StringBuilder sortString = new StringBuilder();
            if (sortColumns.Count > 0)
            {
                sortString.AppendFormat(" ORDER BY [{0}]", sortColumns[0].COLUMNNAME);
                for (int i = 1; i < sortColumns.Count; i++)
                {
                    sortString.AppendFormat(", [{0}]", sortColumns[i].COLUMNNAME);
                }
            }

            StringBuilder criteriastring = new StringBuilder();
            if (comments.Count > 0)
            {
                for (int i = 0; i < comments.Count; i++)
                {
                    criteriastring.AppendFormat(" AND {0}", comments[i]);
                }
            }
            switch (sv.SourceType)
            {
                case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.StoredProc:
                    {
                        //v1.8.8 Alex 2018.10.29
                        if (queryParams == null) queryParams = new List<SqlParameter>();
                        queryParams.Add(new SqlParameter() { ParameterName = "@cols", SqlDbType = SqlDbType.NVarChar, Size = -1, Value = colsString.ToString() });
                        queryParams.Add(new SqlParameter()
                        {
                            ParameterName = "@cmd",
                            SqlDbType = SqlDbType.NVarChar,
                            Size = -1,
                            Value =
                                sql_plus(report.SVID, me)
                                + criteriastring.ToString()
                                + sortString.ToString()
                        });

                        //v1.0.0 Fai 2015.04.02 - SQLParam from 2000 > Max
                        dt = bllcommon.SafeRunSP(UserID, String.Format("[{0}].[qreport].[{1}]", me.DatabaseNAME, sv.TBLVIEWNAME), queryParams.ToArray());
                    }
                    break;
                default:
                    {
                        dt = bllcommon.SafeQuery(UserID, String.Format("SELECT {0} FROM {1} WHERE 1=1 {2}", colsString.ToString(),
                            String.Format("[{0}].[qreport].[{1}]", me.DatabaseNAME, sv.TBLVIEWNAME),
                            sql_plus(report.SVID, me)
                                + criteriastring.ToString()
                                + sortString.ToString()));
                    }
                    break;
            }

            //v1.0.0 Fai 2015.04.16 - Distinct Value - Begin
            string[] l_strColumnNameArray = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
            if (l_strColumnNameArray.Length != l_strColumnNameArray.Distinct().Count())
            {
                dt = dt.DefaultView.ToTable(true, l_strColumnNameArray);
            }
            //v1.0.0 Fai 2015.04.16 - Distinct Value - End

            return dt;
        }

        public static DataTable getDataForReport(int UserID, int svid, string DatabaseName, string colsString, string sqlplus, string criteriastring, string sortString, bool f_distinct, List<SqlParameter> queryParams = null)
        {
            DataTable dt = null;

            if (String.IsNullOrWhiteSpace(colsString)) { return dt; }    // null

            CUSTOMRP.BLL.SOURCEVIEW bllSourceView = new CUSTOMRP.BLL.SOURCEVIEW();
            CUSTOMRP.Model.SOURCEVIEW sv = bllSourceView.GetModel(UserID, svid);

            switch (sv.SourceType)
            {
                case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.StoredProc:
                    {
                        if (queryParams == null) queryParams = new List<SqlParameter>();
                        queryParams.Add(new SqlParameter() { ParameterName = "@cols", SqlDbType = SqlDbType.NVarChar, Size = -1, Value = (f_distinct ? "DISTINCT " : String.Empty) + colsString });
                        queryParams.Add(new SqlParameter()
                        {
                            ParameterName = "@cmd",
                            SqlDbType = SqlDbType.NVarChar,
                            Size = -1,
                            Value =
                                (sqlplus + criteriastring + sortString)
                        });

                        //v1.0.0 Fai 2015.04.01 - SQLParam from 2000 > Max
                        dt = bllcommon.SafeRunSP(UserID, String.Format("[{0}].[qreport].[{1}]", DatabaseName, sv.TBLVIEWNAME), queryParams.ToArray());
                        ////v1.0.0 Fai 2015.04.01 - SQLParam from 2000 > Max
                        //dt = bllcommon.SafeRunSP(UserID, String.Format("[{0}].[qreport].[{1}]", DatabaseName, sv.TBLVIEWNAME), new SqlParameter[2]{
                        //    new SqlParameter(){ ParameterName = "@cols", SqlDbType = SqlDbType.NVarChar, Size = -1, Value = (f_distinct ? "DISTINCT " : String.Empty) + colsString},
                        //    new SqlParameter(){ ParameterName = "@cmd", SqlDbType = SqlDbType.NVarChar, Size = -1, Value =
                        //        (sqlplus + criteriastring + sortString)},
                        //});
                    }
                    break;
                default:
                    {
                        dt = bllcommon.SafeQuery(UserID, String.Format("SELECT {0} FROM {1} WHERE 1=1 {2}", (f_distinct ? "DISTINCT " : String.Empty) + colsString,
                            String.Format("[{0}].[qreport].[{1}]", DatabaseName, sv.TBLVIEWNAME),
                            (sqlplus + criteriastring + sortString)));
                    }
                    break;
            }

            //v1.0.0 Fai 2015.04.16 - Distinct Value - Begin
            string[] l_strColumnNameArray = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
            if (l_strColumnNameArray.Length != l_strColumnNameArray.Distinct().Count())
            {
                dt = dt.DefaultView.ToTable(true, l_strColumnNameArray);
            }
            //v1.0.0 Fai 2015.04.16 - Distinct Value - End

            return dt;
        }


        public static int CheckFormulaForReport(int UserID, int svid, string DatabaseName, string fieldname, string strformula)
        {
            int result = 0;

            DataTable dt = null;

            if (String.IsNullOrWhiteSpace(strformula)) { return result; }    // null

            try
            {
                CUSTOMRP.BLL.SOURCEVIEW bllSourceView = new CUSTOMRP.BLL.SOURCEVIEW();
                CUSTOMRP.Model.SOURCEVIEW sv = bllSourceView.GetModel(UserID, svid);

                switch (sv.SourceType)
                {
                    case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.StoredProc:
                        {
                            //v1.0.0 Fai 2015.04.01 - SQLParam from 2000 > Max
                            dt = bllcommon.SafeRunSP(UserID, String.Format("[{0}].[qreport].[{1}]", DatabaseName, sv.TBLVIEWNAME), new SqlParameter[2]{
                                new SqlParameter(){ ParameterName = "@cols", SqlDbType = SqlDbType.NVarChar, Size = -1, Value = String.Format(" TOP 1 {0} ", strformula)},
                                //v1.1.0 - Cheong - 2016/05/10 - Performance tuning for slow query when editing formula
                                //new SqlParameter(){ ParameterName = "@cmd", SqlDbType = SqlDbType.NVarChar, Size = -1, Value = String.Empty},
                                new SqlParameter(){ ParameterName = "@cmd", SqlDbType = SqlDbType.NVarChar, Size = -1, Value = String.Empty},
                            });
                        }
                        break;
                    default:
                        {
                            dt = bllcommon.SafeQuery(UserID, String.Format("SELECT {0} FROM {1} WHERE 1=1 {2}", String.Format(" TOP 1 {0} ", strformula),
                                String.Format("[{0}].[qreport].[{1}]", DatabaseName, sv.TBLVIEWNAME),
                                String.Empty));
                        }
                        break;
                }

                if ((dt.Rows.Count > 0) && (dt.Columns[0].ColumnName == fieldname))
                {
                    double _;
                    if ((dt.Rows[0][0] == null) || double.TryParse(Convert.ToString(dt.Rows[0][0]), out _))
                    {
                        result = 2;       // 2: Looks 'numeric'
                    }
                    else
                    {
                        result = 1;       // 1: Absolutely not 'numeric'
                    }
                }
            }
            catch
            {
                // ignore all errors, just return false.
            }

            return result;
        }



        public static string sql_plus(int svid, LoginUser me)
        {
            CUSTOMRP.Model.SOURCEVIEW sv = bllSOURCEVIEW.GetModel(me.ID, svid);

            string[] columns = null;
            StringBuilder result = new StringBuilder();

            switch (sv.SourceType)
            {
                case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.StoredProc:
                    {
                        columns = GetColumnNamesForStoredProc(me.ID, me.DatabaseNAME, sv.TBLVIEWNAME);
                    }
                    break;
                default:
                    {
                        columns = GetColumnNamesForTblView(me.ID, me.DatabaseNAME, sv.TBLVIEWNAME);
                    }
                    break;
            }

            int dtCount = columns.Length;

            for (int i = dtCount - 1; i >= 0; i--)
            {
                #region OldCode
                //if (columns[i].ToUpper() == "COMPANYID")
                //{
                //    if (me.companyid != "")
                //    {
                //        result += " and [COMPANYID] in (" + me.companyid + ")";
                //    }
                //}
                //else if (columns[i].ToUpper() == "SECURITYGROUPID")
                //{
                //    if (me.securitygroupid != "")
                //    {
                //        result += " and [SECURITYGROUPID] in (" + me.securitygroupid + ")";
                //    }
                //}

                //else if (columns[i].ToUpper() == "CONTRACTID")
                //{
                //    if (me.contractid != "")
                //    {
                //        result += " and [CONTRACTID] in (" + me.contractid + ")";
                //    }
                //}
                #endregion OldCode

                if (me.UserCriteria.ContainsKey(columns[i].ToUpper()))
                {
                    result.AppendFormat(" and [{0}] in ({1})", columns[i].ToUpper(), me.UserCriteria[columns[i].ToUpper()]); //wtf?
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Get the column type for specified column.
        /// </summary>
        /// <param name="svid">SVID</param>
        /// <param name="svname"></param>
        /// <param name="columnName"></param>
        /// <param name="me"></param>
        /// <returns></returns>
        public static string getColumnType(int svid, string svname, string columnName, CUSTOMRP.Model.LoginUser me)
        {
            CUSTOMRP.Model.SOURCEVIEW sv = bllSOURCEVIEW.GetModel(me.ID, svid);

            // Use old method if table / view only
            if ((sv.SourceType != CUSTOMRP.Model.SOURCEVIEW.SourceViewType.StoredProc)
                || (0 == Convert.ToInt32(bllcommon.GetSingle(me.ID, String.Format("SELECT COUNT(*) FROM [{0}].[INFORMATION_SCHEMA].[ROUTINES] WHERE [SPECIFIC_SCHEMA] = 'qreport' AND [SPECIFIC_NAME] = '{1}' + '_columns'", me.DatabaseNAME, sv.TBLVIEWNAME)))))
            {
                return getColumnType_Internal(sv, svname, columnName, me);
            }

            // SP exists
            string result = "String";   // default to string
            DataTable dt = bllcommon.RunSP(me.ID, String.Format("[{0}].[qreport].[{1}]", me.DatabaseNAME, sv.TBLVIEWNAME + "_columns"), new SqlParameter[0]);
            foreach (DataRow dr in dt.Rows)
            {
                if (columnName.Equals(dr["ColumnName"]))
                {
                    result = dr["DataType"] as string;
                }
            }
            return result;
        }

        private static string getColumnType_Internal(CUSTOMRP.Model.SOURCEVIEW sv, string svname, string columnName, CUSTOMRP.Model.LoginUser me)
        {
            string result = "";

            DataTable mydt = null;
            switch (sv.SourceType)
            {
                case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.StoredProc:
                    {
                        //v1.0.0 Fai 2016.05.11 - 1 = 2 instead of 1 = 1 - Begin
                        //v1.0.0 Fai 2015.04.02 - SQLParam from 2000 > Max
                        mydt = bllcommon.RunSP(me.ID, String.Format("[{0}].[qreport].[{1}]", me.DatabaseNAME, sv.TBLVIEWNAME), new SqlParameter[2]{
                           new SqlParameter(){ ParameterName = "@cols", SqlDbType = SqlDbType.NVarChar, Size = -1, Value = "TOP 1 *"},
                           new SqlParameter(){ ParameterName = "@cmd", SqlDbType = SqlDbType.NVarChar, Size = -1, Value = "AND 1 = 2"},
                        });
                        //v1.0.0 Fai 2016.05.11 - 1 = 2 instead of 1 = 1 - End
                    }
                    break;
                default:
                    {
                        string sql = "select top 1 * from (" + GetSqlForTblView(me.ID, me.DatabaseNAME, sv.TBLVIEWNAME) + ") AS TEMP";
                        mydt = bllcommon.query(me.ID, sql);
                    }
                    break;
            }

            foreach (System.Data.DataColumn dc in mydt.Columns)
            {
                if (dc.ColumnName == columnName)
                {
                    result = dc.DataType.Name;
                }
            }

            if (result == "DateTime")
            {
                result = "DateTime";
            }

            else if (result == "Decimal" || result == "Double")
            {
                result = "Decimal";
            }

            else if (result == "Int64" || result == "Single" || result == "TimeSpan" || result == "UInt16" || result == "UInt32" || result == "UInt64" || result == "Boolean")
            {
                result = "Int";
            }

            else if (result == "Int32" || result == "Int16")
            {
                RpEnum rp = new RpEnum();
                Type[] enums = rp.GetType().GetNestedTypes();

                foreach (Type t in enums)
                {
                    if (t.Name.ToUpper() == (svname + "_" + columnName).ToUpper())
                    {
                        result = "Enum";
                    }
                    else
                    {
                        result = "Int";
                    }
                }
            }

            else if (result == "String" || result == "Byte" || result == "Char")
            {
                result = "String";
            }
            else
            {
                result = "String";
            }
            return result;
        }

        public static void ParseParam(int UserID, ref ReportCriteria[] rpParam)
        {
            for (int i = 0; i < rpParam.Length; i++)
            {
                ReportCriteria rc = rpParam[i];
                ParseParam(UserID, ref rc);
            }
        }

        public static void ParseParam(int UserID, ref ReportCriteria rc)
        {
            if (rc.CRITERIA3 == "{now}") { rc.CRITERIA3 = DateTime.Today.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); }
            if (rc.CRITERIA4 == "{now}") { rc.CRITERIA4 = DateTime.Today.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); }
            switch (rc.DATATYPE.ToLower())
            {
                case "string":
                case "date":
                case "datetime":
                    {
                        #region String
                        rc.CRITERIA1 = String.Empty;
                        switch (rc.CRITERIA2)
                        {
                            case "r1":
                                {
                                    switch (rc.CRITERIA3.ToLower())
                                    {
                                        case "begins with":
                                            {
                                                if (!String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                                    rc.CRITERIA1 = String.Format(" and ([{0}] like '{1}%')", rc.COLUMNNAME, rc.CRITERIA4.Replace("%", String.Empty));
                                                else
                                                    rc.CRITERIA1 = String.Empty;
                                            }
                                            break;
                                        case "contains":
                                            {
                                                if (!String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                                    rc.CRITERIA1 = String.Format(" and ([{0}] like '%{1}%')", rc.COLUMNNAME, rc.CRITERIA4.Replace("%", String.Empty));
                                                else
                                                    rc.CRITERIA1 = String.Empty;
                                            }
                                            break;
                                        case "in":
                                            {
                                                if (!String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                                {
                                                    string[] arr = rc.CRITERIA4.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                    arr = Array.ConvertAll(arr, o => o.Trim());
                                                    if (arr.Any())
                                                    {
                                                        string c1 = "";
                                                        c1 = arr.Aggregate((o, oo) => (o.StartsWith("'") ? o : "'" + o + "'") + ",'" + oo + "'");
                                                        if (!c1.Contains('\'')) {
                                                            c1 = string.Format("'{0}'",c1);
                                                        }
                                                        rc.CRITERIA1 = String.Format(" and ([{0}] in ({1}))", rc.COLUMNNAME, c1);
                                                        rc.CRITERIA4 = String.Join(",", arr);
                                                    }
                                                    else
                                                    {
                                                        rc.CRITERIA1 = String.Empty;
                                                        rc.CRITERIA4 = String.Empty;
                                                    };
                                                }
                                                else
                                                    rc.CRITERIA1 = String.Empty;
                                            }
                                            break;
                                        case "not in":
                                            {
                                                if (!String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                                {
                                                    string[] arr = rc.CRITERIA4.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                    arr = Array.ConvertAll(arr, o => o.Trim());
                                                    if (arr.Any())
                                                    {
                                                        string c1 = "";
                                                        c1 = arr.Aggregate((o, oo) => (o.StartsWith("'") ? o : "'" + o + "'") + ",'" + oo + "'");
                                                        if (!c1.Contains('\'')) {
                                                            c1 = string.Format("'{0}'",c1);
                                                        }
                                                        rc.CRITERIA1 = String.Format(" and ([{0}] not in ({1}))", rc.COLUMNNAME, c1);
                                                        rc.CRITERIA4 = String.Join(",", arr);
                                                    }
                                                    else
                                                    {
                                                        rc.CRITERIA1 = String.Empty;
                                                        rc.CRITERIA4 = String.Empty;
                                                    };
                                                }
                                                else
                                                    rc.CRITERIA1 = String.Empty;
                                            }
                                            break;
                                        case "=":
                                            {
                                                if (!String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                                    rc.CRITERIA1 = String.Format(" and ([{0}] = '{1}')", rc.COLUMNNAME, rc.CRITERIA4);
                                                else
                                                    rc.CRITERIA1 = String.Empty;
                                            }
                                            break;
                                        case ">=":
                                            {
                                                if (!String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                                    rc.CRITERIA1 = String.Format(" and ([{0}] >= '{1}')", rc.COLUMNNAME, rc.CRITERIA4);
                                                else
                                                    rc.CRITERIA1 = String.Empty;
                                            }
                                            break;
                                        case "<=":
                                            {
                                                if (!String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                                    rc.CRITERIA1 = String.Format(" and ([{0}] <= '{1}')", rc.COLUMNNAME, rc.CRITERIA4);
                                                else
                                                    rc.CRITERIA1 = String.Empty;
                                            }
                                            break;
                                        case "does not contain":
                                            {
                                                if (!String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                                    rc.CRITERIA1 = String.Format(" and NOT([{0}] like '%{1}%')", rc.COLUMNNAME, rc.CRITERIA4.Replace("%", String.Empty));
                                                else
                                                    rc.CRITERIA1 = String.Empty;
                                            }
                                            break;
                                        case "does not equal":
                                            {
                                                rc.CRITERIA1 = String.Format(" and ([{0}] <> '{1}')", rc.COLUMNNAME, rc.CRITERIA4);
                                            }
                                            break;
                                    }
                                }
                                break;
                            case "r2":
                                {
                                    if (!String.IsNullOrWhiteSpace(rc.CRITERIA3) && !String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                    {
                                        //Fai 2017.03.21 - Bug Fix - Cater Time - Begin
                                        //rc.CRITERIA1 = String.Format(" and ([{0}] between '{1}' and '{2}')", rc.COLUMNNAME, rc.CRITERIA3, rc.CRITERIA4);
                                        DateTime l_dtFrom;
                                        DateTime l_dtTo;

                                        if (DateTime.TryParseExact(rc.CRITERIA3, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out l_dtFrom) &&
                                            DateTime.TryParseExact(rc.CRITERIA4, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out l_dtTo))
                                            rc.CRITERIA1 = String.Format(" and (DATEADD(dd, 0, DATEDIFF(dd, 0, [{0}])) between '{1}' and '{2}')", rc.COLUMNNAME, rc.CRITERIA3, rc.CRITERIA4);
                                        else
                                            rc.CRITERIA1 = String.Format(" and ([{0}] between '{1}' and '{2}')", rc.COLUMNNAME, rc.CRITERIA3, rc.CRITERIA4);
                                        //Fai 2017.03.21 - Bug Fix - Cater Time - End
                                    }
                                    else
                                        rc.CRITERIA1 = String.Empty;
                                }
                                break;
                            case "r3":
                                {
                                    rc.CRITERIA1 = String.Format(" and (ISNULL([{0}], '') = '')", rc.COLUMNNAME);
                                }
                                break;
                        }

                        #endregion
                    }
                    break;
                case "int":
                case "decimal":
                    {
                        #region Numeric

                        decimal dectemp;

                        switch (rc.CRITERIA2)
                        {
                            case "r1":
                                {
                                    if (!String.IsNullOrWhiteSpace(rc.CRITERIA4) && rc.CRITERIA3 != "In" && !Decimal.TryParse(rc.CRITERIA4, out dectemp))
                                    {
                                        throw new ArgumentException(String.Format(AppNum.Error_AppHelper_ParseParam_NotNumeric, rc.COLUMNNAME, rc.CRITERIA4));
                                    }
                                    switch (rc.CRITERIA3)
                                    {
                                        case "=":
                                            {
                                                if (!String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                                    rc.CRITERIA1 = String.Format(" and ([{0}] = {1})", rc.COLUMNNAME, rc.CRITERIA4);
                                                else
                                                    rc.CRITERIA1 = String.Empty;
                                            }
                                            break;
                                        case ">":
                                            {
                                                if (!String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                                    rc.CRITERIA1 = String.Format(" and ([{0}] > {1})", rc.COLUMNNAME, rc.CRITERIA4);
                                                else
                                                    rc.CRITERIA1 = String.Empty;
                                            }
                                            break;
                                        case "<":
                                            {
                                                if (!String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                                    rc.CRITERIA1 = String.Format(" and ([{0}] < {1})", rc.COLUMNNAME, rc.CRITERIA4);
                                                else
                                                    rc.CRITERIA1 = String.Empty;
                                            }
                                            break;
                                        case ">=":
                                            {
                                                if (!String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                                    rc.CRITERIA1 = String.Format(" and ([{0}] >= {1})", rc.COLUMNNAME, rc.CRITERIA4);
                                                else
                                                    rc.CRITERIA1 = String.Empty;
                                            }
                                            break;
                                        case "<=":
                                            {
                                                if (!String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                                    rc.CRITERIA1 = String.Format(" and ([{0}] <= {1})", rc.COLUMNNAME, rc.CRITERIA4);
                                                else
                                                    rc.CRITERIA1 = String.Empty;
                                            }
                                            break;
                                        case "<>":
                                            {
                                                if (!String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                                    rc.CRITERIA1 = String.Format(" and ([{0}] <> {1})", rc.COLUMNNAME, rc.CRITERIA4);
                                                else
                                                    rc.CRITERIA1 = String.Format(" and ([{0}] <> '{1}')", rc.COLUMNNAME, rc.CRITERIA4);
                                            }
                                            break;
                                        case "In":
                                            {
                                                if (!String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                                {
                                                    string[] arr = rc.CRITERIA4.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                    Array.ConvertAll(arr, o => o.Trim());
                                                    arr = arr.Where(o => Decimal.TryParse(o, out dectemp)).ToArray();
                                                    if (arr.Any())
                                                    {
                                                        string c1 = "";
                                                        c1 = arr.Aggregate((o, oo) => (o.StartsWith("'") ? o : "'" + o + "'") + ",'" + oo + "'");
                                                        rc.CRITERIA1 = String.Format(" and ([{0}] in ({1}))", rc.COLUMNNAME, c1);
                                                        rc.CRITERIA4 = String.Join(",", arr);
                                                    }
                                                    else
                                                    {
                                                        rc.CRITERIA1 = String.Empty;
                                                        rc.CRITERIA4 = String.Empty;
                                                    };
                                                }
                                                else
                                                    rc.CRITERIA1 = String.Empty;
                                            }
                                            break;
                                        case "not in":
                                            {
                                                if (!String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                                {
                                                    string[] arr = rc.CRITERIA4.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                    Array.ConvertAll(arr, o => o.Trim());
                                                    arr = arr.Where(o => Decimal.TryParse(o, out dectemp)).ToArray();
                                                    if (arr.Any())
                                                    {
                                                        string c1 = "";
                                                        c1 = arr.Aggregate((o, oo) => (o.StartsWith("'") ? o : "'" + o + "'") + ",'" + oo + "'");
                                                        rc.CRITERIA1 = String.Format(" and ([{0}] not in ({1}))", rc.COLUMNNAME, c1);
                                                        rc.CRITERIA4 = String.Join(",", arr);
                                                    }
                                                    else
                                                    {
                                                        rc.CRITERIA1 = String.Empty;
                                                        rc.CRITERIA4 = String.Empty;
                                                    };
                                                }
                                                else
                                                    rc.CRITERIA1 = String.Empty;
                                            }
                                            break;
                                    }
                                }
                                break;
                            case "r2":
                                {
                                    if ((!String.IsNullOrWhiteSpace(rc.CRITERIA3)) && (!String.IsNullOrWhiteSpace(rc.CRITERIA4)))
                                    {
                                        if (!Decimal.TryParse(rc.CRITERIA3, out dectemp))
                                        {
                                            throw new ArgumentException(String.Format(AppNum.Error_AppHelper_ParseParam_NotNumeric, rc.COLUMNNAME, rc.CRITERIA3));
                                        }
                                        if (!Decimal.TryParse(rc.CRITERIA4, out dectemp))
                                        {
                                            throw new ArgumentException(String.Format(AppNum.Error_AppHelper_ParseParam_NotNumeric, rc.COLUMNNAME, rc.CRITERIA4));
                                        }

                                        //Fai 2017.03.21 - Bug Fix - Cater Time - Begin
                                        //rc.CRITERIA1 = String.Format(" and ([{0}] between {1} and {2})", rc.COLUMNNAME, rc.CRITERIA3, rc.CRITERIA4);
                                        DateTime l_dtFrom;
                                        DateTime l_dtTo;

                                        if (DateTime.TryParseExact(rc.CRITERIA3, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out l_dtFrom) &&
                                            DateTime.TryParseExact(rc.CRITERIA4, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out l_dtTo))
                                            rc.CRITERIA1 = String.Format(" and (DATEADD(dd, 0, DATEDIFF(dd, 0, [{0}])) between '{1}' and '{2}')", rc.COLUMNNAME, rc.CRITERIA3, rc.CRITERIA4);
                                        else
                                            rc.CRITERIA1 = String.Format(" and ([{0}] between '{1}' and '{2}')", rc.COLUMNNAME, rc.CRITERIA3, rc.CRITERIA4);
                                        //Fai 2017.03.21 - Bug Fix - Cater Time - End 
                                    }
                                    else
                                    {
                                        rc.CRITERIA1 = String.Empty;
                                    }
                                }
                                break;
                            case "r3":
                                {
                                    rc.CRITERIA1 = String.Format(" and ([{0}] IS NULL)", rc.COLUMNNAME);
                                }
                                break;
                        }

                        #endregion
                    }
                    break;
                case "enum":
                    {
                        #region Enum

                        switch (rc.CRITERIA2)
                        {
                            case "r1":
                                {
                                    switch (rc.CRITERIA3)
                                    {
                                        case "=":   // currently this is the only operation supported
                                            {
                                                if (!String.IsNullOrWhiteSpace(rc.CRITERIA4))
                                                    rc.CRITERIA1 = String.Format(" and ([{0}] = '{1}')", rc.COLUMNNAME, rc.CRITERIA4);
                                                else
                                                    rc.CRITERIA1 = String.Empty;
                                            }
                                            break;
                                    }
                                }
                                break;
                        }

                        #endregion
                    }
                    break;
            }

        }

        public static CUSTOMRP.Model.ERRORLOG GetLastError() {
            return DAL.ERRORLOG.GetLast();
        }

        public static void LogException(Exception ex, int UserID, int ReportID, string ReportName)
        {
            DAL.ERRORLOG.Add(ex, UserID, ReportID, ReportName);
        }
    }
}