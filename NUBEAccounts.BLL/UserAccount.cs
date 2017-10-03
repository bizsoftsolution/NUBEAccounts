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
    public class UserAccount : INotifyPropertyChanged
    {
        #region Field

        public static UserAccount User = new UserAccount();
        public static string LoginedACYear;
        public static string CurrentACYear;
        public static bool IsCurrentACYear;
        
        private static UserTypeDetail _UserPermission;
        private bool _IsReadOnly;
        private bool _IsEnabled;


        private static ObservableCollection<UserAccount> _toList;
        public List<BLL.Validation> lstValidation = new List<BLL.Validation>();

        private int _id;
        private string _userName;
        private string _loginId;
        private string _password;
        private int _UserTypeId;
        private UserType _UserType;
        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmUser).FirstOrDefault();
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

        public static ObservableCollection<UserAccount> toList
        {
            get
            {
                if (_toList == null)
                {
                    var l1 = NubeAccountClient.NubeAccountHub.Invoke<List<UserAccount>>("UserAccount_List").Result;
                    _toList = new ObservableCollection<UserAccount>(l1);
                }

                return _toList;
            }
            set
            {
                _toList = value;
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


        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    NotifyPropertyChanged(nameof(UserName));
                }
            }
        }
        public string LoginId
        {
            get
            {
                return _loginId;
            }
            set
            {
                if (_loginId != value)
                {
                    _loginId = value;
                    NotifyPropertyChanged(nameof(LoginId));
                }
            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    NotifyPropertyChanged(nameof(Password));
                }
            }
        }
        public int UserTypeId
        {
            get
            {
                return _UserTypeId;
            }
            set
            {
                if (_UserTypeId != value)
                {
                    _UserTypeId = value;
                    NotifyPropertyChanged(nameof(UserTypeId));
                }
            }
        }
        public UserType UserType
        {
            get
            {
                return _UserType;
            }
            set
            {
                if (_UserType != value)
                {
                    _UserType = value;
                    NotifyPropertyChanged(nameof(UserType));
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

        #region Method
        public static string Login(string AccYear, String CompanyName, String LId, String Pwd)
        {
            var ua = NubeAccountClient.NubeAccountHub.Invoke<UserAccount>("UserAccount_Login", AccYear, CompanyName, LId, Pwd).Result;

            if (isValidLogin(ua, AccYear, CompanyName, LId, Pwd))
            {
                try
                {
                    DateTime dt = DateTime.Now;
                    User = ua;
                    LoginedACYear = AccYear;
                    CurrentACYear = dt.Month <= 3 ? string.Format("{0} - {1}", dt.Year, dt.Year + 1) : string.Format("{0} - {1}",dt.Year-1,dt.Year);
                    IsCurrentACYear = LoginedACYear.StartsWith(CurrentACYear.Substring(0, 4));
                    Data_Init();
                    return "";
                }
                catch (Exception ex)
                { }


            }
            return string.Join("\n", ua.lstValidation.Select(x => x.Message));
        }

        static void Data_Init()
        {
            BLL.UserAccount.Init();
            BLL.UserType.Init();            

            BLL.AccountGroup.Init();
          
            BLL.Ledger.Init();

            BLL.DataKeyValue.Init();

         


        }

        public static bool AllowFormShow(string FormName)
        {
            bool rv = true;
            var t = User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == FormName).FirstOrDefault();
            if (t != null) rv = t.IsViewForm;
            return rv;
        }

        public static bool AllowInsert(string FormName)
        {
            bool rv = true;
            var t = User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == FormName).FirstOrDefault();
            if (t != null) rv = t.AllowInsert;
            return rv;
        }

        public static bool AllowUpdate(string FormName)
        {
            bool rv = true;
            var t = User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == FormName).FirstOrDefault();
            if (t != null) rv = t.AllowUpdate;
            return rv;
        }

        public static bool AllowDelete(string FormName)
        {
            bool rv = true;
            var t = User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == FormName).FirstOrDefault();
            if (t != null) rv = t.AllowDelete;
            return rv;
        }

        public bool Save(bool isServerCall = false)
        {
            if (!isValid()) return false;
            try
            {

                UserAccount d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new UserAccount();
                    toList.Add(d);
                }

                this.toCopy<UserAccount>(d);
                if (isServerCall == false)
                {

                    var i = NubeAccountClient.NubeAccountHub.Invoke<int>("UserAccount_Save", this).Result;
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

            this.UserName = "";
            this.LoginId = "";
            this.Password = "";
            this.UserTypeId = 0;
            this.UserType = null;
            IsReadOnly = !UserPermission.AllowInsert;
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<UserAccount>(this);
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
                if (isServerCall == false) NubeAccountClient.NubeAccountHub.Invoke<int>("UserAccount_Delete", this.Id);
                return true;
            }

            return false;
        }

        public bool isValid()
        {
            bool RValue = true;
            lstValidation.Clear();

            if (string.IsNullOrWhiteSpace(UserName))
            {
                lstValidation.Add(new Validation() { Name = nameof(UserName), Message = string.Format(Message.BLL.Required_Data, nameof(UserName)) });
                RValue = false;
            }

            if (string.IsNullOrWhiteSpace(LoginId))
            {
                lstValidation.Add(new Validation() { Name = nameof(LoginId), Message = string.Format(Message.BLL.Required_Data, nameof(LoginId)) });
                RValue = false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                lstValidation.Add(new Validation() { Name = nameof(Password), Message = string.Format(Message.BLL.Required_Data, nameof(Password)) });
                RValue = false;
            }
            if (UserTypeId == 0)
            {
                lstValidation.Add(new Validation() { Name = nameof(UserType), Message = string.Format(Message.BLL.Required_Data, nameof(UserType)) });
                RValue = false;
            }
            else if (toList.Where(x => x.UserType.FundMasterId == UserType.FundMasterId && x.Id != Id && x.LoginId.ToLower() == LoginId.ToLower()).Count() > 0)
            {
                lstValidation.Add(new Validation() { Name = nameof(LoginId), Message = string.Format(Message.BLL.Existing_Data, LoginId) });
                RValue = false;
            }
            else if (toList.Where(x => x.Id == 0 && x.UserType.FundMasterId == UserType.FundMasterId && x.LoginId.ToLower() == LoginId.ToLower()).Count() > 0)
            {
                lstValidation.Add(new Validation() { Name = nameof(LoginId), Message = string.Format(Message.BLL.Existing_Data, LoginId) });
                RValue = false;
            }
            return RValue;

        }

        public static bool isValidLogin(UserAccount ua, string AccYear, string CompanyName, string LId, string pwd)
        {
            bool RValue = true;
            ua.lstValidation.Clear();

            //if (string.IsNullOrWhiteSpace(AccYear))
            //{
            //    ua.lstValidation.Add(new Validation() { Name = nameof(AccYear), Message = "Please Select the Account Year" });
            //    RValue = false;
            //}

            if (string.IsNullOrWhiteSpace(CompanyName))
            {
                ua.lstValidation.Add(new Validation() { Name = nameof(CompanyName), Message = "Please Select the Company" });
                RValue = false;
            }

            if (string.IsNullOrWhiteSpace(LId))
            {
                ua.lstValidation.Add(new Validation() { Name = nameof(LoginId), Message = "Please Enter the Login Id" });
                RValue = false;
            }

            if (string.IsNullOrWhiteSpace(pwd))
            {
                ua.lstValidation.Add(new Validation() { Name = nameof(Password), Message = "Please Enter the Password" });
                RValue = false;
            }
            if (RValue == true)
            {
                if (ua.LoginId != LId || ua.Password != pwd)
                {
                    ua.lstValidation.Add(new Validation() { Name = nameof(LoginId), Message = "Please Enter the Valid User Id or Password" });
                    RValue = false;
                }
            }

            return RValue;

        }
        public static Boolean Admin_Authentication(String CompanyName, String LId, String Pwd)
        {
            var ua = NubeAccountClient.NubeAccountHub.Invoke<Boolean>("Admin_Authentication", CompanyName, LId, Pwd).Result;

            return ua;

        }

        public static void Init()
        {
            _toList = null;
            UserPermission = null;
            FundMaster.UserPermission = null;
            UserType.UserPermission = null;

        }
        #endregion
    }
}
