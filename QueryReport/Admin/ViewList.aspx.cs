using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QueryReport.Code;
namespace QueryReport.Admin
{
    public partial class ViewList : LoginUserPage
    {

        private CUSTOMRP.BLL.SOURCEVIEW bllsensitivitylevel = new CUSTOMRP.BLL.SOURCEVIEW();

        protected void Page_Init(object sender, EventArgs e)
        {
            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_query, "View", me.LoginID) == false)
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
                System.Data.DataTable MYDT = bllsensitivitylevel.GetCustomList(me.ID, " sourceview.DATABASEID='" + me.DatabaseID + "'");
                this.Repeater3.DataSource = MYDT;
                this.Repeater3.DataBind();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewNew.aspx", false);
        }

        protected void EDIT(object sender, EventArgs e)
        {
            string id = ((Button)sender).CommandArgument;
            Response.Redirect("ViewNew.aspx?ID=" + id, false);
        }
    }
}