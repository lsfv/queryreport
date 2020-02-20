using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace CUSTOMRP.DAL
{
    public partial class SOURCEVIEWCOLUMN
    {
        public SOURCEVIEWCOLUMN()
        { }

        #region BasicMethod

        public int Add(int UserID, CUSTOMRP.Model.SOURCEVIEWCOLUMN model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into SOURCEVIEWCOLUMN(");
            strSql.Append("SVID,COLUMNNAME,DISPLAYNAME,COLUMNTYPE,COLUMNCOMMENT,FORMULAFIELDID,HIDDEN,DEFAULTDISPLAYNAME,DATA_TYPE)");
            strSql.Append(" values (@SVID,@COLUMNNAME,@DISPLAYNAME,@COLUMNTYPE,@COLUMNCOMMENT,@FORMULAFIELDID,@HIDDEN,@DEFAULTDISPLAYNAME,@DATA_TYPE");
            strSql.Append(")");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters =
            {
                new SqlParameter("@SVID", model.SVID),
                new SqlParameter("@COLUMNNAME", model.COLUMNNAME),
                new SqlParameter("@DISPLAYNAME", model.DISPLAYNAME),
                new SqlParameter("@COLUMNTYPE", model.COLUMNTYPE),
                new SqlParameter("@COLUMNCOMMENT", model.COLUMNCOMMENT),
                new SqlParameter("@FORMULAFIELDID", model.FORMULAFIELDID),
                new SqlParameter("@HIDDEN", model.HIDDEN),
                new SqlParameter("@DEFAULTDISPLAYNAME", model.DEFAULTDISPLAYNAME),
                new SqlParameter("@DATA_TYPE", model.DATA_TYPE),
            };
            object obj = DbHelperSQL.GetSingle(UserID, strSql.ToString(), parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        public int AddColList(int UserID, int SVID, string[] columns, string[] displaynames = null)
        {
            #region Hide all columns in sourceview
            DbHelperSQL.ExecuteSql(UserID, "UPDATE SOURCEVIEWCOLUMN SET HIDDEN = 1 WHERE SVID = @SVID", new SqlParameter[] { new SqlParameter("@SVID", SVID)});
            #endregion

            string[] strGetColumns = GetModelsForSourceView(UserID, SVID, true).Select(x => x.COLUMNNAME).ToArray();

            StringBuilder strInsertSql = new StringBuilder();
            strInsertSql.Append("insert into SOURCEVIEWCOLUMN(");
            strInsertSql.Append("SVID,COLUMNNAME,DISPLAYNAME,COLUMNTYPE,COLUMNCOMMENT,FORMULAFIELDID,HIDDEN,DEFAULTDISPLAYNAME)");
            strInsertSql.Append(" values (@SVID,@COLUMNNAME,@DISPLAYNAME,@COLUMNTYPE,@COLUMNCOMMENT,@FORMULAFIELDID,@HIDDEN,@DEFAULTDISPLAYNAME,@DATA_TYPE");
            strInsertSql.Append(")");

            SqlParameter[] insertParameters =
            {
                new SqlParameter("@SVID", SVID),
                new SqlParameter("@COLUMNNAME", SqlDbType.NVarChar),
                new SqlParameter("@DISPLAYNAME", SqlDbType.NVarChar),
                new SqlParameter("@COLUMNTYPE", CUSTOMRP.Model.SOURCEVIEWCOLUMN.ColumnTypes.Normal),
                new SqlParameter("@COLUMNCOMMENT", String.Empty),
                new SqlParameter("@FORMULAFIELDID", Convert.DBNull),
                new SqlParameter("@HIDDEN", false),
                new SqlParameter("@DEFAULTDISPLAYNAME", String.Empty),
                new SqlParameter("@DATA_TYPE", String.Empty),
            };

            string strUpdateSql = "UPDATE SOURCEVIEWCOLUMN SET HIDDEN = 0, DISPLAYNAME = @DISPLAYNAME WHERE SVID = @SVID AND COLUMNNAME = @COLUMNNAME";
            SqlParameter[] updateParameters =
            {
                new SqlParameter("@DISPLAYNAME", SqlDbType.NVarChar),
                new SqlParameter("@SVID", SVID),
                new SqlParameter("@COLUMNNAME", SqlDbType.NVarChar),
            };

            for (int i = 0; i < columns.Length; i++)
            {
                if (strGetColumns.Contains(columns[i]))
                {
                    #region Column Exist, re-enable it and update display name accordingly

                    updateParameters[2].Value = columns[i];
                    if ((displaynames != null) && (!String.IsNullOrEmpty(displaynames[i])))
                    {
                        updateParameters[0].Value = displaynames[i];
                    }
                    else
                    {
                        updateParameters[0].Value = String.Empty;
                    }
                    DbHelperSQL.ExecuteSql(UserID, strUpdateSql, updateParameters);

                    #endregion
                }
                else
                {
                    #region Column Not Exist, insert new record

                    insertParameters[1].Value = columns[i];
                    if ((displaynames != null) && (!String.IsNullOrEmpty(displaynames[i])))
                    {
                        insertParameters[2].Value = displaynames[i];
                    }
                    else
                    {
                        insertParameters[2].Value = String.Empty;
                    }
                    DbHelperSQL.ExecuteSql(UserID, strInsertSql.ToString(), insertParameters);

                    #endregion
                }
            }

            return columns.Length;
        }

        public bool Update(int UserID, CUSTOMRP.Model.SOURCEVIEWCOLUMN model)
        {
            int rows = DbHelperSQL.ExecuteSql(UserID, "UPDATE SOURCEVIEWCOLUMN SET"
                + " SVID = @SVID,"
                + " COLUMNNAME = @COLUMNNAME,"
                + " DISPLAYNAME = @DISPLAYNAME,"
                + " COLUMNTYPE = @COLUMNTYPE,"
                + " COLUMNCOMMENT = @COLUMNCOMMENT,"
                + " FORMULAFIELDID = @FORMULAFIELDID,"
                + " HIDDEN = @HIDDEN,"
                + " DEFAULTDISPLAYNAME = @DEFAULTDISPLAYNAME,"
                + " DATA_TYPE = @DATA_TYPE"
                + " WHERE ID = @ID",
                new SqlParameter("@SVID", model.SVID),
                new SqlParameter("@COLUMNNAME", model.COLUMNNAME),
                new SqlParameter("@DISPLAYNAME", model.DISPLAYNAME),
                new SqlParameter("@COLUMNTYPE", model.COLUMNTYPE),
                new SqlParameter("@COLUMNCOMMENT", model.COLUMNCOMMENT),
                new SqlParameter("@FORMULAFIELDID", model.FORMULAFIELDID),
                new SqlParameter("@HIDDEN", model.HIDDEN),
                new SqlParameter("@DEFAULTDISPLAYNAME", model.DEFAULTDISPLAYNAME),
                new SqlParameter("@DATA_TYPE", model.DATA_TYPE),
                new SqlParameter("@ID", model.ID)
            );
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Delete(int UserID, int SVCID)
        {
            int rows = DbHelperSQL.ExecuteSql(UserID, "DELETE FROM SOURCEVIEWCOLUMN WHERE ID = @ID",
                new SqlParameter("@ID", SVCID)
            );
            return (rows > 0);
        }

        public bool DeleteForSourceView(int UserID, int SVID)
        {
            Model.SOURCEVIEWCOLUMN[] original = GetModelsForSourceView(UserID, SVID, true).ToArray();

            int rows = DbHelperSQL.ExecuteSql(UserID, "DELETE FROM SOURCEVIEWCOLUMN WHERE SVID = @SVID",
                new SqlParameter("@SVID", SVID)
            );

            if (rows > 0)
            {
                Model.AUDITLOG auditobj = original.GetAuditLogObject(null);
                auditobj.UserID = UserID;
                auditobj.CreateDate = DateTime.Now;
                auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
                auditobj.ModuleName = "DAL.SOURCEVIEWCOLUMN.DeleteForSourceView.Delete";
                auditobj.Message = String.Format(AppNum.AuditMessage.SourceViewColumnDeleteSuccess, SVID);

                AUDITLOG.Add(auditobj);
            }

            return (rows > 0);
        }

        public CUSTOMRP.Model.SOURCEVIEWCOLUMN GetModel(int UserID, int ID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,SVID,COLUMNNAME,DISPLAYNAME,COLUMNTYPE,COLUMNCOMMENT,FORMULAFIELDID,HIDDEN,DEFAULTDISPLAYNAME from SOURCEVIEWCOLUMN");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)
            };
            parameters[0].Value = ID;

            CUSTOMRP.Model.SOURCEVIEWCOLUMN model = new CUSTOMRP.Model.SOURCEVIEWCOLUMN();
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

        public CUSTOMRP.Model.SOURCEVIEWCOLUMN DataRowToModel(DataRow row)
        {
            CUSTOMRP.Model.SOURCEVIEWCOLUMN model = new CUSTOMRP.Model.SOURCEVIEWCOLUMN();
            int tempint;
            if (row != null)
            {
                if (Int32.TryParse(Convert.ToString(row["ID"]), out tempint))
                {
                    model.ID = tempint;
                }
                if (Int32.TryParse(Convert.ToString(row["SVID"]), out tempint))
                {
                    model.SVID = tempint;
                }
                model.COLUMNNAME = row["COLUMNNAME"] as string;
                model.DISPLAYNAME = row["DISPLAYNAME"] as string;
                if (Int32.TryParse(Convert.ToString(row["COLUMNTYPE"]), out tempint))
                {
                    model.COLUMNTYPE = tempint;
                }
                model.COLUMNCOMMENT = row["COLUMNCOMMENT"] as string;
                model.FORMULAFIELDID = Convert.IsDBNull(row["FORMULAFIELDID"]) ? (int?)null : Convert.ToInt32(row["FORMULAFIELDID"]);
                model.HIDDEN = Convert.ToBoolean(row["HIDDEN"]);
                model.DEFAULTDISPLAYNAME = row["DEFAULTDISPLAYNAME"] as string;
            }
            return model;
        }

        public DataSet GetForSourceView(int UserID, int SVID, bool showHidden)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,SVID,COLUMNNAME,DISPLAYNAME,COLUMNTYPE,COLUMNCOMMENT,FORMULAFIELDID,HIDDEN,DEFAULTDISPLAYNAME from SOURCEVIEWCOLUMN");
            strSql.Append(" where SVID=@SVID");
            if (!showHidden)
            {
                strSql.Append(" AND HIDDEN = 0");
            }
            SqlParameter[] parameters = {
                    new SqlParameter("@SVID", SqlDbType.Int,4)
            };
            parameters[0].Value = SVID;
            return DbHelperSQL.Query(UserID, strSql.ToString(), parameters);
        }

        public List<CUSTOMRP.Model.SOURCEVIEWCOLUMN> GetModelsForSourceView(int UserID, int SVID, bool showHidden)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,SVID,COLUMNNAME,DISPLAYNAME,COLUMNTYPE,COLUMNCOMMENT,FORMULAFIELDID,HIDDEN,DEFAULTDISPLAYNAME from SOURCEVIEWCOLUMN");
            strSql.Append(" where SVID=@SVID");
            if (!showHidden)
            {
                strSql.Append(" AND HIDDEN <> 1");
            }
            SqlParameter[] parameters = {
                    new SqlParameter("@SVID", SqlDbType.Int,4)
            };
            parameters[0].Value = SVID;
            DataSet ds = DbHelperSQL.Query(UserID, strSql.ToString(), parameters);

            List<CUSTOMRP.Model.SOURCEVIEWCOLUMN> result = new List<Model.SOURCEVIEWCOLUMN>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                result.Add(DataRowToModel(dr));
            }
            return result;
        }

        #endregion BasicMethod
    }
}
