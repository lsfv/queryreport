using QueryReport.Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QueryReport.Admin
{
    public partial class DbUpdate : LoginUserPage
    {
        private CUSTOMRP.BLL.COMMON bll_common = new CUSTOMRP.BLL.COMMON();
        private CUSTOMRP.BLL.DBVersion_Script bll_dbversion = new CUSTOMRP.BLL.DBVersion_Script();


        //page_load只有2个动作，1重设某些每次都需要重置的控件 2。第一次初始化控件。
        //之后所有的控件的属性变化都是在事件中更改。
        //这个处理套路是比较简洁。不允许存在任何session和cookie.就算要缓存数据，请放到bll,ui不应该知道细节。
        protected void Page_Load(object sender, EventArgs e)
        {
            ResetControls();
            if (!IsPostBack)
            {
                //needupdate:-1, need set init version  first    .0 no need update .>0: need update
                //check weather contain dbversion's table.
                bll_common.executesql(1, CUSTOMRP.BLL.DBVersion_Script.sql_create);
                bll_common.executesql(1, CUSTOMRP.BLL.DBVersion_Script.sql_create2);

                //chekc update msg.
                int currentVersion = bll_dbversion.CurrentVersion();
                int needUpdate = bll_dbversion.GetCountNeedUpdate(currentVersion);
                if (needUpdate > 0)
                {
                    this.Label_Tip.Text = "Current version:" + currentVersion + ".      Update now.";
                    this.btn_update.Enabled = true;
                }
                else if (needUpdate == -1)
                {
                    this.Label_Tip.Text = "Need to set init verion first. please contract administartor.";
                    this.btn_update.Enabled = false;
                }
                else
                {
                    this.Label_Tip.Text = "No need update.";
                    this.btn_update.Enabled = false;
                }
            }
        }

        private void ResetControls()
        {
            this.Label_Tip.Text = "";
        }

        protected void btn_update_Click(object sender, EventArgs e)
        {
            int currentVersion = bll_dbversion.CurrentVersion();
            DataTable res = bll_dbversion.GetNeedUpdate(currentVersion);
            bool isSuccess = true;
            foreach (DataRow row in res.Rows)
            {
                try
                {
                    bll_common.executesql(1, row["script_sql"].ToString());
                }
                catch(Exception ex)
                {
                    this.Label_Tip.Text = "Error :"+ex.Message;
                    isSuccess = false;
                    break;
                }
                bll_dbversion.addhistory((int)row["script_version"],"");
            }

            if (isSuccess==false)
            {
                this.btn_update.Enabled = false;
            }
            else
            {
                this.Label_Tip.Text = "success";
                this.btn_update.Enabled = false;
            }
        }
    }
}