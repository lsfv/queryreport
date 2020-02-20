using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CUSTOMRP.DAL
{
    public abstract class AppNum
    {
        private static System.Collections.Specialized.NameValueCollection g_Config = System.Configuration.ConfigurationManager.AppSettings;

        public abstract class AuditMessage
        {
            public const string ReportInsertSuccess = "Report addition with RPID = {0} is successful.";
            public const string ReportUpdateSuccess = "Report update with RPID = {0} is successful.";
            public const string ReportDeleteSuccess = "Report deletion with RPID = {0} is successful.";
            public const string SourceViewInsertSuccess = "SourceView addition with SVID = {0} is successful.";
            public const string SourceViewUpdateSuccess = "SourceView update with SVID = {0} is successful.";
            public const string SourceViewDeleteSuccess = "SourceView deletion with SVID = {0} is successful.";
            public const string SourceViewColumnInsertSuccess = "SourceView Columns addition with SVID = {0} is successful.";
            public const string SourceViewColumnUpdateSuccess = "SourceView Columns update with SVID = {0} is successful.";
            public const string SourceViewColumnDeleteSuccess = "SourceView Columns deletion with SVID = {0} is successful.";

            public const string ApplicationInsertSuccess = "Application addition with ID = {0} is successful.";
            public const string ApplicationUpdateSuccess = "Application update with ID = {0} is successful.";
            public const string ApplicationDeleteSuccess = "Application deletion with ID = {0} is successful.";
            public const string DatabaseInsertSuccess = "Database addition with ID = {0} is successful.";
            public const string DatabaseUpdateSuccess = "Database update with ID = {0} is successful.";
            public const string DatabaseDeleteSuccess = "Database deletion with ID = {0} is successful.";
            public const string GroupRightInsertSuccess = "GroupRight addition with ID = {0} is successful.";
            public const string GroupRightUpdateSuccess = "GroupRight update with ID = {0} is successful.";
            public const string GroupRightDeleteSuccess = "GroupRight deletion with ID = {0} is successful.";
            public const string ReportGroupInsertSuccess = "ReportGroup addition with ID = {0} is successful.";
            public const string ReportGroupUpdateSuccess = "ReportGroup update with ID = {0} is successful.";
            public const string ReportGroupDeleteSuccess = "ReportGroup deletion with ID = {0} is successful.";
            public const string RPCategoryInsertSuccess = "RPCategory addition with ID = {0} is successful.";
            public const string RPCategoryUpdateSuccess = "RPCategory update with ID = {0} is successful.";
            public const string RPCategoryDeleteSuccess = "RPCategory deletion with ID = {0} is successful.";
            public const string SensitivityLevelInsertSuccess = "SensitivityLevel addition with ID = {0} is successful.";
            public const string SensitivityLevelUpdateSuccess = "SensitivityLevel update with ID = {0} is successful.";
            public const string SensitivityLevelDeleteSuccess = "SensitivityLevel deletion with ID = {0} is successful.";
            public const string UserInsertSuccess = "User addition with ID = {0} is successful.";
            public const string UserUpdateSuccess = "User update with ID = {0} is successful.";
            public const string UserDeleteSuccess = "User deletion with ID = {0} is successful.";
            public const string UserGroupInsertSuccess = "UserGroup addition with ID = {0} is successful.";
            public const string UserGroupUpdateSuccess = "UserGroup update with ID = {0} is successful.";
            public const string UserGroupDeleteSuccess = "UserGroup deletion with ID = {0} is successful.";

            public const string QueryParamsInsertSuccess = "UserGroup addition with Report = {0} and Name = {1} is successful.";
            public const string QueryParamsUpdateSuccess = "UserGroup update with Report = {0} and Name = {1} is successful.";
            public const string QueryParamsDeleteSuccess = "UserGroup deletion with Report = {0} and Name = {1} is successful.";
        }
    }
}
