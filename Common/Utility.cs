using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Common
{
    public static class Utility
    {
        private static System.Collections.Specialized.NameValueCollection g_Config = ConfigurationManager.AppSettings;

        public static string GetEnumDescription(Enum value)
        {
            return GetEnumDescription(value, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static string GetEnumDescription(Enum value, System.Globalization.CultureInfo ci)
        {
            try
            {
                System.Reflection.FieldInfo fi = value.GetType().GetField(value.ToString());
                System.ComponentModel.DescriptionAttribute[] attributes =
                      (System.ComponentModel.DescriptionAttribute[])fi.GetCustomAttributes(
                      typeof(System.ComponentModel.DescriptionAttribute), false);

                return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
                //v1.5.0 Fai 2014.07.23 - Multi Language
                //return (attributes.Length > 0) ? Common.ResourcesManager.GetValue(attributes[0].Description, attributes[0].Description, ci) : value.ToString();
            }
            catch
            {
                return "-- N/A --";
            }
        }


        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }


        public static string GetDBConnectionString()
        {
            try
            {
                //v1.2.0 Fai 2018.08.21 - Allowed DBConnectionUserID and DBConnectionPassword not exist or empty value - Begin
                //string l_strConnectionUserID = Common.Encryption.GeneralDecrypt(g_Config["DBConnectionUserID"]);
                //string l_strConnectionPassword = Common.Encryption.GeneralDecrypt(g_Config["DBConnectionPassword"]);
                //string l_strConnection = string.Format(ConfigurationManager.ConnectionStrings["pmsstagingConnectionString"].ConnectionString, l_strConnectionUserID, l_strConnectionPassword);

                string l_strConnectionUserID = string.Empty;
                string l_strConnectionPassword = string.Empty;

                if (g_Config["DBConnectionUserID"] != null && g_Config["DBConnectionUserID"] != string.Empty)
                    l_strConnectionUserID = Common.Encryption.GeneralDecrypt(g_Config["DBConnectionUserID"]);

                if (g_Config["DBConnectionPassword"] != null && g_Config["DBConnectionPassword"] != string.Empty)
                    l_strConnectionPassword = Common.Encryption.GeneralDecrypt(g_Config["DBConnectionPassword"]);

                string l_strConnection = string.Format(ConfigurationManager.ConnectionStrings["pmsstagingConnectionString"].ConnectionString, l_strConnectionUserID, l_strConnectionPassword);
                //v1.2.0 Fai 2018.08.21 - Allowed DBConnectionUserID and DBConnectionPassword not exist or empty value - End

                return l_strConnection;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
