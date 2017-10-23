using MahApps.Metro.Controls;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NUBEAccounts.Pl.frm.Master
{
    /// <summary>
    /// Interaction logic for frmUserType.xaml
    /// </summary>
    public partial class frmUserType : MetroWindow
    {
        #region Field

        public static string FormName = "UserType";
        BLL.UserType data = new BLL.UserType();

        #endregion

        #region Constructor

        public frmUserType()
        {
            InitializeComponent();
            this.DataContext = data;

            onClientEvents();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        public void LoadWindow()
        {
            dgvDetail.ItemsSource = BLL.UserType.toList;
            Clear();
            btnSave.Visibility = (BLL.UserType.UserPermission.AllowInsert || BLL.UserType.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.UserType.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id == 0 && !BLL.UserAccount.AllowInsert(Common.Forms.frmUserType))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName));
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(Common.Forms.frmUserType))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName));
            }
            else
            {
                if (data.Save() == true)
                {
                    MessageBox.Show("Saved");
                    this.Close();
                }
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id == 0) MessageBox.Show("No Records to Delete");
            else
            {
                if (!BLL.UserAccount.AllowDelete(Common.Forms.frmUserType)) MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName));
                else if (MessageBox.Show("Do you want to Delete this record?", "DELETE", MessageBoxButton.YesNo) != MessageBoxResult.No)
                {
                    if (data.Delete() == true)
                    {
                        MessageBox.Show("Deleted");
                        this.Close();
                    }
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void dgvDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ckbAllViewForm.IsChecked = false;
            ckbAllAllowInsert.IsChecked = false;
            ckbAllAllowUpdate.IsChecked = false;
            ckbAllAllowDelete.IsChecked = false;

            ViewForm();
        }



        #endregion

        #region Methods

        private void Clear()
        {
            data.Clear();
            ckbAllViewForm.IsChecked = false;
            ckbAllAllowInsert.IsChecked = false;
            ckbAllAllowUpdate.IsChecked = false;
            ckbAllAllowDelete.IsChecked = false;           
        }
        private void onClientEvents()
        {
            BLL.NubeAccountClient.NubeAccountHub.On<BLL.UserType>("userType_Save", (rv) => {

                this.Dispatcher.Invoke(() =>
                {
                    rv.Save(true);
                });

            });

            BLL.NubeAccountClient.NubeAccountHub.On("userType_Delete", (Action<int>)((pk) => {
                this.Dispatcher.Invoke((Action)(() => {
                    BLL.UserType d = new BLL.UserType();
                    d.Find((int)pk);
                    d.Delete((bool)true);
                }));

            }));
        }

        #endregion

        private void ckbAllViewForm_Checked(object sender, RoutedEventArgs e)
        {
            if (ckbAllViewForm.IsFocused) foreach (var d in data.UserTypeDetails.Where(x => x.UserTypeFormDetail.IsActive).ToList()) d.IsViewForm = true;
            dgvUserTypeDetail.Focus();
        }

        private void ckbAllAllowInsert_Checked(object sender, RoutedEventArgs e)
        {
            if (ckbAllAllowInsert.IsFocused) foreach (var d in data.UserTypeDetails.Where(x => x.UserTypeFormDetail.IsActive && x.UserTypeFormDetail.IsInsert).ToList()) d.AllowInsert = true;
            dgvUserTypeDetail.Focus();
        }

        private void ckbAllAllowUpdate_Checked(object sender, RoutedEventArgs e)
        {
            if (ckbAllAllowUpdate.IsFocused) foreach (var d in data.UserTypeDetails.Where(x => x.UserTypeFormDetail.IsActive && x.UserTypeFormDetail.IsUpdate).ToList()) d.AllowUpdate = true;
            dgvUserTypeDetail.Focus();
        }

        private void ckbAllAllowDelete_Checked(object sender, RoutedEventArgs e)
        {
            if (ckbAllAllowDelete.IsFocused) foreach (var d in data.UserTypeDetails.Where(x => x.UserTypeFormDetail.IsActive && x.UserTypeFormDetail.IsDelete).ToList()) d.AllowDelete = true;
            dgvUserTypeDetail.Focus();
        }

        private void ckbAllViewForm_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ckbAllViewForm.IsFocused) foreach (var d in data.UserTypeDetails) d.IsViewForm = false;
            dgvUserTypeDetail.Focus();
        }

        private void ckbAllAllowInsert_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ckbAllAllowInsert.IsFocused) foreach (var d in data.UserTypeDetails.ToList()) d.AllowInsert = false;
            dgvUserTypeDetail.Focus();
        }

        private void ckbAllAllowUpdate_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ckbAllAllowUpdate.IsFocused) foreach (var d in data.UserTypeDetails.ToList()) d.AllowUpdate = false;
            dgvUserTypeDetail.Focus();
        }

        private void ckbAllAllowDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ckbAllAllowDelete.IsFocused) foreach (var d in data.UserTypeDetails.ToList()) d.AllowDelete = false;
            dgvUserTypeDetail.Focus();
        }

        private void ckbViewForm_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.UserTypeDetail;
                if (d != null) d.IsViewForm = true;
                if (data.UserTypeDetails.Where(x => x.UserTypeFormDetail.IsActive && !x.IsViewForm).Count() == 0) ckbAllViewForm.IsChecked = true;
            }
            catch (Exception ex) { }
        }

        private void ckbAllowInsert_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.UserTypeDetail;
                if (d != null) d.AllowInsert = true;
                if (data.UserTypeDetails.Where(x => x.UserTypeFormDetail.IsActive && x.UserTypeFormDetail.IsInsert && x.AllowInsert == false).Count() == 0) ckbAllAllowInsert.IsChecked = true;
            }
            catch (Exception ex) { }
        }

        private void ckbAllowUpdate_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.UserTypeDetail;
                if (d != null) d.AllowUpdate = true;
                if (data.UserTypeDetails.Where(x => x.UserTypeFormDetail.IsActive && x.UserTypeFormDetail.IsUpdate && x.AllowUpdate == false).Count() == 0) ckbAllAllowUpdate.IsChecked = true;
            }
            catch (Exception ex) { }
        }

        private void ckbAllowDelete_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.UserTypeDetail;
                if (d != null) d.AllowDelete = true;
                if (data.UserTypeDetails.Where(x => x.UserTypeFormDetail.IsActive && x.UserTypeFormDetail.IsDelete && x.AllowDelete == false).Count() == 0) ckbAllAllowDelete.IsChecked = true;
            }
            catch (Exception ex) { }
        }

        private void ckbViewForm_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.UserTypeDetail;
                if (d != null) d.IsViewForm = false;
                ckbAllViewForm.IsChecked = false;
            }
            catch (Exception ex) { }
        }

        private void ckbAllowInsert_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.UserTypeDetail;
                if (d != null) d.AllowInsert = false;
                ckbAllAllowInsert.IsChecked = false;
            }
            catch (Exception ex) { }
        }

        private void ckbAllowUpdate_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.UserTypeDetail;
                if (d != null) d.AllowUpdate = false;
                ckbAllAllowUpdate.IsChecked = false;
            }
            catch (Exception ex) { }
        }

        private void ckbAllowDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.UserTypeDetail;
                if (d != null) d.AllowDelete = false;
                ckbAllAllowDelete.IsChecked = false;
            }
            catch (Exception ex) { }
        }

        private void dgvDetail_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ViewForm();
        }
        void ViewForm()
        {
            var d = dgvDetail.SelectedItem as BLL.UserType;
            if (d != null)
            {
                if (data.Find(d.Id))
                {
                    //    ckbAllViewForm.IsChecked = data.UserTypeDetails.Where(x => x.UserTypeFormDetail.IsActive && !x.IsViewForm).Count() == 0;
                    //    ckbAllAllowInsert.IsChecked = data.UserTypeDetails.Where(x => x.UserTypeFormDetail.IsActive && x.UserTypeFormDetail.IsInsert && x.AllowInsert == false).Count() == 0;
                    //    ckbAllAllowUpdate.IsChecked = data.UserTypeDetails.Where(x => x.UserTypeFormDetail.IsActive && x.UserTypeFormDetail.IsUpdate && x.AllowUpdate == false).Count() == 0;
                    //    ckbAllAllowDelete.IsChecked = data.UserTypeDetails.Where(x => x.UserTypeFormDetail.IsActive && x.UserTypeFormDetail.IsDelete && x.AllowDelete == false).Count() == 0;
                    //
                }
            }
        }
    }
}
