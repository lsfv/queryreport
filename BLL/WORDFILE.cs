using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CUSTOMRP.BLL
{
    public class WORDFILE
    {
        private readonly CUSTOMRP.DAL.WORDFILE dal = new DAL.WORDFILE();

        public int AddFile(CUSTOMRP.Model.WORDFILE model)
        {
            return dal.AddFile(model);
        }

        public bool UpdateFile(CUSTOMRP.Model.WORDFILE model)
        {
            return dal.UpdateFile(model);
        }

        public CUSTOMRP.Model.WORDFILE GetModel(int UserID, int ID)
        {
            return dal.GetModel(UserID, ID);
        }

        public CUSTOMRP.Model.WORDFILE GetModelByReportID(int UserID, int rpID)
        {
            return dal.GetModelByReportID(UserID, rpID);
        }

        public List<CUSTOMRP.Model.WORDFILE> GetModelByTemplateID(int UserID, int WordTemplateID)
        {
            return dal.GetModelByTemplateID(UserID, WordTemplateID);
        }
    }
}