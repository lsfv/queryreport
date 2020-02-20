using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QueryReport.Code;

namespace QueryReport.Admin
{
    public partial class UserGroup : LoginUserPage
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_usergroup, "View",me.LoginID) == false)
            {
                //Common.JScript.Alert(AppNum.accesserror);
                //Common.JScript.GoHistory(-1);
                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "../Report/rplist.aspx");
                Response.End();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                System.Data.DataTable MYDT = WebHelper.bllUserGroup.GetList(me.ID, 1000, " DATABASEID='" + me.DatabaseID + "'", "NAME").Tables[0];
                this.Repeater3.DataSource = MYDT;
                this.Repeater3.DataBind();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("UserGroupNew.aspx", false);
        }

        protected void EDIT(object sender, EventArgs e)
        {
            string id = ((Button)sender).CommandArgument;
            Response.Redirect("UserGroupNew.aspx?ID=" + id, false);
        }
    }
}