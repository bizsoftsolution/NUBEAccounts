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
    public class CustomFormat : INotifyPropertyChanged
    {
        #region Fileds

        private static ObservableCollection<CustomFormat> _toList;

        private int _Id;
        private string _CurrencyPositiveSymbolPrefix;
        private string _CurrencyPositiveSymbolSuffix;
        private string _CurrencyNegativeSymbolPrefix;
        private string _CurrencyNegativeSymbolSuffix;
        private string _CurrencyToWordPrefix;

        private string _CurrencyToWordSuffix;
        private string _DecimalToWordPrefix;
        private string _DecimalToWordSuffix;
        private string _DecimalSymbol;
        private string _DigitGroupingSymbol;
        private int _DigitGroupingBy;

        private int _NoOfDigitAfterDecimal;
        private int _CurrencyCaseSensitive;
        private bool _IsDisplayWithOnlyOnSuffix;

        private int _CompanyId;

        private decimal? _SampleCurrency;
        private decimal? _SampleCurrencyPositive;
        private decimal? _SampleCurrencyNegative;

        private CompanyDetail _Company;

        private static UserTypeDetail _UserPermission;
        private bool _IsReadOnly;
        private CustomFormat d;
        private bool _IsEnabled;


        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmCustomFormat).FirstOrDefault();
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
                _IsEnabled = !value;
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
                    NotifyPropertyChanged(nameof(IsReadOnly));
                }

            }
        }
        public static ObservableCollection<CustomFormat> toList
        {
            get
            {
                if (_toList == null) _toList = new ObservableCollection<CustomFormat>(NubeAccountClient.NubeAccountHub.Invoke<List<CustomFormat>>("CustomFormat_List").Result);
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
        public string CurrencyPositiveSymbolPrefix
        {
            get
            {
                return _CurrencyPositiveSymbolPrefix;
            }

            set
            {
                if (_CurrencyPositiveSymbolPrefix != value)
                {
                    _CurrencyPositiveSymbolPrefix = value;
                    AppLib.CurrencyPositiveSymbolPrefix = value;
                    NotifyPropertyChanged(nameof(CurrencyPositiveSymbolPrefix));
                    NotifyAllPropertyChanged();

                }
            }
        }
        public string CurrencyPositiveSymbolSuffix
        {
            get
            {
                return _CurrencyPositiveSymbolSuffix;
            }

            set
            {
                if (_CurrencyPositiveSymbolSuffix != value)
                {
                    _CurrencyPositiveSymbolSuffix = value;
                    Common.AppLib.CurrencyPositiveSymbolSuffix = value;
                    NotifyPropertyChanged(nameof(CurrencyPositiveSymbolSuffix));
                    NotifyAllPropertyChanged();
                }
            }
        }
        public string CurrencyNegativeSymbolPrefix
        {
            get
            {
                return _CurrencyNegativeSymbolPrefix;
            }

            set
            {
                if (_CurrencyNegativeSymbolPrefix != value)
                {
                    _CurrencyNegativeSymbolPrefix = value;
                    Common.AppLib.CurrencyNegativeSymbolPrefix = value;
                    NotifyPropertyChanged(nameof(CurrencyNegativeSymbolPrefix));
                    NotifyAllPropertyChanged();
                }
            }
        }
        public string CurrencyNegativeSymbolSuffix
        {
            get
            {
                return _CurrencyNegativeSymbolSuffix;
            }

            set
            {
                if (_CurrencyNegativeSymbolSuffix != value)
                {
                    _CurrencyNegativeSymbolSuffix = value;
                    Common.AppLib.CurrencyNegativeSymbolSuffix = value;
                    NotifyPropertyChanged(nameof(CurrencyNegativeSymbolSuffix));
                    NotifyAllPropertyChanged();
                }
            }
        }
        public string CurrencyToWordPrefix
        {
            get
            {
                return _CurrencyToWordPrefix;
            }

            set
            {
                if (_CurrencyToWordPrefix != value)
                {
                    _CurrencyToWordPrefix = value;
                    Common.AppLib.CurrencyToWordPrefix = value;
                    NotifyPropertyChanged(nameof(CurrencyToWordPrefix));
                    NotifyAllPropertyChanged();
                }
            }
        }
        public string CurrencyToWordSuffix
        {
            get
            {
                return _CurrencyToWordSuffix;
            }

            set
            {
                if (_CurrencyToWordSuffix != value)
                {
                    _CurrencyToWordSuffix = value;
                    Common.AppLib.CurrencyToWordSuffix = value;
                    NotifyPropertyChanged(nameof(CurrencyToWordSuffix));
                    NotifyAllPropertyChanged();
                }
            }
        }
        public string DecimalToWordPrefix
        {
            get
            {
                return _DecimalToWordPrefix;
            }

            set
            {
                if (_DecimalToWordPrefix != value)
                {
                    _DecimalToWordPrefix = value;
                    Common.AppLib.DecimalToWordPrefix = value;
                    NotifyPropertyChanged(nameof(DecimalToWordPrefix));
                    NotifyAllPropertyChanged();
                }
            }
        }

        public string DecimalToWordSuffix
        {
            get
            {
                return _DecimalToWordSuffix;
            }

            set
            {
                if (_DecimalToWordSuffix != value)
                {
                    _DecimalToWordSuffix = value;
                    Common.AppLib.DecimalToWordSuffix = value;
                    NotifyPropertyChanged(nameof(DecimalToWordSuffix));
                    NotifyAllPropertyChanged();
                }
            }
        }
        public string DecimalSymbol
        {
            get
            {
                return _DecimalSymbol;
            }

            set
            {
                if (_DecimalSymbol != value)
                {
                    _DecimalSymbol = value;
                    Common.AppLib.DecimalSymbol = value;
                    NotifyPropertyChanged(nameof(DecimalSymbol));
                    NotifyAllPropertyChanged();
                }
            }
        }
        public int DigitGroupingBy
        {
            get
            {
                return _DigitGroupingBy;
            }

            set
            {
                if (_DigitGroupingBy != value)
                {
                    _DigitGroupingBy = value;
                    Common.AppLib.DigitGroupingBy = value;
                    NotifyPropertyChanged(nameof(DigitGroupingBy));
                    NotifyAllPropertyChanged();
                }
            }
        }

        public string DigitGroupingSymbol
        {
            get
            {
                return _DigitGroupingSymbol;
            }

            set
            {
                if (_DigitGroupingSymbol != value)
                {
                    _DigitGroupingSymbol = value;
                    Common.AppLib.DigitGroupingSymbol = value;
                    NotifyPropertyChanged(nameof(DigitGroupingSymbol));
                    NotifyAllPropertyChanged();
                }
            }
        }
        public int NoOfDigitAfterDecimal
        {
            get
            {
                return _NoOfDigitAfterDecimal;
            }

            set
            {
                if (_NoOfDigitAfterDecimal != value)
                {
                    _NoOfDigitAfterDecimal = value;
                    Common.AppLib.NoOfDigitAfterDecimal = value;
                    NotifyPropertyChanged(nameof(NoOfDigitAfterDecimal));
                    NotifyAllPropertyChanged();
                }
            }
        }
        public int CurrencyCaseSensitive
        {
            get
            {
                return _CurrencyCaseSensitive;
            }

            set
            {
                if (_CurrencyCaseSensitive != value)
                {
                    _CurrencyCaseSensitive = value;
                    Common.AppLib.CurrencyCaseSensitive = value;
                    NotifyPropertyChanged(nameof(CurrencyCaseSensitive));
                    NotifyAllPropertyChanged();
                }
            }
        }
        public bool IsDisplayWithOnlyOnSuffix
        {
            get
            {
                return _IsDisplayWithOnlyOnSuffix;
            }

            set
            {
                if (_IsDisplayWithOnlyOnSuffix != value)
                {
                    _IsDisplayWithOnlyOnSuffix = value;
                    Common.AppLib.IsDisplayWithOnlyOnSuffix = value;
                    NotifyPropertyChanged(nameof(IsDisplayWithOnlyOnSuffix));
                    NotifyAllPropertyChanged();
                }
            }
        }


        public decimal? SampleCurrency
        {
            get
            {
                if (_SampleCurrency == null) _SampleCurrency = (decimal)1234567.89;
                return _SampleCurrency;
            }

            set
            {
                if (_SampleCurrency != value)
                {
                    _SampleCurrency = value;
                    if (value == null)
                    {
                        SampleCurrencyPositive = null;
                    }
                    else
                    {
                        SampleCurrencyPositive = Math.Abs(value.Value);
                    }
                    NotifyAllPropertyChanged();
                }
            }
        }

        public decimal? SampleCurrencyPositive
        {
            get
            {
                if (_SampleCurrencyPositive == null) SampleCurrencyPositive = Math.Abs(SampleCurrency.Value);
                return _SampleCurrencyPositive;
            }

            set
            {
                if (_SampleCurrencyPositive != value)
                {
                    _SampleCurrencyPositive = value;
                    if (value == null)
                    {
                        SampleCurrencyNegative = null;
                    }
                    else
                    {
                        SampleCurrencyNegative = value * -1;
                    }
                    NotifyPropertyChanged(nameof(SampleCurrencyPositive));
                }
            }
        }


        public decimal? SampleCurrencyNegative
        {
            get
            {
                return _SampleCurrencyNegative;
            }

            set
            {
                if (_SampleCurrencyNegative != value)
                {
                    _SampleCurrencyNegative = value;
                    NotifyPropertyChanged(nameof(SampleCurrencyNegative));
                }
            }
        }


        public int CompanyId
        {
            get
            {
                return _CompanyId;
            }

            set
            {
                if (_CompanyId != value)
                {
                    _CompanyId = value;
                    NotifyPropertyChanged(nameof(CompanyId));
                }
            }
        }

        public CompanyDetail Company
        {
            get
            {
                return _Company;
            }
            set
            {
                if (_Company != value)
                {
                    _Company = value;
                    NotifyPropertyChanged(nameof(Company));
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

            try
            {
                CustomFormat d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new CustomFormat();
                    toList.Add(d);
                }

                this.toCopy<CustomFormat>(d);
                if (isServerCall == false)
                {
                    var i = NubeAccountClient.NubeAccountHub.Invoke<int>("CustomFormat_Save", this).Result;
                    d.Id = i;
                }
                SetDataFormat();
                return true;

            }
            catch (Exception ex) { }
            return false;
        }

        public void Clear()
        {
            new CustomFormat().toCopy<CustomFormat>(this);

            NotifyAllPropertyChanged();
        }

        public bool Find(int CompanyId)
        {
            int CId;

            if (BLL.UserAccount.User.UserType.Company.CompanyType == "Warehouse" || BLL.UserAccount.User.UserType.Company.CompanyType == "Dealer")
            {
                CId = (int)BLL.UserAccount.User.UserType.Company.UnderCompanyId;
                d = toList.Where(x => x.CompanyId == CId).FirstOrDefault();

            }
            else
            {
                d = toList.Where(x => x.CompanyId == CompanyId).FirstOrDefault();

            }
            if (d != null)
            {
                d.toCopy<CustomFormat>(this);
                //IsReadOnly = !UserPermission.AllowUpdate;

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
                    rv = NubeAccountClient.NubeAccountHub.Invoke<bool>("CustomFormat_Delete", this.Id).Result;
                    if (rv == true)
                    {
                        toList.Remove(d);

                    }

                }
                else
                {
                    toList.Remove(d);
                }
                return rv;
            }

            return rv;
        }



        public static void Init()
        {
            _toList = null;
        }

        public static void SetDataFormat()
        {
            CustomFormat V = new CustomFormat();
            V.Find(UserAccount.User.UserType.CompanyId);

            Common.AppLib.CurrencyPositiveSymbolPrefix = V.CurrencyPositiveSymbolPrefix;
            Common.AppLib.CurrencyPositiveSymbolSuffix = V.CurrencyPositiveSymbolSuffix;

            Common.AppLib.CurrencyToWordPrefix = V.CurrencyToWordPrefix;
            Common.AppLib.CurrencyToWordSuffix = V.CurrencyToWordSuffix;
            Common.AppLib.DecimalToWordPrefix = V.DecimalToWordPrefix;
            Common.AppLib.DecimalToWordSuffix = V.DecimalToWordSuffix;
            Common.AppLib.IsDisplayWithOnlyOnSuffix = V.IsDisplayWithOnlyOnSuffix;

        }


        #endregion

    }
}
