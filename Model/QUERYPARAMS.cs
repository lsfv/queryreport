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
    public partial class QUERYPARAMS
    {
        public QUERYPARAMS()
        { }
        #region Model
        private int _report;
        private string _name;
        private string _value;

        /// <summary>
        /// 
        /// </summary>
        public int REPORT
        {
            set { _report = value; }
            get { return _report; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string NAME
        {
            set { _name = value; }
            get { return _name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string VALUE
        {
            set { _value = value; }
            get { return _value; }
        }
        #endregion
    }
}

