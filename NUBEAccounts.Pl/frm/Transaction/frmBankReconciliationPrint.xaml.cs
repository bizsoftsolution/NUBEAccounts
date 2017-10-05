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
using Microsoft.Reporting.WinForms;

namespace NUBEAccounts.Pl.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmBankReconciliationPrint.xaml
    /// </summary>
    public partial class frmBankReconciliationPrint : MetroWindow
    {
        public frmBankReconciliationPrint()
        {
            InitializeComponent();
            RptViewer.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(int LID, DateTime dtFrom, DateTime dtTo, string endBal, string diffAmt, string BalAmt)
        {
            try
            {
                List<BLL.BankReconcilation> list = BLL.BankReconcilation.ToList((int)LID, dtFrom, dtTo);
                list = list.Select(x => new BLL.BankReconcilation()
                { AccountName = x.Ledger.AccountName, Particular = x.Particular, CrAmt = x.CrAmt, DrAmt = x.DrAmt,  EDate = x.EDate, EntryNo = x.EntryNo, EType = x.EType, Ledger = x.Ledger, RefNo = x.RefNo }).ToList();

                try
                {
                    RptViewer.Reset();
                    ReportDataSource data = new ReportDataSource("BankReconciliation", list);
                    RptViewer.LocalReport.DataSources.Add(data);
                    RptViewer.LocalReport.ReportPath = @"Transaction\rptBankReconciiation.rdlc";

                    ReportParameter[] par = new ReportParameter[6];
                    par[0] = new ReportParameter("FromDate", dtFrom.ToString());
                    par[1] = new ReportParameter("ToDate", dtTo.ToString());
                    par[2] = new ReportParameter("Fund",BLL.UserAccount.User.UserType.Fund.FundName);
                    par[3] = new ReportParameter("EndingBalance", endBal.ToString());
                    par[4] = new ReportParameter("DiffAmt", diffAmt.ToString());
                    par[5] = new ReportParameter("BalAmt", BalAmt.ToString());
                    RptViewer.LocalReport.SetParameters(par);

                    RptViewer.RefreshReport();

                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
