using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUBEAccounts.Common;

namespace NUBEAccounts.BLL
{
    public class MonthlyReport : INotifyPropertyChanged
    {
        #region Fields
        private string _Description;
        private decimal? _M1;
        private decimal? _M2;
        private decimal? _M3;
        private decimal? _M4;
        private decimal? _M5;
        private decimal? _M6;
        private decimal? _M7;
        private decimal? _M8;
        private decimal? _M9;
        private decimal? _M10;
        private decimal? _M11;
        private decimal? _M12;
        private decimal? _Amount;
        private static UserTypeDetail _UserPermission;
        private Ledger _LedgerList;
        private int _LId;


        #endregion

        #region Property

        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmIncomeAndExpenditure.ToString()).FirstOrDefault();
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
        public int  LId
        {
            get
            {
                return _LId;
            }
            set
            {
                if (_LId != value)
                {
                    _LId = value;
                    NotifyPropertyChanged(nameof(LId));
                }
            }
        }

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    NotifyPropertyChanged(nameof(Description));
                }
            }
        }

        public decimal? Amount
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

        public decimal? M1
        {
            get
            {
                return _M1;
            }
            set
            {
                if (_M1 != value)
                {
                    _M1 = value;
                    NotifyPropertyChanged(nameof(M1));
                }
            }
        }
        public decimal? M2
        {
            get
            {
                return _M2;
            }
            set
            {
                if (_M2 != value)
                {
                    _M2 = value;
                    NotifyPropertyChanged(nameof(M2));
                }
            }
        }
        public decimal? M3
        {
            get
            {
                return _M3;
            }
            set
            {
                if (_M3 != value)
                {
                    _M3 = value;
                    NotifyPropertyChanged(nameof(M3));
                }
            }
        }
        public decimal? M4
        {
            get
            {
                return _M4;
            }
            set
            {
                if (_M4 != value)
                {
                    _M4 = value;
                    NotifyPropertyChanged(nameof(M4));
                }
            }
        }
        public decimal? M5
        {
            get
            {
                return _M5;
            }
            set
            {
                if (_M5 != value)
                {
                    _M5 = value;
                    NotifyPropertyChanged(nameof(M5));
                }
            }
        }
        public decimal? M6
        {
            get
            {
                return _M6;
            }
            set
            {
                if (_M6 != value)
                {
                    _M6 = value;
                    NotifyPropertyChanged(nameof(M6));
                }
            }
        }
        public decimal? M7
        {
            get
            {
                return _M7;
            }
            set
            {
                if (_M7 != value)
                {
                    _M7 = value;
                    NotifyPropertyChanged(nameof(M7));
                }
            }
        }
        public decimal? M8
        {
            get
            {
                return _M7;
            }
            set
            {
                if (_M7 != value)
                {
                    _M7 = value;
                    NotifyPropertyChanged(nameof(M7));
                }
            }
        }
        public decimal? M9
        {
            get
            {
                return _M9;
            }
            set
            {
                if (_M9 != value)
                {
                    _M9 = value;
                    NotifyPropertyChanged(nameof(M9));
                }
            }
        }
        public decimal? M10
        {
            get
            {
                return _M10;
            }
            set
            {
                if (_M10 != value)
                {
                    _M10 = value;
                    NotifyPropertyChanged(nameof(M10));
                }
            }
        }
        public decimal? M11
        {
            get
            {
                return _M11;
            }
            set
            {
                if (_M11 != value)
                {
                    _M11 = value;
                    NotifyPropertyChanged(nameof(M11));
                }
            }
        }
        public decimal? M12
        {
            get
            {
                return _M12;
            }
            set
            {
                if (_M12 != value)
                {
                    _M12 = value;
                    NotifyPropertyChanged(nameof(M12));
                }
            }
        }

        public Ledger Ledger { get; set; }
        public Ledger LedgerList
        {
            get
            {
                return _LedgerList;
            }
            set
            {
                if (_LedgerList != value)
                {
                    _LedgerList = value;
                    NotifyPropertyChanged(nameof(LedgerList));
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

        public static List<MonthlyReport> ToList(DateTime dtFrom, DateTime dtTo, bool isMonthly)
        {
            return NubeAccountClient.NubeAccountHub.Invoke<List<MonthlyReport>>("MonthlyReport_List", dtFrom, dtTo, isMonthly).Result;
        }
       

        #endregion
    }
}
