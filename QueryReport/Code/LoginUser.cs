using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections;
using System.Collections.Generic;
using CUSTOMRP.Model;

namespace QueryReport.Code
{
    public class LoginUser
    {
        private const string strSessionCurrentUser = "__SESSION_CURRENTUSER";
        private CUSTOMRP.Model.LoginUser _currentUser = null;

        public CUSTOMRP.Model.LoginUser CurrentUser
        {
            get {
                if (_currentUser == null) { _currentUser = (CUSTOMRP.Model.LoginUser)HttpContext.Current.Session[strSessionCurrentUser]; }
                if (_currentUser == null) { _currentUser = Create(); }
                return _currentUser;
            }
            set {
                _currentUser = value;
                HttpContext.Current.Session[strSessionCurrentUser] = _currentUser;
            }
        }

        public CUSTOMRP.Model.LoginUser Create()
        {
            CUSTOMRP.Model.LoginUser result = null;
            if (isLogin())
            {
                //get baseinfo from cookie
                //LoginID = HttpContext.Current.Request.Cookies[QueryReport.Code.AppNum.str_var_UserCookieName].Values.Get(QueryReport.Code.AppNum.str_var_UserCookie_uid);
                //DatabaseID = HttpContext.Current.Request.Cookies[QueryReport.Code.AppNum.str_var_UserCookieName].Values.Get(QueryReport.Code.AppNum.str_var_UserCookie_Databaseid);
                //DatabaseNAME = HttpContext.Current.Request.Cookies[QueryReport.Code.AppNum.str_var_UserCookieName].Values.Get(QueryReport.Code.AppNum.str_var_UserCookie_DatabaseName);
                //APPLICATIONID = Int32.Parse(HttpContext.Current.Request.Cookies[QueryReport.Code.AppNum.str_var_UserCookieName].Values.Get(QueryReport.Code.AppNum.str_var_UserCookie_APPLICATIONID));

                string LoginID = (string)HttpContext.Current.Session[QueryReport.Code.AppNum.str_var_UserCookie_uid];
                int DatabaseID = (int)HttpContext.Current.Session[QueryReport.Code.AppNum.str_var_UserCookie_Databaseid];
                string DatabaseNAME = (string)HttpContext.Current.Session[QueryReport.Code.AppNum.str_var_UserCookie_DatabaseName];
                int APPLICATIONID = (int)HttpContext.Current.Session[QueryReport.Code.AppNum.str_var_UserCookie_APPLICATIONID];              

                //session -user info
                CUSTOMRP.Model.USER myuser = new CUSTOMRP.Model.USER();
                if (HttpContext.Current.Session[QueryReport.Code.AppNum.str_var_UserSessionName] != null)
                {
                    myuser = (CUSTOMRP.Model.USER)HttpContext.Current.Session[QueryReport.Code.AppNum.str_var_UserSessionName];
                }
                else
                {
                    CUSTOMRP.BLL.USER blluser = new CUSTOMRP.BLL.USER();
                    myuser = blluser.GetModelForUser(-1, LoginID, DatabaseID);
                    HttpContext.Current.Session[QueryReport.Code.AppNum.str_var_UserSessionName] = myuser;
                }
                //session -user group right

                CUSTOMRP.Model.GROUPRIGHT gr = null;
                if (HttpContext.Current.Session[QueryReport.Code.AppNum.str_var_UserSessionGroupRight] != null)
                {
                    gr = (CUSTOMRP.Model.GROUPRIGHT)HttpContext.Current.Session[QueryReport.Code.AppNum.str_var_UserSessionGroupRight];
                }
                else
                {
                    gr = QueryReport.Code.WebHelper.bllGroupRight.GetModel(myuser.ID, myuser.GID);
                    HttpContext.Current.Session[QueryReport.Code.AppNum.str_var_UserSessionGroupRight] = gr;
                }


                decimal ViewLevel = myuser.SENSITIVITYLEVEL == null ? 9999 : myuser.SENSITIVITYLEVEL.Value;
                int ID = myuser.ID;
                string ReportGroup = myuser.REPORTGROUPLIST;
                int REPORTRIGHT = myuser.REPORTRIGHT;

                //string companyid, securitygroupid, contractid;
                //QueryReport.Code.WebHelper.bllUSER.getUserInfo(out companyid, out securitygroupid, out contractid, LoginID, Convert.ToInt32(DatabaseID), DatabaseNAME);

                result = new CUSTOMRP.Model.LoginUser(ID, LoginID, APPLICATIONID, DatabaseID, DatabaseNAME, ViewLevel, ReportGroup, REPORTRIGHT,gr);

                if (result != null)
                {
                    List<CUSTOMRP.Model.ColumnInfo> columns = CUSTOMRP.BLL.AppHelper.GetColumnInfoForTblView(myuser.ID, result.DatabaseNAME, "v_Security")
                        .Where(x => (x.ColName != "UserName") && (x.ColName != "ID")).ToList();

                    DataTable dt = CUSTOMRP.BLL.AppHelper.getSecurityForUser(myuser.ID, result.DatabaseNAME, LoginID);

                    result.UserCriteria = new Dictionary<string, string>();

                    Dictionary<string, List<dynamic>> tempStorage = new Dictionary<string, List<dynamic>>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        foreach (CUSTOMRP.Model.ColumnInfo column in columns)
                        {
                            if (!tempStorage.ContainsKey(column.ColName))
                            {
                                tempStorage.Add(column.ColName, new List<dynamic>());
                            }

                            if (!tempStorage[column.ColName].Contains(dr[column.ColName]))
                            {
                                tempStorage[column.ColName].Add(dr[column.ColName]);
                            }
                        }
                    }

                    foreach (CUSTOMRP.Model.ColumnInfo column in columns)
                    {
                        //v1.2.0 - Cheong - 2016/07/05 - Handle case where user is not listed in v_Security
                        if ((tempStorage.ContainsKey(column.ColName)) && (tempStorage[column.ColName].Count > 0))
                        {
                            result.UserCriteria.Add(column.ColName.ToUpper(), String.Join(", ", tempStorage[column.ColName].Select(x => (column.DataType == "String") ?
                                String.Format("'{0}'", x) :
                                String.Format("{0}", x)
                            ).OrderBy(x => x).ToArray()));
                        }
                        else
                        {
                            result.UserCriteria.Add(column.ColName.ToUpper(), "NULL");
                        }
                    }

                    //v1.2.0 Fai 2016.10.28 - Store Login User to Session to prevent retrieve data on every refresh - Begin
                    CurrentUser = result;
                    //v1.2.0 Fai 2016.10.28 - Store Login User to Session to prevent retrieve data on every refresh - End
                }
            }
            return result;
        }

        public static bool isLogin()
        {
            bool result = false;
            if (HttpContext.Current.Session[QueryReport.Code.AppNum.str_var_UserCookie_uid] != null || HttpContext.Current.Session[QueryReport.Code.AppNum.str_var_UserCookie_Databaseid] != null)
            {
                result = true;
            }
            return result;
        }


    }
}