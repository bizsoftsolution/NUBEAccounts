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

namespace NUBEAccounts.Pl.frm.Vouchers
{
    /// <summary>
    /// Interaction logic for frmQuickJournalVoucher.xaml
    /// </summary>
    public partial class frmQuickJournalVoucher : MetroWindow
    {
        public frmQuickJournalVoucher()
        {
            InitializeComponent();
            rptQuickJournal.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.Journal data)
        {
            try
            {

                List<BLL.Journal> JList = new List<BLL.Journal>();
                List<BLL.FundMaster> CList = new List<BLL.FundMaster>();
                List<BLL.JournalDetail> JDList = new List<BLL.JournalDetail>();

                JList.Add(data);
                JDList.Add(data.JDetail);
                CList.Add(BLL.UserAccount.User.UserType.Fund);


                rptQuickJournal.Reset();
                ReportDataSource data1 = new ReportDataSource("Journal", JList);
                ReportDataSource data2 = new ReportDataSource("FundMaster", CList);
                ReportDataSource data3 = new ReportDataSource("JDetails", data.JDetails);

                rptQuickJournal.LocalReport.DataSources.Add(data1);
                rptQuickJournal.LocalReport.DataSources.Add(data2);
                rptQuickJournal.LocalReport.DataSources.Add(data3);
                rptQuickJournal.LocalReport.ReportPath = @"Transaction\rptJournalReceipt.rdlc";

                ReportParameter[] par = new ReportParameter[1];
               par[0] = new ReportParameter("Fund", BLL.UserAccount.User.UserType.Fund.FundName);
                rptQuickJournal.LocalReport.SetParameters(par);


                rptQuickJournal.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }
    }
}
