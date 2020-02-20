using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QueryReport.Controls
{
    public partial class CriteriaQueryParam : System.Web.UI.UserControl
    {
        private string _controlType = "string";

        public string ColumnName { get; set; }
        public string DisplayName { get; set; }
        public string[] EnumArr { get; set; }
        public string Value { get; set; }

        public string ControlType
        {
            get { return _controlType; }
            set
            {
                switch (value.ToLower())
                {
                    case "bool":
                        _controlType = "bool";
                        break;
                    case "int":
                        _controlType = "int";
                        break;
                    case "string":
                        _controlType = "string";
                        break;
                    case "datetime":
                        _controlType = "datetime";
                        break;
                    case "enum":
                        _controlType = "enum";
                        break;
                }
            }
        }

        public string op1 { get; set; }
        public string range1 { get; set; }
        public string range2 { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.ControlType == "bool")
            {
                this.DisplayName += "?";
            }
            else
            {
                this.DisplayName += ":";
            }
        }
    }
}