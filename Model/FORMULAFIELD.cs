using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Common;

namespace CUSTOMRP.Model
{
    public class FORMULAFIELD
    {
        private int _ID;
        private string _FIELDNAME;
        private int _SVID;
        private int? _RPID;
        private int _FUNCTION;

        private string _ControlID;  // volatile field that will not be stored in database, but assigned dynamically just before render
        private string _ParentID;  // volatile field that will not be stored in database, but assigned dynamically just before render

        #region Model

        public int ID
        {
            set { _ID = value; }
            get { return _ID; }
        }
        public string FIELDNAME
        {
            set { _FIELDNAME = value; }
            get { return _FIELDNAME; }
        }
        public int SVID
        {
            set { _SVID = value; }
            get { return _SVID; }
        }
        public int? RPID
        {
            set { _RPID = value; }
            get { return _RPID; }
        }
        public int FUNCTION
        {
            set { _FUNCTION = value; }
            get { return _FUNCTION; }
        }
        public readonly List<FORMULAPARAM> Params = new List<FORMULAPARAM>();

        #endregion Model

        #region Extension

        public enum Functions: int
        {
            [Description("")]
            NONE = 0,
            [Description("Add")]
            Add = 1,
            [Description("Subtract")]
            Subtract = 2,
            [Description("Multiply")]
            Multiply = 3,
            [Description("Divide")]
            Divide = 4,
            [Description("Decode")]
            Decode = 5,
            [Description("IfNull")]
            IfNull = 6,
        }
        public Functions Function
        {
            get { return (Functions)_FUNCTION; }
        }

        /// <summary>
        /// volatile field that will not be stored in database, but assigned dynamically just before render
        /// </summary>
        public string ControlID
        {
            get { return _ControlID; }
        }

        /// <summary>
        /// Sets Control ID, also invoke code to check number of parameters here
        /// </summary>
        /// <param name="strControlID"></param>
        public void SetControlID(string strControlID)
        {
            #region Parameter check

            switch (this.Function)
            {
                case Functions.Add:
                case Functions.Subtract:
                case Functions.Multiply:
                case Functions.Divide:
                case Functions.IfNull:
                    {
                        while (this.Params.Count < 2)
                        {
                            this.Params.Add(new FORMULAPARAM() { PARAMTYPE = 0, VALUE1 = String.Empty });
                        }
                    }
                    break;
                case Functions.Decode:
                    {
                        while (this.Params.Count < 3)
                        {
                            this.Params.Add(new FORMULAPARAM() { PARAMTYPE = 0, VALUE1 = String.Empty });
                        }
                    }
                    break;
                default: // None
                    {
                        while (this.Params.Count < 1)
                        {
                            this.Params.Add(new FORMULAPARAM() { PARAMTYPE = 0, VALUE1 = String.Empty });
                        }
                    }
                    break;
            }

            #endregion

            for (int i = 0; i < this.Params.Count; i++)
            {
                this.Params[i].SetControlID(String.Format("{0}_{1}", strControlID, i));
                this.Params[i].SetParentID(strControlID);
            }
            this._ControlID = strControlID;
        }

        /// <summary>
        /// volatile field that will not be stored in database, but assigned dynamically just before render
        /// </summary>
        public string ParentID
        {
            get { return _ParentID; }
        }

        public void SetParentID(string strFormulaID)
        {
            this._ParentID = strFormulaID;
        }

