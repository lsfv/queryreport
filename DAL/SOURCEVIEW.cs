using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace CUSTOMRP.DAL
{
    /// <summary>
    /// 数据访问类:SOURCEVIEW
    /// </summary>
    public partial class SOURCEVIEW
    {
        public SOURCEVIEW()
        {}
        #region  BasicMethod



        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(int UserID, CUSTOMRP.Model.SOURCEVIEW model)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("insert into SOURCEVIEW(");
            strSql.Append("SOURCEVIEWNAME,DATABASEID,SOURCETYPE,TBLVIEWNAME,AUDODATE,VIEWLEVEL,[DESC],FORMATTYPE)");
            strSql.Append(" values (");
            strSql.Append("@SOURCEVIEWNAME,@DATABASEID,@SOURCETYPE,@TBLVIEWNAME,@AUDODATE,@VIEWLEVEL,@DESC,@FORMATTYPE)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                    new SqlParameter("@SOURCEVIEWNAME", SqlDbType.NVarChar,50),
                    new SqlParameter("@DATABASEID", SqlDbType.Int,4),
                    new SqlParameter("@SOURCETYPE", SqlDbType.Int,4),
                    new SqlParameter("@TBLVIEWNAME", SqlDbType.NVarChar,128),
                    new SqlParameter("@AUDODATE", SqlDbType.DateTime),
                    new SqlParameter("@VIEWLEVEL", SqlDbType.Decimal,9),
                    new SqlParameter("@DESC", SqlDbType.NVarChar,50),
                    new SqlParameter("@FORMATTYPE", SqlDbType.Int,4)};
            parameters[0].Value = model.SOURCEVIEWNAME;
            parameters[1].Value = model.DATABASEID;
            parameters[2].Value = model.SOURCETYPE;
            parameters[3].Value = model.TBLVIEWNAME;
            parameters[4].Value = model.AUDODATE;
            parameters[5].Value = model.VIEWLEVEL;
            parameters[6].Value = model.DESC;
            parameters[7].Value = model.FORMATTYPE;

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
                auditobj.ModuleName = "DAL.SOURCEVIEW.Add";
                auditobj.Message = String.Format(AppNum.AuditMessage.SourceViewInsertSuccess, model.ID);

                AUDITLOG.Add(auditobj);

                return model.ID;
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(int UserID, CUSTOMRP.Model.SOURCEVIEW model)
        {
            Model.SOURCEVIEW original = GetModel(UserID, model.ID);

            StringBuilder strSql=new StringBuilder();
            strSql.Append("update SOURCEVIEW set ");
            strSql.Append("SOURCEVIEWNAME=@SOURCEVIEWNAME,");
            strSql.Append("DATABASEID=@DATABASEID,");
            strSql.Append("SOURCETYPE=@SOURCETYPE,");
            strSql.Append("TBLVIEWNAME=@TBLVIEWNAME,");
            strSql.Append("AUDODATE=@AUDODATE,");
            strSql.Append("VIEWLEVEL=@VIEWLEVEL,");
            strSql.Append("[DESC]=@DESC,");
            strSql.Append("FORMATTYPE=@FORMATTYPE");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@SOURCEVIEWNAME", SqlDbType.NVarChar,50),
                    new SqlParameter("@DATABASEID", SqlDbType.Int,4),
                    new SqlParameter("@SOURCETYPE", SqlDbType.Int,4),
                    new SqlParameter("@TBLVIEWNAME", SqlDbType.NVarChar,128),
                    new SqlParameter("@AUDODATE", SqlDbType.DateTime),
                    new SqlParameter("@VIEWLEVEL", SqlDbType.Decimal,9),
                    new SqlParameter("@DESC", SqlDbType.NVarChar,50),
                    new SqlParameter("@FORMATTYPE", SqlDbType.Int,4),
                    new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = model.SOURCEVIEWNAME;
            parameters[1].Value = model.DATABASEID;
            parameters[2].Value = model.SOURCETYPE;
            parameters[3].Value = model.TBLVIEWNAME;
            parameters[4].Value = model.AUDODATE;
            parameters[5].Value = model.VIEWLEVEL;
            parameters[6].Value = model.DESC;
            parameters[7].Value = model.FORMATTYPE;
            parameters[8].Value = model.ID;

            int rows=DbHelperSQL.ExecuteSql(UserID, strSql.ToString(),parameters);
            if (rows > 0)
            {
                Model.AUDITLOG auditobj = model.GetAuditLogObject(original);
                auditobj.UserID = UserID;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.SOURCEVIEW.Update";
                auditobj.Message = String.Format(AppNum.AuditMessage.SourceViewUpdateSuccess, model.ID);

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
            Model.SOURCEVIEW original = GetModel(UserID, ID);

            StringBuilder strSql=new StringBuilder();
            strSql.Append("delete from SOURCEVIEW ");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)
            };
            parameters[0].Value = ID;

            int rows=DbHelperSQL.ExecuteSql(UserID, strSql.ToString(),parameters);
            if (rows > 0)
            {
                Model.AUDITLOG auditobj = original.GetAuditLogObject(null);
                auditobj.UserID = UserID;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.SOURCEVIEW.Delete";
                auditobj.Message = String.Format(AppNum.AuditMessage.SourceViewDeleteSuccess, ID);

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
            strSql.Append("delete from SOURCEVIEW ");
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
        public CUSTOMRP.Model.SOURCEVIEW GetModel(int UserID, int ID)
        {
            CUSTOMRP.Model.SOURCEVIEW result = null;
            StringBuilder strSql=new StringBuilder();
            strSql.Append("select top 1 ID,SOURCEVIEWNAME,DATABASEID,SOURCETYPE,TBLVIEWNAME,AUDODATE,VIEWLEVEL,[DESC],FORMATTYPE from SOURCEVIEW ");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)
            };
            parameters[0].Value = ID;

            CUSTOMRP.Model.SOURCEVIEW model=new CUSTOMRP.Model.SOURCEVIEW();
            DataSet ds=DbHelperSQL.Query(UserID, strSql.ToString(),parameters);
            if(ds.Tables[0].Rows.Count > 0)
            {
                result = DataRowToModel(ds.Tables[0].Rows[0]);
            }

            if (result != null)
            {
                DAL.WORDTEMPLATE dalWT = new DAL.WORDTEMPLATE();
                result.WordTemplate = dalWT.GetModelBySVID(UserID, ID, -1);
            }

            return result;
        }

        public CUSTOMRP.Model.SOURCEVIEW GetModelByQueryName(int UserID, string TBLVIEWNAME, int DATABASEID)
        {
            CUSTOMRP.Model.SOURCEVIEW result = null;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 ID,SOURCEVIEWNAME,DATABASEID,SOURCETYPE,TBLVIEWNAME,AUDODATE,VIEWLEVEL,[DESC],FORMATTYPE from SOURCEVIEW ");
            strSql.Append(" where TBLVIEWNAME = @TBLVIEWNAME AND DATABASEID = @DATABASEID");
            SqlParameter[] parameters = {
                    new SqlParameter("@TBLVIEWNAME", TBLVIEWNAME),
                    new SqlParameter("@DATABASEID", DATABASEID),
            };

            CUSTOMRP.Model.SOURCEVIEW model = new CUSTOMRP.Model.SOURCEVIEW();
            DataSet ds = DbHelperSQL.Query(UserID, strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                result = DataRowToModel(ds.Tables[0].Rows[0]);
            }

            if (result != null)
            {
                DAL.WORDTEMPLATE dalWT = new DAL.WORDTEMPLATE();
                result.WordTemplate = dalWT.GetModelBySVID(UserID, result.ID, -1);
            }

            return result;
        }



        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public CUSTOMRP.Model.SOURCEVIEW DataRowToModel(DataRow row)
        {
            CUSTOMRP.Model.SOURCEVIEW model=new CUSTOMRP.Model.SOURCEVIEW();
            if (row != null)
            {
                if(row["ID"]!=null && row["ID"].ToString()!="")
                {
                    model.ID=Int32.Parse(row["ID"].ToString());
                }
                if(row["SOURCEVIEWNAME"]!=null)
                {
                    model.SOURCEVIEWNAME=row["SOURCEVIEWNAME"].ToString();
                }
                if(row["DATABASEID"]!=null && row["DATABASEID"].ToString()!="")
                {
                    model.DATABASEID=Int32.Parse(row["DATABASEID"].ToString());
                }
                if (row["SOURCETYPE"] != null && row["SOURCETYPE"].ToString() != "")
                {
                    model.SOURCETYPE = Int32.Parse(row["SOURCETYPE"].ToString());
                }
                if (row["TBLVIEWNAME"] != null)
                {
                    model.TBLVIEWNAME = row["TBLVIEWNAME"].ToString();
                } 
                if (row["AUDODATE"] != null && row["AUDODATE"].ToString() != "")
                {
                    model.AUDODATE=DateTime.Parse(row["AUDODATE"].ToString());
                }
                if(row["VIEWLEVEL"]!=null && row["VIEWLEVEL"].ToString()!="")
                {
                    model.VIEWLEVEL=Decimal.Parse(row["VIEWLEVEL"].ToString());
                }
                if(row["DESC"]!=null)
                {
                    model.DESC=row["DESC"].ToString();
                }
                if (row["FORMATTYPE"] != null && row["FORMATTYPE"].ToString() != "")
                {
                    model.FORMATTYPE = Int32.Parse(row["FORMATTYPE"].ToString());
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
            strSql.Append("select ID,SOURCEVIEWNAME,DATABASEID,SOURCETYPE,TBLVIEWNAME,AUDODATE,VIEWLEVEL,[DESC],FORMATTYPE ");
            strSql.Append(" FROM SOURCEVIEW ");
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
            strSql.Append(" ID,SOURCEVIEWNAME,DATABASEID,SOURCETYPE,TBLVIEWNAME,AUDODATE,VIEWLEVEL,[DESC],FORMATTYPE ");
            strSql.Append(" FROM SOURCEVIEW ");
            if(strWhere.Trim()!="")
            {
                strSql.Append(" where "+strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return DbHelperSQL.Query(UserID, strSql.ToString());
        }

        /// <summary>
        /// Get Query List for dropdown on rpexcel / rpworddetail
        /// </summary>
        /// <param name="me">Current user, used to determine privilege level</param>
        /// <param name="FormatType">1 = Excel, 2 = Word, 3 = All</param>
        public DataSet GetQueryListForDropdown(CUSTOMRP.Model.LoginUser me, int FormatType)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select [ID], [SOURCEVIEWNAME] + '  |  ' +[DESC] [NewDesc] ");
            strSql.Append(" FROM [SOURCEVIEW] ");
            strSql.AppendFormat(" WHERE [VIEWLEVEL]>={0} AND [DATABASEID]={1} AND (FORMATTYPE & {2}) = {2}", me.ViewLevel, me.DatabaseID, FormatType);
            strSql.Append(" ORDER BY [SOURCEVIEWNAME], [DESC]");
            return DbHelperSQL.Query(me.ID, strSql.ToString());
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(int UserID, string strWhere)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("select count(1) FROM SOURCEVIEW ");
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
                strSql.Append("order by T.ID DESC");
            }
            strSql.Append(")AS Row, T.*  from SOURCEVIEW T ");
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
            parameters[0].Value = "SOURCEVIEW";
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
        public DataTable GetcustomList(int UserID, string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select sourceview.ID,SOURCEVIEWNAME,sourceview.DATABASEID,sourceview.SOURCETYPE,sourceview.TBLVIEWNAME,AUDODATE,sensitivitylevel.name,[DESC],FORMATTYPE ");
            strSql.Append(" FROM SOURCEVIEW left join sensitivitylevel on sourceview.viewlevel=sensitivitylevel.slevel and sensitivitylevel.databaseid=sourceview.databaseid");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperSQL.Query(UserID, strSql.ToString()).Tables[0];
        }
        #endregion  ExtensionMethod
    }
}