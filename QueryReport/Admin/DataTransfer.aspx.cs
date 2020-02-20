using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using QueryReport.Code;

namespace QueryReport.Admin
{
    public partial class DataTransfer : LoginUserPage
    {

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
                System.Data.DataTable mydt = new System.Data.DataTable();
                //mydt = WebHelper.bllV_DATABASE.GetList(" [STATUS]='1' and APPLICATIONID='" + me.APPLICATIONID + "' and [ID]<>'"+me.DatabaseID+"'").Tables[0];
                mydt = WebHelper.bllCompany.GetDBListWithApp(me.ID, " and db.APPLICATIONID='" + me.APPLICATIONID + "' and db.[ID]<>'" + me.DatabaseID + "'").Tables[0];
                this.DDLDESTINATION.DataSource = mydt;
                this.DDLDESTINATION.DataTextField = "NAME";
                this.DDLDESTINATION.DataValueField = "ID";
                this.DDLDESTINATION.DataBind();

                //report group
                this.CBLReportGroup.DataSource = WebHelper.bllrpGroup.GetList(me.ID, " [DATABASEID]='" + me.DatabaseID + "'").Tables[0];
                this.CBLReportGroup.DataTextField = "NAME";
                this.CBLReportGroup.DataValueField = "ID";
                this.CBLReportGroup.DataBind();
                foreach( ListItem LI in CBLReportGroup.Items)
                {
                    LI.Selected = true;
                }

                //REPORT Category
                this.CBLCATEGORY.DataSource = WebHelper.bllcategory.GetList(me.ID, " [DATABASEID]='" + me.DatabaseID + "'").Tables[0];
                this.CBLCATEGORY.DataTextField = "NAME";
                this.CBLCATEGORY.DataValueField = "ID";
                this.CBLCATEGORY.DataBind();
                foreach (ListItem LI in CBLCATEGORY.Items)
                {
                    LI.Selected = true;
                }

                //REPORT Category
                this.CBLVIEWLEVEL.DataSource = WebHelper.bllviewLevel.GetList(me.ID, " [DATABASEID]='" + me.DatabaseID + "'").Tables[0];
                this.CBLVIEWLEVEL.DataTextField = "NAME";
                this.CBLVIEWLEVEL.DataValueField = "ID";
                this.CBLVIEWLEVEL.DataBind();
                foreach (ListItem LI in CBLVIEWLEVEL.Items)
                {
                    LI.Selected = true;
                }

                //Query
                this.CBLQUERY.DataSource = WebHelper.bllSOURCEVIEW.GetList(me.ID, " [DATABASEID]='" + me.DatabaseID + "'").Tables[0];
                this.CBLQUERY.DataTextField = "SOURCEVIEWNAME";
                this.CBLQUERY.DataValueField = "ID";
                this.CBLQUERY.DataBind();
                foreach (ListItem LI in CBLQUERY.Items)
                {
                    LI.Selected = true;
                }

                //Report
                this.CBLREPORT.DataSource = WebHelper.bllReport.GetList(me.ID, 10000, " [DATABASEID]='" + me.DatabaseID + "' and [TYPE]=1", " TYPE").Tables[0];
                this.CBLREPORT.DataTextField = "REPORTNAME";
                this.CBLREPORT.DataValueField = "ID";
                this.CBLREPORT.DataBind();
                foreach (ListItem LI in CBLREPORT.Items)
                {
                    LI.Selected = true;
                }

                //Report
                this.CBLREPORT2.DataSource = WebHelper.bllReport.GetList(me.ID, 10000, " [DATABASEID]='" + me.DatabaseID + "' AND [TYPE]=2", " TYPE").Tables[0];
                this.CBLREPORT2.DataTextField = "REPORTNAME";
                this.CBLREPORT2.DataValueField = "ID";
                this.CBLREPORT2.ToolTip = "REPORTNAME";
                this.CBLREPORT2.DataBind();
                foreach (ListItem LI in CBLREPORT2.Items)
                {
                    LI.Selected = true;
                }

                //User Group
                this.CBLUSERGROUP.DataSource = WebHelper.bllUserGroup.GetList(me.ID, " [DATABASEID]='" + me.DatabaseID + "'").Tables[0];
                this.CBLUSERGROUP.DataTextField = "NAME";
                this.CBLUSERGROUP.DataValueField = "ID";
                
                this.CBLUSERGROUP.DataBind();
                foreach (ListItem LI in CBLUSERGROUP.Items)
                {
                    LI.Selected = true;
                }

