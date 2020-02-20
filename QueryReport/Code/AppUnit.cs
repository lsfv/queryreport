//using System;
//using System.Data;
//using System.Configuration;
//using System.Web;
//using System.Web.Security;
//using System.Web.UI;
//using System.Web.UI.HtmlControls;
//using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
//using System.Text;

//namespace QueryReport.Code
//{
//    /// <summary>
//    /// 此程序帮助类
//    /// </summary>
//    public abstract class appunit
//    {
//        /// <summary>
//        /// 枚举类型转为table
//        /// </summary>
//        /// <param name="p_EnumType"></param>
//        /// <returns></returns>
//        public static DataTable GetTableFEnum(Type p_EnumType, int scode,int endcode)
//        {
//            if (!p_EnumType.IsEnum)
//                throw new Exception("不是枚举类型");

//            DataTable dt = new DataTable();
//            dt.Columns.Add("funname", typeof(System.String));
//            dt.Columns.Add("add", typeof(System.Int32));
//            dt.Columns.Add("modify", typeof(System.Int32));
//            dt.Columns.Add("del", typeof(System.Int32));
//            dt.Columns.Add("view", typeof(System.Int32));
//            dt.Columns.Add("special", typeof(System.Int32));

//            string strfunfs = "empty";
//            foreach (object value in Enum.GetValues(p_EnumType))
//            {
//                strfunfs = "empty";
//                for (int i = 0; i <= AppNum.funfs.Rank; i++)
//                {
//                    if (AppNum.funfs[i, 0] == Convert.ToInt32(value).ToString())
//                    {
//                        strfunfs = AppNum.funfs[i, 1];
//                    }
//                }

//                if (strfunfs == "empty")
//                { }
//                else if (strfunfs.Split(',').Length == 5)
//                {
//                    if (strfunfs.Split(',')[4] == "2")
//                    {
//                        DataRow a = dt.NewRow();
//                        string name = Enum.GetName(p_EnumType, value);
//                        dt.Rows.Add(name, 1, 1, 1, 1, 2);
//                    }
//                    else
//                    {
//                        DataRow a = dt.NewRow();
//                        string name = Enum.GetName(p_EnumType, value);
//                        dt.Rows.Add(name, strfunfs.Split(',')[0], strfunfs.Split(',')[1], strfunfs.Split(',')[2], strfunfs.Split(',')[3], 1);
//                    }
//                }
//                else
//                { }
//            }
//            return dt;
//        }

//        public static DataTable GetTableFEnum2(Type p_EnumType, int scode, int endcode,int groupid)
//        {
//            if (!p_EnumType.IsEnum)
//                throw new Exception("不是枚举类型");

//            DataTable dt = new DataTable();
//            dt.Columns.Add("funname", typeof(System.String));
//            dt.Columns.Add("view", typeof(System.Int32));
//            dt.Columns.Add("add", typeof(System.Int32));
//            dt.Columns.Add("modify", typeof(System.Int32));
//            dt.Columns.Add("del", typeof(System.Int32));
//            dt.Columns.Add("special", typeof(System.Int32));
//            dt.Columns.Add("funid", typeof(System.Int32));

//            string strfunfs = "empty";
//            foreach (object value in Enum.GetValues(p_EnumType))
//            {
//                if (Convert.ToInt32(value) >= scode && Convert.ToInt32(value) <= endcode)
//                {
//                    strfunfs = "empty";
//                    for (int i = 0; i < AppNum.funfs.Length/AppNum.funfs.Rank; i++)
//                    {
//                        if (AppNum.funfs[i, 0] == Convert.ToInt32(value).ToString())
//                        {
//                            strfunfs = AppNum.funfs[i, 1];
//                        }
//                    }

//                    if (strfunfs == "empty")
//                    { }
//                    else if (strfunfs.Split(',').Length == 5)
//                    {
//                        if (strfunfs.Split(',')[4] == "2")
//                        {
//                            DataRow a = dt.NewRow();
//                            string name = Enum.GetName(p_EnumType, value);

