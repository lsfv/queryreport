using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace CUSTOMRP.DAL
{
    /// <summary>
    /// 数据访问类:APPLICATION
    /// </summary>
    public partial class APPLICATION
    {
        public APPLICATION()
        {}
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(CUSTOMRP.Model.APPLICATION model)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("insert into APPLICATION(");
            strSql.Append("NAME,[DESC],CONFIG_VIEW1,CONFIG_VIEW2,CONFIG_VIEW3,LASTMODIFYDATE,LASTMODIFYUSER,AUDODATE)");
            strSql.Append(" values (");
            strSql.Append("@NAME,@DESC,@CONFIG_VIEW1,@CONFIG_VIEW2,@CONFIG_VIEW3,@LASTMODIFYDATE,@LASTMODIFYUSER,@AUDODATE)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                    new SqlParameter("@NAME", SqlDbType.NVarChar,50),
                    new SqlParameter("@DESC", SqlDbType.NVarChar,-1),
                    new SqlParameter("@CONFIG_VIEW1", SqlDbType.NVarChar,-1),
                    new SqlParameter("@CONFIG_VIEW2", SqlDbType.NVarChar,-1),
                    new SqlParameter("@CONFIG_VIEW3", SqlDbType.NVarChar,-1),
                    new SqlParameter("@LASTMODIFYDATE", SqlDbType.DateTime),
                    new SqlParameter("@LASTMODIFYUSER", SqlDbType.Int,4),
                    new SqlParameter("@AUDODATE", SqlDbType.DateTime)};
            parameters[0].Value = model.NAME;
            parameters[1].Value = model.DESC;
            parameters[2].Value = model.CONFIG_VIEW1;
            parameters[3].Value = model.CONFIG_VIEW2;
            parameters[4].Value = model.CONFIG_VIEW3;
            parameters[5].Value = model.LASTMODIFYDATE;
            parameters[6].Value = model.LASTMODIFYUSER;
            parameters[7].Value = model.AUDODATE;

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
                auditobj.ModuleName = "DAL.APPLICATION.Add";
                auditobj.Message = String.Format(AppNum.AuditMessage.ApplicationInsertSuccess, model.ID);

                AUDITLOG.Add(auditobj);

                return model.ID;
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(CUSTOMRP.Model.APPLICATION model)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("update APPLICATION set ");
            strSql.Append("NAME=@NAME,");
            strSql.Append("DESC=@DESC,");
            strSql.Append("CONFIG_VIEW1=@CONFIG_VIEW1,");
            strSql.Append("CONFIG_VIEW2=@CONFIG_VIEW2,");
            strSql.Append("CONFIG_VIEW3=@CONFIG_VIEW3,");
            strSql.Append("LASTMODIFYDATE=@LASTMODIFYDATE,");
            strSql.Append("LASTMODIFYUSER=@LASTMODIFYUSER,");
            strSql.Append("AUDODATE=@AUDODATE");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@NAME", SqlDbType.NVarChar,50),
                    new SqlParameter("@DESC", SqlDbType.NVarChar,-1),
                    new SqlParameter("@CONFIG_VIEW1", SqlDbType.NVarChar,-1),
                    new SqlParameter("@CONFIG_VIEW2", SqlDbType.NVarChar,-1),
                    new SqlParameter("@CONFIG_VIEW3", SqlDbType.NVarChar,-1),
                    new SqlParameter("@LASTMODIFYDATE", SqlDbType.DateTime),
                    new SqlParameter("@LASTMODIFYUSER", SqlDbType.Int,4),
                    new SqlParameter("@AUDODATE", SqlDbType.DateTime),
                    new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = model.NAME;
            parameters[1].Value = model.DESC;
            parameters[2].Value = model.CONFIG_VIEW1;
            parameters[3].Value = model.CONFIG_VIEW2;
            parameters[4].Value = model.CONFIG_VIEW3;
            parameters[5].Value = model.LASTMODIFYDATE;
            parameters[6].Value = model.LASTMODIFYUSER;
            parameters[7].Value = model.AUDODATE;
            parameters[8].Value = model.ID;

            int rows=DbHelperSQL.ExecuteSql(model.LASTMODIFYUSER, strSql.ToString(),parameters);
            if (rows > 0)
            {
                Model.AUDITLOG auditobj = model.GetAuditLogObject(null);
                auditobj.UserID = model.LASTMODIFYUSER;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.APPLICATION.Update";
                auditobj.Message = String.Format(AppNum.AuditMessage.ApplicationUpdateSuccess, model.ID);

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
            strSql.Append("delete from APPLICATION ");
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
                auditobj.ModuleName = "DAL.APPLICATION.Delete";
                auditobj.Message = String.Format(AppNum.AuditMessage.ApplicationDeleteSuccess, ID);

                AUDITLOG.Add(auditobj);

                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(int UserID, string IDlist)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("delete from APPLICATION ");
            strSql.Append(" where ID in ("+IDlist + ")  ");
            int rows=DbHelperSQL.ExecuteSql(UserID, strSql.ToString());
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
        public CUSTOMRP.Model.APPLICATION GetModel(int UserID, int ID)
        {
            
            StringBuilder strSql=new StringBuilder();
            strSql.Append("select  top 1 ID,NAME,[DESC],CONFIG_VIEW1,CONFIG_VIEW2,CONFIG_VIEW3,LASTMODIFYDATE,LASTMODIFYUSER,AUDODATE from APPLICATION ");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)
            };
            parameters[0].Value = ID;

            CUSTOMRP.Model.APPLICATION model=new CUSTOMRP.Model.APPLICATION();
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
        public CUSTOMRP.Model.APPLICATION DataRowToModel(DataRow row)
        {
            CUSTOMRP.Model.APPLICATION model=new CUSTOMRP.Model.APPLICATION();
            if (row != null)
            {
                if(row["ID"]!=null && row["ID"].ToString()!="")
                {
                    model.ID=Int32.Parse(row["ID"].ToString());
                }
                if(row["NAME"]!=null)
                {
                    model.NAME=row["NAME"].ToString();
                }
                if(row["DESC"]!=null)
                {
                    model.DESC=row["DESC"].ToString();
                }
                if(row["CONFIG_VIEW1"]!=null)
                {
                    model.CONFIG_VIEW1=row["CONFIG_VIEW1"].ToString();
                }
                if(row["CONFIG_VIEW2"]!=null)
                {
                    model.CONFIG_VIEW2=row["CONFIG_VIEW2"].ToString();
                }
                if(row["CONFIG_VIEW3"]!=null)
                {
                    model.CONFIG_VIEW3=row["CONFIG_VIEW3"].ToString();
                }
                if(row["LASTMODIFYDATE"]!=null && row["LASTMODIFYDATE"].ToString()!="")
                {
                    model.LASTMODIFYDATE=DateTime.Parse(row["LASTMODIFYDATE"].ToString());
                }
                if(row["LASTMODIFYUSER"]!=null && row["LASTMODIFYUSER"].ToString()!="")
                {
                    model.LASTMODIFYUSER=Int32.Parse(row["LASTMODIFYUSER"].ToString());
                }
                if(row["AUDODATE"]!=null && row["AUDODATE"].ToString()!="")
                {
                    model.AUDODATE=DateTime.Parse(row["AUDODATE"].ToString());
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
            strSql.Append("select ID,NAME,[DESC],CONFIG_VIEW1,CONFIG_VIEW2,CONFIG_VIEW3,LASTMODIFYDATE,LASTMODIFYUSER,AUDODATE ");
            strSql.Append(" FROM APPLICATION ");
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
            strSql.Append(" ID,NAME,[DESC],CONFIG_VIEW1,CONFIG_VIEW2,CONFIG_VIEW3,LASTMODIFYDATE,LASTMODIFYUSER,AUDODATE ");
            strSql.Append(" FROM APPLICATION ");
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
            strSql.Append("select count(1) FROM APPLICATION ");
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
                strSql.Append("order by T.ID desc");
            }
            strSql.Append(")AS Row, T.*  from APPLICATION T ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            return DbHelperSQL.Query(UserID, strSql.ToString());
        }

        /*
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetList(int PageSize,int PageIndex,string strWhere)
        {
            SqlParameter[] parameters = {
                    new SqlParameter("@tblName", SqlDbType.VarChar, 255),
                    new SqlParameter("@fldName", SqlDbType.VarChar, 255),
                    new SqlParameter("@PageSize", SqlDbType.Int),
                    new SqlParameter("@PageIndex", SqlDbType.Int),
                    new SqlParameter("@IsReCount", SqlDbType.Bit),
                    new SqlParameter("@OrderType", SqlDbType.Bit),
                    new SqlParameter("@strWhere", SqlDbType.VarChar,1000),
                    };
            parameters[0].Value = "APPLICATION";
            parameters[1].Value = "ID";
            parameters[2].Value = PageSize;
            parameters[3].Value = PageIndex;
            parameters[4].Value = 0;
            parameters[5].Value = 0;
            parameters[6].Value = strWhere;	
            return DbHelperSQL.RunProcedure("UP_GetRecordByPage",parameters,"ds");
        }*/

        #endregion  BasicMethod
        #region  ExtensionMethod

        #endregion  ExtensionMethod
    }
}

