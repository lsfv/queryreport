using System;
using System.Collections.Generic;
using System.Web;
using QueryReport.Code;

namespace QueryReport
{
    public partial class SignIn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //this.baseUrl.Attributes.Add("href", String.Format("{0}://{1}:{2}/{3}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port, Request.ApplicationPath != "/" ? Request.ApplicationPath : String.Empty));
            if (!IsPostBack)
            {
                this.lblBuildNumber.Text = "Version " + Common.IncCommon.GetBuildNumber(4);

                if (string.IsNullOrEmpty(Request.QueryString["uid"]) == false && string.IsNullOrEmpty(Request.QueryString["hash"]) == false && string.IsNullOrEmpty(Request.QueryString["dbid"]) == false && string.IsNullOrEmpty(Request.QueryString["dbname"]) == false)
                {
                    string uid = Request.QueryString["uid"].ToString();
                    string password = Request.QueryString["hash"].ToString();
                    string DATABASE = Request.QueryString["dbid"].ToString();
                    string DATABASENAME = Request.QueryString["dbname"].ToString();
                    login(uid, password, DATABASE, 0);
                }

                System.Data.DataTable mydt;
                //mydt = WebHelper.bllV_DATABASE.GetList(" [STATUS]='1'").Tables[0];
                mydt = WebHelper.bllCompany.GetDBListWithApp(-1).Tables[0];
                mydt.Columns.Add("customdesc", Type.GetType("System.String"), "APPNAME+' - '+NAME");
                this.ddlDatabase.DataSource = mydt;
                this.ddlDatabase.DataTextField = "customdesc";
                this.ddlDatabase.DataValueField = "ID";
                this.ddlDatabase.DataBind();
            }
        }

        protected void signin(object sender, EventArgs e)
        {
            string uid = this.uid.Value;
            string password = this.password.Value;
            string DATABASE = this.ddlDatabase.SelectedValue;
            string DATABASENAME = this.ddlDatabase.SelectedItem.Text;
            DATABASENAME = DATABASENAME.Substring(DATABASENAME.IndexOf("-") + 1);
            login(uid, password, DATABASE, 1);
        }


        protected void login(string uid, string password, string DATABASE, int loginType)
        {
            throw (new Exception("aaa"));
            CUSTOMRP.Model.USER myUser;

            if (loginType == 1)
            {
                myUser = WebHelper.bllUSER.GetModel(-1, uid, Int32.Parse(DATABASE), Common.Utils.MD5NET(password));
            }
            else
            {
                //v1.1.0 - Cheong - 2016/05/18 - Make hashkey configurable
                CUSTOMRP.Model.DATABASE mydb = WebHelper.bllCompany.GetModel(-1, Int32.Parse(DATABASE));
                string salt = (mydb != null) ? mydb.HASHKEY : "com";

                //string hash = Common.Utils.MD5NET(uid + DateTime.Now.ToString("yyyyMMdd") + "com");
                string hash = Common.Utils.MD5NET(uid + DateTime.Now.ToString("yyyyMMdd") + salt);

                if (mydb == null)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.FailedToConnectQueryReportDatabase, "#");
                    return;
                }

                if (hash == password)
                {
                    myUser = WebHelper.bllUSER.GetModel(-1, uid, mydb.ID);
                }
                else
                {
                    myUser = null;
                }
            }


            if (myUser != null)
            {
                //store user's information to cookie,

                CUSTOMRP.Model.DATABASE mydb = WebHelper.bllCompany.GetModel(-1, Int32.Parse(DATABASE));

                HttpContext.Current.Session[AppNum.str_var_UserCookie_uid] = uid;
                HttpContext.Current.Session[AppNum.str_var_UserCookie_logintime] = DateTime.Now.ToString("yyyyMMddhhmm");
                HttpContext.Current.Session[AppNum.str_var_UserCookie_Databaseid] = mydb.ID;
                HttpContext.Current.Session[AppNum.str_var_UserCookie_DatabaseName] = mydb.NAME;
                HttpContext.Current.Session[AppNum.str_var_UserCookie_APPLICATIONID] = mydb.APPLICATIONID;

                HttpContext.Current.Session[AppNum.str_var_UserSessionName] = myUser;

                #region Perform redirect if mode = embedded

                if (Request.Params["mode"] == "embedded")
                {
                    //v1.0.0 - Cheong - 2015/07/21 - Modify SignIn.aspx to allow embedded mode relay to relay for "action"
                    //Response.Redirect("~/Report/rpEmbedded.aspx?rpid=" + Request.Params["rpid"] + "rptType=" + (Request.Params["rptType"] ?? "1"), true);
                    List<string> pList = new List<string>();

                    if (Request.Params["action"] != null)
                    {
                        pList.Add(String.Format("action={0}", Request.Params["action"]));
                        if (Request.Params["rpid"] != null)
                        {
                            pList.Add(String.Format("rpid={0}", Request.Params["rpid"]));
                        }
                        if (Request.Params["type"] != null)
                        {
                            pList.Add(String.Format("type={0}", Request.Params["type"]));
                        }
                    }
                    else
                    {
                        if (Request.Params["rpid"] != null)
                        {
                            pList.Add(String.Format("rpid={0}&rptType={1}", Request.Params["rpid"], (Request.Params["rptType"] ?? "1")));
                        }
                    }

                    //v1.2.0 - Cheong - 2016/07/15 - Add relay on parameter rpgrp
                    if (Request.Params["rpgrp"] != null)
                    {
                        pList.Add(String.Format("rpgrp={0}", Request.Params["rpgrp"]));
                    }

                    Response.Redirect("~/Report/rpEmbedded.aspx?" + String.Join("&", pList), true);
                }

                #endregion

                Response.Redirect("~/Report/rpList.aspx");
            }
            else
            {
                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.loginerror, "#");
            }
        }
    }
}