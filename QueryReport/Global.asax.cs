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
            // Code that runs when an unhandled error occurs

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
