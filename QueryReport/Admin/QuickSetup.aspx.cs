using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace QueryReport.Admin
{
    public partial class QuickSetup : QueryReport.Code.LoginUserPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.lblJavascript.Text = String.Empty;
            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_user, "Modify", me.LoginID) == false)
            {
                //Common.JScript.Alert(QueryReport.Code.AppNum.accesserror);
                //Common.JScript.GoHistory(-1);
                Common.JScript.AlertAndRedirect(QueryReport.Code.AppNum.ErrorMsg.accesserror, "UserList.aspx");
                Response.End();
            }
            if (!IsPostBack)
            {
                this.Repeater3.DataSource = dtUser(me.DatabaseID, me.DatabaseNAME);
                this.Repeater3.DataBind();
            }
        }

        public void BINDING(object Sender, RepeaterItemEventArgs e)
        {
            DataTable MYDT =(DataTable)((Repeater)Sender).DataSource;

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.EditItem)
            {
                int index_row = e.Item.ItemIndex;
                DropDownList myDrop = e.Item.FindControl("DDLQUERYLEVEL") as DropDownList;
                if (myDrop.Items.Count == 0)
                {
                    System.Data.DataTable dt = QueryReport.Code.WebHelper.bllviewLevel.GetList(me.ID, " DATABASEID='" + me.DatabaseID + "'").Tables[0];
                    myDrop.DataSource = dt;
                    myDrop.DataTextField = "NAME";
                    myDrop.DataValueField = "SLEVEL";

                    myDrop.DataBind();
                    myDrop.SelectedValue = MYDT.Rows[index_row]["SENSITIVITYLEVEL"].ToString();
                }

                DropDownList DDLUG = e.Item.FindControl("DDLUSERGROUP") as DropDownList;
                if (DDLUG.Items.Count == 0)
                {
                    System.Data.DataTable USERGROUP = QueryReport.Code.WebHelper.bllUserGroup.GetList(me.ID, " DATABASEID='" + me.DatabaseID + "'").Tables[0];
                    DDLUG.DataSource = USERGROUP;
                    DDLUG.DataTextField = "NAME";
                    DDLUG.DataValueField = "ID";

                    DDLUG.DataBind();
                    DDLUG.SelectedValue = MYDT.Rows[index_row]["GID"].ToString();
                }

                CheckBoxList CBLRG = e.Item.FindControl("CBLREPORTGROUP") as CheckBoxList;
                if (CBLRG.Items.Count == 0)
                {
                    System.Data.DataTable USERGROUP = QueryReport.Code.WebHelper.bllrpGroup.GetList(me.ID, " DATABASEID='" + me.DatabaseID + "'").Tables[0];
                    CBLRG.DataSource = USERGROUP;
                    CBLRG.DataTextField = "NAME";
                    CBLRG.DataValueField = "ID";

                    CBLRG.DataBind();
                    Common.IncWeb.CheckBoxList_LoadString(MYDT.Rows[index_row]["REPORTGROUPLIST"].ToString(), CBLRG);
                }


                CheckBoxList CBLRA = e.Item.FindControl("CBLReportRight") as CheckBoxList;
                Common.IncWeb.CheckBoxList_LoadList(Common.Utils.getList2N(long.Parse(MYDT.Rows[index_row]["REPORTRIGHT"].ToString())), CBLRA);
            }
        }

        public DataTable dtUser(int databaseid,string databasename)
        {
            DataTable result = null;
            decimal lowestVIEWLevel = Decimal.Parse(QueryReport.Code.WebHelper.bllviewLevel.GetList(me.ID, 1, "DATABASEID=" + databaseid, "SLEVEL DESC").Tables[0].Rows[0]["Slevel"].ToString());
            int lowestGroupLevel = Int32.Parse(QueryReport.Code.WebHelper.bllUserGroup.GetList(me.ID, 1, "DATABASEID=" + databaseid, "ID DESC").Tables[0].Rows[0]["ID"].ToString());
            string lowestReportGroup = "";
            long reportView = 8;

            string sql_insertUser = "insert into [user]([UID] ,[GID] ,[DATABASEID],[PASSWORD] ,[VIEWLEVEL] ,[REPORTGROUPLIST] ,[USERGROUPLEVEL] ,[SETUPUSER] ,[REPORTRIGHT] ,[EMAIL] ,[USERGROUP]";
            sql_insertUser += " ,[NAME],[SENSITIVITYLEVEL],[AUTOTIME]) SELECT   [Username] COLLATE Chinese_PRC_CI_AS , " + lowestGroupLevel + ", " + databaseid + ",[PasswordHash] COLLATE Chinese_PRC_CI_AS,'', '" + lowestReportGroup + "', '', -1,";
            sql_insertUser += " " + reportView + ",[Email] COLLATE Chinese_PRC_CI_AS, '', [Fullname] COLLATE Chinese_PRC_CI_AS, " + lowestVIEWLevel + " from "+databasename+"..[t_user]";
            sql_insertUser += ", getdate() where [Username] COLLATE Chinese_PRC_CI_AS  NOT IN (SELECT [UID]   FROM [USER] WHERE DATABASEID=" + databaseid + ")";

            QueryReport.Code.WebHelper.bllCommon.executesql(me.ID, sql_insertUser);

            string sql = @"SELECT  [UID],[NAME],[SENSITIVITYLEVEL],[EMAIL],[GID],[REPORTGROUPLIST],[REPORTRIGHT],[PASSWORD],[SETUPUSER]";
            sql+=" FROM [USER] WHERE DATABASEID='"+databaseid+"' order by id desc";

            result = QueryReport.Code.WebHelper.bllCommon.query(me.ID, sql);
            return result;
        }

        protected void save_Click(object sender, EventArgs e)
        {
            foreach(RepeaterItem ri in this.Repeater3.Items)
            {
                if(ri.ItemType==ListItemType.Item||ri.ItemType==ListItemType.AlternatingItem)
                {
                    Literal uid = ri.FindControl("uid") as Literal;
                    DropDownList viewLevel = ri.FindControl("DDLQUERYLEVEL") as DropDownList;
                    DropDownList USERGROUP = ri.FindControl("DDLUSERGROUP") as DropDownList;
                    CheckBoxList REPORTGROUP = ri.FindControl("CBLREPORTGROUP") as CheckBoxList;
                    CheckBoxList ReportRight = ri.FindControl("CBLReportRight") as CheckBoxList;

                    string struid = uid.Text;
                    int gid = Int32.Parse(USERGROUP.SelectedValue);
                    decimal vlevel = Decimal.Parse(viewLevel.SelectedValue);
                    string strreportGroup = Common.IncWeb.CheckBoxList_GetString(REPORTGROUP);
                    IList<int> strReportRight = Common.IncWeb.CheckBoxList_ToList(ReportRight);
                    long intReportRight= Common.Utils.getSum2N(strReportRight.ToArray());

                    CUSTOMRP.Model.USER myuser = QueryReport.Code.WebHelper.bllUSER.GetModel(me.ID, struid, me.DatabaseID);
                    myuser.SENSITIVITYLEVEL = vlevel;
                    myuser.REPORTGROUPLIST = strreportGroup;
                    myuser.REPORTRIGHT = (int)intReportRight;
                    myuser.GID = gid;

                    QueryReport.Code.WebHelper.bllUSER.Update(myuser);

                }
            }

            //Server.Transfer("quicksetup.aspx", false);
            Common.JScript.AlertAndRedirect(QueryReport.Code.AppNum.ErrorMsg.success, "UserList.aspx");
            Response.End();
            //this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert({0});</script>", QueryReport.Code.AppNum.success);
        }

        protected void back_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Report/rplist.aspx", true);
        }


    }
}