                //User
                this.CBLUSER.DataSource = WebHelper.bllUSER.GetList(me.ID, " [DATABASEID]='" + me.DatabaseID + "' ").Tables[0];
                this.CBLUSER.DataTextField = "UID";
                this.CBLUSER.DataValueField = "ID";
                this.CBLUSER.DataBind();
                foreach (ListItem LI in CBLUSER.Items)
                {
                    LI.Selected = true;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_groupright, "Modify", me.LoginID) == false)
            {
                Common.JScript.Alert(AppNum.ErrorMsg.accesserror);
                Response.End();
                return;
            }

            string reportID = Common.IncWeb.CheckBoxList_GetString(this.CBLREPORT);
            if (String.IsNullOrEmpty(reportID))
            {
                reportID = Common.IncWeb.CheckBoxList_GetString(this.CBLREPORT2);
            }
            else
            {
                string reportID2 = Common.IncWeb.CheckBoxList_GetString(this.CBLREPORT2);
                if (!String.IsNullOrEmpty(reportID2))
                {
                    reportID = reportID + "," + reportID2;
                }
            }

            string userid = Common.IncWeb.CheckBoxList_GetString(this.CBLUSER);
            
            int databaseid=Int32.Parse(this.DDLDESTINATION.SelectedValue);
            StringBuilder sb = new StringBuilder();

            //CLEAR COMPANY INFO FIRST
            sb.AppendLine("DELETE FROM [USER] where [DATABASEID]='" + databaseid + "'");
            sb.AppendLine("DELETE FROM [USERGROUP] WHERE [DATABASEID]='" + databaseid + "'");
            sb.AppendLine("DELETE FROM [REPORTCOLUMN] WHERE RPID IN (SELECT ID FROM [REPORT] WHERE DATABASEID='" + databaseid + "')");
            sb.AppendLine("DELETE FROM [REPORT] WHERE DATABASEID='" + databaseid + "'");
            sb.AppendLine("DELETE FROM [SOURCEVIEW] WHERE DATABASEID='" + databaseid + "'");
            sb.AppendLine("DELETE FROM [SENSITIVITYLEVEL] WHERE DATABASEID='" + databaseid + "'");
            sb.AppendLine("DELETE FROM [RPCATEGORY] WHERE DATABASEID='" + databaseid + "'");
            sb.AppendLine("DELETE FROM [REPORTGROUP] WHERE DATABASEID='" + databaseid + "'");

            //sb.AppendLine("DELETE FROM [WORDFILE] WHERE WordTemplateID in (select [WORDTEMPLATEID2] from V_WORDFILE2 where DATABASEID='" + databaseid + "')");
            //sb.AppendLine("DELETE FROM [WORDTEMPLATE] WHERE WordTemplateID in (select [WORDTEMPLATEID2] from V_WORDFILE2 where DATABASEID='" + databaseid + "')");
            sb.AppendLine("DELETE FROM [WORDFILE] WHERE WordFileID in (SELECT WordFileID FROM WORDFILE wf INNER JOIN REPORT rp ON wf.RPID = rp.ID WHERE rp.DATABASEID = " + databaseid + ")");
            sb.AppendLine("DELETE FROM [WORDTEMPLATE] WHERE WordTemplateID in (SELECT WORDTEMPLATEID FROM WORDTEMPLATE wt INNER JOIN SOURCEVIEW sv ON wt.VIEWID = sv.ID WHERE sv.DATABASEID = " + databaseid + ")");

            //REPORT GROUP
            sb.AppendLine("insert into [REPORTGROUP] ([DATABASEID] ,[NAME] ,[DESCRIPTION]) select " + databaseid + " ,[NAME] ,[DESCRIPTION] from [REPORTGROUP] where DATABASEID='" + me.DatabaseID + "'");
            //CATEGROY
            sb.AppendLine(" insert into [RPCATEGORY] ([DATABASEID] ,[NAME] ,[DESCRIPTION]) select " + databaseid + " ,[NAME] ,[DESCRIPTION] from [RPCATEGORY] where DATABASEID='" + me.DatabaseID + "'");
            //SENSITIVITY LEVEL
            sb.AppendLine(" insert into [SENSITIVITYLEVEL] ([DATABASEID] ,[NAME] ,[DESCRIPTION],[SLEVEL]) select " + databaseid + " ,[NAME] ,[DESCRIPTION],[SLEVEL] from [SENSITIVITYLEVEL] where DATABASEID='" + me.DatabaseID + "'");
            //SOURCEVIEW
            //sb.Append(" insert into [SOURCEVIEW] ([DATABASEID],[SOURCEVIEWNAME] ,[VIEWLEVEL],[DESC] ,[NOSHOW],[LASTMODIFYUSER]) select " + databaseid + " ,[SOURCEVIEWNAME] ,[VIEWLEVEL],[DESC] ,[NOSHOW],[LASTMODIFYUSER] from [SOURCEVIEW] where DATABASEID='" + me.DatabaseID + "'");
            sb.AppendLine(" insert into [SOURCEVIEW] ([DATABASEID],[TBLVIEWNAME],[SOURCEVIEWNAME] ,[VIEWLEVEL],[DESC],[FORMATTYPE]) select " + databaseid + ",[TBLVIEWNAME],[SOURCEVIEWNAME] ,[VIEWLEVEL],[DESC],[FORMATTYPE] from [SOURCEVIEW] where DATABASEID='" + me.DatabaseID + "'");
            
