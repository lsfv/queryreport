using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace Common
{
    public static class IncWeb
    {
        public static void AddNodes(TreeView tv, DataTable dt, string text,
            string value, string url, string rootselect, string rootselectvalue,
            int rootindex, string childselect, string childselectvalue, string ordervalue, string ovalue)
        {

            DataRow[] root = dt.Select(rootselect + "=" + rootselectvalue, ordervalue + " " + ovalue);
            for (int i = 0; i < root.Length; i++)
            {
                TreeNode tn = new TreeNode();
                tn.Text = root[i][text].ToString();
                tn.SelectAction = TreeNodeSelectAction.None;
                tn.Value = root[i][value].ToString();
                tv.Nodes.AddAt(rootindex, tn);
                if (i != root.Length - 1)
                {
                    tn.Expanded = false;
                }

                addchilid(dt, text, value, url, childselect, childselectvalue, tn, root[i], ordervalue, ovalue);
            }
        }

        /// <summary>
        /// get master page control
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ControlName"></param>
        /// <returns></returns>
        public static System.Web.UI.Control GetMasterPageControl(Page p, string ControlName)
        {
            return p.Master.FindControl(ControlName);
        }

        //((MasterPage)Page.Master).FindControl("literal_msg");

        private static void addchilid(DataTable dt, string text,
            string value, string url, string childselect,
            string childselectvalue, TreeNode ftn, DataRow fdr, string ordervalue, string ovalue)
        {
            DataRow[] child = dt.Select(childselect + "=" + fdr[childselectvalue].ToString(), ordervalue);
            for (int j = 0; j < child.Length; j++)
            {
                TreeNode ctn = new TreeNode();
                ctn.Text = child[j][text].ToString();
                ctn.NavigateUrl = child[j][url].ToString();
                ctn.Value = child[j][value].ToString();
                ftn.ChildNodes.Add(ctn);

                addchilid(dt, text, value, url, childselect, childselectvalue, ctn, child[j], ordervalue, ovalue);
            }
        }

        public static System.Drawing.Color readonlycolor()
        {
            return System.Drawing.Color.FromArgb(212, 208, 200);
        }

        /// <summary>
        /// 组合搜索sql语句
        /// </summary>
        /// <param name="wc"></param>
        /// <param name="cs">1为 加''"     *#null#* 代表除text之外的不做查询的约定,text不查询就为空</param>
        /// <param name="searchwhat">select [u2_id] from [lx_user2] where 1=1</param>
        /// <returns></returns>
        public static string searend(WebControl[] wc, string[,] cs, string searchwhat, bool[] likebool)
        {
            string str = "";
            str = searenda(wc, cs, likebool);
            if (str != "")
            {
                str = searchwhat + str;
            }
            return str;
        }

        private static string searenda(WebControl[] wc, string[,] cs, bool[] likebool)
        {
            string str = "";
            for (int i = 0; i < wc.Length; i++)
            {
                string test = wc[i].GetType().ToString().ToLower();
                switch (wc[i].GetType().ToString().ToLower())
                {
                    case "system.web.ui.webcontrols.textbox":
                        {
                            if (((TextBox)wc[i]).Text != "")
                            {
                                if (cs[i, 1] == "0")
                                {
                                    if (likebool[i])
                                    {

                                        str = str + " and " + cs[i, 0] + " like '%" + ((TextBox)wc[i]).Text + "%' ";
                                    }
                                    else
                                    {

                                        str = str + " and " + cs[i, 0] + "=" + ((TextBox)wc[i]).Text;
                                    }

                                }
                                else
                                {
                                    if (likebool[i])
                                    {

                                        str = str + " and " + cs[i, 0] + " like '%" + ((TextBox)wc[i]).Text + "%' ";
                                    }
                                    else
                                    {

                                        str = str + " and " + cs[i, 0] + "='" + ((TextBox)wc[i]).Text + "'";
                                    }

                                }
                            }
                            break;
                        }
                    case "system.web.ui.webcontrols.dropdownlist":
                        {
                            if (((DropDownList)wc[i]).SelectedValue != "*#null#*")
                            {
                                if (cs[i, 1] == "0")
                                {
                                    if (likebool[i])
                                    {
                                        str = str + " and " + cs[i, 0] + " like '%" + ((DropDownList)wc[i]).SelectedValue + "%' ";
                                    }
                                    else
                                    {
                                        str = str + " and " + cs[i, 0] + "=" + ((DropDownList)wc[i]).SelectedValue;
                                    }

                                }
                                else
                                {
                                    if (likebool[i])
                                    {
                                        str = str + " and " + cs[i, 0] + " like '%" + ((DropDownList)wc[i]).SelectedValue + "%' ";
                                    }
                                    else
                                    {
                                        str = str + " and " + cs[i, 0] + "='" + ((DropDownList)wc[i]).SelectedValue + "'";
                                    }
                                }
                            }
                            break;
                        }
                    case "system.web.ui.webcontrols.radiobuttonlist":
                        {

                            if (((RadioButtonList)wc[i]).SelectedValue != "*#null#*")
                            {
                                if (cs[i, 1] == "0")
                                {
                                    if (likebool[i])
                                    {
                                        str = str + " and " + cs[i, 0] + " like '%" + ((RadioButtonList)wc[i]).SelectedValue + "%' ";
                                    }
                                    else
                                    {

                                        str = str + " and " + cs[i, 0] + "=" + ((RadioButtonList)wc[i]).SelectedValue;
                                    }

                                }
                                else
                                {
                                    if (likebool[i])
                                    {
                                        str = str + " and " + cs[i, 0] + " like '%" + ((RadioButtonList)wc[i]).SelectedValue + "%' ";
                                    }
                                    else
                                    {
                                        str = str + " and " + cs[i, 0] + "='" + ((RadioButtonList)wc[i]).SelectedValue + "'";
                                    }

                                }
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            return str;
        }

        /// <summary>
        /// *#null#*代表不做查询
        /// </summary>
        /// <param name="wc"></param>
        /// <param name="cs"></param>
        /// <returns></returns>
        private static string searenda(string[] wc, string[,] cs)
        {
            string str = "";
            for (int i = 0; i < wc.Length; i++)
            {
                if (wc[i] != "*#null#*")
                {
                    if (cs[i, 1] == "0")
                    {
                        str = str + " and " + cs[i, 0] + "=" + wc[i];
                    }
                    else
                    {
                        str = str + " and " + cs[i, 0] + "='" + wc[i] + "'";
                    }
                }
            }
            return str;
        }

        /// <summary>
        /// 检查Request.QueryString的参数是否都存在,否则转到错误页面
        /// </summary>
        /// <param name="qs"></param>
        /// <param name="errorurl"></param>
        public static void checkqstring(string[] qs, string errorurl)
        {

            foreach (string qs1 in qs)
            {
                if (string.IsNullOrEmpty(HttpContext.Current.Request.QueryString[qs1]))
                {
                    HttpContext.Current.Response.Redirect(errorurl, true);
                }
                break;
            }
        }

        /// <summary>
        /// 检查Request.QueryString的参数是否都存在
        /// </summary>
        /// <param name="qs"></param>
        /// <param name="errorurl"></param>
        public static bool checkqstring(string[] qs)
        {
            bool full = true;
            foreach (string qs1 in qs)
            {
                if (string.IsNullOrEmpty(HttpContext.Current.Request.QueryString[qs1]))
                {
                    full = false;
                }
                break;
            }
            return full;
        }

        /// <summary>
        ///  value=string 如果选择把value加上，如str += it.Value
        /// </summary>
        /// <param name="str"></param>
        /// <param name="cbl"></param>
        /// <returns></returns>
        public static string CheckBoxList_GetString(CheckBoxList cbl)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (ListItem it in cbl.Items)
            {
                if (it.Selected)
                {
                    sb.Append((sb.Length != 0 ? "," : String.Empty) + it.Value);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// value=string 如果含有则选中(和前配合)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="cbl"></param>
        /// <returns></returns>
        public static void CheckBoxList_LoadString(string str, CheckBoxList cbl)
        {

            foreach (ListItem it in cbl.Items)
            {
                if (("," + str+",").Contains("," + it.Value + ","))
                {
                    it.Selected = true;
                }
            }
        }

        /// <summary>
        ///  value=int 
        /// </summary>
        /// <param name="cbl"></param>
        /// <returns></returns>
        public static IList<int> CheckBoxList_ToList(CheckBoxList cbl)
        {
            IList<int> result = new List<int>();
            foreach (ListItem it in cbl.Items)
            {
                if (it.Selected == true)
                {
                    result.Add(Int32.Parse(it.Value));
                }
            }
            return result;
        }


        /// <summary>
        /// value=int 如果含有则选中(和前配合)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="cbl"></param>
        /// <returns></returns>
        public static void CheckBoxList_LoadList(List<int> array, CheckBoxList cbl)
        {
            foreach (ListItem it in cbl.Items)
            {
                if (array.Contains(Int32.Parse(it.Value)))
                {
                    it.Selected = true;
                }
            }
        }

        public static HttpCookie cookie_new(string domain, string cookiename,System.Collections.Hashtable keyvalue,double expires)
        {
            HttpCookie myCookie = new HttpCookie(cookiename);
            if (domain != "")
            {
                myCookie.Domain = domain;
            }

            foreach (System.Collections.DictionaryEntry de in keyvalue) //ht为一个Hashtable实例
            {
                myCookie.Values.Add(de.Key.ToString(), de.Value.ToString());
            }
            myCookie.Expires = DateTime.Now.AddDays(expires);

            HttpContext.Current.Response.AppendCookie(myCookie);
            return myCookie; 
        }

        public static HttpCookie cookie_modify(string cookiename, System.Collections.Hashtable keyvalue)
        {

            HttpCookie myCookie = HttpContext.Current.Request.Cookies[cookiename];

            foreach (System.Collections.DictionaryEntry de in keyvalue)
            {
                if (myCookie.Values.GetValues(de.Key.ToString()) != null)
                {
                    myCookie.Values.Remove(de.Key.ToString());
                }
                myCookie.Values.Add(de.Key.ToString(), de.Value.ToString());
            }
            HttpContext.Current.Response.AppendCookie(myCookie);
            return myCookie; 
        }

        public static int[] getColumnIndexByColumnName(string[] ColumnName, DataColumnCollection Columns)
        {
            int[] Result = new int[ColumnName.Length];
            for (int i = 0; i < ColumnName.Length; i++)
            {
                foreach (DataColumn dc in Columns)
                {
                    if (ColumnName[i] == dc.ColumnName)
                    {
                        Result[i] = dc.Ordinal;
                    }
                }
            }
            Result = Common.Utils.intOrder(Result);
            return Result;
        }

        public static int[] getColumnIndexByColumnNameNotOrdered(string[] ColumnName, DataColumnCollection Columns)
        {
            int[] Result = new int[ColumnName.Length];
            for (int i = 0; i < ColumnName.Length; i++)
            {
                foreach (DataColumn dc in Columns)
                {
                    if (ColumnName[i] == dc.ColumnName)
                    {
                        Result[i] = dc.Ordinal;
                    }
                }
            }
            return Result;
        }
    }
}