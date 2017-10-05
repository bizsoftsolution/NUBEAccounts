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
    /// Interaction logic for frmIncomeAndExpenditurePrint.xaml
    /// </summary>
    public partial class frmIncomeAndExpenditurePrint : MetroWindow
    {
        public static int yy = BLL.UserAccount.User.UserType.Fund.LoginAccYear;

        DateTime? dtFrom = new DateTime(yy, 4, 1);
        DateTime? dtTo = new DateTime(yy + 1, 3, 31);

        public frmIncomeAndExpenditurePrint()
        {
            InitializeComponent();
            rptViewer.SetDisplayMode(DisplayMode.PrintLayout);

            LoadReport(Convert.ToDateTime(dtFrom), Convert.ToDateTime(dtTo)); ;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReport(Convert.ToDateTime(dtFrom), Convert.ToDateTime(dtTo)); ;

        }

        public void LoadReport(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.IncomeExpenditure> list = BLL.IncomeExpenditure.ToList(dtFrom, dtTo);
            list = list.Select(x => new BLL.IncomeExpenditure()
            { AccountName = x.Ledger.AccountName, CrAmt = x.CrAmt, DrAmt = x.DrAmt, DrAmtOP = x.DrAmtOP, CrAmtOP = x.CrAmtOP }).ToList();

            try
            {
                rptViewer.Reset();
                ReportDataSource data = new ReportDataSource("IncomeExpenditure", list);
                ReportDataSource data1 = new ReportDataSource("FundMaster", BLL.FundMaster.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Fund.Id).ToList());
                rptViewer.LocalReport.DataSources.Add(data);
                rptViewer.LocalReport.DataSources.Add(data1);
                rptViewer.LocalReport.ReportPath = @"Reports\rptIncomeAndExpenditure.rdlc";

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
