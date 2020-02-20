using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QueryReport.Code;

namespace QueryReport.Admin
{
    public partial class Slevel : LoginUserPage
    {
        private CUSTOMRP.BLL.SensitivityLevel bllsensitivitylevel = new CUSTOMRP.BLL.SensitivityLevel();

        protected void Page_Init(object sender, EventArgs e)
        {
            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_securitylevel, "View", me.LoginID) == false)
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
                System.Data.DataTable MYDT = bllsensitivitylevel.GetList(me.ID, 1000, " DATABASEID='" + me.DatabaseID + "'", "SLEVEL").Tables[0];
                MYDT.Columns.Add(new System.Data.DataColumn("Order number", Type.GetType("System.String")));

                for (int i = 0; i < MYDT.Rows.Count; i++)
                {
                    MYDT.Rows[i]["Order number"] = (i + 1).ToString();
                }

                this.Repeater3.DataSource = MYDT;
                this.Repeater3.DataBind();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("SlevelNew.aspx", false);
        }

        protected void EDIT(object sender, EventArgs e)
        {
            string id = ((Button)sender).CommandArgument;
            Response.Redirect("SlevelNew.aspx?ID=" + id, false);
        }
    }
}