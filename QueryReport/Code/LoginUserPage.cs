using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace QueryReport.Code
{
    /// <summary>
    /// 程序所有涉及到用户登陆后的操作的web页面，都必须继承此类
    /// </summary>
    public class LoginUserPage : System.Web.UI.Page
    {
        public CUSTOMRP.Model.LoginUser me;
        //public QueryReport.Code.AppNum.AccessPage AccessPageCode;

        protected System.Collections.Specialized.NameValueCollection g_Config = System.Configuration.ConfigurationManager.AppSettings;

        public LoginUserPage()
        {
            this.Init += new EventHandler(LoginUserPage_Init);
            this.LoadComplete+=new EventHandler(LoginUserPage_LoadComplete);
        }

        public void LoginUserPage_Init(object sender, EventArgs e)
        {
            me = (new LoginUser()).CurrentUser;
            if (!QueryReport.Code.LoginUser.isLogin())
            {
                Response.Redirect("~/SignIn.aspx", false);
                Response.End();
            }
        }

        public void LoginUserPage_LoadComplete(object sender, EventArgs e)
        {
            QueryReport.Controls.mp MyMaster = (QueryReport.Controls.mp)this.Page.Master;
            string cookiemenu = (string)HttpContext.Current.Session[QueryReport.Code.AppNum.str_var_UserCookie_menuselected];
            if (cookiemenu != "")
            {
                MyMaster.ActiveMenu(cookiemenu);
            }
        }
    }
}