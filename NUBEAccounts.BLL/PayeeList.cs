using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUBEAccounts.BLL
{
    public class PayeeList : INotifyPropertyChanged
    {

        private int _Id;
        private string _PayName;
        private static ObservableCollection<PayeeList> _PayList;
        public string PayName
        {
            get
            {
                return _PayName;
            }

            set
            {
                if (_PayName != value)
                {
                    _PayName = value;
                    NotifyPropertyChanged(nameof(PayName));
                }
            }
        }

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

        public static ObservableCollection<PayeeList> PayList
        {
            get
            {
                if (_PayList == null) _PayList = new ObservableCollection<PayeeList>(NubeAccountClient.NubeAccountHub.Invoke<List<PayeeList>>("PayeeList").Result);
                return _PayList;
            }
            set
            {
                _PayList = value;
            }
        }

        #endregion

    }
}
