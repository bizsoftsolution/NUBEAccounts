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
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Reporting.WinForms;


namespace NUBEAccounts.Pl.frm.Master
{
    /// <summary>
    /// Interaction logic for frmAccountGroup.xaml
    /// </summary>
    public partial class frmAccountGroup : UserControl
    {
        #region Field

        public static string FormName = "Account Group";
        BLL.AccountGroup data = new BLL.AccountGroup();
    
        #endregion

        #region Constructor

        public frmAccountGroup()
        {
            InitializeComponent();
            this.DataContext = data;
            data.Clear();
            rptAccountGroup.SetDisplayMode(DisplayMode.PrintLayout);
        
            onClientEvents();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWindow();
            clear();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.GroupName == null)
            {
                MessageBox.Show(String.Format(Message.BLL.Required_Data, "Group Name"));
            }
            else if (data.Id == 0 && !BLL.UserAccount.AllowInsert(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName));
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName));
            }
            else
            {
                if (data.Save() == true)
                {
                    MessageBox.Show(Message.PL.Saved_Alert);
                    clear();
                    Grid_Refresh();
                }
                else
                {
                    MessageBox.Show(string.Format(Message.PL.Existing_Data, data.GroupName));
                }
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id != 0)
            {
                if (!BLL.UserAccount.AllowDelete(FormName))
                {
                    MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName));
                }
                else
                {
                    if (MessageBox.Show(Message.PL.Delete_confirmation, "", MessageBoxButton.YesNo) != MessageBoxResult.No)
                    {
                        if (data.Delete() == true)
                        {
                            MessageBox.Show(Message.PL.Delete_Alert);
                            clear();
                            Grid_Refresh();
                        }
                        else
                        {
                            MessageBox.Show(Message.PL.Cant_Delete_Alert);
                            clear();
                        }

                    }
                }
            }
            else
            {
                MessageBox.Show("No Records to Delete");
            }


        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
        }

        private void dgvAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = dgvAccount.SelectedItem as BLL.AccountGroup;
            if (d != null)
            {
                data.Find(d.Id);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Grid_Refresh();
        }

        private void cbxCase_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptStartWith_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void cbxCase_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptContain_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptEndWith_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tc = sender as TabControl;

            if (tc.SelectedIndex == 1)
            {
                LoadReport();
            }

        }

        #endregion

        #region Methods

        private bool AccountGroup_Filter(object obj)
        {
            bool RValue = false;
            var d = obj as BLL.AccountGroup;

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string strSearch = cbxCase.IsChecked == true ? txtSearch.Text : txtSearch.Text.ToLower();
                string strValue = "";

                foreach (var p in d.GetType().GetProperties())
                {
                    if (p.Name.ToLower().Contains("id") ||
                            p.GetValue(d) == null ||
                            (p.Name != nameof(d.GroupName) &&
                                p.Name != nameof(d.UnderAccountGroup.GroupName) &&
                                p.Name != nameof(d.GroupCode)


                             )
                        ) continue;
                    strValue = p.GetValue(d).ToString();
                    if (cbxCase.IsChecked == false)
                    {
                        strValue = strValue.ToLower();
                    }
                    if (rptStartWith.IsChecked == true && strValue.StartsWith(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                    else if (rptContain.IsChecked == true && strValue.Contains(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                    else if (rptEndWith.IsChecked == true && strValue.EndsWith(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                }
            }
            else
            {
                RValue = true;
            }
            return RValue;
        }

        private void Grid_Refresh()
        {
            try
            {
                CollectionViewSource.GetDefaultView(dgvAccount.ItemsSource).Refresh();
            }
            catch (Exception ex) { };

        }

        private void LoadReport()
        {
            try
            {
                rptAccountGroup.Reset();
                ReportDataSource data = new ReportDataSource("AccountGroup", BLL.AccountGroup.toList.Where(x => AccountGroup_Filter(x)).Select(x => new { x.GroupCode, x.GroupName, underGroupName = x.UnderAccountGroup.GroupName }).OrderBy(x => x.GroupCode).ToList());
                ReportDataSource data1 = new ReportDataSource("FundMaster", BLL.FundMaster.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Fund.Id).ToList());
                rptAccountGroup.LocalReport.DataSources.Add(data);
                rptAccountGroup.LocalReport.DataSources.Add(data1);
                rptAccountGroup.LocalReport.ReportPath = @"Master\rptAccountGroup.rdlc";

                ReportParameter[] rp = new ReportParameter[1];
                rp[0] = new ReportParameter("Fund", BLL.UserAccount.User.UserType.Fund.FundName);
                rptAccountGroup.LocalReport.SetParameters(rp);

                rptAccountGroup.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }

        private void onClientEvents()
        {
            BLL.NubeAccountClient.NubeAccountHub.On<BLL.AccountGroup>("AccountGroup_Save", (Account) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    Account.Save(true);
                });

            });

            BLL.NubeAccountClient.NubeAccountHub.On("AccountGroup_Delete", (Action<int>)((pk) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    BLL.AccountGroup agp = new BLL.AccountGroup();
                    agp.Find((int)pk);
                    agp.Delete((bool)true);
                }));

            }));
        }

        #endregion

        private void dgvAccount_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var d = dgvAccount.SelectedItem as BLL.AccountGroup;
            if (d != null)
            {
                data.Find(d.Id);
            }
        }

      
        //private void trvAccount_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    try
        //    {
        //        var d = trvAccount.SelectedItem as BLL.AccountGroup;
        //        if (d != null)
        //        {
        //            data.Find(d.Id);
        //        }
        //    }
        //    catch (Exception ex) { }


        //}
        void clear()
        {
            data.Clear();
          //  trvAccount.ItemsSource = BLL.AccountGroup.toGroup(BLL.DataKeyValue.Primary_Value);
        }
        private void cmbUnder_GotFocus(object sender, RoutedEventArgs e)
        {
            //   var LAGIds = BLL.Ledger.toList.Select(x => x.AccountGroupId).ToList();
            // cmbUnder.ItemsSource = BLL.AccountGroup.toList.Where(x => !LAGIds.Contains(x.Id)).ToList();
            cmbUnder.ItemsSource = BLL.AccountGroup.toList.ToList();
            cmbUnder.SelectedValuePath = "Id";
            cmbUnder.DisplayMemberPath = "GroupNameWithCode";
        }

        
        private void LoadWindow()
        {
            BLL.AccountGroup.Init();

            dgvAccount.ItemsSource = BLL.AccountGroup.toList;

            CollectionViewSource.GetDefaultView(dgvAccount.ItemsSource).Filter = AccountGroup_Filter;
            CollectionViewSource.GetDefaultView(dgvAccount.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.GroupCode), System.ComponentModel.ListSortDirection.Ascending));

            rptContain.IsChecked = true;
            btnSave.Visibility = (BLL.FundMaster.UserPermission.AllowInsert || BLL.FundMaster.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = BLL.FundMaster.UserPermission.AllowDelete ? Visibility.Visible : Visibility.Collapsed;

        }
      
    }
}
