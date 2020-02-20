using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CUSTOMRP.Model;

namespace QueryReport.Code
{
    public abstract class AppHelper
    {
        internal static string GetHtmlTableString(string[] ExtendedFields, Dictionary<string, string> content, List<string> subtotal, List<string> subavg, List<string> SubCount, List<string> Count, DataTable dt, List<string> sum, List<string> avg, List<string> group, string rptTitle = null, string comment = null,
            string subcountLabel = "Sub Count", List<string> sortonCols = null, List<bool> isAscending = null, List<int> seq = null,
            List<string> hideRows = null)
        {
            bool showChangeOnly = ExtendedFields[CUSTOMRP.Model.REPORT.EXTENDFIELDs.ReportType] == "1";
            bool hideCriteria = ExtendedFields[CUSTOMRP.Model.REPORT.EXTENDFIELDs.HideCriteria] == "1";

            StringBuilder sb = new StringBuilder();
            int columnsCount = dt.Columns.Count;

            if (rptTitle != null)
            {
                sb.AppendFormat("<div data-type=\"rptTitle\">{0}</div>", rptTitle);
            }
            if (comment != null)
            {
                sb.Append(comment);
            }

            //start set value to each cell.
            int[] Subsum_Index = WebHelper.getColumnIndexByColumnName_OrderbyIndex(subtotal.ToArray(), dt.Columns);
            int[] Subavg_Index = WebHelper.getColumnIndexByColumnName_OrderbyIndex(subavg.ToArray(), dt.Columns);
            int[] Subcount_Index = WebHelper.getColumnIndexByColumnName_OrderbyIndex(SubCount.ToArray(), dt.Columns);
            int[] sum_Index = WebHelper.getColumnIndexByColumnName_OrderbyIndex(sum.ToArray(), dt.Columns);
            int[] avg_Index = WebHelper.getColumnIndexByColumnName_OrderbyIndex(avg.ToArray(), dt.Columns);
            int[] count_Index = WebHelper.getColumnIndexByColumnName_OrderbyIndex(Count.ToArray(), dt.Columns);
            int[] groupIndex = WebHelper.getColumnIndexByColumnName(group.ToArray(), dt.Columns);
            int[] sortIndex = WebHelper.getColumnIndexByColumnName(sortonCols.ToArray(), dt.Columns);
            int[] hideIndex = WebHelper.getColumnIndexByColumnName(hideRows.ToArray(), dt.Columns);
            Type[] numeric = new Type[3] { typeof(Double), typeof(Decimal), typeof(Int32) };

            subTotal[] subTotalInfo = new subTotal[Subsum_Index.Length];
            subAvg[] subAvginfo = new subAvg[Subavg_Index.Length];
            subCount[] subcountinfo = new subCount[Subcount_Index.Length];

            sb.Append("<table data-tblname=\"custom\">");
            sb.AppendLine("<tr data-rowtype=\"header\">");
            for (int m = 0; m < columnsCount; m++)
            {
                if (hideIndex.Contains(m))
                {
                    continue;
                }
                DataColumn dc = dt.Columns[m];
                //v1.0.0 - Cheong - 2016/03/21 - Align right for column headers of numeric fields
                if ((dt.Columns[m].DataType == typeof(Double)) || (dt.Columns[m].DataType == typeof(Decimal)) || (dt.Columns[m].DataType == typeof(Int32)))
                {
                    sb.Append("<th style=\"text-align: right\">" + content[dc.ColumnName] + "</th>");
                }
                else
                {
                    //v1.0.0 - Cheong - 2015/05/27 - use DisplayName instead
                    //sb.Append("<th>" + dc.ColumnName + "</th>");
                    sb.Append("<th>" + content[dc.ColumnName] + "</th>");
                }
            }
            sb.Append("</tr>");

            for (int x = 0; x < subTotalInfo.Length; x++)
            {
                subTotalInfo[x].ColumnIndex = Subsum_Index[x];
            }
            for (int y = 0; y < subAvginfo.Length; y++)
            {
                subAvginfo[y].ColumnIndex = Subavg_Index[y];
            }


            OrderedEnumerableRowCollection<DataRow> tmp = null;
            //bool grouped = false;
            // 1. Sort columns which belong to a group (only if the user specified grouping).
            if (sortIndex.Length > 0)
            {
                DateTime tpdatetime;
                //grouped = true;
                if (numeric.Contains(dt.Columns[sortIndex[0]].DataType))
                {
                    tmp = isAscending[0] ? dt.AsEnumerable().OrderBy(f => Convert.ToDouble(f[sortIndex[0]]))
                        : dt.AsEnumerable().OrderByDescending(f => Convert.ToDouble(f[sortIndex[0]]));
                }
                else if (dt.Columns[sortIndex[0]].DataType == typeof(DateTime))
                {
                    tmp = isAscending[0] ? dt.AsEnumerable().OrderBy(f => DateTime.TryParse(Convert.ToString(f[sortIndex[0]]), out tpdatetime) ? tpdatetime : DateTime.MinValue)
                        : dt.AsEnumerable().OrderByDescending(f => DateTime.TryParse(Convert.ToString(f[sortIndex[0]]), out tpdatetime) ? tpdatetime : DateTime.MinValue);
                }
                else
                {
                    tmp = isAscending[0] ? dt.AsEnumerable().OrderBy(f => Convert.ToString(f[sortIndex[0]]))
                        : dt.AsEnumerable().OrderByDescending(f => Convert.ToString(f[sortIndex[0]]));
                }
                for (int z = 1; z < sortIndex.Count(); z++)
                {

                    if (isAscending[z])
                    {
                        var thisI = z;
                        tmp = numeric.Contains(dt.Columns[sortIndex[thisI]].DataType) ? tmp.ThenBy(f => f[sortIndex[thisI]] is DBNull ? 0d : Convert.ToDouble(f[sortIndex[thisI]]))
                            : (dt.Columns[sortIndex[thisI]].DataType == typeof(DateTime) ? tmp.ThenBy(f => DateTime.TryParse(Convert.ToString(f[sortIndex[thisI]]), out tpdatetime) ? tpdatetime : DateTime.MinValue) : tmp.ThenBy(f => Convert.ToString(f[sortIndex[thisI]])));
                    }
                    else
                    {
                        var thisI = z;
                        tmp = numeric.Contains(dt.Columns[sortIndex[thisI]].DataType) ? tmp.ThenByDescending(f => f[sortIndex[thisI]] is DBNull ? 0d : Convert.ToDouble(f[sortIndex[thisI]]))
                            : (dt.Columns[sortIndex[thisI]].DataType == typeof(DateTime) ? tmp.ThenByDescending(f => DateTime.TryParse(Convert.ToString(f[sortIndex[thisI]]), out tpdatetime) ? tpdatetime : DateTime.MinValue) : tmp.ThenByDescending(f => Convert.ToString(f[sortIndex[thisI]])));
                    }
                }
                //v1.8.8 Ben 2019.10.31 - CopyToDataTable Must have rows otherwise it cause error
                //dt = tmp.CopyToDataTable();
                if (tmp.Any()) dt = tmp.CopyToDataTable();
            }


            int tempRowsCount = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (!showChangeOnly)
                {
                    if (groupIndex.Length > 0)
                    {
                        #region group header

                        bool GroupBegin = false;

                        if (i == 0)
                        {
                            GroupBegin = true;
                        }
                        else
                        {
                            for (int g_i = 0; g_i < groupIndex.Count(); g_i++)
                            {
                                if (dt.Rows[i][groupIndex[g_i]].ToString() != dt.Rows[i - 1][groupIndex[g_i]].ToString())
                                {
                                    GroupBegin = true;
                                    break;
                                }
                            }
                        }

                        if (GroupBegin)
                        {
                            tempRowsCount = 0;

                            if (!Convert.IsDBNull(dt.Rows[i][groupIndex[0]]) && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][groupIndex[0]])))
                            {
                                sb.Append("<tr data-rowtype=\"groupheader\">");
                                sb.AppendFormat("<td{0}>", dt.Columns.Count != 1 ? String.Format(" colspan=\"{0}\"", dt.Columns.Count) : String.Empty);
                                StringBuilder sbgrouptitle = new StringBuilder();
                                //v1.0.0 - Cheong - 2015/05/27 - Use Displayname instead
                                //sbgrouptitle.AppendFormat("{0} : {1}", group[0], dt.Rows[i][groupIndex[0]]);
                                //sbgrouptitle.AppendFormat("{0} : {1}", content[group[0]], dt.Rows[i][groupIndex[0]]);
                                if (!Convert.IsDBNull(dt.Rows[i][groupIndex[0]]))
                                {
                                    if (dt.Columns[groupIndex[0]].DataType == typeof(DateTime))
                                    {
                                        string l_strDateTime = ((DateTime)dt.Rows[i][groupIndex[0]]).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                        l_strDateTime = l_strDateTime.Replace("1900-01-01", "").Trim();
                                        l_strDateTime = l_strDateTime.Replace("00:00:00", "").Trim();

                                        sbgrouptitle.AppendFormat("{0} : {1}", content[group[0]], l_strDateTime);
                                    }
                                    else
                                    {
                                        sbgrouptitle.AppendFormat("{0} : {1}", content[group[0]], dt.Rows[i][groupIndex[0]]);
                                    }
                                }
                                else
                                {
                                    sbgrouptitle.AppendFormat("{0} : {1}", content[group[0]], String.Empty);
                                }

                                for (int g_i = 1; g_i < groupIndex.Length; g_i++)
                                {
                                    //v1.0.0 - Cheong - 2015/05/27 - Same as above comment
                                    //sbgrouptitle.AppendFormat("     {0} : {1}", group[g_i], dt.Rows[i][groupIndex[g_i]]);
                                    //sbgrouptitle.AppendFormat("     {0} : {1}", content[group[g_i]], dt.Rows[i][groupIndex[g_i]]);
                                    if (!Convert.IsDBNull(dt.Rows[i][groupIndex[0]]))
                                    {
                                        if (dt.Columns[groupIndex[g_i]].DataType == typeof(DateTime))
                                        {
                                            string l_strDateTime = ((DateTime)dt.Rows[i][groupIndex[g_i]]).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                            l_strDateTime = l_strDateTime.Replace("1900-01-01", "").Trim();
                                            l_strDateTime = l_strDateTime.Replace("00:00:00", "").Trim();

                                            sbgrouptitle.AppendFormat("     {0} : {1}", content[group[g_i]], l_strDateTime);
                                        }
                                        else
                                        {
                                            sbgrouptitle.AppendFormat("     {0} : {1}", content[group[g_i]], dt.Rows[i][groupIndex[g_i]]);
                                        }
                                    }
                                    else
                                    {
                                        sbgrouptitle.AppendFormat("     {0} : {1}", content[group[g_i]], String.Empty);
                                    }
                                }
                                sb.Append(sbgrouptitle.ToString());

                                sb.Append("</td>");
                                sb.Append("</tr>");
                            }
                        }