        public string GetText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (this.Params.Count > 0)
                {
                    switch (this.Function)
                    {
                        #region NONE
                        case Functions.NONE:
                            {
                                // acts as pure bracket
                                sb.AppendFormat(this.Params[0].GetText);
                            }
                            break;
                        #endregion NONE

                        #region Add
                        case Functions.Add:
                            {
                                if (this.Params.Count > 1)
                                {
                                    sb.AppendFormat("({0}", this.Params[0].GetText, this.Params[1].GetText);
                                    for (int i = 1; i < this.Params.Count; i++)
                                    {
                                        sb.AppendFormat(" + {0}", this.Params[i].GetText);
                                    }
                                    sb.Append(")");
                                }
                            }
                            break;
                        #endregion Add

                        #region Subtract
                        case Functions.Subtract:
                            {
                                if (this.Params.Count > 1)
                                {
                                    sb.AppendFormat("({0}", this.Params[0].GetText, this.Params[1].GetText);
                                    for (int i = 1; i < this.Params.Count; i++)
                                    {
                                        sb.AppendFormat(" - {0}", this.Params[i].GetText);
                                    }
                                    sb.Append(")");
                                }
                            }
                            break;
                        #endregion Subtract

                        #region Multiply
                        case Functions.Multiply:
                            if (this.Params.Count > 1)
                            {
                                sb.AppendFormat("({0}", this.Params[0].GetText, this.Params[1].GetText);
                                for (int i = 1; i < this.Params.Count; i++)
                                {
                                    sb.AppendFormat(" * {0}", this.Params[i].GetText);
                                }
                                sb.Append(")");
                            }
                            break;
                        #endregion

                        #region Divide
                        case Functions.Divide:
                            {
                                if (this.Params.Count > 1)
                                {
                                    sb.AppendFormat("({0}", this.Params[0].GetText, this.Params[1].GetText);
                                    for (int i = 1; i < this.Params.Count; i++)
                                    {
                                        sb.AppendFormat(" / {0}", this.Params[i].GetText);
                                    }
                                    sb.Append(")");
                                }
                            }
                            break;
                        #endregion

                        #region Decode
                        case Functions.Decode:
                            {
                                if (this.Params.Count > 2)
                                {
                                    int cnt = 1;
                                    sb.AppendFormat("CASE {0}", this.Params[0].GetText);
                                    while (cnt < this.Params.Count)
                                    {
                                        if (cnt + 2 <= this.Params.Count)
                                        {
                                            // WHEN ... THEN ...
                                            sb.AppendFormat(" WHEN {0} THEN {1}", this.Params[cnt].GetText, this.Params[cnt+1].GetText);
                                            cnt++;
                                        }
                                        else
                                        {
                                            // ELSE
                                            sb.AppendFormat(" ELSE {0}", this.Params[cnt].GetText);
                                        }
                                        cnt++;
                                    }
                                    sb.Append(" END");
                                }
                            }
                            break;
                        #endregion Decode

                        #region IfNull
                        case Functions.IfNull:
                            {
                                if (this.Params.Count == 2)
                                {
                                    sb.AppendFormat("IFNULL({0}, {1})", this.Params[0].GetText, this.Params[1].GetText);
                                }
                            }
                            break;
                        #endregion
                    }
                }
                if (sb.Length == 0)
                {
                    sb.Append("NULL");
                }

