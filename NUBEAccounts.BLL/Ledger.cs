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
    public class Ledger : INotifyPropertyChanged
    {
        #region Fileds

        private static ObservableCollection<Ledger> _toList;     

        private int _Id;
        private string _LedgerName;
        private string _LedgerCode;
        private int? _AccountGroupId;

        private string _AccountName;
        private AccountGroup _AccountGroup;

        decimal _OPDr;
        decimal _OPCr;
        decimal _DrAmt;
        decimal _CrAmt;

        private static UserTypeDetail _UserPermission;
        private bool _IsReadOnly;
        private bool _IsEnabled;

        #endregion

        #region Property
        
        public static ObservableCollection<Ledger> toList
        {
            get
            {
                if (_toList == null) _toList = new ObservableCollection<Ledger>(NubeAccountClient.NubeAccountHub.Invoke<List<Ledger>>("Ledger_List").Result);
                return _toList;
            }
            set
            {
                _toList = value;
            }
        }
       
       
        public int Id
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
                    SetAccountName();
                }
            }
        }
        public string LedgerCode
        {
            get
            {
                return _LedgerCode;
            }
            set
            {
                if (_LedgerCode != value)
                {
                    _LedgerCode = value;
                    NotifyPropertyChanged(nameof(LedgerCode));
                    SetAccountName();
                }
            }
        }
        public int? AccountGroupId
        {
            get
            {
                return _AccountGroupId;
            }

            set
            {
                if (_AccountGroupId != value)
                {
                    _AccountGroupId = value;
                    NotifyPropertyChanged(nameof(AccountGroupId));
                }
            }
        }

        public decimal OPDr
        {
            get
            {
                return _OPDr;
            }

            set
            {
                if (_OPDr != value)
                {
                    _OPDr = value;
                    NotifyPropertyChanged(nameof(OPDr));
                }
            }
        }

        public decimal OPCr
        {
            get
            {
                return _OPCr;
            }

            set
            {
                if (_OPCr != value)
                {
                    _OPCr = value;
                    NotifyPropertyChanged(nameof(OPCr));
                }
            }
        }
        
        public decimal DrAmt
        {
            get
            {
                return _DrAmt;
            }

            set
            {
                if (_DrAmt != value)
                {
                    _DrAmt = value;
                    NotifyPropertyChanged(nameof(DrAmt));
                }
            }
        }

        public decimal CrAmt
        {
            get
            {
                return _CrAmt;
            }

            set
            {
                if (_CrAmt != value)
                {
                    _CrAmt = value;
                    NotifyPropertyChanged(nameof(CrAmt));
                }
            }
        }

        public AccountGroup AccountGroup
        {
            get
            {
                return _AccountGroup;
            }
            set
            {
                if (_AccountGroup != value)
                {
                    _AccountGroup = value;
                    NotifyPropertyChanged(nameof(AccountGroup));
                    SetAccountName();
                }

            }
        }

        public string AccountName
        {
            get
            {
                return _AccountName;
            }

            set
            {
                if (_AccountName != value)
                {
                    _AccountName = value;
                    NotifyPropertyChanged(nameof(AccountName));
                }
            }
        }
        
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmLedger).FirstOrDefault();
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
                    NotifyPropertyChanged(nameof(IsReadOnly));
                }
                IsEnabled = !value;
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

        #region Methods

        public bool Save(bool isServerCall = false)
        {
            if (!isValid()) return false;
            try
            {

                Ledger d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new Ledger();
                    toList.Add(d);
                }

                this.toCopy<Ledger>(d);
                if (isServerCall == false)
                {
                    var i = NubeAccountClient.NubeAccountHub.Invoke<int>(Message.SL.Ledger_Save, this).Result;
                    d.Id = i;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }

        public void Clear()
        {
            new Ledger().toCopy<Ledger>(this);
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<Ledger>(this);
                IsReadOnly = !UserPermission.AllowUpdate;

                return true;
            }

            return false;
        }

        public bool Delete(bool isServerCall = false)
        {
            var rv = false;
            var d = toList.Where(x => x.Id == Id).FirstOrDefault();
            if (d != null)
            {

                if (isServerCall == false)
                {
                    rv = NubeAccountClient.NubeAccountHub.Invoke<bool>(Message.SL.Ledger_Delete, this.Id).Result;
                    if (rv == true) toList.Remove(d);

                }
                return rv;
            }

            return rv;
        }

        public bool isValid()
        {
            bool RValue = true;
            if (toList.Where(x => x.LedgerName.ToLower() == LedgerName.ToLower() && x.Id != Id).Count() > 0)
            {
                RValue = false;
            }
            return RValue;

        }

        public static void Init()
        {
            _toList = null;
          
        }

        private void SetAccountName()
        {
            try
            {
                AccountName = string.Format("{0}{1}{2}", LedgerCode, string.IsNullOrWhiteSpace(LedgerCode) ? "" : "-", LedgerName);
            }
            catch (Exception ex)
            {

            }
        }
        
        #endregion


    }
}
