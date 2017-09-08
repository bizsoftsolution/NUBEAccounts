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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNet.SignalR.Client;
using NUBEAccounts.Common;

namespace NUBEAccounts.Pl.frm.Master
{
    /// <summary>
    /// Interaction logic for FundSetting.xaml
    /// </summary>
    public partial class FundSetting : UserControl
    {
        BLL.CompanyDetail data = new BLL.CompanyDetail();
        public string FormName = "CompanySetting";
        public FundSetting()
        {
            InitializeComponent();
            this.DataContext = data;

            onClientEvents();
        }

        private void onClientEvents()
        {
            BLL.NubeAccountClient.NubeAccountHub.On<BLL.CompanyDetail>("CompanyDetail_Save", (cs) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    cs.Save(true);
                });

            });

            BLL.NubeAccountClient.NubeAccountHub.On<BLL.UserAccount>("UserAccount_Save", (ua) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    BLL.UserAccount u = new BLL.UserAccount();
                    ua.toCopy<BLL.UserAccount>(u);
                    BLL.UserAccount.toList.Add(u);
                });

            });
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BLL.CompanyDetail.Init();
            data.Find(BLL.UserAccount.User.UserType.Company.Id);
                     
        }
       


        #region ButtonEvents

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if (!BLL.UserAccount.AllowInsert(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName));
            }
            else if (!BLL.UserAccount.AllowUpdate(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName));
            }
           
            else
            {
                if (data.Save() == true)
                {
                    MessageBox.Show(Message.PL.Saved_Alert);
                    App.frmHome.ShowWelcome();
                }
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!BLL.CompanyDetail.UserPermission.AllowDelete)
                MessageBox.Show(string.Format(Message.PL.DenyDelete, lblHead.Text));
            //    else if (MessageBox.Show(Message.PL.Delete_confirmation, "", MessageBoxButton.YesNo) != MessageBoxResult.No)
            else if (MessageBox.Show(Message.PL.Delete_confirmation, "", MessageBoxButton.YesNo) != MessageBoxResult.No)
            {
                //frmDeleteConfirmation frm = new frmDeleteConfirmation();
                //frm.ShowDialog();
                //if (frm.RValue == true)
                //{
                    if (data.Delete() == true)
                    {
                        MessageBox.Show(Message.PL.Delete_Alert);
                        App.frmHome.IsForcedClose = true;
                        App.frmHome.Close();
                    }
                //}
                else
                {
                    MessageBox.Show(Message.PL.Cant_Delete_Alert);

                }


            }

        }
   #endregion 

     
    
      
        private void btnUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmUserManager f = new frmUserManager();
                f.LoadWindow(BLL.UserAccount.User.UserType.CompanyId);
                f.CompanyId = BLL.UserAccount.User.UserType.CompanyId;
                f.Title = string.Format("Login Users - {0}", BLL.UserAccount.User.UserType.Company.CompanyName);
                f.ShowDialog();
            }
            catch (Exception ex) { }
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmCustomSetting f = new frmCustomSetting();
                f.LoadWindow();
                f.ShowDialog();
            }
            catch (Exception ex) { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }


    }
}
