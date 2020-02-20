using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QueryReport.Code;

namespace QueryReport.Admin
{
    public partial class CategoryNew : LoginUserPage
    {
        protected CUSTOMRP.Model.RPCATEGORY myCategory;
        protected void Page_Init(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_reportcategory, "Add", me.LoginID) == false)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "Category.aspx");
                    Response.End();
                }

                
            }
            else
            {
                if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_reportcategory, "Modify",me.LoginID) == false)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "Category.aspx");
                    Response.End();
                }
                int id = Int32.Parse(Request.QueryString["id"]);
                myCategory = WebHelper.bllcategory.GetModel(me.ID, id);
                if (myCategory == null)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.parameter_error, "Category.aspx");
                    Response.End();
                }
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (myCategory == null)
                {
                }
                else
                {
                    this.TextBox1.Text = myCategory.NAME;
                    this.TextBox2.Text = myCategory.DESCRIPTION;
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string name = this.TextBox1.Text.Trim();
            string description = this.TextBox2.Text.Trim();


            if (myCategory == null)
            {
                if (WebHelper.bllcategory.GetList(me.ID, "NAME='" + name + "'").Tables[0].Rows.Count > 0)
                {
                    //Common.JScript.Alert(AppNum.Commonexits);
                    //Common.JScript.GoHistory(-1);
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.Commonexits, "Category.aspx");
                    Response.End();
                }

                CUSTOMRP.Model.RPCATEGORY mylevel = new CUSTOMRP.Model.RPCATEGORY();
                mylevel.DATABASEID = me.DatabaseID;
                mylevel.DESCRIPTION = description;
                mylevel.NAME = name;

                WebHelper.bllcategory.Add(me.ID, mylevel);
            }
            else
            {

                myCategory.DESCRIPTION = description;
                myCategory.NAME = name;;

                WebHelper.bllcategory.Update(me.ID, myCategory);
            }

            Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "Category.aspx");
            Response.End();

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_reportcategory, "Delete", me.LoginID) == false)
            {
                //Common.JScript.Alert(AppNum.accesserror);
                //Common.JScript.GoHistory(-1);
                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "Category.aspx");
                Response.End();
            }

            if (myCategory != null)
            {
                WebHelper.bllcategory.Delete(me.ID, myCategory.ID);
                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "Category.aspx");
                Response.End();
            }
        }
    }
}