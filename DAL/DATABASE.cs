using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace CUSTOMRP.DAL
{
    /// <summary>
    /// 数据访问类:DATABASE
    /// </summary>
    public partial class DATABASE
    {
        public DATABASE()
        {}
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(CUSTOMRP.Model.DATABASE model)
        {
            if (String.IsNullOrEmpty(model.HASHKEY)) { model.HASHKEY = "com"; }

            StringBuilder strSql=new StringBuilder();
            strSql.Append("insert into [DATABASE](");
            strSql.Append("APPLICATIONID,NAME,[DESC],STATUS,LASTMODIFYDATE,LASTMODIFYUSER,AUDOTIME,HASHKEY)");
            strSql.Append(" values (");
            strSql.Append("@APPLICATIONID,@NAME,@DESC,@STATUS,@LASTMODIFYDATE,@LASTMODIFYUSER,@AUDOTIME,@HASHKEY)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                    new SqlParameter("@APPLICATIONID", SqlDbType.Int,4),
                    new SqlParameter("@NAME", SqlDbType.NVarChar,50),
                    new SqlParameter("@DESC", SqlDbType.NVarChar,2000),
                    new SqlParameter("@STATUS", SqlDbType.Int,4),
                    new SqlParameter("@LASTMODIFYDATE", SqlDbType.DateTime),
                    new SqlParameter("@LASTMODIFYUSER", SqlDbType.Int,4),
                    new SqlParameter("@AUDOTIME", SqlDbType.DateTime),
                    new SqlParameter("@HASHKEY", SqlDbType.NVarChar,50)};
            parameters[0].Value = model.APPLICATIONID;
            parameters[1].Value = model.NAME;
            parameters[2].Value = model.DESC;
            parameters[3].Value = model.STATUS;
            parameters[4].Value = model.LASTMODIFYDATE;
            parameters[5].Value = model.LASTMODIFYUSER;
            parameters[6].Value = model.AUDOTIME;
            parameters[7].Value = model.HASHKEY;

            object obj = DbHelperSQL.GetSingle(model.LASTMODIFYUSER, strSql.ToString(),parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                model.ID = Convert.ToInt32(obj);

                Model.AUDITLOG auditobj = model.GetAuditLogObject(null);
                auditobj.UserID = model.LASTMODIFYUSER;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.DATABASE.Add";
                auditobj.Message = String.Format(AppNum.AuditMessage.DatabaseInsertSuccess, model.ID);

                AUDITLOG.Add(auditobj);

                return model.ID;
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(CUSTOMRP.Model.DATABASE model)
        {
            if (String.IsNullOrEmpty(model.HASHKEY)) { model.HASHKEY = "com"; }

            StringBuilder strSql=new StringBuilder();
            strSql.Append("update [DATABASE] set ");
            strSql.Append("APPLICATIONID=@APPLICATIONID,");
            strSql.Append("NAME=@NAME,");
            strSql.Append("[DESC]=@DESC,");
            strSql.Append("STATUS=@STATUS,");
            strSql.Append("LASTMODIFYDATE=@LASTMODIFYDATE,");
            strSql.Append("LASTMODIFYUSER=@LASTMODIFYUSER,");
            strSql.Append("AUDOTIME=@AUDOTIME,");
            strSql.Append("HASHKEY=@HASHKEY");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@APPLICATIONID", SqlDbType.Int,4),
                    new SqlParameter("@NAME", SqlDbType.NVarChar,50),
                    new SqlParameter("@DESC", SqlDbType.NVarChar,2000),
                    new SqlParameter("@STATUS", SqlDbType.Int,4),
                    new SqlParameter("@LASTMODIFYDATE", SqlDbType.DateTime),
                    new SqlParameter("@LASTMODIFYUSER", SqlDbType.Int,4),
                    new SqlParameter("@AUDOTIME", SqlDbType.DateTime),
                    new SqlParameter("@HASHKEY", SqlDbType.NVarChar,50),
                    new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = model.APPLICATIONID;
            parameters[1].Value = model.NAME;
            parameters[2].Value = model.DESC;
            parameters[3].Value = model.STATUS;
            parameters[4].Value = model.LASTMODIFYDATE;
            parameters[5].Value = model.LASTMODIFYUSER;
            parameters[6].Value = model.AUDOTIME;
            parameters[7].Value = model.HASHKEY;
            parameters[8].Value = model.ID;

            int rows=DbHelperSQL.ExecuteSql(model.LASTMODIFYUSER, strSql.ToString(),parameters);
            if (rows > 0)
            {
                Model.AUDITLOG auditobj = model.GetAuditLogObject(null);
                auditobj.UserID = model.LASTMODIFYUSER;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.DATABASE.Update";
                auditobj.Message = String.Format(AppNum.AuditMessage.DatabaseUpdateSuccess, model.ID);

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
        public bool Delete(int UserID, int ID)
        {
            
            StringBuilder strSql=new StringBuilder();
            strSql.Append("delete from [DATABASE] ");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)
            };
            parameters[0].Value = ID;

            int rows=DbHelperSQL.ExecuteSql(UserID, strSql.ToString(),parameters);
            if (rows > 0)
            {
                Model.AUDITLOG auditobj = new Model.AUDITLOG();
                auditobj.UserID = UserID;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.DATABASE.Delete";
                auditobj.Message = String.Format(AppNum.AuditMessage.DatabaseDeleteSuccess, ID);

                AUDITLOG.Add(auditobj);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteList(int UserID, string IDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [DATABASE] ");
            strSql.Append(" where ID in (" + IDlist + ")  ");
            int rows = DbHelperSQL.ExecuteSql(UserID, strSql.ToString());
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public CUSTOMRP.Model.DATABASE GetModel(int UserID, int ID)
        {
            
            StringBuilder strSql=new StringBuilder();
            strSql.Append("select  top 1 ID,APPLICATIONID,NAME,[DESC],STATUS,LASTMODIFYDATE,LASTMODIFYUSER,AUDOTIME,CONNECTIONSTRING,HASHKEY from [DATABASE] ");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)
            };
            parameters[0].Value = ID;

            CUSTOMRP.Model.DATABASE model=new CUSTOMRP.Model.DATABASE();
            DataSet ds=DbHelperSQL.Query(UserID, strSql.ToString(),parameters);
            if(ds.Tables[0].Rows.Count>0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public CUSTOMRP.Model.DATABASE DataRowToModel(DataRow row)
        {
            CUSTOMRP.Model.DATABASE model=new CUSTOMRP.Model.DATABASE();
            if (row != null)
            {
                if(row["ID"]!=null && row["ID"].ToString()!="")
                {
                    model.ID=Int32.Parse(row["ID"].ToString());
                }
                if(row["APPLICATIONID"]!=null && row["APPLICATIONID"].ToString()!="")
                {
                    model.APPLICATIONID=Int32.Parse(row["APPLICATIONID"].ToString());
                }
                if(row["NAME"]!=null)
                {
                    model.NAME=row["NAME"].ToString();
                }
                if(row["DESC"]!=null)
                {
                    model.DESC=row["DESC"].ToString();
                }
                if(row["STATUS"]!=null && row["STATUS"].ToString()!="")
                {
                    model.STATUS=Int32.Parse(row["STATUS"].ToString());
                }
                if(row["LASTMODIFYDATE"]!=null && row["LASTMODIFYDATE"].ToString()!="")
                {
                    model.LASTMODIFYDATE=DateTime.Parse(row["LASTMODIFYDATE"].ToString());
                }
                if(row["LASTMODIFYUSER"]!=null && row["LASTMODIFYUSER"].ToString()!="")
                {
                    model.LASTMODIFYUSER=Int32.Parse(row["LASTMODIFYUSER"].ToString());
                }
                if(row["AUDOTIME"]!=null && row["AUDOTIME"].ToString()!="")
                {
                    model.AUDOTIME=DateTime.Parse(row["AUDOTIME"].ToString());
                }
                if (row["HASHKEY"] != null)
                {
                    model.HASHKEY = row["HASHKEY"].ToString();
                }
            }
            return model;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(int UserID, string strWhere)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("select ID,APPLICATIONID,NAME,[DESC],STATUS,LASTMODIFYDATE,LASTMODIFYUSER,AUDOTIME,CONNECTIONSTRING,HASHKEY ");
            strSql.Append(" FROM [DATABASE] ");
            if(strWhere.Trim()!="")
            {
                strSql.Append(" where "+strWhere);
            }
            return DbHelperSQL.Query(UserID, strSql.ToString());
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int UserID, int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("select ");
            if(Top>0)
            {
                strSql.Append(" top "+Top.ToString());
            }
            strSql.Append(" ID,APPLICATIONID,NAME,[DESC],STATUS,LASTMODIFYDATE,LASTMODIFYUSER,AUDOTIME,CONNECTIONSTRING,HASHKEY ");
            strSql.Append(" FROM [DATABASE] ");
            if(strWhere.Trim()!="")
            {
                strSql.Append(" where "+strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return DbHelperSQL.Query(UserID, strSql.ToString());
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(int UserID, string strWhere)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("select count(1) FROM [DATABASE] ");
            if(strWhere.Trim()!="")
            {
                strSql.Append(" where "+strWhere);
            }
            object obj = DbHelperSQL.GetSingle(UserID, strSql.ToString());
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetListByPage(int UserID, string strWhere, string orderby, int startIndex, int endIndex)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby );
            }
            else
            {
                strSql.Append("order by T.ID [DESC]");
            }
            strSql.Append(")AS Row, T.*  from [DATABASE] T ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            return DbHelperSQL.Query(UserID, strSql.ToString());
        }

        #endregion  BasicMethod
        #region  ExtensionMethod

        public DataSet GetDBListWithApp(int UserID, string strWhere = null)
        {
            StringBuilder strSql = new StringBuilder("SELECT");
            strSql.AppendLine("	db.ID, db.APPLICATIONID, db.NAME, db.[DESC],");
            strSql.AppendLine("	app.NAME AS APPNAME, app.[DESC] AS APPDESC, db.CONNECTIONSTRING,");
            strSql.AppendLine("	db.[HASHKEY]");
            strSql.AppendLine("FROM dbo.[APPLICATION] app");
            strSql.AppendLine("INNER JOIN dbo.[DATABASE] db ON app.ID = db.APPLICATIONID");
            strSql.AppendLine("WHERE db.[STATUS] = 1");

            if (strWhere != null)
            {
                strSql.AppendLine(strWhere);
            }

            strSql.AppendLine("ORDER BY db.APPLICATIONID, app.NAME");
            return DbHelperSQL.Query(UserID, strSql.ToString());
        }

        #endregion  ExtensionMethod
    }
}