using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QueryReport.Code;

namespace QueryReport.Admin
{
    public partial class CompanyNew : LoginUserPage
    {
        private CUSTOMRP.Model.DATABASE myCompany;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_company, "Add", me.LoginID) == false)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "company.aspx");
                    Response.End();
                }
                this.Button2.Visible = false;
            }
            else
            {
                if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_company, "Modify", me.LoginID) == false)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "company.aspx");
                    Response.End();
                }
                int id = Int32.Parse(Request.QueryString["id"]);
                myCompany = WebHelper.bllCompany.GetModel(me.ID, id);
                if (myCompany == null)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.parameter_error, "company.aspx");
                    Response.End();
                }
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (myCompany == null)
                {
                }
                else
                {
                    this.TextBox1.Text = myCompany.NAME;
                    this.TextBox2.Text = myCompany.DESC;
                    this.DDLSTATUS.SelectedValue = myCompany.STATUS.ToString();
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string name = this.TextBox1.Text.Trim();
            string description = this.TextBox2.Text.Trim();
            int applicationid = Int32.Parse(this.DDLSTATUS.SelectedValue);

            if (myCompany == null)
            {
                if (WebHelper.bllCompany.GetList(me.ID, "NAME='" + name + "'").Tables[0].Rows.Count > 0)
                {
                    //Common.JScript.Alert(AppNum.Commonexits);
                    //Common.JScript.GoHistory(-1);
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.Commonexits, "Company.aspx");
                    Response.End();
                }

                CUSTOMRP.Model.DATABASE _database = new CUSTOMRP.Model.DATABASE();
                _database.NAME = name;
                _database.DESC = description;
                _database.APPLICATIONID = me.APPLICATIONID;
                _database.STATUS = applicationid;
                _database.LASTMODIFYDATE = DateTime.Now;
                _database.AUDOTIME = DateTime.Now;
                WebHelper.bllCompany.Add(_database);
            }
            else
            {
                myCompany.DESC = description;
                myCompany.NAME = name;
                myCompany.STATUS = Int32.Parse(this.DDLSTATUS.SelectedValue);

                WebHelper.bllCompany.Update(myCompany);
            }

            Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "company.aspx");
            Response.End();

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_company, "Delete", me.LoginID) == false)
            {
                //Common.JScript.Alert(AppNum.accesserror);
                //Common.JScript.GoHistory(-1);
                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "Company.aspx");
                Response.End();
            }
            WebHelper.bllCompany.Delete(me.ID, myCompany.ID);
            Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "Company.aspx");
            Response.End();
        }
    }
}