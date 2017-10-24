using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Data;
using System.Reflection;

namespace NUBEAccounts.Pl.frm
{
    /// <summary>
    /// Interaction logic for frmHome.xaml
    /// </summary>
    public partial class frmHome : MetroWindow
    {
        public bool IsForcedClose = false;
        public frmHome()
        {
            InitializeComponent();
            ShowWelcome();
            onClientEvents();
            IsForcedClose = false;
            var l1 = BLL.UserAccount.User.UserType.UserTypeDetails.Where(x => x.IsViewForm && 
                                                                              x.UserTypeFormDetail.IsMenu && 
                                                                              x.UserTypeFormDetail.IsActive && 
                                                                              x.UserTypeFormDetail.FormType == "Master")
                                                                  .Select(x=> new Common.NavMenuItem() {
                                                                                    MenuName=x.UserTypeFormDetail.Description,
                                                                                    FormName=x.UserTypeFormDetail.FormName
                                                                              })
                                                                  .ToList();
            lstMaster.ItemsSource = l1;

            var l2 = BLL.UserAccount.User.UserType.UserTypeDetails.Where(x => x.IsViewForm &&
                                                                              x.UserTypeFormDetail.IsMenu &&
                                                                              x.UserTypeFormDetail.IsActive &&
                                                                              x.UserTypeFormDetail.FormType == "Transaction")
                                                                  .Select(x => new Common.NavMenuItem()
                                                                  {
                                                                      MenuName = x.UserTypeFormDetail.Description,
                                                                      FormName = x.UserTypeFormDetail.FormName
                                                                  })
                                                                  .ToList();
            lstTransaction.ItemsSource = l2;

            var l3 = BLL.UserAccount.User.UserType.UserTypeDetails.Where(x => x.IsViewForm &&
                                                                              x.UserTypeFormDetail.IsMenu &&
                                                                              x.UserTypeFormDetail.IsActive &&
                                                                              x.UserTypeFormDetail.FormType == "Report")
                                                                  .Select(x => new Common.NavMenuItem()
                                                                  {
                                                                      MenuName = x.UserTypeFormDetail.Description,
                                                                      FormName = x.UserTypeFormDetail.FormName
                                                                  })
                                                                  .ToList();
            lstReport.ItemsSource = l3;
        }
        public void ShowWelcome()
        {
            ccContent.Content = new frmWelcome();
        }

        public void ShowForm(object o)
        {
            ccContent.Content = o;
        }

        private void onClientEvents()
        {

        }

        private void ListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var dependencyObject = Mouse.Captured as DependencyObject;
                while (dependencyObject != null)
                {
                    if (dependencyObject is ScrollBar) return;
                    dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
                }
                ListBox lb = sender as ListBox;
                Common.NavMenuItem mi = lb.SelectedItem as Common.NavMenuItem;
                if (mi.Content == null)
                {
                    object obj = Activator.CreateInstance(Type.GetType(mi.FormName));
                    mi.Content = obj;                    
                }
                if (mi.Content != null) ccContent.Content = mi.Content;
            }
            catch (Exception ex) { }
            MenuToggleButton.IsChecked = false;
            
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dynamic frm = ccContent.Content;
            if (frm.GetType().Name != "frmWelcome")
            {
                ShowWelcome();
                e.Cancel = true;
            }
            else if (!IsForcedClose && MessageBox.Show("Are you sure to Exit?", "Exit", MessageBoxButton.YesNo) != MessageBoxResult.Yes) e.Cancel = true;
        }
    }
}
