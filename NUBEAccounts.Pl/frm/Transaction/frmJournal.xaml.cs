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
    /// Interaction logic for frmJournal.xaml
    /// </summary>
    public partial class frmJournal : UserControl
    {
        public BLL.Journal data = new BLL.Journal();
        public string FormName = "Journal";
        decimal drAmt = 0, crAmt = 0, DiffAmt = 0;
        public frmJournal()
        {
            InitializeComponent();
            this.DataContext = data;
            data.Clear();
            onClientEvents();
        }
        private void onClientEvents()
        {
            BLL.NubeAccountClient.NubeAccountHub.On<String>("Journal_RefNoRefresh", (VoucherNo) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    data.VoucherNo = VoucherNo;
                });
            });
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (data.JDetail.LedgerId == 0)
            {
                MessageBox.Show("Enter LedgerName");
            }
            else if (data.JDetail.DrAmt == 0 && data.JDetail.CrAmt == 0)
            {
                MessageBox.Show("Enter Amount Dr or Amount Cr");
            }
            else
            {
                data.SaveDetail();
                FindDiff();
            }
        }

        private void FindDiff()
        {
            var l1 = data.JDetails;
            drAmt = l1.Sum(x => x.CrAmt);
            crAmt = l1.Sum(x => x.DrAmt);

            lblMsg.Text = string.Format("Total Debit Balance : {0:N2}, Total Credit Balance : {1:N2}, Difference : {2:N2}", drAmt, crAmt, Math.Abs(drAmt - crAmt));
            lblMsg.Foreground = drAmt == crAmt ? new SolidColorBrush(Color.FromRgb(0, 0, 255)) : new SolidColorBrush(Color.FromRgb(255, 0, 0));
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DiffAmt = Math.Abs(drAmt - crAmt);
            if (data.Id == 0 && !BLL.UserAccount.AllowInsert(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName));
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName));
            }
            else if (data.VoucherNo == null)
            {
                MessageBox.Show("Enter Entry No");
            }
            else if (data.JDetails.Count == 0)
            {
                MessageBox.Show("Enter Details");

            }
            else if (DiffAmt != 0)
            {
                MessageBox.Show("Difference between Credit and Debit Should be Zero");

            }
            else if (data.FindEntryNo())
            {
                MessageBox.Show("Entry No Already Exist!..");
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
            lblMsg.Text = "";
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
            frmJournalSearch frm = new frmJournalSearch();
            frm.ShowDialog();
            frm.Close();
        }

        private void dgvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BLL.JournalDetail pod = dgvDetails.SelectedItem as BLL.JournalDetail;
                pod.toCopy<BLL.JournalDetail>(data.JDetail);
            }
            catch (Exception ex) { }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Print();
        }

        private void Print()
        {
            frm.Vouchers.frmQuickJournalVoucher f = new Vouchers.frmQuickJournalVoucher();

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
                    FindDiff();
                }
            }
            catch (Exception ex) { }

        }

        private void btnDClear_Click(object sender, RoutedEventArgs e)
        {
            data.ClearDetail();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            btnSave.Visibility = (BLL.CompanyDetail.UserPermission.AllowInsert || BLL.CompanyDetail.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.CompanyDetail.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

            data.Clear();


            cmbDebitAC.ItemsSource = BLL.Ledger.toList;
            cmbDebitAC.SelectedValuePath = "Id";
            cmbDebitAC.DisplayMemberPath = "AccountName";

            if (data.Id != 0)
            {
                btnPrint.IsEnabled = true;
            }
            else
            {
                btnPrint.IsEnabled = false;
            }

        }

        private void dtpJournalDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            data.SetEntryNo();
        }

        private void txtAmountDr_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtAmountDr.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void txtAmountCr_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtAmountCr.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }
    }
}
