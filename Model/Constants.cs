using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CUSTOMRP.Model
{
    public enum APPModuleID
    {
        //******** Administration ***********//
        usergroupright_company = 1001,
        usergroupright_reportgroup = 1002,
        usergroupright_reportcategory = 1003,
        usergroupright_securitylevel = 1004,
        usergroupright_query = 1005,
        usergroupright_usergroup = 1006,
        usergroupright_groupright = 1007,
        usergroupright_user = 1008,
        usergroupright_wordtemplate = 1009,
        usergroupright_copy = 1010
        //******** Administration ***********//
    }

    public enum keyValue
    {
        QueryApply = 1
    }

    public class RpEnum
    {
        public enum V_ICITEM_VALUES
        {
            Active = 0,
            Inactive = 1,
            Hold = 2
        }
    }
}
