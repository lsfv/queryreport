using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using CUSTOMRP.Model;
using CUSTOMRP.DAL;

namespace CUSTOMRP.BLL
{
    /// <summary>
    /// SORUCEVIEWCOLUMN
    /// </summary>
    public class SOURCEVIEWCOLUMN
    {
        private readonly CUSTOMRP.DAL.SOURCEVIEWCOLUMN dal = new DAL.SOURCEVIEWCOLUMN();
        private readonly CUSTOMRP.DAL.DATABASE dalDB = new DAL.DATABASE();
        private readonly CUSTOMRP.DAL.SOURCEVIEW dalSV = new DAL.SOURCEVIEW();

        #region BasicMethod

        //internal int Add(int UserID, CUSTOMRP.Model.SOURCEVIEWCOLUMN model)
        //{
        //    return dal.Add(UserID, model);
        //}

        //internal int AddColList(int UserID, int SVID, string[] columns)
        //{
        //    return dal.AddColList(UserID, SVID, columns);
        //}

        //internal bool Delete(int UserID, int SVCID)
        //{
        //    return dal.Delete(UserID, SVCID);
        //}

        //public bool DeleteForSourceView(int SVID)
        //{
        //    return dal.DeleteForSourceView(SVID);
        //}

        public void UpdateModelList(int UserID, CUSTOMRP.Model.SOURCEVIEWCOLUMN[] modelList)
        {
            if ((modelList == null) || (modelList.Length == 0)) { return; }

            CUSTOMRP.Model.SOURCEVIEWCOLUMN[] currentlist = null;
            CUSTOMRP.Model.SOURCEVIEWCOLUMN currentitem = null;


            CUSTOMRP.Model.SOURCEVIEWCOLUMN[] original = GetModelsForSourceView(UserID, modelList[0].SVID).ToArray();
            // prepare AuditLog object
            Model.AUDITLOG auditobj = modelList.GetAuditLogObject(original);
            auditobj.UserID = UserID;
            auditobj.CreateDate = DateTime.Now;

            try
            {
                currentlist = GetModelsForSourceView(UserID, modelList[0].SVID).ToArray();
            }
            catch
            {
                currentlist = new CUSTOMRP.Model.SOURCEVIEWCOLUMN[0];
            }

            #region Delete non-existent columns

            foreach (CUSTOMRP.Model.SOURCEVIEWCOLUMN item in currentlist)
            {
                if (modelList.Where(x => x.COLUMNNAME == item.COLUMNNAME).Count() == 0)
                {
                    dal.Delete(UserID, item.ID);
                }
            }

            foreach (CUSTOMRP.Model.SOURCEVIEWCOLUMN item in modelList)
            {
                currentitem = currentlist.Where(x => x.COLUMNNAME == item.COLUMNNAME).FirstOrDefault();

                if (currentitem == null)
                {
                    dal.Add(UserID, item);
                }
                else
                {
                    currentitem.DISPLAYNAME = item.DISPLAYNAME;
                    currentitem.HIDDEN = item.HIDDEN;
                    currentitem.DATA_TYPE = item.DATA_TYPE;

                    dal.Update(UserID, currentitem);
                }
            }

            #endregion

            auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
            auditobj.ModuleName = "BLL.SOURCEVIEWCOLUMN.UpdateModelList";
            auditobj.Message = String.Format(original.Length == 0 ? DAL.AppNum.AuditMessage.SourceViewColumnInsertSuccess : DAL.AppNum.AuditMessage.SourceViewColumnUpdateSuccess, modelList[0].SVID);

            DAL.AUDITLOG.Add(auditobj);

        }
        //v1.7.0 Ben 2017.08.21 - Above will no update Hidden as GetModelsForSourceView will not get hidden
        public void UpdateModelList(int UserID, CUSTOMRP.Model.SOURCEVIEWCOLUMN[] modelList, bool UpdateHidden)
        {
            if ((modelList == null) || (modelList.Length == 0)) { return; }

            CUSTOMRP.Model.SOURCEVIEWCOLUMN[] currentlist = null;
            CUSTOMRP.Model.SOURCEVIEWCOLUMN currentitem = null;


            CUSTOMRP.Model.SOURCEVIEWCOLUMN[] original = GetModelsForSourceView(UserID, modelList[0].SVID, UpdateHidden).ToArray();
            // prepare AuditLog object
            Model.AUDITLOG auditobj = modelList.GetAuditLogObject(original);
            auditobj.UserID = UserID;
            auditobj.CreateDate = DateTime.Now;
            currentlist = original;

            #region Delete non-existent columns
            foreach (CUSTOMRP.Model.SOURCEVIEWCOLUMN item in currentlist)
            {
                //if (modelList.Where(x => x.COLUMNNAME == item.COLUMNNAME && x.ID != -1).Count() == 0)
                if (modelList.Where(x => x.COLUMNNAME.ToUpper() == item.COLUMNNAME.ToUpper() && x.ID != -1).Count() == 0)
                {
                    dal.Delete(UserID, item.ID);
                    //v1.8.2 Ben 2018.02.22 - Same as below but even deleted record pass to Below seems not update/insert
                    var models = modelList.Where(x => x.COLUMNNAME.ToUpper() == item.COLUMNNAME.ToUpper() && x.ID != -1);
                    if (models.Any())
                        models.First().ID = -1;
                    item.ID = 0;
                }
            }
            #endregion
            
            #region Delete existing more than one columns
            foreach (CUSTOMRP.Model.SOURCEVIEWCOLUMN item in currentlist)
            {
                //v1.8.2 Ben 2018.02.26 - Prevent duplicate even just difeerent case
                //if (modelList.Where(x => x.COLUMNNAME == item.COLUMNNAME && x.ID != -1).Count() > 1)
                if (modelList.Where(x => x.COLUMNNAME.ToUpper() == item.COLUMNNAME.ToUpper() && x.ID != -1).Count() > 1)
                {
                    dal.Delete(UserID, item.ID);
                    //modelList.Where(x => x.COLUMNNAME == item.COLUMNNAME && x.ID != -1).First().ID = -1;
                    var models = modelList.Where(x => x.COLUMNNAME.ToUpper() == item.COLUMNNAME.ToUpper() && x.ID != -1);
                    if (models.Any())
                        models.First().ID = -1;
                    item.ID = 0;
                }
            }
            modelList = modelList.Where(x => x.ID != -1).ToArray();
            #endregion

            foreach (CUSTOMRP.Model.SOURCEVIEWCOLUMN item in modelList)
            {
                //currentitem = currentlist.Where(x => x.COLUMNNAME == item.COLUMNNAME).FirstOrDefault();
                //v1.8.2 Ben 2018.02.26 - Prevent duplicate even just difeerent case
                //currentitem = currentlist.Where(x => x.COLUMNNAME == item.COLUMNNAME).OrderByDescending(x => x.ID).FirstOrDefault();
                currentitem = currentlist.Where(x => x.COLUMNNAME.ToUpper() == item.COLUMNNAME.ToUpper()).OrderByDescending(x => x.ID).FirstOrDefault();
                
                if (currentitem == null || currentitem.ID == 0)
                {
                    dal.Add(UserID, item);
                }
                else
                {
                    //v1.8.2 Ben 2018.02.26 - Use actual case
                    currentitem.COLUMNNAME = item.COLUMNNAME;

                    currentitem.DISPLAYNAME = item.DISPLAYNAME;
                    currentitem.HIDDEN = item.HIDDEN;
                    dal.Update(UserID, currentitem);
                }
            }
            
            auditobj.MessageType = Model.AUDITLOG.Severity.Audit;
            auditobj.ModuleName = "BLL.SOURCEVIEWCOLUMN.UpdateModelList";
            auditobj.Message = String.Format(original.Length == 0 ? DAL.AppNum.AuditMessage.SourceViewColumnInsertSuccess : DAL.AppNum.AuditMessage.SourceViewColumnUpdateSuccess, modelList[0].SVID);
            DAL.AUDITLOG.Add(auditobj);
        }

