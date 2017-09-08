using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {
        #region ExpenseReport

        public List<BLL.ExpenseReport> ExpenseReport_List(int? LedgerId, DateTime dtFrom, DateTime dtTo, string EntryNo, string PayTo,bool AccountHead)
        {
            List<BLL.ExpenseReport> lstExpenseReport = new List<BLL.ExpenseReport>();

         
            BLL.ExpenseReport rp = new BLL.ExpenseReport();

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId && (x.AccountGroup.GroupName == BLL.DataKeyValue.Expenses_Key || x.AccountGroup.AccountGroup2.GroupName == BLL.DataKeyValue.Expenses_Key) && (LedgerId == null || x.Id == LedgerId)).ToList();

            if(AccountHead==false)
            {
                #region Ledger
                foreach (var l in lstLedger)
                {
                    #region PaymentDetails
                    foreach (var pd in l.PaymentDetails.Where(x => x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo &&
                    (EntryNo == "" || x.Payment.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                    && (PayTo == "" || x.Payment.PayTo.ToLower().Contains(PayTo.ToLower()))).ToList())
                    {
                        if (pd.Amount != 0)
                        {
                            rp = new BLL.ExpenseReport();
                            rp.Particulars = l.LedgerName;
                            lstExpenseReport.Add(rp);

                            if (rp.Payto == "" || rp.Payto == null)
                            {
                                rp = new BLL.ExpenseReport();
                                rp.Particulars = pd.Payment.PayTo;
                                lstExpenseReport.Add(rp);
                            }

                            rp = new BLL.ExpenseReport();
                            rp.EntryNo = pd.Payment.EntryNo;
                            rp.VoucherNo = pd.Payment.VoucherNo;
                            rp.Particulars = pd.Particular;
                            rp.DrAmt = pd.Amount;
                            rp.CrAmt = 0;
                            lstExpenseReport.Add(rp);

                            rp = new BLL.ExpenseReport();
                            lstExpenseReport.Add(rp);

                        }
                    }
                    #endregion

                    #region Payment
                    foreach (var p in l.Payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo
                    && (EntryNo == "" || x.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                    && (PayTo == "" || x.PayTo.ToLower().Contains(PayTo.ToLower()))).ToList())
                    {
                        if (p.Amount != 0)
                        {

                            foreach (var pd in p.PaymentDetails)
                            {
                                rp = new BLL.ExpenseReport();
                                rp.Particulars = l.LedgerName;

                                if (p.PayTo == "" || p.PayTo == null)
                                {
                                    rp.Particulars = p.PayTo;
                                    lstExpenseReport.Add(rp);
                                }
                                rp = new BLL.ExpenseReport();
                                rp.DrAmt = pd.Amount;
                                rp.CrAmt = 0;
                                rp.EntryNo = pd.Payment.EntryNo;
                                rp.VoucherNo = pd.Payment.VoucherNo;
                                rp.Particulars = pd.Particular;
                                lstExpenseReport.Add(rp);
                            }
                        }
                    }
                    #endregion

                    #region Receipts
                    foreach (var r in l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo &&
                    (EntryNo == "" || x.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                    && (PayTo == "" || x.ReceivedFrom.ToLower().Contains(PayTo.ToLower()))).ToList())

                    {
                        if (r.Amount != 0)
                        {
                            foreach (var rd in r.ReceiptDetails)
                            {
                                rp = new BLL.ExpenseReport();
                                rp.Particulars = r.Ledger.LedgerName;
                                lstExpenseReport.Add(rp);
                                rp = new BLL.ExpenseReport();
                                if (rd.Receipt.ReceivedFrom == "" || rd.Receipt.ReceivedFrom == null)
                                {
                                    rp.Particulars = rd.Receipt.ReceivedFrom;
                                    lstExpenseReport.Add(rp);
                                }

                                rp = new BLL.ExpenseReport();
                                rp.EntryNo = r.EntryNo;
                                rp.VoucherNo = r.VoucherNo;
                                rp.DrAmt = 0;
                                rp.CrAmt = r.Amount;
                                rp.Particulars = r.Particulars;
                                lstExpenseReport.Add(rp);
                                rp = new BLL.ExpenseReport();
                                lstExpenseReport.Add(rp);
                            }
                        }
                    }

                    #endregion

                    #region ReceiptDetails
                    foreach (var rd in l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo
                    && (EntryNo == "" || x.Receipt.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                    && (PayTo == "" || x.Receipt.ReceivedFrom.ToLower().Contains(PayTo.ToLower()))).ToList())

                    {
                        if (rd.Amount != 0)
                        {

                            rp = new BLL.ExpenseReport();
                            rp.Particulars = rd.Ledger.LedgerName;
                            lstExpenseReport.Add(rp);

                            rp = new BLL.ExpenseReport();
                            if (rd.Receipt.ReceivedFrom == "" || rd.Receipt.ReceivedFrom == null)
                            {
                                rp.Particulars = rd.Receipt.ReceivedFrom;
                                lstExpenseReport.Add(rp);
                            }

                            rp = new BLL.ExpenseReport();
                            rp.EntryNo = rd.Receipt.EntryNo;
                            rp.VoucherNo = rd.Receipt.VoucherNo;
                            rp.DrAmt = 0;
                            rp.CrAmt = rd.Amount;
                            rp.Particulars = rd.Particulars;
                            lstExpenseReport.Add(rp);
                            rp = new BLL.ExpenseReport();
                            lstExpenseReport.Add(rp);
                        }
                    }
                    #endregion

                    #region journal
                    foreach (var rd in l.JournalDetails.Where(x => x.Journal.JournalDate >= dtFrom && x.Journal.JournalDate <= dtTo
                                   && (EntryNo == "" || x.Journal.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                                   ).ToList())
                    {
                        if (rd.CrAmt != 0 || rd.DrAmt != 0)
                        {

                            rp = new BLL.ExpenseReport();
                            rp.Particulars = rd.Ledger.LedgerName;
                            lstExpenseReport.Add(rp);

                            rp = new BLL.ExpenseReport();
                            rp.EntryNo = rd.Journal.EntryNo;
                            rp.VoucherNo = rd.Journal.VoucherNo;
                            rp.DrAmt = rd.DrAmt;
                            rp.CrAmt = rd.CrAmt;
                            rp.Particulars = rd.Particulars;
                       
                            lstExpenseReport.Add(rp);
                            rp = new BLL.ExpenseReport();
                            lstExpenseReport.Add(rp);

                        }
                    }
                    #endregion

                }
                if (lstExpenseReport.Sum(x => x.DrAmt) != 0 || lstExpenseReport.Sum(x => x.CrAmt) != 0)
                {
                    rp = new BLL.ExpenseReport();
                    rp.Particulars = "Total";
                    rp.DrAmt = lstExpenseReport.Sum(x => x.DrAmt);
                    rp.CrAmt = lstExpenseReport.Sum(x => x.CrAmt);
                    lstExpenseReport.Add(rp);
                }
                return lstExpenseReport;
                #endregion
            }
            else
            {
                #region Ledger
                foreach (var l in lstLedger)
                {
                    #region PaymentDetails
                    foreach (var pd in l.PaymentDetails.Where(x => x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo &&
                    (EntryNo == "" || x.Payment.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                    && (PayTo == "" || x.Payment.PayTo.ToLower().Contains(PayTo.ToLower()))).ToList())
                    {
                        if (pd.Amount != 0)
                        {
                            rp = new BLL.ExpenseReport();
                            rp.Particulars = l.LedgerName;
                            rp.EntryNo = pd.Payment.EntryNo;
                            rp.VoucherNo = pd.Payment.VoucherNo;
                            rp.DrAmt = pd.Amount;
                            rp.CrAmt =0;
                            lstExpenseReport.Add(rp);

                        }
                    }
                    #endregion

                    #region Payment
                    foreach (var p in l.Payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo
                    && (EntryNo == "" || x.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                    && (PayTo == "" || x.PayTo.ToLower().Contains(PayTo.ToLower()))).ToList())
                    {
                        if (p.Amount != 0)
                        {

                            foreach (var pd in p.PaymentDetails)
                            {
                                rp = new BLL.ExpenseReport();
                                rp.Particulars = l.LedgerName;
                                rp.EntryNo = pd.Payment.EntryNo;
                                rp.VoucherNo = pd.Payment.VoucherNo;
                                rp.DrAmt = pd.Amount;
                                rp.CrAmt = 0;
                                lstExpenseReport.Add(rp);
                            }
                        }
                    }
                    #endregion

                    #region Receipts
                    foreach (var r in l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo &&
                    (EntryNo == "" || x.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                    && (PayTo == "" || x.ReceivedFrom.ToLower().Contains(PayTo.ToLower()))).ToList())

                    {
                        if (r.Amount != 0)
                        {
                            foreach (var rd in r.ReceiptDetails)
                            {
                                rp = new BLL.ExpenseReport();
                                rp.Particulars = r.Ledger.LedgerName;
                                rp.EntryNo = r.EntryNo;
                                rp.VoucherNo = r.VoucherNo;
                                rp.DrAmt = 0;
                                rp.CrAmt = r.Amount;
                                lstExpenseReport.Add(rp);
                            }
                        }
                    }

                    #endregion

                    #region ReceiptDetails
                    foreach (var rd in l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo
                    && (EntryNo == "" || x.Receipt.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                    && (PayTo == "" || x.Receipt.ReceivedFrom.ToLower().Contains(PayTo.ToLower()))).ToList())

                    {
                        if (rd.Amount != 0)
                        {

                            rp = new BLL.ExpenseReport();
                            rp.Particulars = rd.Ledger.LedgerName;
                            rp.EntryNo = rd.Receipt.EntryNo;
                            rp.VoucherNo = rd.Receipt.VoucherNo;
                            rp.DrAmt = 0;
                            rp.CrAmt = rd.Amount;
                            lstExpenseReport.Add(rp);
                          
                        }
                    }
                    #endregion

                    #region journal
                    foreach (var rd in l.JournalDetails.Where(x => x.Journal.JournalDate >= dtFrom && x.Journal.JournalDate <= dtTo
                                   && (EntryNo == "" || x.Journal.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                                   ).ToList())
                    {
                        if (rd.CrAmt != 0 || rd.DrAmt != 0)
                        {
                            rp = new BLL.ExpenseReport();
                            rp.Particulars = rd.Ledger.LedgerName;
                            rp.EntryNo = rd.Journal.EntryNo;
                            rp.VoucherNo = rd.Journal.VoucherNo;
                            rp.DrAmt = rd.DrAmt;
                            rp.CrAmt = rd.CrAmt;
                            lstExpenseReport.Add(rp);
                        }
                    }
                    #endregion

                }
                if (lstExpenseReport.Sum(x => x.DrAmt) != 0 || lstExpenseReport.Sum(x => x.CrAmt) != 0)
                {
                    rp = new BLL.ExpenseReport();
                    rp.Particulars = "Total";
                    rp.DrAmt = lstExpenseReport.Sum(x => x.DrAmt);
                    rp.CrAmt = lstExpenseReport.Sum(x => x.CrAmt);
                    lstExpenseReport.Add(rp);
                }
                return lstExpenseReport;
                #endregion
            }

        }
        #endregion
    }
}