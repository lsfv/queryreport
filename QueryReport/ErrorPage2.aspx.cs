using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace QueryReport
{
    public partial class ErrorPage2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["message"]))
            {
                this.Label1.Text = "Error Msg:" + Server.UrlDecode(Request.QueryString["message"]);
            }
        }
    }
}