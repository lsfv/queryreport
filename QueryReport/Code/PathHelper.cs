using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QueryReport.Code
{
    public static class PathHelper
    {
        public static string getFontFolderName()
        {
            return System.Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\";
        }

        public static string getTempFolderName()
        {
            return System.IO.Path.GetTempPath() + "Downloads\\";
        }

        /// <summary>
        /// Remove all illegal characters in Windows filename for string.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string getSafePath(string path)
        {
            return path.Replace("/", "").Replace("\\", "").Replace(":", "").Replace("*", "").Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
        }
    }
}