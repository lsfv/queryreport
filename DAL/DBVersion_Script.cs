using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace CUSTOMRP.DAL
{
    //DBVersion_Script
    public partial class DBVersion_Script
    {

        public bool Exists(int script_version)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from DBVersion_Script");
            strSql.Append(" where ");
            strSql.Append(" script_version = @script_version  ");
            SqlParameter[] parameters = {
                    new SqlParameter("@script_version", SqlDbType.Int,4)            };
            parameters[0].Value = script_version;

            object ret = DbHelperSQL.ExecuteSql(1, strSql.ToString(), parameters);
            return (int)ret == 0 ? false : true;
        }



        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(Model.DBVersion_Script model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into DBVersion_Script(");
            strSql.Append("script_version,script_title,script_desc,script_sql,script_extend,script_date");
            strSql.Append(") values (");
            strSql.Append("@script_version,@script_title,@script_desc,@script_sql,@script_extend,@script_date");
            strSql.Append(") ");

            SqlParameter[] parameters = {
                        new SqlParameter("@script_version", SqlDbType.Int,4) ,
                        new SqlParameter("@script_title", SqlDbType.NVarChar,50) ,
                        new SqlParameter("@script_desc", SqlDbType.NVarChar,250) ,
                        new SqlParameter("@script_sql", SqlDbType.NVarChar,-1) ,
                        new SqlParameter("@script_extend", SqlDbType.NVarChar,250) ,
                        new SqlParameter("@script_date", SqlDbType.DateTime)

            };

            parameters[0].Value = model.script_version;
            parameters[1].Value = model.script_title;
            parameters[2].Value = model.script_desc;
            parameters[3].Value = model.script_sql;
            parameters[4].Value = model.script_extend;
            parameters[5].Value = model.script_date;
            DbHelperSQL.ExecuteSql(1, strSql.ToString(), parameters);

        }


        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.DBVersion_Script model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update DBVersion_Script set ");

            strSql.Append(" script_version = @script_version , ");
            strSql.Append(" script_title = @script_title , ");
            strSql.Append(" script_desc = @script_desc , ");
            strSql.Append(" script_sql = @script_sql , ");
            strSql.Append(" script_extend = @script_extend , ");
            strSql.Append(" script_date = @script_date  ");
            strSql.Append(" where script_version=@script_version  ");

            SqlParameter[] parameters = {
                        new SqlParameter("@script_version", SqlDbType.Int,4) ,
                        new SqlParameter("@script_title", SqlDbType.NVarChar,50) ,
                        new SqlParameter("@script_desc", SqlDbType.NVarChar,250) ,
                        new SqlParameter("@script_sql", SqlDbType.NVarChar,-1) ,
                        new SqlParameter("@script_extend", SqlDbType.NVarChar,250) ,
                        new SqlParameter("@script_date", SqlDbType.DateTime)
            };

            parameters[0].Value = model.script_version;
            parameters[1].Value = model.script_title;
            parameters[2].Value = model.script_desc;
            parameters[3].Value = model.script_sql;
            parameters[4].Value = model.script_extend;
            parameters[5].Value = model.script_date;
            int rows = DbHelperSQL.ExecuteSql(1, strSql.ToString(), parameters);
            return rows > 0 ? true : false;
        }


        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int script_version)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from DBVersion_Script ");
            strSql.Append(" where script_version=@script_version ");
            SqlParameter[] parameters = {
                    new SqlParameter("@script_version", SqlDbType.Int,4)            };
            parameters[0].Value = script_version;


            int rows = DbHelperSQL.ExecuteSql(1, strSql.ToString(), parameters);
            return rows > 0 ? true : false;
        }



        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.DBVersion_Script GetModel(int script_version)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select script_version, script_title, script_desc, script_sql, script_extend, script_date  ");
            strSql.Append("  from DBVersion_Script ");
            strSql.Append(" where script_version=@script_version ");
            SqlParameter[] parameters = {
                    new SqlParameter("@script_version", SqlDbType.Int,4)            };
            parameters[0].Value = script_version;


            Model.DBVersion_Script model = null;
            DataTable dt = DbHelperSQL.Query(1, strSql.ToString(), parameters).Tables[0];

            if (dt.Rows.Count > 0)
            {
                model = new Model.DBVersion_Script();
                if (dt.Rows[0]["script_version"].ToString() != "")
                {
                    model.script_version = int.Parse(dt.Rows[0]["script_version"].ToString());
                }
                model.script_title = dt.Rows[0]["script_title"].ToString();
                model.script_desc = dt.Rows[0]["script_desc"].ToString();
                model.script_sql = dt.Rows[0]["script_sql"].ToString();
                model.script_extend = dt.Rows[0]["script_extend"].ToString();
                if (dt.Rows[0]["script_date"].ToString() != "")
                {
                    model.script_date = DateTime.Parse(dt.Rows[0]["script_date"].ToString());
                }
            }
            return model;
        }


        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataTable GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM DBVersion_Script ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperSQL.Query(1, strSql.ToString(), null).Tables[0];
        }


        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataTable GetList(int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" * ");
            strSql.Append(" FROM DBVersion_Script ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return DbHelperSQL.Query(1, strSql.ToString(), null).Tables[0];
        }

    }

    public partial class DBVersion_Script
    {
        public int CurrentVersion()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT isnull(max(history_version),-1)  FROM [DBVersion_History]");
            object ret = DbHelperSQL.GetSingle(1, strSql.ToString(), null);
            return (int)ret;
        }

        public int GetCountNeedUpdate(int current)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT count(*)  FROM [DBVersion_script] where script_version>" + current);
            object ret = DbHelperSQL.GetSingle(1, strSql.ToString(), null);
            return (int)ret;
        }


        /// <summary>
		/// 增加一条数据
		/// </summary>
		public int AddHistory(int version,string extend)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into DBVersion_History(");
            strSql.Append("history_version,history_extend,history_datetime");
            strSql.Append(") values (");
            strSql.Append("@history_version,@history_extend,@history_datetime");
            strSql.Append(") ");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                        new SqlParameter("@history_version", SqlDbType.Int,4) ,
                        new SqlParameter("@history_extend", SqlDbType.NVarChar,250) ,
                        new SqlParameter("@history_datetime", SqlDbType.DateTime)

            };

            parameters[0].Value = version;
            parameters[1].Value = extend;
            parameters[2].Value = DateTime.Now;

            object obj = DbHelperSQL.ExecuteSql(1, strSql.ToString(), parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }

        }

    }
}