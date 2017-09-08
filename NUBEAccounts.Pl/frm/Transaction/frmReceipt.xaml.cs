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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNet.SignalR.Client;
using NUBEAccounts.Common;

namespace NUBEAccounts.Pl.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmReceipt.xaml
    /// </summary>
    public partial class frmReceipt : UserControl
    {
        public BLL.Receipt data = new BLL.Receipt();
        public String FormName = "Receipt";
        public frmReceipt()
        {
            InitializeComponent();
            this.DataContext = data;
            data.Clear();
            onClientEvents();
        }
        private void onClientEvents()
        {
            BLL.NubeAccountClient.NubeAccountHub.On<String>("Receipt_RefNoRefresh", (RefNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    data.RefNo = RefNo;
                });
            });
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (data.RDetail.LedgerId == 0)
            {
                MessageBox.Show("Enter LedgerName");
            }
            else if (data.RDetail.Amount == 0)
            {
                MessageBox.Show("Enter Amount");
            }
            else
            {
                data.SaveDetail();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id == 0 && !BLL.UserAccount.AllowInsert(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName));
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName));
            }
            else if (data.EntryNo == null)
            {
                MessageBox.Show("Enter Entry No");
            }
            else if (data.LedgerId == 0)
            {
                MessageBox.Show("Enter LedgerName");
            }
            else if (data.ReceiptMode == null)
            {
                MessageBox.Show("select Paymode");
            }

            else if (data.RDetails.Count == 0)
            {
                MessageBox.Show("Enter Receipt");
            }
            else if (data.FindEntryNo())
            {
                MessageBox.Show("Entry No Already Exist");
            }
            else
            {
                var rv = data.Save();
                if (rv == true)
                {
                    MessageBox.Show(Message.PL.Saved_Alert);
                    if (ckxAutoPrint.IsChecked == true) Print();
                    data.Clear();
                }
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!BLL.UserAccount.AllowDelete(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName));
            }
            else
            {
                if (MessageBox.Show("Do you want to delete?", "DELETE", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var rv = data.Delete();
                    if (rv == true)
                    {
                        MessageBox.Show(Message.PL.Delete_Alert);
                        data.Clear();
                        if (data.Id != 0)
                        {
                            btnPrint.IsEnabled = true;
                        }
                        else
                        {
                            btnPrint.IsEnabled = false;
                        }
                    }
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
            if (data.Id != 0)
            {
                btnPrint.IsEnabled = true;
            }
            else
            {
                btnPrint.IsEnabled = false;
            }
        }

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            //var rv = data.Find();
            //if (data.Id != 0)
            //{
            //    btnPrint.IsEnabled = true;
            //}
            //if (data.RefCode != null)
            //{
            //    btnSave.IsEnabled = true;
            //    btnDelete.IsEnabled = true;
            //}
            //if (rv == false) MessageBox.Show(String.Format("Data Not Found"));
            frmReceiptSearch frm = new frmReceiptSearch();
            frm.ShowDialog();
            frm.Close();
        }

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.ReceiptDetail pod = dgvDetails.SelectedItem as BLL.ReceiptDetail;
                pod.toCopy<BLL.ReceiptDetail>(data.RDetail);
            }
            catch (Exception ex) { }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {

            Print();
        }

        private void Print()
        {

            frm.Vouchers.frmQuickReceipt f = new Vouchers.frmQuickReceipt();

            f.LoadReport(data);
            f.ShowDialog();
        }
        private void btnEditDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                data.FindDetail((int)btn.Tag);
            }
            catch (Exception ex) { }

        }

        private void btnDeleteDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("do you want to delete this detail?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Button btn = (Button)sender;
                    data.DeleteDetail((int)btn.Tag);
                }
            }
            catch (Exception ex) { }

        }

        private void btnDClear_Click(object sender, RoutedEventArgs e)
        {
            data.ClearDetail();
        }

        private void txtAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtAmount.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void txtChequeNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtChequeNo.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            cmbDebitAC.ItemsSource = BLL.Ledger.toList;
            cmbDebitAC.SelectedValuePath = "Id";
            cmbDebitAC.DisplayMemberPath = "AccountName";

            cmbCreditAC.ItemsSource = BLL.Ledger.toList;
            cmbCreditAC.SelectedValuePath = "Id";
            cmbCreditAC.DisplayMemberPath = "AccountName";

            btnSave.Visibility = (BLL.CompanyDetail.UserPermission.AllowInsert || BLL.CompanyDetail.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.CompanyDetail.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

            data.Clear();

            if (data.Id != 0)
            {
                btnPrint.IsEnabled = true;
            }
            else
            {
                btnPrint.IsEnabled = false;
            }

        }

        private void dtpReceiptDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            data.SetEntryNo();
        }
    }
}
