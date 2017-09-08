using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {
        public List<BLL.VoucherReport> VoucherReport_List(DateTime dtFrom, DateTime dtTo,int? LedgerId)
        {
            List<BLL.VoucherReport> lstVoucherReport = new List<BLL.VoucherReport>();
            BLL.VoucherReport tb = new BLL.VoucherReport();

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId & (LedgerId == null || x.Id == LedgerId)).ToList();
            decimal TotDr = 0, TotCr = 0, GTotCr = 0, GTotDr = 0;

            foreach (var l in lstLedger)
            {
                #region Payment
                foreach (var d1 in l.Payments.Where(x=>x.PaymentDate>=dtFrom && x.PaymentDate<=dtTo))
                {
                    decimal PTotDr = 0, PTotCr = 0;
                    tb = new BLL.VoucherReport();
                    tb.Ledger = LedgerDAL_BLL(l);

                    tb.Ledger.AccountName = d1.Ledger.LedgerName;
                    tb.CrAmt = d1.Amount;
                    PTotCr += d1.Amount;
                    tb.EntryNo = d1.EntryNo;
                    tb.VoucherNo = d1.VoucherNo;
                    tb.VDate = d1.PaymentDate;
                    tb.Particulars = d1.Particulars;
                    tb.EType = "P";
                    lstVoucherReport.Add(tb);
                    foreach (var d2 in d1.PaymentDetails)
                    {
                        tb = new BLL.VoucherReport();
                        tb.Ledger = LedgerDAL_BLL(d2.Ledger);
                        tb.VDate = null;
                        tb.Ledger.AccountName = d2.Ledger.LedgerName;
                        tb.EntryNo = "";
                        tb.Particulars = "";
                        tb.EType = "";
                        tb.DrAmt = d2.Amount;
                        PTotDr += d2.Amount;
                        lstVoucherReport.Add(tb);
                    }
                    if (PTotDr != 0 || PTotCr != 0)
                    {
                        tb = new BLL.VoucherReport();
                        tb.Ledger = new BLL.Ledger();
                        tb.Ledger.AccountName = "Total";
                        tb.DrAmt = PTotDr;
                        tb.CrAmt = PTotCr;
                        lstVoucherReport.Add(tb);
                        GTotCr += PTotCr;
                        GTotDr += PTotDr;
                    }
                }
                
                #endregion

                #region Receipt
                foreach (var d1 in l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo))
                {
                    tb = new BLL.VoucherReport();
                    tb.Ledger = LedgerDAL_BLL(l);

                    decimal RTotDr = 0, RTotCr = 0;
                    tb.Ledger.AccountName = d1.Ledger.LedgerName;
                    tb.DrAmt = d1.Amount;
                    TotDr += d1.Amount;
                    tb.EntryNo = d1.EntryNo;
                    tb.VoucherNo = d1.VoucherNo;
                    tb.VDate = d1.ReceiptDate;
                    tb.Particulars = d1.Particulars;
                    tb.EType = "R";
                    lstVoucherReport.Add(tb);
                    foreach (var d2 in d1.ReceiptDetails)
                    {
                        tb = new BLL.VoucherReport();
                        tb.Ledger = LedgerDAL_BLL(d2.Ledger);
                        tb.VDate = null;
                        tb.Ledger.AccountName = d2.Ledger.LedgerName;
                        tb.EntryNo = "";
                        tb.Particulars = "";
                        tb.EType = "";
                        tb.CrAmt = d2.Amount;
                        RTotCr += d2.Amount;
                        lstVoucherReport.Add(tb);
                    }
                    if (RTotDr != 0 || RTotCr != 0)
                    {
                        tb = new BLL.VoucherReport();
                        tb.Ledger = new BLL.Ledger();
                        tb.Ledger.AccountName = "Total";
                        tb.DrAmt = RTotDr;
                        tb.CrAmt = RTotCr;
                        lstVoucherReport.Add(tb);
                        GTotCr += RTotCr;
                        GTotDr += RTotDr;
                    }
                }

                #endregion

                #region Journal
                decimal JTotDr = 0, JTotCr = 0;
                foreach (var d1 in l.JournalDetails.Where(x => x.Journal.JournalDate >= dtFrom && x.Journal.JournalDate <= dtTo))
                {
                    tb = new BLL.VoucherReport();
                    tb.Ledger = LedgerDAL_BLL(l);
                    tb.Ledger.AccountName = d1.Ledger.LedgerName;
                    tb.DrAmt = d1.DrAmt;
                    JTotDr += d1.DrAmt;
                    tb.EntryNo = d1.Journal.EntryNo;
                    tb.VoucherNo = d1.Journal.VoucherNo;
                    tb.VDate = d1.Journal.JournalDate;
                    tb.Particulars = d1.Particulars;
                    tb.CrAmt = d1.CrAmt;
                    JTotCr += d1.CrAmt;
                    tb.EType = "J";
                    lstVoucherReport.Add(tb);                                                         
                }
                if (JTotDr != 0 || JTotCr != 0)
                {
                    tb = new BLL.VoucherReport();
                    tb.Ledger = new BLL.Ledger();
                    tb.Ledger.AccountName = "Total";
                    tb.DrAmt = JTotDr;
                    tb.CrAmt = JTotCr;
                    lstVoucherReport.Add(tb);
                    GTotCr += JTotDr;
                    GTotDr += JTotDr;
                }
                #endregion
            }

            tb = new BLL.VoucherReport();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = "Grand Total";
            tb.DrAmt = GTotDr;
            tb.CrAmt = GTotCr;
            lstVoucherReport.Add(tb);

            return lstVoucherReport;
        }
    }
}
