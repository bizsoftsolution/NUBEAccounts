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


namespace NUBEAccounts.Pl.frm.Reports
{
    /// <summary>
    /// Interaction logic for frmIndividualReport.xaml
    /// </summary>
    public partial class frmIndividualReport : UserControl
    {
        public List<LedgerListItem> _checked;
        public frmIndividualReport()
        {
            InitializeComponent();

            dtpDateFrom.SelectedDate = DateTime.Now;
            dtpDateTo.SelectedDate = DateTime.Now;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            lstLedgerName.ItemsSource = BLL.Ledger.toList;
            lstPayName.ItemsSource = BLL.PayeeList.PayList;        
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
          
            foreach (var item in _checked)
            {
                DataTable dt = new DataTable();
                
                    var l1 = BLL.DailyBankBalance.ToList_Individual(dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, (int)item.LedgerId, item.LedgerName);
                    var l2 = l1.Select(x => x.PayName).Distinct();
                    var l3 = l1.Select(x => x.Amount).Distinct();
                    dt.Columns.Add("SNo");
                    int j = 1;
                    foreach (var str in l2)
                    {
                        dt.Columns.Add(string.Format("Bank{0}", j++));
                    }
                    dt.Columns.Add("Total");

                    foreach (var d in l3)
                    {
                        string[] arr = new string[l2.Count() + 2];
                        arr[0] = string.Format("{0}", d + 1);
                        int i = 1;
                        decimal Tot = 0, Amount = 0;
                        foreach (var b in l2)
                        {
                            Amount = l1.Where(x => x.Amount == d && x.Ledger.LedgerName == b).FirstOrDefault().Amount.Value;
                            arr[i] = Amount.ToString();
                            Tot += Amount;
                            i++;
                        }
                        arr[i] = Tot.ToString();
                        dt.Rows.Add(arr);
                    }
                    int k = 1;
                    foreach (var b in l2)
                    {
                        dgvDetails.Columns[k++].Header = b;
                    }
                
                dgvDetails.ItemsSource = dt.AsDataView();

            }







        }

       
    }
}
public class LedgerListItem
{

  
    public int LedgerId { get; set; }
    public string LedgerName { get; set; }
    public bool IsChecked { get; set; }
}
