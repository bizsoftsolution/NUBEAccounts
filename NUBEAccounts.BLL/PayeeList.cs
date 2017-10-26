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

        
        private string _PayeeName;
        private static ObservableCollection<PayeeList> _toList;
        private bool _IsChecked;

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
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }

            set
            {
                if (_IsChecked != value)
                {
                    _IsChecked = value;
                    NotifyPropertyChanged(nameof(IsChecked));
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

        public static ObservableCollection<PayeeList> toList
        {
            get
            {
                if (_toList == null) _toList = new ObservableCollection<PayeeList>(NubeAccountClient.NubeAccountHub.Invoke<List<PayeeList>>("PayeeList").Result);
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
