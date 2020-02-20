using System;
using System.Data;
using System.Collections.Generic;
using CUSTOMRP.Model;

namespace CUSTOMRP.BLL
{
    /// <summary>
    /// SOURCEVIEW
    /// </summary>
    public class SOURCEVIEW
    {
        private readonly CUSTOMRP.DAL.SOURCEVIEW dal=new CUSTOMRP.DAL.SOURCEVIEW();
        private readonly CUSTOMRP.DAL.SOURCEVIEWCOLUMN dalsvc = new DAL.SOURCEVIEWCOLUMN();

        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(int UserID, CUSTOMRP.Model.SOURCEVIEW model)
        {
            return dal.Add(UserID, model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(int UserID, CUSTOMRP.Model.SOURCEVIEW model)
        {
            return dal.Update(UserID, model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int UserID, int ID)
        {
            return dalsvc.DeleteForSourceView(UserID, ID) && dal.Delete(UserID, ID);
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
        public CUSTOMRP.Model.SOURCEVIEW GetModel(int UserID, int ID)
        {
            return dal.GetModel(UserID, ID);
        }

        public CUSTOMRP.Model.SOURCEVIEW GetModelByQueryName(int UserID, string TBLVIEWNAME, int DATABASEID)
        {
            return dal.GetModelByQueryName(UserID, TBLVIEWNAME, DATABASEID);
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
        /// Get Query List for dropdown on rpexcel / rpworddetail
        /// </summary>
        /// <param name="me">Current user, used to determine privilege level</param>
        /// <param name="FormatType">1 = Excel, 2 = Word, 3 = All</param>
        public DataSet GetQueryListForDropdown(CUSTOMRP.Model.LoginUser me, int FormatType = 1)
        {
            return dal.GetQueryListForDropdown(me, FormatType);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<CUSTOMRP.Model.SOURCEVIEW> GetModelList(int UserID, string strWhere)
        {
            DataSet ds = dal.GetList(UserID, strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<CUSTOMRP.Model.SOURCEVIEW> DataTableToList(DataTable dt)
        {
            List<CUSTOMRP.Model.SOURCEVIEW> modelList = new List<CUSTOMRP.Model.SOURCEVIEW>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                CUSTOMRP.Model.SOURCEVIEW model;
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
        public DataTable GetCustomList(int UserID, string strwhere)
        {       
            return dal.GetcustomList(UserID, strwhere);
        }
        #endregion  ExtensionMethod
    }
}