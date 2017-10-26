using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUBEAccounts.BLL;

namespace NUBEAccounts.SL.Hubs
{

    public partial class NubeServerHub
    {
        #region IncomeReport

        public List<BLL.IndividualReport> IndividualReport_List(DateTime dtFrom, DateTime dtTo, List<int> LedgerIdList, List<string> PayeeNameList)
        {
            List<IndividualReport> lstIndividualReport = new List<IndividualReport>();

            var LList = DB.Ledgers.Where(x => LedgerIdList.Contains(x.Id)).ToList();
            #region Ledger
            foreach (var l in LList)
            {                
                foreach (var p in PayeeNameList)
                {
                    decimal amt = 0;

                    var amt1 = l.Payments.Where(x => x.PayTo== p && x.PaymentDate<=dtTo && x.PaymentDate>=dtFrom).Sum(x => x.Amount);
                    var amt2 = l.PaymentDetails.Where(x=> x.Payment.PayTo == p&&x.Payment.PaymentDate <= dtTo && x.Payment.PaymentDate >= dtFrom).Sum(x => x.Amount);

                    var amt3 = l.Receipts.Where(x => x.ReceivedFrom == p&&x.ReceiptDate<=dtTo&&x.ReceiptDate>=dtFrom).Sum(x => x.Amount);
                    var amt4 = l.ReceiptDetails.Where(x => x.Receipt.ReceivedFrom == p&&x.Receipt.ReceiptDate<=dtTo&& x.Receipt.ReceiptDate>=dtFrom).Sum(x => x.Amount);

                    amt = Math.Abs( (amt1 + amt4) - (amt2 + amt3));
                    
                    lstIndividualReport.Add(new BLL.IndividualReport() {LedgerId=l.Id, LedgerName=l.LedgerName, PayeeName=p,Amount=amt });
                }                
            }

            return lstIndividualReport;
            #endregion
        }
        #endregion
    }
}
