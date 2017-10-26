using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {
        public List<BLL.PayeeList> PayeeList()
        {            
            var l1 = DB.Payments.Where(x => x.Ledger.AccountGroup.FundMasterId == Caller.FundMasterId).Select(x=> x.PayTo).Distinct().ToList();
            var l2 = DB.Receipts.Where(x => x.Ledger.AccountGroup.FundMasterId == Caller.FundMasterId).Select(x => x.ReceivedFrom).Distinct().ToList();
            return l1.Union(l2).OrderBy(x=> x).ToList().Select(x=> new BLL.PayeeList() { PayeeName=x,IsChecked=false }).ToList();
        }
        public List<BLL.LedgerList> LedgerList()
        {
            return DB.Ledgers.Where(x => x.AccountGroup.FundMasterId == Caller.FundMasterId).OrderBy(x=> x.LedgerCode).Select(x => new BLL.LedgerList() { LedgerId=x.Id,LedgerName=x.LedgerName,IsChecked=false }).ToList();
        }
    }
}