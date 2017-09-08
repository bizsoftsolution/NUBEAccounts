using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {
        public List<BLL.DailyBankBalance> DailyBankBalance(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.DailyBankBalance> rv = new List<BLL.DailyBankBalance>();

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == BLL.DataKeyValue.BankAccounts_Key).OrderBy(x => x.AccountGroup.GroupCode).ToList();
            BLL.DailyBankBalance tb = new BLL.DailyBankBalance();
            decimal OPDr = 0, OPCr = 0, Dr = 0, Cr = 0;
            decimal GTotalDr = 0, GTotalCr = 0, GTotalDrOP = 0, GTotalCrOP = 0, BalAmt = 0;
            var n = (dtTo.Date - dtFrom.Date).TotalDays;
            for (int i = 1; i <= n; i++)
            {
                foreach (var l in lstLedger)
                {
                    tb = new BLL.DailyBankBalance();
                    tb.Ledger = LedgerDAL_BLL(l);
                    LedgerBalance(l, dtFrom, dtFrom.Date.AddDays(i), ref OPDr, ref OPCr, ref Dr, ref Cr);
                    if ((Dr != 0 || Cr != 0) && (OPDr != 0 || OPCr != 0))
                    {
                        tb.DrAmt = Dr;
                        tb.CrAmt = Cr;
                        tb.DrAmtOP = OPDr;
                        tb.CrAmtOP = OPCr;

                        if (tb.DrAmt != 0 || tb.CrAmt != 0)
                        {
                            GTotalDr += tb.DrAmt ?? 0;
                            GTotalCr += tb.CrAmt ?? 0;
                            GTotalDrOP += tb.DrAmtOP ?? 0;
                            GTotalCrOP += tb.CrAmtOP ?? 0;
                        }


                        tb.Ledger.AccountName = l.LedgerName;
                        GTotalCr = tb.CrAmt > tb.DrAmt ? Math.Abs(tb.CrAmt.Value - tb.DrAmt.Value) : 0;
                        GTotalDr = tb.CrAmt < tb.DrAmt ? Math.Abs(tb.DrAmt.Value - tb.CrAmt.Value) : 0;
                        GTotalDrOP = tb.DrAmtOP > tb.CrAmtOP ? Math.Abs(tb.DrAmtOP.Value - tb.CrAmtOP.Value) : 0;
                        GTotalDrOP = tb.CrAmtOP > tb.DrAmtOP ? Math.Abs(tb.CrAmtOP.Value - tb.DrAmtOP.Value) : 0;
                        BalAmt = Math.Abs((GTotalCr + GTotalCrOP) - (GTotalDr - GTotalDrOP));
                        tb.Date = dtFrom.Date.AddDays(i);
                        tb.Amount = BalAmt;
                        rv.Add(tb);
                    }



                }

            }



            return rv;
        }
        public List<BLL.DailyBankBalance> IndividualReport(DateTime dtFrom, DateTime dtTo, int? LId, string PayName)
        {
            List<BLL.DailyBankBalance> rv = new List<BLL.DailyBankBalance>();

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId && (LId == null || x.Id == LId));
            BLL.DailyBankBalance tb = new BLL.DailyBankBalance();
            decimal Dr = 0, Cr = 0;
            var n = (dtTo.Date - dtFrom.Date).TotalDays;
            foreach (var l in lstLedger)
            {
                tb = new BLL.DailyBankBalance();
                tb.Ledger = new BLL.Ledger();
                tb.Ledger = LedgerDAL_BLL(l);
                foreach (var pd in l.PaymentDetails.Where(x => x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo && (PayName == null || x.Payment.PayTo == PayName)).ToList())
                {
                    Dr += pd.Amount;
                    tb.PayName = pd.Payment.Particulars;
                }
                foreach (var p in l.Payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo && (PayName == null || x.PayTo == PayName)).ToList())
                {
                    Dr += p.Amount;
                    tb.PayName = p.Particulars;
                }
                foreach (var rd in l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo && (PayName == null || x.Receipt.ReceivedFrom == PayName)).ToList())
                {
                    Cr += rd.Amount;
                    tb.PayName = rd.Receipt.Particulars;
                }
                foreach (var p in l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo && (PayName == null || x.ReceivedFrom == PayName)).ToList())
                {
                    Cr += p.Amount;
                    tb.PayName = p.Particulars;
                }
                if (Dr != 0 || Cr != 0)
                {
                    tb.PayName = PayName;
                    tb.Amount = Math.Abs(Cr - Dr);
                    rv.Add(tb);
                }
            }
            return rv;
        }

    }


}
