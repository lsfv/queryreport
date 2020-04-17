using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
namespace CUSTOMRP.Model
{
    //goodsMaster
    public class DBVersion_Script
    {
        private int _script_version;
        public int script_version
        {
            get { return _script_version; }
            set { _script_version = value; }
        }

        private string _script_title;
        public string script_title
        {
            get { return _script_title; }
            set { _script_title = value; }
        }

        private string _script_desc;
        public string script_desc
        {
            get { return _script_desc; }
            set { _script_desc = value; }
        }

        private string _script_sql;
        public string script_sql
        {
            get { return _script_sql; }
            set { _script_sql = value; }
        }

        private string _script_extend;
        public string script_extend
        {
            get { return _script_extend; }
            set { _script_extend = value; }
        }

        private DateTime _script_date;
        public DateTime script_date
        {
            get { return _script_date; }
            set { _script_date = value; }
        }

    }
}