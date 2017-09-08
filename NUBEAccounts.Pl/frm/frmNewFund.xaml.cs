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

namespace NUBEAccounts.Pl.frm
{
    /// <summary>
    /// Interaction logic for frmNewFund.xaml
    /// </summary>
    public partial class frmNewFund : MetroWindow
    {
        public BLL.CompanyDetail data = new BLL.CompanyDetail();
        public bool IsForcedClose = false;
        public frmNewFund()
        {
            InitializeComponent();
            this.DataContext = data;
            IsForcedClose = false;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
            txtPassword.Password = "";
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Save() == true)
            {
                MessageBox.Show(Message.PL.Saved_Alert);
                BLL.UserType.Init();
                BLL.UserAccount.Init();
                IsForcedClose = true;
                Close();
            }
            else
            {
                MessageBox.Show(string.Join("\n", data.lstValidation.Select(x => x.Message).ToList()));
            }
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

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            data.Password = txtPassword.Password;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsForcedClose == false && MessageBox.Show("Are you sure to close the signup?", "Close", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }

      
    }
}
