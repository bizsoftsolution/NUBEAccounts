using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {
        #region IncomeReport

        public List<BLL.IncomeReport> IncomeReport_List(int? LedgerId, DateTime dtFrom, DateTime dtTo, string EntryNo, string PayTo, bool AccountHead)
        {
            List<BLL.IncomeReport> lstIncomeReport = new List<BLL.IncomeReport>();

            BLL.IncomeReport rp = new BLL.IncomeReport();

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId && (x.AccountGroup.GroupName == BLL.DataKeyValue.Income_Key || x.AccountGroup.AccountGroup2.GroupName == BLL.DataKeyValue.Income_Key) && (LedgerId == null || x.Id == LedgerId)).ToList();

            if (AccountHead == false)
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
                            rp = new BLL.IncomeReport();
                            rp.Particulars = l.LedgerName;
                            lstIncomeReport.Add(rp);

                            if (rp.Payto == "" || rp.Payto == null)
                            {
                                rp = new BLL.IncomeReport();
                                rp.Particulars = pd.Payment.PayTo;
                                lstIncomeReport.Add(rp);
                            }

                            rp = new BLL.IncomeReport();
                            rp.EntryNo = pd.Payment.EntryNo;
                            rp.VoucherNo = pd.Payment.VoucherNo;
                            rp.Particulars = pd.Particular;
                            rp.DrAmt = pd.Amount;
                            rp.CrAmt = 0;
                            lstIncomeReport.Add(rp);

                            rp = new BLL.IncomeReport();
                            lstIncomeReport.Add(rp);

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
                                rp = new BLL.IncomeReport();
                                rp.Particulars = l.LedgerName;
                                lstIncomeReport.Add(rp);

                                rp = new BLL.IncomeReport();
                                if (p.PayTo == "" || p.PayTo == null)
                                {
                                    rp.Particulars = p.PayTo;
                                    lstIncomeReport.Add(rp);
                                }
                              
                                rp = new BLL.IncomeReport();
                                rp.DrAmt = pd.Amount;
                                rp.CrAmt = 0;
                                rp.EntryNo = pd.Payment.EntryNo;
                                rp.VoucherNo = pd.Payment.VoucherNo;
                                rp.Particulars = pd.Particular;
                                lstIncomeReport.Add(rp);
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
                                rp = new BLL.IncomeReport();
                                rp.Particulars = r.Ledger.LedgerName;
                                lstIncomeReport.Add(rp);

                                rp = new BLL.IncomeReport();
                                if (rd.Receipt.ReceivedFrom == "" || rd.Receipt.ReceivedFrom == null)
                                {
                                    rp.Particulars = rd.Receipt.ReceivedFrom;
                                    lstIncomeReport.Add(rp);
                                }

                                rp = new BLL.IncomeReport();
                                rp.EntryNo = r.EntryNo;
                                rp.VoucherNo = r.VoucherNo;
                                rp.DrAmt = 0;
                                rp.CrAmt = r.Amount;
                                rp.Particulars = r.Particulars;
                                lstIncomeReport.Add(rp);

                                rp = new BLL.IncomeReport();
                                lstIncomeReport.Add(rp);
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

                            rp = new BLL.IncomeReport();
                            rp.Particulars = rd.Ledger.LedgerName;
                            lstIncomeReport.Add(rp);

                            rp = new BLL.IncomeReport();
                            if (rd.Receipt.ReceivedFrom == "" || rd.Receipt.ReceivedFrom == null)
                            {
                                rp.Particulars = rd.Receipt.ReceivedFrom;
                                lstIncomeReport.Add(rp);
                            }

                            rp = new BLL.IncomeReport();
                            rp.EntryNo = rd.Receipt.EntryNo;
                            rp.VoucherNo = rd.Receipt.VoucherNo;
                            rp.DrAmt = 0;
                            rp.CrAmt = rd.Amount;
                            rp.Particulars = rd.Particulars;
                            lstIncomeReport.Add(rp);
                            rp = new BLL.IncomeReport();
                            lstIncomeReport.Add(rp);
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

                            rp = new BLL.IncomeReport();
                            rp.Particulars = rd.Ledger.LedgerName;
                            lstIncomeReport.Add(rp);

                            rp = new BLL.IncomeReport();
                            rp.EntryNo = rd.Journal.EntryNo;
                            rp.VoucherNo = rd.Journal.VoucherNo;
                            rp.DrAmt = rd.DrAmt;
                            rp.CrAmt = rd.CrAmt;
                            rp.Particulars = rd.Particulars;
                            rp = new BLL.IncomeReport();
                            lstIncomeReport.Add(rp);

                        }
                    }
                    #endregion

                }
                if (lstIncomeReport.Sum(x => x.DrAmt) != 0 || lstIncomeReport.Sum(x => x.CrAmt) != 0)
                {
                    rp = new BLL.IncomeReport();
                    rp.Particulars = "Total";
                    rp.DrAmt = lstIncomeReport.Sum(x => x.DrAmt);
                    rp.CrAmt = lstIncomeReport.Sum(x => x.CrAmt);
                    lstIncomeReport.Add(rp);
                }
                return lstIncomeReport;
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
                            rp = new BLL.IncomeReport();
                            rp.Particulars = l.LedgerName;
                            rp.EntryNo = pd.Payment.EntryNo;
                            rp.VoucherNo = pd.Payment.VoucherNo;
                            rp.DrAmt = pd.Amount;
                            rp.CrAmt = 0;
                            lstIncomeReport.Add(rp);

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
                                rp = new BLL.IncomeReport();
                                rp.Particulars = l.LedgerName;
                                rp.EntryNo = pd.Payment.EntryNo;
                                rp.VoucherNo = pd.Payment.VoucherNo;
                                rp.DrAmt = pd.Amount;
                                rp.CrAmt = 0;
                                lstIncomeReport.Add(rp);
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
                                rp = new BLL.IncomeReport();
                                rp.Particulars = r.Ledger.LedgerName;
                                rp.EntryNo = rd.Receipt.EntryNo;
                                rp.VoucherNo = rd.Receipt.VoucherNo;
                                rp.DrAmt = 0;
                                rp.CrAmt = r.Amount;
                                lstIncomeReport.Add(rp);
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
                            rp = new BLL.IncomeReport();
                            rp.Particulars = rd.Ledger.LedgerName;
                            rp.EntryNo = rd.Receipt.EntryNo;
                            rp.VoucherNo = rd.Receipt.VoucherNo;
                            rp.DrAmt = 0;
                            rp.CrAmt = rd.Amount;
                            lstIncomeReport.Add(rp);
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

                            rp = new BLL.IncomeReport();
                            rp.Particulars = rd.Ledger.LedgerName;
                            rp.EntryNo = rd.Journal.EntryNo;
                            rp.VoucherNo = rd.Journal.VoucherNo;
                            rp.DrAmt = rd.DrAmt;
                            rp.CrAmt = rd.CrAmt;
                            lstIncomeReport.Add(rp);

                        }
                    }
                    #endregion

                }
                if (lstIncomeReport.Sum(x => x.DrAmt) != 0 || lstIncomeReport.Sum(x => x.CrAmt) != 0)
                {
                    rp = new BLL.IncomeReport();
                    rp.Particulars = "Total";
                    rp.DrAmt = lstIncomeReport.Sum(x => x.DrAmt);
                    rp.CrAmt = lstIncomeReport.Sum(x => x.CrAmt);
                    lstIncomeReport.Add(rp);
                }
                return lstIncomeReport;
                #endregion
            }
        }
        #endregion
    }
}

