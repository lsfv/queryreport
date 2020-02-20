using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace QueryReport.Code
{
    public class MailMerge
    {
        private static readonly XNamespace nsW = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        private static readonly XNamespace nsWP = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing";
        private static readonly XNamespace nsVT = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes";

        private const string guid_CustomPropFmt = "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}";
        enum FieldCharType
        {
            None = 0,
            Begin = 1,
            Seperator = 2,
            End = 3,
        }

        public static MemoryStream PerformMailMergeFromTemplate(string sourcefile, DataTable dt, string[] columnNames)
        {
            XmlWriterSettings xwsettings = new XmlWriterSettings();
            xwsettings.Encoding = new UpperCaseUTF8Encoding(false);

            MemoryStream result = new MemoryStream();

            ZipOutputStream zipSteam = new ZipOutputStream(result);
            zipSteam.SetLevel(3);

            using (FileStream instream = File.Open(sourcefile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ZipInputStream inZipSteam = new ZipInputStream(instream);
                ZipEntry entry = inZipSteam.GetNextEntry();
                MemoryStream entrySteam = null;
                while (entry != null)
                {
                    entrySteam = CopyFromMemoryStream(inZipSteam, entry.Size);

                    switch (entry.Name)
                    {
                        case "word/document.xml":
                            {
                                ProcessDocumentData(ref entrySteam, dt, columnNames);
                                CopyToZipStream(entry.Name, zipSteam, ref entrySteam);
                            }
                            break;
                        case "word/settings.xml":
                            {
                                ProcessSettingsData(ref entrySteam);
                                CopyToZipStream(entry.Name, zipSteam, ref entrySteam);
                            }
                            break;
                        case "word/_rels/settings.xml.rels":
                            {
                                // Do not copy this set of file
                            }
                            break;
                        default:
                            {
                                CopyToZipStream(entry.Name, zipSteam, ref entrySteam);
                            }
                            break;
                    }

                    entry = inZipSteam.GetNextEntry();
                }

                zipSteam.Flush();
                zipSteam.Finish();

            }

            result.Position = 0;

            return result;
        }

        public static MemoryStream CreateBlankMailMergeTemplate(string blanksourcefile, string rptname, string queryname, string[] columnNames)
        {
            string[] safeColumnNames = columnNames.Select(x => GetSafeFieldName(x)).ToArray();
            string datafilename = rptname.Replace(' ', '_') + ".xlsx";

            XmlWriterSettings xwsettings = new XmlWriterSettings();
            xwsettings.Encoding = new UpperCaseUTF8Encoding(false);

            MemoryStream result = new MemoryStream();

            ZipOutputStream zipSteam = new ZipOutputStream(result);
            zipSteam.SetLevel(3);

            using (FileStream instream = File.Open(blanksourcefile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ZipInputStream inZipSteam = new ZipInputStream(instream);
                ZipEntry entry = inZipSteam.GetNextEntry();
                MemoryStream entrySteam = null;
                while (entry != null)
                {
                    entrySteam = CopyFromMemoryStream(inZipSteam, entry.Size);

                    switch (entry.Name)
                    {
                        case "docProps/custom.xml":
                            {
                                PatchCustomProps(ref entrySteam, rptname, queryname);
                                CopyToZipStream(entry.Name, zipSteam, ref entrySteam);
                            }
                            break;
                        case "word/document.xml":
                            {
                                DocumentAddMergeFields(ref entrySteam, safeColumnNames);
                                CopyToZipStream(entry.Name, zipSteam, ref entrySteam);
                            }
                            break;
                        case "word/settings.xml":
                            {
                                PatchSettingsPath(ref entrySteam, datafilename);
                                CopyToZipStream(entry.Name, zipSteam, ref entrySteam);
                            }
                            break;
                        case "word/_rels/settings.xml.rels":
                            {
                                PatchSettingsRelsPath(ref entrySteam, datafilename);
                                CopyToZipStream(entry.Name, zipSteam, ref entrySteam);
                            }
                            break;
                        default:
                            {
                                CopyToZipStream(entry.Name, zipSteam, ref entrySteam);
                            }
                            break;
                    }

                    entry = inZipSteam.GetNextEntry();
                }

                zipSteam.Flush();
                zipSteam.Finish();

            }

            result.Position = 0;

            return result;
        }

        public static MemoryStream ChangeDataFilePath(string sourcefile, string datafilename)
        {
            XmlWriterSettings xwsettings = new XmlWriterSettings();
            xwsettings.Encoding = new UpperCaseUTF8Encoding(false);

            MemoryStream result = new MemoryStream();

            ZipOutputStream zipSteam = new ZipOutputStream(result);
            zipSteam.SetLevel(3);

            using (FileStream instream = File.Open(sourcefile, FileMode.Open))
            {
                ZipInputStream inZipSteam = new ZipInputStream(instream);
                ZipEntry entry = inZipSteam.GetNextEntry();
                MemoryStream entrySteam = null;
                while (entry != null)
                {
                    entrySteam = CopyFromMemoryStream(inZipSteam, entry.Size);

                    switch (entry.Name)
                    {
                        case "word/settings.xml":
                            {
                                PatchSettingsPath(ref entrySteam, datafilename);
                                CopyToZipStream(entry.Name, zipSteam, ref entrySteam);
                            }
                            break;
                        case "word/_rels/settings.xml.rels":
                            {
                                PatchSettingsRelsPath(ref entrySteam, datafilename);
                                CopyToZipStream(entry.Name, zipSteam, ref entrySteam);
                            }
                            break;
                        default:
                            {
                                CopyToZipStream(entry.Name, zipSteam, ref entrySteam);
                            }
                            break;
                    }

                    entry = inZipSteam.GetNextEntry();
                }

                zipSteam.Flush();
                zipSteam.Finish();

            }

            result.Position = 0;

            return result;
        }

        public static System.Collections.Specialized.NameValueCollection ReadCustomPropsFromFile(string sourcefile)
        {
            System.Collections.Specialized.NameValueCollection result = new System.Collections.Specialized.NameValueCollection();

            using (FileStream instream = File.Open(sourcefile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ZipInputStream inZipSteam = new ZipInputStream(instream);
                ZipEntry entry = inZipSteam.GetNextEntry();
                MemoryStream entrySteam = null;
                bool found = false;
                while ((entry != null) && (!found))
                {
                    switch (entry.Name)
                    {
                        case "docProps/custom.xml":
                            {
                                entrySteam = CopyFromMemoryStream(inZipSteam, entry.Size);
                                ReadCustomPropsFromStream(ref entrySteam, ref result);
                                found = true;
                            }
                            break;
                        default:
                            {
                            }
                            break;
                    }

                    entry = inZipSteam.GetNextEntry();
                }
            }

            return result;
        }

        #region Methods related to create blank Mail Merge file

        protected static void PatchCustomProps(ref MemoryStream ms, string rptname, string queryname)
        {
            XElement mainDocumentXML = null;
            XElement Properties = null;
            using (StreamReader sr = new StreamReader(ms))
            {
                mainDocumentXML = XElement.Load(sr);
            }

            mainDocumentXML.RemoveNodes();

            int pid = 2;    // start with 2
            XNamespace mainNS = mainDocumentXML.GetDefaultNamespace();
            Properties = new XElement(mainNS + "property", new XAttribute("fmtid", guid_CustomPropFmt), new XAttribute("pid", pid.ToString()), new XAttribute("name", "RptName"),
                new XElement(nsVT + "lpwstr", rptname));
            mainDocumentXML.Add(Properties);
            pid++;

            Properties = new XElement(mainNS + "property", new XAttribute("fmtid", guid_CustomPropFmt), new XAttribute("pid", pid.ToString()), new XAttribute("name", "QueryName"),
                new XElement(nsVT + "lpwstr", queryname));
            mainDocumentXML.Add(Properties);
            pid++;

            ms = new MemoryStream();
            mainDocumentXML.Save(ms);
            ms.Position = 0;
        }

        protected static void DocumentAddMergeFields(ref MemoryStream ms, string[] columnNames)
        {
            XElement mainDocumentXML = null;
            XElement section = null;
            List<XElement> pages = new List<XElement>();
            string rsid = String.Empty;

            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                mainDocumentXML = XElement.Load(sr);

                XElement mainBody = mainDocumentXML.Descendants(nsW + "body").FirstOrDefault();
                if (mainBody == null) { throw new InvalidDataException("Document is in invalid format."); }

                section = mainBody.Elements().Where(x => x.Name == nsW + "sectPr").First();
                rsid = section.Attribute(nsW + "rsidR").Value;

                pages.Add(CreateGoBackField(rsid, columnNames));

                for (int i = 0; i < columnNames.Length; i++)
                {
                    pages.Add(CreateMergeFields(rsid, columnNames[i]));
                }

                mainBody.Elements().Where(x => x.Name != nsW + "sectPr").Remove();

                mainBody.AddFirst(pages.ToArray());
            }

            mainDocumentXML = mainDocumentXML.DeepCopy();

            ms = new MemoryStream();
            mainDocumentXML.Save(ms);
            ms.Position = 0;
        }

        private static XElement CreateGoBackField(string rsid, string[] fieldname)
        {
            XElement element;

            XElement result = new XElement(nsW + "p");
            result.SetAttributeValue(nsW + "rsidR", rsid);
            result.SetAttributeValue(nsW + "rsidRDefault", rsid);

            //for (int i = 0; i < fieldname.Length; i++)
            //{
            //    fldSimple = new XElement(nsW + "fldSimple", new XAttribute(nsW + "instr", String.Format(" MERGEFIELD {0} ", fieldname[i])),
            //        new XElement(nsW + "r",
            //            new XElement(nsW + "rPr",
            //                new XElement(nsW + "noProof")),
            //            new XElement(nsW + "t"), String.Format("«{0}»", fieldname[i])));

            //    result.Add(fldSimple);
            //}

            #region bookmark
            element = new XElement(nsW + "bookmarkStart", new XAttribute(nsW + "id", "0"), new XAttribute(nsW + "name", "_GoBack"));
            result.Add(element);
            #endregion bookmark

            #region bookmark
            element = new XElement(nsW + "bookmarkEnd", new XAttribute(nsW + "id", "0"));
            result.Add(element);
            #endregion bookmark

            return result;
        }

        private static XElement CreateMergeFields(string rsid, string fieldname)
        {
            const string charheight = "20";
            XElement pPr, fldSimple;

            XElement result = new XElement(nsW + "p");
            result.SetAttributeValue(nsW + "rsidR", rsid);
            result.SetAttributeValue(nsW + "rsidRDefault", rsid);

            #region Paragraph

            pPr = new XElement(nsW + "pPr",
                new XElement(nsW + "rPr",
                    new XElement(nsW + "szCs", new XAttribute(nsW + "val", charheight))));

            result.Add(pPr);

            #endregion Paragraph

            #region Runs

            string mergeFieldName = fieldname.Replace(' ', '_');    // space is not allowed in merge fields

            fldSimple = new XElement(nsW + "fldSimple", new XAttribute(nsW + "instr", String.Format(" MERGEFIELD {0} ", mergeFieldName)),
                new XElement(nsW + "r",
                    new XElement(nsW + "rPr",
                        new XElement(nsW + "noProof")),
                    new XElement(nsW + "t"), String.Format("«{0}»", mergeFieldName)));

            result.Add(fldSimple);

            #endregion Runs

            return result;
        }

        #endregion

        #region Methods related to completing Mail Merge

        protected static void ProcessDocumentData(ref MemoryStream ms, DataTable dt, string[] columnNames)
        {
            XElement mainDocumentXML = null;
            XElement[] srcPageData, destPageData = null;
            List<XElement> pages = new List<XElement>();
            int drawingID = 1;

            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                mainDocumentXML = XElement.Load(sr);

                XElement mainBody = mainDocumentXML.Descendants(nsW + "body").FirstOrDefault();
                if (mainBody == null) { throw new InvalidDataException("Document is in invalid format."); }

                //srcPageData = Array.ConvertAll(mainBody.Elements().Where(x => x.Name != nsW + "sectPr").ToArray(), x => Common.Utility<XElementContainer>.DeepCopy(new XElementContainer(x)).Element);
                srcPageData = Array.ConvertAll(mainBody.Elements().Where(x => x.Name != nsW + "sectPr").ToArray(), x => x.DeepCopy());

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i != 0)
                    {
                        pages.Add(
                            new XElement(nsW + "p",
                                new XElement(nsW + "r",
                                    new XElement(nsW + "br", new XAttribute(nsW + "type", "page")))));
                    }
                    //destPageData = Array.ConvertAll(srcPageData, x => Common.Utility<XElementContainer>.DeepCopy(new XElementContainer(x)).Element);
                    destPageData = Array.ConvertAll(srcPageData, x => x.DeepCopy());
                    foreach (XElement e in destPageData)
                    {
                        pages.Add(PatchDrawingID(replaceData(e, dt.Rows[i], columnNames, String.Empty), ref drawingID));
                    }

                }

                mainBody.Elements().Where(x => x.Name != nsW + "sectPr").Remove();

                #region testing
                //XElement[] bookmarkElement = mainBody.Descendants(nsW + "headerReference").ToArray();
                //foreach (XElement e in bookmarkElement)
                //{
                //    e.Remove();
                //}
                //bookmarkElement = mainBody.Descendants(nsW + "footerReference").ToArray();
                //foreach (XElement e in bookmarkElement)
                //{
                //    e.Remove();
                //}
                #endregion

                mainBody.AddFirst(pages.ToArray());
            }

            mainDocumentXML = mainDocumentXML.DeepCopy();

            ms = new MemoryStream();
            mainDocumentXML.Save(ms);
            ms.Position = 0;
        }

        protected static void ProcessSettingsData(ref MemoryStream ms)
        {
            // This will remove mail merge settings
            XElement mainDocumentXML = null;
            using (StreamReader sr = new StreamReader(ms))
            {
                mainDocumentXML = XElement.Load(sr);
            }

            mainDocumentXML.Descendants(nsW + "mailMerge").Remove();

            ms = new MemoryStream();
            mainDocumentXML.Save(ms);
            ms.Position = 0;
        }

        private static XElement replaceData(XElement current, DataRow dr, string[] columnnames, string parentrsid)
        {
            if (!(current.Value.Contains("MERGEFIELD") || (current.Value.Contains("«") && current.Value.Contains("»"))))
            {
                XElement[] bookmarkElement = current.Elements(nsW + "bookmarkStart").ToArray();
                foreach (XElement e in bookmarkElement)
                {
                    e.Remove();
                }
                bookmarkElement = current.Elements(nsW + "bookmarkEnd").ToArray();
                foreach (XElement e in bookmarkElement)
                {
                    e.Remove();
                }
                return current;
            }

            List<XElement> childs = new List<XElement>();
            int fieldStart = -1;
            int fieldSeperator = -1;
            string FieldName = null;
            bool broken = false;
            XAttribute rsidR = current.Attribute(nsW + "rsidR");
            string rsid = (rsidR != null) ? rsidR.Value : parentrsid;

            XElement[] current_childs = current.Elements().ToArray();
            int i = 0;
            while (i < current_childs.Length)
            {
                if (current_childs[i].Name == (nsW + "fldSimple"))
                {
                    string fieldname = current_childs[i].Attribute(nsW + "instr").Value;
                    fieldname = fieldname.Substring(fieldname.IndexOf("MERGEFIELD ") + 11).Trim();
                    // this is simple field
                    if (columnnames.Contains(fieldname))
                    {
                        XElement newRun = new XElement(nsW + "r",
                            new XElement(nsW + "t", new XAttribute(XNamespace.Xml + "space", "preserve"), new XText(AppHelper.FormatData(dr[fieldname]))));
                        childs.Add(newRun);
                    }
                    else if (columnnames.Where(x => GetSafeFieldName(x) == fieldname).Count() > 0)
                    {
                        string origFieldName = columnnames.Where(x => GetSafeFieldName(x) == fieldname).FirstOrDefault();
                        XElement newRun = new XElement(nsW + "r",
                            new XElement(nsW + "t", new XAttribute(XNamespace.Xml + "space", "preserve"), new XText(AppHelper.FormatData(dr[origFieldName]))));
                        childs.Add(newRun);
                    }
                }
                else
                {
                    switch (IsFieldIndicator(current_childs[i]))
                    {
                        case FieldCharType.Begin:
                            {
                                fieldStart = i;
                            }
                            break;
                        case FieldCharType.Seperator:
                            {
                                fieldSeperator = i;
                            }
                            break;
                        case FieldCharType.End:
                            {
                                fieldStart = fieldSeperator = -1;
                                FieldName = null;
                            }
                            break;
                        default:    // None
                            {
                                //if ((fieldStart == -1) || (fieldSeperator != -1))
                                if (fieldStart == -1)
                                {
                                    //if (FieldName != null)
                                    //{

                                    //    XElement TElement = current_childs[i].Elements(nsW + "t").FirstOrDefault();
                                    //    if (TElement != null)
                                    //    {
                                    //        TElement.ReplaceNodes(new XText(Convert.ToString(dr[FieldName])));
                                    //    }
                                    //}

                                    if (FieldName == null)
                                    {
                                        XElement[] bookmarkElement = current_childs[i].Elements(nsW + "bookmarkStart").ToArray();
                                        foreach (XElement e in bookmarkElement)
                                        {
                                            e.Remove();
                                        }
                                        bookmarkElement = current_childs[i].Elements(nsW + "bookmarkEnd").ToArray();
                                        foreach (XElement e in bookmarkElement)
                                        {
                                            e.Remove();
                                        }
                                        //childs.Add(current_childs[i]);
                                        childs.Add(replaceData(current_childs[i], dr, columnnames, rsid));
                                    }
                                }
                                else
                                {
                                    if (fieldSeperator == -1)   // ignore anything behind seperator
                                    {
                                        // seek mergefieldname
                                        XElement[] instrElement = current_childs[i].Elements(nsW + "instrText").ToArray();
                                        foreach (XElement e in instrElement)
                                        {
                                            FieldName = null;

                                            if (e.Value.Contains("MERGEFIELD "))
                                            {
                                                string[] content = e.Value.Trim().Replace("\"", "").Split(' ');
                                                if (content.Length > 1)
                                                {
                                                    FieldName = e.Value.Trim().Replace("\"", "").Split(' ')[1];
                                                }
                                                else
                                                {
                                                    broken = true;
                                                }
                                            }
                                            else if (broken)
                                            {
                                                FieldName = e.Value.Trim().Replace("\"", "");
                                                broken = false;
                                            }

                                            if (!String.IsNullOrEmpty(FieldName))
                                            {
                                                if (columnnames.Contains(FieldName))  // Word will replace space in field name with underscore, so test this case first
                                                {
                                                    e.ReplaceNodes(new XText(AppHelper.FormatData(dr[FieldName])));
                                                }
                                                else if (columnnames.Where(x => GetSafeFieldName(x) == FieldName).Count() > 0)
                                                {
                                                    string origFieldName = columnnames.Where(x => GetSafeFieldName(x) == FieldName).FirstOrDefault();
                                                    e.ReplaceNodes(new XText(AppHelper.FormatData(dr[origFieldName])));
                                                }
                                            }
                                        }
                                        XElement[] bookmarkElement = current_childs[i].Elements(nsW + "bookmarkStart").ToArray();
                                        foreach (XElement e in bookmarkElement)
                                        {
                                            e.Remove();
                                        }
                                        bookmarkElement = current_childs[i].Elements(nsW + "bookmarkEnd").ToArray();
                                        foreach (XElement e in bookmarkElement)
                                        {
                                            e.Remove();
                                        }
                                        //childs.Add(current_childs[i]);
                                        childs.Add(replaceData(current_childs[i], dr, columnnames, rsid));
                                    }
                                }
                            }
                            break;
                    }
                }
                i++;
            }

            current.ReplaceNodes(childs.AsEnumerable());
            return current;
        }

        private static FieldCharType IsFieldIndicator(XElement current)
        {
            XElement field = current.Elements(nsW + "fldChar").FirstOrDefault();
            if (field == null) { return FieldCharType.None; }
            switch (field.Attribute(nsW + "fldCharType").Value)
            {
                case "begin":
                    {
                        return FieldCharType.Begin;
                    }
                case "end":
                    {
                        return FieldCharType.End;
                    }
                case "separate":
                    {
                        return FieldCharType.Seperator;
                    }
                default:
                    {
                        return FieldCharType.None;
                    }
            }
        }

        private static XElement PatchDrawingID(XElement current, ref int drawingID)
        {
            XElement[] fields = current.Descendants(nsWP + "docPr").ToArray();

            foreach (XElement field in fields)
            {
                field.SetAttributeValue("id", Convert.ToString(drawingID));
                drawingID++;
            }

            return current;
        }

        #endregion

        #region Methods related to change data file path

        protected static void PatchSettingsPath(ref MemoryStream ms, string datafilename)
        {
            XElement mainDocumentXML = null;
            using (StreamReader sr = new StreamReader(ms))
            {
                mainDocumentXML = XElement.Load(sr);
            }

            #region For TAB

            //XElement queryXML = mainDocumentXML.Descendants(nsW + "query").FirstOrDefault();
            //if (queryXML != null)
            //{
            //    queryXML.SetAttributeValue(nsW + "val", String.Format("SELECT * FROM {0}", datafilename));
            //}

            #endregion

            #region for XLSX

            XElement connXML = mainDocumentXML.Descendants(nsW + "connectString").FirstOrDefault();
            if (connXML != null)
            {
                connXML.SetAttributeValue(nsW + "val", String.Format("Provider=Microsoft.ACE.OLEDB.12.0;User ID=Admin;Data Source={0};Mode=Read;Extended Properties=&quot;HDR=YES;IMEX=1;&quot;;Jet OLEDB:System database=&quot;&quot;;Jet OLEDB:Registry Path=&quot;&quot;;Jet OLEDB:Engine Type=37;Jet OLEDB:Database Locking Mode=0;Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Global Bulk Transactions=1;Jet OLEDB:New Database Password=&quot;&quot;;Jet OLEDB:Create System Database=False;Jet OLEDB:Encrypt Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;Jet OLEDB:SFP=False;Jet OLEDB:Support Complex Data=False;Jet OLEDB:Bypass UserInfo Validation=False", datafilename));
            }

            XElement udlXML = mainDocumentXML.Descendants(nsW + "udl").FirstOrDefault();
            if (udlXML != null)
            {
                udlXML.SetAttributeValue(nsW + "val", String.Format("Provider=Microsoft.ACE.OLEDB.12.0;User ID=Admin;Data Source={0};Mode=Read;Extended Properties=&quot;HDR=YES;IMEX=1;&quot;;Jet OLEDB:System database=&quot;&quot;;Jet OLEDB:Registry Path=&quot;&quot;;Jet OLEDB:Engine Type=37;Jet OLEDB:Database Locking Mode=0;Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Global Bulk Transactions=1;Jet OLEDB:New Database Password=&quot;&quot;;Jet OLEDB:Create System Database=False;Jet OLEDB:Encrypt Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;Jet OLEDB:SFP=False;Jet OLEDB:Support Complex Data=False;Jet OLEDB:Bypass UserInfo Validation=False", datafilename));
            }

            #endregion

            ms = new MemoryStream();
            mainDocumentXML.Save(ms);
            ms.Position = 0;
        }

        protected static void PatchSettingsRelsPath(ref MemoryStream ms, string datafilename)
        {
            // This will remove mail merge settings
            XElement mainDocumentXML = null;
            using (StreamReader sr = new StreamReader(ms))
            {
                mainDocumentXML = XElement.Load(sr);
            }

            XElement[] queryXML = mainDocumentXML.Elements().ToArray();
            foreach (XElement item in queryXML)
            {
                if (item.Attribute("Type").Value == "http://schemas.openxmlformats.org/officeDocument/2006/relationships/mailMergeSource")
                {
                    item.SetAttributeValue("Target", datafilename);
                }
            }

            ms = new MemoryStream();
            mainDocumentXML.Save(ms);
            ms.Position = 0;
        }

        #endregion

        #region Methods related to read CustomProp from file

        protected static void ReadCustomPropsFromStream(ref MemoryStream ms, ref System.Collections.Specialized.NameValueCollection nvc)
        {
            XElement mainDocumentXML = null;
            using (StreamReader sr = new StreamReader(ms))
            {
                mainDocumentXML = XElement.Load(sr);
            }

            XNamespace mainNS = mainDocumentXML.GetDefaultNamespace();
            XElement[] properties = mainDocumentXML.Descendants(mainNS + "property").ToArray();
            XElement lpwstr;
            for (int i = 0; i < properties.Length; i++)
            {
                lpwstr = properties[i].Descendants(nsVT + "lpwstr").FirstOrDefault();
                if (lpwstr != null)
                {
                    switch (properties[i].Attribute("name").Value)
                    {
                        case "RptName":
                        case "QueryName":
                            {
                                nvc.Add(properties[i].Attribute("name").Value, lpwstr.Value);
                            }
                            break;
                    }
                }
            }
        }

        #endregion

        #region Common methods

        internal static string GetSafeFieldName(string input)
        {
            //v1.1.0 - Cheong - 2015/05/31 - Add characters that is known to be unsafe to filter
            return input.Replace(' ', '_').Replace("(", "").Replace(")", "").Replace(".", "").Replace("!", "").Replace("`", "").Replace("[", "").Replace("]", "").Replace("'", "").Replace("\"", "");
        }

        private static MemoryStream CopyFromMemoryStream(ZipInputStream zipSteam, long Size)
        {
            MemoryStream ms = new MemoryStream();
            int buffersize = (Size > 2048) ? 2048 : (int)Size;
            byte[] buffer = new byte[buffersize];
            int counter = 0;
            int cnt = 0;

            cnt = zipSteam.Read(buffer, 0, buffersize);
            while (cnt > 0)
            {
                ms.Write(buffer, 0, cnt);

                counter += cnt;
                cnt = zipSteam.Read(buffer, 0, (buffersize + counter > Size) ? (int)(Size - counter) : buffersize);
            }

            ms.Position = 0;

            return ms;
        }

        private static void CopyToZipStream(string name, ZipOutputStream zipSteam, ref MemoryStream inputStream)
        {
            ZipEntry z = new ZipEntry(name);
            z.DateTime = DateTime.Now;
            zipSteam.PutNextEntry(z);

            byte[] buffer = new byte[2048];
            int cnt = inputStream.Read(buffer, 0, 2048);
            while (cnt > 0)
            {
                zipSteam.Write(buffer, 0, cnt);
                cnt = inputStream.Read(buffer, 0, 2048);
            }
        }

        #endregion
    }

    internal static class Helper
    {
        public static XElement DeepCopy(this XElement source)
        {
            return XElement.Parse(source.ToString(SaveOptions.OmitDuplicateNamespaces), LoadOptions.PreserveWhitespace);
        }
    }

    /// <summary>
    /// Open XML Documents does not like BOM markers.
    /// For use with XMLWriter encoding to show upper-cased code page name.
    /// Modified to allow constructor that surpress generation of BOM marker.
    /// </summary>
    internal class UpperCaseUTF8Encoding : System.Text.UTF8Encoding
    {
        // Code from a blog http://www.distribucon.com/blog/CategoryView,category,XML.aspx
        //
        // Dan Miser - Thoughts from Dan Miser
        // Tuesday, January 29, 2008 
        // He used the Reflector to understand the heirarchy of the encoding class
        //
        //      Back to Reflector, and I notice that the Encoding.WebName is the property used to
        //      write out the encoding string. I now create a descendant class of UTF8Encoding.
        //      The class is listed below. Now I just call XmlTextWriter, passing in
        //      UpperCaseUTF8Encoding.UpperCaseUTF8 for the Encoding type, and everything works
        //      perfectly. - Dan Miser
        public UpperCaseUTF8Encoding()
            : base()
        {
        }

        public UpperCaseUTF8Encoding(bool encoderShouldEmitUTF8Identifier)
            : base(encoderShouldEmitUTF8Identifier)
        {
        }

        public override string WebName
        {
            get { return base.WebName.ToUpper(); }
        }

        public static UpperCaseUTF8Encoding UpperCaseUTF8
        {
            get
            {
                if (upperCaseUtf8Encoding == null)
                {
                    upperCaseUtf8Encoding = new UpperCaseUTF8Encoding(false);
                }
                return upperCaseUtf8Encoding;
            }
        }

        private static UpperCaseUTF8Encoding upperCaseUtf8Encoding = null;
    }
}