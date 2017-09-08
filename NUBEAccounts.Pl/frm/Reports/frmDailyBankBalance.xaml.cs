using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Xceed.Utils.Collections;

namespace NUBEAccounts.Pl.frm.Reports
{
    /// <summary>
    /// Interaction logic for frmDailyBankBalance.xaml
    /// </summary>
    public partial class frmDailyBankBalance : UserControl
    {
          public frmDailyBankBalance()
        {
            InitializeComponent();
            int yy = BLL.UserAccount.User.UserType.Company.LoginAccYear;

            DateTime? dtFrom = new DateTime(yy, 4, 1);
            DateTime? dtTo = new DateTime(yy + 1, 3, 31);

            dtpDateFrom.SelectedDate = DateTime.Now;
            dtpDateTo.SelectedDate = DateTime.Now;

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //SetHeading(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
        }
        void SetHeading(DateTime dtFrom, DateTime dtTo)
        {
            var lst = BLL.Ledger.toList.Where(x => x.AccountGroup.GroupName == BLL.DataKeyValue.BankAccounts_Key).OrderBy(x=>x.AccountGroup.GroupCode).ToList();
            int i = 0;
            foreach (var l in lst)
            {
                
                dgvDetails.Columns[i + 1].Header = string.Format("{0}-{1}", l.AccountGroup.GroupCode, l.LedgerName);
                dgvDetails.Columns[i + 1].Visibility = Visibility.Visible;
                i++;
            }
            

        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            busyIndicator.IsBusy = true;
            var l1 = BLL.DailyBankBalance.ToList(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
            var l2 = l1.Select(x => x.Ledger.LedgerName).Distinct();
            var l3 = l1.Select(x => x.Date).Distinct();

           BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
               
                DataTable dt = new DataTable();
                dt.Columns.Add("Date");
                int j = 1;
                foreach (var str in l2)
                {

                    dt.Columns.Add(string.Format("Bank{0}", j++));
                }
                dt.Columns.Add("Total");

                foreach (var d in l3)
                {
                    string[] arr = new string[l2.Count() + 2];
                    arr[0] = string.Format("{0:dd/MM/yyyy}", d);
                    int i = 1;
                    decimal Tot = 0, Amount = 0;
                    foreach (var b in l2)
                    {
                        Amount = l1.Where(x => x.Date == d && x.Ledger.LedgerName == b).FirstOrDefault().Amount.Value;
                        arr[i] = Amount.ToString();
                        Tot += Amount;
                        i++;
                    }
                    arr[i] = Tot.ToString();
                    dt.Rows.Add(arr);
                }

                Dispatcher.Invoke((Action)(() => dgvDetails.ItemsSource = dt.AsDataView()));
              
            };
            int k = 1;
            foreach (var b in l2)
            {
                dgvDetails.Columns[k++].Header = b;
            }
            worker.RunWorkerCompleted += (o, ea) =>
            {
                busyIndicator.IsBusy = false;
            };
            busyIndicator.IsBusy = true;
            worker.RunWorkerAsync();
                                                      
        }

    }
}
