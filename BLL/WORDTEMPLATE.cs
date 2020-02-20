using System;
using System.Data;
using System.Collections.Generic;
using CUSTOMRP.Model;
using System.Text;

namespace CUSTOMRP.BLL
{
    public class WORDTEMPLATE
    {
        private readonly CUSTOMRP.DAL.WORDTEMPLATE dal = new DAL.WORDTEMPLATE();
        private readonly CUSTOMRP.DAL.WORDFILE dalWF = new DAL.WORDFILE();

        #region  BasicMethod

        public int Replace(CUSTOMRP.Model.WORDTEMPLATE model)
        {
            int result = 0;

            if ((model.WordTemplateID != 0) && (dal.GetModel(model.ModifyUser, model.WordTemplateID, model.ModifyUser) != null))
            {
                if (dal.Update(model))
                {
                    result = model.WordTemplateID;
                }
            }
            else
            {
                result = dal.Add(model);
            }

            return result;
        }

        public bool Delete(int UserID, int ID)
        {
            if (dalWF.GetModelByTemplateID(UserID, ID).Count > 0)
            {
                throw new InvalidOperationException(AppNum.Error_Delete_WordTemplate_In_Use);
            }
            else
            {
                return dal.Delete(UserID, ID);
            }
        }

        /// <summary>
        /// Get POCO object for WordTemplate
        /// </summary>
        /// <param name="UserID">ID of Current User</param>
        /// <param name="ID">WordTemplate ID</param>
        /// <param name="perspectiveUserID">UserID, set it as -1 to ignore</param>
        /// <returns></returns>
        public CUSTOMRP.Model.WORDTEMPLATE GetModel(int UserID, int ID, int perspectiveUserID)
        {
            return dal.GetModel(UserID, ID, perspectiveUserID);
        }

        /// <summary>
        /// Get POCO object for WordTemplate
        /// </summary>
        /// <param name="UserID">ID of Current User</param>
        /// <param name="ID">Report ID</param>
        /// <param name="perspectiveUserID">UserID, set it as -1 to ignore</param>
        /// <returns></returns>
        public CUSTOMRP.Model.WORDTEMPLATE GetModelByReportID(int UserID, int ID, int perspectiveUserID)
        {
            return dal.GetModelByReportID(UserID, ID, perspectiveUserID);
        }

        /// <summary>
        /// Get POCO object for WordTemplate
        /// </summary>
        /// <param name="UserID">ID of Current User</param>
        /// <param name="svID">SourceView ID</param>
        /// <param name="perspectiveUserID">UserID, set it as -1 to ignore</param>
        /// <returns></returns>
        public CUSTOMRP.Model.WORDTEMPLATE GetModelBySVID(int UserID, int svID, int perspectiveUserID)
        {
            return dal.GetModelBySVID(UserID, svID, perspectiveUserID);
        }

        /// <summary>
        /// Get POCO object list for WordTemplate
        /// </summary>
        /// <param name="UserID">ID of Current User</param>
        /// <param name="ID">Database ID</param>
        /// <param name="sourceType">0 = View, 1 = Table, 2 = Stored Proc, -1 to ignore</param>
        /// <param name="perspectiveUserID">UserID, set it as -1 to ignore</param>
        /// <returns></returns>
        public List<CUSTOMRP.Model.WORDTEMPLATE> GetTemplateList(int UserID, int databaseID, int sourceType, int perspectiveUserID)
        {
            return dal.GetTemplateList(UserID, databaseID, sourceType, perspectiveUserID);
        }

        ///// <summary>
        ///// 获得数据列表
        ///// </summary>
        //public DataSet GetList(string strWhere)
        //{
        //    return dal.GetList(strWhere);
        //}


        ///// <summary>
        ///// 获得前几行数据
        ///// </summary>
        //public DataSet GetList(int Top, string strWhere, string filedOrder)
        //{
        //    return dal.GetList(Top, strWhere, filedOrder);
        //}
        ///// <summary>
        ///// 获得数据列表
        ///// </summary>
        //public List<CUSTOMRP.Model.WORDTEMPLATE> GetModelList(string strWhere)
        //{
        //    DataSet ds = dal.GetList(strWhere);
        //    return DataTableToList(ds.Tables[0]);
        //}

        ///// <summary>
        ///// 获得数据列表
        ///// </summary>
        //public DataSet GetAllList()
        //{
        //    return GetList("");
        //}

        ///// <summary>
        ///// 分页获取数据列表
        ///// </summary>
        //public int GetRecordCount(string strWhere)
        //{
        //    return dal.GetRecordCount(strWhere);
        //}
        ///// <summary>
        ///// 分页获取数据列表
        ///// </summary>
        //public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        //{
        //    return dal.GetListByPage(strWhere, orderby, startIndex, endIndex);
        //}
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        //public DataSet GetList(int PageSize,int PageIndex,string strWhere)
        //{
        //return dal.GetList(PageSize,PageIndex,strWhere);
        //}

        #endregion  BasicMethod
        #region  ExtensionMethod

        //public DataTable GetWordFileList(int databaseid)
        //{
        //    return dal.GetFileList(databaseid);
        //}

        //public DataTable GetWordFileListByID(int WordFileID)
        //{
        //    return dal.GetWordFileListByID(WordFileID);
        //}

        //public DataTable GetWordTemplateList(int databaseid, int sourceType)
        //{
        //    return dal.GetWordTemplateList(databaseid, sourceType);
        //}

        //public DataTable GetWordTemplateListByTemplateID(int WORDTEMPLATEID)
        //{
        //    return dal.GetWordTemplateListByTemplateID(WORDTEMPLATEID);
        //}

        //public string CreateLocalViewByViewAndDBName(string ViewName, string DBName,string LoginUserID,string sql)
        //{
        //    return dal.CreateLocalViewByViewAndDBName(ViewName, DBName, LoginUserID,sql);
        //}

        //public DataSet GetWordReportInfoByID(int rpid)
        //{
        //    string strSql = "";
        //    strSql = "select * FROM V_WORDREPORT where ID='" + rpid + "'";

        //    return dal.GetWordReportInfoByID(rpid); ;
        //}

        //public List<WORDFILE> GetWordFileByTemplateID(int templatedID)
        //{
        //    return dalWF.GetModelByTemplateID(templatedID);
        //}
       
        #endregion  ExtensionMethod
    }
}
