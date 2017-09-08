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
    public class Journal : INotifyPropertyChanged
    {
        #region Fields
        private long _Id;
        private string _EntryNo;
        private DateTime _JournalDate;
        private decimal _Amount;
        private string _Particular;
        private string _HQNo;
        private string _VoucherNo;
        private string _Status;

        private string _AmountInwords;
        private JournalDetail _JDetail;

        private string _SearchText;

        private ObservableCollection<JournalDetail> _JDetails;
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
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == AppLib.Forms.frmJournal.ToString()).FirstOrDefault();
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
        public DateTime JournalDate
        {
            get
            {
                return _JournalDate;
            }
            set
            {
                if (_JournalDate != value)
                {
                    _JournalDate = value;
                    NotifyPropertyChanged(nameof(JournalDate));
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
                    NotifyPropertyChanged(nameof(Amount));
                }
            }
        }
        public string Particular
        {
            get
            {
                return _Particular;
            }
            set
            {
                if (_Particular != value)
                {
                    _Particular = value;
                    NotifyPropertyChanged(nameof(Particular));
                }
            }
        }
        public string HQNo
        {
            get
            {
                return _HQNo;
            }
            set
            {
                if (_HQNo != value)
                {
                    _HQNo = value;
                    NotifyPropertyChanged(nameof(HQNo));
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
                    NotifyPropertyChanged(nameof(Status));
                }
            }
        }

        public JournalDetail JDetail
        {
            get
            {
                if (_JDetail == null)
                {
                    _JDetail = new JournalDetail();
                }
                return _JDetail;
            }
            set
            {
                if (_JDetail != value)
                {
                    _JDetail = value;
                    NotifyPropertyChanged(nameof(JDetail));
                }
            }
        }

        public ObservableCollection<JournalDetail> JDetails
        {
            get
            {
                if (_JDetails == null) _JDetails = new ObservableCollection<JournalDetail>();
                return _JDetails;
            }
            set
            {
                if (_JDetails != value)
                {
                    _JDetails = value;
                    NotifyPropertyChanged(nameof(JDetails));
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


        #region List
        public static ObservableCollection<Ledger> LedgerList
        {
            get
            {
                return Ledger.toList;
            }
        }
        #endregion
        #endregion

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

        #region Master
        public bool Save()
        {
            try
            {
                return NubeAccountClient.NubeAccountHub.Invoke<bool>("Journal_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Clear()
        {
            new Journal().toCopy<Journal>(this);
            ClearDetail();
            _JDetails = new ObservableCollection<JournalDetail>();

            JournalDate = DateTime.Now;
            IsReadOnly = !UserPermission.AllowInsert;
            var v = NubeAccountClient.NubeAccountHub.Invoke<string>("Journal_NewRefNo", JournalDate).Result;
            VoucherNo = v;
            var e = NubeAccountClient.NubeAccountHub.Invoke<string>("Journal_NewEntryNo").Result;
            EntryNo = e;

            NotifyAllPropertyChanged();
        }
        public void SetEntryNo()
        {
            VoucherNo = NubeAccountClient.NubeAccountHub.Invoke<string>("Journal_NewRefNo", JournalDate).Result;
            EntryNo = NubeAccountClient.NubeAccountHub.Invoke<string>("Journal_NewEntryNo").Result;
        }
        public bool Find()
        {
            try
            {
                Journal po = NubeAccountClient.NubeAccountHub.Invoke<Journal>("Journal_Find", EntryNo).Result;
                if (po.Id == 0) return false;
                po.toCopy<Journal>(this);
                this.JDetails = po.JDetails;
                IsReadOnly = !UserPermission.AllowInsert;

                NotifyAllPropertyChanged();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool FindById(int Id)
        {
            try
            {
                Journal po = NubeAccountClient.NubeAccountHub.Invoke<Journal>("Journal_FindById", Id).Result;
                if (po.Id == 0) return false;
                po.toCopy<Journal>(this);
                this.JDetails = po.JDetails;
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
                return NubeAccountClient.NubeAccountHub.Invoke<bool>("Journal_Delete", this.Id).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool FindEntryNo()
        {
            try
            {
                return NubeAccountClient.NubeAccountHub.Invoke<bool>("FindEntryNo", VoucherNo, this).Result;
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

            JournalDetail pod = JDetails.Where(x => x.LedgerId == JDetail.LedgerId).FirstOrDefault();

            if (pod == null)
            {
                pod = new JournalDetail();
                JDetails.Add(pod);
            }

            JDetail.toCopy<JournalDetail>(pod);
            ClearDetail();
            Amount = JDetails.Sum(x => x.DrAmt) - JDetails.Sum(x => x.CrAmt);
        }

        public void ClearDetail()
        {
            JournalDetail pod = new JournalDetail();
            pod.toCopy<JournalDetail>(JDetail);
        }

        public void DeleteDetail(int LedgerId)
        {
            JournalDetail pod = JDetails.Where(x => x.LedgerId == LedgerId).FirstOrDefault();

            if (pod != null)
            {
                JDetails.Remove(pod);
                Amount = JDetails.Sum(x => x.DrAmt) - JDetails.Sum(x => x.CrAmt);
            }
        }

        public void FindDetail(int LedgerId)
        {
            JournalDetail pod = JDetails.Where(x => x.LedgerId == LedgerId).FirstOrDefault();

            if (pod != null)
            {
                pod.toCopy<JournalDetail>(JDetail);
            }
        }
        public static List<Journal> ToList(int? LedgerId, DateTime dtFrom, DateTime dtTo)
        {
            return NubeAccountClient.NubeAccountHub.Invoke<List<Journal>>("Journal_List", LedgerId, dtFrom, dtTo).Result;
        }
        #endregion
    }
}