            //REPORT AND REPORTCOLUMN
            sb.AppendLine("insert into [REPORT]");
            sb.AppendLine("([DATABASEID],[SVID],[REPORTNAME],[CATEGORY],[TYPE],[REPORTGROUPLIST],[RPTITLE],[ADDUSER],[DEFAULTFORMAT],[EXTENDFIELD],[PRINT_ORIENTATION]");
            sb.AppendLine(",[PRINT_FITTOPAGE],[REPORT_HEADER],[REPORT_FOOTER],[FONT_FAMILY])");
            sb.AppendLine("SELECT [DATABASEID],[SVID],[REPORTNAME],[CATEGORY],[TYPE],[REPORTGROUPLIST],[RPTITLE],[ADDUSER],[DEFAULTFORMAT],[EXTENDFIELD],[PRINT_ORIENTATION]");
            sb.AppendLine(",[PRINT_FITTOPAGE],[REPORT_HEADER],[REPORT_FOOTER],[FONT_FAMILY] FROM (");
            sb.AppendLine("SELECT " + databaseid);
            sb.AppendLine(" [DATABASEID] ,(SELECT ID FROM [SOURCEVIEW] WHERE SOURCEVIEWNAME=(SELECT SOURCEVIEWNAME FROM [SOURCEVIEW] WHERE ID=[SVID]) AND DATABASEID=" + databaseid + ") [SVID]");
            sb.AppendLine(" ,[REPORTNAME]");
            sb.AppendLine(" ,(SELECT ID FROM [RPCATEGORY] WHERE NAME=(SELECT NAME FROM [RPCATEGORY] WHERE ID=[CATEGORY])AND [DATABASEID]=" + databaseid + ") [CATEGORY]");
            sb.AppendLine(" ,[TYPE]");
            sb.AppendLine(" ,(SELECT ID FROM [REPORTGROUP] WHERE NAME=(SELECT NAME FROM [REPORTGROUP] WHERE ID=[REPORTGROUPLIST]) AND [DATABASEID]=" + databaseid + ") [REPORTGROUPLIST]");
            sb.AppendLine(" ,[RPTITLE],[ADDUSER],[DEFAULTFORMAT],[EXTENDFIELD],[PRINT_ORIENTATION]");
            sb.AppendLine(",[PRINT_FITTOPAGE],[REPORT_HEADER],[REPORT_FOOTER],[FONT_FAMILY]");
            sb.AppendLine(" FROM [REPORT] where [DATABASEID]='" + me.DatabaseID + "'");
            if (reportID == "")
            {
                sb.AppendLine(" AND 1=2 ");
            }
            else
            {
                sb.AppendLine(" AND [ID] IN (" + reportID + ")");
            }
            sb.AppendLine(") T WHERE SVID IS NOT NULL AND REPORTGROUPLIST IS NOT NULL");



