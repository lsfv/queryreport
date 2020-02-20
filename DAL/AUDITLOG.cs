using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CUSTOMRP.DAL
{
    public class AUDITLOG
    {
        private static bool LogDebugMessage = false;

        static AUDITLOG()
        {
            // Static constructor will load once when the AppDomain loads
            LogDebugMessage = System.Configuration.ConfigurationManager.AppSettings["LogDebugMessage"] == "Y";
        }

        public static void Add(CUSTOMRP.Model.AUDITLOG auditlog)
        {
            const string strSQL = "INSERT INTO AUDITLOG (UserID, CreateDate, MessageType, ModuleName, Message, MessageDetail) VALUES (@UserID, @CreateDate, @MessageType, @ModuleName, @Message, @MessageDetail)";

            try
            {
                DbHelperSQL.ExecuteSql(auditlog.UserID, strSQL,
                    new SqlParameter("@UserID", auditlog.UserID),
                    new SqlParameter("@CreateDate", auditlog.CreateDate),
                    new SqlParameter("@MessageType", auditlog.MessageType),
                    new SqlParameter("@ModuleName", auditlog.ModuleName),
                    new SqlParameter("@Message", auditlog.Message),
                    new SqlParameter("@MessageDetail", auditlog.MessageDetail)
                    );
            }
            catch
            {
                AddEvent(auditlog);
            }
        }

        private static void AddEvent(CUSTOMRP.Model.AUDITLOG auditlog)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("UserID = {0}\r\n", auditlog.UserID);
            sb.AppendFormat("Time = {0:yyyy/MM/dd HH:mm:ss}\r\n", auditlog.CreateDate);
            sb.AppendFormat("Type = {0}\r\n", Enum.GetName(typeof(CUSTOMRP.Model.AUDITLOG.Severity), auditlog.MessageType));
            sb.AppendFormat("Module = {0}\r\n", auditlog.ModuleName);
            sb.AppendFormat("Message = {0}\r\n", auditlog.Message);
            sb.AppendFormat("Detail = {0}\r\n", auditlog.MessageDetail);

            using (System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog())
            {
                appLog.Source = "Application";
                appLog.WriteEntry(sb.ToString(), (auditlog.MessageType <= CUSTOMRP.Model.AUDITLOG.Severity.Warning) ? System.Diagnostics.EventLogEntryType.SuccessAudit : System.Diagnostics.EventLogEntryType.FailureAudit);
            }
        }
    }

    public static class AUDITLOGHelper
    {
        #region Extension Methods

        /// <summary>
        /// Create AUDITLOG object and set MessageDetail according to object difference
        /// </summary>
        /// <param name="newvalue">Model holding new values</param>
        /// <param name="oldvalue">Model holding original values, can be null if it's insert action</param>
        /// <returns></returns>
        internal static Model.AUDITLOG GetAuditLogObject(this Model.REPORT newvalue, Model.REPORT oldvalue)
        {
            Model.AUDITLOG result = new Model.AUDITLOG();
            PropertyInfo[] pinfo = typeof(Model.REPORT).GetProperties();
            StringBuilder sb = new StringBuilder();

            if (oldvalue == null)
            {
                // new record
                foreach (PropertyInfo p in pinfo)
                {
                    switch (p.Name)
                    {
                        case "TYPE":
                        case "EXTENDFIELD":
                        case "PRINT_ORIENTATION":
                        case "REPORT_HEADER":
                            {
                                // skip these properties
                            }
                            break;
                        case "ReportColumns":
                            {
                                PropertyInfo[] rcinfo = typeof(Model.REPORTCOLUMN).GetProperties();
                                for (int i = 0; i < newvalue.ReportColumns.Count; i++)
                                {
                                    foreach (PropertyInfo rcp in rcinfo)
                                    {
                                        switch (rcp.Name)
                                        {
                                            case "COLUMNFUNC":
                                            case "COLUMNTYPE":
                                            case "SelectStatement":
                                                {
                                                    // skip these properties
                                                }
                                                break;
                                            default:
                                                {
                                                    sb.AppendFormat("ReportColumns[{0}].{1} = {2}\r\n", i, rcp.Name, rcp.GetValue(newvalue.ReportColumns[i], null));
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                            break;
                        case "WordFile":
                            {
                                if (newvalue.WordFile != null)
                                {
                                    PropertyInfo[] wfinfo = typeof(Model.WORDFILE).GetProperties();
                                    foreach (PropertyInfo wfp in wfinfo)
                                    {
                                        sb.AppendFormat("WordFile.{0} = {1}\r\n", wfp.Name, wfp.GetValue(newvalue.WordFile, null));
                                    }
                                }
                                else
                                {
                                    sb.Append("WordFile = (null)\r\n");
                                }
                            }
                            break;
                        default:
                            {
                                sb.AppendFormat("{0} = {1}\r\n", p.Name, p.GetValue(newvalue, null));
                            }
                            break;
                    }
                }
            }
            else
            {
                // old record
                foreach (PropertyInfo p in pinfo)
                {
                    switch (p.Name)
                    {
                        case "TYPE":
                        case "EXTENDFIELD":
                        case "PRINT_ORIENTATION":
                        case "REPORT_HEADER":
                            {
                                // skip these properties
                            }
                            break;
                        case "ReportColumns":
                            {
                                PropertyInfo[] rcinfo = typeof(Model.REPORTCOLUMN).GetProperties();
                                #region Check for new and updated columns
                                Model.REPORTCOLUMN oldrc = null;
                                for (int i = 0; i < newvalue.ReportColumns.Count; i++)
                                {
                                    oldrc = oldvalue.ReportColumns.Where(x => (x.COLUMNFUNC == newvalue.ReportColumns[i].COLUMNFUNC)
                                        && (x.COLUMNTYPE == newvalue.ReportColumns[i].COLUMNTYPE)
                                        && (x.DisplayName == newvalue.ReportColumns[i].DisplayName)).FirstOrDefault();
                                    if (oldrc == null)
                                    {
                                        // new report column
                                        foreach (PropertyInfo rcp in rcinfo)
                                        {
                                            switch (rcp.Name)
                                            {
                                                case "COLUMNFUNC":
                                                case "COLUMNTYPE":
                                                case "SelectStatement":
                                                    {
                                                        // skip these properties
                                                    }
                                                    break;
                                                default:
                                                    {
                                                        sb.AppendFormat("ReportColumns[{0}].{1} = {2}\r\n", i, rcp.Name, rcp.GetValue(newvalue.ReportColumns[i], null));
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // modified column
                                        foreach (PropertyInfo rcp in rcinfo)
                                        {
                                            switch (rcp.Name)
                                            {
                                                case "COLUMNFUNC":
                                                case "COLUMNTYPE":
                                                case "SelectStatement":
                                                    {
                                                        // skip these properties
                                                    }
                                                    break;
                                                default:
                                                    {
                                                        if (Convert.ToString(rcp.GetValue(newvalue.ReportColumns[i], null)) != Convert.ToString(rcp.GetValue(oldrc, null)))
                                                        {
                                                            sb.AppendFormat("ReportColumns[{0}].{1} = {2} -> {3}\r\n", i, rcp.Name, rcp.GetValue(oldrc, null), rcp.GetValue(newvalue.ReportColumns[i], null));
                                                        }
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }
                                #endregion Check for new and updated columns

                                #region Check for delete columns
                                int deletecounter = 0;
                                foreach (Model.REPORTCOLUMN item in oldvalue.ReportColumns)
                                {
                                    if (newvalue.ReportColumns.Where(x => (x.COLUMNFUNC == item.COLUMNFUNC)
                                        && (x.COLUMNTYPE == item.COLUMNTYPE)
                                        && (x.DisplayName == item.DisplayName)).Count() == 0)
                                    {
                                        foreach (PropertyInfo rcp in rcinfo)
                                        {
                                            switch (rcp.Name)
                                            {
                                                case "COLUMNFUNC":
                                                case "COLUMNTYPE":
                                                case "SelectStatement":
                                                    {
                                                        // skip these properties
                                                    }
                                                    break;
                                                default:
                                                    {
                                                        sb.AppendFormat("ReportColumns[deleted {0}].{1} = {2}\r\n", deletecounter, rcp.Name, rcp.GetValue(item, null));
                                                    }
                                                    break;
                                            }
                                        }
                                        deletecounter++;
                                    }
                                }
                                #endregion
                            }
                            break;
                        case "WordFile":
                            {
                                if (newvalue.WordFile != null)
                                {
                                    if (oldvalue.WordFile != null)
                                    {
                                        PropertyInfo[] wfinfo = typeof(Model.WORDFILE).GetProperties();
                                        foreach (PropertyInfo wfp in wfinfo)
                                        {
                                            if (Convert.ToString(wfp.GetValue(newvalue.WordFile, null)) != Convert.ToString(wfp.GetValue(oldvalue.WordFile, null)))
                                            {
                                                sb.AppendFormat("WordFile.{0} = {1} -> {2}\r\n", wfp.Name, wfp.GetValue(oldvalue.WordFile, null), wfp.GetValue(newvalue.WordFile, null));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // new Wordfile entry
                                        PropertyInfo[] wfinfo = typeof(Model.WORDFILE).GetProperties();
                                        foreach (PropertyInfo wfp in wfinfo)
                                        {
                                            sb.AppendFormat("WordFile.{0} = {1}\r\n", wfp.Name, wfp.GetValue(newvalue.WordFile, null));
                                        }
                                    }

                                }
                                else
                                {
                                    if (oldvalue.WordFile != null)
                                    {
                                        sb.Append("WordFile = (removed)\r\n");
                                    }
                                }
                            }
                            break;
                        default:
                            {
                                if (Convert.ToString(p.GetValue(newvalue, null)) != Convert.ToString(p.GetValue(oldvalue, null)))
                                {
                                    sb.AppendFormat("{0} = {1} -> {2}\r\n", p.Name, p.GetValue(oldvalue, null), p.GetValue(newvalue, null));
                                }
                            }
                            break;
                    }
                }
            }

            result.MessageDetail = sb.ToString();

            return result;
        }

        /// <summary>
        /// Create AUDITLOG object and set MessageDetail according to object difference
        /// </summary>
        /// <param name="newvalue">Model holding new values</param>
        /// <param name="oldvalue">Model holding original values, can be null if it's insert action</param>
        /// <returns></returns>
        internal static Model.AUDITLOG GetAuditLogObject(this Model.SOURCEVIEW newvalue, Model.SOURCEVIEW oldvalue)
        {
            Model.AUDITLOG result = new Model.AUDITLOG();
            PropertyInfo[] pinfo = typeof(Model.SOURCEVIEW).GetProperties();
            StringBuilder sb = new StringBuilder();


            if (oldvalue == null)
            {
                // new record
                foreach (PropertyInfo p in pinfo)
                {
                    switch (p.Name)
                    {
                        case "SOURCETYPE":
                        case "FORMATTYPE":
                            {
                                // skip these properties
                            }
                            break;
                        default:
                            {
                                sb.AppendFormat("{0} = {1}\r\n", p.Name, p.GetValue(newvalue, null));
                            }
                            break;
                    }
                }
            }
            else
            {
                // old record
                foreach (PropertyInfo p in pinfo)
                {
                    switch (p.Name)
                    {
                        case "SOURCETYPE":
                        case "FORMATTYPE":
                            {
                                // skip these properties
                            }
                            break;
                        default:
                            {
                                if (p.GetValue(newvalue, null) != p.GetValue(oldvalue, null))
                                {
                                    sb.AppendFormat("{0} = {1} -> {2}\r\n", p.Name, p.GetValue(oldvalue, null), p.GetValue(newvalue, null));
                                }
                            }
                            break;
                    }
                }
            }

            result.MessageDetail = sb.ToString();

            return result;
        }

        /// <summary>
        /// Create AUDITLOG object and set MessageDetail according to object difference.
        /// </summary>
        /// <param name="newvalue">Model holding new values</param>
        /// <param name="oldvalue">Model holding original values, can be null if it's insert action</param>
        /// <returns></returns>
        /// <remarks>Because this has to be done in BLL, we need to expose this static method.</remarks>
        public static Model.AUDITLOG GetAuditLogObject(this Model.SOURCEVIEWCOLUMN[] newvalue, Model.SOURCEVIEWCOLUMN[] oldvalue)
        {
            Model.AUDITLOG result = new Model.AUDITLOG();
            PropertyInfo[] svcinfo = typeof(Model.SOURCEVIEWCOLUMN).GetProperties();
            StringBuilder sb = new StringBuilder();

            if (oldvalue == null)
            {
                // new record
                for (int i = 0; i < newvalue.Length; i++)
                {
                    foreach (PropertyInfo svcp in svcinfo)
                    {
                        switch (svcp.Name)
                        {
                            case "COLUMNTYPE":
                            case "DisplayName":
                            case "GetText":
                                {
                                    // skip these properties
                                }
                                break;
                            default:
                                {
                                    sb.AppendFormat("SOURCEVIEWCOLUMN[{0}].{1} = {2}\r\n", i, svcp.Name, svcp.GetValue(newvalue[i], null));
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                // old record
                #region Check for new and updated columns
                Model.SOURCEVIEWCOLUMN oldsvc = null;
                for (int i = 0; i < newvalue.Length; i++)
                {
                    oldsvc = oldvalue.Where(x => (x.COLUMNTYPE == newvalue[i].COLUMNTYPE)
                        && (x.COLUMNNAME == newvalue[i].COLUMNNAME)).FirstOrDefault();
                    if (oldsvc == null)
                    {
                        // new report column
                        foreach (PropertyInfo svcp in svcinfo)
                        {
                            switch (svcp.Name)
                            {
                                case "COLUMNTYPE":
                                case "DisplayName":
                                case "GetText":
                                    {
                                        // skip these properties
                                    }
                                    break;
                                default:
                                    {
                                        sb.AppendFormat("SOURCEVIEWCOLUMN[{0}].{1} = {2}\r\n", i, svcp.Name, svcp.GetValue(newvalue[i], null));
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        // modified column
                        foreach (PropertyInfo svcp in svcinfo)
                        {
                            switch (svcp.Name)
                            {
                                case "COLUMNTYPE":
                                case "DisplayName":
                                case "GetText":
                                    {
                                        // skip these properties
                                    }
                                    break;
                                default:
                                    {
                                        if (Convert.ToString(svcp.GetValue(newvalue[i], null)) != Convert.ToString(svcp.GetValue(oldsvc, null)))
                                        {
                                            sb.AppendFormat("SOURCEVIEWCOLUMN[{0}].{1} = {2} -> {3}\r\n", i, svcp.Name, svcp.GetValue(oldsvc, null), svcp.GetValue(newvalue[i], null));
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
                #endregion Check for new and updated columns

                #region Check for delete columns
                int deletecounter = 0;
                foreach (Model.SOURCEVIEWCOLUMN item in oldvalue)
                {
                    if (newvalue.Where(x => (x.COLUMNTYPE == item.COLUMNTYPE)
                        && (x.COLUMNNAME == item.COLUMNNAME)).Count() == 0)
                    {
                        foreach (PropertyInfo svcp in svcinfo)
                        {
                            switch (svcp.Name)
                            {
                                case "COLUMNTYPE":
                                case "DisplayName":
                                case "GetText":
                                    {
                                        // skip these properties
                                    }
                                    break;
                                default:
                                    {
                                        sb.AppendFormat("SOURCEVIEWCOLUMN[deleted {0}].{1} = {2}\r\n", deletecounter, svcp.Name, svcp.GetValue(item, null));
                                    }
                                    break;
                            }
                        }
                        deletecounter++;
                    }
                }
                #endregion
            }

            result.MessageDetail = sb.ToString();

            return result;
        }

        /// <summary>
        /// Create AUDITLOG object and set MessageDetail according to object difference.
        /// </summary>
        /// <param name="newvalue">Model holding new values</param>
        /// <param name="oldvalue">Model holding original values, can be null if it's insert action</param>
        /// <returns></returns>
        public static Model.AUDITLOG GetAuditLogObject(this object newvalue, object oldvalue)
        {
            Model.AUDITLOG result = new Model.AUDITLOG();
            PropertyInfo[] pinfo = newvalue.GetType().GetProperties();
            StringBuilder sb = new StringBuilder();


            if (oldvalue == null)
            {
                // new record
                foreach (PropertyInfo p in pinfo)
                {
                    switch (p.Name)
                    {
                        default:
                            {
                                sb.AppendFormat("{0} = {1}\r\n", p.Name, p.GetValue(newvalue, null));
                            }
                            break;
                    }
                }
            }
            else
            {
                // old record
                foreach (PropertyInfo p in pinfo)
                {
                    switch (p.Name)
                    {
                        default:
                            {
                                if (p.GetValue(newvalue, null) != p.GetValue(oldvalue, null))
                                {
                                    sb.AppendFormat("{0} = {1} -> {2}\r\n", p.Name, p.GetValue(oldvalue, null), p.GetValue(newvalue, null));
                                }
                            }
                            break;
                    }
                }
            }

            result.MessageDetail = sb.ToString();

            return result;
        }
        #endregion Extension Methods
    }
}
