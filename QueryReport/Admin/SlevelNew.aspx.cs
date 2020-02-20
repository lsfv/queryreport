using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QueryReport.Code;

namespace QueryReport.Admin
{
    public partial class SlevelNew : LoginUserPage
    {
        private CUSTOMRP.BLL.SensitivityLevel bllsensitivitylevel = new CUSTOMRP.BLL.SensitivityLevel();
        private bool isexist;
        int id;
        private CUSTOMRP.Model.SensitivityLevel myviewLevel;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_securitylevel, "Add", me.LoginID) == false)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "Slevel.aspx");
                    Response.End();
                }
                isexist = false;
                id = 0;
                this.Button2.Visible = false;
            }
            else
            {
                if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_securitylevel, "Modify", me.LoginID) == false)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "Slevel.aspx");
                    Response.End();
                }
                isexist = true;
                id = Int32.Parse(Request.QueryString["id"]);
                myviewLevel = bllsensitivitylevel.GetModel(me.ID, id);
                if (myviewLevel == null)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.parameter_error, "Slevel.aspx");
                    Response.End();
                }
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (isexist==false)
                {
                    System.Data.DataTable MYDT = bllsensitivitylevel.GetList(me.ID, 1000, " DATABASEID='" + me.DatabaseID + "'", "SLEVEL").Tables[0];
                    if (MYDT.Rows.Count <= 0)
                    {
                        this.DropDownList1.Items.Clear();
                        this.DropDownList1.Items.Add(new ListItem("N/A", "0"));
                    }
                    else
                    {
                        this.DropDownList1.Items.Clear();
                        this.DropDownList1.DataSource = MYDT;
                        this.DropDownList1.DataTextField = "NAME";
                        this.DropDownList1.DataValueField = "SLEVEL";
                        this.DropDownList1.DataBind();
                        this.DropDownList1.SelectedIndex = MYDT.Rows.Count - 1;
                    }
                }
                else
                {
                    CUSTOMRP.Model.SensitivityLevel mylevel = bllsensitivitylevel.GetModel(me.ID, id);
                    this.TextBox1.Text = mylevel.NAME;
                    this.TextBox2.Text = mylevel.DESCRIPTION;
                    this.DropDownList1.Items.Add(new ListItem(mylevel.NAME,mylevel.SLEVEL.Value.ToString()));
                    this.DropDownList1.Visible = false;
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
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.Commonexits, "Slevel.aspx");
                    Response.End();
                }

                CUSTOMRP.Model.SensitivityLevel mylevel = new CUSTOMRP.Model.SensitivityLevel();
                mylevel.DATABASEID = me.DatabaseID;
                mylevel.DESCRIPTION = description;
                mylevel.NAME = name;
                decimal slevel;
                if (this.DropDownList1.SelectedIndex == this.DropDownList1.Items.Count - 1)//Last option
                {
                    slevel = Decimal.Parse(this.DropDownList1.SelectedValue) + 16;
                }
                else
                {
                    decimal d1 = Decimal.Parse(this.DropDownList1.SelectedValue);
                    decimal d2 = Decimal.Parse(this.DropDownList1.Items[this.DropDownList1.SelectedIndex + 1].Value);

                    slevel = (d1 + d2) / 2;
                }

                mylevel.SLEVEL = slevel;

                bllsensitivitylevel.Add(me.ID, mylevel);
                
            }
            else
            {
                CUSTOMRP.Model.SensitivityLevel mylevel = bllsensitivitylevel.GetModel(me.ID, id);
                mylevel.DESCRIPTION = description;
                mylevel.NAME = name;

                bllsensitivitylevel.Update(me.ID, mylevel);
            }

            Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "Slevel.aspx");
            Response.End();
        }

        protected void Delete(object sender, EventArgs e)
        {

            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_securitylevel, "Delete", me.LoginID) == false)
            {
                //Common.JScript.Alert(AppNum.accesserror);
                //Common.JScript.GoHistory(-1);
                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "Slevel.aspx");
                Response.End();
            }

            if (id != 0)
            {
                bllsensitivitylevel.Delete(me.ID, id);
                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "Slevel.aspx");
                Response.End();
            }
        }
    }
}