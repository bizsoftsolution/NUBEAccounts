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
        BLL.AccountGroup AccountGroupDAL_BLL(DAL.AccountGroup d)
        {
            BLL.AccountGroup b = d.toCopy<BLL.AccountGroup>(new BLL.AccountGroup());
            b.Fund = d.FundMaster == null ? new BLL.FundMaster() : d.FundMaster.toCopy<BLL.FundMaster>(new BLL.FundMaster());
            //b.UnderAccountGroup = d.AccountGroup2 == null ? new BLL.AccountGroup() : AccountGroupDAL_BLL(d.AccountGroup2);
            b.UnderAccountGroup = d.AccountGroup2 == null ? new BLL.AccountGroup() : new BLL.AccountGroup() { GroupName= d.AccountGroup2.GroupName, GroupCode = d.AccountGroup2.GroupCode };
            return b;
        }
        public List<BLL.AccountGroup> accountGroup_List()
        {
             return DB.AccountGroups.Where(x => x.FundMasterId == Caller.FundMasterId).ToList()
                               .Select(x => AccountGroupDAL_BLL(x)).ToList();
        }

        public int AccountGroup_Save(BLL.AccountGroup agp)
        {
            try
            {
                agp.FundMasterId = Caller.FundMasterId;
                DAL.AccountGroup d = DB.AccountGroups.Where(x => x.Id == agp.Id).FirstOrDefault();

                if (d == null)
                {
                    d = new DAL.AccountGroup();
                    DB.AccountGroups.Add(d);

                    agp.toCopy<DAL.AccountGroup>(d);
                    DB.SaveChanges();

                    agp.Id = d.Id;
                    LogDetailStore(agp, LogDetailType.INSERT);
                }
                else
                {
                    agp.toCopy<DAL.AccountGroup>(d);
                    DB.SaveChanges();
                    LogDetailStore(agp, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).AccountGroup_Save(agp);

                return agp.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public bool AccountGroup_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.AccountGroups.Where(x => x.Id == pk).FirstOrDefault();
                if (d.Ledgers != null && AccountGroup_CanDelete(d))
                {
                    if (d != null)
                    {
                        DB.AccountGroups.Remove(d);
                        DB.SaveChanges();
                        LogDetailStore(d.toCopy<BLL.AccountGroup>(new BLL.AccountGroup()), LogDetailType.DELETE);
                    }

                    Clients.Clients(OtherLoginClientsOnGroup).AccountGroup_Delete(pk);
                    Clients.All.delete(pk);
                    rv = true;
                }
                else
                {
                    rv = false;
                }

            }
            catch (Exception ex)
            {
                rv = false;
            }
            return rv;
        }

        public bool AccountGroup_CanDelete(DAL.AccountGroup l)
        {
            return l.Ledgers.Count() == 0;

        }
        #endregion
    }
}