using System;
using System.Collections.Generic;
using System.Data;
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
using Microsoft.Reporting.WinForms;

namespace NUBEAccounts.Pl.frm.Reports
{
    /// <summary>
    /// Interaction logic for frmIndividualReport.xaml
    /// </summary>
    public partial class frmIndividualReport : UserControl
    {        
        
        BLL.IndividualReport data = new BLL.IndividualReport();
        public frmIndividualReport()
        {
            InitializeComponent();
            this.DataContext = data;
            dtpDateFrom.SelectedDate = DateTime.Now;
            dtpDateTo.SelectedDate = DateTime.Now;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvLedger.ItemsSource = BLL.LedgerList.toList;
            dgvPayeeName.ItemsSource = BLL.PayeeList.toList;

            CollectionViewSource.GetDefaultView(dgvLedger.ItemsSource).Filter = Ledger_Filter;
            CollectionViewSource.GetDefaultView(dgvLedger.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.LedgerName), System.ComponentModel.ListSortDirection.Ascending));
            CollectionViewSource.GetDefaultView(dgvPayeeName.ItemsSource).Filter = PayeeName_Filter;
            CollectionViewSource.GetDefaultView(dgvPayeeName.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.PayeeName), System.ComponentModel.ListSortDirection.Ascending));


        }
        private bool Ledger_Filter(object obj)
        {
            bool RValue = false;
            var d = obj as BLL.LedgerList;
            
            if (!string.IsNullOrEmpty(txtLedgerName.Text))
            {               
                if((d.LedgerName.ToLower()).Contains(txtLedgerName.Text.ToLower())) RValue = true;
            }
            else
            {
                RValue = true;
            }
            return RValue;
        }
        private bool PayeeName_Filter(object obj)
        {
            bool RValue = false;
            var d = obj as BLL.PayeeList;

            if (!string.IsNullOrEmpty(txtPayeeName.Text))
            {
                if ((d.PayeeName.ToLower()).Contains(txtPayeeName.Text.ToLower())) RValue = true;
            }
            else
            {
                RValue = true;
            }
            return RValue;
        }
        private void Ledger_Grid_Refresh()
        {
            try
            {
                CollectionViewSource.GetDefaultView(dgvLedger.ItemsSource).Refresh();
            }
            catch (Exception ex) { };

        }
        private void PayeeName_Grid_Refresh()
        {
            try
            {
                CollectionViewSource.GetDefaultView(dgvPayeeName.ItemsSource).Refresh();
            }
            catch (Exception ex) { };

        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            
            try
            {
                List<BLL.IndividualReport> list = BLL.IndividualReport.ToList(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, BLL.LedgerList.toList.Where(x => x.IsChecked).Select(x => x.LedgerId).ToList(), BLL.PayeeList.toList.Where(x => x.IsChecked).Select(x => x.PayeeName).ToList());
                list = list.Select(x => new BLL.IndividualReport()
                { LedgerName = x.LedgerName, PayeeName = x.PayeeName, Amount = x.Amount }).ToList();
                rptViewer.Reset();
                ReportDataSource data = new ReportDataSource("IndividualReport", list);
                rptViewer.LocalReport.DataSources.Add(data);
                if(rdbInverse.IsChecked==false)
                {
                    rptViewer.LocalReport.ReportPath = @"Reports\rptIndividualReportPayee.rdlc";
                }
                else
                {
                    rptViewer.LocalReport.ReportPath = @"Reports\rptIndividualReport.rdlc";
                }

                ReportParameter[] par = new ReportParameter[3];
                par[0] = new ReportParameter("DateFrom", dtpDateFrom.SelectedDate.Value.ToString());
                par[1] = new ReportParameter("DateTo", dtpDateTo.SelectedDate.Value.ToString());
                par[2] = new ReportParameter("Fund", BLL.UserAccount.User.UserType.Fund.FundName);

                rptViewer.LocalReport.SetParameters(par);

                rptViewer.RefreshReport();

            }
            catch (Exception ex)
            { }
        }

    

    private void chkLedgerName_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.LedgerList;
                if (d != null) d.IsChecked = true;                
            }
            catch (Exception ex) { }
        }

        private void chkLedgerName_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.LedgerList;
                if (d != null) d.IsChecked = false;
            }
            catch (Exception ex) { }
        }            
       private void chkPayName_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.PayeeList;
                if (d != null) d.IsChecked = false;
            }
            catch (Exception ex) { }
        }

        private void chkPayName_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = ((CheckBox)sender).Tag as BLL.PayeeList;
                if (d != null) d.IsChecked = true;
            }
            catch (Exception ex) { }
        }

        private void txtPayeeName_TextChanged(object sender, TextChangedEventArgs e)
        {
            PayeeName_Grid_Refresh();
        }

        private void txtLedgerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Ledger_Grid_Refresh();
        }

        private void Inverse_Checked(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void rdbNoInverse_Checked(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }
    }
}

