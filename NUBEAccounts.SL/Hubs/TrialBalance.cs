using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {
        void LedgerBalance(DAL.Ledger l, DateTime dtFrom, DateTime dtTo, ref decimal OPDr, ref decimal OPCr, ref decimal Dr, ref decimal Cr)
        {
            decimal PDr, PCr, RDr, RCr, JDr, JCr;

            OPDr = l.OPDr ?? 0;
            OPCr = l.OPCr ?? 0;

            PDr = l.PaymentDetails.Where(x => x.Payment.PaymentDate < dtFrom).Sum(x => x.Amount);
            PCr = l.Payments.Where(x => x.PaymentDate < dtFrom).Sum(x => x.Amount);

            RDr = l.Receipts.Where(x => x.ReceiptDate < dtFrom).Sum(x => x.Amount);
            RCr = l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate < dtFrom).Sum(x => x.Amount);

            JDr = l.JournalDetails.Where(x => x.Journal.JournalDate < dtFrom).Sum(x => x.DrAmt);
            JCr = l.JournalDetails.Where(x => x.Journal.JournalDate < dtFrom).Sum(x => x.CrAmt);

            OPDr = OPDr + PDr + RDr + JDr;
            OPCr = OPCr + PCr + RCr + JCr;


            PDr = l.PaymentDetails.Where(x => x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo).Sum(x => x.Amount);
            PCr = l.Payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo).Sum(x => x.Amount);

            RDr = l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo).Sum(x => x.Amount);
            RCr = l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo).Sum(x => x.Amount);

            JDr = l.JournalDetails.Where(x => x.Journal.JournalDate >= dtFrom && x.Journal.JournalDate <= dtTo).Sum(x => x.DrAmt);
            JCr = l.JournalDetails.Where(x => x.Journal.JournalDate >= dtFrom && x.Journal.JournalDate <= dtTo).Sum(x => x.CrAmt);

            Dr = OPDr + PDr + RDr + JDr;
            Cr = OPCr + PCr + RCr + JCr;

            if (OPDr > OPCr)
            {
                OPDr = Math.Abs(OPDr - OPCr);
                OPCr = 0;
            }
            else
            {
                OPCr = Math.Abs(OPDr - OPCr);
                OPDr = 0;
            }

            if (Dr > Cr)
            {
                Dr = Math.Abs(Dr - Cr);
                Cr = 0;
            }
            else
            {
                Cr = Math.Abs(Dr - Cr);
                Dr = 0;
            }
        }

        public decimal GetLedgerBalance(int LedgerId)
        {
            return TrialBalance_GetLedgerBalance(LedgerId, DateTime.Now);
        }
        public decimal TrialBalance_GetLedgerBalance(int LedgerId,DateTime dt)
        {
            return GetLedgerBalance(DB.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault(), dt);
        }

        public decimal GetLedgerBalance(DAL.Ledger l,DateTime dt)
        {
            decimal OPDr = 0, OPCr = 0, Dr = 0, Cr = 0;
            LedgerBalance(l, dt, dt, ref OPDr, ref OPCr, ref Dr, ref Cr);
            return Dr + Cr;
        }

        public void GetLedgerTotal(DAL.AccountGroup ag, ref decimal total)
        {
            foreach (var l in ag.Ledgers)
            {
                total += GetLedgerBalance(l,DateTime.Now);
            }
            foreach (var ag1 in ag.AccountGroup1)
            {
                GetLedgerTotal(ag1, ref total);
            }
        }

        public List<BLL.TrialBalance> TrialBalance_List(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.TrialBalance> lstTrialBalance = new List<BLL.TrialBalance>();
            BLL.TrialBalance tb = new BLL.TrialBalance();

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId).OrderBy(x=> x.LedgerCode).ThenBy(x=> x.LedgerName).ToList();
            decimal TotDr = 0, TotCr = 0, TotOPCr = 0, TotOPDr = 0;

            foreach (var l in lstLedger)
            {
                tb = new BLL.TrialBalance();
                tb.Ledger = LedgerDAL_BLL(l);

                decimal OPDr = 0, OPCr = 0, Dr = 0, Cr = 0;
                LedgerBalance(l, dtFrom, dtTo, ref OPDr, ref OPCr, ref Dr, ref Cr);

                tb.DrAmtOP = OPDr;
                tb.CrAmtOP = OPCr;

                tb.DrAmt = Dr;
                tb.CrAmt = Cr;


                if (tb.DrAmt != 0 || tb.CrAmt != 0)
                {
                    lstTrialBalance.Add(tb);
                    TotDr += tb.DrAmt;
                    TotCr += tb.CrAmt;
                    TotOPCr += tb.CrAmtOP;
                    TotOPDr += tb.DrAmtOP;
                }
            }

            tb = new BLL.TrialBalance();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = "Total";
            tb.DrAmt = TotDr;
            tb.CrAmt = TotCr;
            tb.CrAmtOP = TotOPCr;
            tb.DrAmtOP = TotOPDr;
            lstTrialBalance.Add(tb);

            return lstTrialBalance;
        }
    }
}