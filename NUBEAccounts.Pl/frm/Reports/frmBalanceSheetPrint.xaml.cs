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

namespace NUBEAccounts.Pl.frm.Reports
{
    /// <summary>
    /// Interaction logic for frmBalanceSheetPrint.xaml
    /// </summary>
    public partial class frmBalanceSheetPrint : MetroWindow
    {
        public static int yy = BLL.UserAccount.User.UserType.Fund.LoginAccYear;

        DateTime? dtFrom = new DateTime(yy, 4, 1);
        DateTime? dtTo = new DateTime(yy + 1, 3, 31);

        public frmBalanceSheetPrint()
        {
            InitializeComponent();
            RptViewer.SetDisplayMode(DisplayMode.PrintLayout);

            LoadReport(Convert.ToDateTime(dtFrom), Convert.ToDateTime(dtTo)); ;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReport(Convert.ToDateTime(dtFrom), Convert.ToDateTime(dtTo)); ;

        }

        public void LoadReport(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.BalanceSheet> list = BLL.BalanceSheet.ToList(dtFrom, dtTo);
            list = list.Select(x => new BLL.BalanceSheet()
            { Ledger = x.LedgerList.AccountName, CrAmt = x.CrAmt, DrAmt = x.DrAmt, CrAmtOP = x.CrAmtOP, DrAmtOP = x.DrAmtOP }).ToList();
            try
            {
                RptViewer.Reset();
                ReportDataSource data = new ReportDataSource("BalanceSheet", list);
                ReportDataSource data1 = new ReportDataSource("FundMaster", BLL.FundMaster.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Fund.Id).ToList());
                RptViewer.LocalReport.DataSources.Add(data);
                RptViewer.LocalReport.DataSources.Add(data1);
                RptViewer.LocalReport.ReportPath = @"Reports\rptBalancesheet.rdlc";

                ReportParameter[] par = new ReportParameter[3];
                par[0] = new ReportParameter("DateFrom", dtFrom.ToString());
                par[1] = new ReportParameter("DateTo", dtTo.ToString());
                par[2] = new ReportParameter("Fund", BLL.UserAccount.User.UserType.Fund.FundName.ToString());
                RptViewer.LocalReport.SetParameters(par);


                RptViewer.RefreshReport();

            }
            catch (Exception ex)
            {

            }

        }
    }
}
