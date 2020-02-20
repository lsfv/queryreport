using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace CUSTOMRP.DAL
{
    public partial class WORDFILE
    {
        public WORDFILE()
        { }

        #region  BasicMethod

        public int AddFile(CUSTOMRP.Model.WORDFILE model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("INSERT INTO [WORDFILE](");
            strSql.Append("[WordFileName],[OrigFileName],[Description],[WordTemplateID],[RPID],[ModifyDate],[ModifyUser],[CreateDate],[CreateUser]");
            strSql.Append(") VALUES (");
            strSql.Append("@WordFileName,@OrigFileName,@Description,@WordTemplateID,@RPID,@ModifyDate,@ModifyUser,@CreateDate,@CreateUser");
            strSql.Append(");select @@IDENTITY");
            SqlParameter[] parameters = new SqlParameter[]
            {
                    new SqlParameter("@WordFileName", model.WordFileName),
                    new SqlParameter("@OrigFileName", model.OrigFileName),
                    new SqlParameter("@Description", model.Description),
                    new SqlParameter("@WordTemplateID", model.WordTemplateID),
                    new SqlParameter("@RPID", model.RPID),
                    new SqlParameter("@ModifyDate", model.ModifyDate),
                    new SqlParameter("@ModifyUser", model.ModifyUser),
                    new SqlParameter("@CreateDate", model.CreateDate),
                    new SqlParameter("@CreateUser", model.CreateUser),
            };

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

        public bool UpdateFile(CUSTOMRP.Model.WORDFILE model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("UPDATE [WORDFILE] SET ");
            strSql.Append("[WordFileName] = @WordFileName,");
            strSql.Append("[OrigFileName] = @OrigFileName,");
            strSql.Append("[Description] = @Description,");
            strSql.Append("[WordTemplateID] = @WordTemplateID,");
            strSql.Append("[ModifyDate] = @ModifyDate,");
            strSql.Append("[ModifyUser] = @ModifyUser");
            strSql.Append(" WHERE [RPID] = @RPID");
            SqlParameter[] parameters = new SqlParameter[]
            {
                    new SqlParameter("@WordFileName", model.WordFileName),
                    new SqlParameter("@OrigFileName", model.OrigFileName),
                    new SqlParameter("@Description", model.Description),
                    new SqlParameter("@WordTemplateID", model.WordTemplateID),
                    new SqlParameter("@ModifyDate", model.ModifyDate),
                    new SqlParameter("@ModifyUser", model.ModifyUser),
                    new SqlParameter("@RPID", model.RPID),
            };

            int rows = DbHelperSQL.ExecuteSql(model.ModifyUser, strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public bool DeleteFile(int UserID, int ID)
        //{

        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("DELETE FROM [WORDFILE] WHERE WordFileID=@ID");
        //    SqlParameter[] parameters = {
        //            new SqlParameter("@ID", SqlDbType.Int,4)
        //    };
        //    parameters[0].Value = ID;

        //    int rows = DbHelperSQL.ExecuteSql(UserID, strSql.ToString(), parameters);
        //    if (rows > 0)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public CUSTOMRP.Model.WORDFILE GetModel(int UserID, int ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT [WordFileID],[WordFileName],[OrigFileName],[Description],[WordTemplateID],[RPID],[ModifyDate],[ModifyUser],[CreateDate],[CreateUser] FROM [WORDFILE] ");
            strSql.Append("WHERE WordFileID = @ID");
            SqlParameter[] parameters = new SqlParameter[]
            {
                    new SqlParameter("@ID", SqlDbType.Int,4),
            };
            parameters[0].Value = ID;

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

        public CUSTOMRP.Model.WORDFILE GetModelByReportID(int UserID, int rpID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT [WordFileID],[WordFileName],[OrigFileName],[Description],[WordTemplateID],[RPID],[ModifyDate],[ModifyUser],[CreateDate],[CreateUser] FROM [WORDFILE] ");
            strSql.Append("WHERE RPID = @RPID");

            SqlParameter[] parameters = new SqlParameter[]
            {
                    new SqlParameter("@RPID", rpID),
            };

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

        public List<CUSTOMRP.Model.WORDFILE> GetModelByTemplateID(int UserID, int WordTemplateID)
        {
            List<CUSTOMRP.Model.WORDFILE> result = new List<Model.WORDFILE>();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT [WordFileID],[WordFileName],[OrigFileName],[Description],[WordTemplateID],[RPID],[ModifyDate],[ModifyUser],[CreateDate],[CreateUser] FROM [WORDFILE] ");
            strSql.Append("WHERE WordTemplateID = @WordTemplateID");

            SqlParameter[] parameters = new SqlParameter[]
            {
                    new SqlParameter("@WordTemplateID", WordTemplateID),
            };

            DataSet ds = DbHelperSQL.Query(UserID, strSql.ToString(), parameters);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                result.Add(DataRowToModel(dr));
            }
            return result;
        }

        /// <summary>
        /// Convert DataRow to POCO object of WordFile
        /// </summary>
        /// <param name="row">DataRow to be converted</param>
        /// <returns></returns>
        private CUSTOMRP.Model.WORDFILE DataRowToModel(DataRow row)
        {
            CUSTOMRP.Model.WORDFILE model = new CUSTOMRP.Model.WORDFILE();
            if (row != null)
            {
                if (row["WordFileID"] != null)
                {
                    model.WordFileID = Convert.ToInt32(row["WordFileID"]);
                }
                if (row["WordFileName"] != null)
                {
                    model.WordFileName = Convert.ToString(row["WordFileName"]);
                }
                if (row["OrigFileName"] != null)
                {
                    model.OrigFileName = Convert.ToString(row["OrigFileName"]);
                }
                if (row["Description"] != null)
                {
                    model.Description = Convert.ToString(row["Description"]);
                }
                if (row["WordTemplateID"] != null)
                {
                    model.WordTemplateID = Convert.ToInt32(row["WordTemplateID"]);
                }
                if (row["RPID"] != null)
                {
                    model.RPID = Convert.ToInt32(row["RPID"]);
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
            }
            return model;
        }

        #endregion  BasicMethod
    }
}
