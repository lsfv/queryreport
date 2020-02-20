using System;
using System.ComponentModel;

namespace CUSTOMRP.Model
{
    public class AUDITLOG
    {
        public enum Severity : int
        {
            [Description("Debug")]
            Debug = 0,
            [Description("Info")]
            Info = 1,
            [Description("Audit")]
            Audit = 2,
            [Description("Warning")]
            Warning = 3,
            [Description("Error")]
            Error = 4,
            [Description("Fatal")]
            Fatal = 5,
        }

        public int ID { get; set; }

        public int UserID { get; set; }

        public DateTime CreateDate { get; set; }

        public Severity MessageType { get; set; }

        public string ModuleName { get; set; }

        public string Message { get; set; }

        public string MessageDetail { get; set; }
    }
}