        public void DeleteModelList(int UserID, CUSTOMRP.Model.SOURCEVIEWCOLUMN[] modelList)
        {
            if ((modelList == null) || (modelList.Length == 0)) { return; }
            foreach (CUSTOMRP.Model.SOURCEVIEWCOLUMN item in modelList)
            {
                dal.Delete(UserID, item.ID);
            }
        }

        //public CUSTOMRP.Model.SOURCEVIEWCOLUMN GetModel(int ID)
        //{
        //    return dal.GetModel(ID);
        //}

        private CUSTOMRP.Model.SOURCEVIEWCOLUMN DataRowToModel(DataRow row)
        {
            return dal.DataRowToModel(row);
        }

        //v1.8.8 2019.03.08 Alex - Don't run the view/sp to refresh
        public List<CUSTOMRP.Model.SOURCEVIEWCOLUMN> GetModelsForSourceView(int UserID, int SVID, bool showHidden = false)
        {
            List<CUSTOMRP.Model.SOURCEVIEWCOLUMN> result = dal.GetModelsForSourceView(UserID, SVID, showHidden);

            //#region Check if there's new columns to add, and whether there are columns to hide

            //if (showHidden)
            //{
            //    string[] svColumns = result.Select(x => x.COLUMNNAME).ToArray();
            //    CUSTOMRP.Model.SOURCEVIEW sv = dalSV.GetModel(UserID, SVID);
            //    //CUSTOMRP.Model.V_DATABASE db = (new V_DATABASE()).GetModel(sv.DATABASEID);
            //    CUSTOMRP.Model.DATABASE db = dalDB.GetModel(UserID, sv.DATABASEID);
            //    string[] colnames = new string[0];
            //    switch (sv.SourceType)
            //    {
            //        case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.View:
            //        case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.Table:
            //            {
            //                colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForTblView(UserID, db.NAME, sv.TBLVIEWNAME);
            //            }
            //            break;
            //        case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.StoredProc:
            //            {
            //                colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForStoredProc(UserID, db.NAME, sv.TBLVIEWNAME);
            //            }
            //            break;
            //    }

            //    for (int i = 0; i < colnames.Length; i++)
            //    {
            //        //if (!svColumns.Contains(colnames[i]))
            //        if (!svColumns.Contains(colnames[i], StringComparer.OrdinalIgnoreCase))
            //        {
            //            // new column
            //            result.Add(new CUSTOMRP.Model.SOURCEVIEWCOLUMN()
            //            {
            //                ID = 0,
            //                SVID = SVID,
            //                COLUMNNAME = colnames[i],
            //                DISPLAYNAME = "",
            //                COLUMNTYPE = 1,
            //                COLUMNCOMMENT = "",
            //                HIDDEN = false,
            //                DEFAULTDISPLAYNAME = "",
            //            });
            //        }
            //        else
            //        {
            //            result.Where(x => x.COLUMNNAME.ToUpper() == colnames[i].ToUpper()).ToList().ForEach(x => x.COLUMNNAME = colnames[i]);
            //        }
            //    }
            //}

            //#endregion

            //#region Remove hidden records from list if hidden record is not to be included in result
 
            //if (!showHidden)
            //{
            //    result = result.Where(x => !x.HIDDEN).ToList();
            //}

            //#endregion

            return result;
        }

