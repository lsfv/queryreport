using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QueryReport
{
    public partial class rpselecttype : QueryReport.Code.LoginUserPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Choose(object sender, EventArgs e)
        {
            string choose = Request.Form["rd1"];
            if (choose == "1")
            {
                Response.Redirect("rpexcel.aspx", false);
            }
            else
            {
                //v1.6.9 - Cheong - 2016/05/31 - Enable upload page
                //Response.Redirect("rpworddetail.aspx", false);
                Response.Redirect("rpword.aspx", false);
            }
        }

    }
}