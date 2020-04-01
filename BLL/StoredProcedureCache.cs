using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Net.Cache;

namespace CUSTOMRP.BLL
{
    public class StoredProcedureCache
    {
        private static Dictionary<string, DataTable> storedPTables = new Dictionary<string, DataTable>();

        public static DataTable GetTable(string spName,string dbname)
        {
            
            DataTable dt = null;
            string key = spName + DateTime.Now.ToString("hhmm");//保留1分钟的数据
            if (storedPTables.Keys.Contains(key))
            {
                dt= storedPTables[key];
            }
            else
            {
                string sql = "use "+dbname+";EXEC [qreport].["+spName+"];";
                BLL.COMMON bllcommon = new COMMON();
                dt=bllcommon.query(1, sql.ToString());
                storedPTables.Add(key, dt);


                for (int i = 0; i < storedPTables.Keys.Count; i++)
                {
                    if (storedPTables.Keys.ToArray()[i].StartsWith(spName))
                    {
                        storedPTables.Remove(storedPTables.Keys.ToArray()[i]);
                        break;
                    }
                }
            }
            return dt;
        }

        public static HashSet<string> GetValues(DataTable dt, string columnName)
        {
            HashSet<string> res = new HashSet<string>();
            if (dt != null && dt.Columns.Contains(columnName))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    res.Add(dt.Rows[i][columnName].ToString());
                }
            }
            return res;
        }
    }
}
