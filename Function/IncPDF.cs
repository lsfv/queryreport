using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web;
using System.Data.SqlClient;
using System.Collections;


namespace Function
{

    public abstract class MyPdf
    {
        #region Fields

        public struct subTotal
        {
            public int ColumnIndex { get; set; }
            public decimal total { get; set; }
        }
        public struct subAvg
        {
            public int ColumnIndex { get; set; }
            public decimal total { get; set; }
        }
        public struct subCount
        {
            public int ColumnIndex { get; set; }
            public int count { get; set; }
        }

        #endregion

        /// <summary>
        /// 转换GridView为PDF文档
        /// </summary>
        /// <param name="sdr_Context">SqlDataReader</param>
        /// <param name="title">标题名称</param>
        /// <param name="fontpath_Title">标题字体路径</param>
        /// <param name="fontsize_Title">标题字体大小</param>
        /// <param name="fontStyle_Title">标题样式</param>
        /// <param name="fontColor_Title">标题颜色</param>
        /// <param name="fontpath_Col">列头字体路径</param>
        /// <param name="fontsize_Col">列头字体大小</param>
        /// <param name="fontStyle_Col">列头字体样式</param>
        /// <param name="fontColor_Col">列头字体颜色</param>
        /// <param name="col_Width">表格总宽度</param>
        /// <param name="arr_Width">每列的宽度</param>
        /// <param name="pdf_Filename">在服务器端保存PDF时的文件名</param>
        /// <param name="FontPath">正文字体路径</param>
        /// <param name="FontSize">正文字体大小</param>
        /// <param name="fontStyle_Context">正文字体样式</param>
        /// <param name="fontColor_Context">正文字体颜色</param>
        /// <param name="para">Criteria rows / Report header, depending on the mode</param>
        /// <returns>返回调用是否成功</returns>
        public static void exp_Pdf(string[] ExtendedFields,
            Dictionary<string, string> content, List<string> subtotal,List<string> subavg,List<string> SubCount, List<string> Count,
            List<string> group, List<string> avg, List<string> sum, string companyname, string[] para, System.Data.DataTable sdr_Context, string title,
            string fontpath_Title, float fontsize_Title, int fontStyle_Title, BaseColor fontColor_Title,
            string fontpath_Col, float fontsize_Col, int fontStyle_Col, BaseColor fontColor_Col,
            string tempFolderPath, string pdf_Filename,
            string FontPath, float FontSize, int fontStyle_Context, BaseColor fontColor_Context,
            string[] reportFooter = null)
        {
            bool showChangeOnly = ExtendedFields[0] == "1";     // CUSTOMERP.Model.REPORT.EXTENDFIELDs.ChangeOnly
            bool hideCriteria = ExtendedFields[1] == "1";       //CUSTOMRP.Model.REPORT.EXTENDFIELDs.HideCriteria

            Single tableWidth = (sdr_Context.Columns.Count * 200 >= 800 ? 800 : sdr_Context.Columns.Count * 200);
            //在服务器端保存PDF时的文件名
            string strFileName = pdf_Filename + ".pdf";
            //初始化一个目标文档类 
            Document document = new Document(PageSize.A4.Rotate(), 0, 0, 10, 10);
            //调用PDF的写入方法流
            //注意FileMode-Create表示如果目标文件不存在，则创建，如果已存在，则覆盖。
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(HttpContext.Current.Server.MapPath(strFileName), FileMode.Create));
            using (FileStream fs = new FileStream(tempFolderPath + strFileName, FileMode.Create))
            {
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                //try
                //{

                //标题字体
                BaseFont basefont_Title = BaseFont.CreateFont(fontpath_Title, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                Font font_Title = new Font(basefont_Title, fontsize_Title, fontStyle_Title, fontColor_Title);
                Font font_COMPANY = new Font(basefont_Title, fontsize_Title - 3, fontStyle_Title, fontColor_Title);
                Font font_CRITERIAL = new Font(basefont_Title, fontsize_Title - 4, fontStyle_Title, fontColor_Title);
                Font font_DATETIME = new Font(basefont_Title, fontsize_Title - 4, fontStyle_Title, fontColor_Title);

                //表格列字体
                BaseFont basefont_Col = BaseFont.CreateFont(fontpath_Col, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                Font font_Col = new Font(basefont_Col, fontsize_Col, fontStyle_Col, fontColor_Col);
                //正文字体
                BaseFont basefont_Context = BaseFont.CreateFont(FontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                FontSelector selector = new FontSelector(); //Cheong - 2015/02/10 - use selector when phrase can have mixed language (like English and Chinese)
                selector.AddFont(FontFactory.GetFont(FontFactory.COURIER, FontSize, fontStyle_Context, fontColor_Context));
                selector.AddFont(new Font(basefont_Context, FontSize, fontStyle_Context, fontColor_Context));           // feeded is SimSun for CHS
                selector.AddFont(FontFactory.GetFont("MHei-Medium", "UniCNS-UCS2-H", FontSize, fontStyle_Context, fontColor_Context));    // add CHT font

                FontSelector boldselector = new FontSelector(); //Cheong - 2015/02/10 - use selector when phrase can have mixed language (like English and Chinese)
                boldselector.AddFont(FontFactory.GetFont(FontFactory.COURIER, FontSize, fontStyle_Context | iTextSharp.text.Font.BOLD, fontColor_Context));
                boldselector.AddFont(new Font(basefont_Context, FontSize, fontStyle_Context | iTextSharp.text.Font.BOLD, fontColor_Context));           // feeded is SimSun for CHS
                boldselector.AddFont(FontFactory.GetFont("MHei-Medium", "UniCNS-UCS2-H", FontSize, fontStyle_Context | iTextSharp.text.Font.BOLD, fontColor_Context));    // add CHT font

                FontSelector greyselector = new FontSelector(); //Cheong - 2015/02/10 - use selector when phrase can have mixed language (like English and Chinese)
                greyselector.AddFont(FontFactory.GetFont(FontFactory.COURIER, FontSize, fontStyle_Context, BaseColor.LIGHT_GRAY));
                greyselector.AddFont(new Font(basefont_Context, FontSize, fontStyle_Context, BaseColor.LIGHT_GRAY));           // feeded is SimSun for CHS
                greyselector.AddFont(FontFactory.GetFont("MHei-Medium", "UniCNS-UCS2-H", FontSize, fontStyle_Context, BaseColor.LIGHT_GRAY));    // add CHT font

                //打开目标文档对象
                document.Open();

                #region Company name

                Paragraph Company = new Paragraph(companyname, font_COMPANY);
                Company.IndentationLeft = 100;
                Company.Alignment = Element.ALIGN_LEFT;

                document.Add(Company);

                #endregion Company name

                #region Report title

                Paragraph p_Title = new Paragraph(title, font_Title);
                p_Title.Alignment = Element.ALIGN_LEFT;
                p_Title.IndentationLeft = 100;
                p_Title.SpacingAfter = 8;
                document.Add(p_Title);

                #endregion Report Title

                #region Criteria

                //v1.1.0 - Cheong - 2016/06/01 - Hide criteria text
                if (!hideCriteria)
                {
                    for (int i = 0; i < para.Length; i++)
                    {
                        Paragraph P_pa = new Paragraph(para[i], font_CRITERIAL);
                        P_pa.Alignment = Element.ALIGN_LEFT;
                        P_pa.IndentationLeft = 100;
                        document.Add(P_pa);
                    }
                }

                #endregion

                #region Print on date

                Paragraph P_date = new Paragraph("Print on : " + DateTime.Now.ToString("yyyy-MM-dd"), font_DATETIME);
                P_date.Alignment = Element.ALIGN_LEFT;
                P_date.IndentationLeft = 100;
                document.Add(P_date);
                P_date.SpacingBefore = 32;

                #endregion

                //根据数据表内容创建一个PDF格式的表
                PdfPTable table = new PdfPTable(sdr_Context.Columns.Count);
                table.SpacingBefore = 42;
                table.TotalWidth = tableWidth;//表格总宽度
                table.LockedWidth = true;//锁定宽度
                List<int> arr = new List<int>();
                for (int jj = 0; jj < sdr_Context.Columns.Count; jj++)
                {
                    arr.Add(100);
                }

                table.SetWidths(arr.ToArray());//设置每列宽度

                //构建列头
                //设置列头背景色
                table.DefaultCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                //设置列头文字水平、垂直居中
                table.DefaultCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                table.DefaultCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;

                // 告诉程序这行是表头，这样页数大于1时程序会自动为你加上表头。
                table.HeaderRows = 1;
                if (sdr_Context.Rows.Count > 0)
                {
                    for (int i = 0; i < sdr_Context.Columns.Count; i++)
                    {
                        //v1.0.0 - Cheong - 2015/05/27 - Use Displayname instead
                        //table.AddCell(new Phrase(sdr_Context.Columns[i].ColumnName, font_Col));
                        table.AddCell(new Phrase(content[sdr_Context.Columns[i].ColumnName], font_Col));
                    }
                    // 添加数据
                    //设置标题靠左居中
                    table.DefaultCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    // 设置表体背景色
                    table.DefaultCell.BackgroundColor = BaseColor.WHITE;
                    table.DefaultCell.BorderColor = BaseColor.WHITE;

                    //start set value to each cell.
                    int[] Subsum_Index = Function.IncWeb.getColumnIndexByColumnName(subtotal.ToArray(), sdr_Context.Columns);
                    int[] Subavg_Index = Function.IncWeb.getColumnIndexByColumnName(subavg.ToArray(), sdr_Context.Columns);
                    int[] Subcount_Index = Function.IncWeb.getColumnIndexByColumnName(SubCount.ToArray(), sdr_Context.Columns);
                    int[] sum_Index = Function.IncWeb.getColumnIndexByColumnName(sum.ToArray(), sdr_Context.Columns);
                    int[] avg_Index = Function.IncWeb.getColumnIndexByColumnName(avg.ToArray(), sdr_Context.Columns);
                    int[] count_Index = Function.IncWeb.getColumnIndexByColumnName(Count.ToArray(), sdr_Context.Columns);
                    int[] groupIndex = Function.IncWeb.getColumnIndexByColumnName(group.ToArray(), sdr_Context.Columns);

                    subTotal[] subTotalInfo = new subTotal[Subsum_Index.Length];
                    subAvg[] subAvginfo = new subAvg[Subavg_Index.Length];
                    subCount[] subcountinfo = new subCount[Subcount_Index.Length];

                    for (int x = 0; x < subTotalInfo.Length; x++)
                    {
                        subTotalInfo[x].ColumnIndex = Subsum_Index[x];
                    }
                    for (int y = 0; y < subAvginfo.Length; y++)
                    {
                        subAvginfo[y].ColumnIndex = Subavg_Index[y];
                    }


                    int tempRowsCount = 0;
                    for (int i = 0; i < sdr_Context.Rows.Count; i++)
                    {
                        //if (showType == "0")
                        //{
                            if (groupIndex.Length > 0)
                            {
                                //v1.0.0 - Cheong - 2016/03/18 - Insert new row before each group (except the first one)
                                if (i != 0)
                                {
                                    for (int j = 0; j < sdr_Context.Columns.Count; j++)
                                    {
                                        table.AddCell(selector.Process(" "));
                                    }
                                }

                                #region group header

                                bool GroupBegin = false;
                                if (i == 0)
                                {
                                    GroupBegin = true;
                                }
                                else
                                {
                                    for (int g_i = 0; g_i < groupIndex.Length; g_i++)
                                    {
                                        if (sdr_Context.Rows[i][groupIndex[g_i]].ToString() != sdr_Context.Rows[i - 1][groupIndex[g_i]].ToString())
                                        {
                                            GroupBegin = true;
                                            break;
                                        }
                                    }
                                }

                                if (GroupBegin)
                                {
                                    tempRowsCount = 0;

                                    StringBuilder sb = new StringBuilder();
                                    //v1.0.0 - Cheong - 2015/05/27 - Use DisplayName instead
                                    //sb.AppendFormat("{0} : {1}", group[0], sdr_Context.Rows[i][groupIndex[0]]);
                                    //sb.AppendFormat("{0} : {1}", content[group[0]], sdr_Context.Rows[i][groupIndex[0]]);
                                    if (!Convert.IsDBNull(sdr_Context.Rows[i][groupIndex[0]]))
                                    {
                                        if (sdr_Context.Columns[groupIndex[0]].DataType == typeof(DateTime))
                                        {
                                            string l_strDateTime = ((DateTime)sdr_Context.Rows[i][groupIndex[0]]).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                            l_strDateTime = l_strDateTime.Replace("1900-01-01", "").Trim();
                                            l_strDateTime = l_strDateTime.Replace("00:00:00", "").Trim();

                                            sb.AppendFormat("{0} : {1}", content[group[0]], l_strDateTime);
                                            //row.CreateCell(j).SetCellValue(((DateTime)dt.Rows[i][j]).ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture));
                                        }
                                        else
                                        {
                                            sb.AppendFormat("{0} : {1}", content[group[0]], sdr_Context.Rows[i][groupIndex[0]]);
                                        }
                                    }
                                    else
                                    {
                                        sb.AppendFormat("{0} : {1}", content[group[0]], String.Empty);
                                    }

                                    for (int g_i = 1; g_i < groupIndex.Length; g_i++)
                                    {
                                        //v1.0.0 - Cheong - 2015/05/27 - Same as above comment
                                        //sb.AppendFormat("     {0} : {1}", group[g_i], sdr_Context.Rows[i][groupIndex[g_i]]);
                                        //sb.AppendFormat("     {0} : {1}", content[group[g_i]], sdr_Context.Rows[i][groupIndex[g_i]]);
                                        if (!Convert.IsDBNull(sdr_Context.Rows[i][groupIndex[g_i]]))
                                        {
                                            if (sdr_Context.Columns[groupIndex[g_i]].DataType == typeof(DateTime))
                                            {
                                                string l_strDateTime = ((DateTime)sdr_Context.Rows[i][groupIndex[g_i]]).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                                l_strDateTime = l_strDateTime.Replace("1900-01-01", "").Trim();
                                                l_strDateTime = l_strDateTime.Replace("00:00:00", "").Trim();

                                                sb.AppendFormat("     {0} : {1}", content[group[g_i]], l_strDateTime);
                                            }
                                            else
                                            {
                                                sb.AppendFormat("     {0} : {1}", content[group[g_i]], sdr_Context.Rows[i][groupIndex[g_i]]);
                                            }
                                        }
                                        else
                                        {
                                            sb.AppendFormat("     {0} : {1}", content[group[g_i]], String.Empty);
                                        }
                                    }

                                    if (sdr_Context.Columns.Count == 1)
                                    {
                                        table.AddCell(new PdfPCell(boldselector.Process(sb.ToString())) {/*BorderWidthTop = table.DefaultCell.BorderWidthTop * 2*/ });
                                    }
                                    else
                                    {
                                        table.AddCell(new PdfPCell(boldselector.Process(sb.ToString())) { /*BorderWidthTop = table.DefaultCell.BorderWidthTop * 2,*/ Colspan = sdr_Context.Columns.Count });
                                    }
                                }

                                #endregion
                            }

                            for (int j = 0; j < sdr_Context.Columns.Count; j++)
                            {
                                if (!Convert.IsDBNull(sdr_Context.Rows[i][j]))
                                {
                                    //v1.0.0 - Cheong - 2016/03/18 - Alter Show changed data only logic
                                    if (showChangeOnly)
                                    {
                                        #region Show changed data only
                                        if ((i != 0) && (sdr_Context.Rows[i][j].Equals(sdr_Context.Rows[i - 1][j]) && (sdr_Context.Rows[i][0].Equals(sdr_Context.Rows[i - 1][0]))))
                                        {
                                            //v1.1.0 - Cheong - 2016/06/07 - "Show changed data only" will change text color to light grey instead of inserting blank
                                            //table.AddCell(selector.Process(String.Empty));
                                            if (sdr_Context.Columns[j].DataType == typeof(DateTime))
                                            {
                                                string l_strDateTime = ((DateTime)sdr_Context.Rows[i][j]).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                                l_strDateTime = l_strDateTime.Replace("1900-01-01", "").Trim();
                                                l_strDateTime = l_strDateTime.Replace("00:00:00", "").Trim();

                                                table.AddCell(greyselector.Process(l_strDateTime));
                                            }
                                            else if ((sdr_Context.Columns[j].DataType == typeof(Double)) || (sdr_Context.Columns[j].DataType == typeof(Decimal))
                                                || (sdr_Context.Columns[j].DataType == typeof(Int32)))
                                            {
                                                table.AddCell(greyselector.Process(Convert.ToDouble(sdr_Context.Rows[i][j]).ToString()));
                                            }
                                            else
                                            {
                                                table.AddCell(greyselector.Process(sdr_Context.Rows[i][j].ToString()));
                                            }
                                        }
                                        else
                                        {
                                            if (sdr_Context.Columns[j].DataType == typeof(DateTime))
                                            {
                                                string l_strDateTime = ((DateTime)sdr_Context.Rows[i][j]).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                                l_strDateTime = l_strDateTime.Replace("1900-01-01", "").Trim();
                                                l_strDateTime = l_strDateTime.Replace("00:00:00", "").Trim();

                                                table.AddCell(selector.Process(l_strDateTime));
                                            }
                                            else if ((sdr_Context.Columns[j].DataType == typeof(Double)) || (sdr_Context.Columns[j].DataType == typeof(Decimal))
                                                || (sdr_Context.Columns[j].DataType == typeof(Int32)))
                                            {
                                                table.AddCell(selector.Process(Convert.ToDouble(sdr_Context.Rows[i][j]).ToString()));
                                            }
                                            else
                                            {
                                                table.AddCell(selector.Process(sdr_Context.Rows[i][j].ToString()));
                                            }
                                        }
                                        #endregion Show changed data only
                                    }
                                    else
                                    {
                                        #region Show all
                                        if (sdr_Context.Columns[j].DataType == typeof(DateTime))
                                        {
                                            string l_strDateTime = ((DateTime)sdr_Context.Rows[i][j]).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                            l_strDateTime = l_strDateTime.Replace("1900-01-01", "").Trim();
                                            l_strDateTime = l_strDateTime.Replace("00:00:00", "").Trim();

                                            table.AddCell(selector.Process(l_strDateTime));
                                        }
                                        else if ((sdr_Context.Columns[j].DataType == typeof(Double)) || (sdr_Context.Columns[j].DataType == typeof(Decimal))
                                            || (sdr_Context.Columns[j].DataType == typeof(Int32)))
                                        {
                                            table.AddCell(selector.Process(Convert.ToDouble(sdr_Context.Rows[i][j]).ToString()));
                                        }
                                        else
                                        {
                                            table.AddCell(selector.Process(sdr_Context.Rows[i][j].ToString()));
                                        }
                                        #endregion Show all
                                    }
                                }
                                else
                                {
                                    table.AddCell(selector.Process(String.Empty));
                                }
                            }
                            //whether need sub .
                            if (groupIndex.Length > 0)
                            {
                                #region Grouping

                                tempRowsCount++;

                                bool SubTotal = false;
                                decimal dectemp;

                                if (i == sdr_Context.Rows.Count - 1)
                                {
                                    SubTotal = true;
                                }
                                else
                                {
                                    for (int g_i = 0; g_i < groupIndex.Length; g_i++)
                                    {
                                        if (sdr_Context.Rows[i][groupIndex[g_i]].ToString() != sdr_Context.Rows[i + 1][groupIndex[g_i]].ToString())
                                        {
                                            SubTotal = true;
                                            break;
                                        }
                                    }
                                }

                                for (int j = 0; j < subTotalInfo.Length; j++)
                                {
                                    //v1.1.0 - Cheong - 2016/05/27 - Do not throw exception when attempt to sum a field that can have null value
                                    //subTotalInfo[j].total += !Convert.IsDBNull(sdr_Context.Rows[i][subTotalInfo[j].ColumnIndex]) ? decimal.Parse(sdr_Context.Rows[i][subTotalInfo[j].ColumnIndex].ToString()) : 0M;
                                    if (Decimal.TryParse(sdr_Context.Rows[i][subTotalInfo[j].ColumnIndex].ToString(), out dectemp))
                                    {
                                        subTotalInfo[j].total += dectemp;
                                    }
                                }
                                for (int j = 0; j < subAvginfo.Length; j++)
                                {
                                    //v1.1.0 - Cheong - 2016/05/27 - Do not throw exception when attempt to sum a field that can have null value
                                    //subAvginfo[j].total += !Convert.IsDBNull(sdr_Context.Rows[i][subAvginfo[j].ColumnIndex]) ? decimal.Parse(sdr_Context.Rows[i][subAvginfo[j].ColumnIndex].ToString()) : 0M;
                                    if (Decimal.TryParse(sdr_Context.Rows[i][subAvginfo[j].ColumnIndex].ToString(), out dectemp))
                                    {
                                        subAvginfo[j].total += dectemp;
                                    }
                                }

                                #region sub total

                                if (SubTotal && Subsum_Index.Length > 0)
                                {
                                    for (int column_index = 0; column_index < sdr_Context.Columns.Count; column_index++)
                                    {
                                        int array_index = Array.IndexOf(Subsum_Index, column_index);
                                        if (array_index >= 0)
                                        {
                                            if (Subsum_Index[0] == column_index && column_index == 0)
                                            {
                                                table.AddCell(selector.Process("Total:" + subTotalInfo[array_index].total.ToString()));
                                            }
                                            else
                                            {
                                                table.AddCell(selector.Process(subTotalInfo[array_index].total.ToString()));
                                            }
                                            subTotalInfo[array_index].total = 0;
                                        }
                                        else
                                        {
                                            if (Subsum_Index[0] != 0 && column_index == Subsum_Index[0] - 1)
                                            {
                                                table.AddCell(selector.Process("Total"));
                                            }
                                            else
                                            {
                                                table.AddCell(selector.Process(""));
                                            }
                                        }
                                    }
                                }

                                #endregion

                                #region sub count

                                if (SubTotal && Subcount_Index.Length > 0)
                                {
                                    for (int column_index = 0; column_index < sdr_Context.Columns.Count; column_index++)
                                    {
                                        int array_index = Array.IndexOf(Subcount_Index, column_index);
                                        if (array_index >= 0)// it is the count column.
                                        {
                                            if (Subcount_Index[0] == column_index && column_index == 0)//first column ' position =0
                                            {
                                                table.AddCell(selector.Process("Count:" + sdr_Context.Rows.Count.ToString()));
                                            }
                                            else
                                            {
                                                table.AddCell(selector.Process(sdr_Context.Rows.Count.ToString()));
                                            }
                                            //subAvginfo[array_index].total = 0;
                                        }
                                        else
                                        {
                                            if (Subcount_Index[0] != 0 && column_index == Subcount_Index[0] - 1)//first column ' index=1
                                            {
                                                table.AddCell(selector.Process("Count"));
                                            }
                                            else
                                            {
                                                table.AddCell(selector.Process(""));
                                            }
                                        }
                                    }
                                }

                                #endregion

                                #region sub avg

                                if (SubTotal && Subavg_Index.Length > 0)
                                {
                                    for (int column_index = 0; column_index < sdr_Context.Columns.Count; column_index++)
                                    {
                                        int array_index = Array.IndexOf(Subavg_Index, column_index);
                                        if (array_index >= 0)
                                        {
                                            if (Subavg_Index[0] == column_index && column_index == 0)
                                            {
                                                if (tempRowsCount > 0)
                                                {
                                                    table.AddCell(selector.Process("Avg:" + ((subAvginfo[array_index].total) / tempRowsCount).ToString("#.##")));
                                                }
                                                else
                                                {
                                                    table.AddCell(selector.Process("Avg:0"));
                                                }
                                            }
                                            else
                                            {
                                                if (tempRowsCount > 0)
                                                {
                                                    table.AddCell(selector.Process(((subAvginfo[array_index].total) / tempRowsCount).ToString("#.##")));
                                                }
                                                else
                                                {
                                                    table.AddCell(selector.Process("Avg:0"));
                                                }
                                            }
                                            subAvginfo[array_index].total = 0;
                                        }
                                        else
                                        {
                                            if (Subavg_Index[0] != 0 && column_index == Subavg_Index[0] - 1)
                                            {
                                                table.AddCell(selector.Process("Avg"));
                                            }
                                            else
                                            {
                                                table.AddCell(selector.Process(""));
                                            }
                                        }
                                    }
                                }

                                #endregion

                                #endregion
                            }
                        //}
                        #region TBD - show changed data only
                        //else if (showType == "1")
                        //{
                        //    bool hidden = false;
                        //    if (groupIndex.Length > 0 && i > 0)
                        //    {
                        //        int sameCount = 0;
                        //        for (int g_i = 0; g_i < groupIndex.Length; g_i++)
                        //        {
                        //            if (sdr_Context.Rows[i][groupIndex[g_i]].ToString() == sdr_Context.Rows[i - 1][groupIndex[g_i]].ToString())
                        //            {
                        //                sameCount++;
                        //            }
                        //        }
                        //        if (sameCount == groupIndex.Length)
                        //        {
                        //            hidden = true;
                        //        }
                        //    }

                        //    for (int j = 0; j < sdr_Context.Columns.Count; j++)
                        //    {
                        //        if (!Convert.IsDBNull(sdr_Context.Rows[i][j]))
                        //        {
                        //            //v1.0.0 - Cheong - 2016/03/17 - In "Show changed data only" mode, if the first column value is different, show it.
                        //            //if (((i != 0) && (sdr_Context.Rows[i][j].Equals(sdr_Context.Rows[i - 1][j]))) || hidden)
                        //            if (((i != 0) && (sdr_Context.Rows[i][j].Equals(sdr_Context.Rows[i - 1][j]) && (sdr_Context.Rows[i][0].Equals(sdr_Context.Rows[i - 1][0])))) || hidden)
                        //            {
                        //                table.AddCell(selector.Process(String.Empty));
                        //            }
                        //            else
                        //            {
                        //                //v1.0.0 - Cheong - 2016/03/15 - Add datatype handling to "Show changed data only"
                        //                //table.AddCell(selector.Process(sdr_Context.Rows[i][j].ToString()));
                        //                if (sdr_Context.Columns[j].DataType == typeof(DateTime))
                        //                {
                        //                    string l_strDateTime = ((DateTime)sdr_Context.Rows[i][j]).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        //                    l_strDateTime = l_strDateTime.Replace("1900-01-01", "").Trim();
                        //                    l_strDateTime = l_strDateTime.Replace("00:00:00", "").Trim();

                        //                    table.AddCell(selector.Process(l_strDateTime));
                        //                }
                        //                else if ((sdr_Context.Columns[j].DataType == typeof(Double)) || (sdr_Context.Columns[j].DataType == typeof(Decimal))
                        //                    || (sdr_Context.Columns[j].DataType == typeof(Int32)))
                        //                {
                        //                    table.AddCell(selector.Process(Convert.ToDouble(sdr_Context.Rows[i][j]).ToString()));
                        //                }
                        //                else
                        //                {
                        //                    table.AddCell(selector.Process(sdr_Context.Rows[i][j].ToString()));
                        //                }
                        //            }
                        //        }
                        //        else
                        //        {
                        //            table.AddCell(selector.Process(String.Empty));
                        //        }
                        //    }
                        //}
                        #endregion TBD - show changed data only
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

                    for (int i = 0; i < sdr_Context.Rows.Count; i++)
                    {
                        for (int j = 0; j < sum_Index.Length; j++)
                        {
                            sum_result[j] += !Convert.IsDBNull(sdr_Context.Rows[i][sum_Index[j]]) ? decimal.Parse(sdr_Context.Rows[i][sum_Index[j]].ToString()) : 0M;
                        }

                        for (int j = 0; j < avg_Index.Length; j++)
                        {
                            avg_result[j] += !Convert.IsDBNull(sdr_Context.Rows[i][avg_Index[j]]) ? decimal.Parse(sdr_Context.Rows[i][avg_Index[j]].ToString()) : 0M;
                        }
                    }


                    //int sumsmallest = min(sum_Index);
                    //int avgsmallest = min(avg_Index);
                    #region Total

                    if (sum_Index.Length > 0)
                    {
                        for (int i = 0; i < sdr_Context.Columns.Count; i++)
                        {
                            int contain_index = Array.IndexOf(sum_Index, i);
                            if (contain_index >= 0)
                            {
                                if (sum_Index[0] == 0 && i == 0)
                                {
                                    table.AddCell(selector.Process("Report Total:" + sum_result[contain_index].ToString()));
                                }
                                else
                                {
                                    table.AddCell(selector.Process(sum_result[contain_index].ToString()));
                                }
                            }
                            else
                            {
                                if (sum_Index[0] != 0 && i == sum_Index[0] - 1)
                                {
                                    table.AddCell(selector.Process("Report Total"));
                                }
                                else
                                {
                                    table.AddCell(selector.Process(""));
                                }
                            }
                        }
                    }

                    #endregion Total

                    #region Count

                    if (count_Index.Length > 0)
                    {
                        for (int i = 0; i < sdr_Context.Columns.Count; i++)
                        {
                            int contain_index = Array.IndexOf(count_Index, i);
                            if (contain_index >= 0)
                            {
                                if (count_Index[0] == 0 && i == 0)
                                {
                                    table.AddCell(selector.Process("Report Count:" + sdr_Context.Rows.Count.ToString()));
                                }
                                else
                                {
                                    table.AddCell(selector.Process(sdr_Context.Rows.Count.ToString()));
                                }
                            }
                            else
                            {
                                if (count_Index[0] != 0 && i == count_Index[0] - 1)
                                {
                                    table.AddCell(selector.Process("Report Count"));
                                }
                                else
                                {
                                    table.AddCell(selector.Process(""));
                                }
                            }
                        }
                    }

                    #endregion Count

                    #region Avg

                    if (avg_Index.Length > 0)
                    {
                        for (int i = 0; i < sdr_Context.Columns.Count; i++)
                        {
                            int contain_index = Array.IndexOf(avg_Index, i);
                            if (contain_index >= 0)
                            {
                                if (avg_Index[0] == 0 && i == 0)
                                {
                                    if (sdr_Context.Rows.Count > 0)
                                    {
                                        table.AddCell(selector.Process("Report Avg:" + (avg_result[contain_index] / sdr_Context.Rows.Count).ToString("#.##")));
                                    }
                                    else
                                    {
                                        table.AddCell(selector.Process("Report Avg:0"));
                                    }
                                }
                                else
                                {
                                    table.AddCell(selector.Process((avg_result[contain_index] / sdr_Context.Rows.Count).ToString("#.##")));
                                }
                            }
                            else
                            {
                                if (avg_Index[0] != 0 && i == avg_Index[0] - 1)
                                {
                                    table.AddCell(selector.Process("Report Avg"));
                                }
                                else
                                {
                                    table.AddCell(selector.Process(""));
                                }
                            }
                        }
                    }

                    #endregion Avg

                    #endregion rp sum,avg,count
                }

                #region Report footer

                if (reportFooter != null)
                {
                    Paragraph P_pa = null;
                    // add new line above
                    P_pa = new Paragraph(String.Empty, font_CRITERIAL);
                    document.Add(P_pa);
                    for (int i = 0; i < reportFooter.Length; i++)
                    {
                        P_pa = new Paragraph(reportFooter[i], font_CRITERIAL);
                        P_pa.Alignment = Element.ALIGN_LEFT;
                        P_pa.IndentationLeft = 100;
                        document.Add(P_pa);
                    }
                }

                #endregion Report footer

                //如果最后一个单元格数据过多，不要移动到下一页显示
                table.SplitLate = false;
                //
                table.SplitRows = true;
                //在目标文档中添加转化后的表数据
                document.Add(table);
                //table.DefaultCell.Width = 0.5F;
                //}
                //catch (Exception)
                //{
                //    throw;
                //}
                //finally
                //{
                //关闭目标文件
                document.Close();
                //关闭写入流
                writer.Close();
                //}
            }

            // 弹出提示框，提示用户是否下载保存到本地
            try
            {
                //这里是你文件在项目中的位置,根目录下就这么写 
                //String FullFileName = System.Web.HttpContext.Current.Server.MapPath(strFileName);
                string FullFileName = tempFolderPath + strFileName;
                FileInfo DownloadFile = new FileInfo(FullFileName);
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.ClearHeaders();
                System.Web.HttpContext.Current.Response.Buffer = false;
                System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename="
                    + System.Web.HttpUtility.UrlEncode(DownloadFile.Name.Replace(' ', '_'), System.Text.Encoding.UTF8));
                System.Web.HttpContext.Current.Response.AppendHeader("Content-Length", DownloadFile.Length.ToString());
                System.Web.HttpContext.Current.Response.WriteFile(DownloadFile.FullName);
            }
            //catch (Exception)
            //{
            //    throw;
            //}
            finally
            {
                System.Web.HttpContext.Current.Response.Flush();
                System.Web.HttpContext.Current.Response.End();
            }
        }

    }
}