using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Common;

namespace CUSTOMRP.DAL
{
    public abstract class DbHelperSQL
    {
        //public static string connectionString = ConfigurationManager.ConnectionStrings["pmsstagingConnectionString"].ConnectionString;
        public static string connectionString = Utility.GetDBConnectionString();

        /// <summary>
        /// Default command timeout in seconds for all helper methods without "Time"
        /// </summary>
        public static int CommandTimeout = 300;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public static int ExecuteSql(int UserID, string strCmd, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(strCmd, connection))
                    {
                        connection.Open();

                        cmd.CommandTimeout = CommandTimeout;
                        PreParms(cmdParms, cmd);

                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();     // much like other List<T> implementations. Calling .Clear() before dispose can reduce memory usage
                        ERRORLOG.LoggedDBError = false; // SQL statement successfully executed, reset SQL error tracking
                        return rows;
                    }
                }
                catch (Exception ex)
                {
                    ERRORLOG.Add(ex, UserID);
                    throw;
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
        }

        private static void PreParms(SqlParameter[] cmdParms, SqlCommand cmd)
        {
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public static object GetSingle(int UserID, string strCmd, params SqlParameter[] cmdParms)
        {
            object result = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(strCmd, connection))
                    {

                        connection.Open();

                        cmd.CommandTimeout = CommandTimeout;

                        PreParms(cmdParms, cmd);

                        result = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();     // much like other List<T> implementations. Calling .Clear() before dispose can reduce memory usage
                        ERRORLOG.LoggedDBError = false; // SQL statement successfully executed, reset SQL error tracking
                    }
                }
                catch (Exception ex)
                {
                    ERRORLOG.Add(ex, UserID);
                    throw;
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public static DataSet Query(int UserID, string strCmd, params SqlParameter[] cmdParms)
        {
            DataSet result = new DataSet();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(strCmd, connection))
                    {
                        connection.Open();

                        cmd.CommandTimeout = CommandTimeout;

                        PreParms(cmdParms, cmd);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(result, "ds");
                        cmd.Parameters.Clear();     // much like other List<T> implementations. Calling .Clear() before dispose can reduce memory usage
                        ERRORLOG.LoggedDBError = false; // SQL statement successfully executed, reset SQL error tracking
                    }
                }
                catch (Exception ex)
                {
                    ERRORLOG.Add(ex, UserID);
                    throw;
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Execute query to get dataset, with transaction rollback in the end to undo the changes. (Note it'll still alter identity seed, and possibly other triggered effects)
        /// </summary>
        /// <param name="strCmd"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public static DataSet SafeQuery(int UserID, string strCmd)
        {
            if (!CheckQuery(strCmd))
            {
                throw new SystemException("Illegal query token detected. Execution aborted.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                SqlTransaction ts = null;
                try
                {
                    connection.Open();
                    ts = connection.BeginTransaction();
                    SqlDataAdapter command = new SqlDataAdapter(strCmd, connection);
                    command.SelectCommand.Transaction = ts;
                    command.SelectCommand.CommandTimeout = CommandTimeout;
                    command.Fill(ds, "ds");
                    ERRORLOG.LoggedDBError = false; // SQL statement successfully executed, reset SQL error tracking
                }
                catch (Exception ex)
                {
                    ERRORLOG.Add(ex, UserID);
                    throw;
                }
                finally
                {
                    if (ts != null)
                    {
                        ts.Rollback();
                    }
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
                return ds;
            }
        }

        private static bool CheckQuery(string strSQL)
        {
            if (String.IsNullOrWhiteSpace(strSQL)) { return false; }

            foreach (string statement in strSQL.Split(';'))
            {
                if (statement.Trim().ToUpper().StartsWith("COMMIT")) { return false; }
            }

            return true;
        }

        /// <summary>
        /// Run stored procedure with transaction rollback in the end.
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public static DataTable RunProcedure(int UserID, string storedProcName, params SqlParameter[] cmdParms)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlDataReader reader = null;
            DataTable result = new DataTable();
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(storedProcName, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandTimeout = CommandTimeout;

                PreParms(cmdParms, cmd);

                reader = cmd.ExecuteReader();
                result.Load(reader);

                ERRORLOG.LoggedDBError = false; // SQL statement successfully executed, reset SQL error tracking
            }
            catch (Exception ex)
            {
                ERRORLOG.Add(ex, UserID);
                throw;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// Run stored procedure with transaction rollback in the end.
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public static DataTable SafeRunProcedure(int UserID, string storedProcName, params SqlParameter[] cmdParms)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlTransaction ts = null;
            SqlDataReader reader = null;
            DataTable result = new DataTable();
            try
            {
                connection.Open();
                ts = connection.BeginTransaction();
                SqlCommand cmd = new SqlCommand(storedProcName, connection, ts);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandTimeout = CommandTimeout;

                PreParms(cmdParms, cmd);

                reader = cmd.ExecuteReader();
                result.Load(reader);

                ERRORLOG.LoggedDBError = false; // SQL statement successfully executed, reset SQL error tracking
            }
            catch (Exception ex)
            {
                ERRORLOG.Add(ex, UserID);
                throw;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (ts != null)
                {
                    ts.Rollback();
                }
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;
        }

        public static int GetMaxID(int UserID, string FieldName, string TableName, string DatabaseName = null)
        {
            string strsql = String.Format("SELECT MAX([{0}]) + 1 FROM {2}[{1}]", FieldName, TableName, DatabaseName != null ? String.Format("[{0}].[qreport].", DatabaseName) : String.Empty);
            object obj = GetSingle(UserID, strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return Int32.Parse(obj.ToString());
            }
        }
    }
}