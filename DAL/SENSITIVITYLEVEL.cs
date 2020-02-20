using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace CUSTOMRP.DAL
{
    /// <summary>
    /// 数据访问类:SensitivityLevel
    /// </summary>
    public partial class SensitivityLevel
    {
        public SensitivityLevel()
        {}
        #region  BasicMethod



        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(int UserID, CUSTOMRP.Model.SensitivityLevel model)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("insert into SensitivityLevel(");
            strSql.Append("DATABASEID,NAME,SLEVEL,DESCRIPTION,AUDOTIME)");
            strSql.Append(" values (");
            strSql.Append("@DATABASEID,@NAME,@SLEVEL,@DESCRIPTION,@AUDOTIME)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                    new SqlParameter("@DATABASEID", SqlDbType.Int,4),
                    new SqlParameter("@NAME", SqlDbType.NVarChar,50),
                    new SqlParameter("@SLEVEL", SqlDbType.Decimal,9),
                    new SqlParameter("@DESCRIPTION", SqlDbType.NVarChar,-1),
                    new SqlParameter("@AUDOTIME", SqlDbType.DateTime)};
            parameters[0].Value = model.DATABASEID;
            parameters[1].Value = model.NAME;
            parameters[2].Value = model.SLEVEL;
            parameters[3].Value = model.DESCRIPTION;
            parameters[4].Value = model.AUDOTIME;

            object obj = DbHelperSQL.GetSingle(UserID, strSql.ToString(),parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                model.ID = Convert.ToInt32(obj);

                Model.AUDITLOG auditobj = model.GetAuditLogObject(null);
                auditobj.UserID = UserID;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.SENSITIVITYLEVEL.Add";
                auditobj.Message = String.Format(AppNum.AuditMessage.SensitivityLevelInsertSuccess, model.ID);

                AUDITLOG.Add(auditobj);

                return Convert.ToInt32(obj);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(int UserID, CUSTOMRP.Model.SensitivityLevel model)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("update SensitivityLevel set ");
            strSql.Append("DATABASEID=@DATABASEID,");
            strSql.Append("NAME=@NAME,");
            strSql.Append("SLEVEL=@SLEVEL,");
            strSql.Append("DESCRIPTION=@DESCRIPTION,");
            strSql.Append("AUDOTIME=@AUDOTIME");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@DATABASEID", SqlDbType.Int,4),
                    new SqlParameter("@NAME", SqlDbType.NVarChar,50),
                    new SqlParameter("@SLEVEL", SqlDbType.Decimal,9),
                    new SqlParameter("@DESCRIPTION", SqlDbType.NVarChar,-1),
                    new SqlParameter("@AUDOTIME", SqlDbType.DateTime),
                    new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = model.DATABASEID;
            parameters[1].Value = model.NAME;
            parameters[2].Value = model.SLEVEL;
            parameters[3].Value = model.DESCRIPTION;
            parameters[4].Value = model.AUDOTIME;
            parameters[5].Value = model.ID;

            int rows=DbHelperSQL.ExecuteSql(UserID, strSql.ToString(),parameters);
            if (rows > 0)
            {
                Model.AUDITLOG auditobj = model.GetAuditLogObject(null);
                auditobj.UserID = UserID;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.SENSITIVITYLEVEL.Update";
                auditobj.Message = String.Format(AppNum.AuditMessage.SensitivityLevelUpdateSuccess, model.ID);

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
            strSql.Append("delete from SensitivityLevel ");
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
                auditobj.ModuleName = "DAL.SENSITIVITYLEVEL.Delete";
                auditobj.Message = String.Format(AppNum.AuditMessage.SensitivityLevelDeleteSuccess, ID);

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
        //public bool DeleteList(int UserID, string IDlist)
        //{
        //    StringBuilder strSql=new StringBuilder();
        //    strSql.Append("delete from SensitivityLevel ");
        //    strSql.Append(" where ID in ("+IDlist + ")  ");
        //    int rows=DbHelperSQL.ExecuteSql(UserID, strSql.ToString());
        //    if (rows > 0)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public CUSTOMRP.Model.SensitivityLevel GetModel(int UserID, int ID)
        {
            
            StringBuilder strSql=new StringBuilder();
            strSql.Append("select  top 1 ID,DATABASEID,NAME,SLEVEL,DESCRIPTION,AUDOTIME from SensitivityLevel ");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)
            };
            parameters[0].Value = ID;

            CUSTOMRP.Model.SensitivityLevel model=new CUSTOMRP.Model.SensitivityLevel();
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
        public CUSTOMRP.Model.SensitivityLevel DataRowToModel(DataRow row)
        {
            CUSTOMRP.Model.SensitivityLevel model=new CUSTOMRP.Model.SensitivityLevel();
            if (row != null)
            {
                if(row["ID"]!=null && row["ID"].ToString()!="")
                {
                    model.ID=Int32.Parse(row["ID"].ToString());
                }
                if(row["DATABASEID"]!=null && row["DATABASEID"].ToString()!="")
                {
                    model.DATABASEID=Int32.Parse(row["DATABASEID"].ToString());
                }
                if(row["NAME"]!=null)
                {
                    model.NAME=row["NAME"].ToString();
                }
                if(row["SLEVEL"]!=null && row["SLEVEL"].ToString()!="")
                {
                    model.SLEVEL=Decimal.Parse(row["SLEVEL"].ToString());
                }
                if(row["DESCRIPTION"]!=null)
                {
                    model.DESCRIPTION=row["DESCRIPTION"].ToString();
                }
                if(row["AUDOTIME"]!=null && row["AUDOTIME"].ToString()!="")
                {
                    model.AUDOTIME=DateTime.Parse(row["AUDOTIME"].ToString());
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
            strSql.Append("select ID,DATABASEID,NAME,SLEVEL,DESCRIPTION,AUDOTIME ");
            strSql.Append(" FROM SensitivityLevel ");
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
            strSql.Append(" ID,DATABASEID,NAME,SLEVEL,DESCRIPTION,AUDOTIME ");
            strSql.Append(" FROM SensitivityLevel ");
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
            strSql.Append("select count(1) FROM SensitivityLevel ");
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
            strSql.Append(")AS Row, T.*  from SensitivityLevel T ");
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
            parameters[0].Value = "SensitivityLevel";
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