            sb.AppendLine(" insert into  [REPORTCOLUMN] ([RPID],[COLUMNNAME],[COLUMNFUNC],[CRITERIA1],[CRITERIA2],[CRITERIA3],[CRITERIA4]");
            sb.AppendLine(",[AUDODATE],[SOURCEVIEWCOLUMNID],[DISPLAYNAME],[COLUMNTYPE],[COLUMNCOMMENT],[FORMULAFIELDID],[HIDDEN],[SEQ],[EXCEL_COLWIDTH]");
            sb.AppendLine(",[FONT_SIZE],[FONT_BOLD],[FONT_ITALIC],[HORIZONTAL_TEXT_ALIGN],[CELL_FORMAT],[BACKGROUND_COLOR],[FONT_COLOR])");
            sb.AppendLine(" SELECT RPID2,[COLUMNNAME],[COLUMNTYPE],[CRITERIA1],[CRITERIA2],[CRITERIA3],[CRITERIA4]");
            sb.AppendLine(",[AUDODATE],[SOURCEVIEWCOLUMNID],[DISPLAYNAME],[COLUMNTYPE],[COLUMNCOMMENT],[FORMULAFIELDID],[HIDDEN],[SEQ],[EXCEL_COLWIDTH]");
            sb.AppendLine(",[FONT_SIZE],[FONT_BOLD],[FONT_ITALIC],[HORIZONTAL_TEXT_ALIGN],[CELL_FORMAT],[BACKGROUND_COLOR],[FONT_COLOR]");
            sb.AppendLine(" From ");
            sb.AppendLine(" (");
            sb.AppendLine(" SELECT");
            sb.AppendLine(" (SELECT ID FROM [REPORT] WHERE");
            sb.AppendLine(" DATABASEID="+databaseid+" AND");
            sb.AppendLine(" REPORTNAME=(SELECT REPORTNAME FROM [REPORT] WHERE ID=[REPORTCOLUMN].[RPID])");
            sb.AppendLine(" ) as RPID2");
            sb.AppendLine(" ,[COLUMNNAME],[COLUMNTYPE] ,[CRITERIA1] ,[CRITERIA2],[CRITERIA3],[CRITERIA4]");
            sb.AppendLine(",[AUDODATE],[SOURCEVIEWCOLUMNID],[DISPLAYNAME],[COLUMNFUNC],[COLUMNCOMMENT],[FORMULAFIELDID],[HIDDEN],[SEQ],[EXCEL_COLWIDTH]");
            sb.AppendLine(",[FONT_SIZE],[FONT_BOLD],[FONT_ITALIC],[HORIZONTAL_TEXT_ALIGN],[CELL_FORMAT],[BACKGROUND_COLOR],[FONT_COLOR]");
            sb.AppendLine(" FROM [REPORTCOLUMN]");
            sb.AppendLine(" WHERE RPID IN (SELECT ID FROM [REPORT] WHERE DATABASEID="+me.DatabaseID+")");
            sb.AppendLine("  ) TEMP WHERE ISNULL(RPID2,0)<>0");

            ////USER GROUP
            sb.AppendLine("insert into [USERGROUP] ([DATABASEID],[NAME] ,[DESCRIPTION],[EXTEND]) select " + databaseid + " ,[NAME] ,[DESCRIPTION],[EXTEND] from [USERGROUP] where DATABASEID='" + me.DatabaseID + "'");

            //USER GROUP RIGHT
            sb.AppendLine("INSERT INTO GROUPRIGHT([GID],[COMPANY],[REPORTGROUP],[CATEGARY],[SECURITY],[QUERY],[USERGROUP],[USERGROUPRIGHT],[USERSETUP],[EXTEND1],[EXTEND2],[EXTEND3])");
            sb.AppendLine(" SELECT "); 
            sb.AppendLine(" (SELECT ID FROM [USERGROUP] WHERE NAME=(SELECT NAME FROM [USERGROUP] WHERE ID=[GID])AND DATABASEID="+databaseid+") AS GID2");
            sb.AppendLine(" ,[COMPANY],[REPORTGROUP],[CATEGARY],[SECURITY],[QUERY],[USERGROUP],[USERGROUPRIGHT],[USERSETUP],[EXTEND1],[EXTEND2] ,[EXTEND3]");
            sb.AppendLine(" FROM [GROUPRIGHT] where [GID] in (select [ID] from [usergroup] where [databaseid]="+me.DatabaseID+")");

