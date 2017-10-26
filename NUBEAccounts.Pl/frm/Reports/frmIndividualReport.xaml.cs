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
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            List<BLL.IndividualReport> list = BLL.IndividualReport.ToList(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value,BLL.LedgerList.toList.Where(x=>x.IsChecked).Select(x=> x.LedgerId).ToList(),BLL.PayeeList.toList.Where(x => x.IsChecked).Select(x=> x.PayeeName).ToList());
            list = list.Select(x => new BLL.IndividualReport()
            { LedgerName = x.LedgerName, PayeeName = x.PayeeName, Amount = x.Amount }).ToList();

            try
            {
                rptViewer.Reset();
                ReportDataSource data = new ReportDataSource("IndividualReport", list);
                rptViewer.LocalReport.DataSources.Add(data);
                rptViewer.LocalReport.ReportPath = @"Reports\rptIndividualReport.rdlc";

                ReportParameter[] par = new ReportParameter[1];
                //par[0] = new ReportParameter("DateFrom", dtpDateFrom.SelectedDate.Value.ToString());
                //par[1] = new ReportParameter("DateTo", dtpDateTo.SelectedDate.Value.ToString());

                par[0] = new ReportParameter("Fund", BLL.UserAccount.User.UserType.Fund.FundName);

                rptViewer.LocalReport.SetParameters(par);

                rptViewer.RefreshReport();

            }
            catch(Exception ex)
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
    }
}

