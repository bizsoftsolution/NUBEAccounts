using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUBEAccounts.BLL
{
   public class ExpenseReport : INotifyPropertyChanged
    {
        #region Fields

        private Ledger _Ledger;
        private string _EntryNo;
        private string _Particulars;
        private string _Payto;
        private decimal? _CrAmt;
        private decimal? _DrAmt;
        private decimal? _CrAmtOP;
        private decimal? _DrAmtOP;
        private decimal? _Amt;

        private string _AccountName;
        private string _VoucherNo;
        #endregion

        #region Property
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
        public string Payto
        {
            get
            {
                return _Payto;
            }
            set
            {
                if (_Payto != value)
                {
                    _Payto = value;
                    NotifyPropertyChanged(nameof(Payto));
                }
            }
        }
        public decimal? CrAmt
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
        public decimal? DrAmt
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

        public decimal? CrAmtOP
        {
            get
            {
                return _CrAmtOP;
            }
            set
            {
                if (_CrAmtOP != value)
                {
                    _CrAmtOP = value;
                    NotifyPropertyChanged(nameof(CrAmtOP));
                }
            }
        }
        public decimal? DrAmtOP
        {
            get
            {
                return _DrAmtOP;
            }
            set
            {
                if (_DrAmtOP != value)
                {
                    _DrAmtOP = value;
                    NotifyPropertyChanged(nameof(DrAmtOP));
                }
            }
        }

        public decimal? Amt
        {
            get
            {
                return _Amt;
            }
            set
            {
                if (_Amt != value)
                {
                    _Amt = value;
                    NotifyPropertyChanged(nameof(Amt));
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

        public static List<ExpenseReport> ToList(int? LedgerId, DateTime dtFrom, DateTime dtTo, string entryNo, string PayTo,bool AccountHead)
        {
            return NubeAccountClient.NubeAccountHub.Invoke<List<ExpenseReport>>("ExpenseReport_List", LedgerId, dtFrom, dtTo, entryNo, PayTo,AccountHead).Result;
        }

        #endregion
    }
}