        //v1.8.8 2019.03.08 Alex - Don't run the view/sp to refresh
        public List<CUSTOMRP.Model.SOURCEVIEWCOLUMN> RefreshModelsForSourceView(int UserID, int SVID, bool showHidden = false)
        {
            List<CUSTOMRP.Model.SOURCEVIEWCOLUMN> result = dal.GetModelsForSourceView(UserID, SVID, showHidden);

            #region Check if there's new columns to add, and whether there are columns to hide

            if (showHidden)
            {
                string[] svColumns = result.Select(x => x.COLUMNNAME).ToArray();
                CUSTOMRP.Model.SOURCEVIEW sv = dalSV.GetModel(UserID, SVID);
                //CUSTOMRP.Model.V_DATABASE db = (new V_DATABASE()).GetModel(sv.DATABASEID);
                CUSTOMRP.Model.DATABASE db = dalDB.GetModel(UserID, sv.DATABASEID);
                string[] colnames = new string[0];
                switch (sv.SourceType)
                {
                    case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.View:
                    case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.Table:
                        {
                            colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForTblView(UserID, db.NAME, sv.TBLVIEWNAME);
                        }
                        break;
                    case CUSTOMRP.Model.SOURCEVIEW.SourceViewType.StoredProc:
                        {
                            colnames = CUSTOMRP.BLL.AppHelper.GetColumnNamesForStoredProc(UserID, db.NAME, sv.TBLVIEWNAME);
                        }
                        break;
                }

                for (int i = 0; i < colnames.Length; i++)
                {
                    //if (!svColumns.Contains(colnames[i]))
                    if (!svColumns.Contains(colnames[i], StringComparer.OrdinalIgnoreCase))
                    {
                        // new column
                        result.Add(new CUSTOMRP.Model.SOURCEVIEWCOLUMN()
                        {
                            ID = 0,
                            SVID = SVID,
                            COLUMNNAME = colnames[i],
                            DISPLAYNAME = "",
                            COLUMNTYPE = 1,
                            COLUMNCOMMENT = "",
                            HIDDEN = false,
                            DEFAULTDISPLAYNAME = "",
                        });
                    }
                    else
                    {
                        result.Where(x => x.COLUMNNAME.ToUpper() == colnames[i].ToUpper()).ToList().ForEach(x => x.COLUMNNAME = colnames[i]);
                    }
                }
            }

            #endregion

            #region Remove hidden records from list if hidden record is not to be included in result

            if (!showHidden)
            {
                result = result.Where(x => !x.HIDDEN).ToList();
            }

            #endregion

            return result;
        }

        #endregion BasicMethod
    }
}
