using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace CUSTOMRP.DAL
{
    public partial class Common
    {
        public static int CommandTimeout
        {
            get { return DbHelperSQL.CommandTimeout; }
            set { DbHelperSQL.CommandTimeout = value; }
        }

        public Common()
        { }
        #region  BasicMethod
        public void executeSql(int UserID, string sql)
        {
            DbHelperSQL.ExecuteSql(UserID, sql);
        }

        public DataTable query(int UserID, string sql)
        {
            DataTable result = null;
            result = DbHelperSQL.Query(UserID, sql).Tables[0];
            return result;
        }

        public DataTable SafeQuery(int UserID, string sql)
        {
            DataTable result = null;
            result = DbHelperSQL.SafeQuery(UserID, sql).Tables[0];
            return result;
        }

        public object GetSingle(int UserID, string sql)
        {
            return DbHelperSQL.GetSingle(UserID, sql);
        }

        public DataTable RunSP(int UserID, string SPName, params SqlParameter[] spParams)
        {
            DataTable result = DbHelperSQL.RunProcedure(UserID, SPName, spParams);
            return result;
        }

        public DataTable SafeRunSP(int UserID, string SPName, params SqlParameter[] spParams)
        {
            DataTable result = DbHelperSQL.SafeRunProcedure(UserID, SPName, spParams);
            return result;
        }
        #endregion
    }
}