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
using Microsoft.Reporting.WinForms;
using NUBEAccounts.Common;

namespace NUBEAccounts.Pl.frm.Master
{
    /// <summary>
    /// Interaction logic for frmLedger.xaml
    /// </summary>
    public partial class frmLedger : UserControl
    {

        #region Field

        public static string FormName = "Ledger";
        BLL.Ledger data = new BLL.Ledger();

        #endregion

        #region Constructor

        public frmLedger()
        {
            InitializeComponent();
            this.DataContext = data;
            data.Clear();
            RptLedger.SetDisplayMode(DisplayMode.PrintLayout);
            onClientEvents();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //BLL.Ledger.Init();
            rptContain.IsChecked = true;
            dgvLedger.ItemsSource = BLL.Ledger.toList;

            CollectionViewSource.GetDefaultView(dgvLedger.ItemsSource).Filter = Ledger_Filter;
            CollectionViewSource.GetDefaultView(dgvLedger.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.AccountName), System.ComponentModel.ListSortDirection.Ascending));

            var AUGIds = BLL.AccountGroup.toList.Select(x => x.UnderGroupId).ToList();

            //cmbAccountGroupId.ItemsSource = BLL.AccountGroup.toList.Where(x => !AUGIds.Contains(x.Id)).ToList();
            cmbAccountGroupId.ItemsSource = BLL.AccountGroup.toList.ToList();
            cmbAccountGroupId.DisplayMemberPath = "GroupName";
            cmbAccountGroupId.SelectedValuePath = "Id";



            //cmbCreditLimitTypeId.ItemsSource = BLL.CreditLimitType.toList;
            //cmbCreditLimitTypeId.SelectedValuePath = "Id";
            //cmbCreditLimitTypeId.DisplayMemberPath = "LimitType";

            cmbAccountType.ItemsSource = BLL.Ledger.ACTypeList;

