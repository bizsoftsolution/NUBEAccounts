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
    /// Interaction logic for frmVoucherReportPrint.xaml
    /// </summary>
    public partial class frmVoucherReportPrint : MetroWindow
    {
        public static int yy = BLL.UserAccount.User.UserType.Company.LoginAccYear;

        DateTime? dtFrom = new DateTime(yy, 4, 1);
        DateTime? dtTo = new DateTime(yy + 1, 3, 31);

        public frmVoucherReportPrint()
        {
            InitializeComponent();
            RptViewer.SetDisplayMode(DisplayMode.PrintLayout);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {


        }
        public void LoadReport(List<BLL.VoucherReport> list)
        {
            try
            {
                RptViewer.Reset();
                ReportDataSource data = new ReportDataSource("VoucherReport", list);
                RptViewer.LocalReport.DataSources.Add(data);
                RptViewer.LocalReport.ReportPath = @"Reports\rptVoucherReport.rdlc";

                ReportParameter[] par = new ReportParameter[3];
                par[0] = new ReportParameter("DateFrom", dtFrom.ToString());
                par[1] = new ReportParameter("DateTo", dtTo.ToString());
                par[2] = new ReportParameter("Fund", BLL.UserAccount.User.UserType.Company.CompanyName);

                RptViewer.LocalReport.SetParameters(par);

                RptViewer.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }

    }
}
