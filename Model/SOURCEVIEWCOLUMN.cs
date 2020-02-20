using System;

namespace CUSTOMRP.Model
{
    [Serializable]
    public partial class SOURCEVIEWCOLUMN
    {
        public SOURCEVIEWCOLUMN()
        { }

        #region Model
        private int _id;
        private int _svid;
        private string _columnname;
        private string _displayname;
        private int _columntype;
        private string _columncomment;
        private int? _formulafieldid;
        private bool _hidden;
        private string _defaultdisplayname;
        private string _data_type;
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int SVID
        {
            set { _svid = value; }
            get { return _svid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string COLUMNNAME
        {
            set { _columnname = value; }
            get { return _columnname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DISPLAYNAME
        {
            set { _displayname = value; }
            get { return _displayname; }
        }
        /// <summary>
        /// 1,NORMAL 2,FORMULA
        /// </summary>
        public int COLUMNTYPE
        {
            set { _columntype = value; }
            get { return _columntype; }
        }
        /// <summary>
        /// Formula text to run in query, performed basic screening to remove ";" and "--"
        /// </summary>
        public string COLUMNCOMMENT
        {
            set
            {
                string temp = value.Replace(";", String.Empty);
                while (temp.IndexOf("--") > -1)
                {
                    temp = temp.Replace("--", String.Empty);
                }
                while (temp.IndexOf("/*") > -1)
                {
                    temp = temp.Replace("/*", String.Empty);
                }
                _columncomment = temp;
            }
            get { return _columncomment; }
        }/**/
        /// <summary>
        /// 
        /// </summary>
        public int? FORMULAFIELDID
        {
            set { _formulafieldid = value; }
            get { return _formulafieldid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool HIDDEN
        {
            set { _hidden = value; }
            get { return _hidden; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DEFAULTDISPLAYNAME
        {
            set { _defaultdisplayname = value; }
            get { return _defaultdisplayname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DATA_TYPE
        {
            set { _data_type = value; }
            get { return _data_type; }
        }
        #endregion Model

        #region Extension
        public enum ColumnTypes: int
        {
            Normal = 1,
            Formula = 2,
        }
        public ColumnTypes ColumnType
        {
            get { return (ColumnTypes) this.COLUMNTYPE; }
        }
        public string DisplayName
        {
            get
            {
                return !String.IsNullOrEmpty(_displayname) ?  String.Format("{0}({1})", _displayname, _columnname) :
                    !String.IsNullOrEmpty(_defaultdisplayname) ? String.Format("{0}({1})", _defaultdisplayname, _columnname) :
                    _columnname;
            }
        }
        /// <summary>
        /// Gets string representation of the object ready for use in T-SQL
        /// </summary>
        public string GetText
        {
            get
            {
                return this.ColumnType == ColumnTypes.Normal ? this.COLUMNNAME : this.COLUMNCOMMENT;
            }
        }
        #endregion
    }
}
