using System;

namespace CUSTOMRP.Model
{
    public class ERRORLOG
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        public DateTime CreateDate { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public int? ReportID { get; set; }

        public string ReportName { get; set; }

        public string ColumnName { get; set; }
    }
}
