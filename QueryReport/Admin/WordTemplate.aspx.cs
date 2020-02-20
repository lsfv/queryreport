using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using QueryReport.Code;

namespace QueryReport.Admin
{
    public partial class WordTemplate : LoginUserPage
    {
        /*
        private CUSTOMRP.BLL.WORDTEMPLATE MyWordTemplate = new CUSTOMRP.BLL.WORDTEMPLATE();
        private CUSTOMRP.BLL.SOURCEVIEW MySourceView = new CUSTOMRP.BLL.SOURCEVIEW();
        private CUSTOMRP.BLL.REPORT MyReport = new CUSTOMRP.BLL.REPORT();
        private CUSTOMRP.BLL.COMMON bllcommon = new CUSTOMRP.BLL.COMMON();

        protected void Page_Init(object sender, EventArgs e)
        {
            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_wordtemplate, "View", me.LoginID) == false)
            {
                Common.JScript.Alert(AppNum.ErrorMsg.accesserror);
                Common.JScript.GoHistory(-1);
                Response.End();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //DataTable dt3 = MyWordTemplate.GetWordFileList(me.DatabaseID);
                var dt3 = MyWordTemplate.GetTemplateList(Convert.ToInt32(me.DatabaseID), -1, -1);
                this.repeaterTemplate.DataSource = dt3;
                this.repeaterTemplate.DataBind();
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("wordtemplatenew.aspx", false);
        }

        protected void EDIT(object sender, EventArgs e)
        {
            Button myb = (Button)sender;
            string id = myb.CommandArgument;
            Response.Redirect("WordTemplateNew.aspx?id=" + id, false);
        }

        protected void Download(object sender, EventArgs e)
        {
            Button myb = (Button)sender;
            string fileName = g_Config["WordTemplatePath"] + myb.CommandArgument.Replace("..\\", "");
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            long fileSize = fileStream.Length;
            string downloadFilename = myb.CommandArgument.Replace(' ', '_');

            Context.Response.ContentType = "application/octet-stream";
            //Context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + downloadFilename + "\"");
            Context.Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"; filename*=utf-8''{1}", downloadFilename, HttpUtility.UrlPathEncode(downloadFilename)));
            Context.Response.AddHeader("Content-Length", fileSize.ToString());
            byte[] fileBuffer = new byte[fileSize];
            fileStream.Read(fileBuffer, 0, (int)fileSize);
            fileStream.Close();
            Context.Response.BinaryWrite(fileBuffer);
            Context.Response.End();
        }
        */
    }
}