//                            int ba = 0;
//                            ba = CheckAccessRight(groupid, (QueryReport.Code.FunctionID)value, QueryReport.Code.UserRightType.SPECIAL) == true ? 4 : 0;

//                            dt.Rows.Add(name, 1, 1, 1, 1, 2 + ba, Convert.ToInt32(value));
//                        }
//                        else
//                        {
//                            DataRow a = dt.NewRow();
//                            string name = Enum.GetName(p_EnumType, value);
//                            int ba = 0, bb = 0, bc = 0, bd = 0;

//                            ba = CheckAccessRight(groupid, (QueryReport.Code.FunctionID)value, QueryReport.Code.UserRightType.VIEW) == true ? 4 : 0;
//                            bb = CheckAccessRight(groupid, (QueryReport.Code.FunctionID)value, QueryReport.Code.UserRightType.ADD) == true ? 4 : 0;
//                            bc = CheckAccessRight(groupid, (QueryReport.Code.FunctionID)value, QueryReport.Code.UserRightType.EDIT) == true ? 4 : 0;
//                            bd = CheckAccessRight(groupid, (QueryReport.Code.FunctionID)value, QueryReport.Code.UserRightType.DELETE) == true ? 4 : 0;

//                            ba = Int32.Parse(strfunfs.Split(',')[0]) + (Int32.Parse(strfunfs.Split(',')[0]) == 1 ? 0 : ba);
//                            bb = Int32.Parse(strfunfs.Split(',')[1]) + (Int32.Parse(strfunfs.Split(',')[1]) == 1 ? 0 : bb);
//                            bc = Int32.Parse(strfunfs.Split(',')[2]) + (Int32.Parse(strfunfs.Split(',')[2]) == 1 ? 0 : bc);
//                            bd = Int32.Parse(strfunfs.Split(',')[3]) + (Int32.Parse(strfunfs.Split(',')[3]) == 1 ? 0 : bd);

//                            dt.Rows.Add(name, ba, bb, bc, bd, 1, Convert.ToInt32(value));
//                        }
//                    }
//                    else
//                    { }
//                }
//            }
//            return dt;
//        }


//        public static bool CheckAccessRight(int groupid, QueryReport.Code.FunctionID functionid, QueryReport.Code.UserRightType functitiondetail)
//        {
//            Maticsoft.BLL.t_UserGroup_Function bll =new Maticsoft.BLL.t_UserGroup_Function();

//            bool hasright = false;

//            Maticsoft.Model.t_UserGroup_Function mymodel;

//            mymodel = bll.GetModel(groupid,Convert.ToInt32(functionid));
//            if (mymodel != null)
//            {
//                switch (functitiondetail)
//                {
//                    case QueryReport.Code.UserRightType.ADD:
//                        {
//                            hasright = mymodel.AddRight;
//                            break;
//                        }
//                    case QueryReport.Code.UserRightType.DELETE:
//                        {
//                            hasright = mymodel.DeleteRight;
//                            break;
//                        }
//                    case QueryReport.Code.UserRightType.EDIT:
//                        {
//                            hasright = mymodel.EditRight;
//                            break;
//                        }
//                    case QueryReport.Code.UserRightType.VIEW:
//                        {
//                            hasright = mymodel.ViewRight;
//                            break;
//                        }
//                    case QueryReport.Code.UserRightType.SPECIAL:
//                        {
//                            hasright = mymodel.ViewRight;
//                            break;
//                        }
//                }
//            }
//            return hasright;
//        }

//        public static void checkaccess(int groupid, QueryReport.Code.FunctionID functionid, QueryReport.Code.UserRightType functitiondetail,string uid)
//        {
//            if (QueryReport.Code.appunit.CheckAccessRight(groupid, functionid, functitiondetail) || uid.ToUpper()=="ADMIN")
//            {

//            }
//            else
//            {
//                HttpContext.Current.Response.Redirect("../erroraccess.aspx", true);
//            }
//        }
//    }
//}