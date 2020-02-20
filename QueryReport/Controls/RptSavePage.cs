using System.Collections.Generic;
using System.Data;
using QueryReport.Code;

namespace QueryReport.Controls
{
    public class RptSavePage: LoginUserPage
    {
        #region public properties - to be accessed via Server.transfer

        public ReportParameterContainer container = null;
        public List<string> rpcr = null;
        public DataTable rpdt = null;

        #endregion
    }
}