            btnSave.Visibility = (BLL.CompanyDetail.UserPermission.AllowInsert || BLL.CompanyDetail.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.CompanyDetail.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.LedgerName == null)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "LedgerName"));
            }
            else if (txtMail.Text != "" && !Common.AppLib.IsValidEmailAddress(txtMail.Text))
            {
                MessageBox.Show("Please Enter the Valid Email or Leave Empty");

            }
            else if (data.Id == 0 && !BLL.UserAccount.AllowInsert(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName));
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName));
            }
            else
            {
                if (data.Save() == true)
                {
                    MessageBox.Show(Message.PL.Saved_Alert);
                    data.Clear();
                    Grid_Refresh();
                }

                else
                {
                    MessageBox.Show(string.Format(Message.PL.Existing_Data, data.LedgerName));
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id != 0)
            {
                if (!BLL.UserAccount.AllowDelete(FormName))
                {
                    MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName));
                }
                else
                {
                    if (MessageBox.Show(Message.PL.Delete_confirmation, "", MessageBoxButton.YesNo) != MessageBoxResult.No)
                    {
                        if (data.Delete() == true)
                        {
                            MessageBox.Show(Message.PL.Delete_Alert);
                            data.Clear();
                            Grid_Refresh();
                        }
                        else
                        {
                            MessageBox.Show(Message.PL.Cant_Delete_Alert);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No Records to Delete");
            }


        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
        }

        private void dgvLedger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = dgvLedger.SelectedItem as BLL.Ledger;
            if (d != null)
            {
                data.Find(d.Id);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Grid_Refresh();
        }

        private void cbxCase_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptStartWith_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void cbxCase_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptContain_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptEndWith_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tc = sender as TabControl;

            if (tc.SelectedIndex == 1)
            {
                LoadReport();
            }

        }

        private void NumericOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = Common.AppLib.IsTextNumeric(e.Text);
        }

        #endregion

        #region Methods

        private bool Ledger_Filter(object obj)
        {
            bool RValue = false;
            var d = obj as BLL.Ledger;

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string strSearch = cbxCase.IsChecked == true ? txtSearch.Text : txtSearch.Text.ToLower();

                if (rptStartWith.IsChecked == true)
                {                    
                    if (
                            (cbxCase.IsChecked == true ? d.LedgerCode : d.LedgerCode.ToLower()).StartsWith(strSearch) ||
                            (cbxCase.IsChecked == true ? d.LedgerName : d.LedgerName.ToLower()).StartsWith(strSearch) ||
                            (cbxCase.IsChecked == true ? d.AccountGroup.GroupName : d.AccountGroup.GroupName.ToLower()).StartsWith(strSearch) 

                        ) RValue = true;                
                }
                else if (rptContain.IsChecked == true)
                {
                    if (
                           (cbxCase.IsChecked == true ? d.LedgerCode : d.LedgerCode.ToLower()).Contains(strSearch) ||
                           (cbxCase.IsChecked == true ? d.LedgerName : d.LedgerName.ToLower()).Contains(strSearch) ||
                           (cbxCase.IsChecked == true ? d.AccountGroup.GroupName : d.AccountGroup.GroupName.ToLower()).Contains(strSearch)

                       ) RValue = true;
                }
                else if (rptEndWith.IsChecked == true )
                {
                    if (
                           (cbxCase.IsChecked == true ? d.LedgerCode : d.LedgerCode.ToLower()).EndsWith(strSearch) ||
                           (cbxCase.IsChecked == true ? d.LedgerName : d.LedgerName.ToLower()).EndsWith(strSearch) ||
                           (cbxCase.IsChecked == true ? d.AccountGroup.GroupName : d.AccountGroup.GroupName.ToLower()).EndsWith(strSearch)

                       ) RValue = true;
                }
                
            }
            else
            {
                RValue = true;
            }
            return RValue;
        }

        private void Grid_Refresh()
        {
            try
            {
                CollectionViewSource.GetDefaultView(dgvLedger.ItemsSource).Refresh();
            }
            catch (Exception ex) { };

        }

        private void LoadReport()
        {
            try
            {
                RptLedger.Reset();
                ReportDataSource data = new ReportDataSource("Ledger", BLL.Ledger.toList.Where(x => Ledger_Filter(x)).Select(x => new { x.LedgerName, x.AddressLine1, x.AddressLine2, x.CityName, x.OPCr, x.OPDr,x.LedgerCode,PersonIncharge = x.AccountGroup.GroupName }).OrderBy(x => x.LedgerCode).ToList());
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Company.Id).ToList());
                RptLedger.LocalReport.DataSources.Add(data);
                RptLedger.LocalReport.DataSources.Add(data1);
                RptLedger.LocalReport.ReportPath = @"Master\RptLedger.rdlc";

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("Title", "LEDGER LIST");

                RptLedger.LocalReport.SetParameters(param);
                RptLedger.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }

        private void onClientEvents()
        {
            BLL.NubeAccountClient.NubeAccountHub.On<BLL.Ledger>("Ledger_Save", (led) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    led.Save(true);
                });

            });

            BLL.NubeAccountClient.NubeAccountHub.On("Ledger_Delete", (Action<int>)((pk) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    BLL.Ledger led = new BLL.Ledger();
                    led.Find((int)pk);
                    led.Delete((bool)true);
                }));

            }));
        }

        #endregion

        private void txtCreditAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
          //  textBox.Text = AppLib.NumericOnly(txtCreditAmount.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;
                       
        }

        private void rptStartWith_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptContain_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptEndWith_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void txtLedgerOP_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            textBox.Text = AppLib.NumericOnly(txtLedgerOP.Text);
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;

        }

        private void dgvLedger_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var d = dgvLedger.SelectedItem as BLL.Ledger;
            if (d != null)
            {
                data.Find(d.Id);
            }
        }

        private void txtMail_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            String newText = String.Empty;

            int AtCount = 0;
            foreach (Char c in textBox.Text.ToCharArray())
            {
                if (Char.IsLetterOrDigit(c) || Char.IsControl(c) || (c == '.' || c == '_') || (c == '@' && AtCount == 0))
                {
                    newText += c;
                    if (c == '@') AtCount += 1;
                }
            }
            textBox.Text = newText;
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;
        }

        private void txtMail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMail.Text != "" && !Common.AppLib.IsValidEmailAddress(txtMail.Text))
            {
                MessageBox.Show("Please Enter the Valid Email or Leave Empty");

            }

        }

      
    }
}
