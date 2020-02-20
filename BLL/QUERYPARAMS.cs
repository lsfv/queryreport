using System;
using System.Data;

namespace CUSTOMRP.BLL
{
    /// <summary>
    /// QUERYPARAMS
    /// </summary>
    public class QUERYPARAMS
    {
        private readonly CUSTOMRP.DAL.QUERYPARAMS dal = new DAL.QUERYPARAMS();

        #region  BasicMethod

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int UserID, int REPORT, string NAME)
        {
            return dal.Exists(UserID, REPORT, NAME);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(int UserID, CUSTOMRP.Model.QUERYPARAMS model)
        {
            return dal.Add(UserID, model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(int UserID, CUSTOMRP.Model.QUERYPARAMS model)
        {
            return dal.Update(UserID, model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int UserID, int REPORT, string NAME)
        {
            return dal.Delete(UserID, REPORT, NAME);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(int UserID, int REPORT)
        {
            return dal.GetList(UserID, REPORT);
        }
        #endregion
    }
}
