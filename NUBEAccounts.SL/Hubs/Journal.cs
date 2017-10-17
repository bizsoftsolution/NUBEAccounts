using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUBEAccounts.Common;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {
        #region Journal        
        public string Journal_NewRefNo(DateTime dt)
        {
            return Journal_NewRefNoByFund(Caller.FundMasterId, dt);
        }

        public string Journal_NewRefNoByFund(int FundMasterId, DateTime dt)
        {
            string Prefix = string.Format("{0}/{1:X}/", BLL.FormPrefix.Journal, dt.Month);
            long No = 0;

            var d1 = DB.Journals.Where(x => x.JournalDetails.FirstOrDefault().Ledger.AccountGroup.FundMasterId == Caller.FundMasterId && x.VoucherNo.StartsWith(Prefix) && x.JournalDate.Year == dt.Year).Select(x => x.VoucherNo).ToList();
            if (d1.Count() > 0)
            {
                No = d1.Select(x => Convert.ToInt64(x.Substring(Prefix.Length), 10)).Max();
            }

            return string.Format("{0}{1}", Prefix, No + 1);
        }
        public string Journal_NewEntryNo()
        {
            return Journal_NewEntryNoByFund(Caller.FundMasterId);
        }

        public string Journal_NewEntryNoByFund(int FundMasterId)
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.Journal, dt, dt.Month);
            long No = 0;

            var d = DB.Journals.Where(x => x.JournalDetails.FirstOrDefault().Ledger.AccountGroup.FundMasterId == FundMasterId && x.EntryNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.EntryNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.EntryNo.Substring(Prefix.Length), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }
        public bool Journal_Save(BLL.Journal PO)
        {
            try
            {

                DAL.Journal d = DB.Journals.Where(x => x.Id == PO.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.Journal();
                    DB.Journals.Add(d);

                    PO.toCopy<DAL.Journal>(d);

                    foreach (var b_pod in PO.JDetails)
                    {
                        DAL.JournalDetail d_pod = new DAL.JournalDetail();
                        b_pod.toCopy<DAL.JournalDetail>(d_pod);
                        d.JournalDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    PO.Id = d.Id;
                    LogDetailStore(PO, LogDetailType.INSERT);
                }
                else
                {

                    //foreach (var d_SOd in d.JournalDetails)
                    //{
                    //    BLL.JournalDetail b_SOd = PO.JDetails.Where(x => x.Id == d_SOd.Id).FirstOrDefault();
                    //    if (b_SOd == null) d.JournalDetails.Remove(d_SOd);
                    //}
                    decimal rd = PO.JDetails.Select(X => X.JournalId).FirstOrDefault();
                    DB.JournalDetails.RemoveRange(d.JournalDetails.Where(x => x.JournalId == rd).ToList());

                    PO.toCopy<DAL.Journal>(d);

                    foreach (var b_pod in PO.JDetails)
                    {
                        DAL.JournalDetail d_pod = d.JournalDetails.Where(x => x.Id == b_pod.Id).FirstOrDefault();
                        if (d_pod == null)
                        {
                            d_pod = new DAL.JournalDetail();
                            d.JournalDetails.Add(d_pod);
                        }
                        b_pod.toCopy<DAL.JournalDetail>(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(PO, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Journal_RefNoRefresh(Journal_NewRefNo(DateTime.Now));
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.Journal Journal_Find(string SearchText)
        {
            BLL.Journal PO = new BLL.Journal();
            try
            {

                DAL.Journal d = DB.Journals.Where(x => x.EntryNo == SearchText && x.JournalDetails.FirstOrDefault().Ledger.AccountGroup.FundMasterId == Caller.FundMasterId).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Journal>(PO);
                    foreach (var d_pod in d.JournalDetails)
                    {
                        BLL.JournalDetail b_pod = new BLL.JournalDetail();
                        d_pod.toCopy<BLL.JournalDetail>(b_pod);
                        PO.JDetails.Add(b_pod);
                        b_pod.LedgerName = (d_pod.Ledger ?? DB.Ledgers.Find(d_pod.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    }

                }
            }
            catch (Exception ex) { }
            return PO;
        }
        public BLL.Journal Journal_FindById(int id)
        {
            BLL.Journal PO = new BLL.Journal();
            try
            {

                DAL.Journal d = DB.Journals.Where(x => x.Id == id && x.JournalDetails.FirstOrDefault().Ledger.AccountGroup.FundMasterId == Caller.FundMasterId).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Journal>(PO);
                    foreach (var d_pod in d.JournalDetails)
                    {
                        BLL.JournalDetail b_pod = new BLL.JournalDetail();
                        d_pod.toCopy<BLL.JournalDetail>(b_pod);
                        PO.JDetails.Add(b_pod);
                        b_pod.LedgerName = (d_pod.Ledger ?? DB.Ledgers.Find(d_pod.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    }

                }
            }
            catch (Exception ex) { }
            return PO;
        }

        public bool Journal_Delete(long pk)
        {
            try
            {
                DAL.Journal d = DB.Journals.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    DB.JournalDetails.RemoveRange(d.JournalDetails);
                    DB.Journals.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(Journal_DALtoBLL(d), LogDetailType.DELETE);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.Journal Journal_DALtoBLL(DAL.Journal d)
        {
            BLL.Journal J = d.toCopy<BLL.Journal>(new BLL.Journal());
            foreach (var d_Jd in d.JournalDetails)
            {
                J.JDetails.Add(d_Jd.toCopy<BLL.JournalDetail>(new BLL.JournalDetail()));
            }
            return J;
        }

        public bool Find_JEntryNo(string entryNo, BLL.Payment PO)

        {
            DAL.Journal d = DB.Journals.Where(x => x.EntryNo == entryNo & x.Id != PO.Id && x.JournalDetails.FirstOrDefault().Ledger.AccountGroup.FundMasterId == Caller.FundMasterId).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public List<BLL.Journal> Journal_List(int? LedgerId, DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.Journal> lstJournal = new List<BLL.Journal>();
            BLL.Journal rp = new BLL.Journal();

         
                foreach (var l1 in DB.JournalDetails.Where(x => x.Journal.JournalDate >= dtFrom && x.Journal.JournalDate <= dtTo &&( LedgerId == null||x.LedgerId == LedgerId )&&x.Ledger.AccountGroup.FundMasterId==Caller.FundMasterId).ToList())
                {

                    rp = new BLL.Journal();
                    rp.JDetail.CrAmt = l1.CrAmt;
                    rp.JDetail.DrAmt = l1.DrAmt;
                    rp.EntryNo = l1.Journal.EntryNo;
                    rp.Id = l1.Journal.Id;
                    rp.JDetail.LedgerId = l1.LedgerId;
                    rp.JDetail.LedgerName = l1.Ledger.LedgerName;
                    rp.Particular = l1.Journal.Particular;
                    rp.JournalDate = l1.Journal.JournalDate;
                
                    rp.RefCode = l1.Journal.RefCode;
                    rp.VoucherNo = l1.Journal.VoucherNo;
                lstJournal.Add(rp);
            }

               

            

            return lstJournal;

        }

        int LedgerIdByKey(string key)
        {
            return LedgerIdByKeyAndFund(key, Caller.FundMasterId);
        }

        int LedgerIdByKeyAndFund(string key, int FundMasterId)
        {
            return DB.DataKeyValues.Where(x => x.FundMasterId == FundMasterId && x.DataKey == key).FirstOrDefault().DataValue;
        }
        int LedgerIdByFund(string LName, int FundMasterId)
        {
            var l = DB.Ledgers.Where(x => x.LedgerName == LName && x.AccountGroup.FundMasterId == FundMasterId).FirstOrDefault();
            return l == null ? 0 : l.Id;
        }
        int FundMasterIdByLedgerName(string LedgerName)
        {
            var FName = LedgerName.Substring(3);
            var fm = DB.FundMasters.Where(x => x.FundName == FName).FirstOrDefault();
            return fm == null ? 0 : fm.Id;
        }
        string LedgerNameByFund(int FundMasterId)
        {
            var cm = DB.FundMasters.Where(x => x.Id == FundMasterId).FirstOrDefault();
            return string.Format("CM-{0}",  cm.FundName);
        }

        #region Payment
        void Journal_SaveByPayment(BLL.Payment P)
        {
            var EntryNo = string.Format("PMT-{0}", P.Id);

            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j == null)
            {
                var pd = P.PDetails.FirstOrDefault();
                var ld = DB.Ledgers.Where(x => x.Id == pd.LedgerId).FirstOrDefault();

                if (ld.LedgerName.StartsWith("CM-") || ld.LedgerName.StartsWith("WH-") || ld.LedgerName.StartsWith("DL-"))
                {
                    j = new DAL.Journal();
                    j.EntryNo = Journal_NewRefNo(DateTime.Now);
                    j.JournalDate = P.PaymentDate;

                    var CId = FundMasterIdByLedgerName(ld.LedgerName);
                    if (CId != 0)
                    {
                        var LName = LedgerNameByFund(Caller.FundMasterId);

                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = LedgerIdByKeyAndFund(BLL.DataKeyValue.CashLedger_Key, CId),
                            DrAmt = P.Amount,
                            Particulars = P.Particulars
                        });
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {

                            LedgerId = LedgerIdByFund(LName, CId),
                            CrAmt = P.Amount,
                            Particulars = P.Particulars
                        });
                        DB.Journals.Add(j);
                        DB.SaveChanges();
                    }


                }
            }
            else
            {
                j.JournalDate = P.PaymentDate;
                foreach (var jd in j.JournalDetails)
                {
                    if (jd.CrAmt != 0) jd.CrAmt = P.Amount;
                    if (jd.DrAmt != 0) jd.DrAmt = P.Amount;
                    jd.Particulars = P.Particulars;
                }
                DB.SaveChanges();
            }

        }
        void Journal_DeleteByPayment(BLL.Payment P)
        {
            var EntryNo = string.Format("PMT-{0}", P.Id);
            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }

        #endregion

        #region Receipt
        void Journal_SaveByReceipt(BLL.Receipt R)
        {
            var EntryNo = string.Format("RPT-{0}", R.Id);

            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j == null)
            {
                var pd = R.RDetails.FirstOrDefault();
                var ld = DB.Ledgers.Where(x => x.Id == pd.LedgerId).FirstOrDefault();

                if (ld.LedgerName.StartsWith("CM-") || ld.LedgerName.StartsWith("WH-") || ld.LedgerName.StartsWith("DL-"))
                {
                    j = new DAL.Journal();
                    j.EntryNo = R.EntryNo;
                    j.JournalDate = R.ReceiptDate;

                    var CId = FundMasterIdByLedgerName(ld.LedgerName);
                    if (CId != 0)
                    {
                        var LName = LedgerNameByFund(Caller.FundMasterId);

                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {
                            LedgerId = LedgerIdByKeyAndFund(BLL.DataKeyValue.CashLedger_Key, CId),
                            DrAmt = R.Amount,
                            Particulars = R.Particulars
                        });
                        j.JournalDetails.Add(new DAL.JournalDetail()
                        {

                            LedgerId = LedgerIdByFund(LName, CId),
                            CrAmt = R.Amount,
                            Particulars = R.Particulars
                        });
                        DB.Journals.Add(j);
                        DB.SaveChanges();
                    }


                }
            }
            else
            {

                j.JournalDate = R.ReceiptDate;
                foreach (var jd in j.JournalDetails)
                {
                    if (jd.CrAmt != 0) jd.CrAmt = R.Amount;
                    if (jd.DrAmt != 0) jd.DrAmt = R.Amount;
                    jd.Particulars = R.Particulars;
                }
                DB.SaveChanges();
            }

        }
        void Journal_DeleteByReceipt(BLL.Receipt P)
        {
            var EntryNo = string.Format("RPT-{0}", P.Id);
            DAL.Journal j = DB.Journals.Where(x => x.EntryNo == EntryNo).FirstOrDefault();
            if (j != null) Journal_Delete(j.Id);
        }
        #endregion


        #endregion
    }
}