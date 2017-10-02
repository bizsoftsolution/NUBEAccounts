using MahApps.Metro.Controls;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NUBEAccounts.Pl.frm
{
    /// <summary>
    /// Interaction logic for frmNewFund.xaml
    /// </summary>
    public partial class frmNewFund : MetroWindow
    {
        public BLL.FundMaster data = new BLL.FundMaster();
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
            txtPassword.Password = string.Empty;
            txtFund.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Save() == true)
            {
                MessageBox.Show(Message.PL.Saved_Alert);                
                IsForcedClose = true;
                Close();
            }
            else
            {
                MessageBox.Show(string.Join(Message.PL.NewLine, data.lstValidation.Select(x => x.Message).ToList()));
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
            if (IsForcedClose == false && MessageBox.Show(Message.PL.NewFund_Exit_Confirm, this.Title, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }        
    }
}
