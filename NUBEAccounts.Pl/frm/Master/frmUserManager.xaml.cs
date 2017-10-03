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
using NUBEAccounts.Common;
using Microsoft.AspNet.SignalR.Client;
using MahApps.Metro.Controls;

namespace NUBEAccounts.Pl.frm.Master
{
    /// <summary>
    /// Interaction logic for frmUserManager.xaml
    /// </summary>
    public partial class frmUserManager : MetroWindow
    {

        public frmUserManager()
        {
            InitializeComponent();
            onClientEvents();            
        }


        private void onClientEvents()
        {


            BLL.NubeAccountClient.NubeAccountHub.On<BLL.UserAccount>("UserAccount_Save", (ua) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    BLL.UserAccount u = new BLL.UserAccount();
                    ua.toCopy<BLL.UserAccount>(u);
                    BLL.UserAccount.toList.Add(u);
                });

            });
        }
        private void btnNewUser_Click(object sender, RoutedEventArgs e)
        {
            frmUser f = new frmUser();
            f.LoadWindow();
            f.ShowDialog();
            LoadWindow();

        }

        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            var u = dgvUsers.SelectedItem as BLL.UserAccount;

            frmUser f = new frmUser();
            f.LoadWindow();
            u.toCopy<BLL.UserAccount>(f.data);             
            f.ShowDialog();
            LoadWindow();

        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var u = dgvUsers.SelectedItem as BLL.UserAccount;
            if (u != null)
            {
                if (BLL.UserAccount.toList.Count() == 1)
                {
                    MessageBox.Show(string.Format("You can not delete this user. atleast one user required"));
                }
                else if (MessageBox.Show("Do you Delete this?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (u.Delete() == true)
                    {
                        MessageBox.Show(Message.PL.Delete_Alert);
                        LoadWindow();
                    }
                }
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
         
        }

        public void LoadWindow()
        {                       
            dgvUsers.ItemsSource = BLL.UserAccount.toList;
        }

    }
}
