using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.UI;
using CUSTOMRP.Model;
using NPOI.XSSF.UserModel;
using QueryReport.Code;

namespace QueryReport.report
{
    /// <summary>
    /// This page is to be called directly from other system, so will implement it's own login process and will not use a masterpage.
    /// </summary>
    public partial class rpEmbedded : System.Web.UI.Page
    {
        protected System.Collections.Specialized.NameValueCollection g_Config = System.Configuration.ConfigurationManager.AppSettings;

        private CUSTOMRP.Model.REPORT myReport = null;
        private bool p_fSuppressRender = false;     // Whether to render page contents

        protected CUSTOMRP.Model.LoginUser _me = null;
        protected CUSTOMRP.Model.LoginUser me
        {
            get
            {
                return _me;
            }
        }

        #region Event Handlers

        protected void Page_Init(object sender, EventArgs e)
        {
            this._me = (new QueryReport.Code.LoginUser()).CurrentUser;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int rpid = 0;

            this.lblContent.Text = String.Empty;

            #region Login Check

            if (Request.Params["hash"] != null)
            {
                string username = Request.Params["uid"];
                string hash = Request.Params["hash"];
                int databaseId;
                if (!Int32.TryParse(Request.Params["dbid"], out databaseId)) { databaseId = -1; }

                this.login(username, hash, databaseId);
            }

            if (!QueryReport.Code.LoginUser.isLogin())
            {
                //return; // print blank page
                Response.StatusCode = 412;
                return;
            }

            #endregion

            switch (Request.QueryString["action"])
            {
                case "getParam":
                    {
                        #region Get Report Parameters
                        if (!String.IsNullOrEmpty(Request.Params["rpid"]) && (Int32.TryParse(Request.Params["rpid"], out rpid)))
                        {
                            //this.GetReportCriteria(rpid);
                            this.GetReportCriteriaNew(rpid);
                        }
                        #endregion
                    }
                    break;
                //case "getQueryParam":
                //    // v1.8.8 Alex 2018.10.25 Get saved values - Begin
                //    if (!String.IsNullOrEmpty(Request.Params["rpid"]) && (Int32.TryParse(Request.Params["rpid"], out rpid)))
                //    {
                //        this.GetQueryParams(rpid);
                //    }
                //    // v1.8.8 Alex 2018.10.25 Get saved values - End
                //break;
                case "getOperands":
                    {
                        #region Get supported operand list

                        this.GetReportOperands();

                        #endregion
                    }
                    break;
                case "getRptTypes":
                    {
                        #region Get supported export types

                        this.GetReportExportTypes();

                        #endregion
                    }
                    break;
                case "getRptGrps":
                    {
                        #region Get report group list

                        this.GetReportGroups();

                        #endregion
                    }
                    break;
                default:
                    {
                        if (!String.IsNullOrEmpty(Request.Params["rpid"]) && (Int32.TryParse(Request.Params["rpid"], out rpid)))
                        {
                            #region Check report access and export

                            myReport = WebHelper.bllReport.GetModel(me.ID, rpid);
                            string strContent = Request.Params["params"];
                            string rptType = Request.Params["rptType"];
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(EmbeddedParam[]));
                            EmbeddedParam[] rpParam = null;
                            ReportCriteria[] rcParam = null;
                            CUSTOMRP.BLL.AppHelper.QueryParamsObject[] qpParam = null;
                            try
                            {
                                rpParam = (EmbeddedParam[])serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(strContent)));
                                rcParam = (from x in rpParam
                                           where string.IsNullOrEmpty(x.ParamName)
                                           select new ReportCriteria()
                                           {
                                               SVID = x.SVID,
                                               RCID = x.RCID,
                                               RPID = x.RPID,
                                               COLUMNNAME = x.COLUMNNAME,
                                               CRITERIA1 = x.CRITERIA1,
                                               CRITERIA2 = x.CRITERIA2,
                                               CRITERIA3 = x.CRITERIA3,
                                               CRITERIA4 = x.CRITERIA4,
                                               REPORTNAME = x.REPORTNAME,
                                               SOURCEVIEWNAME = x.SOURCEVIEWNAME,
                                               CATEGORY = x.CATEGORY,
                                               DATATYPE = x.DATATYPE
                                           }).ToArray();
                                qpParam = (from x in rpParam
                                           where !string.IsNullOrEmpty(x.Value)
                                           select new CUSTOMRP.BLL.AppHelper.QueryParamsObject()
                                           {
                                               ParamName = x.ParamName,
                                               SqlType = x.SqlType,
                                               Value = x.Value
                                           }).ToArray();
                                CUSTOMRP.BLL.AppHelper.ParseParam(me.ID, ref rcParam);
                            }
                            catch
                            {
                                // ignore errors
                            }



                            //DataContractJsonSerializer qpserializer = new DataContractJsonSerializer(typeof(CUSTOMRP.BLL.AppHelper.QueryParamsEmbedded[]));
                            //CUSTOMRP.BLL.AppHelper.QueryParamsEmbedded[] qrParam = null;
                            //try
                            //{
                            //    qrParam = (CUSTOMRP.BLL.AppHelper.QueryParamsEmbedded[])serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(strContent)));

                            //    //CUSTOMRP.BLL.AppHelper.ParseQueryParam(me.ID, ref qrParam);
                            //}
                            //catch
                            //{
                            //    // ignore errors
                            //}



                            if (myReport != null)
                            {
                                //this.ltr.Text = myReport.REPORTNAME;
                                switch (rptType)
                                {
                                    case "0":   // Excel / Word
                                        {
                                            if (myReport.TYPE == 1)
                                            {
                                                if (rcParam != null)
                                                {
                                                    Excel(myReport.ID, rcParam, qpParam);
                                                }
                                                else
                                                {
                                                    Excel(myReport.ID, qpParam: qpParam);
                                                }
                                            }
                                            else
                                            {
                                                if (rcParam != null)
                                                {
                                                    Word(myReport.ID, rcParam, qpParam);
                                                }
                                                else
                                                {
                                                    Word(myReport.ID, qpParam: qpParam);
                                                }
                                            }
                                        }
                                        break;
                                    case "2":   // PDF
                                        {
                                            if (rpParam != null)
                                            {
                                                PDF(myReport.ID, rcParam, qpParam);
                                            }
                                            else
                                            {
                                                PDF(myReport.ID, qpParam: qpParam);
                                            }
                                        }
                                        break;
                                    default:    // defaulted to on screen
                                        {
                                            if (rpParam != null)
                                            {
                                                HtmlExport1(myReport.ID, rcParam, qpParam);
                                            }
                                            else
                                            {
                                                HtmlExport1(myReport.ID, qpParam: qpParam);
                                            }
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                Common.JScript.AlertAndRedirect(AppNum.ErrorMsg.parameter_error, "rplist.aspx");
                                Response.End();
                            }
                            #endregion
                        }
                        else
                        {
                            #region Get Report listing

                            //v1.2.0 - Cheong - 2016/07/14 - Add support for report category filtering
                            //this.GetReportCatalog();
                            this.GetReportCatalog(Request.Params["rpgrp"]);

                            #endregion
                        }
                    }
                    break;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.p_fSuppressRender)
            {
                base.Render(writer);
            }
        }

