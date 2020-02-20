using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QueryReport.Code;

namespace QueryReport.Admin
{
    public partial class GroupRight : LoginUserPage
    {
        protected int GID = 0;
        protected CUSTOMRP.Model.GROUPRIGHT myGRight = null;
        //load and save.

        protected void Page_Init(object sender, EventArgs e)
        {
            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_groupright, "View", me.LoginID) == false)
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
                System.Data.DataTable MYDT = WebHelper.bllUserGroup.GetList(me.ID, " DATABASEID='" + me.DatabaseID + "'").Tables[0];
                this.DDLUSERGROUP.DataSource = MYDT;
                this.DDLUSERGROUP.DataTextField = "NAME";
                this.DDLUSERGROUP.DataValueField = "ID";
                this.DDLUSERGROUP.DataBind();

                if (string.IsNullOrEmpty(this.DDLUSERGROUP.SelectedValue) == false)
                {
                    string STRGID = this.DDLUSERGROUP.SelectedValue;
                    GID = Int32.Parse(STRGID);
                    myGRight = WebHelper.bllGroupRight.GetModel(me.ID, GID);
                    if (myGRight != null)
                    {
                        load(myGRight);
                    }
                }
            }
        }


        protected void Button1_Click(object sender, EventArgs e)
        {
            //Add check for save
            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_groupright, "Modify", me.LoginID) == false)
            {
                Common.JScript.Alert(AppNum.ErrorMsg.accesserror);
                Common.JScript.GoHistory(-1);
                Response.End();
            }

            //save date.
            //reload date
            string company = (this.companyadd.Checked == true ? "Add," : "") + (this.companydel.Checked == true ? "Delete," : "") + (this.companymodify.Checked == true ? "Modify," : "") + (this.companyview.Checked == true ? "View," : "");
            string rg = (this.rgadd.Checked == true ? "Add," : "") + (this.rgdel.Checked == true ? "Delete," : "") + (this.rgmodify.Checked == true ? "Modify," : "") + (this.rgview.Checked == true ? "View," : "");
            string category = (this.categoryadd.Checked == true ? "Add," : "") + (this.categorydel.Checked == true ? "Delete," : "") + (this.categorymodify.Checked == true ? "Modify," : "") + (this.categoryview.Checked == true ? "View," : "");
            string sl = (this.sladd.Checked == true ? "Add," : "") + (this.sldel.Checked == true ? "Delete," : "") + (this.slmodify.Checked == true ? "Modify," : "") + (this.slview.Checked == true ? "View," : "");
            string query = (this.queryadd.Checked == true ? "Add," : "") + (this.querydel.Checked == true ? "Delete," : "") + (this.querymodify.Checked == true ? "Modify," : "") + (this.queryview.Checked == true ? "View," : "");
            string usergroup = (this.usergroupadd.Checked == true ? "Add," : "") + (this.usergroupdel.Checked == true ? "Delete," : "") + (this.usergroupmodify.Checked == true ? "Modify," : "") + (this.usergroupview.Checked == true ? "View," : "");
            string usergroupright = (this.ugradd.Checked == true ? "Add," : "") + (this.ugrdel.Checked == true ? "Delete," : "") + (this.ugrmodify.Checked == true ? "Modify," : "") + (this.ugrview.Checked == true ? "View," : "");
            string user = (this.useradd.Checked == true ? "Add," : "") + (this.userdel.Checked == true ? "Delete," : "") + (this.usermodify.Checked == true ? "Modify," : "") + (this.userview.Checked == true ? "View," : "");
            string word = (this.Wordadd.Checked == true ? "Add," : "") + (this.Worddel.Checked == true ? "Delete," : "") + (this.Wordmodify.Checked == true ? "Modify," : "") + (this.Wordview.Checked == true ? "View," : "");
            string copy = (this.Copyadd.Checked == true ? "Add," : "") + (this.Copydel.Checked == true ? "Delete," : "") + (this.Copymodify.Checked == true ? "Modify," : "") + (this.Copyview.Checked == true ? "View," : "");

            company = company == "" ? "" : company.Substring(0, company.Length - 1);
            rg = rg == "" ? "" : rg.Substring(0, rg.Length - 1);
            category = category == "" ? "" : category.Substring(0, category.Length - 1);
            sl = sl == "" ? "" : sl.Substring(0, sl.Length - 1);
            query = query == "" ? "" : query.Substring(0, query.Length - 1);
            usergroup = usergroup == "" ? "" : usergroup.Substring(0, usergroup.Length - 1);
            usergroupright = usergroupright == "" ? "" : usergroupright.Substring(0, usergroupright.Length - 1);
            user = user == "" ? "" : user.Substring(0, user.Length - 1);
            word = word == "" ? "" : word.Substring(0, word.Length - 1);
            copy = copy == "" ? "" : copy.Substring(0, copy.Length - 1);

            string STRGID = this.DDLUSERGROUP.SelectedValue;
            GID = Int32.Parse(STRGID);
            myGRight = WebHelper.bllGroupRight.GetModel(me.ID, GID);

            if (GID != 0)
            {
                if (myGRight == null)
                {
                    //add
                    myGRight = new CUSTOMRP.Model.GROUPRIGHT();
                    myGRight.GID = GID;
                    myGRight.QUERY = query;
                    myGRight.REPORTGROUP = rg;
                    myGRight.SECURITY = sl;
                    myGRight.USERSETUP = user;
                    myGRight.COMPANY = company;
                    myGRight.CATEGARY = category;
                    myGRight.USERGROUP = usergroup;
                    myGRight.USERGROUPRIGHT = usergroupright;
                    myGRight.EXTEND1 = word;
                    myGRight.EXTEND2 = copy;


                    WebHelper.bllGroupRight.Add(me.ID, myGRight);
                }
                else
                { 
                    //update
                    myGRight.GID = GID;
                    myGRight.QUERY = query;
                    myGRight.REPORTGROUP = rg;
                    myGRight.SECURITY = sl;
                    myGRight.USERSETUP = user;
                    myGRight.COMPANY = company;
                    myGRight.CATEGARY = category;
                    myGRight.USERGROUP = usergroup;
                    myGRight.USERGROUPRIGHT = usergroupright;
                    myGRight.EXTEND1 = word;
                    myGRight.EXTEND2 = copy;
                    WebHelper.bllGroupRight.Update(me.ID, myGRight);
                }
            }
            changeGID(this.Button1, new EventArgs());
            //Response.Redirect("GROUPRIGHT.ASPX");
            Common.JScript.Alert(AppNum.ErrorMsg.success);
        }


        protected void changeGID(object Sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.DDLUSERGROUP.SelectedValue) == false)
            {
                string STRGID = this.DDLUSERGROUP.SelectedValue;
                GID = Int32.Parse(STRGID);
                myGRight = WebHelper.bllGroupRight.GetModel(me.ID, GID);
                if (myGRight != null)
                {
                    load(myGRight);
                }
                else
                {
                    load(new CUSTOMRP.Model.GROUPRIGHT());
                }
            }
        }


        protected void load(CUSTOMRP.Model.GROUPRIGHT _gright)
        {
            this.companyadd.Checked = WebHelper.checkRight(_gright.COMPANY, "Add");
            this.companydel.Checked = WebHelper.checkRight(_gright.COMPANY, "Delete");
            this.companymodify.Checked = WebHelper.checkRight(_gright.COMPANY, "Modify");
            this.companyview.Checked = WebHelper.checkRight(_gright.COMPANY, "View");

            this.rgadd.Checked = WebHelper.checkRight(_gright.REPORTGROUP, "Add");
            this.rgdel.Checked = WebHelper.checkRight(_gright.REPORTGROUP, "Delete");
            this.rgmodify.Checked = WebHelper.checkRight(_gright.REPORTGROUP, "Modify");
            this.rgview.Checked = WebHelper.checkRight(_gright.REPORTGROUP, "View");

            this.categoryadd.Checked = WebHelper.checkRight(_gright.CATEGARY, "Add");
            this.categorydel.Checked = WebHelper.checkRight(_gright.CATEGARY, "Delete");
            this.categorymodify.Checked = WebHelper.checkRight(_gright.CATEGARY, "Modify");
            this.categoryview.Checked = WebHelper.checkRight(_gright.CATEGARY, "View");

            this.sladd.Checked = WebHelper.checkRight(_gright.SECURITY, "Add");
            this.sldel.Checked = WebHelper.checkRight(_gright.SECURITY, "Delete");
            this.slmodify.Checked = WebHelper.checkRight(_gright.SECURITY, "Modify");
            this.slview.Checked = WebHelper.checkRight(_gright.SECURITY, "View");

            this.queryadd.Checked = WebHelper.checkRight(_gright.QUERY, "Add");
            this.querydel.Checked = WebHelper.checkRight(_gright.QUERY, "Delete");
            this.querymodify.Checked = WebHelper.checkRight(_gright.QUERY, "Modify");
            this.queryview.Checked = WebHelper.checkRight(_gright.QUERY, "View");

            this.usergroupadd.Checked = WebHelper.checkRight(_gright.USERGROUP, "Add");
            this.usergroupdel.Checked = WebHelper.checkRight(_gright.USERGROUP, "Delete");
            this.usergroupmodify.Checked = WebHelper.checkRight(_gright.USERGROUP, "Modify");
            this.usergroupview.Checked = WebHelper.checkRight(_gright.USERGROUP, "View");

            this.useradd.Checked = WebHelper.checkRight(_gright.USERSETUP, "Add");
            this.userdel.Checked = WebHelper.checkRight(_gright.USERSETUP, "Delete");
            this.usermodify.Checked = WebHelper.checkRight(_gright.USERSETUP, "Modify");
            this.userview.Checked = WebHelper.checkRight(_gright.USERSETUP, "View");

            this.ugradd.Checked = WebHelper.checkRight(_gright.USERGROUPRIGHT, "Add");
            this.ugrdel.Checked = WebHelper.checkRight(_gright.USERGROUPRIGHT, "Delete");
            this.ugrmodify.Checked = WebHelper.checkRight(_gright.USERGROUPRIGHT, "Modify");
            this.ugrview.Checked = WebHelper.checkRight(_gright.USERGROUPRIGHT, "View");

            this.Wordadd.Checked = WebHelper.checkRight(_gright.EXTEND1, "Add");
            this.Worddel.Checked = WebHelper.checkRight(_gright.EXTEND1, "Delete");
            this.Wordmodify.Checked = WebHelper.checkRight(_gright.EXTEND1, "Modify");
            this.Wordview.Checked = WebHelper.checkRight(_gright.EXTEND1, "View");

            this.Copyadd.Checked = WebHelper.checkRight(_gright.EXTEND2, "Add");
            this.Copydel.Checked = WebHelper.checkRight(_gright.EXTEND2, "Delete");
            this.Copymodify.Checked = WebHelper.checkRight(_gright.EXTEND2, "Modify");
            this.Copyview.Checked = WebHelper.checkRight(_gright.EXTEND2, "View");
        }
    }
}