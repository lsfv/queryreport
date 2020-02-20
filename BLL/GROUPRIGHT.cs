using System;
using System.Data;
using System.Collections.Generic;
using CUSTOMRP.Model;
namespace CUSTOMRP.BLL
{
    public class GROUPRIGHT
    {
        private readonly CUSTOMRP.DAL.GROUPRIGHT dal = new DAL.GROUPRIGHT();

        #region  BasicMethod

        /// <summary>
        /// 得到最大ID
        /// </summary>
        public int GetMaxId(int UserID)
        {
            return dal.GetMaxId(UserID);
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int UserID, int GID)
        {
            return dal.Exists(UserID, GID);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(int UserID, CUSTOMRP.Model.GROUPRIGHT model)
        {
            return dal.Add(UserID, model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(int UserID, CUSTOMRP.Model.GROUPRIGHT model)
        {
            return dal.Update(UserID, model);
        }

    
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int UserID, int GID)
        {
            return dal.Delete(UserID, GID);
        }

        //public bool DeleteList(int UserID, string IDlist)
        //{
        //    return dal.DeleteList(UserID, IDlist);
        //}

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public CUSTOMRP.Model.GROUPRIGHT GetModel(int UserID, int ID)
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
        public List<CUSTOMRP.Model.GROUPRIGHT> GetModelList(int UserID, string strWhere)
        {
            DataSet ds = dal.GetList(UserID, strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<CUSTOMRP.Model.GROUPRIGHT> DataTableToList(DataTable dt)
        {
            List<CUSTOMRP.Model.GROUPRIGHT> modelList = new List<CUSTOMRP.Model.GROUPRIGHT>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                CUSTOMRP.Model.GROUPRIGHT model;
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

