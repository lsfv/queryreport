using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CUSTOMRP.Model
{
    [Serializable]
    public class LoginUser
    {
        [Flags]
        public enum ReportRight : int
        {
            Add = 1,
            Modify = 2,
            Delete = 4,
            View = 8,
        }

        private ReportRight _ReportRight;

        public int ID;
        /// <summary>
        /// 登录id
        /// </summary>
        public string LoginID;
        /// <summary>
        /// APPLICATIONID
        /// </summary>
        public int APPLICATIONID;
        /// <summary>
        /// Database id
        /// </summary>
        public int DatabaseID;
        /// <summary>
        /// database name
        /// </summary>
        public string DatabaseNAME;
        /// <summary>
        /// view level .
        /// </summary>
        public decimal ViewLevel;
        /// <summary>
        /// report group .xxx,xxx,xxx,xxxx
        /// </summary>
        public string ReportGroup;

        //v1.1.0 - Cheong - 2016/06/03 - Simplify report right assignment
        //public bool rp_add;
        //public bool rp_modify;
        //public bool rp_delete;
        //public bool rp_view;
        public bool rp_add { get { return (this._ReportRight & ReportRight.Add) == ReportRight.Add; } }
        public bool rp_modify { get { return (this._ReportRight & ReportRight.Modify) == ReportRight.Modify; } }
        public bool rp_delete { get { return (this._ReportRight & ReportRight.Delete) == ReportRight.Delete; } }
        public bool rp_view { get { return (this._ReportRight & ReportRight.View) == ReportRight.View; } }

        public DateTime loginDate;

        private CUSTOMRP.Model.GROUPRIGHT gr;
        
        public Dictionary<string, string> UserCriteria { get; set; }

        public LoginUser(int ID, string LoginID, int APPLICATIONID, int DatabaseID, string DatabaseNAME, decimal ViewLevel, string ReportGroup, int REPORTRIGHT, CUSTOMRP.Model.GROUPRIGHT gr)
        {
            this.ID = ID;
            this.LoginID = LoginID;
            this.APPLICATIONID = APPLICATIONID;
            this.DatabaseID = DatabaseID;
            this.DatabaseNAME = DatabaseNAME;
            this.ViewLevel = ViewLevel;
            this.ReportGroup = ReportGroup;
            this.gr = gr;

            //v1.1.0 - Cheong - 2016/06/03 - Simplify report right assignment
            _ReportRight = (ReportRight)REPORTRIGHT;
            //List<int> reportRight = Function.Utils.getList2N(REPORTRIGHT);
            //if (reportRight.Contains(3))
            //{
            //    rp_view = true;             // = 8
            //}
            //if (reportRight.Contains(2))
            //{
            //    rp_delete = true;           // = 4
            //}
            //if (reportRight.Contains(1))
            //{
            //    rp_modify = true;           // = 2
            //}
            //if (reportRight.Contains(0))
            //{
            //    rp_add = true;              // = 1
            //}
        }

        public bool checkUserGroupRight(APPModuleID module, string function, string UID)
        {
            bool resutl = false;

            if (UID.ToUpper() == "ADMIN")
            {
                return true;
            }

            if (LoginID.ToUpper() == "ADMIN")
            {
                resutl = true;
            }

            if (gr == null) { return false; }

            if (module == APPModuleID.usergroupright_company)
            {
                resutl = (gr.COMPANY != null) && (gr.COMPANY.Contains(function));
            }
            else if (module == APPModuleID.usergroupright_reportgroup)
            {
                resutl = (gr.REPORTGROUP != null) && (gr.REPORTGROUP.Contains(function));
            }
            else if (module == APPModuleID.usergroupright_reportcategory)
            {
                resutl = (gr.CATEGARY != null) && (gr.CATEGARY.Contains(function));
            }
            else if (module == APPModuleID.usergroupright_securitylevel)
            {
                resutl = (gr.SECURITY != null) && (gr.SECURITY.Contains(function));
            }
            else if (module == APPModuleID.usergroupright_query)
            {
                resutl = (gr.QUERY != null) && (gr.QUERY.Contains(function));
            }
            else if (module == APPModuleID.usergroupright_usergroup)
            {
                resutl = (gr.USERGROUP != null) && (gr.USERGROUP.Contains(function));
            }
            else if (module == APPModuleID.usergroupright_groupright)
            {
                resutl = (gr.USERGROUPRIGHT != null) && (gr.USERGROUPRIGHT.Contains(function));
            }
            else if (module == APPModuleID.usergroupright_user)
            {
                resutl = (gr.USERSETUP != null) && (gr.USERSETUP.Contains(function));
            }
            else if (module == APPModuleID.usergroupright_wordtemplate)
            {
                resutl = (gr.EXTEND1 != null) && (gr.EXTEND1.Contains(function));
            }

            return resutl;
        }
    }
}
