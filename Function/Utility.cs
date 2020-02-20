using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Function
{
    public static class Utility
    {
        public static string GetEnumDescription(Enum value)
        {
            return GetEnumDescription(value, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }


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
            catch (System.Exception)
            {
                return "-- N/A --";
            }
        }


        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
