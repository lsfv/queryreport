using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace CUSTOMRP.DAL
{
    /// <summary>
    /// 数据访问类:QUERYPARAMS
    /// </summary>
    public partial class QUERYPARAMS
    {
        public QUERYPARAMS()
        { }
        #region  BasicMethod

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int UserID, int REPORT, string NAME)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from QUERYPARAMS");
            strSql.Append(" where REPORT=@REPORT and NAME=@NAME ");
            SqlParameter[] parameters = {
                    new SqlParameter("@REPORT", SqlDbType.Int),
                    new SqlParameter("@NAME", SqlDbType.NVarChar,50)      };
            parameters[0].Value = REPORT;
            parameters[1].Value = NAME;

            return Convert.ToInt32(DbHelperSQL.GetSingle(UserID, strSql.ToString(), parameters)) > 0;
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(int UserID, CUSTOMRP.Model.QUERYPARAMS model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into QUERYPARAMS(");
            strSql.Append("REPORT,NAME,VALUE)");
            strSql.Append(" values (");
            strSql.Append("@REPORT,@NAME,@VALUE)");
            SqlParameter[] parameters = {
                    new SqlParameter("@REPORT", SqlDbType.Int),
                    new SqlParameter("@NAME", SqlDbType.NVarChar,50),
                    new SqlParameter("@VALUE", SqlDbType.NVarChar,100)};
            parameters[0].Value = model.REPORT;
            parameters[1].Value = model.NAME;
            parameters[2].Value = model.VALUE;

            int rows = DbHelperSQL.ExecuteSql(UserID, strSql.ToString(), parameters);
            if (rows > 0)
            {
                Model.AUDITLOG auditobj = model.GetAuditLogObject(null);
                auditobj.UserID = UserID;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.QUERYPARAMS.Add";
                auditobj.Message = String.Format(AppNum.AuditMessage.QueryParamsInsertSuccess, model.REPORT, model.NAME);

                AUDITLOG.Add(auditobj);

                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(int UserID, CUSTOMRP.Model.QUERYPARAMS model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update QUERYPARAMS set ");
            strSql.Append("REPORT=@REPORT,");
            strSql.Append("NAME=@NAME,");
            strSql.Append("VALUE=@VALUE");
            strSql.Append(" where REPORT=@REPORT and NAME=@NAME");
            SqlParameter[] parameters = {
                    new SqlParameter("@REPORT", SqlDbType.Int),
                    new SqlParameter("@NAME", SqlDbType.NVarChar,50),
                    new SqlParameter("@VALUE", SqlDbType.NVarChar,100)};
            parameters[0].Value = model.REPORT;
            parameters[1].Value = model.NAME;
            parameters[2].Value = model.VALUE;

            int rows = DbHelperSQL.ExecuteSql(UserID, strSql.ToString(), parameters);
            if (rows > 0)
            {
                Model.AUDITLOG auditobj = model.GetAuditLogObject(null);
                auditobj.UserID = UserID;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.QUERYPARAMS.Update";
                auditobj.Message = String.Format(AppNum.AuditMessage.QueryParamsUpdateSuccess, model.REPORT, model.NAME);

                AUDITLOG.Add(auditobj);

                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int UserID, int REPORT, string NAME)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from QUERYPARAMS ");
            strSql.Append(" where REPORT=@REPORT and NAME=@NAME ");
            SqlParameter[] parameters = {
                    new SqlParameter("@REPORT", SqlDbType.Int),
                    new SqlParameter("@NAME", SqlDbType.NVarChar,50)};
            parameters[0].Value = REPORT;
            parameters[1].Value = NAME;

            int rows = DbHelperSQL.ExecuteSql(UserID, strSql.ToString(), parameters);
            if (rows > 0)
            {
                Model.AUDITLOG auditobj = new Model.AUDITLOG();
                auditobj.UserID = UserID;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.QUERYPARAMS.Delete";
                auditobj.Message = String.Format(AppNum.AuditMessage.QueryParamsDeleteSuccess, REPORT, NAME);

                AUDITLOG.Add(auditobj);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(int UserID, int REPORT)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT qp.NAME, qp.VALUE")
                .Append(" from QUERYPARAMS qp")
                .AppendFormat(" WHERE qp.REPORT = {0}", REPORT);
            return DbHelperSQL.Query(UserID, strSql.ToString());
        }

        #endregion  BasicMethod
    }
}

