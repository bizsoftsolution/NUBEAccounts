using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {
        public List<BLL.BankReconcilation> BankReconcilation_List(int LedgerId, DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.BankReconcilation> lstBankReconcilation = new List<BLL.BankReconcilation>();


            BLL.BankReconcilation gl = new BLL.BankReconcilation();

            var lstLedger = DB.Ledgers.Where(x => x.AccountGroup.CompanyDetail.Id == Caller.CompanyId && x.Id == LedgerId).ToList();

            #region Ledger

            foreach (var l in lstLedger)
            {

                foreach (var pd in l.PaymentDetails.Where(x => x.Payment.Status == "Process" && x.Payment.PaymentDate >= dtFrom && x.Payment.PaymentDate <= dtTo).ToList())
                {
                    gl = new BLL.BankReconcilation();
                    gl.Ledger = new BLL.Ledger();
                    gl.Ledger = LedgerDAL_BLL(pd.Payment.Ledger);
                    gl.Particular = pd.Particular;
                    gl.EId = pd.Payment.Id;
                    gl.EType = 'P';
                    gl.EDate = pd.Payment.PaymentDate;
                    gl.RefNo = pd.Payment.PaymentMode == "Cheque" ? pd.Payment.ChequeNo : pd.Payment.RefNo;
                    gl.EntryNo = pd.Payment.EntryNo;
                    gl.VoucherNo = pd.Payment.EntryNo;
                    gl.DrAmt = pd.Amount;
                    gl.CrAmt = 0;

                    lstBankReconcilation.Add(gl);
                }

                foreach (var p in l.Payments.Where(x => x.Status == "Process" && x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo).ToList())
                {
                    foreach (var pd in p.PaymentDetails)
                    {
                        gl = new BLL.BankReconcilation();
                        gl.Ledger = new BLL.Ledger();
                        gl.Ledger = LedgerDAL_BLL(pd.Ledger);
                        gl.Particular = pd.Particular;
                        gl.EId = p.Id;
                        gl.EType = 'P';
                        gl.EDate = p.PaymentDate;
                        gl.RefNo = p.PaymentMode == "Cheque" ? p.ChequeNo : p.RefNo;
                        gl.EntryNo = p.EntryNo;
                        gl.VoucherNo = p.VoucherNo;
                        gl.DrAmt = 0;
                        gl.CrAmt = pd.Amount;
                        lstBankReconcilation.Add(gl);
                    }
                }

                foreach (var r in l.Receipts.Where(x => x.Status == "Process" && x.ReceiptDate >= dtFrom && x.ReceiptDate <= dtTo).ToList())
                {
                    foreach (var rd in r.ReceiptDetails)
                    {
                        gl = new BLL.BankReconcilation();
                        gl.Ledger = new BLL.Ledger();
                        gl.Ledger = LedgerDAL_BLL(rd.Ledger);
                        gl.Particular = rd.Particulars;
                        gl.EId = r.Id;
                        gl.EType = 'R';
                        gl.EDate = r.ReceiptDate;
                        gl.RefNo = r.ReceiptMode == "Cheque" ? r.ChequeNo : r.RefNo;
                        gl.EntryNo = r.EntryNo;
                        gl.VoucherNo = r.VoucherNo;
                        gl.DrAmt = rd.Amount;
                        gl.CrAmt = 0;
                        lstBankReconcilation.Add(gl);
                    }

                }
                foreach (var rd in l.ReceiptDetails.Where(x => x.Receipt.Status == "Process" && x.Receipt.ReceiptDate >= dtFrom && x.Receipt.ReceiptDate <= dtTo).ToList())
                {
                    gl = new BLL.BankReconcilation();
                    gl.Ledger = new BLL.Ledger();
                    gl.Ledger = LedgerDAL_BLL(rd.Receipt.Ledger);
                    gl.Particular = rd.Particulars;
                    gl.EId = rd.Receipt.Id;
                    gl.EType = 'R';
                    gl.EDate = rd.Receipt.ReceiptDate;
                    gl.RefNo = rd.Receipt.ReceiptMode == "Cheque" ? rd.Receipt.ChequeNo : rd.Receipt.RefNo;
                    gl.EntryNo = rd.Receipt.EntryNo;
                    gl.VoucherNo = rd.Receipt.VoucherNo;
                    gl.DrAmt = 0;
                    gl.CrAmt = rd.Amount;
                    lstBankReconcilation.Add(gl);
                }
            }
            #endregion


            return lstBankReconcilation;
        }

    }
}