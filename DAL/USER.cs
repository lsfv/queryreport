using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace CUSTOMRP.DAL
{
    /// <summary>
    /// 数据访问类:USER
    /// </summary>
    public partial class USER
    {
        public USER()
        {}
        #region  BasicMethod

        /// <summary>
        /// 得到最大ID
        /// </summary>
        public int GetMaxId(int UserID)
        {
            return DbHelperSQL.GetMaxID(UserID, "GID", "USER");
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int UserID, string UID, int GID, int ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from USER");
            strSql.Append(" where UID=@UID and GID=@GID and ID=@ID ");
            SqlParameter[] parameters = {
                    new SqlParameter("@UID", SqlDbType.NVarChar,200),
                    new SqlParameter("@GID", SqlDbType.Int,4),
                    new SqlParameter("@ID", SqlDbType.Int,4)			};
            parameters[0].Value = UID;
            parameters[1].Value = GID;
            parameters[2].Value = ID;

            return Convert.ToInt32(DbHelperSQL.GetSingle(UserID, strSql.ToString(), parameters)) > 0;
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(CUSTOMRP.Model.USER model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [USER](");
            strSql.Append("UID,GID,DATABASEID,PASSWORD,VIEWLEVEL,REPORTGROUPLIST,USERGROUPLEVEL,SETUPUSER,REPORTRIGHT,AUTODATE,EMAIL,USERGROUP,NAME,SENSITIVITYLEVEL)");
            strSql.Append(" values (");
            strSql.Append("@UID,@GID,@DATABASEID,@PASSWORD,@VIEWLEVEL,@REPORTGROUPLIST,@USERGROUPLEVEL,@SETUPUSER,@REPORTRIGHT,@AUTODATE,@EMAIL,@USERGROUP,@NAME,@SENSITIVITYLEVEL)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                    new SqlParameter("@UID", SqlDbType.NVarChar,200),
                    new SqlParameter("@GID", SqlDbType.Int,4),
                    new SqlParameter("@DATABASEID", SqlDbType.Int,4),
                    new SqlParameter("@PASSWORD", SqlDbType.NVarChar,50),
                    new SqlParameter("@VIEWLEVEL", SqlDbType.NVarChar,2000),
                    new SqlParameter("@REPORTGROUPLIST", SqlDbType.NVarChar,2000),
                    new SqlParameter("@USERGROUPLEVEL", SqlDbType.NVarChar,2000),
                    new SqlParameter("@SETUPUSER", SqlDbType.Int,4),
                    new SqlParameter("@REPORTRIGHT", SqlDbType.Int,4),
                    new SqlParameter("@AUTODATE", SqlDbType.DateTime),
                    new SqlParameter("@EMAIL", SqlDbType.NVarChar,2000),
                    new SqlParameter("@USERGROUP", SqlDbType.NVarChar,100),
                    new SqlParameter("@NAME", SqlDbType.NVarChar,50),
                    new SqlParameter("@SENSITIVITYLEVEL", SqlDbType.Decimal,9)};
            parameters[0].Value = model.UID;
            parameters[1].Value = model.GID;
            parameters[2].Value = model.DATABASEID;
            parameters[3].Value = model.PASSWORD;
            parameters[4].Value = model.VIEWLEVEL;
            parameters[5].Value = model.REPORTGROUPLIST;
            parameters[6].Value = model.USERGROUPLEVEL;
            parameters[7].Value = model.SETUPUSER;
            parameters[8].Value = model.REPORTRIGHT;
            parameters[9].Value = model.AUTODATE;
            parameters[10].Value = model.EMAIL;
            parameters[11].Value = model.USERGROUP;
            parameters[12].Value = model.NAME;
            parameters[13].Value = model.SENSITIVITYLEVEL;

            object obj = DbHelperSQL.GetSingle(model.SETUPUSER, strSql.ToString(), parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                model.ID = Convert.ToInt32(obj);

                Model.AUDITLOG auditobj = model.GetAuditLogObject(null);
                auditobj.UserID = model.SETUPUSER;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.USER.Add";
                auditobj.Message = String.Format(AppNum.AuditMessage.UserInsertSuccess, model.ID);

                AUDITLOG.Add(auditobj);

                return model.ID;
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(CUSTOMRP.Model.USER model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [USER] set ");
            strSql.Append("DATABASEID=@DATABASEID,");
            strSql.Append("PASSWORD=@PASSWORD,");
            strSql.Append("VIEWLEVEL=@VIEWLEVEL,");
            strSql.Append("REPORTGROUPLIST=@REPORTGROUPLIST,");
            strSql.Append("USERGROUPLEVEL=@USERGROUPLEVEL,");
            strSql.Append("SETUPUSER=@SETUPUSER,");
            strSql.Append("REPORTRIGHT=@REPORTRIGHT,");
            strSql.Append("AUTODATE=@AUTODATE,");
            strSql.Append("EMAIL=@EMAIL,");
            strSql.Append("USERGROUP=@USERGROUP,");
            strSql.Append("NAME=@NAME,");
            strSql.Append("SENSITIVITYLEVEL=@SENSITIVITYLEVEL,");
            strSql.Append("GID=@GID");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@DATABASEID", SqlDbType.Int,4),
                    new SqlParameter("@PASSWORD", SqlDbType.NVarChar,50),
                    new SqlParameter("@VIEWLEVEL", SqlDbType.NVarChar,2000),
                    new SqlParameter("@REPORTGROUPLIST", SqlDbType.NVarChar,2000),
                    new SqlParameter("@USERGROUPLEVEL", SqlDbType.NVarChar,2000),
                    new SqlParameter("@SETUPUSER", SqlDbType.Int,4),
                    new SqlParameter("@REPORTRIGHT", SqlDbType.Int,4),
                    new SqlParameter("@AUTODATE", SqlDbType.DateTime),
                    new SqlParameter("@EMAIL", SqlDbType.NVarChar,2000),
                    new SqlParameter("@USERGROUP", SqlDbType.NVarChar,100),
                    new SqlParameter("@NAME", SqlDbType.NVarChar,50),
                    new SqlParameter("@SENSITIVITYLEVEL", SqlDbType.Decimal,9),
                    new SqlParameter("@ID", SqlDbType.Int,4),
                    new SqlParameter("@UID", SqlDbType.NVarChar,200),
                    new SqlParameter("@GID", SqlDbType.Int,4)};
            parameters[0].Value = model.DATABASEID;
            parameters[1].Value = model.PASSWORD;
            parameters[2].Value = model.VIEWLEVEL;
            parameters[3].Value = model.REPORTGROUPLIST;
            parameters[4].Value = model.USERGROUPLEVEL;
            parameters[5].Value = model.SETUPUSER;
            parameters[6].Value = model.REPORTRIGHT;
            parameters[7].Value = model.AUTODATE;
            parameters[8].Value = model.EMAIL;
            parameters[9].Value = model.USERGROUP;
            parameters[10].Value = model.NAME;
            parameters[11].Value = model.SENSITIVITYLEVEL;
            parameters[12].Value = model.ID;
            parameters[13].Value = model.UID;
            parameters[14].Value = model.GID;

            int rows = DbHelperSQL.ExecuteSql(model.SETUPUSER, strSql.ToString(), parameters);
            if (rows > 0)
            {
                Model.AUDITLOG auditobj = model.GetAuditLogObject(null);
                auditobj.UserID = model.SETUPUSER;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.USER.Update";
                auditobj.Message = String.Format(AppNum.AuditMessage.UserUpdateSuccess, model.ID);

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
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [USER] ");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)
            };
            parameters[0].Value = ID;

            int rows = DbHelperSQL.ExecuteSql(UserID, strSql.ToString(), parameters);
            if (rows > 0)
            {
                Model.AUDITLOG auditobj = new Model.AUDITLOG();
                auditobj.UserID = UserID;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.USER.Delete";
                auditobj.Message = String.Format(AppNum.AuditMessage.UserDeleteSuccess, ID);

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
        public bool Delete(int UserID, string UID, int GID, int ID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [USER] ");
            strSql.Append(" where UID=@UID and GID=@GID and ID=@ID ");
            SqlParameter[] parameters = {
                    new SqlParameter("@UID", SqlDbType.NVarChar,200),
                    new SqlParameter("@GID", SqlDbType.Int,4),
                    new SqlParameter("@ID", SqlDbType.Int,4)			};
            parameters[0].Value = UID;
            parameters[1].Value = GID;
            parameters[2].Value = ID;

            int rows = DbHelperSQL.ExecuteSql(UserID, strSql.ToString(), parameters);
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
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(int UserID, string IDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [USER] ");
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
        public CUSTOMRP.Model.USER GetModel(int UserID, int ID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,UID,GID,DATABASEID,PASSWORD,VIEWLEVEL,REPORTGROUPLIST,USERGROUPLEVEL,SETUPUSER,REPORTRIGHT,AUTODATE,EMAIL,USERGROUP,NAME,SENSITIVITYLEVEL from [USER] ");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)
            };
            parameters[0].Value = ID;

            CUSTOMRP.Model.USER model = new CUSTOMRP.Model.USER();
            DataSet ds = DbHelperSQL.Query(UserID, strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        public CUSTOMRP.Model.USER GetModelForUser(int UserID, string LoginID, int DatabaseID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 ID,UID,GID,DATABASEID,PASSWORD,VIEWLEVEL,REPORTGROUPLIST,USERGROUPLEVEL,SETUPUSER,REPORTRIGHT,AUTODATE,EMAIL,USERGROUP,NAME,SENSITIVITYLEVEL from [USER]");
            strSql.Append(" where UID = @UID AND DATABASEID = @DATABASEID");

            CUSTOMRP.Model.USER model = new CUSTOMRP.Model.USER();
            DataSet ds = DbHelperSQL.Query(UserID, strSql.ToString(), new SqlParameter("@UID", LoginID), new SqlParameter("@DATABASEID", DatabaseID));
            if (ds.Tables[0].Rows.Count > 0)
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
        public CUSTOMRP.Model.USER DataRowToModel(DataRow row)
        {
            CUSTOMRP.Model.USER model = new CUSTOMRP.Model.USER();
            if (row != null)
            {
                if (row["ID"] != null && row["ID"].ToString() != "")
                {
                    model.ID = Int32.Parse(row["ID"].ToString());
                }
                if (row["UID"] != null)
                {
                    model.UID = row["UID"].ToString();
                }
                if (row["GID"] != null && row["GID"].ToString() != "")
                {
                    model.GID = Int32.Parse(row["GID"].ToString());
                }
                if (row["DATABASEID"] != null && row["DATABASEID"].ToString() != "")
                {
                    model.DATABASEID = Int32.Parse(row["DATABASEID"].ToString());
                }
                if (row["PASSWORD"] != null)
                {
                    model.PASSWORD = row["PASSWORD"].ToString();
                }
                if (row["VIEWLEVEL"] != null)
                {
                    model.VIEWLEVEL = row["VIEWLEVEL"].ToString();
                }
                if (row["REPORTGROUPLIST"] != null)
                {
                    model.REPORTGROUPLIST = row["REPORTGROUPLIST"].ToString();
                }
                if (row["USERGROUPLEVEL"] != null)
                {
                    model.USERGROUPLEVEL = row["USERGROUPLEVEL"].ToString();
                }
                if (row["SETUPUSER"] != null && row["SETUPUSER"].ToString() != "")
                {
                    model.SETUPUSER = Int32.Parse(row["SETUPUSER"].ToString());
                }
                if (row["REPORTRIGHT"] != null && row["REPORTRIGHT"].ToString() != "")
                {
                    model.REPORTRIGHT = Int32.Parse(row["REPORTRIGHT"].ToString());
                }
                if (row["AUTODATE"] != null && row["AUTODATE"].ToString() != "")
                {
                    model.AUTODATE = DateTime.Parse(row["AUTODATE"].ToString());
                }
                if (row["EMAIL"] != null)
                {
                    model.EMAIL = row["EMAIL"].ToString();
                }
                if (row["USERGROUP"] != null)
                {
                    model.USERGROUP = row["USERGROUP"].ToString();
                }
                if (row["NAME"] != null)
                {
                    model.NAME = row["NAME"].ToString();
                }
                if (row["SENSITIVITYLEVEL"] != null && row["SENSITIVITYLEVEL"].ToString() != "")
                {
                    model.SENSITIVITYLEVEL = Decimal.Parse(row["SENSITIVITYLEVEL"].ToString());
                }
            }
            return model;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(int UserID, string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,UID,GID,DATABASEID,PASSWORD,VIEWLEVEL,REPORTGROUPLIST,USERGROUPLEVEL,SETUPUSER,REPORTRIGHT,AUTODATE,EMAIL,USERGROUP,NAME,SENSITIVITYLEVEL ");
            strSql.Append(" FROM [USER] ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperSQL.Query(UserID, strSql.ToString());
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int UserID, int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" ID,UID,GID,DATABASEID,PASSWORD,VIEWLEVEL,REPORTGROUPLIST,USERGROUPLEVEL,SETUPUSER,REPORTRIGHT,AUTODATE,EMAIL,USERGROUP,NAME,SENSITIVITYLEVEL ");
            strSql.Append(" FROM [USER] ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return DbHelperSQL.Query(UserID, strSql.ToString());
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(int UserID, string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM [USER] ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
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
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.ID desc");
            }
            strSql.Append(")AS Row, T.*  from [USER] T ");
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
            parameters[0].Value = "USER";
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
        public DataTable GetcustomList(int UserID, int databaseID)
        {
            StringBuilder strSql = new StringBuilder("SELECT");
            strSql.AppendLine(" 	u.ID, u.REPORTGROUPLIST, u.SETUPUSER, u.REPORTRIGHT, u.AUTODATE, u.EMAIL,");
            strSql.AppendLine(" 	u.NAME, u.SENSITIVITYLEVEL, u.[UID], u.GID, u.USERGROUP,");
            strSql.AppendLine(" 	ug.NAME AS USERGROUPNAME, ug.DATABASEID,");
            strSql.AppendLine(" 	db.NAME AS CNAME, sl.NAME AS SNAME");
            strSql.AppendLine(" FROM dbo.[USER] u");
            strSql.AppendLine(" INNER JOIN dbo.USERGROUP ug ON u.GID = ug.ID");
            strSql.AppendLine(" INNER JOIN dbo.[DATABASE] db ON ug.DATABASEID = db.ID");
            strSql.AppendLine(" INNER JOIN dbo.SensitivityLevel sl ON db.ID = sl.DATABASEID AND u.SENSITIVITYLEVEL = sl.SLEVEL");
            strSql.AppendLine(" WHERE ug.DATABASEID=@databaseid");
            SqlParameter[] parameters = {
                    new SqlParameter("@databaseid", SqlDbType.Int, 4),
            };
            parameters[0].Value = databaseID;

            DataSet ds = DbHelperSQL.Query(UserID, strSql.ToString(), parameters);

            //string sql = "SELECT * FROM V_USER WHERE DATABASEID=" + databaseid;

            if (ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }

            return null;
        }

        public CUSTOMRP.Model.USER GetModel(int UserID, string UID, int DATABASEID, string PASSWORD)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 ID,UID,GID,DATABASEID,PASSWORD,VIEWLEVEL,REPORTGROUPLIST,USERGROUPLEVEL,SETUPUSER,REPORTRIGHT,AUTODATE,EMAIL,USERGROUP,NAME,SENSITIVITYLEVEL from [USER] ");
            strSql.Append(" where UID=@UID and DATABASEID=@DATABASEID and PASSWORD=@PASSWORD ");
            SqlParameter[] parameters = {
                    new SqlParameter("@UID", SqlDbType.NVarChar,200),
                    new SqlParameter("@DATABASEID", SqlDbType.Int,4),
                    new SqlParameter("@PASSWORD", SqlDbType.NVarChar,50)			};
            parameters[0].Value = UID;
            parameters[1].Value = DATABASEID;
            parameters[2].Value = PASSWORD;

            CUSTOMRP.Model.USER model = new CUSTOMRP.Model.USER();
            DataSet ds = DbHelperSQL.Query(UserID, strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        public CUSTOMRP.Model.USER GetModel(int UserID, string UID, int DATABASEID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 ID,UID,GID,DATABASEID,PASSWORD,VIEWLEVEL,REPORTGROUPLIST,USERGROUPLEVEL,SETUPUSER,REPORTRIGHT,AUTODATE,EMAIL,USERGROUP,NAME,SENSITIVITYLEVEL from [USER] ");
            strSql.Append(" where UID=@UID and DATABASEID=@DATABASEID ");
            SqlParameter[] parameters = {
                    new SqlParameter("@UID", SqlDbType.NVarChar,200),
                    new SqlParameter("@DATABASEID", SqlDbType.Int,4)};
            parameters[0].Value = UID;
            parameters[1].Value = DATABASEID;

            CUSTOMRP.Model.USER model = new CUSTOMRP.Model.USER();
            DataSet ds = DbHelperSQL.Query(UserID, strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        #endregion  ExtensionMethod
    }
}