using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

namespace QueryReport.Controls
{
    public partial class CriteriaInt : System.Web.UI.UserControl
    {
        public DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            //this.Label1.Text = ColumnName;
            //this.Label2.Text = ColumnName;
            this.Label1.Text = DisplayName ?? ColumnName;
            this.Label2.Text = DisplayName ?? ColumnName;
            InitControl(dt);
        }

        public void InitControl(DataTable dt)
        {
            this.Select1.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                this.Select1.Items.Add(new ListItem(dr["text"].ToString(), dr["value"].ToString()));
            }
        }

        public string ColumnName { get; set; }
        public string DisplayName { get; set; }
        public string prefix
        {
            get { return this.Select1.ClientID; }
        }
        public string controlType
        {
            get { return "enum"; }
        }
    }
}