using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {
        #region General Ledger
        public List<BLL.GeneralLedger> GeneralLedger_List(int LedgerId, DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.GeneralLedger> lstGeneralLedger = new List<BLL.GeneralLedger>();
            BLL.GeneralLedger gl = new BLL.GeneralLedger();
            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyDetail.Id == Caller.CompanyId && x.Id == LedgerId).ToList();

            #region Ledger
            decimal? OPDr, OPCr, PDr, PCr, RDr, RCr, JDr, JCr, BalAmt;
            BalAmt = 0;
            foreach (var l in lstLedger)
            {
                gl = new BLL.GeneralLedger();
                gl.Ledger = LedgerDAL_BLL(l);
                OPDr = l.OPDr ?? 0;
                OPCr = l.OPCr ?? 0;

                PDr = l.PaymentDetails.Where(x => x.Payment.PaymentDate < dtFrom).Sum(x => x.Amount);
                PCr = l.Payments.Where(x => x.PaymentDate < dtFrom).Sum(x => x.Amount);

                RDr = l.Receipts.Where(x => x.ReceiptDate < dtFrom).Sum(x => x.Amount);
                RCr = l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate < dtFrom).Sum(x => x.Amount);

                JDr = l.JournalDetails.Where(x => x.Journal.JournalDate < dtFrom).Sum(x => x.DrAmt);
                JCr = l.JournalDetails.Where(x => x.Journal.JournalDate < dtFrom).Sum(x => x.CrAmt);

                gl.DrAmt = OPDr + PDr + RDr + JDr;
                gl.CrAmt = OPCr + PCr + RCr + JCr;

                if (gl.DrAmt > gl.CrAmt)
                {
                    gl.DrAmt = gl.DrAmt - gl.CrAmt;
                    gl.CrAmt = 0;
                }
                else
                {
                    gl.CrAmt = gl.CrAmt - gl.DrAmt;
                    gl.DrAmt = 0;
                }
                BalAmt += (gl.DrAmt - gl.CrAmt);
                gl.BalAmt = Math.Abs(BalAmt.Value);

                gl.Ledger = new BLL.Ledger();
                gl.Particular = string.Format("Balance {0}", l.LedgerName);
                lstGeneralLedger.Add(gl);

                foreach (var pd in l.PaymentDetails.Where(x => x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralLedger();
                    gl.Ledger = new BLL.Ledger();
                    gl.Ledger = LedgerDAL_BLL(pd.Payment.Ledger);
                    gl.Particular = pd.Particular;
                    gl.EId = pd.Payment.Id;
                    gl.EType = BLL.FormPrefix.Payment;
                    gl.EDate = pd.Payment.PaymentDate;
                    gl.RefNo = pd.Payment.PaymentMode == "Cheque" ? pd.Payment.ChequeNo : pd.Payment.RefNo;
                    gl.EntryNo = pd.Payment.EntryNo;
                    gl.VoucherNo = pd.Payment.VoucherNo;
                    gl.DrAmt = pd.Amount;
                    gl.CrAmt = 0;
                    BalAmt += (gl.DrAmt - gl.CrAmt);
                    gl.BalAmt = Math.Abs(BalAmt.Value);
                    lstGeneralLedger.Add(gl);
                }


                foreach (var p in l.Payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo).ToList())
                {
                    foreach (var pd in p.PaymentDetails)
                    {
                        gl = new BLL.GeneralLedger();
                        gl.Ledger = new BLL.Ledger();
                        gl.Ledger = LedgerDAL_BLL(pd.Ledger);
                        gl.Particular = pd.Particular;
                        gl.EId = p.Id;
                        gl.EType = BLL.FormPrefix.Payment;
                        gl.EDate = p.PaymentDate;
                        gl.RefNo = p.PaymentMode == "Cheque" ? p.ChequeNo : p.RefNo;
                        gl.EntryNo = p.EntryNo;
                        gl.VoucherNo = p.VoucherNo;
                        gl.DrAmt = 0;
                        gl.CrAmt = pd.Amount;
                        BalAmt += (gl.DrAmt - gl.CrAmt);
                        gl.BalAmt = Math.Abs(BalAmt.Value);
                        lstGeneralLedger.Add(gl);
                    }
                }

                foreach (var r in l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo).ToList())
                {
                    foreach (var rd in r.ReceiptDetails)
                    {
                        gl = new BLL.GeneralLedger();
                        gl.Ledger = new BLL.Ledger();
                        gl.Ledger = LedgerDAL_BLL(rd.Ledger);
                        gl.Particular = rd.Particulars;
                        gl.EId = r.Id;
                        gl.EType = BLL.FormPrefix.Receipt;
                        gl.EDate = r.ReceiptDate;
                        gl.RefNo = r.ReceiptMode == "Cheque" ? r.ChequeNo : r.RefNo;
                        gl.EntryNo = r.EntryNo;
                        gl.VoucherNo = r.VoucherNo;
                        gl.DrAmt = rd.Amount;
                        gl.CrAmt = 0;
                        BalAmt += (gl.DrAmt - gl.CrAmt);
                        gl.BalAmt = Math.Abs(BalAmt.Value);
                        lstGeneralLedger.Add(gl);
                    }

                }
                foreach (var rd in l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralLedger();
                    gl.Ledger = new BLL.Ledger();
                    gl.Ledger = LedgerDAL_BLL(rd.Receipt.Ledger);
                    gl.Particular = rd.Particulars;
                    gl.EId = rd.Receipt.Id;
                    gl.EType = BLL.FormPrefix.Receipt;
                    gl.EDate = rd.Receipt.ReceiptDate;
                    gl.RefNo = rd.Receipt.ReceiptMode == "Cheque" ? rd.Receipt.ChequeNo : rd.Receipt.RefNo;
                    gl.EntryNo = rd.Receipt.EntryNo;
                    gl.VoucherNo = rd.Receipt.VoucherNo;
                    gl.DrAmt = 0;
                    gl.CrAmt = rd.Amount;
                    BalAmt += (gl.DrAmt - gl.CrAmt);
                    gl.BalAmt = Math.Abs(BalAmt.Value);
                    lstGeneralLedger.Add(gl);
                }

                foreach (var jd in l.JournalDetails.Where(x => x.Journal.JournalDate >= dtFrom && x.Journal.JournalDate <= dtTo).ToList())
                {
                    gl = new BLL.GeneralLedger();
                    gl.Ledger = new BLL.Ledger();

                    gl.Ledger = LedgerDAL_BLL(jd.Journal.JournalDetails.Where(x => (jd.DrAmt != 0 && x.CrAmt != 0) || (jd.CrAmt != 0 && x.DrAmt != 0)).FirstOrDefault().Ledger);
                    gl.Particular = jd.Particulars;
                    gl.EId = jd.Journal.Id;
                    gl.EDate = jd.Journal.JournalDate;
                    gl.RefNo = "";
                    gl.EntryNo = jd.Journal.EntryNo;
                    gl.VoucherNo = jd.Journal.VoucherNo;
                    gl.DrAmt = jd.DrAmt;
                    gl.CrAmt = jd.CrAmt;
                    BalAmt += (gl.DrAmt - gl.CrAmt);
                    gl.BalAmt = Math.Abs(BalAmt.Value);
                    lstGeneralLedger.Add(gl);
                }
                gl = new BLL.GeneralLedger();
                gl.Ledger = new BLL.Ledger();
                gl.Particular = "Total";
                gl.DrAmt = lstGeneralLedger.Sum(x => x.DrAmt);
                gl.CrAmt = lstGeneralLedger.Sum(x => x.CrAmt);
                gl.BalAmt = Math.Abs(BalAmt.Value);
                lstGeneralLedger.Add(gl);

            }
            #endregion


            return lstGeneralLedger;
        }
        #endregion

        #region Activity Report
        public List<BLL.GeneralLedger> Activity_ToList(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.GeneralLedger> lstActivity_ToList = new List<BLL.GeneralLedger>();
            BLL.GeneralLedger gl = new BLL.GeneralLedger();
            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyDetail.Id == Caller.CompanyId).ToList();

            #region Ledger

            foreach (var l in lstLedger)
            {
                int? i = 0;
                decimal? BalAmt = 0, TotDr = 0, TotCr = 0;
                if (l.PaymentDetails.Where(x => x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo).ToList().Count > 0 ||
                    l.Payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo).ToList().Count()>0 ||
                    l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo).ToList().Count > 0 ||
                    l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo).ToList().Count > 0 ||
                    l.JournalDetails.Where(x => x.Journal.JournalDate >= dtFrom && x.Journal.JournalDate <= dtTo).ToList().Count > 0)
                {                   
                    gl = new BLL.GeneralLedger();
                    gl.Ledger = LedgerDAL_BLL(l);
                    gl.Ledger = new BLL.Ledger();
                    gl.SNo = null;
                    gl.Particular = string.Format("{0}", l.LedgerName);                    
                    lstActivity_ToList.Add(gl);

                    foreach (var pd in l.PaymentDetails.Where(x => x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo).ToList())
                    {
                       
                        gl = new BLL.GeneralLedger();
                        i = i + 1;
                        gl.SNo = i;
                        gl.Ledger = new BLL.Ledger();
                        gl.Ledger = LedgerDAL_BLL(pd.Payment.Ledger);
                        gl.Particular = pd.Particular;
                        gl.EId = pd.Payment.Id;
                        gl.EType = BLL.FormPrefix.Payment;
                        gl.EDate = pd.Payment.PaymentDate;
                        gl.RefNo = pd.Payment.PaymentMode == "Cheque" ? pd.Payment.ChequeNo : pd.Payment.RefNo;
                        gl.EntryNo = pd.Payment.EntryNo;
                        gl.VoucherNo = pd.Payment.VoucherNo;
                        gl.DrAmt = pd.Amount;
                        gl.CrAmt = 0;
                        BalAmt += (gl.DrAmt - gl.CrAmt);
                        if (BalAmt != null) gl.BalAmt = Math.Abs(BalAmt.Value);
                        if ((gl.CrAmt != 0 || gl.DrAmt != 0) && (gl.CrAmt != null || gl.DrAmt != null))
                        {
                            lstActivity_ToList.Add(gl);
                        }
                        TotDr += gl.DrAmt;
                        TotCr += gl.CrAmt;
                    }

                    foreach (var p in l.Payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo).ToList())
                    {
                        foreach (var pd in p.PaymentDetails)
                        {
                            
                            gl = new BLL.GeneralLedger();
                            i = i + 1;
                            gl.SNo = i; gl.Ledger = new BLL.Ledger();
                            gl.Ledger = LedgerDAL_BLL(pd.Ledger);
                            gl.Particular = pd.Particular;
                            gl.EId = p.Id;
                            gl.EType = BLL.FormPrefix.Payment;
                            gl.EDate = p.PaymentDate;
                            gl.RefNo = p.PaymentMode == "Cheque" ? p.ChequeNo : p.RefNo;
                            gl.EntryNo = p.EntryNo;
                            gl.VoucherNo = p.VoucherNo;
                            gl.DrAmt = 0;
                            gl.CrAmt = pd.Amount;
                            BalAmt += (gl.DrAmt - gl.CrAmt);
                            if (BalAmt != null) gl.BalAmt = Math.Abs(BalAmt.Value);
                            lstActivity_ToList.Add(gl);                            
                            TotDr += gl.DrAmt;
                            TotCr += gl.CrAmt;
                        }
                    }

                    foreach (var r in l.Receipts.Where(x => x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo).ToList())
                    {
                        foreach (var rd in r.ReceiptDetails)
                        {
                            if (rd.Amount != 0)
                            {
                               
                                gl = new BLL.GeneralLedger();
                                i = i + 1;
                                gl.SNo = i; gl.Ledger = new BLL.Ledger();
                                gl.Ledger = LedgerDAL_BLL(rd.Ledger);
                                gl.Particular = rd.Particulars;
                                gl.EId = r.Id;
                                gl.EType = BLL.FormPrefix.Receipt;
                                gl.EDate = r.ReceiptDate;
                                gl.RefNo = r.ReceiptMode == "Cheque" ? r.ChequeNo : r.RefNo;
                                gl.EntryNo = r.EntryNo;
                                gl.VoucherNo = r.VoucherNo;
                                gl.DrAmt = rd.Amount;
                                gl.CrAmt = 0;
                                BalAmt += (gl.DrAmt - gl.CrAmt);
                                gl.BalAmt = Math.Abs(BalAmt.Value);
                                lstActivity_ToList.Add(gl);
                            }
                            TotDr += gl.DrAmt;
                            TotCr += gl.CrAmt;
                        }

                    }
                    foreach (var rd in l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo).ToList())
                    {
                        if (rd.Amount != 0)
                        {
                          
                            gl = new BLL.GeneralLedger();
                            i = i + 1;
                            gl.SNo = i; gl.Ledger = new BLL.Ledger();
                            gl.Ledger = LedgerDAL_BLL(rd.Receipt.Ledger);
                            gl.Particular = rd.Particulars;
                            gl.EId = rd.Receipt.Id;
                            gl.EType = BLL.FormPrefix.Receipt;
                            gl.EDate = rd.Receipt.ReceiptDate;
                            gl.RefNo = rd.Receipt.ReceiptMode == "Cheque" ? rd.Receipt.ChequeNo : rd.Receipt.RefNo;
                            gl.EntryNo = rd.Receipt.EntryNo;
                            gl.VoucherNo = rd.Receipt.VoucherNo;
                            gl.DrAmt = 0;
                            gl.CrAmt = rd.Amount;
                            BalAmt += (gl.DrAmt - gl.CrAmt);
                            gl.BalAmt = Math.Abs(BalAmt.Value);

                            lstActivity_ToList.Add(gl);
                        }
                        TotDr += gl.DrAmt;
                        TotCr += gl.CrAmt;
                    }

                    foreach (var jd in l.JournalDetails.Where(x => x.Journal.JournalDate >= dtFrom && x.Journal.JournalDate <= dtTo).ToList())
                    {
                        if (jd.CrAmt != 0 || jd.DrAmt != 0)
                        {
                           
                            gl = new BLL.GeneralLedger();
                            i = i + 1;
                            gl.SNo = i; gl.Ledger = new BLL.Ledger();
                            gl.Ledger = LedgerDAL_BLL(jd.Journal.JournalDetails.Where(x => (jd.DrAmt != 0 && x.CrAmt != 0) || (jd.CrAmt != 0 && x.DrAmt != 0)).FirstOrDefault().Ledger);
                            gl.Particular = jd.Particulars;
                            gl.EId = jd.Journal.Id;
                            gl.EDate = jd.Journal.JournalDate;
                            gl.RefNo = "";
                            gl.EntryNo = jd.Journal.EntryNo;
                            gl.VoucherNo = jd.Journal.VoucherNo;
                            gl.DrAmt = jd.DrAmt;
                            gl.CrAmt = jd.CrAmt;
                            BalAmt += (gl.DrAmt - gl.CrAmt);
                            gl.BalAmt = Math.Abs(BalAmt.Value);

                            lstActivity_ToList.Add(gl);
                        }
                        TotDr += gl.DrAmt;
                        TotCr += gl.CrAmt;
                    }
                    if ((TotCr != 0 || TotDr != 0) && (TotCr != null || TotDr != null))
                    {
                        gl = new BLL.GeneralLedger();
                        gl.Ledger = new BLL.Ledger();
                        gl.Particular = string.Format("Total {0}", l.LedgerName);
                        gl.DrAmt = TotDr;
                        gl.CrAmt = TotCr;
                        gl.BalAmt = Math.Abs(BalAmt.Value);
                        lstActivity_ToList.Add(gl);
                    }
                }
            }
            #endregion


            return lstActivity_ToList;
        }

        #endregion
    }
}