        #endregion

        #region Private methods

        protected void login(string uid, string password, int databaseId)
        {
            CUSTOMRP.Model.USER myUser = null;

            try
            {
                //v1.1.0 - Cheong - 2016/05/18 - Make hashkey configurable
                CUSTOMRP.Model.DATABASE mydb = WebHelper.bllCompany.GetModel(me.ID, databaseId);
                string salt = (mydb != null) ? mydb.HASHKEY : "com";
                //string hash = Common.Utils.MD5NET(uid + DateTime.Now.ToString("yyyyMMdd") + "com");
                string hash = Common.Utils.MD5NET(uid + DateTime.Now.ToString("yyyyMMdd") + salt);
                if (hash == password)
                {
                    myUser = WebHelper.bllUSER.GetModel(me.ID, uid, databaseId);
                }
            }
            catch
            {
                // on any error, assume it's login failure
            }

            if (myUser != null)
            {
                //store user's information to cookie,

                CUSTOMRP.Model.DATABASE mydb = WebHelper.bllCompany.GetModel(me.ID, databaseId);

                HttpContext.Current.Session[AppNum.str_var_UserCookie_uid] = uid;
                HttpContext.Current.Session[AppNum.str_var_UserCookie_logintime] = DateTime.Now.ToString("yyyyMMddhhmm");
                HttpContext.Current.Session[AppNum.str_var_UserCookie_Databaseid] = mydb.ID;
                HttpContext.Current.Session[AppNum.str_var_UserCookie_DatabaseName] = mydb.NAME;
                HttpContext.Current.Session[AppNum.str_var_UserCookie_APPLICATIONID] = mydb.APPLICATIONID;

                HttpContext.Current.Session[AppNum.str_var_UserSessionName] = myUser;

                this._me = (new QueryReport.Code.LoginUser()).CurrentUser;   // update v_Security criteria
            }
            else
            {
                //Common.JScript.AlertAndRedirect(AppNum.loginerror, "~/SignIn.aspx");
                Common.JScript.Alert(AppNum.ErrorMsg.loginerror);
            }
        }

