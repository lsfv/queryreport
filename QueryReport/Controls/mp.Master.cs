using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QueryReport.Controls
{
    public partial class mp : System.Web.UI.MasterPage
    {
        protected System.Collections.Specialized.NameValueCollection g_Config = System.Configuration.ConfigurationManager.AppSettings;

        public Literal msg
        {
            get { return this.literal_msg; }
            set { this.literal_msg=value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.lblCompanyName.Text = g_Config["CompanyName"];
            if (this.Page is QueryReport.Code.LoginUserPage)
            {
                this.lblUsername.InnerText = ((QueryReport.Code.LoginUserPage)this.Page).me.LoginID;
            }
        }


        public void ActiveMenu(string menuid)
        {
            //close all ul and remove all li controls .(active css)
            //main-nav 下所有的ul 去除in。li去除  active。
            this.literal_menuselect.Text = "<script>$(\"#main-nav ul\").removeClass(\"in\");$(\"#main-nav li \").removeClass(\"active\");";
            this.literal_menuselect.Text += "$(\"#" + menuid + "\").parent().addClass(\"in\");  $(\"#" + menuid + "\").addClass(\"active\");</script>";
        }

        public void linkrplista(object sender, EventArgs e)
        {
            Session[QueryReport.Code.AppNum.str_var_UserCookie_menuselected] = "menureportlist";
            Response.Redirect("~/Report/rpList.aspx");
        }

        //public void linkrpportala(object sender, EventArgs e)
        //{
        //    Session[QueryReport.Code.AppNum.str_var_UserCookie_menuselected] = "menudashboard";
        //    Response.Redirect("~/Report/rpportal.aspx");
        //}

        public void linkuserlista(object sender, EventArgs e)
        {
            Session[QueryReport.Code.AppNum.str_var_UserCookie_menuselected] = "menuuserlist";
            Response.Redirect("~/Admin/UserList.aspx");
        }

        public void linkviewlista(object sender, EventArgs e)
        {
            Session[QueryReport.Code.AppNum.str_var_UserCookie_menuselected] = "menuviewlist";
            Response.Redirect("~/Admin/ViewList.aspx");
        }


        public void linklevellista(object sender, EventArgs e)
        {
            Session[QueryReport.Code.AppNum.str_var_UserCookie_menuselected] = "menulevellist";
            Response.Redirect("~/Admin/SLevel.aspx");
        }

        public void linkusergrouplista(object sender, EventArgs e)
        {
            Session[QueryReport.Code.AppNum.str_var_UserCookie_menuselected] = "menuusergrouplist";
            Response.Redirect("~/Admin/ReportGroup.aspx");
        }

        public void linkusergroupa(object sender, EventArgs e)
        {
            Session[QueryReport.Code.AppNum.str_var_UserCookie_menuselected] = "menuusergroup";
            Response.Redirect("~/Admin/UserGroup.aspx");
        }

        public void linkgrouprighta(object sender, EventArgs e)
        {
            Session[QueryReport.Code.AppNum.str_var_UserCookie_menuselected] = "menugroupright";
            Response.Redirect("~/Admin/GroupRight.aspx");
        }

        public void linkcategorya(object sender, EventArgs e)
        {
            Session[QueryReport.Code.AppNum.str_var_UserCookie_menuselected] = "menucategory";
            Response.Redirect("~/Admin/Category.aspx");
        }

        public void linkcompanya(object sender, EventArgs e)
        {
            Session[QueryReport.Code.AppNum.str_var_UserCookie_menuselected] = "menucompany";
            Response.Redirect("~/Admin/Company.aspx");
        }

        public void linkwordlista(object sender, EventArgs e)
        {
            Session[QueryReport.Code.AppNum.str_var_UserCookie_menuselected] = "menuwordlist";
            Response.Redirect("~/Admin/WordTemplate.aspx");
        }

        public void linkdataimporta(object sender, EventArgs e)
        {
            Session[QueryReport.Code.AppNum.str_var_UserCookie_menuselected] = "menudataimport";
            Response.Redirect("~/Admin/DataTransfer.aspx");
        }
        
        protected void loginout(object sender, EventArgs e)
        {

            //菜单折叠的cookie还是要清除。登录后初始化菜单是否关闭。
            //if (Request.Cookies["openNav"] != null)
            //{
            //    HttpCookie myCookie = new HttpCookie("openNav");
            //    myCookie.Expires = DateTime.Now.AddDays(-1d);
            //    Response.Cookies.Add(myCookie);
            //}

            Session.Clear();
            // v1.0.0 - Cheong - Clear Cookies AFTER clearing session as session key is stored in cookies
            Response.Cookies.Clear();//只是说不要向客户端写入cookie了。

            Response.Redirect("~/SignIn.aspx", false);
            Response.End();
        }
    }
}