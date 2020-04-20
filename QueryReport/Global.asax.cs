using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace QueryReport
{
    public class Global : System.Web.HttpApplication
    {
        protected System.Collections.Specialized.NameValueCollection g_Config = System.Configuration.ConfigurationManager.AppSettings;
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            CUSTOMRP.BLL.COMMON.CommandTimeout = Convert.ToInt32(g_Config["CommandTimeout"] ?? "300");  // default to 5 minutes
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
            #region perform cleanup on shutdown
            string tempfoldername = QueryReport.Code.PathHelper.getTempFolderName();
            if (!Directory.Exists(tempfoldername))
            {
                Directory.CreateDirectory(tempfoldername);
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(tempfoldername);
                FileInfo[] fi = di.GetFiles();
                for (int i = fi.Length - 1; i >= 0; i--)
                {
                    fi[i].Delete();
                }
            }
            #endregion
        }

        void Application_Error(object sender, EventArgs e)
        {
            //在出现未处理的错误时运行的代码
            Exception ex = Server.GetLastError().GetBaseException();
            string errorTime = "DataTime：" + DateTime.Now.ToString();
            string errorAddress = "Url：" + Request.Url.ToString();
            string errorInfo = "Message：" + ex.Message;
            string errorInfo1 =ex.Message;
            string errorSource = "Source：" + ex.Source;
            string errorType = "Type：" + ex.GetType();
            string errorFunction = "Function：" + ex.TargetSite;
            string errorTrace = "Trace：" + ex.StackTrace;
            Server.ClearError();
            System.IO.StreamWriter writer = null;
            try
            {
                lock (this)
                {
                    //写入日志 
                    string path = string.Empty;
                    path = Server.MapPath("~/ErrorLogs/");
                    //不存在则创建错误日志文件夹
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    path += string.Format(@"\{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));

                    writer = !File.Exists(path) ? File.CreateText(path) : File.AppendText(path); //判断文件是否存在，如果不存在则创建，存在则添加
                    writer.WriteLine("ip:" + Request.UserHostAddress);
                    writer.WriteLine(errorTime);
                    writer.WriteLine(errorAddress);
                    writer.WriteLine(errorInfo);
                    writer.WriteLine(errorSource);
                    writer.WriteLine(errorType);
                    writer.WriteLine(errorFunction);
                    writer.WriteLine(errorTrace);
                    writer.WriteLine("********************************************************************************************");
                }
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
            Response.Redirect("~/ErrorPage2.aspx?message=" + errorInfo1.Replace(" ","").Replace("    ","").Replace("\n", "").Replace("\t", "").Replace("\r", ""));
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            //Add compatibility tag for all pages
            Response.AddHeader("X-UA-Compatible", "IE=edge,chrome=1");
        }
    }
}
