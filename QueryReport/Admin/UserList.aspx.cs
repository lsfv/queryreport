using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using QueryReport.Code;

namespace QueryReport.Admin
{
    public partial class UserList : LoginUserPage
    {
        private CUSTOMRP.BLL.USER MyUser = new CUSTOMRP.BLL.USER();
        private CUSTOMRP.BLL.SOURCEVIEW MySourceView = new CUSTOMRP.BLL.SOURCEVIEW();
        private CUSTOMRP.BLL.REPORT MyReport = new CUSTOMRP.BLL.REPORT();
        private CUSTOMRP.BLL.COMMON bllcommon = new CUSTOMRP.BLL.COMMON();


        protected void Page_Init(object sender, EventArgs e)
        {
            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_user, "View", me.LoginID) == false)
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
                DataTable dt3 = MyUser.GetCustomList(me.ID, me.DatabaseID); //DBUtilityLB.DbHelper.ExecuteTable(System.Data.CommandType.Text, "select * from [USER]", new SqlParameter() { });
                this.Repeater3.DataSource = dt3;
                this.Repeater3.DataBind();
            }
        }

        protected string getRPGroup(string rg)
        {
            string result = "";
            string sql = "";
            if (rg != "")
            {
                if (rg.LastIndexOf(",") == rg.Length-1)
                {
                    rg = rg.Substring(0, rg.Length - 1);
                }
                sql = "select [name] from [reportgroup] where [id] in (" + rg + ")";
                DataTable mydt = bllcommon.query(me.ID, sql);
                foreach (DataRow dr in mydt.Rows)
                {
                    result += dr[0]+" ";
                }
            }
            return result;
        }

     

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("UserNew.aspx", false);
        }

        protected void EDIT(object sender, EventArgs e)
        {
            Button myb = (Button)sender;
            string id = myb.CommandArgument;
            Response.Redirect("UserNew.aspx?id=" + id, false);
        }

        protected void setup_Click(object sender, EventArgs e)
        {
            Response.Redirect("QuickSetup.aspx", true);
        }

        protected string getLevel(long right)
        {
            string result = "";
            List<int> rights= Common.Utils.getList2N(right);

            if (rights.Contains(0))
            {
                result = "Add, ";
            }
            if (rights.Contains(1))
            {
                result = result+"Modify, ";
            }
            if (rights.Contains(2))
            {
                result = result + "Delete, ";
            }
            if (rights.Contains(3))
            {
                result = result + "View";
            }

            return result;
        }

    }
}