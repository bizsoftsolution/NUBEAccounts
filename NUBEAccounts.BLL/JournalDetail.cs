﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUBEAccounts.BLL
{
    public class JournalDetail : INotifyPropertyChanged
    {

        #region Fields
        private long _Id;
        private long _JournalId;
        private int _LedgerId;
        private decimal _DrAmt;
        private decimal _CrAmt;
        private string _Particular;
        private Ledger _JLedger;
        private string _LedgerName;

        #endregion

        #region Property

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

        public long JournalId
        {
            get
            {
                return _JournalId;
            }
            set
            {
                if (_JournalId != value)
                {
                    _JournalId = value;
                    NotifyPropertyChanged(nameof(JournalId));
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
                    if (value != 0)
                    {
                        CrAmt = 0;

                    }
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
                    if (value != 0)
                    {
                        DrAmt = 0;
                    }
                }
            }
        }

        public string Particulars
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
                    NotifyPropertyChanged(nameof(Particulars));
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

        public Ledger JLedger
        {
            get
            {
                if (_JLedger == null) _JLedger = new Ledger();
                return _JLedger;
            }
            set
            {
                if (_JLedger != value)
                {
                    _JLedger = value;
                    NotifyPropertyChanged(nameof(JLedger));
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
    }
}
