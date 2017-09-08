using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUBEAccounts.BLL
{
    public class ReceiptAndPayment : INotifyPropertyChanged
    {
        #region Fields
        private long _EId;
        private Char _EType;
        private DateTime? _EDate;
        private String _EntryNo;
        private string _RefNo;
        private Ledger _Ledger;
        private decimal _Amount;
        private decimal? _AmountCr;
        private decimal? _AmountDr;
        private string _AccountName;

        public string _PayTo;
        public string _Status;
        public string _Particulars;
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
        public decimal? AmountCr
        {
            get
            {
                return _AmountCr;
            }
            set
            {
                if (_AmountCr != value)
                {
                    _AmountCr = value;
                    NotifyPropertyChanged(nameof(AmountCr));
                }
            }
        }
        public decimal? AmountDr
        {
            get
            {
                return _AmountDr;
            }
            set
            {
                if (_AmountDr != value)
                {
                    _AmountDr = value;
                    NotifyPropertyChanged(nameof(AmountDr));
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
        public string Particular
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
                    NotifyPropertyChanged(nameof(Particular));
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

        public static List<ReceiptAndPayment> ToList(int? LedgerId, DateTime dtFrom, DateTime dtTo, string entryNo, string Status, bool AccountHead)
        {
            return NubeAccountClient.NubeAccountHub.Invoke<List<ReceiptAndPayment>>("ReceiptAndPayment_List", LedgerId, dtFrom, dtTo, entryNo, Status, AccountHead).Result;
        }

        public static List<ReceiptAndPayment> ToListNEC(int? LedgerId, DateTime dtFrom, DateTime dtTo, string entryNo, string Status, bool AccountHead)
        {
            return NubeAccountClient.NubeAccountHub.Invoke<List<ReceiptAndPayment>>("NEC_List", LedgerId, dtFrom, dtTo, entryNo, Status, AccountHead).Result;
        }
        #endregion
    }
}
