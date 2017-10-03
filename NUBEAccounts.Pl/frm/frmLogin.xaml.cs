using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Microsoft.AspNet.SignalR.Client;

namespace NUBEAccounts.Pl.frm
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    /// 

    public partial class frmLogin : MetroWindow
    {

        public frmLogin()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var f = new BLL.FundMaster();
            cmbFund.ItemsSource = BLL.FundMaster.toList;
            cmbFund.SelectedValuePath = nameof(f.Id);
            cmbFund.DisplayMemberPath = nameof(f.FundName);
            onClientEvents();            
        }

        private void onClientEvents()
        {
            BLL.NubeAccountClient.NubeAccountHub.On<BLL.FundMaster>(Message.SL.FundMaster_Save, (d) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    d.Save(true);
                });

            });
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

            string RValue = BLL.UserAccount.Login(cmbYear.Text, cmbFund.Text, txtUserId.Text, txtPassword.Password);

            if (string.IsNullOrEmpty(RValue))
            {
                App.frmHome = new frmHome();
                App.frmHome.Title = String.Format(Message.PL.Home_Title, BLL.UserAccount.User.UserName, BLL.UserAccount.User.UserType.Fund.FundName,BLL.UserAccount.LoginedACYear);
                this.Hide();
                cmbFund.Text = string.Empty;
                txtUserId.Text = string.Empty;
                int yy = BLL.UserAccount.User.UserType.Fund.LoginAccYear;


                Common.AppLib.minDate = new DateTime(yy, 4, 1);
                Common.AppLib.maxDate = new DateTime(yy + 1, 3, 31);

                txtPassword.Password = string.Empty;
                App.frmHome.ShowDialog();
                this.Show();
                cmbFund.Focus();
            }
            else
            {
                MessageBox.Show(RValue);
            }
        }
        
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            cmbFund.Text = string.Empty;
            txtUserId.Text = string.Empty;
            txtPassword.Password = string.Empty;

        }

        private void txtPassword_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show(Message.PL.Login_Exit_Confirm, this.Title, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }

        private void btnAddNewFund_Click(object sender, RoutedEventArgs e)
        {
            frmNewFund f = new frmNewFund();
            f.data.Clear();                  
            f.ShowDialog();
        }

        private void cmbFund_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                var f = cmbFund.SelectedItem as BLL.FundMaster;
                cmbYear.ItemsSource = BLL.FundMaster.AcYearList(f.Id);
                cmbYear.SelectedIndex = cmbFund.Items.Count - 1;
            }
            catch (Exception ex)
            {

            }
        }
    }
}
