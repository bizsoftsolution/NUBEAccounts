using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUBEAccounts.BLL
{
    public class VoucherReport : INotifyPropertyChanged
    {
        #region Fields

        private Ledger _Ledger;
        private decimal? _CrAmt;
        private decimal? _DrAmt;
        private decimal? _CrAmtOP;
        private decimal? _DrAmtOP;
        private string _Particulars;
        private string _EntryNo;
        private DateTime? _VDate;
        private string _PayTo;
        private string _AccountName;
        private string _EType;
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
                if (_Ledger!= value)
                {
                    _Ledger= value;
                    NotifyPropertyChanged(nameof(Ledger));
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
                    NotifyPropertyChanged(nameof(_Particulars));
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
                    NotifyPropertyChanged(nameof(_EntryNo));
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
                    NotifyPropertyChanged(nameof(_PayTo));
                }
            }
        }
        public DateTime? VDate
        {
            get
            {
                return _VDate;
            }
            set
            {
                if (_VDate != value)
                {
                    _VDate = value;
                    NotifyPropertyChanged(nameof(_VDate));
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
                    NotifyPropertyChanged(nameof(_AccountName));
                }
            }
        }
        public string EType
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
                    NotifyPropertyChanged(nameof(_EType));
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

        public static List<VoucherReport> ToList(DateTime dtFrom, DateTime dtTo,int? LID)
        {
            return NubeAccountClient.NubeAccountHub.Invoke<List<VoucherReport>>("VoucherReport_List", dtFrom, dtTo, LID).Result;
        }

        #endregion
    }
}
