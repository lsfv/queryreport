using System;
using System.Data;
using System.Collections.Generic;
using CUSTOMRP.Model;
using CUSTOMRP.DAL;
namespace CUSTOMRP.BLL
{
    //DBVersion_Script
    public partial class DBVersion_Script
    {
        private readonly CUSTOMRP.DAL.DBVersion_Script dal = new CUSTOMRP.DAL.DBVersion_Script();

        public DBVersion_Script()
        { }

        #region  Method
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int script_version)
        {
            return dal.Exists(script_version);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(CUSTOMRP.Model.DBVersion_Script model)
        {
            dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(CUSTOMRP.Model.DBVersion_Script model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int script_version)
        {
            return dal.Delete(script_version);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public CUSTOMRP.Model.DBVersion_Script GetModel(int script_version)
        {
            return dal.GetModel(script_version);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataTable GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataTable GetList(int Top, string strWhere, string filedOrder)
        {
            return dal.GetList(Top, strWhere, filedOrder);
        }

       
        #endregion

    }



    public partial class DBVersion_Script
    {

        public DataTable GetNeedUpdate(int current)
        {
            return GetList("script_version>" + current);
        }


        public int CurrentVersion()
        {
            return dal.CurrentVersion();
        }

        public int GetCountNeedUpdate(int current)
        {
            if (current == -1)
            {
                return -1;
            }
            else
            {
                return dal.GetCountNeedUpdate(current);
            }
        }

        public int addhistory(int version,string extend)
        {
            return dal.AddHistory(version, extend);
        }


        public static string sql_create = @"if (select count(*) from sys.objects where name='dbversion_script') =0 
                            CREATE TABLE [dbo].[DBVersion_Script](
                                    [script_version][int] NOT NULL,
                                   [script_title] [nvarchar] (50) NOT NULL,
                                    [script_desc] [nvarchar] (250) NOT NULL,
                                     [script_sql] [nvarchar] (max) NOT NULL,
	                                [script_extend] [nvarchar] (250) NOT NULL,
                                  CONSTRAINT[PK_DBVersion_Script] PRIMARY KEY CLUSTERED
                                (
                                    [script_version] ASC
                                )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
                                ) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY];";

        public static string sql_create2 = @"if (select count(*) from sys.objects where name='DBVersion_History') =0  
                                    begin
                                    CREATE TABLE [dbo].[DBVersion_History](
	                                [history_id] [int] IDENTITY(1,1) NOT NULL,
	                                [history_version] [int] NOT NULL,
	                                [history_extend] [nvarchar](250) NOT NULL,
	                                [history_datetime] [datetime] NOT NULL,
                                 CONSTRAINT [PK_DBVersion_History] PRIMARY KEY CLUSTERED 
                                (
	                                [history_id] ASC
                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                ) ON [PRIMARY];
                                ALTER TABLE [dbo].[DBVersion_History] ADD  CONSTRAINT [DF_DBVersion_History_history_datetime]  DEFAULT (getdate()) FOR [history_datetime];
                                end";
    }
}