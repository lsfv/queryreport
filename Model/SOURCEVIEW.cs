using System;
namespace CUSTOMRP.Model
{
    /// <summary>
    /// SOURCEVIEW:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class SOURCEVIEW
    {
        public SOURCEVIEW()
        {}
        #region Model
        private int _id;
        private string _sourceviewname;
        private int _databaseid=1;
        private int _sourcetype;
        private string _tblviewname;
        private DateTime _audodate= DateTime.Now;
        private decimal _viewlevel=1M;
        private string _desc="";
        private int? _formattype;
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
        public string SOURCEVIEWNAME
        {
            set{ _sourceviewname=value;}
            get{return _sourceviewname;}
        }
        /// <summary>
        /// 
        /// </summary>
        public int DATABASEID
        {
            set{ _databaseid=value;}
            get{return _databaseid;}
        }
        public int SOURCETYPE
        {
            set { _sourcetype = value; }
            get { return _sourcetype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TBLVIEWNAME
        {
            set { _tblviewname = value; }
            get { return _tblviewname; }
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
        /// 1,NORMAL,2,MEDIUM,3HITH,4TOP
        /// </summary>
        public decimal VIEWLEVEL
        {
            set{ _viewlevel=value;}
            get{return _viewlevel;}
        }
        /// <summary>
        /// 
        /// </summary>
        public string DESC
        {
            set{ _desc=value;}
            get{return _desc;}
        }
        /// <summary>
        /// 
        /// </summary>
        public int? FORMATTYPE
        {
            set { _formattype = value; }
            get { return _formattype; }
        }
        #endregion Model

        public WORDTEMPLATE WordTemplate { get; set; }

        public enum SourceViewType: int
        {
            View = 0,
            Table = 1,
            StoredProc = 2,
        }

        public SourceViewType SourceType
        {
            get { return (SourceViewType)this.SOURCETYPE; }
        }

        public enum FormatTypes : int
        {
            Excel = 1,
            Word = 2,
            Both = Excel | Word,
        }

        public FormatTypes FormatType
        {
            get { return (FormatTypes)this.FORMATTYPE; }
        }
    }
}

