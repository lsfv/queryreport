using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Common;

namespace CUSTOMRP.DAL
{
    /// <summary>
    /// Class for error logging
    /// </summary>
    public class ERRORLOG
    {
        // Note: Do not use DbHelperSQL in this class because that can fail
        //public static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["pmsstagingConnectionString"].ConnectionString;
        public static string connectionString = Utility.GetDBConnectionString();

        public static bool LoggedDBError { get; set; }

        public ERRORLOG() { LoggedDBError = false; }

        public static void Add(Exception ex, int UserID, int? ReportID = null, string ReportName = null)
        {
            const string strSQL = "INSERT INTO ERRORLOG (UserID, CreateDate, Message, StackTrace, ReportID, ReportName, ColumnName) VALUES (@UserID, @CreateDate, @Message, @StackTrace, @ReportID, @ReportName, @ColumnName)";

            CUSTOMRP.Model.ERRORLOG errlog = new Model.ERRORLOG()
            {
                UserID = UserID,
                CreateDate = DateTime.Now,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                ReportID = ReportID,
                ReportName = ReportName,
            };

            MatchCollection regex = Regex.Matches(ex.Message, @"(?<=column (name)? ')[a-zA-Z_ ]*(?=')");
            string ColumnName = string.Join(";", regex.Cast<Match>().Select(m => m.Value));
            if (!string.IsNullOrEmpty(ColumnName)) {
                errlog.ColumnName = ColumnName;
            }
            else
            {
                errlog.ColumnName = "ColumnIsNull";
            }

            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope(System.Transactions.TransactionScopeOption.Suppress))   // do not use transaction here
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand comm = new SqlCommand(strSQL, conn))
                {
                    conn.Open();
                    comm.Parameters.AddWithValue("@UserID", errlog.UserID);
                    comm.Parameters.AddWithValue("@CreateDate", errlog.CreateDate);
                    comm.Parameters.AddWithValue("@Message", errlog.Message);
                    comm.Parameters.AddWithValue("@StackTrace", errlog.StackTrace);
                    comm.Parameters.AddWithValue("@ReportID", errlog.ReportID);
                    comm.Parameters.AddWithValue("@ReportName", errlog.ReportName);
                    comm.Parameters.AddWithValue("@ColumnName", errlog.ColumnName);
                    comm.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                // No database connection, try log to Event Viewer Application Log instead
                if (!LoggedDBError)
                {
                    AddEvent(new Model.ERRORLOG()
                    {
                        UserID = UserID,
                        CreateDate = DateTime.Now,
                        Message = e.Message,
                        StackTrace = e.StackTrace,
                        ReportID = ReportID,
                        ReportName = ReportName,
                        ColumnName = string.Empty
                    });
                    LoggedDBError = true;
                }
                AddEvent(errlog);
            }
        }

        private static void AddEvent(CUSTOMRP.Model.ERRORLOG errlog)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("UserID = {0}\r\n", errlog.UserID);
            sb.AppendFormat("Time = {0:yyyy/MM/dd HH:mm:ss}\r\n", errlog.CreateDate);
            sb.AppendFormat("Error Message = {0}\r\n", errlog.Message);
            // Do not log stacktrace in event log

            using (System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog())
            {
                appLog.Source = "Application";
                appLog.WriteEntry(sb.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }
        }

        public static CUSTOMRP.Model.ERRORLOG GetLast()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT TOP(1) * FROM ERRORLOG ORDER BY CreateDate DESC";
                    var reader = cmd.ExecuteReader();
                    CUSTOMRP.Model.ERRORLOG errlog = new Model.ERRORLOG()
                    {
                        UserID = reader.GetInt32(0),
                        CreateDate = reader.GetDateTime(0),
                        Message = reader.GetString(0),
                        StackTrace = reader.GetString(0),
                        ReportID = reader.GetInt32(0),
                        ReportName = reader.GetString(0),
                        ColumnName = reader.GetString(0)
                    };
                    return errlog;
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
