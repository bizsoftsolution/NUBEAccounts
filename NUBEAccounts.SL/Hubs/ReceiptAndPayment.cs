using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {
        #region Payment Receipt
        public List<BLL.ReceiptAndPayment> ReceiptAndPayment_List(int? LedgerId, DateTime dtFrom, DateTime dtTo, string EntryNo, string Status, bool AccountHead)
        {
            List<BLL.ReceiptAndPayment> lstReceiptAndPayment = new List<BLL.ReceiptAndPayment>();
            BLL.ReceiptAndPayment rp = new BLL.ReceiptAndPayment();
            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId && (LedgerId == null || x.Id == LedgerId)).ToList();
            #region Ledger
            decimal? TotAmountDr = 0, TotAmountCr = 0;

            #region Without Grouping Account
            if (AccountHead == false)
            {
                foreach (var l in lstLedger)
                {
                    decimal? AmountDr = 0, AmountCr = 0;

                    #region PaymentDetils
                    foreach (var pd in l.PaymentDetails.Where(x => x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo &&
                                            (EntryNo == "" || x.Payment.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                                            && (Status == "" || x.Payment.Status == Status)).ToList())
                    {
                        rp = new BLL.ReceiptAndPayment();
                        rp.Ledger = new BLL.Ledger();
                        // pd.Ledger.toCopy<BLL.Ledger>(rp.Ledger);

                        rp.Ledger = LedgerDAL_BLL(pd.Ledger);
                        rp.EId = pd.Payment.Id;
                        rp.EType = 'P';
                        rp.EDate = pd.Payment.PaymentDate;
                        rp.RefNo = pd.Payment.PaymentMode == "Cheque" ? pd.Payment.ChequeNo : pd.Payment.RefNo;
                        rp.EntryNo = pd.Payment.EntryNo;
                        rp.AmountDr = pd.Amount;
                        AmountDr = AmountDr + pd.Amount;
                        rp.Status = pd.Payment.Status;
                        rp.PayTo = pd.Payment.PayTo;
                        rp.Particular = pd.Particular;
                        lstReceiptAndPayment.Add(rp);
                    }
                    #endregion

                    #region Payment
                    foreach (var p in l.Payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo
                                      && (EntryNo == "" || x.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                                      && (Status == "" || x.Status == Status)).ToList())
                    {
                        foreach (var pd in p.PaymentDetails)
                        {
                            rp = new BLL.ReceiptAndPayment();
                            rp.Ledger = new BLL.Ledger();
                            //p.Ledger.toCopy<BLL.Ledger>(rp.Ledger);

                            rp.Ledger = LedgerDAL_BLL(pd.Ledger);
                            rp.EId = p.Id;
                            rp.EType = 'P';
                            rp.EDate = p.PaymentDate;
                            rp.RefNo = p.PaymentMode == "Cheque" ? p.ChequeNo : p.RefNo;
                            rp.EntryNo = p.EntryNo;
                            rp.AmountDr = pd.Amount;
                            AmountDr = AmountDr + pd.Amount;
                            rp.Particular = pd.Particular;
                            lstReceiptAndPayment.Add(rp);
                        }
                    }
                    #endregion

                    #region Receipts
                    foreach (var r in l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo &&
                    (EntryNo == "" || x.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                    && (Status == "" || x.Status == Status)).ToList())
                    {
                        foreach (var rd in r.ReceiptDetails)
                        {
                            rp = new BLL.ReceiptAndPayment();
                            rp.Ledger = new BLL.Ledger();
                            rp.Ledger = LedgerDAL_BLL(rd.Ledger);
                            rp.EId = r.Id;
                            rp.EType = 'R';
                            rp.EDate = r.ReceiptDate;
                            rp.RefNo = r.ReceiptMode == "Cheque" ? r.ChequeNo : r.RefNo;
                            rp.EntryNo = r.EntryNo;
                            rp.AmountCr = rd.Amount;
                            AmountCr = AmountCr + rd.Amount;
                            rp.Status = rd.Receipt.Status;
                            rp.PayTo = rd.Receipt.ReceivedFrom;
                            rp.Particular = rd.Particulars;
                            lstReceiptAndPayment.Add(rp);
                        }
                    }
                    #endregion

                    #region Receipt Detils
                    foreach (var rd in l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo
                    && (EntryNo == "" || x.Receipt.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                    && (Status == "" || x.Receipt.Status == Status)).ToList())

                    {
                        rp = new BLL.ReceiptAndPayment();
                        rp.Ledger = new BLL.Ledger();
                        rp.Ledger = LedgerDAL_BLL(rd.Ledger);

                        rp.EId = rd.Receipt.Id;
                        rp.EType = 'R';
                        rp.EDate = rd.Receipt.ReceiptDate;
                        rp.RefNo = rd.Receipt.ReceiptMode == "Cheque" ? rd.Receipt.ChequeNo : rd.Receipt.RefNo;
                        rp.EntryNo = rd.Receipt.EntryNo;
                        rp.AmountCr = rd.Amount;
                        AmountCr = AmountCr + rd.Amount;
                        rp.Status = rd.Receipt.Status;
                        rp.PayTo = rd.Receipt.ReceivedFrom;
                        rp.Particular = rd.Particulars;
                        lstReceiptAndPayment.Add(rp);
                    }
                    #endregion
                    if (AmountCr != 0 || AmountDr != 0)
                    {
                        rp = new BLL.ReceiptAndPayment();
                        rp.Ledger = new BLL.Ledger();
                        rp.Ledger.AccountName = "Total";
                        rp.AmountCr = AmountCr;
                        rp.AmountDr = AmountDr;
                        TotAmountCr += rp.AmountCr;
                        TotAmountDr += rp.AmountDr;
                        lstReceiptAndPayment.Add(rp);
                    }
                }

                if (TotAmountDr != 0 || TotAmountDr != 0)
                {
                    rp = new BLL.ReceiptAndPayment();
                    rp.Ledger = new BLL.Ledger();
                    rp.Ledger.AccountName = "Grand Total";
                    rp.AmountCr = TotAmountCr;
                    rp.AmountDr = TotAmountDr;
                    lstReceiptAndPayment.Add(rp);
                }
            }
            #endregion

            #region Grouping Account
            else
            {
                decimal? TAmountDr = 0, TAmountCr = 0;
                foreach (var l in lstLedger)
                {
                    decimal? AmountDr = 0, AmountCr = 0;

                    rp = new BLL.ReceiptAndPayment();
                    rp.Ledger = new BLL.Ledger();
                    rp.Ledger = LedgerDAL_BLL(l);
                    foreach (var rd in l.Payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo &&
                                            (EntryNo == "" || x.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                                            && (Status == "" || x.Status == Status)))
                    {
                        foreach (var s in rd.PaymentDetails)
                        {
                            AmountDr += s.Amount;
                        }
                    }
                    foreach (var rd in l.PaymentDetails.Where(x => x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo &&
                                (EntryNo == "" || x.Payment.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                                && (Status == "" || x.Payment.Status == Status)))
                    {
                        AmountDr += rd.Amount;
                    }
                    foreach (var rd in l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo &&
                                (EntryNo == "" || x.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                                && (Status == "" || x.Status == Status)))
                    {
                        foreach (var s in rd.ReceiptDetails)
                        {
                            AmountCr += s.Amount;
                        }
                    }
                    foreach (var rd in l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo
                    && (EntryNo == "" || x.Receipt.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                    && (Status == "" || x.Receipt.Status == Status)))
                    {
                        AmountCr += rd.Amount;
                    }
                    rp.AmountCr = AmountCr;
                    rp.AmountDr = AmountDr;
                    if (AmountCr != 0 || AmountDr != 0)
                    {
                        lstReceiptAndPayment.Add(rp);
                    }

                    TAmountCr += AmountCr;
                    TAmountDr += AmountDr;

                }
                if (TAmountCr != 0 || TAmountDr != 0)
                {
                    rp = new BLL.ReceiptAndPayment();
                    rp.Ledger = new BLL.Ledger();
                    rp.Ledger.AccountName = "Total";
                    rp.AmountCr = TAmountCr;
                    rp.AmountDr = TAmountDr;
                    lstReceiptAndPayment.Add(rp);
                }
                #endregion
                #endregion
            }
            return lstReceiptAndPayment;
        }
        #endregion

        #region NEC
        public List<BLL.ReceiptAndPayment> NEC_List(int? LedgerId, DateTime dtFrom, DateTime dtTo, string EntryNo, string Status, bool AccountHead)
        {
            List<BLL.ReceiptAndPayment> lstReceiptAndPayment = new List<BLL.ReceiptAndPayment>();
            BLL.ReceiptAndPayment rp = new BLL.ReceiptAndPayment();
            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyId == Caller.CompanyId && (LedgerId == null || x.Id == LedgerId)).ToList();
            decimal? TotAmountDr = 0, TotAmountCr = 0;
            decimal? TLAmountDr = 0, TLAmountCr = 0;

            #region Without Grouping Account
            if (AccountHead == false)
            {
                foreach (var l in lstLedger)
                {
                    decimal? AmountDr = 0, AmountCr = 0;
                    decimal? LAmountDr = 0, LAmountCr = 0;
                    #region Payment Details
                    foreach (var pd in l.PaymentDetails.Where(x => x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo &&
                    (EntryNo == "" || x.Payment.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                    && (Status == "" || x.Payment.Status == Status)).ToList())
                    {
                        rp = new BLL.ReceiptAndPayment();
                        rp.Ledger = new BLL.Ledger();
                        rp.Ledger = LedgerDAL_BLL(pd.Ledger);
                        if (!(rp.Ledger.AccountGroup.GroupCode.Contains("236") || rp.Ledger.AccountGroup.GroupCode.Contains("363-01")) && !(rp.Ledger.LedgerName.Contains("salary") || rp.Ledger.LedgerName.Contains("sal ")))
                        {
                            rp.EId = pd.Payment.Id;
                            rp.EType = 'P';
                            rp.EDate = pd.Payment.PaymentDate;
                            rp.RefNo = pd.Payment.PaymentMode == "Cheque" ? pd.Payment.ChequeNo : pd.Payment.RefNo;
                            rp.EntryNo = pd.Payment.EntryNo;
                            rp.VoucherNo = pd.Payment.VoucherNo;
                            rp.AmountDr = pd.Amount;
                            AmountDr = AmountDr + pd.Amount;
                            rp.Status = pd.Payment.Status;
                            rp.PayTo = pd.Payment.PayTo;
                            rp.Particular = pd.Particular;
                            lstReceiptAndPayment.Add(rp);
                        }
                        else
                        {
                            rp.AmountDr= pd.Amount;
                            LAmountDr = LAmountDr + pd.Amount;

                        }
                    }
                    #endregion
                    #region Payments
                    foreach (var p in l.Payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo
                    && (EntryNo == "" || x.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                    && (Status == "" || x.Status == Status)).ToList())
                    {
                        if (!(p.Ledger.AccountGroup.GroupCode.Contains("236") || p.Ledger.AccountGroup.GroupCode.Contains("363-01")) && !(p.Ledger.LedgerName.Contains("salary") || p.Ledger.LedgerName.Contains("sal ")))
                        {
                            foreach (var pd in p.PaymentDetails)
                            {
                                if (!(pd.Ledger.AccountGroup.GroupCode.Contains("236") || pd.Ledger.AccountGroup.GroupCode.Contains("363-01")) && !(pd.Ledger.LedgerName.Contains("salary") || pd.Ledger.LedgerName.Contains("sal ")))
                                {
                                    rp = new BLL.ReceiptAndPayment();
                                    rp.Ledger = new BLL.Ledger();
                                    rp.Ledger = LedgerDAL_BLL(pd.Ledger);

                                    rp.EId = p.Id;
                                    rp.EType = 'P';
                                    rp.EDate = p.PaymentDate;
                                    rp.RefNo = p.PaymentMode == "Cheque" ? p.ChequeNo : p.RefNo;
                                    rp.EntryNo = p.EntryNo;
                                    rp.VoucherNo = p.VoucherNo;
                                    rp.AmountDr = pd.Amount;
                                    AmountDr = AmountDr + pd.Amount;
                                    rp.Particular = pd.Particular;
                                    lstReceiptAndPayment.Add(rp);
                                }
                                else
                                {
                                    rp.AmountDr = pd.Amount;
                                    LAmountDr = LAmountDr + pd.Amount;

                                }
                            }
                        }
                        else
                        {
                            rp.AmountDr = p.Amount;
                            LAmountDr = LAmountDr + p.Amount;

                        }
                    }
                    #endregion
                    #region Receipts
                    foreach (var r in l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo &&
                    (EntryNo == "" || x.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                    && (Status == "" || x.Status == Status)).ToList())
                    {
                        if (!(r.Ledger.AccountGroup.GroupCode.Contains("236") || r.Ledger.AccountGroup.GroupCode.Contains("363-01")) && !(r.Ledger.LedgerName.Contains("salary") || r.Ledger.LedgerName.Contains("sal ")))
                        {
                            foreach (var rd in r.ReceiptDetails)
                            {
                                rp = new BLL.ReceiptAndPayment();
                                rp.Ledger = new BLL.Ledger();
                                rp.Ledger = LedgerDAL_BLL(rd.Ledger);
                                if (!(rd.Ledger.AccountGroup.GroupCode.Contains("236") || rd.Ledger.AccountGroup.GroupCode.Contains("363-01")) && !(rd.Ledger.LedgerName.Contains("salary") || rd.Ledger.LedgerName.Contains("sal ")))
                                {
                                    rp.EId = r.Id;
                                    rp.EType = 'R';
                                    rp.EDate = r.ReceiptDate;
                                    rp.RefNo = r.ReceiptMode == "Cheque" ? r.ChequeNo : r.RefNo;
                                    rp.EntryNo = r.EntryNo;
                                    rp.VoucherNo = r.VoucherNo;
                                    rp.AmountCr = rd.Amount;
                                    AmountCr = AmountCr + rd.Amount;
                                    rp.Status = rd.Receipt.Status;
                                    rp.PayTo = rd.Receipt.ReceivedFrom;
                                    rp.Particular = rd.Particulars;
                                    lstReceiptAndPayment.Add(rp);
                                }
                                else
                                {
                                    rp.AmountCr = rd.Amount;
                                    LAmountCr = LAmountCr + rd.Amount;

                                }
                            }

                        }
                        else
                        {
                            rp.AmountCr = r.Amount;
                            LAmountCr = LAmountCr + r.Amount;

                        }
                    }
                    #endregion
                    #region Receipt Details
                    foreach (var rd in l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo
                    && (EntryNo == "" || x.Receipt.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                    && (Status == "" || x.Receipt.Status == Status)).ToList())
                    {
                        rp = new BLL.ReceiptAndPayment();
                        rp.Ledger = new BLL.Ledger();
                        rp.Ledger = LedgerDAL_BLL(rd.Ledger);
                        if (!(rd.Ledger.AccountGroup.GroupCode.Contains("236") || rp.Ledger.AccountGroup.GroupCode.Contains("363-01")) && !(rp.Ledger.LedgerName.Contains("salary") || rp.Ledger.LedgerName.Contains("sal ")))
                        {
                            rp.EId = rd.Receipt.Id;
                            rp.EType = 'R';
                            rp.EDate = rd.Receipt.ReceiptDate;
                            rp.RefNo = rd.Receipt.ReceiptMode == "Cheque" ? rd.Receipt.ChequeNo : rd.Receipt.RefNo;
                            rp.EntryNo = rd.Receipt.EntryNo;
                            rp.VoucherNo = rd.Receipt.VoucherNo;
                            rp.AmountCr = rd.Amount;
                            AmountCr = AmountCr + rd.Amount;
                            rp.Status = rd.Receipt.Status;
                            rp.PayTo = rd.Receipt.ReceivedFrom;
                            rp.Particular = rd.Particulars;
                            lstReceiptAndPayment.Add(rp);
                        }
                        else
                        {
                            rp.AmountCr = rd.Amount;
                            LAmountCr = LAmountCr + rd.Amount;

                        }
                    }
                    #endregion

                    if (LAmountDr != 0 || LAmountCr != 0)
                    {
                        rp = new BLL.ReceiptAndPayment();
                        rp.Ledger = new BLL.Ledger();
                        rp.Ledger = LedgerDAL_BLL(l);
                        rp.AmountCr = LAmountCr;
                        rp.AmountDr = LAmountDr;
                        TLAmountCr += LAmountCr;
                        TLAmountDr += LAmountDr;
                        lstReceiptAndPayment.Add(rp);
                     
                    }

                    if ((AmountCr != 0 || AmountDr != 0) && !(LAmountDr != 0 || LAmountCr != 0))
                    {
                        rp = new BLL.ReceiptAndPayment();
                        rp.Ledger = new BLL.Ledger();
                        rp.Ledger.AccountName = "Total";
                        rp.AmountCr = AmountCr;
                        rp.AmountDr = AmountDr;
                        TotAmountCr += rp.AmountCr;
                        TotAmountDr += rp.AmountDr;

                        lstReceiptAndPayment.Add(rp);
                    }
                }

                if (TotAmountDr != 0 || TotAmountDr != 0)
                {
                    rp = new BLL.ReceiptAndPayment();
                    rp.Ledger = new BLL.Ledger();
                    rp.Ledger.AccountName = "Grand Total";
                    rp.AmountCr = TotAmountCr+ TLAmountCr;
                    rp.AmountDr = TotAmountDr + TLAmountDr;
                    lstReceiptAndPayment.Add(rp);
                }
                
            }
            #endregion

            #region Grouping Account
            else
            {
                decimal? TAmountDr = 0, TAmountCr = 0;
                #region Ledger
                foreach (var l in lstLedger)
                {
                    decimal? AmountDr = 0, AmountCr = 0;
                    rp = new BLL.ReceiptAndPayment();
                    rp.Ledger = new BLL.Ledger();
                    rp.Ledger = LedgerDAL_BLL(l);
                    foreach (var rd in l.Payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo &&
                                            (EntryNo == "" || x.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                                            && (Status == "" || x.Status == Status)))
                    {
                        foreach (var s in rd.PaymentDetails)
                        {
                            AmountDr += s.Amount;
                        }
                    }
                    foreach (var rd in l.PaymentDetails.Where(x => x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo &&
                                (EntryNo == "" || x.Payment.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                                && (Status == "" || x.Payment.Status == Status)))
                    {
                        AmountDr += rd.Amount;
                    }
                    foreach (var rd in l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo &&
                                (EntryNo == "" || x.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                                && (Status == "" || x.Status == Status)))
                    {
                        foreach (var s in rd.ReceiptDetails)
                        {
                            AmountCr += s.Amount;
                        }
                    }
                    foreach (var rd in l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo
                    && (EntryNo == "" || x.Receipt.EntryNo.ToLower().Contains(EntryNo.ToLower()))
                    && (Status == "" || x.Receipt.Status == Status)))
                    {
                        AmountCr += rd.Amount;
                    }
                    rp.AmountCr = AmountCr;
                    rp.AmountDr = AmountDr;
                    if (AmountCr != 0 || AmountDr != 0)
                    {
                        lstReceiptAndPayment.Add(rp);
                    }

                    TAmountCr += AmountCr;
                    TAmountDr += AmountDr;
                }
                #endregion
                #region Total
                if (TAmountCr != 0 || TAmountDr != 0)
                {
                    rp = new BLL.ReceiptAndPayment();
                    rp.Ledger = new BLL.Ledger();
                    rp.Ledger.AccountName = "Total";
                    rp.AmountCr = TAmountCr;
                    rp.AmountDr = TAmountDr;
                    lstReceiptAndPayment.Add(rp);
                }
                #endregion
            }

            #endregion

            return lstReceiptAndPayment;
        }
        #endregion
    }
}





