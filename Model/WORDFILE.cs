using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CUSTOMRP.Model
{
    public partial class WORDFILE
    {
        public WORDFILE()
        { }
        #region Model
        public int WordFileID { get; set; }
        public string WordFileName { get; set; }
        public string OrigFileName { get; set; }
        public string Description { set; get; }
        public int WordTemplateID { get; set; }
        public int RPID { get; set; }
        public DateTime ModifyDate { get; set; }
        public int ModifyUser { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        #endregion Model
    }
}
