﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QueryReport.Code;

namespace QueryReport.Admin
{
    public partial class ReportGroup : LoginUserPage
    {
        private CUSTOMRP.BLL.REPORTGROUP bllsensitivitylevel = new CUSTOMRP.BLL.REPORTGROUP();


        protected void Page_Init(object sender, EventArgs e)
        {
            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_reportgroup, "View", me.LoginID) == false)
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
                System.Data.DataTable MYDT = bllsensitivitylevel.GetList(me.ID, 1000, " DATABASEID='" + me.DatabaseID + "'", "NAME").Tables[0];
                this.Repeater3.DataSource = MYDT;
                this.Repeater3.DataBind();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("ReportGroupNew.aspx", false);
        }

        protected void EDIT(object sender, EventArgs e)
        {
            string id = ((Button)sender).CommandArgument;
            Response.Redirect("ReportGroupNew.aspx?ID=" + id, false);
        }
    }
}