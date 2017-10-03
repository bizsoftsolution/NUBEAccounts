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
    public class UserType : INotifyPropertyChanged
    {
        #region Field

        private static UserTypeDetail _UserPermission;
        private bool _IsReadOnly;
        private bool _IsEnabled;

        private static ObservableCollection<UserType> _toList;
        public List<BLL.Validation> lstValidation = new List<BLL.Validation>();

        private int _id;
        private string _typeOfUser;
        private string _description;
        private int _FundMasterId;
        private FundMaster _Fund;
        private ObservableCollection<UserTypeDetail> _UserTypeDetails;

        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmUserType).FirstOrDefault();
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

        public static ObservableCollection<UserType> toList
        {
            get
            {
                if (_toList == null)
                {
                    _toList = new ObservableCollection<UserType>(NubeAccountClient.NubeAccountHub.Invoke<List<UserType>>("UserType_List").Result);
                }

                return _toList;
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
        public string TypeOfUser
        {
            get
            {
                return _typeOfUser;
            }
            set
            {
                if (_typeOfUser != value)
                {
                    _typeOfUser = value;
                    NotifyPropertyChanged(nameof(TypeOfUser));
                }
            }
        }
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    NotifyPropertyChanged(nameof(Description));
                }
            }
        }
        public int FundMasterId
        {
            get
            {
                return _FundMasterId;
            }
            set
            {
                if (_FundMasterId != value)
                {
                    _FundMasterId = value;
                    NotifyPropertyChanged(nameof(FundMasterId));
                }

            }
        }
        public FundMaster Fund
        {
            get
            {
                return _Fund;
            }
            set
            {
                if (_Fund != value)
                {
                    _Fund = value;
                    NotifyPropertyChanged(nameof(Fund));
                }
            }
        }


        public ObservableCollection<UserTypeDetail> UserTypeDetails
        {
            get
            {
                if (_UserTypeDetails == null) _UserTypeDetails = new ObservableCollection<UserTypeDetail>();
                return _UserTypeDetails;
            }
            set
            {
                if (_UserTypeDetails != value)
                {
                    _UserTypeDetails = value;
                    NotifyPropertyChanged(nameof(UserTypeDetails));
                }
            }
        }

        #endregion

        #region Property Notify Changed

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

                UserType d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new UserType();
                    toList.Add(d);
                }

                this.toCopy<UserType>(d);
                if (isServerCall == false)
                {
                    var i = NubeAccountClient.NubeAccountHub.Invoke<int>("userType_Save", this).Result;
                    d.Id = i;
                }

                return true;
            }
            catch (Exception ex)
            {
                lstValidation.Add(new Validation() { Name = string.Empty, Message = ex.Message });
                return false;
            }

        }

        public void Clear()
        {
            new UserType().toCopy<UserType>(this);
            this.UserTypeDetails = new ObservableCollection<UserTypeDetail>();
            foreach (var d in UserTypeFormDetail.toList)
            {
                UserTypeDetail utd = new UserTypeDetail();
                utd.UserTypeFormDetail = d.toCopy<BLL.UserTypeFormDetail>(new UserTypeFormDetail());
                utd.UserTypeFormDetailId = utd.UserTypeFormDetail.Id;
                UserTypeDetails.Add(utd);
            }
            IsReadOnly = !UserPermission.AllowInsert;
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<UserType>(this);
                IsReadOnly = !UserPermission.AllowUpdate;
                return true;
            }

            return false;
        }

        public bool Delete(bool isServerCall = false)
        {
            var d = toList.Where(x => x.Id == Id).FirstOrDefault();
            if (d != null)
            {
                toList.Remove(d);
                if (isServerCall == false) NubeAccountClient.NubeAccountHub.Invoke<int>("userType_Delete", this.Id);
                return true;
            }

            return false;
        }
        public bool isValid()
        {
            bool RValue = true;
            lstValidation.Clear();

            if (string.IsNullOrWhiteSpace(TypeOfUser))
            {
                lstValidation.Add(new Validation() { Name = nameof(TypeOfUser), Message = string.Format(Message.BLL.Required_Data, nameof(TypeOfUser)) });
                RValue = false;
            }
            else if (toList.Where(x => x.TypeOfUser.ToLower() == TypeOfUser.ToLower() && x.Id != Id).Count() > 0)
            {
                lstValidation.Add(new Validation() { Name = nameof(TypeOfUser), Message = string.Format(Message.BLL.Existing_Data, TypeOfUser) });
                RValue = false;
            }

            return RValue;

        }
        public static void Init()
        {
            _toList = null;
            UserPermission = null;
        }
        #endregion
    }
}

