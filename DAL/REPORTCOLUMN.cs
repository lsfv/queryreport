using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace CUSTOMRP.DAL
{
    /// <summary>
    /// 数据访问类:REPORTCOLUMN
    /// </summary>
    public partial class REPORTCOLUMN
    {
        public REPORTCOLUMN()
        {}

        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(int UserID, CUSTOMRP.Model.REPORTCOLUMN model)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("insert into REPORTCOLUMN(");
            strSql.Append("RPID,COLUMNNAME,COLUMNFUNC,CRITERIA1,CRITERIA2,CRITERIA3,CRITERIA4,AUDODATE,SOURCEVIEWCOLUMNID,DISPLAYNAME,COLUMNTYPE,COLUMNCOMMENT,FORMULAFIELDID,HIDDEN,SEQ,EXCEL_COLWIDTH,FONT_SIZE,FONT_BOLD,FONT_ITALIC,HORIZONTAL_TEXT_ALIGN,CELL_FORMAT,BACKGROUND_COLOR,FONT_COLOR,IS_NUMERIC,IS_ASCENDING)");
            strSql.Append(" values (");
            strSql.Append("@RPID,@COLUMNNAME,@COLUMNFUNC,@CRITERIA1,@CRITERIA2,@CRITERIA3,@CRITERIA4,@AUDODATE,@SOURCEVIEWCOLUMNID,@DISPLAYNAME,@COLUMNTYPE,@COLUMNCOMMENT,@FORMULAFIELDID,@HIDDEN,@SEQ,@EXCEL_COLWIDTH,@FONT_SIZE,@FONT_BOLD,@FONT_ITALIC,@HORIZONTAL_TEXT_ALIGN,@CELL_FORMAT,@BACKGROUND_COLOR,@FONT_COLOR,@IS_NUMERIC,@IS_ASCENDING)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                    new SqlParameter("@RPID", SqlDbType.Int,4),
                    new SqlParameter("@COLUMNNAME", SqlDbType.NVarChar,-1),
                    new SqlParameter("@COLUMNFUNC", SqlDbType.Int,4),
                    new SqlParameter("@CRITERIA1", SqlDbType.NVarChar,-1),
                    new SqlParameter("@CRITERIA2", SqlDbType.NVarChar,-1),
                    new SqlParameter("@CRITERIA3", SqlDbType.NVarChar,-1),
                    new SqlParameter("@CRITERIA4", SqlDbType.NVarChar,-1),
                    new SqlParameter("@AUDODATE", SqlDbType.DateTime),
                    new SqlParameter("@SOURCEVIEWCOLUMNID", SqlDbType.Int,4),
                    new SqlParameter("@DISPLAYNAME", SqlDbType.NVarChar,-1),
                    new SqlParameter("@COLUMNTYPE", SqlDbType.Int,4),
                    new SqlParameter("@COLUMNCOMMENT", SqlDbType.NVarChar,-1),
                    new SqlParameter("@FORMULAFIELDID", SqlDbType.Int,4),
                    new SqlParameter("@HIDDEN", SqlDbType.Bit),
                    new SqlParameter("@SEQ", SqlDbType.Int,4),
                    new SqlParameter("@EXCEL_COLWIDTH", SqlDbType.Decimal),
                    new SqlParameter("@FONT_SIZE", SqlDbType.Int,4),
                    new SqlParameter("@FONT_BOLD", SqlDbType.Bit),
                    new SqlParameter("@FONT_ITALIC", SqlDbType.Bit),
                    new SqlParameter("@HORIZONTAL_TEXT_ALIGN", SqlDbType.Int,4),
                    new SqlParameter("@CELL_FORMAT", SqlDbType.NVarChar,-1),
                    new SqlParameter("@BACKGROUND_COLOR", SqlDbType.NVarChar,-1),
                    new SqlParameter("@FONT_COLOR", SqlDbType.NVarChar,-1),
                    new SqlParameter("@IS_NUMERIC", SqlDbType.Bit),
                    new SqlParameter("@IS_ASCENDING", SqlDbType.Bit),
            };
            parameters[0].Value = model.RPID;
            parameters[1].Value = model.COLUMNNAME;
            parameters[2].Value = model.COLUMNFUNC;
            parameters[3].Value = model.CRITERIA1;
            parameters[4].Value = model.CRITERIA2;
            parameters[5].Value = model.CRITERIA3;
            parameters[6].Value = model.CRITERIA4;
            parameters[7].Value = model.AUDODATE;
            parameters[8].Value = model.SOURCEVIEWCOLUMNID ?? -1;
            parameters[9].Value = model.DISPLAYNAME;
            parameters[10].Value = model.COLUMNTYPE;
            parameters[11].Value = model.COLUMNCOMMENT ?? String.Empty;
            parameters[12].Value = model.FORMULAFIELDID ?? Convert.DBNull;
            parameters[13].Value = model.HIDDEN;
            parameters[14].Value = model.SEQ;
            parameters[15].Value = model.EXCEL_COLWIDTH;
            parameters[16].Value = model.FONT_SIZE ?? Convert.DBNull;
            parameters[17].Value = model.FONT_BOLD;
            parameters[18].Value = model.FONT_ITALIC;
            parameters[19].Value = model.HORIZONTAL_TEXT_ALIGN;
            parameters[20].Value = model.CELL_FORMAT;
            parameters[21].Value = model.BACKGROUND_COLOR;
            parameters[22].Value = model.FONT_COLOR;
            parameters[23].Value = model.IS_NUMERIC;
            parameters[24].Value = model.IS_ASCENDING;

            object obj = DbHelperSQL.GetSingle(UserID, strSql.ToString(),parameters);
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
        public bool Update(int UserID, CUSTOMRP.Model.REPORTCOLUMN model)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("update REPORTCOLUMN set ");
            strSql.Append("RPID=@RPID,");
            strSql.Append("COLUMNNAME=@COLUMNNAME,");
            strSql.Append("COLUMNFUNC=@COLUMNFUNC,");
            strSql.Append("CRITERIA1=@CRITERIA1,");
            strSql.Append("CRITERIA2=@CRITERIA2,");
            strSql.Append("CRITERIA3=@CRITERIA3,");
            strSql.Append("CRITERIA4=@CRITERIA4,");
            strSql.Append("AUDODATE=@AUDODATE,");
            strSql.Append("SOURCEVIEWCOLUMNID=@SOURCEVIEWCOLUMNID,");
            strSql.Append("DISPLAYNAME=@DISPLAYNAME,");
            strSql.Append("COLUMNTYPE=@COLUMNTYPE,");
            strSql.Append("COLUMNCOMMENT=@COLUMNCOMMENT,");
            strSql.Append("FORMULAFIELDID=@FORMULAFIELDID,");
            strSql.Append("HIDDEN=@HIDDEN,");
            strSql.Append("SEQ=@SEQ,");
            strSql.Append("EXCEL_COLWIDTH=@EXCEL_COLWIDTH,");
            strSql.Append("FONT_SIZE=@FONT_SIZE,");
            strSql.Append("FONT_BOLD=@FONT_BOLD,");
            strSql.Append("FONT_ITALIC=@FONT_ITALIC,");
            strSql.Append("HORIZONTAL_TEXT_ALIGN=@HORIZONTAL_TEXT_ALIGN,");
            strSql.Append("CELL_FORMAT=@CELL_FORMAT,");
            strSql.Append("BACKGROUND_COLOR=@BACKGROUND_COLOR,");
            strSql.Append("FONT_COLOR=@FONT_COLOR,");
            strSql.Append("IS_NUMERIC=@IS_NUMERIC,");
            strSql.Append("IS_ASCENDING=@IS_ASCENDING");
            strSql.Append(" where ID=@ID");

            SqlParameter[] parameters = {
                    new SqlParameter("@RPID", model.RPID),
                    new SqlParameter("@COLUMNNAME", model.COLUMNNAME),
                    new SqlParameter("@COLUMNFUNC", model.COLUMNFUNC),
                    new SqlParameter("@CRITERIA1", model.CRITERIA1),
                    new SqlParameter("@CRITERIA2", model.CRITERIA2),
                    new SqlParameter("@CRITERIA3", model.CRITERIA3),
                    new SqlParameter("@CRITERIA4", model.CRITERIA4),
                    new SqlParameter("@AUDODATE", model.AUDODATE),
                    new SqlParameter("@SOURCEVIEWCOLUMNID", model.SOURCEVIEWCOLUMNID ?? -1),
                    new SqlParameter("@DISPLAYNAME", model.DISPLAYNAME),
                    new SqlParameter("@COLUMNTYPE", model.COLUMNTYPE),
                    new SqlParameter("@COLUMNCOMMENT",model.COLUMNCOMMENT ?? String.Empty),
                    new SqlParameter("@FORMULAFIELDID", model.FORMULAFIELDID),
                    new SqlParameter("@HIDDEN", model.HIDDEN),
                    new SqlParameter("@SEQ", model.SEQ),
                    new SqlParameter("@EXCEL_COLWIDTH", model.EXCEL_COLWIDTH),
                    new SqlParameter("@FONT_SIZE", model.FONT_SIZE),
                    new SqlParameter("@FONT_BOLD", model.FONT_BOLD),
                    new SqlParameter("@FONT_ITALIC", model.FONT_ITALIC),
                    new SqlParameter("@HORIZONTAL_TEXT_ALIGN", model.HORIZONTAL_TEXT_ALIGN),
                    new SqlParameter("@CELL_FORMAT", model.CELL_FORMAT),
                    new SqlParameter("@BACKGROUND_COLOR", model.BACKGROUND_COLOR),
                    new SqlParameter("@FONT_COLOR", model.FONT_COLOR),
                    new SqlParameter("@IS_NUMERIC", model.IS_NUMERIC),
                    new SqlParameter("@IS_ASCENDING", model.IS_ASCENDING),
                    new SqlParameter("@ID", model.ID),
           };

            int rows=DbHelperSQL.ExecuteSql(UserID, strSql.ToString(),parameters);
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
        /// Update REPORTCOLUMN data similar to where REPLACE statement in MySQL works
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Replace(int UserID, CUSTOMRP.Model.REPORTCOLUMN model)
        {
            bool result = false;

            if (model.ColumnType == Model.REPORTCOLUMN.ColumnTypes.Normal)
            {
                string strQuery = "SELECT [ID] FROM [REPORTCOLUMN] WHERE [RPID] = @RPID AND [COLUMNNAME] = @COLUMNNAME AND COLUMNFUNC = @COLUMNFUNC AND COLUMNTYPE = @COLUMNTYPE";

                try
                {
                    object columnid = DbHelperSQL.GetSingle(UserID, strQuery,
                        new SqlParameter("@RPID", model.RPID),
                        new SqlParameter("@COLUMNNAME", model.COLUMNNAME),
                        new SqlParameter("@COLUMNFUNC", model.COLUMNFUNC),
                        new SqlParameter("@COLUMNTYPE", model.COLUMNTYPE));
                    if (columnid == null)
                    {
                        result = (Add(UserID, model) != 0);
                    }
                    else
                    {
                        model.ID = (int)columnid;
                        result = Update(UserID, model);
                    }
                }
                catch
                {
                    // operation failed, return false
                }
            }
            else
            {
                string strQuery = "SELECT [ID] FROM [REPORTCOLUMN] WHERE [RPID] = @RPID AND [DISPLAYNAME] = @DISPLAYNAME AND COLUMNFUNC = @COLUMNFUNC AND COLUMNTYPE = @COLUMNTYPE";

                try
                {
                    object columnid = DbHelperSQL.GetSingle(UserID, strQuery,
                        new SqlParameter("@RPID", model.RPID),
                        new SqlParameter("@DISPLAYNAME", model.DISPLAYNAME),
                        new SqlParameter("@COLUMNFUNC", model.COLUMNFUNC),
                        new SqlParameter("@COLUMNTYPE", model.COLUMNTYPE));
                    if (columnid == null)
                    {
                        result = (Add(UserID, model) != 0);
                    }
                    else
                    {
                        model.ID = (int)columnid;
                        result = Update(UserID, model);
                    }
                }
                catch
                {
                    // operation failed, return false
                }
            }
            return result;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int UserID, int ID)
        {
            
            StringBuilder strSql=new StringBuilder();
            strSql.Append("delete from REPORTCOLUMN ");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)
            };
            parameters[0].Value = ID;

            int rows=DbHelperSQL.ExecuteSql(UserID, strSql.ToString(),parameters);
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
        public CUSTOMRP.Model.REPORTCOLUMN GetModel(int UserID, int ID)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("SELECT TOP 1 rc.ID,rc.RPID,rc.COLUMNNAME,rc.COLUMNFUNC,rc.CRITERIA1,rc.CRITERIA2,rc.CRITERIA3,rc.CRITERIA4,rc.AUDODATE,")
                .Append(" rc.SOURCEVIEWCOLUMNID,rc.DISPLAYNAME,rc.COLUMNTYPE,rc.COLUMNCOMMENT,rc.FORMULAFIELDID,rc.HIDDEN,rc.SEQ,rc.EXCEL_COLWIDTH,rc.FONT_SIZE,rc.FONT_BOLD,rc.FONT_ITALIC,rc.HORIZONTAL_TEXT_ALIGN,rc.CELL_FORMAT,rc.BACKGROUND_COLOR,rc.FONT_COLOR,rc.IS_NUMERIC,rc.IS_ASCENDING,")
                .Append(" COALESCE(NULLIF(svc.DISPLAYNAME, ''), NULLIF(svc.DEFAULTDISPLAYNAME, ''), '') SVCDISPLAYNAME")
                .Append(" from REPORTCOLUMN rc")
                .Append(" INNER JOIN REPORT rp ON rc.RPID = rp.ID")
                .Append(" INNER JOIN SOURCEVIEW sv ON rp.SVID = sv.ID")
                .Append(" LEFT JOIN SOURCEVIEWCOLUMN svc ON sv.ID = svc.SVID AND rc.COLUMNNAME = svc.COLUMNNAME")
                .Append(" WHERE rc.ID=@ID")
                .Append(" AND (((svc.HIDDEN IS NULL) OR (svc.HIDDEN = 0)) OR (rc.COLUMNTYPE = 2))")
                .Append(" ORDER BY rc.COLUMNFUNC, rc.SEQ");

            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)
            };
            parameters[0].Value = ID;

            CUSTOMRP.Model.REPORTCOLUMN model=new CUSTOMRP.Model.REPORTCOLUMN();
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

        public List<CUSTOMRP.Model.REPORTCOLUMN> GetModelListForReport(int UserID, int RPID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT rc.ID,rc.RPID,rc.COLUMNNAME,rc.COLUMNFUNC,rc.CRITERIA1,rc.CRITERIA2,rc.CRITERIA3,rc.CRITERIA4,rc.AUDODATE,")
                .Append(" rc.SOURCEVIEWCOLUMNID,rc.DISPLAYNAME,rc.COLUMNTYPE,rc.COLUMNCOMMENT,rc.FORMULAFIELDID,rc.HIDDEN,rc.SEQ,rc.EXCEL_COLWIDTH,rc.FONT_SIZE,rc.FONT_BOLD,rc.FONT_ITALIC,rc.HORIZONTAL_TEXT_ALIGN,rc.CELL_FORMAT,rc.BACKGROUND_COLOR,rc.FONT_COLOR,rc.IS_NUMERIC,rc.IS_ASCENDING,")
                .Append(" COALESCE(NULLIF(svc.DISPLAYNAME, ''), NULLIF(svc.DEFAULTDISPLAYNAME, ''), '') SVCDISPLAYNAME")
                .Append(" from REPORTCOLUMN rc")
                .Append(" INNER JOIN REPORT rp ON rc.RPID = rp.ID")
                .Append(" INNER JOIN SOURCEVIEW sv ON rp.SVID = sv.ID")
                //v1.8.2 Ben 2018.02.22 - Case insensitive causes Duplicated. Below trick convert both left and right to SQL_Latin1_General_CP1_CS_AS which is case sensitive and basically English can convert to it
                //.Append(" LEFT JOIN SOURCEVIEWCOLUMN svc ON sv.ID = svc.SVID AND rc.COLUMNNAME = svc.COLUMNNAME")
                .Append(" LEFT JOIN SOURCEVIEWCOLUMN svc ON sv.ID = svc.SVID AND rc.COLUMNNAME = svc.COLUMNNAME Collate SQL_Latin1_General_CP1_CS_AS")
                .Append(" WHERE rc.RPID=@RPID")
                .Append(" AND (((svc.HIDDEN IS NULL) OR (svc.HIDDEN = 0)) OR (rc.COLUMNTYPE = 2))")
                .Append(" ORDER BY rc.COLUMNFUNC, rc.SEQ");
            SqlParameter[] parameters = {
                    new SqlParameter("@RPID", SqlDbType.Int,4)
            };
            parameters[0].Value = RPID;

            List<CUSTOMRP.Model.REPORTCOLUMN> result = new List<Model.REPORTCOLUMN>();
            DataSet ds = DbHelperSQL.Query(UserID, strSql.ToString(), parameters);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                result.Add(DataRowToModel(dr));
            }
            return result;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public CUSTOMRP.Model.REPORTCOLUMN DataRowToModel(DataRow row)
        {
            CUSTOMRP.Model.REPORTCOLUMN model=new CUSTOMRP.Model.REPORTCOLUMN();
            if (row != null)
            {
                if(row["ID"]!=null && row["ID"].ToString()!="")
                {
                    model.ID=Int32.Parse(row["ID"].ToString());
                }
                if(row["RPID"]!=null && row["RPID"].ToString()!="")
                {
                    model.RPID=Int32.Parse(row["RPID"].ToString());
                }
                if(row["COLUMNNAME"]!=null)
                {
                    model.COLUMNNAME=row["COLUMNNAME"].ToString();
                }
                if (row["COLUMNFUNC"] != null && row["COLUMNFUNC"].ToString() != "")
                {
                    model.COLUMNFUNC = Int32.Parse(row["COLUMNFUNC"].ToString());
                }
                if(row["CRITERIA1"]!=null)
                {
                    model.CRITERIA1=row["CRITERIA1"].ToString();
                }
                if(row["CRITERIA2"]!=null)
                {
                    model.CRITERIA2=row["CRITERIA2"].ToString();
                }
                if(row["CRITERIA3"]!=null)
                {
                    model.CRITERIA3=row["CRITERIA3"].ToString();
                }
                if(row["CRITERIA4"]!=null)
                {
                    model.CRITERIA4=row["CRITERIA4"].ToString();
                }
                if(row["AUDODATE"]!=null && row["AUDODATE"].ToString()!="")
                {
                    model.AUDODATE=DateTime.Parse(row["AUDODATE"].ToString());
                }
                if ((row["SOURCEVIEWCOLUMNID"] != null) && (!Convert.IsDBNull(row["SOURCEVIEWCOLUMNID"])))
                {
                    model.SOURCEVIEWCOLUMNID = Convert.ToInt32(row["SOURCEVIEWCOLUMNID"]);
                }
                model.DISPLAYNAME = row["DISPLAYNAME"] as string;
                model.COLUMNTYPE = Convert.ToInt32(row["COLUMNTYPE"]);
                model.COLUMNCOMMENT = row["COLUMNCOMMENT"] as string;
                model.FORMULAFIELDID = Convert.IsDBNull(row["FORMULAFIELDID"]) ? (int?)null : Convert.ToInt32(row["FORMULAFIELDID"]);
                model.HIDDEN = Convert.ToBoolean(row["HIDDEN"]);
                model.SEQ = Convert.ToInt32(row["SEQ"]);
                model.EXCEL_COLWIDTH = Convert.ToDecimal(row["EXCEL_COLWIDTH"]);
                model.FONT_SIZE = Convert.IsDBNull(row["FONT_SIZE"]) ? (int?)null : Convert.ToInt32(row["FONT_SIZE"]);
                model.FONT_BOLD = Convert.ToBoolean(row["FONT_BOLD"]);
                model.FONT_ITALIC = Convert.ToBoolean(row["FONT_ITALIC"]);
                model.HORIZONTAL_TEXT_ALIGN = Convert.ToInt32(row["HORIZONTAL_TEXT_ALIGN"]);
                model.CELL_FORMAT = row["CELL_FORMAT"] as string;
                model.BACKGROUND_COLOR = row["BACKGROUND_COLOR"] as string;
                model.FONT_COLOR = row["FONT_COLOR"] as string;
                model.SVCDISPLAYNAME = row["SVCDISPLAYNAME"] as string;
                model.IS_NUMERIC = Convert.IsDBNull(row["IS_NUMERIC"]) ? true : Convert.ToBoolean(row["IS_NUMERIC"]);
                model.IS_ASCENDING = Convert.IsDBNull(row["IS_ASCENDING"]) ? true : Convert.ToBoolean(row["IS_ASCENDING"]);
            }
            return model;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(int UserID, string strWhere)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("SELECT rc.ID,rc.RPID,rc.COLUMNNAME,rc.COLUMNFUNC,rc.CRITERIA1,rc.CRITERIA2,rc.CRITERIA3,rc.CRITERIA4,rc.AUDODATE,")
                .Append(" rc.SOURCEVIEWCOLUMNID,rc.DISPLAYNAME,rc.COLUMNTYPE,rc.COLUMNCOMMENT,rc.FORMULAFIELDID,rc.HIDDEN,rc.SEQ,rc.EXCEL_COLWIDTH,rc.FONT_SIZE,rc.FONT_BOLD,rc.FONT_ITALIC,rc.HORIZONTAL_TEXT_ALIGN,rc.CELL_FORMAT,rc.BACKGROUND_COLOR,rc.FONT_COLOR,rc.IS_NUMERIC,rc.IS_ASCENDING,")
                .Append(" COALESCE(NULLIF(svc.DISPLAYNAME, ''), NULLIF(svc.DEFAULTDISPLAYNAME, ''), '') SVCDISPLAYNAME")
                .Append(" from REPORTCOLUMN rc")
                .Append(" INNER JOIN REPORT rp ON rc.RPID = rp.ID")
                .Append(" INNER JOIN SOURCEVIEW sv ON rp.SVID = sv.ID")
                .Append(" LEFT JOIN SOURCEVIEWCOLUMN svc ON sv.ID = svc.SVID AND rc.COLUMNNAME = svc.COLUMNNAME");

            if(strWhere.Trim()!="")
            {
                strSql.Append(" where "+strWhere);
                strSql.Append(" AND (((svc.HIDDEN IS NULL) OR (svc.HIDDEN = 0)) OR (rc.COLUMNTYPE = 2))");
            }
            else
            {
                strSql.Append(" WHERE (((svc.HIDDEN IS NULL) OR (svc.HIDDEN = 0)) OR (rc.COLUMNTYPE = 2))");
            }
            strSql.Append(" ORDER BY rc.COLUMNFUNC, rc.SEQ");
            return DbHelperSQL.Query(UserID, strSql.ToString());
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int UserID, int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("SELECT");
            if (Top > 0)
            {
                strSql.AppendFormat(" TOP {0}", Top);
            }
            strSql.Append(" rc.ID,rc.RPID,rc.COLUMNNAME,rc.COLUMNFUNC,rc.CRITERIA1,rc.CRITERIA2,rc.CRITERIA3,rc.CRITERIA4,rc.AUDODATE,")
                .Append(" rc.SOURCEVIEWCOLUMNID,rc.DISPLAYNAME,rc.COLUMNTYPE,rc.COLUMNCOMMENT,rc.FORMULAFIELDID,rc.HIDDEN,rc.SEQ,rc.EXCEL_COLWIDTH,rc.FONT_SIZE,rc.FONT_BOLD,rc.FONT_ITALIC,rc.HORIZONTAL_TEXT_ALIGN,rc.CELL_FORMAT,rc.BACKGROUND_COLOR,rc.FONT_COLOR,rc.IS_NUMERIC,rc.IS_ASCENDING,")
                .Append(" COALESCE(NULLIF(svc.DISPLAYNAME, ''), NULLIF(svc.DEFAULTDISPLAYNAME, ''), '') SVCDISPLAYNAME")
                .Append(" from REPORTCOLUMN rc")
                .Append(" INNER JOIN REPORT rp ON rc.RPID = rp.ID")
                .Append(" INNER JOIN SOURCEVIEW sv ON rp.SVID = sv.ID")
                .Append(" LEFT JOIN SOURCEVIEWCOLUMN svc ON sv.ID = svc.SVID AND rc.COLUMNNAME = svc.COLUMNNAME");

            if(strWhere.Trim()!="")
            {
                strSql.Append(" WHERE "+strWhere);
                strSql.Append(" AND (((svc.HIDDEN IS NULL) OR (svc.HIDDEN = 0)) OR (rc.COLUMNTYPE = 2))");
            }
            else
            {
                strSql.Append(" WHERE (((svc.HIDDEN IS NULL) OR (svc.HIDDEN = 0)) OR (rc.COLUMNTYPE = 2))");
            }
            strSql.Append(" ORDER BY " + filedOrder + ", rc.COLUMNFUNC, rc.SEQ");
            return DbHelperSQL.Query(UserID, strSql.ToString());
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(int UserID, string strWhere)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("select count(1) FROM REPORTCOLUMN ");
            if(strWhere.Trim()!="")
            {
                strSql.Append(" WHERE "+strWhere);
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
            strSql.Append(")AS Row, T.*  from REPORTCOLUMN T ");
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
            parameters[0].Value = "REPORTCOLUMN";
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

        public DataTable getCriteriaColumns(int UserID, int rpid)
        {
            string sql = "SELECT"
            + " r.SVID, rc.ID AS RCID, rc.RPID, rc.COLUMNNAME, rc.COLUMNFUNC,"
            + " rc.CRITERIA1, rc.CRITERIA2, rc.CRITERIA3, rc.CRITERIA4,"
            + " r.REPORTNAME, sv.SOURCEVIEWNAME, r.CATEGORY,"
            + " COALESCE(NULLIF(rc.DISPLAYNAME, ''), NULLIF(svc.DISPLAYNAME, ''), NULLIF(svc.DEFAULTDISPLAYNAME, ''), svc.COLUMNNAME) DISPLAYNAME"
            + " FROM dbo.REPORT r"
            + " INNER JOIN dbo.SOURCEVIEW sv ON r.SVID = sv.ID"
            + " INNER JOIN dbo.REPORTCOLUMN rc ON r.ID = rc.RPID"
            + " LEFT JOIN SOURCEVIEWCOLUMN svc ON sv.ID = svc.SVID AND rc.COLUMNNAME = svc.COLUMNNAME"
            + " WHERE (rc.RPID = " + Convert.ToString(rpid) + ")"
            + " AND (((svc.HIDDEN IS NULL) OR (svc.HIDDEN = 0)) OR (rc.COLUMNTYPE = 2))";

            return DbHelperSQL.Query(UserID, sql).Tables[0];
        }
        #endregion  ExtensionMethod
    }
}

