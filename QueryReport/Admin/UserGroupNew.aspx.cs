using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QueryReport.Code;

namespace QueryReport.Admin
{
    public partial class UserGroupNew : LoginUserPage
    {
        private CUSTOMRP.Model.USERGROUP myrpgroup;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_usergroup, "Add", me.LoginID) == false)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "UserGroup.aspx");
                    Response.End();
                }
                this.Button2.Visible = false;
            }
            else
            {
                if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_usergroup, "Modify", me.LoginID) == false)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "UserGroup.aspx");
                    Response.End();
                }
                int id = Int32.Parse(Request.QueryString["id"]);
                myrpgroup = WebHelper.bllUserGroup.GetModel(me.ID, id);
                if (myrpgroup == null)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.parameter_error, "UserGroup.aspx");
                    Response.End();
                }
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (myrpgroup == null)
                {
                }
                else
                {
                    this.TextBox1.Text = myrpgroup.NAME;
                    this.TextBox2.Text = myrpgroup.DESCRIPTION;
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string name = this.TextBox1.Text.Trim();
            string description = this.TextBox2.Text.Trim();


            if (myrpgroup == null)
            {
                if (WebHelper.bllUserGroup.GetList(me.ID, "NAME='" + name + "'").Tables[0].Rows.Count > 0)
                {
                    //Common.JScript.Alert(AppNum.Commonexits);
                    //Common.JScript.GoHistory(-1);
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.Commonexits, "UserGroup.aspx");
                    Response.End();
                }

                CUSTOMRP.Model.USERGROUP mylevel = new CUSTOMRP.Model.USERGROUP();
                mylevel.DATABASEID = me.DatabaseID;
                mylevel.DESCRIPTION = description;
                mylevel.NAME = name;
                mylevel.EXTEND = "";

                WebHelper.bllUserGroup.Add(me.ID, mylevel);
            }
            else
            {
                CUSTOMRP.Model.USERGROUP mylevel = WebHelper.bllUserGroup.GetModel(me.ID, myrpgroup.ID);
                mylevel.DESCRIPTION = description;
                mylevel.NAME = name;
                mylevel.EXTEND = "";

                WebHelper.bllUserGroup.Update(me.ID, mylevel);
            }

            Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "UserGroup.aspx");
            Response.End();

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_usergroup, "Delete", me.LoginID) == false)
            {
                //Common.JScript.Alert(AppNum.accesserror);
                //Common.JScript.GoHistory(-1);
                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "UserGroup.aspx");
                Response.End();
            }

            if (myrpgroup != null)
            {
                WebHelper.bllUserGroup.Delete(me.ID, myrpgroup.ID);
                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "UserGroup.aspx");
                Response.End();
            }
        }
    }
}