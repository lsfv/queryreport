using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace CUSTOMRP.BLL
{
    public class COMMON
    {
        private readonly CUSTOMRP.DAL.Common DAL = new DAL.Common();

        public static int CommandTimeout
        {
            get { return CUSTOMRP.DAL.Common.CommandTimeout; }
            set { CUSTOMRP.DAL.Common.CommandTimeout = value; }
        }

        public void executesql(int UserID, string sql)
        {
            DAL.executeSql(UserID, sql);
        }

        public DataTable query(int UserID, string sql)
        {
            return DAL.query(UserID, sql);
        }

        public DataTable SafeQuery(int UserID, string sql)
        {
            return DAL.SafeQuery(UserID, sql);
        }

        public object GetSingle(int UserID, string sql)
        {
            return DAL.GetSingle(UserID, sql);
        }

        public DataTable RunSP(int UserID, string spName, params SqlParameter[] spParams)
        {
            return DAL.RunSP(UserID, spName, spParams);
        }

        public DataTable SafeRunSP(int UserID, string spName, params SqlParameter[] spParams)
        {
            return DAL.SafeRunSP(UserID, spName, spParams);
        }
    }
}
