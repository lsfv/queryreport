using System;
using System.Runtime.Serialization;

namespace CUSTOMRP.Model
{
    [Serializable, DataContract]
    public class ReportCatalog
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int DATABASEID { get; set; }
        [DataMember]
        public string REPORTNAME { get; set; }
        [DataMember]
        public int REPORTGROUP { get; set; }
        [DataMember]
        public int CATEGORY { get; set; }
        [DataMember]
        public int TYPE { get; set; }
        [DataMember]
        public int DEFAULTFORMAT { get; set; }
    }

    [Serializable, DataContract]
    public class ReportCriteria
    {
        [DataMember]
        public int SVID { get; set; }
        [DataMember]
        public int RCID { get; set; }
        [DataMember]
        public int RPID { get; set; }
        [DataMember]
        public string COLUMNNAME { get; set; }
        [DataMember]
        public string CRITERIA1 { get; set; }
        [DataMember]
        public string CRITERIA2 { get; set; }
        [DataMember]
        public string CRITERIA3 { get; set; }
        [DataMember]
        public string CRITERIA4 { get; set; }
        [DataMember]
        public string REPORTNAME { get; set; }
        [DataMember]
        public string SOURCEVIEWNAME { get; set; }
        [DataMember]
        public int CATEGORY { get; set; }
        [DataMember]
        public string DATATYPE { get; set; }
    }

    //v1.8.8 Alex 2018.10.29 - Design Change - Begin
    [Serializable, DataContract]
    public class EmbeddedParam
    {
        [DataMember]
        public int SVID { get; set; }
        [DataMember]
        public int RCID { get; set; }
        [DataMember]
        public int RPID { get; set; }
        [DataMember]
        public string COLUMNNAME { get; set; }
        [DataMember]
        public string CRITERIA1 { get; set; }
        [DataMember]
        public string CRITERIA2 { get; set; }
        [DataMember]
        public string CRITERIA3 { get; set; }
        [DataMember]
        public string CRITERIA4 { get; set; }
        [DataMember]
        public string REPORTNAME { get; set; }
        [DataMember]
        public string SOURCEVIEWNAME { get; set; }
        [DataMember]
        public int CATEGORY { get; set; }
        [DataMember]
        public string DATATYPE { get; set; }
        [DataMember]
        public string DATATYPE2 { get; set; }
        [DataMember]
        public string DATATYPE3 { get; set; }
        [DataMember]
        public string ParamName { get; set; }
        [DataMember]
        public string SqlType { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public string[] EnumValues { get; set; }
    }
    //v1.8.8 Alex 2018.10.29 - Design Change - End

    #region Container Types

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
}
