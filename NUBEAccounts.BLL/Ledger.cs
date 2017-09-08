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
      
        private static List<string> _ACTypeList;

        private int _Id;
        private string _LedgerName;
        private AccountGroup _AccountGroup;
        private int? _AccountGroupId;
        private string _PersonIncharge;
        private string _AddressLine1;
        private string _AddressLine2;
        private string _cityName;
        private string _TelephoneNo;
        private string _MobileNo;
        private string _EMailId;
        private string _GSTNo;
        private short _CreditLimit;
        private double _CreditAmount;
        private int? _CreditLimitTypeId;
        //private CreditLimitType _CreditLimitType;
        private string _CreditLimitTypeName;

        private decimal? _OPDr;
        private decimal? _OPCr;
        private string _LedgerCode;

        private string _GroupCode;
        private string _AccountName;
        private string _ACType;
        private decimal? _OPBal;

        private static UserTypeDetail _UserPermission;
        private bool _IsReadOnly;
        private bool _IsEnabled;

        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == AppLib.Forms.frmLedger.ToString()).FirstOrDefault();
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
       


        public static List<string> ACTypeList
        {
            get
            {
                if (_ACTypeList == null)
                {
                    _ACTypeList = new List<string>();
                    _ACTypeList.Add("Debit");
                    _ACTypeList.Add("Credit");
                }
                return _ACTypeList;
            }
            set
            {
                if (_ACTypeList != value)
                {
                    _ACTypeList = value;
                }
            }
        }
        public string GroupCode
        {
            get
            {
                return _GroupCode;
            }

            set
            {
                if (_GroupCode != value)
                {
                    _GroupCode = value;
                    NotifyPropertyChanged(nameof(GroupCode));
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
        public string ACType
        {
            get
            {
                return
                    _ACType;
            }

            set
            {
                if (_ACType != value)
                {
                    _ACType = value;
                    OPDr = value == "Debit" ? OPBal : 0;
                    OPCr = value == "Credit" ? OPBal : 0;
                    NotifyPropertyChanged(nameof(ACType));
                }
            }
        }
        public decimal? OPBal
        {
            get
            {
                return _OPBal;
            }
            set
            {
                if (_OPBal != value)
                {
                    _OPBal = value;
                    NotifyPropertyChanged(nameof(OPBal));
                    OPDr = ACType == "Debit" ? OPBal : 0;
                    OPCr = ACType == "Credit" ? OPBal : 0;
                }
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
        public string PersonIncharge
        {
            get
            {
                return
                    _PersonIncharge;
            }

            set
            {
                if (_PersonIncharge != value)
                {
                    _PersonIncharge = value;
                    NotifyPropertyChanged(nameof(PersonIncharge));
                }
            }
        }
        public string AddressLine1
        {
            get
            {
                return
                    _AddressLine1;
            }

            set
            {
                if (_AddressLine1 != value)
                {
                    _AddressLine1 = value;
                    NotifyPropertyChanged(nameof(AddressLine1));
                }
            }
        }
        public string AddressLine2
        {
            get
            {
                return _AddressLine2;
            }

            set
            {
                if (_AddressLine2 != value)
                {
                    _AddressLine2 = value;
                    NotifyPropertyChanged(nameof(AddressLine2));
                }
            }
        }
        public string CityName
        {
            get
            {
                return _cityName;
            }
            set
            {
                if (_cityName != value)
                {
                    _cityName = value;
                    NotifyPropertyChanged(nameof(CityName));
                }
            }
        }
        public string TelephoneNo
        {
            get
            {
                return _TelephoneNo;
            }

            set
            {
                if (_TelephoneNo != value)
                {
                    _TelephoneNo = value;
                    NotifyPropertyChanged(nameof(TelephoneNo));
                }
            }
        }
        public string MobileNo
        {
            get
            {
                return _MobileNo;
            }

            set
            {
                if (_MobileNo != value)
                {
                    _MobileNo = value;
                    NotifyPropertyChanged(nameof(MobileNo));
                }
            }
        }
        public string GSTNo
        {
            get
            {
                return _GSTNo;
            }

            set
            {
                if (_GSTNo != value)
                {
                    _GSTNo = value;
                    NotifyPropertyChanged(nameof(GSTNo));
                }
            }
        }
        public string EMailId
        {
            get
            {
                return _EMailId;
            }

            set
            {
                if (_EMailId != value)
                {
                    _EMailId = value;
                    NotifyPropertyChanged(nameof(EMailId));
                }
            }
        }
        public double CreditAmount
        {
            get
            {
                return _CreditAmount;
            }

            set
            {
                if (_CreditAmount != value)
                {
                    _CreditAmount = value;
                    NotifyPropertyChanged(nameof(CreditAmount));
                }
            }
        }
        public short CreditLimit
        {
            get
            {
                return _CreditLimit;
            }

            set
            {
                if (_CreditLimit != value)
                {
                    _CreditLimit = value;
                    NotifyPropertyChanged(nameof(CreditLimit));
                }
            }
        }
        //public CreditLimitType CreditLimitType
        //{
        //    get
        //    {
        //        return _CreditLimitType;
        //    }
        //    set
        //    {
        //        if (_CreditLimitType != value)
        //        {
        //            _CreditLimitType = value;
        //            NotifyPropertyChanged(nameof(BLL.CreditLimitType));
        //        }
        //    }
        //}
        public int? CreditLimitTypeId
        {
            get
            {
                return _CreditLimitTypeId;
            }
            set
            {
                if (_CreditLimitTypeId != value)
                {
                    _CreditLimitTypeId = value;
                    NotifyPropertyChanged(nameof(CreditLimitTypeId));
                }
            }
        }
        public string CreditLimitTypeName
        {
            get
            {
                return _CreditLimitTypeName;
            }
            set
            {
                if (_CreditLimitTypeName != value)
                {
                    _CreditLimitTypeName = value;
                    NotifyPropertyChanged(nameof(CreditLimitTypeName));
                }
            }
        }
        public decimal? OPCr
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
                    if (value != null && value != 0)
                    {
                        OPDr = 0;
                        OPBal = value;
                        ACType = "Credit";
                    }
                }

            }
        }
        public decimal? OPDr
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
                    if (value != null && value != 0)
                    {
                        OPCr = 0;
                        OPBal = value;
                        ACType = "Debit";
                    }
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
                    var i = NubeAccountClient.NubeAccountHub.Invoke<int>("Ledger_Save", this).Result;
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
                    rv = NubeAccountClient.NubeAccountHub.Invoke<bool>("Ledger_Delete", this.Id).Result;
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
                //AccountName = string.Format("{0}{1}{2}{3}{4}", AccountGroup.GroupCode, string.IsNullOrWhiteSpace(AccountGroup.GroupCode) ? "" : "-", LedgerCode, string.IsNullOrWhiteSpace(LedgerCode) ? "" : "-", LedgerName);
                AccountName = string.Format("{0}{1}{2}", LedgerCode, string.IsNullOrWhiteSpace(LedgerCode) ? "" : "-", LedgerName);
            }
            catch (Exception ex)
            {

            }
        }
        public void SetLedger()
        {
            try
            {
                
                NubeAccountClient.NubeAccountHub.Invoke<int>("Existing_Ledger");
            }
            catch (Exception ex)
            {

            }
        }


        #endregion


    }
}
