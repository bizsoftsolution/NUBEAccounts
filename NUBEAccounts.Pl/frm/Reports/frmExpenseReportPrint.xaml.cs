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
    /// Interaction logic for frmExpenseReportPrint.xaml
    /// </summary>
    public partial class frmExpenseReportPrint : MetroWindow
    {
        public static int yy = BLL.UserAccount.User.UserType.Fund.LoginAccYear;

        DateTime? dtFrom = new DateTime(yy, 4, 1);
        DateTime? dtTo = new DateTime(yy + 1, 3, 31);

        public frmExpenseReportPrint()
        {
            InitializeComponent();
            RptViewer.SetDisplayMode(DisplayMode.PrintLayout);


        }
        public void LoadReport(int? LID, DateTime dtFrom, DateTime dtTo, String EntryNo, String Payto,bool AccountHead)
        {
            try
            {
                List<BLL.ExpenseReport> list = new List<BLL.ExpenseReport>();
                if (AccountHead==true)
                {
                    list = BLL.ExpenseReport.ToList((int?)LID, dtFrom, dtTo, EntryNo, Payto,true);

                }
                else
                {
                    list = BLL.ExpenseReport.ToList((int?)LID, dtFrom, dtTo, EntryNo, Payto, false);

                }

                try
                {
                    RptViewer.Reset();
                    ReportDataSource data = new ReportDataSource("ExpenseReport", list);
                    ReportDataSource data1 = new ReportDataSource("FundMaster", BLL.FundMaster.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.FundMasterId).ToList());
                    RptViewer.LocalReport.DataSources.Add(data);
                    RptViewer.LocalReport.DataSources.Add(data1);
                    RptViewer.LocalReport.ReportPath = @"Reports\rptExpenseReport.rdlc";

                    ReportParameter[] par = new ReportParameter[2];
                    par[0] = new ReportParameter("DateFrom", dtFrom.ToString());
                    par[1] = new ReportParameter("DateTo", dtTo.ToString());
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
