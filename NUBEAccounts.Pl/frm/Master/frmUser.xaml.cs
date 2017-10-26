using MahApps.Metro.Controls;
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
using NUBEAccounts.Common;

namespace NUBEAccounts.Pl.frm.Master
{
    /// <summary>
    /// Interaction logic for frmUser.xaml
    /// </summary>
    public partial class frmUser : MetroWindow
    {
        public BLL.UserAccount data = new BLL.UserAccount();
        public frmUser()
        {
            InitializeComponent();
            this.DataContext = data;                      
        }
        public void LoadWindow()
        {
            btnUserTypeSetting.Visibility = BLL.UserType.UserPermission.IsViewForm ? Visibility.Visible : Visibility.Collapsed;
            cmbUserType.ItemsSource = BLL.UserType.toList;
            cmbUserType.DisplayMemberPath = "TypeOfUser";
            cmbUserType.SelectedValuePath = "Id";

            btnSave.Visibility = (BLL.UserType.UserPermission.AllowInsert || BLL.UserType.UserPermission.AllowUpdate) ? Visibility.Visible : Visibility.Collapsed;
           
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Common.AppLib.WriteLog(string.Format("user save=>Begins=>Id=>{0}", data.Id));

            if (data.Id==0 &&!BLL.UserAccount.AllowInsert(Forms.frmUser))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, "Users"));
            }
            else if (data.Id!=0&&!BLL.UserAccount.AllowUpdate(Forms.frmUser))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, "Users"));
            }

            else if (data.Save() == true)
            {
                Common.AppLib.WriteLog(string.Format("user saved successfully=>Id=>{0}", data.Id));
                MessageBox.Show(Message.PL.Saved_Alert);
                if (BLL.UserAccount.User.Id == data.Id)
                {
                    BLL.UserAccount.User.UserName = data.UserName;
                    App.frmHome.Title = String.Format("{0} - {1}", BLL.UserAccount.User.UserName, BLL.UserAccount.User.UserType.Fund.FundName);
                }
                this.Close();
            }
            else
            {
                String str = String.Join("\n", data.lstValidation.Select(x => x.Message).ToList());
                MessageBox.Show(str);
            }
        }

        private void btnUserTypeSetting_Click(object sender, RoutedEventArgs e)
        {
            frmUserType frm = new frmUserType();
            frm.LoadWindow();
            frm.ShowDialog();
        }
    }
}
