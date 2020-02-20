using System;

namespace CUSTOMRP.BLL
{
    public abstract class AppNum
    {
        private static System.Collections.Specialized.NameValueCollection g_Config = System.Configuration.ConfigurationManager.AppSettings;

        public const string Error_AppHelper_ParseParam_NotNumeric = "Parameter {1} for {0} is not numeric.";
        public const string Error_Delete_WordTemplate_In_Use = "Cannot delete WordTemplate. WordTemplate is in use.";
    }
}
