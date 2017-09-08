using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUBEAccounts.Common;

namespace NUBEAccounts.BLL
{
    public class Payment : INotifyPropertyChanged
    {
        #region Fields
        private long _Id;
        private string _EntryNo;
        private DateTime _PaymentDate;
        private int _LedgerId;
        private string _PaymentMode;
        private decimal _Amount;
        private string _RefNo;
        private string _Status;

        private bool _IsShowReturn;
        private bool _IsShowComplete;
        private bool _IsLedgerEditable = true;


        private decimal? _ExtraCharge;
        private string _ChequeNo;
        private DateTime? _ChequeDate;
        private DateTime? _ClearDate;
        private string _Particulars;
        private string _PayTo;
        private string _VoucherNo;
        private string _LedgerName;
        private string _AmountInwords;


        private Ledger _PLedger;

        private PaymentDetail _PDetail;


        private string _SearchText;

        private bool _IsShowChequeDetail;
        private bool _IsShowOnlineDetail;
        private bool _IsShowTTDetail;

        private ObservableCollection<PaymentDetail> _PDetails;
        private static List<string> _PayModeList;
        private static List<string> _StatusList;

        private static UserTypeDetail _UserPermission;
        private bool _IsReadOnly;
        private bool _IsEnabled;
        private string _RefCode;
        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == AppLib.Forms.frmPayment.ToString()).FirstOrDefault();
                }
                return _UserPermission;
            }

            set
            {
                if (_UserPermission != value)
                {
                    _UserPermission = value;
                }
            }
        }

        public static List<string> PayModeList
        {
            get
            {
                if (_PayModeList == null)
                {
                    _PayModeList = new List<string>();
                    _PayModeList.Add("Cash");
                    _PayModeList.Add("Cheque");
                    _PayModeList.Add("Online");
                    _PayModeList.Add("TT");
                }
                return _PayModeList;
            }
            set
            {
                if (_PayModeList != value)
                {
                    _PayModeList = value;
                }
            }
        }
        public static List<string> StatusList
        {
            get
            {
                if (_StatusList == null)
                {
                    _StatusList = new List<string>();
                    _StatusList.Add("Process");
                    _StatusList.Add("Completed");
                    _StatusList.Add("Returned");
                }
                return _StatusList;
            }
            set
            {
                if (_StatusList != value)
                {
                    _StatusList = value;
                }
            }
        }


        public long Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    NotifyPropertyChanged(nameof(Id));
                }
            }
        }
        public string EntryNo
        {
            get
            {
                return _EntryNo;
            }
            set
            {
                if (_EntryNo != value)
                {
                    _EntryNo = value;
                    NotifyPropertyChanged(nameof(EntryNo));
                }
            }
        }
        public string RefCode
        {
            get
            {
                return _RefCode;
            }
            set
            {
                if (_RefCode != value)
                {
                    _RefCode = value;
                    NotifyPropertyChanged(nameof(RefCode));
                }
            }
        }
        public DateTime PaymentDate
        {
            get
            {
                return _PaymentDate;
            }
            set
            {
                if (_PaymentDate != value)
                {
                    _PaymentDate = value;
                    NotifyPropertyChanged(nameof(PaymentDate));
                }
            }
        }
        public int LedgerId
        {
            get
            {
                return _LedgerId;
            }
            set
            {
                if (_LedgerId != value)
                {
                    _LedgerId = value;

                    NotifyPropertyChanged(nameof(LedgerId));
                }
            }
        }
        public string PaymentMode
        {
            get
            {
                return _PaymentMode;
            }
            set
            {
                if (_PaymentMode != value)
                {
                    _PaymentMode = value;

                    IsShowChequeDetail = value == "Cheque";
                    IsShowOnlineDetail = value == "Online";
                    IsShowTTDetail = value == "TT";





                    NotifyPropertyChanged(nameof(PaymentMode));
                }
            }
        }
        public decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                if (_Amount != value)
                {
                    _Amount = value;
                    AmountInwords = value.ToCurrencyInWords();
                    NotifyPropertyChanged(nameof(Amount));
                }
            }
        }
        public string RefNo
        {
            get
            {
                return _RefNo;
            }
            set
            {
                if (_RefNo != value)
                {
                    _RefNo = value;
                    NotifyPropertyChanged(nameof(RefNo));
                }
            }
        }
        public string Status
        {
            get
            {
                return _Status;
            }
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    IsShowComplete = value == "Completed";
                    IsShowReturn = value == "Returned";
                    NotifyPropertyChanged(nameof(Status));
                }
            }
        }
        public bool IsLedgerEditable
        {
            get
            {
                return _IsLedgerEditable;
            }
            set
            {
                if (_IsLedgerEditable != value)
                {
                    _IsLedgerEditable = value;
                    NotifyPropertyChanged(nameof(IsLedgerEditable));
                }
            }
        }
        public bool IsShowComplete
        {
            get
            {
                return _IsShowComplete;
            }
            set
            {
                if (_IsShowComplete != value)
                {
                    _IsShowComplete = value;
                    NotifyPropertyChanged(nameof(IsShowComplete));
                }
            }
        }
        public bool IsShowReturn
        {
            get
            {
                return _IsShowReturn;
            }
            set
            {
                if (_IsShowReturn != value)
                {
                    _IsShowReturn = value;
                    NotifyPropertyChanged(nameof(IsShowReturn));
                }
            }
        }

        public Nullable<decimal> ExtraCharge
        {
            get
            {
                return _ExtraCharge;
            }
            set
            {
                if (_ExtraCharge != value)
                {
                    _ExtraCharge = value;
                    NotifyPropertyChanged(nameof(ExtraCharge));
                }
            }
        }
        public string ChequeNo
        {
            get
            {
                return _ChequeNo;
            }
            set
            {
                if (_ChequeNo != value)
                {
                    _ChequeNo = value;
                    NotifyPropertyChanged(nameof(ChequeNo));
                }
            }
        }
        public Nullable<System.DateTime> ChequeDate
        {
            get
            {
                return _ChequeDate;
            }
            set
            {
                if (_ChequeDate != value)
                {
                    _ChequeDate = value;
                    NotifyPropertyChanged(nameof(ChequeDate));
                }
            }
        }
        public Nullable<System.DateTime> ClearDate
        {
            get
            {
                return _ClearDate;
            }
            set
            {
                if (_ClearDate != value)
                {
                    _ClearDate = value;
                    NotifyPropertyChanged(nameof(ClearDate));
                }
            }
        }
        public string Particulars
        {
            get
            {
                return _Particulars;
            }
            set
            {
                if (_Particulars != value)
                {
                    _Particulars = value;
                    NotifyPropertyChanged(nameof(Particulars));
                }
            }
        }
        public string PayTo
        {
            get
            {
                return _PayTo;
            }
            set
            {
                if (_PayTo != value)
                {
                    _PayTo = value;
                    NotifyPropertyChanged(nameof(PayTo));
                }
            }
        }
        public string VoucherNo
        {
            get
            {
                return _VoucherNo;
            }
            set
            {
                if (_VoucherNo != value)
                {
                    _VoucherNo = value;
                    NotifyPropertyChanged(nameof(VoucherNo));
                }
            }
        }
        public string LedgerName
        {
            get
            {
                return _LedgerName;
            }
            set
            {
                if (_LedgerName != value)
                {
                    _LedgerName = value;
                    NotifyPropertyChanged(nameof(LedgerName));
                }
            }
        }

        public PaymentDetail PDetail
        {
            get
            {
                if (_PDetail == null)
                {
                    _PDetail = new PaymentDetail();
                }
                return _PDetail;
            }
            set
            {
                if (_PDetail != value)
                {
                    _PDetail = value;
                    NotifyPropertyChanged(nameof(PDetail));
                }
            }
        }

        public ObservableCollection<PaymentDetail> PDetails
        {
            get
            {
                if (_PDetails == null) _PDetails = new ObservableCollection<PaymentDetail>();
                return _PDetails;
            }
            set
            {
                if (_PDetails != value)
                {
                    _PDetails = value;
                    NotifyPropertyChanged(nameof(PDetails));
                }
            }
        }

        public string SearchText
        {
            get
            {
                return _SearchText;
            }
            set
            {
                if (_SearchText != value)
                {
                    _SearchText = value;
                    NotifyPropertyChanged(nameof(SearchText));
                }
            }
        }

        public bool IsShowChequeDetail
        {
            get
            {
                return _IsShowChequeDetail;
            }
            set
            {
                if (_IsShowChequeDetail != value)
                {
                    _IsShowChequeDetail = value;
                    NotifyPropertyChanged(nameof(IsShowChequeDetail));
                }
            }
        }
        public bool IsShowOnlineDetail
        {
            get
            {
                return _IsShowOnlineDetail;
            }
            set
            {
                if (_IsShowOnlineDetail != value)
                {
                    _IsShowOnlineDetail = value;
                    NotifyPropertyChanged(nameof(IsShowOnlineDetail));
                }
            }
        }
        public bool IsShowTTDetail
        {
            get
            {
                return _IsShowTTDetail;
            }
            set
            {
                if (_IsShowTTDetail != value)
                {
                    _IsShowTTDetail = value;
                    NotifyPropertyChanged(nameof(IsShowTTDetail));
                }
            }
        }

        public string AmountInwords
        {
            get
            {
                if (_AmountInwords == null) _AmountInwords = "";
                return _AmountInwords;
            }
            set
            {
                if (_AmountInwords != value)
                {
                    _AmountInwords = value;
                    NotifyPropertyChanged(nameof(AmountInwords));
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }

            set
            {
                if (_IsReadOnly != value)
                {
                    _IsReadOnly = value;
                    IsEnabled = !value;
                    NotifyPropertyChanged(nameof(IsReadOnly));
                }
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }

            set
            {
                if (_IsEnabled != value)
                {
                    _IsEnabled = value;
                    NotifyPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        #endregion

        public Ledger PLedger
        {
            get
            {
                if (_PLedger == null)
                {
                    _PLedger = new Ledger();
                }
                return _PLedger;
            }
            set
            {
                if (_PLedger != value)
                {
                    _PLedger = value;
                    NotifyPropertyChanged(nameof(Ledger));
                }
            }
        }

        #region List
        public static ObservableCollection<Ledger> LedgerList
        {
            get
            {
                try
                {
                    return new ObservableCollection<Ledger>(Ledger.toList.Where(x => x.AccountGroup.GroupName != "Primary" && x.AccountGroup.GroupName != "Cash-in-Hand" && x.AccountGroup.GroupName != "Bank Accounts").ToList());

                }
                catch (Exception ex)
                {

                }
                return new ObservableCollection<Ledger>(Ledger.toList.ToList());
            }


        }


        #endregion

        #region Master
        public bool Save()
        {
            try
            {
                return NubeAccountClient.NubeAccountHub.Invoke<bool>("Payment_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Clear()
        {
            new Payment().toCopy<Payment>(this);
            ClearDetail();
            _PDetails = new ObservableCollection<PaymentDetail>();

            PaymentDate = DateTime.Now;
            IsReadOnly = !UserPermission.AllowInsert;
            var r = NubeAccountClient.NubeAccountHub.Invoke<string>("Payment_NewRefNo",PaymentDate).Result;
            VoucherNo = r;
            var e = NubeAccountClient.NubeAccountHub.Invoke<string>("Payment_NewEntryNo").Result;
            VoucherNo = r; NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                Payment po = NubeAccountClient.NubeAccountHub.Invoke<Payment>("Payment_Find", EntryNo).Result;
                if (po.Id == 0) return false;
                po.toCopy<Payment>(this);
                this.PDetails = po.PDetails;
                IsReadOnly = !UserPermission.AllowInsert;

                NotifyAllPropertyChanged();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete()
        {
            try
            {
                return NubeAccountClient.NubeAccountHub.Invoke<bool>("Payment_Delete", this.Id).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    
        public void SetEntryNo()
        {
            VoucherNo = NubeAccountClient.NubeAccountHub.Invoke<string>("Payment_NewRefNo",PaymentDate).Result;
            EntryNo = NubeAccountClient.NubeAccountHub.Invoke<string>("Payment_NewEntryNo").Result;
        }
        #endregion

        #region Detail

        public void SaveDetail()
        {

            PaymentDetail pod = PDetails.Where(x => x.LedgerId == PDetail.LedgerId).FirstOrDefault();

            if (pod == null)
            {
                pod = new PaymentDetail();
                PDetails.Add(pod);
            }

            PDetail.toCopy<PaymentDetail>(pod);
            ClearDetail();
            Amount = PDetails.Sum(x => x.Amount);
        }

        public void ClearDetail()
        {
            PaymentDetail pod = new PaymentDetail();
            pod.toCopy<PaymentDetail>(PDetail);
        }

        public void DeleteDetail(int LedgerId)
        {
            PaymentDetail pod = PDetails.Where(x => x.LedgerId == LedgerId).FirstOrDefault();

            if (pod != null)
            {
                PDetails.Remove(pod);
                Amount = PDetails.Sum(x => x.Amount);
            }
        }

        public void FindDetail(int LedgerId)
        {
            PaymentDetail pod = PDetails.Where(x => x.LedgerId == LedgerId).FirstOrDefault();

            if (pod != null)
            {
                pod.toCopy<PaymentDetail>(PDetail);
            }
        }

        #endregion



        public bool FindEntryNo()
        {
            var rv = false;
            try
            {
                rv = NubeAccountClient.NubeAccountHub.Invoke<bool>("Find_EntryNo", EntryNo, this).Result;
            }
            catch (Exception ex)
            {
                rv = true;
            }
            return rv;
        }
        public static List<Payment> ToList(int? LedgerId, DateTime dtFrom, DateTime dtTo, string Status)
        {
            return NubeAccountClient.NubeAccountHub.Invoke<List<Payment>>("Payment_List", LedgerId, dtFrom, dtTo, Status).Result;
        }


        #region Property  Changed Event

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }


        private void NotifyAllPropertyChanged()
        {
            foreach (var p in this.GetType().GetProperties()) NotifyPropertyChanged(p.Name);
        }

        #endregion
    }
}
