using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUBEAccounts.Common;

namespace NUBEAccounts.BLL
{
    public class FundMaster : INotifyPropertyChanged
    {       

        #region Field
        bool isServerCall = false;

        private static ObservableCollection<FundMaster> _toList;

        private static UserTypeDetail _UserPermission;
        private bool _IsReadOnly;
        private bool _IsEnabled;

        private int _LoginAccYear;
        private static ObservableCollection<string> _AcYearList;
        public List<BLL.Validation> lstValidation = new List<BLL.Validation>();
        private int _id;
        private string _FundName;
        private bool _IsActive;

        private string _UserId;
        private string _Password;


        #endregion
        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmFundMaster).FirstOrDefault();
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

        public static ObservableCollection<FundMaster> toList
        {
            get
            {
                if (_toList == null)
                {
                    var l1 = NubeAccountClient.NubeAccountHub.Invoke<List<FundMaster>>(Message.SL.FundMaster_List).Result;
                    _toList = new ObservableCollection<FundMaster>(l1);
                }

                return _toList;
            }
        }        

        public bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }

            set
            {
                if (_IsReadOnly != value)
                {
                    _IsReadOnly = value;
                    IsEnabled = !value;
                    NotifyPropertyChanged(nameof(IsReadOnly));
                }
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }

            set
            {
                if (_IsEnabled != value)
                {
                    _IsEnabled = value;
                    NotifyPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged(nameof(Id));
                }
            }
        }

        public int LoginAccYear
        {
            get
            {
                return _LoginAccYear;
            }

            set
            {
                if (_LoginAccYear != value)
                {
                    _LoginAccYear = value;
                    NotifyPropertyChanged(nameof(LoginAccYear));
                }
            }
        }
        public string FundName
        {
            get
            {
                return _FundName;
            }

            set
            {
                if (_FundName != value)
                {
                    _FundName = value;
                    NotifyPropertyChanged(nameof(FundName));
                }
            }
        }

        public bool IsActive
        {
            get
            {
                return _IsActive;
            }

            set
            {
                if (_IsActive != value)
                {
                    _IsActive = value;
                    NotifyPropertyChanged(nameof(IsActive));
                }
            }
        }

        public string UserId
        {
            get
            {
                return _UserId;
            }

            set
            {
                if (_UserId != value)
                {
                    _UserId = value;
                    NotifyPropertyChanged(nameof(UserId));
                }
            }
        }

        public string Password
        {
            get
            {
                return _Password;
            }

            set
            {
                if (_Password != value)
                {
                    _Password = value;
                    NotifyPropertyChanged(nameof(Password));
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

        public bool Save(bool isServerCall = false)
        {
            if (!isValid()) return false;
            try
            {
                FundMaster d = toList.Where(x => x.Id == Id).FirstOrDefault();
                int i = 0;
                if (d == null)
                {
                    d = new FundMaster();
                    toList.Add(d);
                }

                this.toCopy<FundMaster>(d);
                if (isServerCall == false)
                {
                    i = NubeAccountClient.NubeAccountHub.Invoke<int>(Message.SL.FundMaster_Save, this).Result;
                    d.Id = i;
                }

                return i != 0;
            }
            catch (Exception ex)
            {
                lstValidation.Add(new Validation() { Name = string.Empty, Message = ex.Message });
                return false;

            }

        }

        public void Clear()
        {
            new FundMaster().toCopy<FundMaster>(this);
            IsReadOnly = !UserPermission.AllowInsert;
            IsActive = true;
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<FundMaster>(this);
                IsReadOnly = !UserPermission.AllowUpdate;
                return true;
            }

            return false;
        }

        public bool isValid()
        {
            bool RValue = true;

            lstValidation.Clear();
            var cm = toList.Where(x => x.FundName == FundName).FirstOrDefault();

            var user = BLL.UserAccount.toList.Where(x => x.UserName == UserId && x.UserType.Fund.FundName == FundName).FirstOrDefault();




            if (string.IsNullOrWhiteSpace(FundName))
            {
                lstValidation.Add(new Validation() { Name = nameof(FundName), Message = string.Format(Message.BLL.Required_Data, nameof(FundName)) });
                RValue = false;
            }
            else if (cm != null)
            {
                if (cm.IsActive == false)
                {
                    lstValidation.Add(new Validation() { Name = nameof(FundName), Message = string.Format("{0} is Deleted. Please Contact DENARIUSOFT Administrator.", FundName) });
                    RValue = false;
                }
                else if (cm.Id != Id)
                {
                    lstValidation.Add(new Validation() { Name = nameof(FundName), Message = string.Format(Message.BLL.Existing_Data, FundName) });
                    RValue = false;
                }


            }
            else if (user != null)
            {
                if (user.UserName == UserId)
                {
                    lstValidation.Add(new Validation() { Name = nameof(FundName), Message = string.Format(Message.PL.User_Id_Exist, FundName) });
                    RValue = false;
                }
            }

            else if (Id == 0)
            {
                if (string.IsNullOrWhiteSpace(UserId))
                {
                    lstValidation.Add(new Validation() { Name = nameof(UserId), Message = string.Format(Message.BLL.Required_Data, nameof(UserId)) });
                    RValue = false;
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    lstValidation.Add(new Validation() { Name = nameof(Password), Message = string.Format(Message.BLL.Required_Data, nameof(Password)) });
                    RValue = false;
                }


            }

            return RValue;

        }

        public bool Delete(bool isServerCall = false)
        {
            var d = toList.Where(x => x.Id == Id).FirstOrDefault();
            if (d != null)
            {
                toList.Remove(d);
                if (isServerCall == false) NubeAccountClient.NubeAccountHub.Invoke<int>("FundMaster_Delete", this.Id);
                return true;
            }

            return false;
        }
        public bool DeleteWareHouse(int Id)
        {
            var c = toList.Where(x => x.Id == Id).FirstOrDefault();

            if (c != null)
            {
                toList.Remove(c);
                if (isServerCall == false) NubeAccountClient.NubeAccountHub.Invoke<int>("FundMaster_Delete", c.Id);
                return true;
            }
            return false;
        }
        public static void Init()
        {
            _toList = null;
        }

        public static List<string> AcYearList(int FundId)
        {
            return NubeAccountClient.NubeAccountHub.Invoke<List<string>>(Message.SL.FundMaster_AcYearList,FundId).Result;            
        }

        #endregion
    }
}
