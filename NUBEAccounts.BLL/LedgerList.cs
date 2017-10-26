using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUBEAccounts.BLL
{
    public class LedgerList : INotifyPropertyChanged
    {
        private static ObservableCollection<LedgerList> _toList;

        public int LedgerId { get; set; }
        public string LedgerName { get; set; }
        public bool IsChecked { get; set; }


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

        public static ObservableCollection<LedgerList> toList
        {
            get
            {
                if (_toList == null) _toList = new ObservableCollection<LedgerList>(NubeAccountClient.NubeAccountHub.Invoke<List<LedgerList>>("LedgerList").Result);
                return _toList;
            }
            set
            {
                _toList = value;
            }
        }

        #endregion

    }
}
