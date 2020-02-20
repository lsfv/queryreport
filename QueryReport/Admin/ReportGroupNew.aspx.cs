using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QueryReport.Code;

namespace QueryReport.Admin
{
    public partial class ReportGroupNew : LoginUserPage
    {
        private CUSTOMRP.BLL.REPORTGROUP bllsensitivitylevel = new CUSTOMRP.BLL.REPORTGROUP();
        private bool isexist;
        private int id;
        private CUSTOMRP.Model.REPORTGROUP myrpgroup;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_reportgroup, "Add", me.LoginID) == false)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "ReportGroup.aspx");
                    Response.End();
                }
                isexist = false;
                id = 0;
                this.Button2.Visible = false;
            }
            else
            {
                if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_reportgroup, "Modify", me.LoginID) == false)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "ReportGroup.aspx");
                    Response.End();
                }
                isexist = true;
                id = Int32.Parse(Request.QueryString["id"]);
                myrpgroup = bllsensitivitylevel.GetModel(me.ID, id);
                if (myrpgroup == null)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.parameter_error, "ReportGroup.aspx");
                    Response.End();
                }
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (isexist == false)
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


            if (isexist == false)
            {
                if (bllsensitivitylevel.GetList(me.ID, "NAME='" + name + "'").Tables[0].Rows.Count > 0)
                {
                    //Common.JScript.Alert(AppNum.Commonexits);
                    //Common.JScript.GoHistory(-1);
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.Commonexits, "ReportGroup.aspx");
                    Response.End();
                }

                CUSTOMRP.Model.REPORTGROUP mylevel = new CUSTOMRP.Model.REPORTGROUP();
                mylevel.DATABASEID = me.DatabaseID;
                mylevel.DESCRIPTION = description;
                mylevel.NAME = name;

                bllsensitivitylevel.Add(me.ID, mylevel);
            }
            else
            {
                CUSTOMRP.Model.REPORTGROUP mylevel = bllsensitivitylevel.GetModel(me.ID, id);
                mylevel.DESCRIPTION = description;
                mylevel.NAME = name;

                bllsensitivitylevel.Update(me.ID, mylevel);
            }

            Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "ReportGroup.aspx");
            Response.End();

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_reportgroup, "Delete", me.LoginID) == false)
            {
                //Common.JScript.Alert(AppNum.accesserror);
                //Common.JScript.GoHistory(-1);
                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "ReportGroup.aspx");
                Response.End();
            }

            bllsensitivitylevel.Delete(me.ID, id);
            Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "ReportGroup.aspx");
            Response.End();
        }
    }
}