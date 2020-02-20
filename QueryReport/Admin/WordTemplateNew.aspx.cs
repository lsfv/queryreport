using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using System.Text;
using QueryReport.Code;

namespace QueryReport.Admin
{
    public partial class WordTemplateNew : LoginUserPage
    {
        /*
        private CUSTOMRP.Model.WORDTEMPLATE myWordTemplate;
        private int id;

        CUSTOMRP.BLL.REPORTGROUP bllrpGroup = new CUSTOMRP.BLL.REPORTGROUP();
        CUSTOMRP.BLL.SensitivityLevel bllSensitivityLevel = new CUSTOMRP.BLL.SensitivityLevel();

        private void Page_Init(object sender, EventArgs e)
        {
            this.lblJavascript.Text = String.Empty;

            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_wordtemplate, "Add", me.LoginID) == false)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "wordtemplate.aspx");
                    Response.End();
                }
                this.Button2.Visible = false;
            }
            else
            {
                if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_wordtemplate, "Modify", me.LoginID) == false)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.accesserror, "wordtemplate.aspx");
                    Response.End();
                }
                id = Int32.Parse(Request.QueryString["id"]);
                myWordTemplate = WebHelper.bllWORDTEMPLATE.GetModel(id, -1);
                if (myWordTemplate == null)
                {
                    Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.parameter_error, "wordtemplate.aspx");
                    Response.End();
                }
            }

            //this.ddlQueryType.Items.Add(new ListItem("View", "0"));
            //this.ddlQueryType.Items.Add(new ListItem("Stored Procedure", "2"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.FillddlTemplate();

                if (myWordTemplate != null)
                {
                    this.ddlTemplate.SelectedValue = myWordTemplate.ViewID.ToString();
                    this.txtDesc.Text = myWordTemplate.Description.ToString();
                    this.lbWordFile.Visible = true;
                    this.lbWordFile.Text = myWordTemplate.TemplateFileName;
                }
                else
                {
                    this.txtDesc.Text = "";
                    this.lbWordFile.Visible = false;
                }
                CUSTOMRP.Model.WORDTEMPLATE template = WebHelper.bllWORDTEMPLATE.GetTemplateList(me.DatabaseID, -1, -1)
                    .Where(x => x.ViewID == Convert.ToInt32(this.ddlTemplate.SelectedValue)).FirstOrDefault();
                if (template != null)
                {
                    this.txtQueryName.Text = template.SOURCEVIEWNAME;
                    this.txtQueryLevel.Text = template.VIEWLEVEL;
                    this.txtDatafileName.Text = template.DataFileName;
                    this.btnDownloadTemplate.Visible = File.Exists(g_Config["WordTemplatePath"] + template.TemplateFileName);
                }
                else
                {
                    this.txtQueryName.Text = "";
                    this.txtQueryLevel.Text = "";
                    this.txtDatafileName.Text = "";
                    this.btnDownloadTemplate.Visible = false;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            DateTime currTime = DateTime.Now;
            string path = g_Config["WordTemplatePath"];
            //v1.0.0 - Cheong - 2015/03/30 - Add code to create word template folder when not exist.
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }


            if (myWordTemplate == null)
            {
                bool fileOK = false;

                if (FileUpload1.HasFile)
                {
                    String fileExtension = System.IO.Path.GetExtension(FileUpload1.FileName).ToLower();
                    String[] allowedExtensions = { ".docx" };
                    for (int i = 0; i < allowedExtensions.Length; i++)
                    {
                        if (fileExtension == allowedExtensions[i])
                        {
                            fileOK = true;
                        }
                    }

                    if (fileOK==false)
                    {
                        Common.JScript.Alert(AppNum.ErrorMsg.onlyWordFileIsAllowed);
                        Common.JScript.GoHistory(-1);
                        Response.End();
                    } 
                }
                else
                {
                    fileOK = false;
                    Common.JScript.Alert(String.Format(AppNum.ErrorMsg.fieldcannotbeempty, "Template file"));
                    Common.JScript.GoHistory(-1);
                    Response.End();
                }
                if (fileOK)
                {
                    try
                    {
                        FileUpload1.SaveAs(path + FileUpload1.FileName);
                        //LabMessage1.Text = "文件上传成功.";
                        //LabMessage2.Text = "<b>原文件路径：</b>" + FileUpload1.PostedFile.FileName + "<br />" +
                        //              "<b>文件大小：</b>" + FileUpload1.PostedFile.ContentLength + "字节<br />" +
                        //              "<b>文件类型：</b>" + FileUpload1.PostedFile.ContentType + "<br />";
                        string ViewID = ddlTemplate.SelectedValue;
                        string Description = this.txtDesc.Text.Trim();
                        string WordFile = this.FileUpload1.FileName;
                        CUSTOMRP.Model.WORDTEMPLATE myWORDTEMPLATE = new CUSTOMRP.Model.WORDTEMPLATE();
                        myWORDTEMPLATE.ViewID = Int32.Parse(ViewID);
                        myWORDTEMPLATE.TemplateFileName = WordFile;
                        myWORDTEMPLATE.DataFileName = ddlTemplate.SelectedItem.Text.Replace(' ', '_') + ".xlsx";
                        myWORDTEMPLATE.Description = Description;
                        myWordTemplate.ModifyDate = currTime;
                        myWordTemplate.ModifyUser = me.ID;
                        myWordTemplate.CreateDate = currTime;
                        myWordTemplate.CreateUser = me.ID;
                        int ID = WebHelper.bllWORDTEMPLATE.Replace(myWORDTEMPLATE);
                        Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "wordtemplatenew.aspx?ID=" + ID + "");
                    }
                    catch// (Exception ex)
                    {
                        //LabMessage1.Text = "文件上传不成功.";
                        Common.JScript.Alert("File upload failed.");
                        Common.JScript.GoHistory(-1);
                        Response.End();
                    }
                }
            }
            else
            {
                bool fileOK = true;

                if (FileUpload1.HasFile)
                {
                    String fileExtension = System.IO.Path.GetExtension(FileUpload1.FileName).ToLower();
                    String[] allowedExtensions = { ".docx" };
                    for (int i = 0; i < allowedExtensions.Length; i++)
                    {
                        if (fileExtension == allowedExtensions[i])
                        {
                            fileOK = true;
                        }
                    }
                    if (fileOK==false)
                    {
                        Common.JScript.Alert("Only ..docx type file can be uploaded.");
                        Common.JScript.GoHistory(-1);
                        Response.End();
                    } 
                }
                if (fileOK)
                {
                    try
                    {
                        string WordFile = "";
                        if (FileUpload1.HasFile)
                        {
                            FileUpload1.SaveAs(path + FileUpload1.FileName.Replace(' ', '_'));
                            WordFile = this.FileUpload1.FileName.Replace(' ', '_');
                        }
                        else
                        {
                            WordFile = this.lbWordFile.Text.Trim();
                        }
                        string WordFileID = id.ToString();
                        string ViewID = ddlTemplate.SelectedValue;
                        string Description = this.txtDesc.Text.Trim();             

                        CUSTOMRP.Model.WORDTEMPLATE myWORDTEMPLATE = new CUSTOMRP.Model.WORDTEMPLATE();
                        myWORDTEMPLATE.WordTemplateName = ddlTemplate.SelectedItem.Text;
                        myWORDTEMPLATE.ViewID = Int32.Parse(ViewID);
                        myWORDTEMPLATE.TemplateFileName = WordFile;
                        myWORDTEMPLATE.DataFileName = ddlTemplate.SelectedItem.Text.Replace(' ', '_') + ".xlsx";
                        myWORDTEMPLATE.Description = Description;
                        myWordTemplate.ModifyDate = currTime;
                        myWordTemplate.ModifyUser = me.ID;
                        myWordTemplate.CreateDate = currTime;
                        myWordTemplate.CreateUser = me.ID;
                        WebHelper.bllWORDTEMPLATE.Replace(myWORDTEMPLATE);
                        Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "wordtemplatenew.aspx?ID=" + id + "");
                    }
                    catch// (Exception ex)
                    {
                        Common.JScript.Alert(AppNum.ErrorMsg.uploadFailed);
                        Common.JScript.GoHistory(-1);
                        Response.End();
                    }
                }
                
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("wordtemplate.aspx");
        }

        protected void Delete(object sender, EventArgs e)
        {
            if (me.checkUserGroupRight(CUSTOMRP.Model.APPModuleID.usergroupright_wordtemplate, "Delete", me.LoginID) == false)
            {
                Common.JScript.Alert(AppNum.ErrorMsg.accesserror);
                Common.JScript.GoHistory(-1);
                Response.End();
            }

            if (id != 0)
            {
                WebHelper.bllWORDTEMPLATE.Delete(id);
                string sql_delete = "delete from [report] where [DATABASEID]='" + me.DatabaseID + "' and [type]=2 and [RPTITLE]='" + id + "'";
                WebHelper.bllCommon.executesql(sql_delete);
                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.success, "wordtemplate.aspx");
                Response.End();
            }
        }

        protected void ddlTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            //DataTable myDt = WebHelper.bllWORDTEMPLATE.GetWordTemplateList(me.DatabaseID, Int32.Parse(this.ddlQueryType.SelectedValue));
            //for (int i = 0; i < myDt.Rows.Count; i++)
            //{
            //    DataRow dr = myDt.Rows[i];
            //    if (dr["VIEWID"].ToString() == this.ddlTemplate.SelectedValue)
            //    {
            //        this.txtQueryName.Text = dr["SOURCEVIEWNAME"].ToString();
            //        this.txtQueryLevel.Text = dr["VIEWLEVEL"].ToString();
            //        this.txtDatafileName.Text = dr["DATAFILENAME"].ToString();
            //    }
            //}
            CUSTOMRP.Model.WORDTEMPLATE template = WebHelper.bllWORDTEMPLATE.GetTemplateList(me.DatabaseID, -1, -1)
                .Where(x => x.ViewID == Convert.ToInt32(this.ddlTemplate.SelectedValue)).FirstOrDefault();
            if (template != null)
            {
                this.txtQueryName.Text = template.SOURCEVIEWNAME;
                this.txtQueryLevel.Text = template.VIEWLEVEL;
                this.txtDesc.Text = template.Description;
                this.txtDatafileName.Text = template.DataFileName;
                this.btnDownloadTemplate.Visible = File.Exists(g_Config["WordTemplatePath"] + template.TemplateFileName);
            }
            else
            {
                this.txtQueryName.Text = "";
                this.txtQueryLevel.Text = "";
                this.txtDesc.Text = "";
                this.txtDatafileName.Text = "";
                this.btnDownloadTemplate.Visible = false;
            }
        }

        protected void btnDownloadSample_Click(object sender, EventArgs e)
        {
            string fileName = "";
            string fileFullName = "";
            if (Request.QueryString["id"] != null)
            {
                fileFullName = "../file/" + this.lbWordFile.Text.Trim();
                fileName = this.lbWordFile.Text.Trim();
            }
            else
            {
                //DataTable myDt = WebHelper.bllWORDTEMPLATE.GetWordTemplateList(me.DatabaseID, -1);
                //for (int i = 0; i < myDt.Rows.Count; i++)
                //{
                //    DataRow dr = myDt.Rows[i];
                //    if (dr["VIEWID"].ToString() == this.ddlTemplate.SelectedValue)
                //    {
                //        fileFullName = "../file/" + dr["TemplateFileName"].ToString().Trim();
                //        fileName = dr["TemplateFileName"].ToString().Trim();
                //        break;
                //    }
                //}
                CUSTOMRP.Model.WORDTEMPLATE template = WebHelper.bllWORDTEMPLATE.GetTemplateList(me.DatabaseID, 2, -1)
                    .Where(x => x.ViewID == Convert.ToInt32(this.ddlTemplate.SelectedValue)).FirstOrDefault();
                if (template != null)
                {
                    fileFullName = "../file/" + template.TemplateFileName;
                    fileName = template.TemplateFileName;
                }
            }
            //FileStream fileStream = new FileStream(Server.MapPath(fileFullName), FileMode.Open);
            //long fileSize = fileStream.Length;
            string downloadFilename = fileName.Replace(' ', '_');
            MemoryStream filestream = MailMerge.ChangeDataFilePath(Server.MapPath(fileFullName), this.txtDatafileName.Text);

            Context.Response.ContentType = "application/octet-stream";
            //Context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + downloadFilename  + "\"");
            Context.Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"; filename*=utf-8''{1}", downloadFilename, HttpUtility.UrlPathEncode(downloadFilename)));
            Context.Response.AddHeader("Content-Length", filestream.Length.ToString());

            byte[] fileBuffer = new byte[filestream.Length];
            filestream.Read(fileBuffer, 0, (int)filestream.Length);
            filestream.Close();
            Context.Response.BinaryWrite(fileBuffer);
            Context.Response.End();
        }

        protected void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            string fileName = "";
            string fileFullName = "";
            if (Request.QueryString["id"] != null)
            {
                fileFullName = g_Config["WordTemplatePath"] + this.lbWordFile.Text.Trim();
                fileName = this.lbWordFile.Text.Trim();
            }
            
            if ((fileFullName == "") || !(System.IO.File.Exists(fileFullName)))
            {
                //DataTable myDt = WebHelper.bllWORDTEMPLATE.GetWordTemplateList(me.DatabaseID, -1);
                //for (int i = 0; i < myDt.Rows.Count; i++)
                //{
                //    DataRow dr = myDt.Rows[i];
                //    if (dr["VIEWID"].ToString() == this.ddlTemplate.SelectedValue)
                //    {
                //        fileFullName = g_Config["WordTemplatePath"] + dr["TemplateFileName"].ToString().Trim();
                //        fileName = dr["TemplateFileName"].ToString().Trim();
                //        break;
                //    }
                //}
                CUSTOMRP.Model.WORDTEMPLATE template = WebHelper.bllWORDTEMPLATE.GetTemplateList(me.DatabaseID, 2, -1)
                    .Where(x => x.ViewID == Convert.ToInt32(this.ddlTemplate.SelectedValue)).FirstOrDefault();
                if (template != null)
                {
                    fileFullName = g_Config["WordTemplatePath"] + template.TemplateFileName;
                    fileName = template.TemplateFileName;
                }
            }
            //FileStream fileStream = new FileStream(Server.MapPath(fileFullName), FileMode.Open);
            //long fileSize = fileStream.Length;

            if ((fileFullName == "") || !(System.IO.File.Exists(fileFullName)))
            {
                this.lblJavascript.Text = String.Format("<script type=\"text/javascript\">alert(\"{0}\");</script>", AppNum.ErrorMsg.filenotfounderror);
            }
            else
            {
                string downloadFilename = fileName.Replace(' ', '_');
                MemoryStream filestream = MailMerge.ChangeDataFilePath(fileFullName, this.txtDatafileName.Text);
                Context.Response.ContentType = "application/octet-stream";
                //Context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + downloadFilename + "\"");
                Context.Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"; filename*=utf-8''{1}", downloadFilename, HttpUtility.UrlPathEncode(downloadFilename)));
                Context.Response.AddHeader("Content-Length", filestream.Length.ToString());
                byte[] fileBuffer = new byte[filestream.Length];
                filestream.Read(fileBuffer, 0, (int)filestream.Length);
                filestream.Close();
                Context.Response.BinaryWrite(fileBuffer);
                Context.Response.End();
            }
        }

        protected void btnDownloadSampleData_Click(object sender, EventArgs e)
        {
            string fileName = this.txtDatafileName.Text;
            int svid = -1;

            CUSTOMRP.Model.WORDTEMPLATE template = WebHelper.bllWORDTEMPLATE.GetTemplateList(me.DatabaseID, 2, -1)
                .Where(x => x.ViewID == Convert.ToInt32(this.ddlTemplate.SelectedValue)).FirstOrDefault();
            if (template != null)
            {
                svid = template.ViewID;
            }

            //DataTable myDt = WebHelper.bllWORDTEMPLATE.GetWordTemplateList(me.DatabaseID, -1);
            //for (int i = 0; i < myDt.Rows.Count; i++)
            //{
            //    DataRow dr = myDt.Rows[i];
            //    if (dr["VIEWID"].ToString() == this.ddlTemplate.SelectedValue)
            //    {
            //        svid = Convert.ToInt32(dr["ViewID"]);
            //        break;
            //    }
            //}
            if (svid == -1)
            {
                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.parameter_error, "wordtemplate.aspx");
                Response.End();
            }

            //v1.2.0 - Cheong - 2016/07/04 - Add option to hide duplicate items - do not show duplicate data when getting sample
            DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(svid, me.DatabaseNAME, " TOP 5 * ", CUSTOMRP.BLL.AppHelper.sql_plus(svid, me), "", "", true);

            #region old code for TAB

            //StringBuilder sb = new StringBuilder();
            //DataColumn[] cols = new DataColumn[dt.Columns.Count];
            //dt.Columns.CopyTo(cols, 0);
            //sb.Append(String.Join("\t", Array.ConvertAll(cols, x => x.ColumnName)));
            //foreach (DataRow dr in dt.Rows)
            //{
            //    sb.AppendFormat("\r{0}", String.Join("\t", Array.ConvertAll(dr.ItemArray, x => AppHelper.FormatData(x))));
            //}

            #endregion

            NPOI.XSSF.UserModel.XSSFWorkbook XSSFworkbook = NPOIHelper.GetWorkbookAsMailMergeDataSource(dt);

            string folder = PathHelper.getTempFolderName();
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string filePath = folder + PathHelper.getSafePath(fileName);
            try
            {
                using (FileStream outFs = new FileStream(filePath, FileMode.Create))
                {
                    XSSFworkbook.Write(outFs);
                    //CA2202
                    //outFs.Close();
                }

                string downloadFilename = fileName.Replace(' ', '_');
                FileInfo file = new System.IO.FileInfo(filePath);

                Response.ContentType = "application/octet-stream";
                //Response.AddHeader("Content-Disposition", "attachment; filename=\"" + downloadFilename + "\"");
                Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"; filename*=utf-8''{1}", downloadFilename, HttpUtility.UrlPathEncode(downloadFilename)));
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.WriteFile(file.FullName);
                Response.Flush();
                Context.Response.End();
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

        }

        protected void ddlQueryType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.FillddlTemplate();
        }

        private void FillddlTemplate()
        {
            try
            {
                //var myDt = WebHelper.bllWORDTEMPLATE.GetTemplateList(me.DatabaseID, 2, me.ID).Select(x => new {
                //    NEWDesc = x.WordTemplateName,
                //    VIEWID = x.ViewID,
                //}).ToArray();

                DataTable myDt = WebHelper.bllSOURCEVIEW.GetQueryListForDropdown(me, 2).Tables[0];

                this.ddlTemplate.DataSource = myDt;
                this.ddlTemplate.DataTextField = "NEWDesc";
                this.ddlTemplate.DataValueField = "ID";
                this.ddlTemplate.DataBind();
            }
            catch
            {
                // do nothing if cannot be selected.
            }
        }
        */
    }
}