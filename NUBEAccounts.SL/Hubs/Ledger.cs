using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUBEAccounts.Common;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {

        #region Ledger

        private BLL.Ledger LedgerDAL_BLL(DAL.Ledger ledgerFrom)
        {
            BLL.Ledger ledgerTo = new BLL.Ledger();
            try
            {
                ledgerTo = ledgerFrom.toCopy<BLL.Ledger>(new BLL.Ledger());

                ledgerTo.AccountGroup = AccountGroupDAL_BLL(ledgerFrom.AccountGroup);

                //ledgerTo.CreditLimitType = new BLL.CreditLimitType();
                //ledgerFrom.CreditLimitType.toCopy<BLL.CreditLimitType>(ledgerTo.CreditLimitType);


            }
            catch (Exception ex)
            {

            }
            return ledgerTo;

        }

        public List<BLL.Ledger> Ledger_List()
        {
            return DB.Ledgers.Where(x => x.AccountGroup.CompanyDetail.Id == Caller.CompanyId).OrderBy(x=> x.LedgerCode).ThenBy(x=> x.LedgerName).ToList()
                             .Select(x => LedgerDAL_BLL(x)).ToList();
        }

        public List<BLL.Ledger> CashLedger_List()
        {
            return DB.Ledgers.Where(x => x.AccountGroup.CompanyDetail.Id == Caller.CompanyId && x.AccountGroup.GroupName == "Bank Accounts" || x.AccountGroup.GroupName == "Cash-in-Hand").ToList()
                             .Select(x => LedgerDAL_BLL(x)).ToList();
        }

        public int Ledger_Save(BLL.Ledger led)
        {
            try
            {
                DAL.Ledger d = DB.Ledgers.Where(x => x.Id == led.Id).FirstOrDefault();
                if (d == null)
                {
                    d = new DAL.Ledger();
                    DB.Ledgers.Add(d);

                    led.toCopy<DAL.Ledger>(d);

                    DB.SaveChanges();
                    led.Id = d.Id;

                    LogDetailStore(led, LogDetailType.INSERT);
                }
                else
                {
                    led.toCopy<DAL.Ledger>(d);
                    DB.SaveChanges();
                    LogDetailStore(led, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Ledger_Save(led);

                return led.Id = d.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public bool Ledger_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.Ledgers.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null && Ledger_CanDelete(d))
                {
                    DB.Ledgers.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(LedgerDAL_BLL(d), LogDetailType.DELETE);


                    Clients.Clients(OtherLoginClientsOnGroup).Ledger_Delete(pk);
                    Clients.All.delete(pk);

                    rv = true;
                }
            }
            catch (Exception ex)
            {
                rv = false;
            }
            return rv;
        }

        public bool Ledger_CanDelete(DAL.Ledger l)
        {
            bool rv = (l == null) ? false : l.Payments.Count() == 0 &&
                   l.PaymentDetails.Count() == 0 &&
                   l.Receipts.Count() == 0 &&
                   l.ReceiptDetails.Count() == 0 &&
                   l.JournalDetails.Count() == 0;
            return rv;
        }

        public bool Ledger_CanDeleteById(int Id)
        {
            return Ledger_CanDelete(DB.Ledgers.Where(x => x.Id == Id).FirstOrDefault());
        }

   

        #endregion
    }
}