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
}