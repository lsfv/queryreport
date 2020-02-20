using System;
using System.Data;
using System.Collections.Generic;
using CUSTOMRP.Model;

namespace CUSTOMRP.BLL
{
    public class REPORT
    {
        private readonly CUSTOMRP.DAL.REPORT dal=new DAL.REPORT();
        private readonly CUSTOMRP.DAL.REPORTCOLUMN dalrc = new DAL.REPORTCOLUMN();

        #region  BasicMethod

        public bool Replace(CUSTOMRP.Model.REPORT model)
        {
            bool result = false;
            using (var ts = DAL.TransactionUtils.CreateTransactionScope())
            {
                try
                {
                    if (dal.Replace(model))
                    {
                        ts.Complete();
                        result = true;
                    }
                    //v1.2.0 Kim 2016.10.26 move upward
                    //result = true;
                }
                catch (Exception)
                {
                    // Log error here
                    throw;
                }
            }
            return result;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int UserID, int ID)
        {
            bool result = false;
            using (var ts = DAL.TransactionUtils.CreateTransactionScope())
            {
                try
                {
                    if (dal.Delete(UserID, ID))
                    {
                        ts.Complete();
                    }
                    result = true;
                }
                catch (Exception)
                {
                    // Log error here
                    throw;
                }
            }
            return result;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        //public bool DeleteList(string IDlist)
        //{
        //    return dal.DeleteList(IDlist);
        //}

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public CUSTOMRP.Model.REPORT GetModel(int UserID, int ID)
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
        public List<CUSTOMRP.Model.REPORT> GetModelList(int UserID, string strWhere)
        {
            DataSet ds = dal.GetList(UserID, strWhere);
            return DataTableToList(ds.Tables[0]);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        private List<CUSTOMRP.Model.REPORT> DataTableToList(DataTable dt)
        {
            List<CUSTOMRP.Model.REPORT> modelList = new List<CUSTOMRP.Model.REPORT>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                CUSTOMRP.Model.REPORT model;
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
        //public DataSet GetAllList()
        //{
        //    return GetList("");
        //}

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

        /// <summary>
        /// Check if report name exist for specified database
        /// </summary>
        /// <param name="newReportName">Report name to check with.</param>
        /// <param name="databaseid">Current database id.</param>
        /// <param name="currentrpid">Current report id (you will want to skip check current report because it should be already in use!)</param>
        /// <returns>True if name already exist, false otherwise.</returns>
        public bool CheckReportNameExist(int UserID, string newReportName, int databaseid, int currentrpid = 0)
        {
            return dal.CheckReportNameExist(UserID, newReportName, databaseid, currentrpid);
        }

        #endregion  BasicMethod

        #region  ExtensionMethod

        public DataTable GetlistByDisplay(int UserID, int databaseid, string reportGroup, bool viewrp, decimal viewLevel)
        {
            if (viewrp)
            {
                return dal.GetListByDisplay(UserID, databaseid, reportGroup, viewLevel);
            }
            else

            {
                return null;
             }
        }

        public DataTable getCriteriaColumns(int UserID, int rpid)
        {
            return dalrc.getCriteriaColumns(UserID, rpid);
        }

        public List<CUSTOMRP.Model.REPORTCOLUMN> getCriteriaColumnModelList(int UserID, int rpid)
        {
            DataSet ds = dalrc.GetList(UserID, String.Format("rc.RPID = {0}", rpid));
            List<CUSTOMRP.Model.REPORTCOLUMN> result = new List<Model.REPORTCOLUMN>();
            int rowsCount = ds.Tables[0].Rows.Count;
            if (rowsCount > 0)
            {
                CUSTOMRP.Model.REPORTCOLUMN model;
                for (int n = 0; n < rowsCount; n++)
                {
                    model = dalrc.DataRowToModel(ds.Tables[0].Rows[n]);
                    if (model != null)
                    {
                        result.Add(model);
                    }
                }
            }
            return result;
        }

        public List<CUSTOMRP.Model.REPORTCOLUMN> GetReportColumnModelListForReport(int UserID, int RPID)
        {
            return dalrc.GetModelListForReport(UserID, RPID);
        }

        #endregion  ExtensionMethod
    }
}

