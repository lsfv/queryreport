﻿using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace CUSTOMRP.DAL
{
    /// <summary>
    /// 数据访问类:USERGROUP
    /// </summary>
    public partial class USERGROUP
    {
        public USERGROUP()
        {}
        #region  BasicMethod



        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(int UserID, CUSTOMRP.Model.USERGROUP model)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("insert into USERGROUP(");
            strSql.Append("DATABASEID,NAME,DESCRIPTION,AUDODATE,EXTEND)");
            strSql.Append(" values (");
            strSql.Append("@DATABASEID,@NAME,@DESCRIPTION,@AUDODATE,@EXTEND)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                    new SqlParameter("@DATABASEID", SqlDbType.Int,4),
                    new SqlParameter("@NAME", SqlDbType.NVarChar,50),
                    new SqlParameter("@DESCRIPTION", SqlDbType.NVarChar,-1),
                    new SqlParameter("@AUDODATE", SqlDbType.DateTime),
                    new SqlParameter("@EXTEND", SqlDbType.NVarChar,-1)};
            parameters[0].Value = model.DATABASEID;
            parameters[1].Value = model.NAME;
            parameters[2].Value = model.DESCRIPTION;
            parameters[3].Value = model.AUDODATE;
            parameters[4].Value = model.EXTEND;

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
                auditobj.ModuleName = "DAL.USERGROUP.Add";
                auditobj.Message = String.Format(AppNum.AuditMessage.UserGroupInsertSuccess, model.ID);

                AUDITLOG.Add(auditobj);

                return model.ID;
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(int UserID, CUSTOMRP.Model.USERGROUP model)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("update USERGROUP set ");
            strSql.Append("DATABASEID=@DATABASEID,");
            strSql.Append("NAME=@NAME,");
            strSql.Append("DESCRIPTION=@DESCRIPTION,");
            strSql.Append("AUDODATE=@AUDODATE,");
            strSql.Append("EXTEND=@EXTEND");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@DATABASEID", SqlDbType.Int,4),
                    new SqlParameter("@NAME", SqlDbType.NVarChar,50),
                    new SqlParameter("@DESCRIPTION", SqlDbType.NVarChar,-1),
                    new SqlParameter("@AUDODATE", SqlDbType.DateTime),
                    new SqlParameter("@EXTEND", SqlDbType.NVarChar,-1),
                    new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = model.DATABASEID;
            parameters[1].Value = model.NAME;
            parameters[2].Value = model.DESCRIPTION;
            parameters[3].Value = model.AUDODATE;
            parameters[4].Value = model.EXTEND;
            parameters[5].Value = model.ID;

            int rows=DbHelperSQL.ExecuteSql(UserID, strSql.ToString(),parameters);
            if (rows > 0)
            {
                Model.AUDITLOG auditobj = model.GetAuditLogObject(null);
                auditobj.UserID = UserID;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.USERGROUP.Update";
                auditobj.Message = String.Format(AppNum.AuditMessage.UserGroupUpdateSuccess, model.ID);

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
            strSql.Append("delete from USERGROUP ");
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
                auditobj.ModuleName = "DAL.USERGROUP.Delete";
                auditobj.Message = String.Format(AppNum.AuditMessage.UserGroupDeleteSuccess, ID);

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
        //    strSql.Append("delete from USERGROUP ");
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
        public CUSTOMRP.Model.USERGROUP GetModel(int UserID, int ID)
        {
            
            StringBuilder strSql=new StringBuilder();
            strSql.Append("select  top 1 ID,DATABASEID,NAME,DESCRIPTION,AUDODATE,EXTEND from USERGROUP ");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)
            };
            parameters[0].Value = ID;

            CUSTOMRP.Model.USERGROUP model=new CUSTOMRP.Model.USERGROUP();
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
        public CUSTOMRP.Model.USERGROUP DataRowToModel(DataRow row)
        {
            CUSTOMRP.Model.USERGROUP model=new CUSTOMRP.Model.USERGROUP();
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
                if(row["DESCRIPTION"]!=null)
                {
                    model.DESCRIPTION=row["DESCRIPTION"].ToString();
                }
                if(row["AUDODATE"]!=null && row["AUDODATE"].ToString()!="")
                {
                    model.AUDODATE=DateTime.Parse(row["AUDODATE"].ToString());
                }
                if(row["EXTEND"]!=null)
                {
                    model.EXTEND=row["EXTEND"].ToString();
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
            strSql.Append("select ID,DATABASEID,NAME,DESCRIPTION,AUDODATE,EXTEND ");
            strSql.Append(" FROM USERGROUP ");
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
            strSql.Append(" ID,DATABASEID,NAME,DESCRIPTION,AUDODATE,EXTEND ");
            strSql.Append(" FROM USERGROUP ");
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
            strSql.Append("select count(1) FROM USERGROUP ");
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
            strSql.Append(")AS Row, T.*  from USERGROUP T ");
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
            parameters[0].Value = "USERGROUP";
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