                        #endregion
                    }

                    sb.Append("<tr>");
                    for (int j = 0; j < columnsCount; j++)
                    {
                        if (hideIndex.Contains(j))
                        {
                            continue;
                        }
                        if (!Convert.IsDBNull(dt.Rows[i][j]))
                        {
                            if (dt.Columns[j].DataType == typeof(DateTime))
                            {
                                string l_strDateTime = ((DateTime)dt.Rows[i][j]).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                l_strDateTime = l_strDateTime.Replace("1900-01-01", "").Trim();
                                l_strDateTime = l_strDateTime.Replace("00:00:00", "").Trim();

                                sb.AppendFormat("<td>{0}</td>", l_strDateTime);
                            }
                            else if (dt.Columns[j].DataType == typeof(string))
                            {
                                sb.AppendFormat("<td>{0}</td>", ((string)dt.Rows[i][j]).Replace("\r", "").Replace("\n", "<br />"));
                            }
                            else
                            {
                                sb.AppendFormat("<td>{0}</td>", dt.Rows[i][j]);
                            }
                        }
                        else
                        {
                            sb.Append("<td></td>");
                        }
                    }
                    sb.Append("</tr>");

                    //weather need sub .
                    if (groupIndex.Length > 0)
                    {
                        #region Grouping

                        bool SubTotal = false;
                        decimal dectemp;

                        if (i == dt.Rows.Count - 1)
                        {
                            SubTotal = true;
                        }
                        else
                        {
                            for (int g_i = 0; g_i < groupIndex.Count(); g_i++)
                            {
                                if (dt.Rows[i][groupIndex[g_i]].ToString() != dt.Rows[i + 1][groupIndex[g_i]].ToString())
                                {
                                    SubTotal = true;
                                    break;
                                }
                            }
                        }

                        tempRowsCount++;

                        for (int j = 0; j < subTotalInfo.Length; j++)
                        {
                            //v1.1.0 - Cheong - 2016/05/27 - Do not throw exception when attempt to sum a field that can have null value
                            //subTotalInfo[j].total += !Convert.IsDBNull(dt.Rows[i][subTotalInfo[j].ColumnIndex]) ? Decimal.Parse(dt.Rows[i][subTotalInfo[j].ColumnIndex].ToString()) : 0M;
                            if (Decimal.TryParse(dt.Rows[i][subTotalInfo[j].ColumnIndex].ToString(), out dectemp))
                            {
                                subTotalInfo[j].total += dectemp;
                            }
                        }
                        for (int j = 0; j < subAvginfo.Length; j++)
                        {
                            //v1.1.0 - Cheong - 2016/05/27 - Do not throw exception when attempt to sum a field that can have null value
                            //subAvginfo[j].total += !Convert.IsDBNull(dt.Rows[i][subAvginfo[j].ColumnIndex]) ? Decimal.Parse(dt.Rows[i][subAvginfo[j].ColumnIndex].ToString()) : 0M;
                            if (Decimal.TryParse(dt.Rows[i][subAvginfo[j].ColumnIndex].ToString(), out dectemp))
                            {
                                subAvginfo[j].total += dectemp;
                            }
                        }

                        #region sub total

                        if (SubTotal && Subsum_Index.Count() > 0)
                        {
                            sb.Append("<tr>");
                            for (int j = 0; j < columnsCount; j++)
                            {
                                if (hideIndex.Contains(j))
                                {
                                    continue;
                                }
                                int array_index = Array.IndexOf(Subsum_Index, j);

                                if (array_index >= 0)
                                {
                                    if (j == 0)
                                    {
                                        sb.Append("<td style='font-weight:bold;'>Total: " + subTotalInfo[array_index].total.ToString() + "</td>");
                                    }
                                    else
                                    {
                                        sb.Append("<td style='font-weight:bold;'>" + subTotalInfo[array_index].total.ToString() + "</td>");
                                    }
                                    subTotalInfo[array_index].total = 0;
                                }
                                else
                                {
                                    if (j == Subsum_Index[0] - 1 && Subsum_Index[0] != 0)
                                    {
                                        sb.Append("<td style='font-weight:bold;'>Total</td>");
                                    }
                                    else
                                    {
                                        sb.Append("<td></td>");
                                    }
                                }
                            }
                            sb.Append("</tr>");
                        }

                        #endregion

                        #region sub count

                        if (SubTotal && Subcount_Index.Count() > 0 && !Convert.IsDBNull(dt.Rows[i][groupIndex[0]]) && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][groupIndex[0]])))
                        {
                            sb.Append("<tr>");
                            for (int j = 0; j < columnsCount; j++)
                            {
                                if (hideIndex.Contains(j))
                                {
                                    continue;
                                }
                                int array_index = Array.IndexOf(Subcount_Index, j);
                                if (array_index >= 0)
                                {
                                    if (j == 0)
                                    {
                                        sb.Append("<td style='font-weight:bold;'>");
                                        sb.Append(subcountLabel);
                                        sb.Append(": " + tempRowsCount.ToString() + "</td>");
                                    }
                                    else
                                    {
                                        sb.Append("<td style='font-weight:bold;'>" + tempRowsCount.ToString() + "</td>");
                                    }
                                }
                                else
                                {
                                    if (j == Subcount_Index[0] - 1 && Subcount_Index[0] != 0)
                                    {
                                        sb.Append("<td style='font-weight:bold;'>");
                                        sb.Append(subcountLabel);
                                        sb.Append("</td>");
                                    }
                                    else
                                    {
                                        sb.Append("<td></td>");
                                    }
                                }
                            }
                            sb.Append("</tr>");
                        }

                        #endregion

                        #region sub avg

                        if (SubTotal && Subavg_Index.Count() > 0)
                        {
                            sb.Append("<tr>");
                            for (int j = 0; j < columnsCount; j++)
                            {
                                if (hideIndex.Contains(j))
                                {
                                    continue;
                                }
                                int array_index = Array.IndexOf(Subavg_Index, j);
                                if (array_index >= 0)
                                {
                                    if (j == 0)
                                    {
                                        if (tempRowsCount > 0)
                                        {
                                            sb.Append("<td style='font-weight:bold;'>Avg: " + (subAvginfo[array_index].total / tempRowsCount).ToString("#.##") + "</td>");
                                        }
                                    }
                                    else
                                    {
                                        if (tempRowsCount > 0)
                                        {
                                            sb.Append("<td style='font-weight:bold;'>" + (subAvginfo[array_index].total / tempRowsCount).ToString("#.##") + "</td>");
                                        }
                                    }
                                    subAvginfo[array_index].total = 0;
                                }
                                else
                                {
                                    if (j == Subavg_Index[0] - 1 && Subavg_Index[0] != 0)
                                    {
                                        sb.Append("<td style='font-weight:bold;'>Avg</td>");
                                    }
                                    else
                                    {
                                        sb.Append("<td></td>");
                                    }
                                }
                            }
                            sb.Append("</tr>");
                        }

                        #endregion

                        #endregion
                    }
                }
                else
                {
                    //showChangeOnly

                    //v1.2.0 Kim 2016.11.16 add grouping logic to show changed only case( hidden flag seem irrelevant)
                    //bool hidden = false;
                    //if (groupIndex.Length > 0 && i > 0)
                    //{
                    //    int sameCount = 0;
                    //    for (int g_i = 0; g_i < groupIndex.Count(); g_i++)
                    //    {
                    //        if (dt.Rows[i][groupIndex[g_i]].ToString() == dt.Rows[i - 1][groupIndex[g_i]].ToString())
                    //        {
                    //            sameCount++;
                    //        }
                    //    }
                    //    if (sameCount == groupIndex.Length)
                    //    {
                    //        hidden = true;
                    //    }
                    //}

                    //v1.2.0 Kim 2016.11.16 add grouping logic to show changed only case
                    if (groupIndex.Length > 0)
                    {
                        #region group header

                        bool GroupBegin = false;

                        if (i == 0)
                        {
                            GroupBegin = true;
                        }
                        else
                        {
                            for (int g_i = 0; g_i < groupIndex.Count(); g_i++)
                            {
                                if (dt.Rows[i][groupIndex[g_i]].ToString() != dt.Rows[i - 1][groupIndex[g_i]].ToString())
                                {
                                    GroupBegin = true;
                                    break;
                                }
                            }
                        }

                        if (GroupBegin)
                        {
                            tempRowsCount = 0;

                            sb.Append("<tr data-rowtype=\"groupheader\">");
                            sb.AppendFormat("<td{0}>", dt.Columns.Count != 1 ? String.Format(" colspan=\"{0}\"", dt.Columns.Count) : String.Empty);
                            StringBuilder sbgrouptitle = new StringBuilder();
                            //v1.0.0 - Cheong - 2015/05/27 - Use Displayname instead
                            //sbgrouptitle.AppendFormat("{0} : {1}", group[0], dt.Rows[i][groupIndex[0]]);
                            //sbgrouptitle.AppendFormat("{0} : {1}", content[group[0]], dt.Rows[i][groupIndex[0]]);
                            if (!Convert.IsDBNull(dt.Rows[i][groupIndex[0]]))
                            {
                                if (dt.Columns[groupIndex[0]].DataType == typeof(DateTime))
                                {
                                    string l_strDateTime = ((DateTime)dt.Rows[i][groupIndex[0]]).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                    l_strDateTime = l_strDateTime.Replace("1900-01-01", "").Trim();
                                    l_strDateTime = l_strDateTime.Replace("00:00:00", "").Trim();

                                    sbgrouptitle.AppendFormat("{0} : {1}", content[group[0]], l_strDateTime);
                                }
                                else
                                {
                                    sbgrouptitle.AppendFormat("{0} : {1}", content[group[0]], dt.Rows[i][groupIndex[0]]);
                                }
                            }
                            else
                            {
                                sbgrouptitle.AppendFormat("{0} : {1}", content[group[0]], String.Empty);
                            }

                            for (int g_i = 1; g_i < groupIndex.Length; g_i++)
                            {
                                //v1.0.0 - Cheong - 2015/05/27 - Same as above comment
                                //sbgrouptitle.AppendFormat("     {0} : {1}", group[g_i], dt.Rows[i][groupIndex[g_i]]);
                                //sbgrouptitle.AppendFormat("     {0} : {1}", content[group[g_i]], dt.Rows[i][groupIndex[g_i]]);
                                if (!Convert.IsDBNull(dt.Rows[i][groupIndex[0]]))
                                {
                                    if (dt.Columns[groupIndex[g_i]].DataType == typeof(DateTime))
                                    {
                                        string l_strDateTime = ((DateTime)dt.Rows[i][groupIndex[g_i]]).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                        l_strDateTime = l_strDateTime.Replace("1900-01-01", "").Trim();
                                        l_strDateTime = l_strDateTime.Replace("00:00:00", "").Trim();

                                        sbgrouptitle.AppendFormat("     {0} : {1}", content[group[g_i]], l_strDateTime);
                                    }
                                    else
                                    {
                                        sbgrouptitle.AppendFormat("     {0} : {1}", content[group[g_i]], dt.Rows[i][groupIndex[g_i]]);
                                    }
                                }
                                else
                                {
                                    sbgrouptitle.AppendFormat("     {0} : {1}", content[group[g_i]], String.Empty);
                                }
                            }
                            sb.Append(sbgrouptitle.ToString());

                            sb.Append("</td>");
                            sb.Append("</tr>");
                        }

                        #endregion
                    }

                    sb.Append("<tr>");
                    for (int j = 0; j < columnsCount; j++)
                    {
                        if (hideIndex.Contains(j))
                        {
                            continue;
                        }
                        if (!Convert.IsDBNull(dt.Rows[i][j]))
                        {
                            //v1.0.0 - Cheong - 2016/03/17 - In "Show changed data only" mode, if the first column value is different, show it.
                            //if (((i != 0) && (dt.Rows[i][j].Equals(dt.Rows[i - 1][j]))) || hidden)
                            //v1.2.0 Kim 2016.11.16 add grouping logic to show changed only case(hidden flag seem irrelevant)
                            //if (((i != 0) && (dt.Rows[i][j].Equals(dt.Rows[i - 1][j]) && (dt.Rows[i][0].Equals(dt.Rows[i - 1][0])))) || hidden)
                            if (((i != 0) && (dt.Rows[i][j].Equals(dt.Rows[i - 1][j]) && (dt.Rows[i][0].Equals(dt.Rows[i - 1][0])))))
                            {
                                //v1.1.0 - Cheong - 2016/06/07 - "Show changed data only" will change text color to light grey instead of inserting blank
                                //sb.Append("<td></td>");
                                if (dt.Columns[j].DataType == typeof(DateTime))
                                {
                                    string l_strDateTime = ((DateTime)dt.Rows[i][j]).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                    l_strDateTime = l_strDateTime.Replace("1900-01-01", "").Trim();
                                    l_strDateTime = l_strDateTime.Replace("00:00:00", "").Trim();

                                    sb.AppendFormat("<td style=\"color: #C0C0C0\">{0}</td>", l_strDateTime);
                                }
                                else if (dt.Columns[j].DataType == typeof(string))
                                {
                                    sb.AppendFormat("<td style=\"color: #C0C0C0\">{0}</td>", ((string)dt.Rows[i][j]).Replace("\r", "").Replace("\n", "<br />"));
                                }
                                else
                                {
                                    sb.AppendFormat("<td style=\"color: #C0C0C0\">{0}</td>", dt.Rows[i][j]);
                                }
                            }
                            else
                            {
                                //sb.Append("<td>" + dt.Rows[i][j].ToString() + "</td>");
                                if (dt.Columns[j].DataType == typeof(DateTime))
                                {
                                    string l_strDateTime = ((DateTime)dt.Rows[i][j]).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                    l_strDateTime = l_strDateTime.Replace("1900-01-01", "").Trim();
                                    l_strDateTime = l_strDateTime.Replace("00:00:00", "").Trim();

                                    sb.AppendFormat("<td>{0}</td>", l_strDateTime);
                                }
                                else if (dt.Columns[j].DataType == typeof(string))
                                {
                                    sb.AppendFormat("<td>{0}</td>", ((string)dt.Rows[i][j]).Replace("\r", "").Replace("\n", "<br />"));
                                }
                                else
                                {
                                    sb.AppendFormat("<td>{0}</td>", dt.Rows[i][j]);
                                }
                            }
                        }
                        else
                        {
                            sb.Append("<td></td>");
                        }
                    }
                    sb.Append("</tr>");

                    //v1.2.0 Kim 2016.11.16 add grouping logic to show changed only case
                    //weather need sub .
                    if (groupIndex.Length > 0)
                    {
                        #region Grouping

                        bool SubTotal = false;
                        decimal dectemp;

                        if (i == dt.Rows.Count - 1)
                        {
                            SubTotal = true;
                        }
                        else
                        {
                            for (int g_i = 0; g_i < groupIndex.Count(); g_i++)
                            {
                                if (dt.Rows[i][groupIndex[g_i]].ToString() != dt.Rows[i + 1][groupIndex[g_i]].ToString())
                                {
                                    SubTotal = true;
                                    break;
                                }
                            }
                        }

                        tempRowsCount++;

                        for (int j = 0; j < subTotalInfo.Length; j++)
                        {
                            //v1.1.0 - Cheong - 2016/05/27 - Do not throw exception when attempt to sum a field that can have null value
                            //subTotalInfo[j].total += !Convert.IsDBNull(dt.Rows[i][subTotalInfo[j].ColumnIndex]) ? Decimal.Parse(dt.Rows[i][subTotalInfo[j].ColumnIndex].ToString()) : 0M;
                            if (Decimal.TryParse(dt.Rows[i][subTotalInfo[j].ColumnIndex].ToString(), out dectemp))
                            {
                                subTotalInfo[j].total += dectemp;
                            }
                        }
                        for (int j = 0; j < subAvginfo.Length; j++)
                        {
                            //v1.1.0 - Cheong - 2016/05/27 - Do not throw exception when attempt to sum a field that can have null value
                            //subAvginfo[j].total += !Convert.IsDBNull(dt.Rows[i][subAvginfo[j].ColumnIndex]) ? Decimal.Parse(dt.Rows[i][subAvginfo[j].ColumnIndex].ToString()) : 0M;
                            if (Decimal.TryParse(dt.Rows[i][subAvginfo[j].ColumnIndex].ToString(), out dectemp))
                            {
                                subAvginfo[j].total += dectemp;
                            }
                        }

                        #region sub total

                        if (SubTotal && Subsum_Index.Count() > 0)
                        {
                            sb.Append("<tr>");
                            for (int j = 0; j < columnsCount; j++)
                            {
                                if (hideIndex.Contains(j))
                                {
                                    continue;
                                }
                                int array_index = Array.IndexOf(Subsum_Index, j);

                                if (array_index >= 0)
                                {
                                    if (j == 0)
                                    {
                                        sb.Append("<td style='font-weight:bold;'>Total: " + subTotalInfo[array_index].total.ToString() + "</td>");
                                    }
                                    else
                                    {
                                        sb.Append("<td style='font-weight:bold;'>" + subTotalInfo[array_index].total.ToString() + "</td>");
                                    }
                                    subTotalInfo[array_index].total = 0;
                                }
                                else
                                {
                                    if (j == Subsum_Index[0] - 1 && Subsum_Index[0] != 0)
                                    {
                                        sb.Append("<td style='font-weight:bold;'>Total</td>");
                                    }
                                    else
                                    {
                                        sb.Append("<td></td>");
                                    }
                                }
                            }
                            sb.Append("</tr>");
                        }

                        #endregion

                        #region sub count

                        if (SubTotal && Subcount_Index.Count() > 0)
                        {
                            sb.Append("<tr>");
                            for (int j = 0; j < columnsCount; j++)
                            {
                                if (hideIndex.Contains(j))
                                {
                                    continue;
                                }
                                int array_index = Array.IndexOf(Subcount_Index, j);
                                if (array_index >= 0)
                                {
                                    if (j == 0)
                                    {
                                        sb.Append("<td style='font-weight:bold;'>Count: " + tempRowsCount.ToString() + "</td>");
                                    }
                                    else
                                    {
                                        sb.Append("<td style='font-weight:bold;'>" + tempRowsCount.ToString() + "</td>");
                                    }
                                }
                                else
                                {
                                    if (j == Subcount_Index[0] - 1 && Subcount_Index[0] != 0)
                                    {
                                        sb.Append("<td style='font-weight:bold;'>Count</td>");
                                    }
                                    else
                                    {
                                        sb.Append("<td></td>");
                                    }
                                }
                            }
                            sb.Append("</tr>");
                        }

                        #endregion

                        #region sub avg

                        if (SubTotal && Subavg_Index.Count() > 0)
                        {
                            sb.Append("<tr>");
                            for (int j = 0; j < columnsCount; j++)
                            {
                                if (hideIndex.Contains(j))
                                {
                                    continue;
                                }
                                int array_index = Array.IndexOf(Subavg_Index, j);
                                if (array_index >= 0)
                                {
                                    if (j == 0)
                                    {
                                        if (tempRowsCount > 0)
                                        {
                                            sb.Append("<td style='font-weight:bold;'>Avg: " + (subAvginfo[array_index].total / tempRowsCount).ToString("#.##") + "</td>");
                                        }
                                    }
                                    else
                                    {
                                        if (tempRowsCount > 0)
                                        {
                                            sb.Append("<td style='font-weight:bold;'>" + (subAvginfo[array_index].total / tempRowsCount).ToString("#.##") + "</td>");
                                        }
                                    }
                                    subAvginfo[array_index].total = 0;
                                }
                                else
                                {
                                    if (j == Subavg_Index[0] - 1 && Subavg_Index[0] != 0)
                                    {
                                        sb.Append("<td style='font-weight:bold;'>Avg</td>");
                                    }
                                    else
                                    {
                                        sb.Append("<td></td>");
                                    }
                                }
                            }
                            sb.Append("</tr>");
                        }

                        #endregion

                        #endregion
                    }
                }
            }


            #region rp sum,avg,count

            IList<decimal> sum_result = new List<decimal>(sum_Index.Length);
            IList<decimal> avg_result = new List<decimal>(avg_Index.Length);

            for (int j = 0; j < sum_Index.Length; j++)
            {
                sum_result.Add(0);
            }

            for (int j = 0; j < avg_Index.Length; j++)
            {
                avg_result.Add(0);
            }


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                decimal _tmp;
                for (int j = 0; j < sum_Index.Length; j++)
                {
                    if (Decimal.TryParse(dt.Rows[i][sum_Index[j]].ToString(), out _tmp))
                    {
                        sum_result[j] += !Convert.IsDBNull(dt.Rows[i][sum_Index[j]]) ? _tmp : 0M;
                    }
                }

                for (int j = 0; j < avg_Index.Length; j++)
                {
                    if (Decimal.TryParse(dt.Rows[i][avg_Index[j]].ToString(), out _tmp))
                    {
                        avg_result[j] += !Convert.IsDBNull(dt.Rows[i][avg_Index[j]]) ? _tmp : 0M;
                    }
                }
            }

            #region Total

            if (sum_Index.Length > 0)
            {
                sb.Append("<tr data-rowtype=\"summary\">");
                for (int i = 0; i < columnsCount; i++)
                {
                    if (hideIndex.Contains(i))
                    {
                        continue;
                    }
                    int contain_index = Array.IndexOf(sum_Index, i);

                    if (contain_index >= 0)
                    {
                        if (sum_Index[0] == 0 && i == 0)
                            sb.Append("<td style='font-weight:bold;'>Report Total:" + (sum_result[0]).ToString() + "</td>");
                        else
                        {
                            sb.Append("<td style='font-weight:bold;'>" + (sum_result[contain_index]).ToString() + "</td>");
                        }
                    }
                    else
                    {
                        if (sum_Index[0] != 0 && i == sum_Index[0] - 1)
                        {
                            sb.Append("<td style='font-weight:bold;'>Report Total</td>");
                        }
                        else
                        {
                            sb.Append("<td></td>");
                        }
                    }
                }
                sb.Append("</tr>");
            }

            #endregion Total

            #region Count

            if (count_Index.Length > 0)
            {
                sb.Append("<tr data-rowtype=\"summary\">");
                for (int i = 0; i < columnsCount; i++)
                {
                    if (hideIndex.Contains(i))
                    {
                        continue;
                    }
                    int contain_index = Array.IndexOf(count_Index, i);

                    if (contain_index >= 0)
                    {
                        if (count_Index[0] == 0 && i == 0)
                            sb.Append("<td style='font-weight:bold;'>Report Count:" + dt.Rows.Count.ToString() + "</td>");
                        else
                        {
                            sb.Append("<td style='font-weight:bold;'>" + dt.Rows.Count.ToString() + "</td>");
                        }
                    }
                    else
                    {
                        if (count_Index[0] != 0 && i == count_Index[0] - 1)
                        {
                            sb.Append("<td style='font-weight:bold;'>Report Count</td>");
                        }
                        else
                        {
                            sb.Append("<td></td>");
                        }
                    }
                }

                sb.Append("</tr>");
            }

            #endregion Count

            #region Avg

            if (avg_Index.Length > 0)
            {
                sb.Append("<tr data-rowtype=\"summary\">");
                for (int i = 0; i < columnsCount; i++)
                {
                    if (hideIndex.Contains(i))
                    {
                        continue;
                    }
                    int contain_index = Array.IndexOf(avg_Index, i);

                    if (contain_index >= 0)
                    {
                        if (avg_Index[0] == 0 && i == 0)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                sb.Append("<td style='font-weight:bold;'>Report Avg:" + (avg_result[0] / dt.Rows.Count).ToString("#.##") + "</td>");
                            }
                            else
                            {
                                sb.Append("<td style='font-weight:bold;'>Report Avg:0</td>");
                            }
                        }
                        else
                        {
                            if (dt.Rows.Count > 0)
                            {
                                sb.Append("<td style='font-weight:bold;'>" + (avg_result[contain_index] / dt.Rows.Count).ToString("#.##") + "</td>");
                            }
                            else
                            {
                                sb.Append("<td style='font-weight:bold;'>Report Avg:0</td>");
                            }
                        }
                    }
                    else
                    {
                        if (avg_Index[0] != 0 && i == avg_Index[0] - 1)
                        {
                            sb.Append("<td style='font-weight:bold;'>Report Avg</td>");
                        }
                        else
                        {
                            sb.Append("<td></td>");
                        }
                    }
                }

                sb.Append("</tr>");
            }

            #endregion Avg

            #endregion rp sum,avg,count

            sb.Append("</table>");

            return sb.ToString();
        }

        public static string FormatData(object x)
        {
            if (x is DateTime) { return ((DateTime)x).ToString("yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture); }
            if (x is Double) { return ((Double)x).ToString("#,##0.00", System.Globalization.CultureInfo.InvariantCulture); }
            if (x is Decimal) { return ((Decimal)x).ToString("#,##0.00", System.Globalization.CultureInfo.InvariantCulture); }
            return Convert.ToString(x);
        }
    }
}