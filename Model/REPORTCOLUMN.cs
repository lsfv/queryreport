using System;

namespace CUSTOMRP.Model
{
    [Serializable]
    public partial class REPORTCOLUMN : ReportCriteria
    {
        public REPORTCOLUMN()
        { }
        #region Model
        private int _id;
        //private int _rpid;
        //private string _columnname;
        private int _columnfunc;
        //private string _criteria1;
        //private string _criteria2;
        //private string _criteria3;
        //private string _criteria4;
        private DateTime? _audodate = DateTime.Now;
        private int? _sourceviewcolumnid;
        private string _displayname;
        private int _columntype;
        private string _columncomment;
        private int? _formulafieldid;
        private bool _hidden;
        private string _svcdisplayname;
        private int _seq;
        private decimal _excel_colwidth = -1;
        private int? _font_size = null;
        private bool _font_bold = false;
        private bool _font_italic = false;
        private int _horizontal_text_align = 0;
        private string _cell_format = "";
        private string _background_color = "";
        private string _font_color = "";
        private bool _is_numeric = true;
        private bool _is_ascending = true;
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            set { _id = value; }
            get { return _id; }
        }
        ///// <summary>
        ///// 
        ///// </summary>
        //public int RPID
        //{
        //    set{ _rpid=value;}
        //    get{return _rpid;}
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        //[Obsolete]
        //public string COLUMNNAME
        //{
        //    set{ _columnname=value;}
        //    get{return _columnname;}
        //}
        /// <summary>
        /// 1,CONTENT 2,CRITERIA 3,SORT ON 4,GROUP BY
        /// </summary>
        public int COLUMNFUNC
        {
            set { _columnfunc = value; }
            get { return _columnfunc; }
        }
        ///// <summary>
        ///// 
        ///// </summary>
        //public string CRITERIA1
        //{
        //    set{ _criteria1=value;}
        //    get{return _criteria1;}
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        //public string CRITERIA2
        //{
        //    set{ _criteria2=value;}
        //    get{return _criteria2;}
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        //public string CRITERIA3
        //{
        //    set{ _criteria3=value;}
        //    get{return _criteria3;}
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        //public string CRITERIA4
        //{
        //    set{ _criteria4=value;}
        //    get{return _criteria4;}
        //}
        /// <summary>
        /// 
        /// </summary>
        public DateTime? AUDODATE
        {
            set { _audodate = value; }
            get { return _audodate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? SOURCEVIEWCOLUMNID
        {
            set { _sourceviewcolumnid = value; }
            get { return _sourceviewcolumnid; }
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
        }
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
        public int SEQ
        {
            set { _seq = value; }
            get { return _seq; }
        }

        /// <summary>
        /// Excel column width in number of characters. Must be less than or equal 254 characters.
        /// </summary>
        public decimal EXCEL_COLWIDTH
        {
            set
            {
                if (value > 254) { throw new ArgumentOutOfRangeException("COLWIDTH", value, "Excel column width must be less than or equal 254 characters."); }
                _excel_colwidth = value;
            }
            get { return _excel_colwidth; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int? FONT_SIZE
        {
            set { _font_size = value; }
            get { return _font_size; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public bool FONT_BOLD
        {
            set { _font_bold = value; }
            get { return _font_bold; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool FONT_ITALIC
        {
            set { _font_italic = value; }
            get { return _font_italic; }
        }

        /// <summary>
        /// NPOI HorizontalAlignment (0-7)
        /// </summary>
        public int HORIZONTAL_TEXT_ALIGN
        {
            set { _horizontal_text_align = value; }
            get { return _horizontal_text_align; }
        }

        /// <summary>
        /// Excel Format String
        /// </summary>
        public string CELL_FORMAT
        {
            set { _cell_format = value; }
            get { return _cell_format; }
        }

        /// <summary>
        /// hex (eg. #000000)
        /// </summary>
        public string BACKGROUND_COLOR
        {
            set { _background_color = value; }
            get { return _background_color; }
        }

        /// <summary>
        /// hex (eg. #000000)
        /// </summary>
        public string FONT_COLOR
        {
            set { _font_color = value; }
            get { return _font_color; }
        }


        /// <summary>
        /// Whether a formula is numeric
        /// </summary>
        public bool IS_NUMERIC
        {
            set { _is_numeric = value; }
            get { return _is_numeric; }
        }

        /// <summary>
        /// Whether the sort order is ascending
        /// </summary>
        public bool IS_ASCENDING
        {
            set { _is_ascending = value; }
            get { return _is_ascending; }
        }

        /// <summary>
        /// Displayname from corresponding SOURCEVIEWCOLUMN, setting it for saving will have no effect.
        /// </summary>
        public string SVCDISPLAYNAME
        {
            set { _svcdisplayname = value; }
            get { return _svcdisplayname; }
        }

        #endregion Model

        #region Extension
        public enum ColumnFuncs : int
        {
            Content = 1,
            Criteria = 2,
            SortOn = 3,
            Avg = 4,
            Sum = 5,
            Group = 6,
            GroupSum = 7,
            GroupAvg = 8,
            GroupCount = 9,
            Count = 10,

            #region Column types related to table fields in word
            /// <summary>
            /// Table field names to View/SP name mapping
            /// </summary>
            TableFieldDefination = 101,
            /// <summary>
            /// Fields in master query result to be passed as parameter
            /// </summary>
            TableParameters = 102,
            /// <summary>
            /// Column names to be included
            /// </summary>
            TableContent = 103,
            #endregion Column types related to table fields in word
        }
        public ColumnFuncs ColumnFunc
        {
            get { return (ColumnFuncs)this.COLUMNFUNC; }
        }
        public enum ColumnTypes : int
        {
            Normal = 1,
            Formula = 2,
        }
        public ColumnTypes ColumnType
        {
            get { return (ColumnTypes)this.COLUMNTYPE; }
        }
        public string DisplayName
        {
            get
            {
                return !String.IsNullOrEmpty(_displayname) ? _displayname :
                    !String.IsNullOrEmpty(_svcdisplayname) ? _svcdisplayname :
                    COLUMNNAME;
            }
        }
        /// <summary>
        /// Get string to be used in Select clause in SQL statement
        /// </summary>
        public string SelectStatement
        {
            get
            {
                return this.ColumnType == ColumnTypes.Normal ? String.Format("[{0}]", this.COLUMNNAME) : String.Format("[{0}] = {1}", this.DisplayName, this._columncomment);
            }
        }
        #endregion
    }
}

