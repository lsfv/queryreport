using System;
using System.Data;
using System.Collections.Generic;
using CUSTOMRP.Model;

namespace CUSTOMRP.BLL
{
    /// <summary>
    /// USER
    /// </summary>
    public class USER
    {
        private readonly CUSTOMRP.DAL.USER dal=new DAL.USER();
        private readonly CUSTOMRP.DAL.Common dalCOMMON = new DAL.Common();

        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int  Add(CUSTOMRP.Model.USER model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(CUSTOMRP.Model.USER model)
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
        public CUSTOMRP.Model.USER GetModel(int UserID, int ID)
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
            return dal.GetList(UserID, Top,strWhere,filedOrder);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<CUSTOMRP.Model.USER> GetModelList(int UserID, string strWhere)
        {
            DataSet ds = dal.GetList(UserID, strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public CUSTOMRP.Model.USER GetModelForUser(int UserID, string LoginID, int DatabaseID)
        {
            return dal.GetModelForUser(UserID, LoginID, DatabaseID);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<CUSTOMRP.Model.USER> DataTableToList(DataTable dt)
        {
            List<CUSTOMRP.Model.USER> modelList = new List<CUSTOMRP.Model.USER>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                CUSTOMRP.Model.USER model;
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

        public DataTable GetCustomList(int UserID, int databaseid)
        {
            return dal.GetcustomList(UserID, databaseid);
        }

        public CUSTOMRP.Model.USER GetModel(int UserID, string UID, int DATABASEID, string PASSWORD)
        {
            return dal.GetModel(UserID, UID, DATABASEID, PASSWORD);
        }

        public CUSTOMRP.Model.USER GetModel(int UserID, string UID, int DATABASEID)
        {
            return dal.GetModel(UserID, UID, DATABASEID);
        }

        public void getUserInfo(int UserID, out string p_strCompanyID, out string p_strSecurityGroupID, out string p_strContractID, string p_strLoginID, int databaseid, string DatabaseNAME)
        {
            p_strCompanyID = "";
            p_strSecurityGroupID = "";
            p_strContractID = "";
            //v1.0.0 - Cheong - 2015/07/23 - Modify SQL statement to allow switching to other database actually works
            //string sql_userInfo = "select * from v_Security where username='" + p_strLoginID + "' and DatabaseID = " + Convert.ToString(databaseid);
            string sql_userInfo = String.Format("SELECT DISTINCT DatabaseID = {0}, v.ID, v.CompanyID, v.SecurityGroupID, v.ContractID"
                + " FROM [{1}].[qreport].[v_Security] v"
                + " WHERE v.Username = '{2}'", databaseid, DatabaseNAME, p_strLoginID);
            List<string> cid = new List<string>();
            List<string> sid = new List<string>();
            List<string> contractid = new List<string>();

            DataTable mydt = dalCOMMON.query(UserID, sql_userInfo);
            if (mydt.Rows.Count <= 0)
            {
                p_strCompanyID = "-1";
                p_strSecurityGroupID = "-1";
                p_strContractID = "-1";
            }
            else
            {
                foreach (DataRow dr in mydt.Rows)
                {
                    if (cid.Contains(dr["CompanyID"].ToString()) == false) cid.Add(dr["CompanyID"].ToString());
                    if (sid.Contains(dr["SecurityGroupID"].ToString()) == false) sid.Add(dr["SecurityGroupID"].ToString());
                    if (contractid.Contains(dr["ContractID"].ToString()) == false) contractid.Add(dr["ContractID"].ToString());
                }

                foreach(string ss in cid)
                {
                    p_strCompanyID += (p_strCompanyID == "" ? ss : "," + ss);
                }
                foreach (string ss in sid)
                {
                    p_strSecurityGroupID += (p_strSecurityGroupID == "" ? ss : "," + ss);
                }
                foreach (string ss in contractid)
                {
                    p_strContractID += (p_strContractID == "" ? ss : "," + ss);
                }
            }
        }

        #endregion  ExtensionMethod
    }
}