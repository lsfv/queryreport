using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CUSTOMRP.Model
{
    /// <summary>
    /// REPORT:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class REPORT
    {
        public REPORT()
        {}
        #region Model
        private int _id;
        private int _databaseid=1;
        private int _svid;
        private string _reportname;
        private DateTime _audodate= DateTime.Now;
        private int _category=1;
        private int _type=1;
        private int _reportgrouplist=1;
        private string _rptitle="";
        private int _adduser=1;
        private int _defaultformat=1;
        private List<string> _extendfield = new List<string>(new string[] { "0", "1", "1" });
        private int _print_orientation = -1;
        //v1.0.0 - Cheong - 2016/02/25 - Default value for PRINT_FITTOPAGE = 1
        private short _print_fittopage = 1;
        private string _report_header = "";
        private string _report_footer = "";
        private string _subcount_label = "Sub Count";
        private string _font_family = "";
        private bool _pdf_grid_lines = false;
        
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            set{ _id=value;}
            get{return _id;}
        }
        /// <summary>
        /// 
        /// </summary>
        public int DATABASEID
        {
            set{ _databaseid=value;}
            get{return _databaseid;}
        }
        /// <summary>
        /// 
        /// </summary>
        public int SVID
        {
            set{ _svid=value;}
            get{return _svid;}
        }
        /// <summary>
        /// 
        /// </summary>
        public string REPORTNAME
        {
            set{ _reportname=value;}
            get{return _reportname;}
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime AUDODATE
        {
            set{ _audodate=value;}
            get{return _audodate;}
        }
        /// <summary>
        /// 
        /// </summary>
        public int CATEGORY
        {
            set{ _category=value;}
            get{return _category;}
        }
        /// <summary>
        /// Report Type: 1 = Excel; 2 = Word
        /// </summary>
        public int TYPE
        {
            set{ _type=value;}
            get{return _type;}
        }
        /// <summary>
        /// 
        /// </summary>
        public int REPORTGROUPLIST
        {
            set{ _reportgrouplist=value;}
            get{return _reportgrouplist;}
        }
        /// <summary>
        /// 
        /// </summary>
        public string RPTITLE
        {
            set{ _rptitle=value;}
            get{return _rptitle;}
        }
        /// <summary>
        /// 
        /// </summary>
        public int ADDUSER
        {
            set{ _adduser=value;}
            get{return _adduser;}
        }
        /// <summary>
        /// 
        /// </summary>
        public int DEFAULTFORMAT
        {
            set{ _defaultformat=value;}
            get{return _defaultformat;}
        }
        /// <summary>
        /// 
        /// </summary>
        public string EXTENDFIELD
        {
            get { return String.Join(",", _extendfield); }
            set
            {
                _extendfield = new List<string>(value.Split(',').Select(x => String.IsNullOrWhiteSpace(x) ? "0" : x));
                // initialize default
                while (_extendfield.Count < EXTENDFIELDs.Count)
                {
                    _extendfield.Add("0");
                }
            }
        }
        /// <summary>
        /// Print orientation, not to be used directly. Use PrintOrientaion instead
        /// </summary>
        public int PRINT_ORIENTATION
        {
            set { _print_orientation = value; }
            get { return _print_orientation; }
        }
        /// <summary>
        /// Number of worksheet to fit in a page. -1 = Not Set. Default: -1
        /// </summary>
        public short PRINT_FITTOPAGE
        {
            set { _print_fittopage = value; }
            get { return _print_fittopage; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string REPORT_HEADER
        {
            set { _report_header = value; }
            get { return _report_header; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string REPORT_FOOTER
        {
            set { _report_footer = value; }
            get { return _report_footer; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string SUBCOUNT_LABEL
        {
            set { _subcount_label = value; }
            get { return _subcount_label; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FONT_FAMILY
        {
            set { _font_family = value; }
            get { return _font_family; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool PDF_GRID_LINES
        {
            set { _pdf_grid_lines = value; }
            get { return _pdf_grid_lines; }
        }
        
        #endregion Model

        #region Extensions

        public List<REPORTCOLUMN> ReportColumns { get; set; }

        public WORDFILE WordFile { get; set; }

        public enum Orientation : int
        {
            NotSet = -1,
            Portrait = 1,
            Landscape = 2,
        }

        public Orientation PrintOrientation
        {
            set { _print_orientation = (int)value; }
            get { return (Orientation)_print_orientation; }
        }

        #endregion

        #region Extended Properties fields
        public abstract class ExtReportType
        {
            public const string ShowAll = "0";
            public const string ChangeOnly = "1";
            public const string DataExport = "2";
            public const string Pivotable = "3";
        }

        public abstract class EXTENDFIELDs
        {
            public const int ReportType = 0;
            public const int HideCriteria = 1;
            public const int HideDuplicate= 2;
            public const int Count = 3; // remember to set it to max enum value for initialization
        }

        /// <summary>
        /// See ExtReportType
        /// </summary>
        public string ReportType
        {
            get { return this._extendfield[EXTENDFIELDs.ReportType]; }
            set { this._extendfield[EXTENDFIELDs.ReportType] = value; }
        }

        public bool fChangeOnly
        {
            get { return this._extendfield[EXTENDFIELDs.ReportType] == ExtReportType.ChangeOnly; }
        }

        public bool fDataExport
        {
            get { return this._extendfield[EXTENDFIELDs.ReportType] == ExtReportType.DataExport; }
        }

        public bool IsPivoTable
        {
            get { return this._extendfield[EXTENDFIELDs.ReportType] == ExtReportType.Pivotable; }
        }

        public bool fHideCriteria
        {
            get { return this._extendfield[EXTENDFIELDs.HideCriteria] == "1"; }
            set { this._extendfield[EXTENDFIELDs.HideCriteria] = value ? "1" : "0"; }
        }

        public bool fHideDuplicate
        {
            get { return this._extendfield[EXTENDFIELDs.HideDuplicate] == "1"; }
            set { this._extendfield[EXTENDFIELDs.HideDuplicate] = value ? "1" : "0"; }
        }
        #endregion
    }
}