        protected void GetReportCatalog(string strReportGroup)
        {
            List<ReportCatalog> rclist = new List<ReportCatalog>();
            ReportCatalog rc = null;

            DataTable dt = WebHelper.bllReport.GetlistByDisplay(me.ID, Convert.ToInt32(me.DatabaseID), me.ReportGroup, me.rp_view, me.ViewLevel);
            foreach (DataRow dr in dt.Rows)
            {
                rc = new ReportCatalog()
                {
                    ID = Convert.ToInt32(dr["ID"]),
                    DATABASEID = Convert.ToInt32(dr["DATABASEID"]),
                    REPORTNAME = dr["REPORTNAME"] as string,
                    REPORTGROUP = Convert.ToInt32(dr["REPORTGROUPLIST"]),
                    CATEGORY = Convert.ToInt32(dr["CATEGORY"]),
                    TYPE = Convert.ToInt32(dr["TYPE"]),
                    DEFAULTFORMAT = Convert.ToInt32(dr["DEFAULTFORMAT"]),
                };
                rclist.Add(rc);
            }

            //v1.2.0 - Cheong - 2016/07/14 - Add support for report category filtering
            if (!String.IsNullOrEmpty(strReportGroup))
            {
                REPORTGROUP rpgrp = WebHelper.bllrpGroup.GetModelList(me.ID, String.Empty).Where(x => (x.DATABASEID == me.DatabaseID) && (x.NAME == strReportGroup)).FirstOrDefault();

                if (rpgrp != null)
                {
                    rclist = rclist.Where(x => x.REPORTGROUP == rpgrp.ID).ToList();
                }
            }

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ReportCatalog[]));
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, rclist.ToArray());

            this.p_fSuppressRender = true;
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.ContentEncoding = Encoding.UTF8;
            ms.WriteTo(Response.OutputStream);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        protected void GetReportCriteriaNew(int rpid)
        {
            List<EmbeddedParam> rclist = new List<EmbeddedParam>();
            EmbeddedParam rc = null;

            DataTable dt = WebHelper.bllReport.getCriteriaColumns(me.ID, rpid);
            if (dt.Rows.Count > 0)
            {
                int svid = Convert.ToInt32(dt.Rows[0]["SVID"]);
                string svname = WebHelper.bllSOURCEVIEW.GetModel(me.ID, svid).SOURCEVIEWNAME;
                foreach (DataRow dr in dt.Rows)
                {
                    //v1.2.0 Kim 2016.11.10 check CRITERIA2 instead as CRITERIA1 may empty in order to search null value
                    //if (!String.IsNullOrEmpty(dr["CRITERIA1"] as string))
                    if (!String.IsNullOrEmpty(dr["CRITERIA2"] as string))
                    {
                        rc = new EmbeddedParam()
                        {
                            SVID = svid,
                            RCID = Convert.ToInt32(dr["RCID"]),
                            RPID = Convert.ToInt32(dr["RPID"]),
                            COLUMNNAME = dr["COLUMNNAME"] as string,
                            CRITERIA1 = dr["CRITERIA1"] as string,
                            CRITERIA2 = dr["CRITERIA2"] as string,
                            CRITERIA3 = dr["CRITERIA3"] as string,
                            CRITERIA4 = dr["CRITERIA4"] as string,
                            REPORTNAME = dr["REPORTNAME"] as string,
                            SOURCEVIEWNAME = dr["SOURCEVIEWNAME"] as string,
                            CATEGORY = Convert.ToInt32(dr["CATEGORY"]),
                            DATATYPE = CUSTOMRP.BLL.AppHelper.getColumnType(svid, svname, dr["COLUMNNAME"] as string, me),
                        };
                        rclist.Add(rc);
                    }
                }

                //////////////////////////////////////////////////////////////////

                var QueryParams = (from x in CUSTOMRP.BLL.AppHelper.GetQueryParams(me.ID, me.DatabaseNAME, "qreport." + WebHelper.bllSOURCEVIEW.GetModel(me.ID, svid).TBLVIEWNAME)
                                   select new EmbeddedParam()
                                   {
                                       ParamName = Convert.ToString(x.ParamName),
                                       SqlType = Convert.ToString(x.SqlType)
                                   });

                foreach (var x in QueryParams)
                {
                    if (x.ParamName.Contains('$'))
                    {
                        string[] chopped = x.ParamName.Substring(12).Split('$');
                        x.EnumValues = CUSTOMRP.BLL.AppHelper.QueryParamGetEnumValues(me.ID, me.DatabaseNAME, chopped[1], chopped[2].Replace("#", " "));
                    }
                }
                rclist.AddRange(QueryParams);
            }
            //////////////////////////////////////////////////////////////////

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(EmbeddedParam[]));
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, rclist.ToArray());

            this.p_fSuppressRender = true;
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.ContentEncoding = Encoding.UTF8;
            ms.WriteTo(Response.OutputStream);
            HttpContext.Current.ApplicationInstance.CompleteRequest();

        }

        //protected void GetReportCriteria(int rpid)
        //{
        //    List<ReportCriteria> rclist = new List<ReportCriteria>();
        //    ReportCriteria rc = null;

        //    DataTable dt = WebHelper.bllReport.getCriteriaColumns(me.ID, rpid);
        //    if (dt.Rows.Count > 0)
        //    {
        //        int svid = Convert.ToInt32(dt.Rows[0]["SVID"]);
        //        string svname = WebHelper.bllSOURCEVIEW.GetModel(me.ID, svid).SOURCEVIEWNAME;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            //v1.2.0 Kim 2016.11.10 check CRITERIA2 instead as CRITERIA1 may empty in order to search null value
        //            //if (!String.IsNullOrEmpty(dr["CRITERIA1"] as string))
        //            if (!String.IsNullOrEmpty(dr["CRITERIA2"] as string))
        //            {
        //                rc = new ReportCriteria()
        //                {
        //                    SVID = svid,
        //                    RCID = Convert.ToInt32(dr["RCID"]),
        //                    RPID = Convert.ToInt32(dr["RPID"]),
        //                    COLUMNNAME = dr["COLUMNNAME"] as string,
        //                    CRITERIA1 = dr["CRITERIA1"] as string,
        //                    CRITERIA2 = dr["CRITERIA2"] as string,
        //                    CRITERIA3 = dr["CRITERIA3"] as string,
        //                    CRITERIA4 = dr["CRITERIA4"] as string,
        //                    REPORTNAME = dr["REPORTNAME"] as string,
        //                    SOURCEVIEWNAME = dr["SOURCEVIEWNAME"] as string,
        //                    CATEGORY = Convert.ToInt32(dr["CATEGORY"]),
        //                    DATATYPE = CUSTOMRP.BLL.AppHelper.getColumnType(svid, svname, dr["COLUMNNAME"] as string, me),
        //                };
        //                rclist.Add(rc);
        //            }
        //        }
        //    }

        //    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ReportCriteria[]));
        //    MemoryStream ms = new MemoryStream();
        //    serializer.WriteObject(ms, rclist.ToArray());

        //    this.p_fSuppressRender = true;
        //    Response.Clear();
        //    Response.ContentType = "application/json; charset=utf-8";
        //    Response.ContentEncoding = Encoding.UTF8;
        //    ms.WriteTo(Response.OutputStream);
        //    HttpContext.Current.ApplicationInstance.CompleteRequest();
        //}

        //protected void GetQueryParams(int rpid)
        //{
        //    CUSTOMRP.BLL.AppHelper.QueryParamsEmbedded[] QueryParams = null;

        //    DataTable dt = WebHelper.bllReport.getCriteriaColumns(me.ID, rpid);
        //    if (dt.Rows.Count > 0)
        //    {
        //        int svid = Convert.ToInt32(dt.Rows[0]["SVID"]);
        //        string svname = WebHelper.bllSOURCEVIEW.GetModel(me.ID, svid).SOURCEVIEWNAME;
        //        QueryParams = (from x in CUSTOMRP.BLL.AppHelper.GetQueryParams(me.ID, me.DatabaseNAME, "qreport." + WebHelper.bllSOURCEVIEW.GetModel(me.ID, svid).TBLVIEWNAME)
        //                  select new CUSTOMRP.BLL.AppHelper.QueryParamsEmbedded()
        //                             {
        //                                 ParamName = Convert.ToString(x.ParamName),
        //                                 SqlType = Convert.ToString(x.SqlType)
        //                             }).ToArray();

        //        foreach (var x in QueryParams) {
        //            if (x.ParamName.Contains('$'))
        //            {
        //                string[] chopped = x.ParamName.Substring(12).Split('$');
        //                x.EnumValues = CUSTOMRP.BLL.AppHelper.QueryParamGetEnumValues(me.ID, me.DatabaseNAME, chopped[1], chopped[2].Replace("#", " "));
        //            }
        //        }
        //    }

        //    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(CUSTOMRP.BLL.AppHelper.QueryParamsEmbedded[]));
        //    MemoryStream ms = new MemoryStream();
        //    serializer.WriteObject(ms, QueryParams);

        //    this.p_fSuppressRender = true;
        //    Response.Clear();
        //    Response.ContentType = "application/json; charset=utf-8";
        //    Response.ContentEncoding = Encoding.UTF8;
        //    ms.WriteTo(Response.OutputStream);
        //    HttpContext.Current.ApplicationInstance.CompleteRequest();
        //}

        //protected void GetQueryParams(int rpid)
        //{
        //    int svid = Convert.ToInt32(dt.Rows[0]["SVID"]);

        //    string svname = WebHelper.bllSOURCEVIEW.GetModel(me.ID, svid).SOURCEVIEWNAME;
        //    var QueryParams = CUSTOMRP.BLL.AppHelper.GetQueryParams(me.ID, me.DatabaseNAME, "qreport." + mySV.TBLVIEWNAME);
        //}

        protected void GetReportOperands()
        {
            string[] oplist = null;
            string datatype = Request.Params["type"] ?? String.Empty;

            switch (datatype.ToLower())
            {
                case "decimal":
                case "int":
                    {
                        oplist = new string[] {
                            "=",
                            ">",
                            "<",
                            ">=",
                            "<=",
                            "<>",
                            "In",
                            "Not In"
                        };
                    }
                    break;
                case "enum":
                    {
                        oplist = new string[] {
                            "=",
                        };
                    }
                    break;
                default:    // string, date and datetime
                    {
                        oplist = new string[] {
                            "Begins With",
                            "Contains",
                            "In",
                            "Not In",
                            "=",
                            ">=",
                            "<=",
                            "Does not contain",
                            "Does not equal",
                            "Between",
                            "Empty",
                        };
                    }
                    break;
            }

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(string[]));
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, oplist);

            this.p_fSuppressRender = true;
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.ContentEncoding = Encoding.UTF8;
            ms.WriteTo(Response.OutputStream);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        protected void GetReportExportTypes()
        {
            string[][] typeList = new string[][]{
                new string[] {"Excel", "On Screen", "PDF"},
                new string[] {"Word"},
            };

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(string[][]));
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, typeList);

            this.p_fSuppressRender = true;
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.ContentEncoding = Encoding.UTF8;
            ms.WriteTo(Response.OutputStream);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        protected void GetReportGroups()
        {
            var RptGroups = WebHelper.bllrpGroup.GetModelList(me.ID, String.Empty).Where(x => x.DATABASEID == me.DatabaseID)
                .Select(x => new System.Web.UI.WebControls.ListItem(x.NAME, Convert.ToString(x.ID))).ToArray();

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(RptGroups.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, RptGroups);

            this.p_fSuppressRender = true;
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.ContentEncoding = Encoding.UTF8;
            ms.WriteTo(Response.OutputStream);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        protected void HtmlExport1(int rpid, ReportCriteria[] rpParam = null, CUSTOMRP.BLL.AppHelper.QueryParamsObject[] qpParam = null)
        {
            //string sql = AppHelper.getSql(rpid, me, rpParam);
            //DataTable dt = WebHelper.bllCommon.query(sql);

            List<string> comments = new List<string>();
            List<string> avgs = new List<string>();
            List<string> sums = new List<string>();
            List<string> groups = new List<string>();
            List<string> subtotal = new List<string>();
            List<string> subavg = new List<string>();
            List<string> subcount = new List<string>();
            List<string> count = new List<string>();

            var tmp = myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.SortOn);
            List<string> sortonCols = tmp.Select(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DisplayName).ToList();
            List<bool> isAscending = tmp.Select(x => x.IS_ASCENDING).ToList();
            List<int> seq = tmp.Select(x => x.SEQ).ToList();
            string subcountLabel = !String.IsNullOrEmpty(myReport.SUBCOUNT_LABEL) ? myReport.SUBCOUNT_LABEL : null;
            bool pdfGridLines = myReport.PDF_GRID_LINES;

            List<string> hideRows = myReport.ReportColumns
                .Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content
                    && x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal
                    && x.EXCEL_COLWIDTH == 0m)
                .Select(x => x.COLUMNNAME).ToList();

            //v1.2.0 - Cheong - 2016/07/04 - Add option to hide duplicate items
            //DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(ref comments, ref avgs, ref sums, ref groups, ref subtotal, ref subavg, ref subcount, ref count, me, rpid, true, rpParam);
            DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(me.ID, ref comments, ref avgs, ref sums, ref groups, ref subtotal, ref subavg, ref subcount, ref count, me,
                rpid, myReport.fHideDuplicate, rpParam, QueryParamsToSqlParams(qpParam));

            //this.ltrcompanyname.Text = AppNum.companyName;
            string rptTitle = myReport.RPTITLE.Trim();

            StringBuilder sb = new StringBuilder("<div data-type=\"criteria\">");
            foreach (string cir in comments)
            {
                sb.AppendFormat("{0}<br />", cir);
            }
            sb.Append("</div>");
            //this.ltrreportDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            HtmlTable(myReport.EXTENDFIELD.Split(','),
                myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content).ToDictionary(x => x.COLUMNNAME, y => y.DisplayName),
                subtotal, subavg, subcount, count, dt, sums, avgs, groups, rptTitle, comments.Count > 0 ? sb.ToString() : String.Empty,
                subcountLabel, sortonCols, isAscending, seq, hideRows);
        }

        //protected void HtmlExport2(string[] ExtendedFields, Dictionary<string, string> content, List<string> subtotal, List<string> subavg, List<string> SubCount, List<string> Count, DataTable dt, List<string> cr, string rpstr, List<string> sums, List<string> avgs, List<string> group)
        //{
        //    //this.ltrcompanyname.Text = AppNum.companyName;
        //    string rptTitle = rpstr;
        //    StringBuilder sb = new StringBuilder("<div data-type=\"criteria\">");
        //    foreach (string cir in cr)
        //    {
        //        //v1.0.0 - Cheong - 2016/03/17 - Add formatting to criteria
        //        //sb.AppendFormat("{0}<br />", cir);
        //        sb.AppendFormat("{0}<br />", cir.Remove(0, 6).TrimEnd(')'));
        //    }
        //    sb.Append("</div>");
        //    //this.ltrreportDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

        //    HtmlTable(ExtendedFields, content, subtotal, subavg, SubCount, Count, dt, sums, avgs, group, rptTitle, cr.Count > 0 ? sb.ToString() : String.Empty);
        //}

        protected void HtmlTable(string[] ExtendedFields, Dictionary<string, string> content, List<string> subtotal, List<string> subavg, List<string> SubCount, List<string> Count, DataTable dt, List<string> sum, List<string> avg, List<string> group, string rptTitle, string comment
            , string subcountLabel, List<string> sortonCols, List<bool> isAscending, List<int> seq, List<string> hiderows)
        {
            this.p_fSuppressRender = true;
            Response.Clear();
            Response.Write(AppHelper.GetHtmlTableString(ExtendedFields, content, subtotal, subavg, SubCount, Count, dt, sum, avg, group, rptTitle, comment,
                subcountLabel, sortonCols, isAscending, seq, hiderows));
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        protected List<SqlParameter> QueryParamsToSqlParams(CUSTOMRP.BLL.AppHelper.QueryParamsObject[] qpParams)
        {
            if (qpParams.Length == 0)
            {
                return null;
            }
            var sqlParams_queryParams = new List<SqlParameter>();
            foreach (var x in qpParams)
            {
                switch (x.SqlType)
                {
                    case "bit":
                        sqlParams_queryParams.Add(new SqlParameter() { ParameterName = x.ParamName, SqlDbType = SqlDbType.Bit, Value = int.Parse(x.Value) });
                        break;
                    case "int":
                        sqlParams_queryParams.Add(new SqlParameter() { ParameterName = x.ParamName, SqlDbType = SqlDbType.Int, Value = int.Parse(x.Value) });
                        break;
                    case "date":
                        sqlParams_queryParams.Add(new SqlParameter() { ParameterName = x.ParamName, SqlDbType = SqlDbType.Date, Value = x.Value });
                        break;
                    case "datetime":
                        sqlParams_queryParams.Add(new SqlParameter() { ParameterName = x.ParamName, SqlDbType = SqlDbType.DateTime, Value = x.Value });
                        break;
                    case "varchar":
                        sqlParams_queryParams.Add(new SqlParameter() { ParameterName = x.ParamName, Size = -1, SqlDbType = SqlDbType.VarChar, Value = x.Value });
                        break;
                    case "nvarchar":
                        sqlParams_queryParams.Add(new SqlParameter() { ParameterName = x.ParamName, Size = -1, SqlDbType = SqlDbType.NVarChar, Value = x.Value });
                        break;
                }
            }
            return sqlParams_queryParams;
        }

        protected void PDF(int rpid, ReportCriteria[] rpParam = null, CUSTOMRP.BLL.AppHelper.QueryParamsObject[] qpParam = null)
        {
            //string sql = AppHelper.getSql(rpid, me, rpParam);
            //DataTable dt = WebHelper.bllCommon.query(sql);

            CUSTOMRP.Model.REPORT myReport = WebHelper.bllReport.GetModel(me.ID, rpid);

            string rpName = myReport.REPORTNAME;
            string rpTitle = myReport.RPTITLE;
            string showType = myReport.EXTENDFIELD.Split(',')[0];

            List<string> comments = new List<string>();
            List<string> avgs = new List<string>();
            List<string> sums = new List<string>();
            List<string> groups = new List<string>();
            List<string> subtotal = new List<string>();
            List<string> subavg = new List<string>();
            List<string> subcount = new List<string>();
            List<string> count = new List<string>();

            //v1.2.0 - Cheong - 2016/07/04 - Add option to hide duplicate items
            //DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(ref comments, ref avgs, ref sums, ref groups, ref subtotal, ref subavg, ref subcount, ref count, me, rpid, true, rpParam);
            DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(me.ID, ref comments, ref avgs, ref sums, ref groups, ref subtotal, ref subavg, ref subcount, ref count, me,
                rpid, myReport.fHideDuplicate, rpParam, QueryParamsToSqlParams(qpParam));

            this.p_fSuppressRender = true;

            string[] rptHeader = !String.IsNullOrEmpty(myReport.REPORT_HEADER) ? myReport.REPORT_HEADER.Split('\n') : null;
            string[] rptFooter = !String.IsNullOrEmpty(myReport.REPORT_FOOTER) ? myReport.REPORT_FOOTER.Split('\n') : null;
            var tmp = myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.SortOn);
            List<string> sortonCols = tmp.Select(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DisplayName).ToList();
            List<bool> isAscending = tmp.Select(x => x.IS_ASCENDING).ToList();
            List<int> seq = tmp.Select(x => x.SEQ).ToList();
            string subcountLabel = !String.IsNullOrEmpty(myReport.SUBCOUNT_LABEL) ? myReport.SUBCOUNT_LABEL : null;
            bool pdfGridLines = myReport.PDF_GRID_LINES;

            List<string> hideRows = myReport.ReportColumns
                .Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content
                    && x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal
                    && x.EXCEL_COLWIDTH == 0m)
                .Select(x => x.COLUMNNAME).ToList();

            ////v1.0.0 - Cheong - 2016/03/17 - Add formatting to criteria
            //comments = comments.Select(x => x.Remove(0, 6).TrimEnd(')')).ToList();

            string fontPath = PathHelper.getFontFolderName() + "simsun.ttc,1";

            bool showChangeOnly = myReport.EXTENDFIELD.Split(',')[CUSTOMRP.Model.REPORT.EXTENDFIELDs.ReportType] == CUSTOMRP.Model.REPORT.ExtReportType.ChangeOnly;
            bool hideHeaders = myReport.EXTENDFIELD.Split(',')[CUSTOMRP.Model.REPORT.EXTENDFIELDs.ReportType] == CUSTOMRP.Model.REPORT.ExtReportType.DataExport;
            bool hideCriteria = myReport.EXTENDFIELD.Split(',')[CUSTOMRP.Model.REPORT.EXTENDFIELDs.HideCriteria] == "1";

            Common.MyPdf.exp_Pdf(showChangeOnly,hideHeaders,hideCriteria,
                myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content).ToDictionary(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DisplayName, y => y.DisplayName),
                subtotal, subavg, subcount, count, groups, avgs, sums, AppNum.companyName, rptHeader ?? comments.ToArray(), dt, rpTitle,
                fontPath, 14, 1, iTextSharp.text.BaseColor.BLACK,
                fontPath, 11, 1, iTextSharp.text.BaseColor.BLACK,//列头字体、大小、样式、颜色
                PathHelper.getTempFolderName(), rpName,
                fontPath, 11, 1, iTextSharp.text.BaseColor.BLACK,//正文字体、大小、样式、颜色
                rptFooter, subcountLabel, sortonCols, isAscending, seq, hideRows, pdfGridLines);
        }

        protected void Excel(int rpid, ReportCriteria[] rpParam = null, CUSTOMRP.BLL.AppHelper.QueryParamsObject[] qpParam = null)
        {
            //string sql = AppHelper.getSql(rpid, me, rpParam);
            //DataTable dt = WebHelper.bllCommon.query(sql);

            CUSTOMRP.Model.REPORT myReport = WebHelper.bllReport.GetModel(me.ID, rpid);

            string rpName = myReport.REPORTNAME;
            string rpTitle = myReport.RPTITLE;

            List<string> comments = new List<string>();
            List<string> avgs = new List<string>();
            List<string> sums = new List<string>();
            List<string> groups = new List<string>();
            List<string> subtotal = new List<string>();
            List<string> subavg = new List<string>();
            List<string> subcount = new List<string>();
            List<string> count = new List<string>();

            //v1.2.0 - Cheong - 2016/07/04 - Add option to hide duplicate items
            //DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(ref comments, ref avgs, ref sums, ref groups, ref subtotal, ref subavg, ref subcount, ref count, me, rpid, true, rpParam);
            DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(me.ID, ref comments, ref avgs, ref sums, ref groups, ref subtotal, ref subavg, ref subcount, ref count, me,
                rpid, myReport.fHideDuplicate, rpParam, QueryParamsToSqlParams(qpParam));

            Dictionary<string, decimal> colwidths = myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content)
                .ToDictionary(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DisplayName,
                y => y.EXCEL_COLWIDTH);

            Dictionary<string, QueryReport.Code.NPOIHelper.ColumnSetting> colSettings = myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content)
                .ToDictionary(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DisplayName,
                y => new QueryReport.Code.NPOIHelper.ColumnSetting(y.FONT_SIZE, y.FONT_BOLD, y.FONT_ITALIC, y.HORIZONTAL_TEXT_ALIGN, y.CELL_FORMAT, y.BACKGROUND_COLOR, y.FONT_COLOR));

            List<string> rptHeader = !String.IsNullOrEmpty(myReport.REPORT_HEADER) ? new List<string>(myReport.REPORT_HEADER.Split('\n')) : null;
            List<string> rptFooter = !String.IsNullOrEmpty(myReport.REPORT_FOOTER) ? new List<string>(myReport.REPORT_FOOTER.Split('\n')) : null;
            string subcountLabel = !String.IsNullOrEmpty(myReport.SUBCOUNT_LABEL) ? myReport.SUBCOUNT_LABEL : null;
            bool pdfGridLines = myReport.PDF_GRID_LINES;
            string fontFamily = !String.IsNullOrEmpty(myReport.FONT_FAMILY) ? myReport.FONT_FAMILY : null;

            ////v1.0.0 - Cheong - 2016/03/17 - Add formatting to criteria
            //comments = comments.Select(x => x.Remove(0, 6).TrimEnd(')')).ToList();

            //v1.8.8 Alex 2018.10.22 Add Group Indent Width
            List<decimal> indentWidths = myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Group).Select(x => x.EXCEL_COLWIDTH).ToList();

            //v1.8.8 Alex 2018.11.01 Move sort orders to rpexcel. Retrieve data here - Begin 
            var tmp = myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.SortOn);
            List<string> sortonCols = tmp.Select(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DisplayName).ToList();
            List<bool> isAscending = tmp.Select(x => x.IS_ASCENDING).ToList();
            List<int> seq = tmp.Select(x => x.SEQ).ToList();
            //v1.8.8 Alex 2018.11.01 Move sort orders to rpexcel. Retrieve data here - End

            XSSFWorkbook XSSFworkbook = NPOIHelper.GetWorkbookFromDt(dt, rpTitle, myReport.EXTENDFIELD.Split(','), rptHeader, comments,
                myReport.ReportColumns.Where(x => x.ColumnFunc == CUSTOMRP.Model.REPORTCOLUMN.ColumnFuncs.Content).ToDictionary(x => x.ColumnType == CUSTOMRP.Model.REPORTCOLUMN.ColumnTypes.Normal ? x.COLUMNNAME : x.DISPLAYNAME, y => y.DisplayName),
                avgs, sums, groups, subtotal, subavg, subcount, count, colwidths, myReport.PrintOrientation, myReport.PRINT_FITTOPAGE,
                rptFooter, colSettings, fontFamily, indentWidths, subcountLabel, sortonCols, isAscending, seq);

            string fileName = rpName + ".xlsx";
            string folder = PathHelper.getTempFolderName();
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string filePath = folder + PathHelper.getSafePath(fileName);
            using (FileStream outFs = new FileStream(filePath, FileMode.Create))
            {
                XSSFworkbook.Write(outFs);
                //CA2202
                //outFs.Close();
            }
            DownloadFile(filePath, fileName);
        }

        protected void Word(int rpid, ReportCriteria[] rpParam = null, CUSTOMRP.BLL.AppHelper.QueryParamsObject[] qpParam = null)
        {
            CUSTOMRP.Model.REPORT myReport = WebHelper.bllReport.GetModel(me.ID, rpid);
            string WordFilePath = null;
            if ((myReport.WordFile != null) && (File.Exists(g_Config["WordTemplatePath"] + myReport.WordFile.WordFileName)))
            {
                WordFilePath = myReport.WordFile.WordFileName;
            }
            else
            {
                CUSTOMRP.Model.WORDTEMPLATE template = WebHelper.bllWORDTEMPLATE.GetModelByReportID(me.ID, myReport.ID, me.ID);
                if (File.Exists(g_Config["WordTemplatePath"] + template.TemplateFileName))
                {
                    WordFilePath = template.TemplateFileName;
                }
                else
                {
                    Context.Response.Write(String.Format("<script type=\"text/javascript\">alert(\"{0}\");</script>",
                        AppNum.ErrorMsg.filenotfounderror));
                    Context.Response.End();
                    return;
                }
            }

            CUSTOMRP.Model.SOURCEVIEW mySV = WebHelper.bllSOURCEVIEW.GetModel(me.ID, myReport.SVID);
            string[] colnames;

            switch (mySV.SourceType)
            {
                case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.StoredProc:
                    {
                        colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForStoredProc(me.ID, me.DatabaseNAME, mySV.TBLVIEWNAME);
                    }
                    break;
                default:
                    {
                        colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForTblView(me.ID, me.DatabaseNAME, mySV.TBLVIEWNAME);
                    }
                    break;
            }

            //just print .no need save to database.
            List<string> rpcr = new List<string>();
            List<string> comments = new List<string>();
            List<string> avgs = new List<string>();
            List<string> sums = new List<string>();
            List<string> groups = new List<string>();
            List<string> subtotal = new List<string>();
            List<string> subavg = new List<string>();
            List<string> subcount = new List<string>();
            List<string> count = new List<string>();

            //v1.2.0 - Cheong - 2016/07/04 - Add option to hide duplicate items
            //DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(ref comments, ref avgs, ref sums, ref groups, ref subtotal, ref subavg, ref subcount, ref count, me, rpid, true, rpParam);
            DataTable dt = CUSTOMRP.BLL.AppHelper.getDataForReport(me.ID, ref comments, ref avgs, ref sums, ref groups, ref subtotal, ref subavg, ref subcount, ref count, me,
                rpid, myReport.fHideDuplicate, rpParam, QueryParamsToSqlParams(qpParam));

            this.p_fSuppressRender = true;

            string path = g_Config["WordTemplatePath"] + WordFilePath;
            string downloadFilename = myReport.REPORTNAME.Replace(' ', '_') + ".docx";

            // Not calling downloadfile because this function need not write on disk
            using (MemoryStream filestream = MailMerge.PerformMailMergeFromTemplate(path, dt, colnames))
            {
                Context.Response.ContentType = "application/octet-stream";
                //Context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + downloadFilename + "\"");
                Context.Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"; filename*=utf-8''{1}", downloadFilename, HttpUtility.UrlPathEncode(downloadFilename)));
                Context.Response.AddHeader("Content-Length", filestream.Length.ToString());
                byte[] fileBuffer = new byte[filestream.Length];
                filestream.Read(fileBuffer, 0, (int)filestream.Length);
                //CA2202
                //filestream.Close();
                Context.Response.BinaryWrite(fileBuffer);
                Context.Response.End();
            }
        }

        public void DownloadFile(string path, string name)
        {
            System.IO.FileInfo file = null;
            try
            {
                string downloadFilename = Server.UrlEncode(name.Replace(' ', '_'));
                file = new System.IO.FileInfo(path);
                Response.Clear();
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                // 添加头信息，为"文件下载/另存为"对话框指定默认文件名
                //Response.AddHeader("Content-Disposition", "attachment; filename=" + downloadFilename);
                Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"; filename*=utf-8''{1}", downloadFilename, HttpUtility.UrlPathEncode(downloadFilename)));
                // 添加头信息，指定文件大小，让浏览器能够显示下载进度
                Response.AddHeader("Content-Length", file.Length.ToString());
                // 指定返回的是一个不能被客户端读取的流，必须被下载
                Response.ContentType = "application/ms-excel";
                // 把文件流发送到客户端
                Response.WriteFile(file.FullName);
                // 停止页面的执行
                // Response.End();     
                this.p_fSuppressRender = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
        }

        #endregion
    }
}