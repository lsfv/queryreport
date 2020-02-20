using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace QueryReport.Code
{
    /// <summary>
    /// 程序的枚举类型，全部在此定义，数据库和程序都以此为基准
    /// </summary>
    public class AppNum
    {
        private static System.Collections.Specialized.NameValueCollection g_Config = System.Configuration.ConfigurationManager.AppSettings;

        public static string companyName
        {
            get
            {
                return g_Config["CompanyName"];
            }
        }

        public class ErrorMsg
        {
            private ErrorMsg() { /* must not be instantiated */ }

            public const string parameter_error = "Invalid Parameter.";
            public const string success = "Success.";

            //public const string housecode_null = "Invalid House Code or CarNo.";
            //public const string carcode_null = "Input CarNo.";

            //public const string ugerror = "Code already exist! Please change!";
            //public const string cumviperror = "VIP number must be greater than 8 digits!";
            //public const string cumvipexist = "VIP number already exist!";

            //public const string cummnumerror = "Member number must 10 bit!";
            public const string Commonexits = "Already exist!";

            public const string ugdeerror = "User group already allocated by some user, please change the user setting first!";
            public const string loginerror = "Invalid User id or Password";

            public const string mandatoryerror = "Please input all of mandatory fields";
            public const string loginidexits = "UserID already exist!,please change!";
            public const string mobilandtel = "Please input phone number either in Mobile Number(1) or Tel Number (1) !";
            public const string accesserror = "Access is denied, you do not have permission to access this function!";
            public const string admindelerror = "Admin cannot be deleted!";
            public const string adminchageerror = "Admin must be administrator!";

            public const string HouseCodeerror = "House Code already exist! Please change!";
            public const string carCodeerror = "Car No already exist! Please change!";

            public const string NoTwice = "You cannot have [field] twice!";//" + duplicate + "
            public const string NoNumber = "You cannot have [field]! It is not a number.";//" + duplicate + "
            public const string NoSortexist = "[field] must be added to Contents list first !";//" + duplicate + "

            public const string GeneralError = "Error occurred, please check the query settings.";

            public const string codesettingerror = "CodeName + codeValue already exist! Please change!";

            public const string InvalidData = "Invalid data has been submitted.";

            public const string cantdeletesv = "Query cannot be deleted, it has been referenced in reports.";

            public const string fieldcannotbeempty = "'{0}' cannot be empty.\\r\\n";

            public const string pleaseselectvalidvaluefrom = "Please select valid value from '{0}'.\\r\\n";

            public const string filenotfounderror = "Error: File not found.";

            public const string onlyWordFileIsAllowed = "Only .docx type file can be uploaded.";

            public const string uploadFailed = "File upload failed.";

            public const string FailedToConnectQueryReportDatabase = "Failed to Connect Query Report Database, please check the configuration.";

            public const string DisplayNameSameAsColumnName = "Display name [{0}] cannot be set to be the same as column name.";

            public const string DuplicateDisplayName = "Display name [{0}] cannot be set to be the same as column name.";

            //1 show,2check 
            //public static string[,] funfs = {
            //                                {"1001","2,2,2,2,1"},
            //                                {"1002","2,1,2,1,1"},
            //                                {"1003","2,2,2,2,1"},
            //                                {"2001","2,2,2,2,1"},
            //                                {"3001","2,2,2,2,1"}
            //                                };

        }

        //public const string str_var_UserCookieName = "UserInfo";
        public const string str_var_UserSessionName = "user";
        public const string str_var_UserCookie_uid = "UID";
        public const string str_var_UserCookie_Databaseid = "CID";
        public const string str_var_UserCookie_menuselected = "menu";
        public const string str_var_UserCookie_logintime = "LOGINDATE";
        public const string str_var_UserCookie_DatabaseName = "DATABASENAME";
        public const string str_var_UserCookie_APPLICATIONID = "APPLICATIONID";

        public const string str_var_UserSessionGroupRight = "usergroupright";

        public const string STR_EXCELTEMPLATEPATH = "excelTemplate";
    }
}