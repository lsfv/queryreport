using System;


namespace CUSTOMRP.Model
{
    public partial class WORDTEMPLATE
    {
        public WORDTEMPLATE()
		{}
		#region Model
        public int WordTemplateID { get; set; }
        public string WordTemplateName { set; get; }
        public string Description { set; get; }
        public int ViewID { set; get; }
        public string TemplateFileName { get; set; }
        public string DataFileName { get; set; }
        public DateTime ModifyDate { get; set; }
        public int ModifyUser { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        #endregion Model

        public int FileCount { get; set; }
        public string SOURCEVIEWNAME { set; get; }
        public int DATABASEID { get; set; }
        public string SVDESC { get; set; }
        public int SOURCETYPE { get; set; }
        public string VIEWLEVEL { get; set; }
    }
}
