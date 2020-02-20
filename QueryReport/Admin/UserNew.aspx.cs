using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QueryReport.Code;

namespace QueryReport.Admin
{
    public partial class UserNew : LoginUserPage
    {
        private bool isexist;
        private int id;
        CUSTOMRP.Model.USER myUser;

        CUSTOMRP.BLL.REPORTGROUP bllrpGroup = WebHelper.bllrpGroup;
        CUSTOMRP.BLL.SensitivityLevel bllSensitivityLevel = WebHelper.bllviewLevel;
        CUSTOMRP.BLL.USER BllUser = WebHelper.bllUSER;

        private void Page_Init(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_user, "Add", me.LoginID) == false)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "userlist.aspx");
                    Response.End();
                }
                isexist = false;
                id = 0;
                myUser = null;
                this.Button2.Visible = false;
            }
            else
            {
                if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_user, "Modify", me.LoginID) == false)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "userlist.aspx");
                    Response.End();
                }

                isexist = true;
                id = Int32.Parse(Request.QueryString["id"]);
                myUser = BllUser.GetModel(me.ID, id);
                if (myUser == null)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.parameter_error, "viewlevel.aspx");
                    Response.End();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                System.Data.DataTable dt = bllSensitivityLevel.GetList(me.ID, " DATABASEID='" + me.DatabaseID + "'").Tables[0];
                System.Data.DataTable dt2 = bllrpGroup.GetList(me.ID, " DATABASEID='" + me.DatabaseID + "'").Tables[0];

                this.ddlsensitivitylevel.DataSource = dt;
                this.ddlsensitivitylevel.DataTextField = "NAME";
                this.ddlsensitivitylevel.DataValueField = "SLEVEL";
                this.ddlsensitivitylevel.DataBind();

                this.CBLReportGroup.DataSource = dt2;
                this.CBLReportGroup.DataTextField = "NAME";
                this.CBLReportGroup.DataValueField = "ID";
                this.CBLReportGroup.DataBind();

                this.DDLUSERGROUP.DataSource = WebHelper.bllUserGroup.GetList(me.ID, 1000, " DATABASEID='" + me.DatabaseID + "'", "ID");
                this.DDLUSERGROUP.DataTextField = "NAME";
                this.DDLUSERGROUP.DataValueField = "ID";
                this.DDLUSERGROUP.DataBind();

                if (isexist)
                {
                    this.RequiredFieldValidator2.Enabled = false;
                    this.RequiredFieldValidator2.Enabled = false;
                    this.RequiredFieldValidator3.Enabled = false;
                    this.RequiredFieldValidator3.Enabled = false;   
                    this.txtuid.Text = myUser.UID;
                    this.txtemail.Text = myUser.EMAIL;
                    this.txtp1.Text = "";
                    this.txtp2.Text = "";
                    this.txtusername.Text = myUser.NAME;
                    try
                    {
                        this.ddlsensitivitylevel.SelectedValue = myUser.SENSITIVITYLEVEL.Value.ToString(".00");
                    }
                    catch
                    { }
                    Common.IncWeb.CheckBoxList_LoadString(myUser.REPORTGROUPLIST, this.CBLReportGroup);
                    this.DDLUSERGROUP.SelectedValue = myUser.GID.ToString();

                    Common.IncWeb.CheckBoxList_LoadList(Common.Utils.getList2N(myUser.REPORTRIGHT) , this.CBLReportRight);
                }
                else
                {
                    this.txtuid.Text = "";
                    this.txtemail.Text = "";
                    this.txtp1.Text = "";
                    this.txtp2.Text = "";
                    this.txtusername.Text = "";
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (isexist == false)
            {
                string uid = this.txtuid.Text.Trim();
                string password = Common.Utils.MD5NET(this.txtp1.Text.Trim());
                string email = this.txtemail.Text.Trim();
                string name = this.txtusername.Text.Trim();
                decimal sensitivitylevel = Decimal.Parse(this.ddlsensitivitylevel.SelectedValue);
                int usergroup = Int32.Parse(this.DDLUSERGROUP.SelectedValue);

                if (BllUser.GetList(me.ID, "UID='" + uid + "' AND DATABASEID = '" + me.DatabaseID + "'").Tables[0].Rows.Count > 0)
                {
                    //Common.JScript.Alert(AppNum.Commonexits);
                    //Common.JScript.GoHistory(-1);
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.Commonexits, "UserList.aspx");
                    Response.End();
                }

                CUSTOMRP.Model.USER myUser = new CUSTOMRP.Model.USER();

                myUser.SETUPUSER = 0;
                myUser.DATABASEID = me.DatabaseID;
                myUser.UID = uid;
                myUser.PASSWORD = password;
                myUser.EMAIL = email;
                myUser.NAME = name;
                myUser.SENSITIVITYLEVEL = sensitivitylevel;
                myUser.GID =usergroup;

                int[] arrayrpr = Common.IncWeb.CheckBoxList_ToList(this.CBLReportRight).ToArray();
                myUser.REPORTRIGHT=(int)Common.Utils.getSum2N(arrayrpr);
                myUser.REPORTGROUPLIST = Common.IncWeb.CheckBoxList_GetString(this.CBLReportGroup);

                myUser.VIEWLEVEL = "";
                BllUser.Add(myUser);
            }
            else
            {
                string password = Common.Utils.MD5NET(this.txtp1.Text.Trim());
                string email = this.txtemail.Text.Trim();
                string name = this.txtusername.Text.Trim();
                decimal sensitivitylevel = Decimal.Parse(this.ddlsensitivitylevel.SelectedValue);
                int usergroup = Int32.Parse(DDLUSERGROUP.SelectedValue);


                myUser.DATABASEID = me.DatabaseID;
                if (this.txtp1.Text.Trim() != "")
                {
                    myUser.PASSWORD = password;
                }
                myUser.EMAIL = email;
                myUser.NAME = name;
                myUser.SENSITIVITYLEVEL = sensitivitylevel;
                myUser.GID = usergroup;
                int[] arrayrpr = Common.IncWeb.CheckBoxList_ToList(this.CBLReportRight).ToArray();
                myUser.REPORTRIGHT = (int)Common.Utils.getSum2N(arrayrpr);
                myUser.REPORTGROUPLIST = Common.IncWeb.CheckBoxList_GetString(this.CBLReportGroup);

                BllUser.Update(myUser);
            }

            Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "UserList.aspx");
            Response.End();
        }

        protected void Delete(object sender, EventArgs e)
        {
            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_user, "Delete", me.LoginID) == false)
            {
                //Common.JScript.Alert(AppNum.accesserror);
                //Common.JScript.GoHistory(-1);
                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "UserList.aspx");
                Response.End();
            }

            if (id != 0)
            {
                BllUser.Delete(me.ID, id);
                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "UserList.aspx");
                Response.End();
            }
        }

        
    }
}