using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUBEAccounts.BLL
{
   public class IndividualReport : INotifyPropertyChanged
    {
        #region Fields

        private int _LedgerId;
        private decimal _Amount;
        private string _LedgerName;
        private string _PayeeName;
        #endregion

        #region Property
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
        public string PayeeName
        {
            get
            {
                return _PayeeName;
            }
            set
            {
                if (_PayeeName != value)
                {
                    _PayeeName = value;
                    NotifyPropertyChanged(nameof(PayeeName));
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

        public static List<IndividualReport> ToList(DateTime dtFrom, DateTime dtTo, List<int> LedgerIdList, List<string> PayeeNameList)
        {
            return NubeAccountClient.NubeAccountHub.Invoke<List<IndividualReport>>("IndividualReport_List", dtFrom, dtTo, LedgerIdList, PayeeNameList).Result;
        }
    }
    #endregion
}
