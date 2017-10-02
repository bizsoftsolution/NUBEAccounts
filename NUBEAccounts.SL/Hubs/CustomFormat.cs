using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUBEAccounts.Common;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {
        #region Account Group
        BLL.CustomFormat CustomFormatDAL_BLL(DAL.CustomFormat d)
        {
            BLL.CustomFormat b = d.toCopy<BLL.CustomFormat>(new BLL.CustomFormat());
            b.Fund = d.FundMaster == null ? new BLL.FundMaster() : d.FundMaster.toCopy<BLL.FundMaster>(new BLL.FundMaster());
            return b;
        }
        public List<BLL.CustomFormat> CustomFormat_List()
        {
            return DB.CustomFormats.ToList()
                               .Select(x => CustomFormatDAL_BLL(x)).ToList();
        }

        public int CustomFormat_Save(BLL.CustomFormat agp)
        {
            try
            {
                agp.FundMasterId = Caller.FundMasterId;
                DAL.CustomFormat d = DB.CustomFormats.Where(x => x.Id == agp.Id).FirstOrDefault();

                if (d == null)
                {
                    d = new DAL.CustomFormat();
                    DB.CustomFormats.Add(d);

                    agp.toCopy<DAL.CustomFormat>(d);
                    DB.SaveChanges();

                    agp.Id = d.Id;
                    LogDetailStore(agp, LogDetailType.INSERT);
                }
                else
                {
                    agp.toCopy<DAL.CustomFormat>(d);
                    DB.SaveChanges();
                    LogDetailStore(agp, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).CustomFormat_Save(agp);

                return agp.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public bool CustomFormat_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.CustomFormats.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    DB.CustomFormats.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(d.toCopy<BLL.CustomFormat>(new BLL.CustomFormat()), LogDetailType.DELETE);
                }
                else
                {
                    rv = false;
                }
                Clients.Clients(OtherLoginClientsOnGroup).CustomFormat_Delete(pk);
                Clients.All.delete(pk);
                rv = true;


            }
            catch (Exception ex)
            {
                rv = false;
            }
            return rv;
        }

        #endregion
    }
}