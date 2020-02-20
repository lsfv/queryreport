using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace CUSTOMRP.DAL
{
    /// <summary>
    /// 数据访问类:GROUPRIGHT
    /// </summary>
    public partial class GROUPRIGHT
    {
        public GROUPRIGHT()
        {}
        #region  BasicMethod

        /// <summary>
        /// 得到最大ID
        /// </summary>
        public int GetMaxId(int UserID)
        {
        return DbHelperSQL.GetMaxID(UserID, "GID", "GROUPRIGHT"); 
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int UserID, int GID)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("select count(1) from GROUPRIGHT");
            strSql.Append(" where GID=@GID ");
            SqlParameter[] parameters = {
                    new SqlParameter("@GID", SqlDbType.Int,4)			};
            parameters[0].Value = GID;

            return Convert.ToInt32(DbHelperSQL.GetSingle(UserID, strSql.ToString(),parameters)) > 0;
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(int UserID, CUSTOMRP.Model.GROUPRIGHT model)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("insert into GROUPRIGHT(");
            strSql.Append("GID,COMPANY,REPORTGROUP,CATEGARY,SECURITY,QUERY,USERGROUP,USERGROUPRIGHT,USERSETUP,AUDODATE,EXTEND1,EXTEND2,EXTEND3)");
            strSql.Append(" values (");
            strSql.Append("@GID,@COMPANY,@REPORTGROUP,@CATEGARY,@SECURITY,@QUERY,@USERGROUP,@USERGROUPRIGHT,@USERSETUP,@AUDODATE,@EXTEND1,@EXTEND2,@EXTEND3)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                    new SqlParameter("@GID", SqlDbType.Int,4),
                    new SqlParameter("@COMPANY", SqlDbType.NVarChar,50),
                    new SqlParameter("@REPORTGROUP", SqlDbType.NVarChar,50),
                    new SqlParameter("@CATEGARY", SqlDbType.NVarChar,50),
                    new SqlParameter("@SECURITY", SqlDbType.NVarChar,50),
                    new SqlParameter("@QUERY", SqlDbType.NVarChar,50),
                    new SqlParameter("@USERGROUP", SqlDbType.NVarChar,50),
                    new SqlParameter("@USERGROUPRIGHT", SqlDbType.NVarChar,50),
                    new SqlParameter("@USERSETUP", SqlDbType.NVarChar,50),
                    new SqlParameter("@AUDODATE", SqlDbType.DateTime),
                    new SqlParameter("@EXTEND1", SqlDbType.NVarChar,50),
                    new SqlParameter("@EXTEND2", SqlDbType.NVarChar,50),
                    new SqlParameter("@EXTEND3", SqlDbType.NVarChar,-1)};
            parameters[0].Value = model.GID;
            parameters[1].Value = model.COMPANY;
            parameters[2].Value = model.REPORTGROUP;
            parameters[3].Value = model.CATEGARY;
            parameters[4].Value = model.SECURITY;
            parameters[5].Value = model.QUERY;
            parameters[6].Value = model.USERGROUP;
            parameters[7].Value = model.USERGROUPRIGHT;
            parameters[8].Value = model.USERSETUP;
            parameters[9].Value = model.AUDODATE;
            parameters[10].Value = model.EXTEND1;
            parameters[11].Value = model.EXTEND2;
            parameters[12].Value = model.EXTEND3;

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
                auditobj.ModuleName = "DAL.GROUPRIGHT.Add";
                auditobj.Message = String.Format(AppNum.AuditMessage.GroupRightInsertSuccess, model.ID);

                AUDITLOG.Add(auditobj);

                return Convert.ToInt32(obj);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(int UserID, CUSTOMRP.Model.GROUPRIGHT model)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("update GROUPRIGHT set ");
            strSql.Append("COMPANY=@COMPANY,");
            strSql.Append("REPORTGROUP=@REPORTGROUP,");
            strSql.Append("CATEGARY=@CATEGARY,");
            strSql.Append("SECURITY=@SECURITY,");
            strSql.Append("QUERY=@QUERY,");
            strSql.Append("USERGROUP=@USERGROUP,");
            strSql.Append("USERGROUPRIGHT=@USERGROUPRIGHT,");
            strSql.Append("USERSETUP=@USERSETUP,");
            strSql.Append("AUDODATE=@AUDODATE,");
            strSql.Append("EXTEND1=@EXTEND1,");
            strSql.Append("EXTEND2=@EXTEND2,");
            strSql.Append("EXTEND3=@EXTEND3");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@COMPANY", SqlDbType.NVarChar,50),
                    new SqlParameter("@REPORTGROUP", SqlDbType.NVarChar,50),
                    new SqlParameter("@CATEGARY", SqlDbType.NVarChar,50),
                    new SqlParameter("@SECURITY", SqlDbType.NVarChar,50),
                    new SqlParameter("@QUERY", SqlDbType.NVarChar,50),
                    new SqlParameter("@USERGROUP", SqlDbType.NVarChar,50),
                    new SqlParameter("@USERGROUPRIGHT", SqlDbType.NVarChar,50),
                    new SqlParameter("@USERSETUP", SqlDbType.NVarChar,50),
                    new SqlParameter("@AUDODATE", SqlDbType.DateTime),
                    new SqlParameter("@EXTEND1", SqlDbType.NVarChar,50),
                    new SqlParameter("@EXTEND2", SqlDbType.NVarChar,50),
                    new SqlParameter("@EXTEND3", SqlDbType.NVarChar,-1),
                    new SqlParameter("@ID", SqlDbType.Int,4),
                    new SqlParameter("@GID", SqlDbType.Int,4)};
            parameters[0].Value = model.COMPANY;
            parameters[1].Value = model.REPORTGROUP;
            parameters[2].Value = model.CATEGARY;
            parameters[3].Value = model.SECURITY;
            parameters[4].Value = model.QUERY;
            parameters[5].Value = model.USERGROUP;
            parameters[6].Value = model.USERGROUPRIGHT;
            parameters[7].Value = model.USERSETUP;
            parameters[8].Value = model.AUDODATE;
            parameters[9].Value = model.EXTEND1;
            parameters[10].Value = model.EXTEND2;
            parameters[11].Value = model.EXTEND3;
            parameters[12].Value = model.ID;
            parameters[13].Value = model.GID;

            int rows=DbHelperSQL.ExecuteSql(UserID, strSql.ToString(),parameters);
            if (rows > 0)
            {
                Model.AUDITLOG auditobj = model.GetAuditLogObject(null);
                auditobj.UserID = UserID;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.GROUPRIGHT.Update";
                auditobj.Message = String.Format(AppNum.AuditMessage.GroupRightUpdateSuccess, model.ID);

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
        public bool Delete(int UserID, int GID)
        {
            
            StringBuilder strSql=new StringBuilder();
            strSql.Append("delete from GROUPRIGHT ");
            strSql.Append(" where GID=@GID ");
            SqlParameter[] parameters = {
                    new SqlParameter("@GID", SqlDbType.Int,4)			};
            parameters[0].Value = GID;

            int rows=DbHelperSQL.ExecuteSql(UserID, strSql.ToString(),parameters);
            if (rows > 0)
            {
                Model.AUDITLOG auditobj = new Model.AUDITLOG();
                auditobj.UserID = UserID;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.GROUPRIGHT.Delete";
                auditobj.Message = String.Format(AppNum.AuditMessage.GroupRightDeleteSuccess, GID);

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
        //    strSql.Append("delete from GROUPRIGHT ");
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
        public CUSTOMRP.Model.GROUPRIGHT GetModel(int UserID, int ID)
        {
            
            StringBuilder strSql=new StringBuilder();
            strSql.Append("select  top 1 ID,GID,COMPANY,REPORTGROUP,CATEGARY,SECURITY,QUERY,USERGROUP,USERGROUPRIGHT,USERSETUP,AUDODATE,EXTEND1,EXTEND2,EXTEND3 from GROUPRIGHT ");
            strSql.Append(" where GID=@GID");
            SqlParameter[] parameters = {
                    new SqlParameter("@GID", SqlDbType.Int,4)
            };
            parameters[0].Value = ID;

            CUSTOMRP.Model.GROUPRIGHT model=new CUSTOMRP.Model.GROUPRIGHT();
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
        public CUSTOMRP.Model.GROUPRIGHT DataRowToModel(DataRow row)
        {
            CUSTOMRP.Model.GROUPRIGHT model=new CUSTOMRP.Model.GROUPRIGHT();
            if (row != null)
            {
                if(row["ID"]!=null && row["ID"].ToString()!="")
                {
                    model.ID=Int32.Parse(row["ID"].ToString());
                }
                if(row["GID"]!=null && row["GID"].ToString()!="")
                {
                    model.GID=Int32.Parse(row["GID"].ToString());
                }
                if(row["COMPANY"]!=null)
                {
                    model.COMPANY=row["COMPANY"].ToString();
                }
                if(row["REPORTGROUP"]!=null)
                {
                    model.REPORTGROUP=row["REPORTGROUP"].ToString();
                }
                if(row["CATEGARY"]!=null)
                {
                    model.CATEGARY=row["CATEGARY"].ToString();
                }
                if(row["SECURITY"]!=null)
                {
                    model.SECURITY=row["SECURITY"].ToString();
                }
                if(row["QUERY"]!=null)
                {
                    model.QUERY=row["QUERY"].ToString();
                }
                if(row["USERGROUP"]!=null)
                {
                    model.USERGROUP=row["USERGROUP"].ToString();
                }
                if(row["USERGROUPRIGHT"]!=null)
                {
                    model.USERGROUPRIGHT=row["USERGROUPRIGHT"].ToString();
                }
                if(row["USERSETUP"]!=null)
                {
                    model.USERSETUP=row["USERSETUP"].ToString();
                }
                if(row["AUDODATE"]!=null && row["AUDODATE"].ToString()!="")
                {
                    model.AUDODATE=DateTime.Parse(row["AUDODATE"].ToString());
                }
                if(row["EXTEND1"]!=null)
                {
                    model.EXTEND1=row["EXTEND1"].ToString();
                }
                if(row["EXTEND2"]!=null)
                {
                    model.EXTEND2=row["EXTEND2"].ToString();
                }
                if(row["EXTEND3"]!=null)
                {
                    model.EXTEND3=row["EXTEND3"].ToString();
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
            strSql.Append("select ID,GID,COMPANY,REPORTGROUP,CATEGARY,SECURITY,QUERY,USERGROUP,USERGROUPRIGHT,USERSETUP,AUDODATE,EXTEND1,EXTEND2,EXTEND3 ");
            strSql.Append(" FROM GROUPRIGHT ");
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
            strSql.Append(" ID,GID,COMPANY,REPORTGROUP,CATEGARY,SECURITY,QUERY,USERGROUP,USERGROUPRIGHT,USERSETUP,AUDODATE,EXTEND1,EXTEND2,EXTEND3 ");
            strSql.Append(" FROM GROUPRIGHT ");
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
            strSql.Append("select count(1) FROM GROUPRIGHT ");
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
            strSql.Append(")AS Row, T.*  from GROUPRIGHT T ");
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
            parameters[0].Value = "GROUPRIGHT";
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

