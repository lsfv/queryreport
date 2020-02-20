using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace CUSTOMRP.DAL
{
    public partial class WORDTEMPLATE
    {
        public WORDTEMPLATE()
        {}

        #region  BasicMethod

        public int Add(CUSTOMRP.Model.WORDTEMPLATE model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("INSERT INTO [WORDTEMPLATE](");
            strSql.Append("[WORDTEMPLATEName],[Description],[VIEWID],[TemplateFileName],[DataFileName],[ModifyDate],[ModifyUser],[CreateDate],[CreateUser]");
            strSql.Append(") VALUES (");
            strSql.Append("@WORDTEMPLATEName,@Description,@VIEWID,@TemplateFileName,@DataFileName,@ModifyDate,@ModifyUser,@CreateDate,@CreateUser");
            strSql.Append(");select @@IDENTITY");
            SqlParameter[] parameters = new SqlParameter[]
            {
                    new SqlParameter("@WORDTEMPLATEName", SqlDbType.NVarChar,50),
                    new SqlParameter("@Description", SqlDbType.NVarChar,50),
                    new SqlParameter("@VIEWID", SqlDbType.Int,4),
                    new SqlParameter("@TemplateFileName", SqlDbType.NVarChar,50),
                    new SqlParameter("@DataFileName", SqlDbType.NVarChar,200),
                    new SqlParameter("@ModifyDate", SqlDbType.DateTime),
                    new SqlParameter("@ModifyUser", SqlDbType.Int,4),
                    new SqlParameter("@CreateDate", SqlDbType.DateTime),
                    new SqlParameter("@CreateUser", SqlDbType.Int,4),
            };
            parameters[0].Value = model.WordTemplateName;
            parameters[1].Value = model.Description;
            parameters[2].Value = model.ViewID;
            parameters[3].Value = model.TemplateFileName;
            parameters[4].Value = model.DataFileName;
            parameters[5].Value = model.ModifyDate;
            parameters[6].Value = model.ModifyUser;
            parameters[7].Value = model.CreateDate;
            parameters[8].Value = model.CreateUser;

            object obj = DbHelperSQL.GetSingle(model.ModifyUser, strSql.ToString(), parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        public bool Update(CUSTOMRP.Model.WORDTEMPLATE model)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("UPDATE [WORDTEMPLATE] SET ");
            strSql.Append("[WORDTEMPLATEName] = @WORDTEMPLATEName,");
            strSql.Append("[Description] = @Description,");
            strSql.Append("[TemplateFileName] = @TemplateFileName,");
            strSql.Append("[DataFileName] = @DataFileName,");
            strSql.Append("[ModifyDate] = @ModifyDate,");
            strSql.Append("[ModifyUser] = @ModifyUser");
            strSql.Append(" WHERE [VIEWID] = @VIEWID");
            SqlParameter[] parameters = new SqlParameter[]
            {
                    new SqlParameter("@WORDTEMPLATEName", SqlDbType.NVarChar,50),
                    new SqlParameter("@Description", SqlDbType.NVarChar,50),
                    new SqlParameter("@TemplateFileName", SqlDbType.NVarChar,100),
                    new SqlParameter("@DataFileName", SqlDbType.NVarChar,100),
                    new SqlParameter("@ModifyDate", SqlDbType.DateTime),
                    new SqlParameter("@ModifyUser", SqlDbType.Int,4),
                    new SqlParameter("@VIEWID", SqlDbType.Int,4),
            };
            parameters[0].Value = model.WordTemplateName;
            parameters[1].Value = model.Description;
            parameters[2].Value = model.TemplateFileName;
            parameters[3].Value = model.DataFileName;
            parameters[4].Value = model.ModifyDate;
            parameters[5].Value = model.ModifyUser;
            parameters[6].Value = model.ViewID;

            int rows=DbHelperSQL.ExecuteSql(model.ModifyUser, strSql.ToString(),parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Delete(int UserID, int ID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("DELETE FROM [WORDTEMPLATE] WHERE WORDTEMPLATEID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)
            };
            parameters[0].Value = ID;

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
        /// Get POCO object for WordTemplate
        /// </summary>
        /// <param name="UserID">ID of Current User</param>
        /// <param name="ID">WordTemplate ID</param>
        /// <param name="perspectiveUserID">UserID, set it as -1 to ignore</param>
        /// <returns></returns>
        public CUSTOMRP.Model.WORDTEMPLATE GetModel(int UserID, int ID, int perspectiveUserID)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("SELECT\r\n");
            strSql.Append(" wt.WORDTEMPLATEID, wt.WORDTEMPLATEName, wt.[Description], wt.VIEWID,\r\n");
            strSql.Append(" wt.TemplateFileName, wt.DataFileName,\r\n");
            strSql.Append(" wt.ModifyDate, wt.ModifyUser, wt.CreateDate, wt.CreateUser,\r\n");
            strSql.Append(" [FileCount] = ISNULL(wf.FileCount, 0),\r\n");
            strSql.Append(" sv.SOURCEVIEWNAME, sv.DATABASEID, [SVDESC] = sv.[DESC], sv.SOURCETYPE,\r\n");
            strSql.Append(" sl.NAME VIEWLEVEL\r\n");
            strSql.Append("FROM dbo.WORDTEMPLATE wt\r\n");
            strSql.Append("INNER JOIN dbo.SOURCEVIEW sv ON wt.VIEWID = sv.ID\r\n");
            strSql.Append("INNER JOIN dbo.SENSITIVITYLEVEL sl ON sv.VIEWLEVEL = sl.SLEVEL AND sv.DATABASEID = sl.DATABASEID\r\n");
            strSql.Append("LEFT JOIN (\r\n");
            strSql.Append(" SELECT WordTemplateID, FileCount = COUNT(*) FROM dbo.WORDFILE GROUP BY WordTemplateID\r\n");
            strSql.Append(") wf ON wt.WordTemplateID = wf.WordTemplateID\r\n");
            strSql.Append("LEFT JOIN (\r\n");
            strSql.Append(" SELECT ID, DATABASEID, SENSITIVITYLEVEL FROM [USER] WHERE ID = @UserID\r\n");
            strSql.Append(") u ON u.DATABASEID = sl.DATABASEID AND u.SENSITIVITYLEVEL <= sl.SLEVEL\r\n");
            strSql.Append("WHERE wt.WordTemplateID=@ID");
            strSql.Append(" AND ((@UserID = -1) OR (u.ID IS NOT NULL))");

            SqlParameter[] parameters = new SqlParameter[]
            {
                    new SqlParameter("@ID", SqlDbType.Int,4),
                    new SqlParameter("@UserID", SqlDbType.Int,4),
            };
            parameters[0].Value = ID;
            parameters[1].Value = perspectiveUserID;

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
        /// Get POCO object for WordTemplate
        /// </summary>
        /// <param name="UserID">ID of Current User</param>
        /// <param name="ID">Report ID</param>
        /// <param name="perspectiveUserID">UserID, set it as -1 to ignore</param>
        /// <returns></returns>
        public CUSTOMRP.Model.WORDTEMPLATE GetModelByReportID(int UserID, int rpID, int perspectiveUserID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT\r\n");
            strSql.Append(" wt.WORDTEMPLATEID, wt.WORDTEMPLATEName, wt.[Description], wt.VIEWID,\r\n");
            strSql.Append(" wt.TemplateFileName, wt.DataFileName,\r\n");
            strSql.Append(" wt.ModifyDate, wt.ModifyUser, wt.CreateDate, wt.CreateUser,\r\n");
            strSql.Append(" [FileCount] = ISNULL(wf.FileCount, 0),\r\n");
            strSql.Append(" sv.SOURCEVIEWNAME, sv.DATABASEID, [SVDESC] = sv.[DESC], sv.SOURCETYPE,\r\n");
            strSql.Append(" sl.NAME VIEWLEVEL\r\n");
            strSql.Append("FROM dbo.WORDTEMPLATE wt\r\n");
            strSql.Append("INNER JOIN dbo.SOURCEVIEW sv ON wt.VIEWID = sv.ID\r\n");
            strSql.Append("INNER JOIN dbo.SENSITIVITYLEVEL sl ON sv.VIEWLEVEL = sl.SLEVEL AND sv.DATABASEID = sl.DATABASEID\r\n");
            strSql.Append("LEFT JOIN (\r\n");
            strSql.Append(" SELECT WordTemplateID, FileCount = COUNT(*) FROM dbo.WORDFILE GROUP BY WordTemplateID\r\n");
            strSql.Append(") wf ON wt.WordTemplateID = wf.WordTemplateID\r\n");
            strSql.Append("LEFT JOIN (\r\n");
            strSql.Append(" SELECT ID, DATABASEID, SENSITIVITYLEVEL FROM [USER] WHERE ID = @UserID\r\n");
            strSql.Append(") u ON u.DATABASEID = sl.DATABASEID AND u.SENSITIVITYLEVEL <= sl.SLEVEL\r\n");
            strSql.Append("INNER JOIN REPORT r ON r.SVID = wt.VIEWID\r\n");
            strSql.Append("WHERE r.ID = @ID");
            strSql.Append(" AND ((@UserID = -1) OR (u.ID IS NOT NULL))");

            SqlParameter[] parameters = new SqlParameter[]
            {
                    new SqlParameter("@ID", SqlDbType.Int,4),
                    new SqlParameter("@UserID", SqlDbType.Int,4),
            };
            parameters[0].Value = rpID;
            parameters[1].Value = perspectiveUserID;

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

        /// <summary>
        /// Get POCO object for WordTemplate
        /// </summary>
        /// <param name="UserID">ID of Current User</param>
        /// <param name="svID">SourceView ID</param>
        /// <param name="perspectiveUserID">UserID, set it as -1 to ignore</param>
        /// <returns></returns>
        public CUSTOMRP.Model.WORDTEMPLATE GetModelBySVID(int UserID, int svID, int perspectiveUserID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT\r\n");
            strSql.Append(" wt.WORDTEMPLATEID, wt.WORDTEMPLATEName, wt.[Description], wt.VIEWID,\r\n");
            strSql.Append(" wt.TemplateFileName, wt.DataFileName,\r\n");
            strSql.Append(" wt.ModifyDate, wt.ModifyUser, wt.CreateDate, wt.CreateUser,\r\n");
            strSql.Append(" [FileCount] = ISNULL(wf.FileCount, 0),\r\n");
            strSql.Append(" sv.SOURCEVIEWNAME, sv.DATABASEID, [SVDESC] = sv.[DESC], sv.SOURCETYPE,\r\n");
            strSql.Append(" sl.NAME VIEWLEVEL\r\n");
            strSql.Append("FROM dbo.WORDTEMPLATE wt\r\n");
            strSql.Append("INNER JOIN dbo.SOURCEVIEW sv ON wt.VIEWID = sv.ID\r\n");
            strSql.Append("INNER JOIN dbo.SENSITIVITYLEVEL sl ON sv.VIEWLEVEL = sl.SLEVEL AND sv.DATABASEID = sl.DATABASEID\r\n");
            strSql.Append("LEFT JOIN (\r\n");
            strSql.Append(" SELECT WordTemplateID, FileCount = COUNT(*) FROM dbo.WORDFILE GROUP BY WordTemplateID\r\n");
            strSql.Append(") wf ON wt.WordTemplateID = wf.WordTemplateID\r\n");
            strSql.Append("LEFT JOIN (\r\n");
            strSql.Append(" SELECT ID, DATABASEID, SENSITIVITYLEVEL FROM [USER] WHERE ID = @UserID\r\n");
            strSql.Append(") u ON u.DATABASEID = sl.DATABASEID AND u.SENSITIVITYLEVEL <= sl.SLEVEL\r\n");
            strSql.Append("WHERE wt.VIEWID = @ID");
            strSql.Append(" AND ((@UserID = -1) OR (u.ID IS NOT NULL))");

            SqlParameter[] parameters = new SqlParameter[]
            {
                    new SqlParameter("@ID", SqlDbType.Int,4),
                    new SqlParameter("@UserID", SqlDbType.Int,4),
            };
            parameters[0].Value = svID;
            parameters[1].Value = perspectiveUserID;

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

        /// <summary>
        /// Get POCO object list for WordTemplate
        /// </summary>
        /// <param name="UserID">ID of Current User</param>
        /// <param name="databaseID">Database ID</param>
        /// <param name="sourceType">0 = View, 1 = Table, 2 = Stored Proc, -1 to ignore</param>
        /// <param name="perspectiveUserID">UserID, set it as -1 to ignore</param>
        /// <returns></returns>
        public List<CUSTOMRP.Model.WORDTEMPLATE> GetTemplateList(int UserID, int databaseID, int sourceType, int perspectiveUserID)
        {
            List<CUSTOMRP.Model.WORDTEMPLATE> result = new List<Model.WORDTEMPLATE>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT\r\n");
            strSql.Append(" wt.WORDTEMPLATEID, wt.WORDTEMPLATEName, wt.[Description], wt.VIEWID,\r\n");
            strSql.Append(" wt.TemplateFileName, wt.DataFileName,\r\n");
            strSql.Append(" wt.ModifyDate, wt.ModifyUser, wt.CreateDate, wt.CreateUser,\r\n");
            strSql.Append(" [FileCount] = ISNULL(wf.FileCount, 0),\r\n");
            strSql.Append(" sv.SOURCEVIEWNAME, sv.DATABASEID, [SVDESC] = sv.[DESC], sv.SOURCETYPE,\r\n");
            strSql.Append(" sl.NAME VIEWLEVEL\r\n");
            strSql.Append("FROM dbo.WORDTEMPLATE wt\r\n");
            strSql.Append("INNER JOIN dbo.SOURCEVIEW sv ON wt.VIEWID = sv.ID\r\n");
            strSql.Append("INNER JOIN dbo.SENSITIVITYLEVEL sl ON sv.VIEWLEVEL = sl.SLEVEL AND sv.DATABASEID = sl.DATABASEID\r\n");
            strSql.Append("LEFT JOIN (\r\n");
            strSql.Append(" SELECT WordTemplateID, FileCount = COUNT(*) FROM dbo.WORDFILE GROUP BY WordTemplateID\r\n");
            strSql.Append(") wf ON wt.WordTemplateID = wf.WordTemplateID\r\n");
            strSql.Append("LEFT JOIN (\r\n");
            strSql.Append(" SELECT ID, DATABASEID, SENSITIVITYLEVEL FROM [USER] WHERE ID = @UserID\r\n");
            strSql.Append(") u ON u.DATABASEID = sl.DATABASEID AND u.SENSITIVITYLEVEL <= sl.SLEVEL\r\n");
            strSql.Append("WHERE sv.DATABASEID=@ID AND (@sourceType = -1 OR sv.SOURCETYPE = @sourceType)");
            strSql.Append(" AND ((@UserID = -1) OR (u.ID IS NOT NULL))");

            SqlParameter[] parameters = new SqlParameter[]
            {
                    new SqlParameter("@ID", SqlDbType.Int,4),
                    new SqlParameter("@sourceType", SqlDbType.Int,4),
                    new SqlParameter("@UserID", SqlDbType.Int,4),
            };
            parameters[0].Value = databaseID;
            parameters[1].Value = sourceType;
            parameters[2].Value = perspectiveUserID;

            DataSet ds = DbHelperSQL.Query(UserID, strSql.ToString(), parameters);

            foreach (DataRow dr in DbHelperSQL.Query(UserID, strSql.ToString(), parameters).Tables[0].Rows)
            {
                result.Add(DataRowToModel(dr));
            }
            return result;
        }

        /// <summary>
        /// Convert DataRow to POCO object of WordTemplate
        /// </summary>
        /// <param name="row">DataRow to be converted</param>
        /// <returns></returns>
        private CUSTOMRP.Model.WORDTEMPLATE DataRowToModel(DataRow row)
        {
            CUSTOMRP.Model.WORDTEMPLATE model = new CUSTOMRP.Model.WORDTEMPLATE();
            if (row != null)
            {
                if (row["WORDTEMPLATEID"] != null)
                {
                    model.WordTemplateID = Convert.ToInt32(row["WORDTEMPLATEID"]);
                }
                if (row["WORDTEMPLATEName"] != null)
                {
                    model.WordTemplateName = Convert.ToString(row["WORDTEMPLATEName"]);
                }
                if (row["Description"] != null)
                {
                    model.Description = Convert.ToString(row["Description"]);
                }
                if (row["VIEWID"] != null)
                {
                    model.ViewID = Convert.ToInt32(row["VIEWID"]);
                }
                if (row["TemplateFileName"] != null)
                {
                    model.TemplateFileName = Convert.ToString(row["TemplateFileName"]);
                }
                if (row["DataFileName"] != null)
                {
                    model.DataFileName = Convert.ToString(row["DataFileName"]);
                }
                if (row["ModifyDate"] != null)
                {
                    model.ModifyDate = Convert.ToDateTime(row["ModifyDate"]);
                }
                if (row["ModifyUser"] != null)
                {
                    model.ModifyUser = Convert.ToInt32(row["ModifyUser"]);
                }
                if (row["CreateDate"] != null)
                {
                    model.CreateDate = Convert.ToDateTime(row["CreateDate"]);
                }
                if (row["CreateUser"] != null)
                {
                    model.CreateUser = Convert.ToInt32(row["CreateUser"]);
                }
                if (row["FileCount"] != null)
                {
                    model.FileCount = Convert.ToInt32(row["FileCount"]);
                }
                if (row["SOURCEVIEWNAME"] != null)
                {
                    model.SOURCEVIEWNAME = Convert.ToString(row["SOURCEVIEWNAME"]);
                }
                if (row["DATABASEID"] != null)
                {
                    model.DATABASEID = Convert.ToInt32(row["DATABASEID"]);
                }
                if (row["SVDESC"] != null)
                {
                    model.SVDESC = Convert.ToString(row["SVDESC"]);
                }
                if (row["SOURCETYPE"] != null)
                {
                    model.SOURCETYPE = Convert.ToInt32(row["SOURCETYPE"]);
                }
                if (row["VIEWLEVEL"] != null)
                {
                    model.VIEWLEVEL = Convert.ToString(row["VIEWLEVEL"]);
                }
            }
            return model;
        }

        //public DataSet GetList(string strWhere)
        //{
        //    StringBuilder strSql=new StringBuilder();
        //    strSql.Append("select a.*,b.WORDTEMPLATEName,b.VIEWID from WORDFILE a");
        //    strSql.Append("inner join WORDTEMPLATE b on a.WordTemplateID = b.WORDTEMPLATEID");
        //    if(strWhere.Trim()!="")
        //    {
        //        strSql.Append(" where "+strWhere);
        //    }
        //    return DbHelperSQL.Query(strSql.ToString());
        //}

        //public DataSet GetList(int Top,string strWhere,string filedOrder)
        //{
        //    StringBuilder strSql=new StringBuilder();
        //    strSql.Append("select ");
        //    if(Top>0)
        //    {
        //        strSql.Append(" top "+Top.ToString());
        //    }
        //    strSql.Append(" ID,UID,DATABASEID,PASSWORD,VIEWLEVEL,REPORTGROUPLIST,USERGROUPLEVEL,SETUPUSER,REPORTRIGHT,AUTODATE,EMAIL,USERGROUP,NAME,SENSITIVITYLEVEL ");
        //    strSql.Append(" FROM [USER] ");
        //    if(strWhere.Trim()!="")
        //    {
        //        strSql.Append(" where "+strWhere);
        //    }
        //    strSql.Append(" order by " + filedOrder);
        //    return DbHelperSQL.Query(strSql.ToString());
        //}

        //public int GetRecordCount(string strWhere)
        //{
        //    StringBuilder strSql=new StringBuilder();
        //    strSql.Append("select count(1) FROM [USER] ");
        //    if(strWhere.Trim()!="")
        //    {
        //        strSql.Append(" where "+strWhere);
        //    }
        //    object obj = DbHelperSQL.GetSingle(strSql.ToString());
        //    if (obj == null)
        //    {
        //        return 0;
        //    }
        //    else
        //    {
        //        return Convert.ToInt32(obj);
        //    }
        //}

        //public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        //{
        //    StringBuilder strSql=new StringBuilder();
        //    strSql.Append("SELECT * FROM ( ");
        //    strSql.Append(" SELECT ROW_NUMBER() OVER (");
        //    if (!string.IsNullOrEmpty(orderby.Trim()))
        //    {
        //        strSql.Append("order by T." + orderby );
        //    }
        //    else
        //    {
        //        strSql.Append("order by T.ID desc");
        //    }
        //    strSql.Append(")AS Row, T.*  from [USER] T ");
        //    if (!string.IsNullOrEmpty(strWhere.Trim()))
        //    {
        //        strSql.Append(" WHERE " + strWhere);
        //    }
        //    strSql.Append(" ) TT");
        //    strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
        //    return DbHelperSQL.Query(strSql.ToString());
        //}

        #endregion  BasicMethod

        #region  ExtensionMethod

        //public DataTable GetWordFileListByID(int WordFileID)
        //{
        //    DataTable result = null;
        //    string sql = "SELECT * FROM V_WORDFILE WHERE WordFileID=@WordFileID";
        //    result = DbHelperSQL.Query(sql, new SqlParameter("@WordFileID", WordFileID)).Tables[0];
        //    return result;
        //}

        //public DataTable GetWordTemplateList(int databaseID, int sourceType)
        //{
        //    DataTable result = null;
        //    string sql = "SELECT * FROM V_WORDTEMPLATE WHERE DATABASEID=@databaseid AND (@sourceType = -1 OR SOURCETYPE = @sourceType)";
        //    result = DbHelperSQL.Query(sql, new SqlParameter("@databaseid", databaseID), new SqlParameter("@sourceType", sourceType)).Tables[0];
        //    return result;
        //}

        //public DataTable GetWordTemplateListByTemplateID(int WORDTEMPLATEID)
        //{
        //    DataTable result = null;
        //    string sql = "SELECT * FROM V_WORDTEMPLATE WHERE WORDTEMPLATEID=@WORDTEMPLATEID";
        //    result = DbHelperSQL.Query(sql, new SqlParameter("@WORDTEMPLATEID", WORDTEMPLATEID)).Tables[0];
        //    return result;
        //}

        /*
        public string CreateLocalViewByViewAndDBName(string ViewName, string DBName, string UserID, string sql)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[" + ViewName + DBName + UserID + "]') ");
            object obj = DbHelperSQL.GetSingle(strSql.ToString());
            if (obj != null && Convert.ToInt32(obj) >0)
            {
                string sqlDropView = "DROP VIEW [dbo].[" + ViewName + DBName + UserID + "] " ;
                DbHelperSQL.ExecuteSql(sqlDropView);
            }

            string sqlCreateView = "";
            sqlCreateView = "CREATE VIEW [dbo].[" + ViewName + DBName + UserID + "] as  ";
            sqlCreateView = sqlCreateView+ sql;
            int rows = DbHelperSQL.ExecuteSql(sqlCreateView.ToString());
            return ViewName + DBName + UserID;
        }
         * */

        //public DataSet GetWordReportInfoByID(int rpid)
        //{
        //    string strSql = "select * FROM V_WORDREPORT where ID=@rpid";

        //    return DbHelperSQL.Query(strSql.ToString(), new SqlParameter("@rpid", rpid));
        //}

        #endregion  ExtensionMethod
    }
}