            //USER 直接插入。之后更新2个column。
            sb.AppendLine(" insert into [user] ([UID],[GID],[DATABASEID],[PASSWORD],[VIEWLEVEL],[REPORTGROUPLIST],[USERGROUPLEVEL],[SETUPUSER],[REPORTRIGHT],[EMAIL],[USERGROUP],[NAME],[SENSITIVITYLEVEL])");
            sb.AppendLine(" SELECT [UID],");
            sb.AppendLine(" (SELECT ID FROM [USERGROUP] WHERE NAME=(SELECT NAME FROM [USERGROUP] WHERE ID=[GID]) AND DATABASEID='"+databaseid+"') AS GID2");
            sb.AppendLine(" ,"+databaseid+",[PASSWORD],'','',[USERGROUPLEVEL],[SETUPUSER],[REPORTRIGHT],[EMAIL],[USERGROUP],[NAME],[SENSITIVITYLEVEL]");
            sb.AppendLine(" FROM [USER] where [DATABASEID]=" + me.DatabaseID + "");
            if (userid != "")
            {
                sb.AppendLine(" and ID IN (" + userid + ")");
            }
            else
            {
                sb.AppendLine(" and 1=2");
            }

#if EnableWord
            //WORDTEMPLATE
            sb.AppendLine("INSERT [WORDTEMPLATE] ([WORDTEMPLATEName],[Description],[VIEWID],[TemplateFileName],[DataFileName])");
            sb.AppendLine(" SELECT [WORDTEMPLATEName],[Description]");
            sb.AppendLine(" ,(SELECT [ID] from [SOURCEVIEW] where [DATABASEID]='"+databaseid+"' AND [SOURCEVIEWNAME]=(SELECT [SOURCEVIEWNAME] FROM [SOURCEVIEW] WHERE [ID]=[VIEWID])) AS VIEWID2");
            sb.AppendLine(" ,[TemplateFileName],[DataFileName] FROM [WORDTEMPLATE]");
            sb.AppendLine(" WHERE [VIEWID] IN (SELECT [ID] from [SOURCEVIEW] where [DATABASEID]='" + me.DatabaseID + "')");

            //WORDFILE
            sb.AppendLine("INSERT INTO [WORDFILE] ([WordFileName],[Description],[WordTemplateID])");
            sb.AppendLine(" SELECT [WordFileName],[Description],");
            sb.AppendLine(" (SELECT [WORDTEMPLATEID2] FROM [V_WORDFILE2] AS O WHERE O.[WORDTEMPLATEName]=V.WORDTEMPLATEName AND O.DATABASEID='"+databaseid+"') AS W2");
            sb.AppendLine(" FROM V_WORDFILE2 AS V WHERE [DATABASEID]='"+me.DatabaseID+"'");
#endif
            WebHelper.bllCommon.executesql(me.ID, sb.ToString());

            string sql_user = "select * from [USER] where NAME in(select name from [USER] where DATABASEID='" + databaseid + "') and DATABASEID='" + me.DatabaseID + "'";
            System.Data.DataTable needChangeInPreData = WebHelper.bllCommon.query(me.ID, sql_user);

            string categorys = "";
            string uid = "";
            string sql_getNew="";
            DataTable dtNew = new DataTable();
            string str_new = "";
            string sql_update = "";
            foreach (DataRow dr in needChangeInPreData.Rows)
            {
                categorys = dr["REPORTGROUPLIST"].ToString();
                if (categorys != "")
                {
                    uid = dr["UID"].ToString();

                    sql_getNew = "select ID from [REPORTGROUP] where NAME in (SELECT NAME FROM [REPORTGROUP] where ID in (" + categorys + ")) and [DATABASEID]=" + databaseid + "";
                    dtNew = WebHelper.bllCommon.query(me.ID, sql_getNew);

                    for (int i = 0; i < dtNew.Rows.Count; i++)
                    {
                        if (i == 0)
                        {
                            str_new = dtNew.Rows[i]["ID"].ToString();
                        }
                        else
                        {
                            str_new += "," + dtNew.Rows[i]["ID"].ToString();
                        }
                    }
                    sql_update = "update [user] set [REPORTGROUPLIST]='" + str_new + "' where [UID]='" + uid + "' AND DATABASEID='" + databaseid + "'";
                    WebHelper.bllCommon.executesql(me.ID, sql_update);

                    //v1.1.0 - Cheong - 2016/05/18 - Not sure why it is trying to update RPTTITLE this way, but will not work because WordFileID is type int not string
                    //sb.Append(" UPDATE REPORT  SET [RPTITLE]=");
                    //sb.Append(" (SELECT [WORDFILEID] FROM [V_WORDFILE2] A ");
                    //sb.Append(" WHERE A.[WordFileName]=(SELECT [WORDFILENAME] FROM [WORDFILE] B WHERE B.WordFileID=REPORT.RPTITLE)");
                    //sb.Append(" AND A.DATABASEID='"+databaseid+"')");
                    //sb.Append(" WHERE DATABASEID='" + databaseid + "' AND TYPE='2'");
                    //WebHelper.bllCommon.executesql(sql_update);

                    Common.JScript.Alert(AppNum.ErrorMsg.success, this.Page);
                }
            }
        }
    }
}