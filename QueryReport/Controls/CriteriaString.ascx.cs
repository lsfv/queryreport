using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

namespace QueryReport.Controls
{
    public partial class CriteriaString : System.Web.UI.UserControl
    {
        private string _controlType = "string";
        private int _userID { get; set; }
        private string _dbName { get; set; }
        private string _sourceView { get; set; }
        private int _sourceType { get; set; }

        public string[] EnumArr { get; set; }
        public string ColumnName { get; set; }
        public string DisplayName { get; set; }

        public string prefix
        {
            get { return this.Label1.ClientID; }
        }
        public string ControlType
        {
            get { return _controlType; }
            set
            {
                switch (value.ToLower())
                {
                    case "string":
                        _controlType = "string";
                        break;
                    case "datetime":
                        _controlType = "datetime";
                        break;
                }
            }
        }
        public int UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }
        public string DBName
        {
            get { return _dbName; }
            set { _dbName = value; }
        }
        public string SourceView
        {
            get { return _sourceView; }
            set { _sourceView = value; }
        }
        public int SourceType
        {
            get { return _sourceType; }
            set { _sourceType = value; }
        }

        public string op1 { get; set; }
        public string range1 { get; set; }
        public string range2 { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //this.Label1.Text = ColumnName;
                this.Label1.Text = DisplayName ?? ColumnName;
            }
        }

        protected void Autocomplete_Click(object sender, EventArgs e)
        {
            EnumArr = CUSTOMRP.BLL.AppHelper.ParamGetEnumValues(_userID, _dbName, _sourceView, _sourceType, ColumnName);
            var serializer = new JavaScriptSerializer();

            this.lbLiteral.Text = "<script>$('#" + prefix + "tb1, #" + prefix + "tb2, #" + prefix + "tb3').autocomplete({minLength: 0, source: " + serializer.Serialize(EnumArr) + ", autoFocus: true }).focus(function () { if (this.value == '') { $(this).autocomplete('search', '');} });$('#" + prefix + "tb1').focus();</script>";
        }
    }
}