using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUBEAccounts.BLL
{
    public class BankReconcilation : INotifyPropertyChanged
    {
        #region Fields
        private long _EId;
        private Char _EType;
        private DateTime? _EDate;
        private String _EntryNo;
        private string _RefNo;
        private Ledger _Ledger;
        private decimal _CrAmt;
        private decimal _DrAmt;
        private string _AccountName;
        private string _Particular;
        private bool _IsCompleted;
        private string _VoucherNo;

        #endregion

        #region Property

        public long EId
        {
            get
            {
                return _EId;
            }
            set
            {
                if (_EId != value)
                {
                    _EId = value;
                    NotifyPropertyChanged(nameof(EId));
                }
            }
        }

        public bool IsCompleted
        {
            get
            {
                return _IsCompleted;
            }
            set
            {
                if (_IsCompleted != value)
                {
                    _IsCompleted = value;
                    NotifyPropertyChanged(nameof(IsCompleted));
                }
            }
        }
        public char EType
        {
            get
            {
                return _EType;
            }
            set
            {
                if (_EType != value)
                {
                    _EType = value;
                    NotifyPropertyChanged(nameof(EType));
                }
            }
        }
        public DateTime? EDate
        {
            get
            {
                return _EDate;
            }
            set
            {
                if (_EDate != value)
                {
                    _EDate = value;
                    NotifyPropertyChanged(nameof(EDate));
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
        public Ledger Ledger
        {
            get
            {
                return _Ledger;
            }
            set
            {
                if (_Ledger != value)
                {
                    _Ledger = value;
                    NotifyPropertyChanged(nameof(Ledger));
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

        public static List<BankReconcilation> ToList(int LedgerId, DateTime dtFrom, DateTime dtTo)
        {
            return NubeAccountClient.NubeAccountHub.Invoke<List<BankReconcilation>>("BankReconcilation_List", LedgerId, dtFrom, dtTo).Result;
        }

        #endregion
    }
}
