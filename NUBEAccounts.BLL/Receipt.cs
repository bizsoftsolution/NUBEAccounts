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
    public class Receipt : INotifyPropertyChanged
    {
        #region Fields
        private long _Id;
        private string _EntryNo;
        private DateTime _ReceiptDate;
        private int _LedgerId;
        private string _ReceiptMode;
        private decimal _Amount;
        private string _RefNo;
        private string _Status;
        private bool _IsShowReturn;
        private bool _IsShowComplete;
        private decimal? _ExtraCharge;
        private string _ChequeNo;
        private DateTime? _ChequeDate;
        private DateTime? _CleareDate;
        private string _Particulars;
        private string _ReceivedFrom;
        private string _VoucherNo;
        private string _LedgerName;
        private string _AmountInwords;


        private Ledger _RLedger;

        private ReceiptDetail _RDetail;


        private string _SearchText;

        private bool _IsShowChequeDetail;
        private bool _IsShowOnlineDetail;
        private bool _IsShowTTDetail;
        private bool _IsLedgerEditable = true;


        private ObservableCollection<ReceiptDetail> _RDetails;
        private static List<string> _ReceiptModeList;
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
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == AppLib.Forms.frmReceipt.ToString()).FirstOrDefault();
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


        public static List<string> ReceiptModeList
        {
            get
            {
                if (_ReceiptModeList == null)
                {
                    _ReceiptModeList = new List<string>();
                    _ReceiptModeList.Add("Cash");
                    _ReceiptModeList.Add("Cheque");
                    _ReceiptModeList.Add("Online");
                    _ReceiptModeList.Add("TT");
                }
                return _ReceiptModeList;
            }
            set
            {
                if (_ReceiptModeList != value)
                {
                    _ReceiptModeList = value;
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
        public DateTime ReceiptDate
        {
            get
            {
                return _ReceiptDate;
            }
            set
            {
                if (_ReceiptDate != value)
                {
                    _ReceiptDate = value;
                    NotifyPropertyChanged(nameof(ReceiptDate));
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
        public string ReceiptMode
        {
            get
            {
                return _ReceiptMode;
            }
            set
            {
                if (_ReceiptMode != value)
                {
                    _ReceiptMode = value;
                    NotifyPropertyChanged(nameof(ReceiptMode));
                    IsShowChequeDetail = value == "Cheque";
                    IsShowOnlineDetail = value == "Online";
                    IsShowTTDetail = value == "TT";
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
        public Nullable<System.DateTime> CleareDate
        {
            get
            {
                return _CleareDate;
            }
            set
            {
                if (_CleareDate != value)
                {
                    _CleareDate = value;
                    NotifyPropertyChanged(nameof(CleareDate));
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
        public string ReceivedFrom
        {
            get
            {
                return _ReceivedFrom;
            }
            set
            {
                if (_ReceivedFrom != value)
                {
                    _ReceivedFrom = value;
                    NotifyPropertyChanged(nameof(_ReceivedFrom));
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


        public ReceiptDetail RDetail
        {
            get
            {
                if (_RDetail == null)
                {
                    _RDetail = new ReceiptDetail();
                }
                return _RDetail;
            }
            set
            {
                if (_RDetail != value)
                {
                    _RDetail = value;
                    NotifyPropertyChanged(nameof(_RDetail));
                }
            }
        }

        public ObservableCollection<ReceiptDetail> RDetails
        {
            get
            {
                if (_RDetails == null) _RDetails = new ObservableCollection<ReceiptDetail>();
                return _RDetails;
            }
            set
            {
                if (_RDetails != value)
                {
                    _RDetails = value;
                    NotifyPropertyChanged(nameof(_RDetails));
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

        public Ledger RLedger
        {
            get
            {
                if (_RLedger == null)
                {
                    _RLedger = new Ledger();
                }
                return _RLedger;
            }
            set
            {
                if (_RLedger != value)
                {
                    _RLedger = value;
                    NotifyPropertyChanged(nameof(Ledger));
                }
            }
        }


        #region List
        public static ObservableCollection<Ledger> LedgerList
        {
            get
            {
                return new ObservableCollection<Ledger>(Ledger.toList.Where(x => x.AccountGroup.GroupName != "Primary" && x.AccountGroup.GroupName != "Cash-in-Hand" && x.AccountGroup.GroupName != "Bank Accounts").ToList());
            }
        }
        public static ObservableCollection<Ledger> CashLedgerList
        {
            get
            {
                return new ObservableCollection<Ledger>(Ledger.toList.Where(x => x.AccountGroup.GroupName == "Cash-in-Hand" || x.AccountGroup.GroupName == "Bank Accounts").ToList());
            }
        }
        #endregion

        #region Master
        public bool Save()
        {
            try
            {
                return NubeAccountClient.NubeAccountHub.Invoke<bool>("Receipt_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Clear()
        {
            new Receipt().toCopy<Receipt>(this);
            ClearDetail();
            _RDetails = new ObservableCollection<ReceiptDetail>();

            ReceiptDate = DateTime.Now;
            IsReadOnly = !UserPermission.AllowInsert;
            VoucherNo = NubeAccountClient.NubeAccountHub.Invoke<string>("Receipt_NewRefNo",ReceiptDate).Result;
            EntryNo = NubeAccountClient.NubeAccountHub.Invoke<string>("Receipt_NewEntryNo").Result;
            NotifyAllPropertyChanged();
        }

        public void SetEntryNo()
        {
            VoucherNo = NubeAccountClient.NubeAccountHub.Invoke<string>("Receipt_NewRefNo", ReceiptDate).Result;
            EntryNo = NubeAccountClient.NubeAccountHub.Invoke<string>("Receipt_NewEntryNo").Result;
        }
        public bool Find()
        {
            try
            {
                Receipt po = NubeAccountClient.NubeAccountHub.Invoke<Receipt>("Receipt_Find", EntryNo).Result;
                if (po.Id == 0) return false;
                po.toCopy<Receipt>(this);
                this.RDetails = po.RDetails;
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
                return NubeAccountClient.NubeAccountHub.Invoke<bool>("Receipt_Delete", this.Id).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Detail

        public void SaveDetail()
        {

            ReceiptDetail pod = RDetails.Where(x => x.LedgerId == RDetail.LedgerId).FirstOrDefault();

            if (pod == null)
            {
                pod = new ReceiptDetail();
                RDetails.Add(pod);
            }

            RDetail.toCopy<ReceiptDetail>(pod);
            ClearDetail();
            Amount = RDetails.Sum(x => x.Amount);
        }

        public void ClearDetail()
        {
            ReceiptDetail pod = new ReceiptDetail();
            pod.toCopy<ReceiptDetail>(RDetail);
        }

        public void DeleteDetail(int LedgerId)
        {
            ReceiptDetail pod = RDetails.Where(x => x.LedgerId == LedgerId).FirstOrDefault();

            if (pod != null)
            {
                RDetails.Remove(pod);
                Amount = RDetails.Sum(x => x.Amount);
            }
        }

        public void FindDetail(int LedgerId)
        {
            ReceiptDetail pod = RDetails.Where(x => x.LedgerId == LedgerId).FirstOrDefault();

            if (pod != null)
            {
                pod.toCopy<ReceiptDetail>(RDetail);
            }
        }

        #endregion

        public bool FindEntryNo()
        {
            var rv = false;
            try
            {
                rv = NubeAccountClient.NubeAccountHub.Invoke<bool>("Find_REntryNo", EntryNo, this).Result;
            }
            catch (Exception ex)
            {
                rv = true;
            }
            return rv;
        }

        public static List<Receipt> ToList(int? LedgerId, DateTime dtFrom, DateTime dtTo, string Status)
        {
            return NubeAccountClient.NubeAccountHub.Invoke<List<Receipt>>("Receipt_List", LedgerId, dtFrom, dtTo, Status).Result;
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
