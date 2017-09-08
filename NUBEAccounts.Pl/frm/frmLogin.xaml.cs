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
using MahApps.Metro.Controls;
using Microsoft.AspNet.SignalR.Client;

namespace NUBEAccounts.Pl.frm
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmLogin : MetroWindow
    {

        public frmLogin()
        {
            InitializeComponent();
            var l1 = BLL.CompanyDetail.toList;
            cmbFund.ItemsSource = l1;
            cmbFund.SelectedValuePath = "Id";
            cmbFund.DisplayMemberPath = "CompanyName";

            cmbYear.ItemsSource = BLL.CompanyDetail.AcYearList;
            cmbYear.SelectedIndex = BLL.CompanyDetail.AcYearList.Count() - 1;

            onClientEvents();

        }
        private void onClientEvents()
        {
            BLL.NubeAccountClient.NubeAccountHub.On<BLL.CompanyDetail>("CompanyDetail_Save", (cs) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    cs.Save(true);
                });

            });
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

            string RValue = BLL.UserAccount.Login(cmbYear.Text, cmbFund.Text, txtUserId.Text, txtPassword.Password);

            if (RValue == "")
            {
                App.frmHome = new frmHome();
                App.frmHome.Title = String.Format("NUBE ACCOUNTS : {0} - {1} [ {2} ]", BLL.UserAccount.User.UserName, BLL.UserAccount.User.UserType.Company.CompanyName,BLL.UserAccount.LoginedACYear);
                this.Hide();
                cmbFund.Text = "";
                txtUserId.Text = "";
                txtPassword.Password = "";
                App.frmHome.ShowDialog();
                this.Show();
                cmbFund.Focus();
            }
            else
            {
                MessageBox.Show(RValue);
            }
        }

        private void btnSignup_Click(object sender, RoutedEventArgs e)
        {
           // frmCompanySignup f = new frmCompanySignup();
           // f.ShowDialog();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            cmbFund.Text = "";
            txtUserId.Text = "";
            txtPassword.Password = "";

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
            if (MessageBox.Show("Are you sure to Exit?", "Exit", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }

        

        private void btnAddNewFund_Click(object sender, RoutedEventArgs e)
        {
            frmNewFund f = new frmNewFund();
            f.data.Clear();
            f.data.CompanyType = "Company";         
            f.ShowDialog();
         

        }
    }
}
