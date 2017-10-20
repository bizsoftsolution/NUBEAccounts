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
    /// Interaction logic for frmTrialBalancePrint.xaml
    /// </summary>
    public partial class frmTrialBalancePrint : MetroWindow
    {
        public static int yy = BLL.UserAccount.User.UserType.Fund.LoginAccYear;

       
        public frmTrialBalancePrint()
        {
            InitializeComponent();
            rptViewer.SetDisplayMode(DisplayMode.PrintLayout);

            //LoadReport(Convert.ToDateTime(dtFrom), Convert.ToDateTime(dtTo)); ;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           // LoadReport(Convert.ToDateTime(dtFrom), Convert.ToDateTime(dtTo)); ;

        }

        public void LoadReport(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.TrialBalance> list = BLL.TrialBalance.ToList(dtFrom, dtTo);
            list = list.Select(x => new BLL.TrialBalance()
            { AccountName = x.Ledger.AccountName, CrAmt = x.CrAmt, DrAmt = x.DrAmt, CrAmtOP = x.CrAmtOP, DrAmtOP = x.DrAmtOP }).ToList();

            try
            {
                rptViewer.Reset();
                ReportDataSource data = new ReportDataSource("TrialBalance", list);               
                rptViewer.LocalReport.DataSources.Add(data);
                rptViewer.LocalReport.ReportPath = @"Reports\rptTrialBalance.rdlc";

                ReportParameter[] par = new ReportParameter[3];
                par[0] = new ReportParameter("DateFrom", dtFrom.ToString());
                par[1] = new ReportParameter("DateTo", dtTo.ToString());
                par[2] = new ReportParameter("Fund", BLL.UserAccount.User.UserType.Fund.FundName.ToString());
                rptViewer.LocalReport.SetParameters(par);

                rptViewer.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }

    }
}

