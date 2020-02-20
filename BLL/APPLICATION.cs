using System;
using System.Data;
using System.Collections.Generic;
using CUSTOMRP.Model;

namespace CUSTOMRP.BLL
{
    /// <summary>
    /// APPLICATION
    /// </summary>
    public partial class APPLICATION
    {
        private readonly CUSTOMRP.DAL.APPLICATION dal=new CUSTOMRP.DAL.APPLICATION();
        public APPLICATION()
        {}
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int  Add(CUSTOMRP.Model.APPLICATION model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(CUSTOMRP.Model.APPLICATION model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int UserID, int ID)
        {
            
            return dal.Delete(UserID, ID);
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool DeleteList(int UserID, string IDlist)
        {
            return dal.DeleteList(UserID, IDlist);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public CUSTOMRP.Model.APPLICATION GetModel(int UserID, int ID)
        {

            return dal.GetModel(UserID, ID);
        }

        

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(int UserID, string strWhere)
        {
            return dal.GetList(UserID, strWhere);
        }
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int UserID, int Top, string strWhere, string filedOrder)
        {
            return dal.GetList(UserID, Top, strWhere, filedOrder);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<CUSTOMRP.Model.APPLICATION> GetModelList(int UserID, string strWhere)
        {
            DataSet ds = dal.GetList(UserID, strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<CUSTOMRP.Model.APPLICATION> DataTableToList(DataTable dt)
        {
            List<CUSTOMRP.Model.APPLICATION> modelList = new List<CUSTOMRP.Model.APPLICATION>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                CUSTOMRP.Model.APPLICATION model;
                for (int n = 0; n < rowsCount; n++)
                {
                    model = dal.DataRowToModel(dt.Rows[n]);
                    if (model != null)
                    {
                        modelList.Add(model);
                    }
                }
            }
            return modelList;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetAllList(int UserID)
        {
            return GetList(UserID, "");
        }

        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public int GetRecordCount(int UserID, string strWhere)
        {
            return dal.GetRecordCount(UserID, strWhere);
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetListByPage(int UserID, string strWhere, string orderby, int startIndex, int endIndex)
        {
            return dal.GetListByPage(UserID, strWhere, orderby, startIndex, endIndex);
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        //public DataSet GetList(int PageSize,int PageIndex,string strWhere)
        //{
            //return dal.GetList(PageSize,PageIndex,strWhere);
        //}

        #endregion  BasicMethod
        #region  ExtensionMethod

        #endregion  ExtensionMethod
    }
}