                if (String.IsNullOrEmpty(this.FIELDNAME))
                {
                    return sb.ToString();
                }
                else
                {
                    return String.Format("[{0}] = {1}", this._FIELDNAME, sb.ToString());
                }
            }
        }

        /// <summary>
        /// Get HTML representation for formula field edit.
        /// </summary>
        public string GetHtml
        {
            get {
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("<div> <select id=\"ddlFuncName_{0}\" data-formulactrl='true' data-formulaid='{1}' onchange=\"return paramFuncChanged('{0}');\">", this._ControlID, this._ControlID, this.GetColorForLevel());
                foreach (Functions f in Utility.GetValues<Functions>())
                {
                    sb.AppendFormat("<option value=\"{0}\"{1}>{2}</option>", (int)f,
                        this.Function == f ? " selected=\"true\"" : String.Empty,
                        Utility.GetEnumDescription(f));
                }
                sb.Append("</select> ( ");

                if (this.Params.Count > 0)
                {
                    sb.Append(this.Params[0].GetHtml);
                }
                for (int i = 1; i < this.Params.Count; i++)
                {
                    sb.Append(", ").Append(this.Params[i].GetHtml);
                }
                switch (this.Function)
                {
                    case Functions.NONE:    // this function (?) needs exactly 1 parameter
                    case Functions.IfNull:  // this function needs exactly 2 parameters only
                        break;
                    default:
                        {
                            sb.AppendFormat(", <input type=\"button\" id=\"btn_addParam_{0}\" value=\"...\" onclick=\"return paramAdd('{0}');\" />", this.ControlID);
                        }
                        break;
                }
                sb.Append(" ) </div>");
                return sb.ToString();
            }
        }

        public bool AddParamter(string strControlID)
        {
            bool result = false;
            if (strControlID == _ControlID) {
                this.Params.Add(new FORMULAPARAM() { PARAMTYPE = 0, VALUE1 = String.Empty, });
                this.AdjustParamCount();
                return true;
            }
            int i = 0;
            while ((!result) && (i < this.Params.Count))
            {
                if (this.Params[i].ParamType == FORMULAPARAM.ParamTypes.FormulaField)
                {
                    if (this.Params[i].VALUE2 != null)
                    {
                        result = this.Params[i].VALUE2.AddParamter(strControlID);
                    }
                }
                i++;
            }
            return result;
        }

        public bool ChangeFunctionType(string strControlID, Functions newFunc)
        {
            bool result = false;
            int i = 0;
            if (strControlID == _ControlID)
            {
                this.FUNCTION = (int)newFunc;
                this.AdjustParamCount();
                return true;
            }
            i = 0;
            while ((!result) && (i < this.Params.Count))
            {
                if (this.Params[i].ParamType == FORMULAPARAM.ParamTypes.FormulaField)
                {
                    if (this.Params[i].VALUE2 != null)
                    {
                        // Perform parameter type change for inner formula only
                        if ((this.Params[i].ControlID == strControlID) && (newFunc == Functions.NONE))
                        {
                            if (newFunc == Functions.NONE)
                            {
                                // change type to literal
                                this.Params[i].PARAMTYPE = (int)FORMULAPARAM.ParamTypes.Literals;
                                this.Params[i].VALUE2 = null;
                                if (this.Params[i].VALUE1 == null) { this.Params[i].VALUE1 = String.Empty; }
                                result = true;
                            }
                        }
                        else
                        {
                            result = this.Params[i].VALUE2.ChangeFunctionType(strControlID, newFunc);
                        }
                    }
                }
                i++;
            }
            return result;
        }

        public bool ChangeParamText(string strControlID, string newValue, bool isColumnName)
        {
            bool result = false;
            int i = 0;
            while ((!result) && (i < this.Params.Count))
            {
                if (this.Params[i].ParamType != FORMULAPARAM.ParamTypes.FormulaField)
                {
                    if (this.Params[i].ControlID == strControlID)
                    {
                        this.Params[i].VALUE1 = newValue;
                        if (isColumnName)
                        {
                            this.Params[i].PARAMTYPE = (int)FORMULAPARAM.ParamTypes.ColumnName;
                        }
                        else
                        {
                            this.Params[i].PARAMTYPE = (int)FORMULAPARAM.ParamTypes.Literals;
                        }
                        result = true;
                    }
                }
                else
                {
                    if (this.Params[i].VALUE2 != null)
                    {
                        result = this.Params[i].VALUE2.ChangeParamText(strControlID, newValue, isColumnName);
                    }
                }
                i++;
            }
            return result;
        }

        public bool RemoveParam(string strControlID)
        {
            bool result = false;
            int i = 0;
            while ((!result) && (i < this.Params.Count))
            {
                if (this.Params[i].ControlID == strControlID)
                {
                    this.Params.RemoveAt(i);
                    result = true;
                }
                else
                {
                    if ((this.Params[i].ParamType == FORMULAPARAM.ParamTypes.FormulaField) && (this.Params[i].VALUE2 != null))
                    {
                        result = this.Params[i].VALUE2.RemoveParam(strControlID);
                    }
                }
                this.AdjustParamCount();
                i++;
            }
            return result;
        }

        public bool ChangeParamTypeToFormula(string strControlID)
        {
            bool result = false;
            int i = 0;
            while ((!result) && (i < this.Params.Count))
            {
                if (this.Params[i].ControlID == strControlID)
                {
                    this.Params[i].PARAMTYPE = (int)FORMULAPARAM.ParamTypes.FormulaField;
                    this.Params[i].VALUE1 = null;
                    if (this.Params[i].VALUE2 == null)
                    {
                        #region initialize

                        CUSTOMRP.Model.FORMULAFIELD f = new CUSTOMRP.Model.FORMULAFIELD();
                        f.FIELDNAME = "";
                        f.FUNCTION = 1;
                        f.Params.Add(new CUSTOMRP.Model.FORMULAPARAM()
                        {
                            PARAMTYPE = 0,
                            VALUE1 = "1"
                        });
                        f.Params.Add(new CUSTOMRP.Model.FORMULAPARAM()
                        {
                            PARAMTYPE = 0,
                            VALUE1 = "1"
                        });

                        this.Params[i].VALUE2 = f;

                        #endregion
                    }
                    result = true;
                }
                else
                {
                    if ((this.Params[i].ParamType == FORMULAPARAM.ParamTypes.FormulaField) && (this.Params[i].VALUE2 != null))
                    {
                        result = this.Params[i].VALUE2.ChangeParamTypeToFormula(strControlID);
                    }
                }
                i++;
            }
            return result;
        }

        private void AdjustParamCount()
        {
            int i = 0;
            switch (this.Function)
            {
                case Functions.NONE:
                    {
                        // requires only 1 parameter
                        for (i = this.Params.Count; i > 1; i--)
                        {
                            this.Params.RemoveAt(i - 1);
                        }
                    }
                    break;
                case Functions.IfNull:
                    {
                        // requires exactly 2 parameters
                        if (this.Params.Count > 2)
                        {
                            for (i = this.Params.Count; i > 2; i--)
                            {
                                this.Params.RemoveAt(i - 1);
                            }
                        }
                        else if (this.Params.Count < 2)
                        {
                            for (i = this.Params.Count; i < 2; i++)
                            {
                                this.Params.Add(new FORMULAPARAM() { PARAMTYPE = 0, VALUE1 = String.Empty });
                            }
                        }
                    }
                    break;
                case Functions.Decode:
                    {
                        // requires > 3 parameters
                        for (i = this.Params.Count; i < 3; i++)
                        {
                            this.Params.Add(new FORMULAPARAM() { PARAMTYPE = 0, VALUE1 = String.Empty });
                        }
                    }
                    break;
                default:
                    {
                        // requires > 2 parameters
                        for (i = this.Params.Count; i < 2; i++)
                        {
                            this.Params.Add(new FORMULAPARAM() { PARAMTYPE = 0, VALUE1 = String.Empty });
                        }
                    }
                    break;
            }
        }

        private string GetColorForLevel()
        {
            int level = 0;
            if (this._ControlID != null)
            {
                level = this._ControlID.Split('_').Length - 2;
            }
            string result = "#FFFFFF";
            switch (level % 5)
            {
                case 1:
                    {
                        result = "#003366";
                    }
                    break;
                case 2:
                    {
                        result = "#336699";
                    }
                    break;
                case 3:
                    {
                        result = "#6699CC";
                    }
                    break;
                case 4:
                    {
                        result = "#99CC00";
                    }
                    break;
                case 5:
                    {
                        result = "#CC0033";
                    }
                    break;
            }
            return result;
        }

        #endregion Extension
    }

    /// <summary>
    /// Container field to store parameter
    /// </summary>
    public class FORMULAPARAM
    {
        private int _ID;
        private int _SVID;
        private int? _RPID;
        private int _PARAMTYPE;
        private string _VALUE1;
        private int? _FORMULAFIELDID;
        private FORMULAFIELD _VALUE2;

        private string _ControlID;  // volatile field that will not be stored in database, but assigned dynamically just before render
        private string _ParentID;  // volatile field that will not be stored in database, but assigned dynamically just before render

        #region Model

        public int ID
        {
            set { _ID = value; }
            get { return _ID; }
        }
        public int SVID
        {
            set { _SVID = value; }
            get { return _SVID; }
        }
        public int? RPID
        {
            set { _RPID = value; }
            get { return _RPID; }
        }
        public int PARAMTYPE
        {
            set { _PARAMTYPE = value; }
            get { return _PARAMTYPE; }
        }
        public string VALUE1
        {
            set { _VALUE1 = value; }
            get { return _VALUE1; }
        }
        public int? FORMULAFIELDID
        {
            set { _FORMULAFIELDID = value; }
            get { return _FORMULAFIELDID; }
        }
        public FORMULAFIELD VALUE2
        {
            set { _VALUE2 = value; }
            get { return _VALUE2; }
        }

        #endregion Model

        #region Extension

        public enum ParamTypes : int
        {
            Literals = 0,
            ColumnName = 1,
            FormulaField = 2,
        }

        public ParamTypes ParamType
        {
            get { return (ParamTypes)_PARAMTYPE; }
        }

        /// <summary>
        /// volatile field that will not be stored in database, but assigned dynamically just before render
        /// </summary>
        public string ControlID
        {
            get { return _ControlID; }
        }

        public void SetControlID(string strControlID)
        {
            if (this.ParamType == ParamTypes.FormulaField)
            {
                this._VALUE2.SetControlID(strControlID);
            }
            this._ControlID = strControlID;
        }

        /// <summary>
        /// volatile field that will not be stored in database, but assigned dynamically just before render
        /// </summary>
        public string ParentID
        {
            get { return _ParentID; }
        }

        public void SetParentID(string strFormulaID)
        {
            if (this.ParamType == ParamTypes.FormulaField)
            {
                this._VALUE2.SetParentID(strFormulaID);
            }
            this._ParentID = strFormulaID;
        }

        public string GetText
        {
            get
            {
                string result = String.Empty;
                
                switch (this.ParamType)
                {
                    case ParamTypes.Literals:
                        {
                            if (this._VALUE1 != null)
                            {
                                decimal tempdec;
                                bool tempbool;
                                if (Decimal.TryParse(this._VALUE1, out tempdec))
                                {
                                    // is numeric
                                    result = this._VALUE1;
                                }
                                else if (Boolean.TryParse(this._VALUE1.ToLower(), out tempbool))
                                {
                                    // is boolean
                                    result = tempbool ? "1" : "0";
                                }
                                else
                                {
                                    // is string
                                    byte[] abytes = Encoding.ASCII.GetBytes(this._VALUE1);
                                    byte[] ubytes = Encoding.UTF8.GetBytes(this._VALUE1);
                                    if (abytes.SequenceEqual(ubytes))
                                    {
                                        result = String.Format("'{0}'", this._VALUE1.Replace("'", "''"));
                                    }
                                    else
                                    {
                                        result = String.Format("N'{0}'", this._VALUE1.Replace("'", "''"));
                                    }
                                }
                            }
                            else
                            {
                                result = "NULL";
                            }
                        }
                        break;
                    case ParamTypes.ColumnName:
                        {
                            result = String.Format("[{0}]", this._VALUE1);
                        }
                        break;
                    case ParamTypes.FormulaField:
                        {
                            result = this._VALUE2.GetText;
                        }
                        break;
                }

                return result;
            }
        }

        /// <summary>
        /// Get HTML representation for formula field edit.
        /// </summary>
        public string GetHtml
        {
            get {
                string result = String.Empty;

                switch (this.ParamType)
                {
                    case ParamTypes.Literals:
                    case ParamTypes.ColumnName:
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendFormat("<input type=\"text\" ID=\"txtCellText_{0}\" data-formulactrl='true' data-formulaid='{2}' value=\"{1}\" onchange=\"return paramTextChanged('{0}');\" onkeyup=\"return checkDelParam(event, '{0}');\" style=\"width:80px\"/>", this._ControlID, this._VALUE1, this._ParentID);
                            sb.AppendFormat("<input type=\"button\" id=\"btn_FX_{0}\" value=\"f(x)\" onclick=\"return paramTextToFormula('{0}');\" />", this.ControlID);
                            result = sb.ToString();
                        }
                        break;
                    case ParamTypes.FormulaField:
                        {
                            result = this._VALUE2.GetHtml;
                        }
                        break;
                }

                return result;
            }
        }

        #endregion Extension
    }
}
