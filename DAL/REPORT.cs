using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CUSTOMRP.DAL
{
    /// <summary>
    /// 数据访问类:REPORT
    /// </summary>
    public partial class REPORT
    {
        public REPORT()
        {}
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(CUSTOMRP.Model.REPORT model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into REPORT(");
            strSql.Append("DATABASEID,SVID,REPORTNAME,AUDODATE,CATEGORY,TYPE,REPORTGROUPLIST,RPTITLE,ADDUSER,DEFAULTFORMAT,EXTENDFIELD,PRINT_ORIENTATION,PRINT_FITTOPAGE,REPORT_HEADER,REPORT_FOOTER,SUBCOUNT_LABEL,PDF_GRID_LINES,FONT_FAMILY)");
            strSql.Append(" values (");
            strSql.Append("@DATABASEID,@SVID,@REPORTNAME,@AUDODATE,@CATEGORY,@TYPE,@REPORTGROUPLIST,@RPTITLE,@ADDUSER,@DEFAULTFORMAT,@EXTENDFIELD,@PRINT_ORIENTATION,@PRINT_FITTOPAGE,@REPORT_HEADER,@REPORT_FOOTER,@SUBCOUNT_LABEL,@PDF_GRID_LINES,@FONT_FAMILY)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                new SqlParameter("@DATABASEID", SqlDbType.Int,4),
                new SqlParameter("@SVID", SqlDbType.Int,4),
                new SqlParameter("@REPORTNAME", SqlDbType.NVarChar,50),
                new SqlParameter("@AUDODATE", SqlDbType.DateTime),
                new SqlParameter("@CATEGORY", SqlDbType.Int,4),
                new SqlParameter("@TYPE", SqlDbType.Int,4),
                new SqlParameter("@REPORTGROUPLIST", SqlDbType.Int,4),
                new SqlParameter("@RPTITLE", SqlDbType.NVarChar,50),
                new SqlParameter("@ADDUSER", SqlDbType.Int,4),
                new SqlParameter("@DEFAULTFORMAT", SqlDbType.Int,4),
                new SqlParameter("@EXTENDFIELD", SqlDbType.NVarChar,2000),
                new SqlParameter("@PRINT_ORIENTATION", SqlDbType.Int,4),
                new SqlParameter("@PRINT_FITTOPAGE", SqlDbType.SmallInt,2),
                new SqlParameter("@REPORT_HEADER", SqlDbType.NVarChar,255),
                new SqlParameter("@REPORT_FOOTER", SqlDbType.NVarChar,255),
                new SqlParameter("@SUBCOUNT_LABEL", SqlDbType.NVarChar,15),
                new SqlParameter("@PDF_GRID_LINES", SqlDbType.Bit),
                new SqlParameter("@FONT_FAMILY", SqlDbType.NVarChar,100),
            };
            parameters[0].Value = model.DATABASEID;
            parameters[1].Value = model.SVID;
            parameters[2].Value = model.REPORTNAME;
            parameters[3].Value = model.AUDODATE;
            parameters[4].Value = model.CATEGORY;
            parameters[5].Value = model.TYPE;
            parameters[6].Value = model.REPORTGROUPLIST;
            parameters[7].Value = model.RPTITLE;
            parameters[8].Value = model.ADDUSER;
            parameters[9].Value = model.DEFAULTFORMAT;
            parameters[10].Value = model.EXTENDFIELD;
            parameters[11].Value = model.PRINT_ORIENTATION;
            parameters[12].Value = model.PRINT_FITTOPAGE;
            parameters[13].Value = model.REPORT_HEADER;
            parameters[14].Value = model.REPORT_FOOTER;
            parameters[15].Value = model.SUBCOUNT_LABEL;
            parameters[16].Value = model.PDF_GRID_LINES;
            parameters[17].Value = model.FONT_FAMILY;

            object obj = DbHelperSQL.GetSingle(model.ADDUSER, strSql.ToString(), parameters);
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
        /// 更新一条数据
        /// </summary>
        public bool Update(CUSTOMRP.Model.REPORT model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update REPORT set ");
            strSql.Append("DATABASEID=@DATABASEID,");
            strSql.Append("SVID=@SVID,");
            strSql.Append("REPORTNAME=@REPORTNAME,");
            strSql.Append("AUDODATE=@AUDODATE,");
            strSql.Append("CATEGORY=@CATEGORY,");
            strSql.Append("TYPE=@TYPE,");
            strSql.Append("REPORTGROUPLIST=@REPORTGROUPLIST,");
            strSql.Append("RPTITLE=@RPTITLE,");
            strSql.Append("ADDUSER=@ADDUSER,");
            strSql.Append("DEFAULTFORMAT=@DEFAULTFORMAT,");
            strSql.Append("EXTENDFIELD=@EXTENDFIELD,");
            strSql.Append("PRINT_ORIENTATION=@PRINT_ORIENTATION,");
            strSql.Append("PRINT_FITTOPAGE=@PRINT_FITTOPAGE,");
            strSql.Append("REPORT_HEADER=@REPORT_HEADER,");
            strSql.Append("REPORT_FOOTER=@REPORT_FOOTER,");
            strSql.Append("SUBCOUNT_LABEL=@SUBCOUNT_LABEL,");
            strSql.Append("PDF_GRID_LINES=@PDF_GRID_LINES,");
            strSql.Append("FONT_FAMILY=@FONT_FAMILY");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@DATABASEID", SqlDbType.Int,4),
                    new SqlParameter("@SVID", SqlDbType.Int,4),
                    new SqlParameter("@REPORTNAME", SqlDbType.NVarChar,50),
                    new SqlParameter("@AUDODATE", SqlDbType.DateTime),
                    new SqlParameter("@CATEGORY", SqlDbType.Int,4),
                    new SqlParameter("@TYPE", SqlDbType.Int,4),
                    new SqlParameter("@REPORTGROUPLIST", SqlDbType.Int,4),
                    new SqlParameter("@RPTITLE", SqlDbType.NVarChar,50),
                    new SqlParameter("@ADDUSER", SqlDbType.Int,4),
                    new SqlParameter("@DEFAULTFORMAT", SqlDbType.Int,4),
                    new SqlParameter("@EXTENDFIELD", SqlDbType.NVarChar,2000),
                    new SqlParameter("@PRINT_ORIENTATION", SqlDbType.Int,4),
                    new SqlParameter("@PRINT_FITTOPAGE", SqlDbType.SmallInt, 2),
                    new SqlParameter("@REPORT_HEADER", SqlDbType.NVarChar,255),
                    new SqlParameter("@REPORT_FOOTER", SqlDbType.NVarChar,255),
                    new SqlParameter("@SUBCOUNT_LABEL", SqlDbType.NVarChar,15),
                    new SqlParameter("@PDF_GRID_LINES", SqlDbType.Bit),
                    new SqlParameter("@FONT_FAMILY", SqlDbType.NVarChar,100),
                    new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = model.DATABASEID;
            parameters[1].Value = model.SVID;
            parameters[2].Value = model.REPORTNAME;
            parameters[3].Value = model.AUDODATE;
            parameters[4].Value = model.CATEGORY;
            parameters[5].Value = model.TYPE;
            parameters[6].Value = model.REPORTGROUPLIST;
            parameters[7].Value = model.RPTITLE;
            parameters[8].Value = model.ADDUSER;
            parameters[9].Value = model.DEFAULTFORMAT;
            parameters[10].Value = model.EXTENDFIELD;
            parameters[11].Value = model.PRINT_ORIENTATION;
            parameters[12].Value = model.PRINT_FITTOPAGE;
            parameters[13].Value = model.REPORT_HEADER;
            parameters[14].Value = model.REPORT_FOOTER;
            parameters[15].Value = model.SUBCOUNT_LABEL;
            parameters[16].Value = model.PDF_GRID_LINES;
            parameters[17].Value = model.FONT_FAMILY;
            parameters[18].Value = model.ID;

            int rows = DbHelperSQL.ExecuteSql(model.ADDUSER, strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Replace(CUSTOMRP.Model.REPORT model)
        {
            bool result = false;
            bool isNew = false;
            CUSTOMRP.Model.REPORT original = null;
            if (model.ID == 0)
            {
                model.ID = Add(model);
                result = model.ID != 0;
                isNew = true;
            }
            else
            {
                original = GetModel(model.ADDUSER, model.ID);

                result = Update(model);
            }
            if (!result) { return false; }  // error occurred

            // prepare AuditLog object
            Model.AUDITLOG auditobj = model.GetAuditLogObject(original);
            auditobj.UserID = model.ADDUSER;
            auditobj.CreateDate = DateTime.Now;

            #region Report Columns handling
 
            try
            {
                CUSTOMRP.DAL.REPORTCOLUMN dalRC = new REPORTCOLUMN(); 

                //v1.0.0 - Cheong - 2015/06/30 - delete removed columns
                if (!isNew)
                {
                    Model.REPORTCOLUMN[] dbRC = (from oldrc in dalRC.GetModelListForReport(model.ADDUSER, model.ID)
                                                 join newrc in model.ReportColumns on new { oldrc.COLUMNNAME, oldrc.COLUMNFUNC, oldrc.COLUMNTYPE } equals new { newrc.COLUMNNAME, newrc.COLUMNFUNC, newrc.COLUMNTYPE } into g 
                                                 from gjoin in g.DefaultIfEmpty()
                                                 where gjoin == null
                                                 select oldrc).ToArray();

                    foreach (Model.REPORTCOLUMN rc in dbRC)
                    {
                        dalRC.Delete(model.ADDUSER, rc.ID);
                    }
                }

                //v1.0.0 - Cheong - 2016/03/23 - Preserve order on criteria and group columns
                int contentSEQ = 1;
                int criteriaSEQ = 1;
                int sortonSEQ = 1;
                int groupSEQ = 1;
                foreach (CUSTOMRP.Model.REPORTCOLUMN rc in model.ReportColumns)
                {
                    rc.RPID = model.ID;
                    switch (rc.COLUMNFUNC)
                    {
                        case 1:
                            {
                                rc.SEQ = contentSEQ;
                                contentSEQ++;
                            }
                            break;
                        case 2:
                            {
                                rc.SEQ = criteriaSEQ;
                                criteriaSEQ++;
                            }
                            break;
                        case 3:
                            {
                                rc.SEQ = sortonSEQ;
                                sortonSEQ++;
                            }
                            break;
                        case 6:
                            {
                                rc.SEQ = groupSEQ;
                                groupSEQ++;
                            }
                            break;
                        default:
                            {
                                rc.SEQ = -1;
                            }
                            break;
                    }
                    //if (rc.COLUMNFUNC == 1)
                    //{
                    //    rc.SEQ = contentSEQ;
                    //    contentSEQ++;
                    //}
                    //else if (rc.COLUMNFUNC == 3)
                    //{
                    //    rc.SEQ = sortonSEQ;
                    //    sortonSEQ++;
                    //}
                    //else
                    //{
                    //    rc.SEQ = -1;
                    //}
                    result = dalRC.Replace(model.ADDUSER, rc);
                    if (!result) { return false; }
                }
            }
            catch
            {
                result = false;
            }

            #endregion Report Columns handling

            #region WordFile handling
            try
            {
                if (model.WordFile != null)
                {
                    DAL.WORDFILE dalWF = new WORDFILE();
                    Model.WORDFILE wf = dalWF.GetModelByReportID(model.ADDUSER, model.ID);
                    if (wf == null)
                    {
                        if (model.WordFile.RPID == 0)
                        {
                            model.WordFile.RPID = model.ID;
                        }
                        dalWF.AddFile(model.WordFile);
                    }
                    else
                    {
                        wf.Description = model.WordFile.Description;
                        wf.WordFileName = model.WordFile.WordFileName;
                        wf.OrigFileName = model.WordFile.OrigFileName;
                        wf.ModifyUser = model.WordFile.ModifyUser;
                        wf.ModifyDate = model.WordFile.ModifyDate;
                        dalWF.UpdateFile(wf);
                    }
                }
            }
            catch
            {
                result = false;
            }

            #endregion WordFile handling

            auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
            auditobj.ModuleName = "DAL.REPORT.REPLACE";
            auditobj.Message = String.Format(isNew ? AppNum.AuditMessage.ReportInsertSuccess : AppNum.AuditMessage.ReportUpdateSuccess, model.ID);

            AUDITLOG.Add(auditobj);

            return result;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int UserID, int ID)
        {
            Model.REPORT original = GetModel(UserID, ID);

            #region Clear columns from ReportColumn table first
            REPORTCOLUMN dalRC = new REPORTCOLUMN();
            List<Model.REPORTCOLUMN> rclist = dalRC.GetModelListForReport(UserID, ID);
            foreach (Model.REPORTCOLUMN rc in rclist)
            {
                dalRC.Delete(UserID, rc.ID);
            }
            #endregion

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from REPORT ");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)
            };
            parameters[0].Value = ID;

            int rows = DbHelperSQL.ExecuteSql(UserID, strSql.ToString(), parameters);
            if (rows > 0)
            {
                Model.AUDITLOG auditobj = original.GetAuditLogObject(null);
                auditobj.UserID = UserID;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.REPORT.Delete";
                auditobj.Message = String.Format(AppNum.AuditMessage.ReportDeleteSuccess, ID);

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
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from REPORT ");
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
        public CUSTOMRP.Model.REPORT GetModel(int UserID, int ID)
        {
            CUSTOMRP.Model.REPORT result = null;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 ID,DATABASEID,SVID,REPORTNAME,AUDODATE,CATEGORY,TYPE,REPORTGROUPLIST,RPTITLE,ADDUSER,DEFAULTFORMAT,EXTENDFIELD,PRINT_ORIENTATION,PRINT_FITTOPAGE,REPORT_HEADER,REPORT_FOOTER,SUBCOUNT_LABEL,PDF_GRID_LINES,FONT_FAMILY from REPORT ");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)
            };
            parameters[0].Value = ID;

            CUSTOMRP.Model.REPORT model = new CUSTOMRP.Model.REPORT();
            DataSet ds = DbHelperSQL.Query(UserID, strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                result = DataRowToModel(ds.Tables[0].Rows[0]);

                REPORTCOLUMN bllRptCol = new REPORTCOLUMN();
                result.ReportColumns = bllRptCol.GetModelListForReport(UserID, ID);

                WORDFILE dalWF = new DAL.WORDFILE();
                result.WordFile = dalWF.GetModelByReportID(UserID, ID);
            }
            return result;
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public CUSTOMRP.Model.REPORT DataRowToModel(DataRow row)
        {
            CUSTOMRP.Model.REPORT model = new CUSTOMRP.Model.REPORT();
            if (row != null)
            {
                if (row["ID"] != null && row["ID"].ToString() != "")
                {
                    model.ID = Int32.Parse(row["ID"].ToString());
                }
                if (row["DATABASEID"] != null && row["DATABASEID"].ToString() != "")
                {
                    model.DATABASEID = Int32.Parse(row["DATABASEID"].ToString());
                }
                if (row["SVID"] != null && row["SVID"].ToString() != "")
                {
                    model.SVID = Int32.Parse(row["SVID"].ToString());
                }
                if (row["REPORTNAME"] != null)
                {
                    model.REPORTNAME = row["REPORTNAME"].ToString();
                }
                if (row["AUDODATE"] != null && row["AUDODATE"].ToString() != "")
                {
                    model.AUDODATE = DateTime.Parse(row["AUDODATE"].ToString());
                }
                if (row["CATEGORY"] != null && row["CATEGORY"].ToString() != "")
                {
                    model.CATEGORY = Int32.Parse(row["CATEGORY"].ToString());
                }
                if (row["TYPE"] != null && row["TYPE"].ToString() != "")
                {
                    model.TYPE = Int32.Parse(row["TYPE"].ToString());
                }
                if (row["REPORTGROUPLIST"] != null && row["REPORTGROUPLIST"].ToString() != "")
                {
                    model.REPORTGROUPLIST = Int32.Parse(row["REPORTGROUPLIST"].ToString());
                }
                if (row["RPTITLE"] != null)
                {
                    model.RPTITLE = row["RPTITLE"].ToString();
                }
                if (row["ADDUSER"] != null && row["ADDUSER"].ToString() != "")
                {
                    model.ADDUSER = Int32.Parse(row["ADDUSER"].ToString());
                }
                if (row["DEFAULTFORMAT"] != null && row["DEFAULTFORMAT"].ToString() != "")
                {
                    model.DEFAULTFORMAT = Int32.Parse(row["DEFAULTFORMAT"].ToString());
                }
                if (row["EXTENDFIELD"] != null)
                {
                    model.EXTENDFIELD = row["EXTENDFIELD"].ToString();
                }
                if (row["PRINT_ORIENTATION"] != null)
                {
                    model.PRINT_ORIENTATION = Convert.ToInt32(row["PRINT_ORIENTATION"]);
                }
                if (row["PRINT_FITTOPAGE"] != null)
                {
                    model.PRINT_FITTOPAGE = Convert.ToInt16(row["PRINT_FITTOPAGE"]);
                }
                if (row["REPORT_HEADER"] != null)
                {
                    model.REPORT_HEADER = row["REPORT_HEADER"].ToString();
                }
                if (row["REPORT_FOOTER"] != null)
                {
                    model.REPORT_FOOTER = row["REPORT_FOOTER"].ToString();
                }
                if (row["SUBCOUNT_LABEL"] != null)
                {
                    model.SUBCOUNT_LABEL = row["SUBCOUNT_LABEL"].ToString();
                }
                if (row["PDF_GRID_LINES"] != null)
                {
                    model.PDF_GRID_LINES = Convert.ToBoolean(row["PDF_GRID_LINES"]);
                }
                if (row["FONT_FAMILY"] != null)
                {
                    model.FONT_FAMILY = row["FONT_FAMILY"].ToString();
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
            strSql.Append("select ID,DATABASEID,SVID,REPORTNAME,AUDODATE,CATEGORY,TYPE,REPORTGROUPLIST,RPTITLE,ADDUSER,DEFAULTFORMAT,EXTENDFIELD,PRINT_ORIENTATION,PRINT_FITTOPAGE,REPORT_HEADER,REPORT_FOOTER,SUBCOUNT_LABEL,PDF_GRID_LINES,FONT_FAMILY ");
            strSql.Append(" FROM REPORT ");

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
            strSql.Append(" ID,DATABASEID,SVID,REPORTNAME,AUDODATE,CATEGORY,TYPE,REPORTGROUPLIST,RPTITLE,ADDUSER,DEFAULTFORMAT,EXTENDFIELD,PRINT_ORIENTATION,PRINT_FITTOPAGE,REPORT_HEADER,REPORT_FOOTER,SUBCOUNT_LABEL,PDF_GRID_LINES,FONT_FAMILY ");
            strSql.Append(" FROM REPORT ");
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
            strSql.Append("select count(1) FROM REPORT ");
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
            strSql.Append(")AS Row, T.*  from REPORT T ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            return DbHelperSQL.Query(UserID, strSql.ToString());
        }

        /// <summary>
        /// Check if report name exist for specified database
        /// </summary>
        /// <param name="UserID">ID of Current User</param>
        /// <param name="newReportName">Report name to check with.</param>
        /// <param name="databaseid">Current database id.</param>
        /// <param name="currentrpid">Current report id (you will want to skip check current report because it should be already in use!)</param>
        /// <returns>True if name already exist, false otherwise.</returns>
        public bool CheckReportNameExist(int UserID, string newReportName, int databaseid, int currentrpid = 0)
        {
            bool result = false;

            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT COUNT(*) FROM REPORT ");
            strSql.Append(" WHERE [REPORTNAME] = @REPORTNAME AND [DATABASEID] = @DATABASEID AND (@ID = 0 OR ID <> @ID)");
            SqlParameter[] parameters = {
                    new SqlParameter("@REPORTNAME", SqlDbType.NVarChar, 50) { Value = newReportName },
                    new SqlParameter("@DATABASEID", SqlDbType.Int, 4) { Value = databaseid },
                    new SqlParameter("@ID", SqlDbType.Int, 4) { Value = currentrpid },
            };

            result = Convert.ToInt32(DbHelperSQL.GetSingle(UserID, strSql.ToString(), parameters)) > 0;

            return result;
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
            parameters[0].Value = "REPORT";
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
        /// <summary>
        /// rplist.aspx .function show rplist.
        /// </summary>
        /// <param name="databaseid"></param>
        /// <param name="reportGroup"></param>
        /// <param name="viewLevel"></param>
        /// <returns></returns>
        public DataTable GetListByDisplay(int UserID, int databaseid, string reportGroup, decimal viewLevel)
        {
            if (reportGroup.Trim() != "")
            {
                string sql = "SELECT"
                + " 	rp.ID, rp.DATABASEID, rp.SVID, rp.REPORTNAME, rp.AUDODATE, rp.CATEGORY,"
                + " 	rp.[TYPE], rp.REPORTGROUPLIST, rp.RPTITLE, rp.ADDUSER, rpc.NAME,"
                + " 	rpg.NAME AS distributiondesc, sv.VIEWLEVEL, rp.DEFAULTFORMAT"
                + " FROM dbo.REPORT rp"
                + " INNER JOIN dbo.SOURCEVIEW sv ON rp.DATABASEID = sv.DATABASEID AND rp.SVID = sv.ID"
                + " INNER JOIN dbo.RPCATEGORY rpc ON rp.CATEGORY = rpc.ID AND rp.DATABASEID = rpc.DATABASEID"
                + " INNER JOIN dbo.REPORTGROUP rpg ON rp.REPORTGROUPLIST = rpg.ID AND rp.DATABASEID = rpg.DATABASEID"
                //v1.1.0 - Cheong - 2016/06/06 - No longer need to bind column defination to report display
                //+ " INNER JOIN ("
                //+ " 	SELECT DISTINCT RPID FROM dbo.REPORTCOLUMN"
                //+ " ) rc ON rp.ID = rc.RPID"
                + " WHERE rp.DATABASEID = '" + databaseid + "'"
                + " AND rp.REPORTGROUPLIST IN (" + reportGroup + ") and sv.VIEWLEVEL >='" + viewLevel + "'"
                + " ORDER BY rp.ID DESC";

                return DbHelperSQL.Query(UserID, sql).Tables[0];
            }
            else
            {
                return null;
            }
        }
        #endregion  ExtensionMethod
    }
}