using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QueryReport.Controls
{
    public partial class D3Chart : System.Web.UI.UserControl
    {
        public int cID { get; set; }
        public int Type { get; set; }
        public string chart_data { get; set; }
        public string chart_title { get; set; }
        public string bar_x_axis_label { get; set; }
        public string bar_y_axis_label { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder("<script type=\"text/javascript\">");
            sb.Append("char" + this.cID);    //var
            sb.Append(" = ");
            sb.Append(chart_data + ";</script>");
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "mydata", sb.ToString(), false);
        }

        protected void RemoveChart(object sender, EventArgs e)
        {
            //Response.Write("<script>alert('Data inserted successfully')</script>");
            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "mydata2", "<script type=\"text/javascript\">window.confirm('Do you really want to remove this chart?');</script>", false);
            //this.lblJavascript.Text = "<script type=\"text/javascript\">alert('lol');window.confirm('Do you really want to remove this chart?');</script>";
            CUSTOMRP.BLL.AppHelper.DeleteChart(1, "CUSTOMRP", cID);
            Response.Redirect("./rpportal.aspx");
        }
    }
}