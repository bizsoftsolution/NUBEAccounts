﻿using System;
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
    public partial class frmFundMaster : UserControl
    {
        BLL.FundMaster data = new BLL.FundMaster();
        public string FormName = BLL.UserTypeFormDetail.toList.Where(x => x.FormName == Forms.frmFundMaster).FirstOrDefault().Description;

        public frmFundMaster()
        {
            InitializeComponent();
            this.DataContext = data;

            onClientEvents();
        }

        private void onClientEvents()
        {
            BLL.NubeAccountClient.NubeAccountHub.On<BLL.FundMaster>("FundMaster_Save", (cs) =>
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
            BLL.FundMaster.Init();
            data.Find(BLL.UserAccount.User.UserType.Fund.Id);
                     
        }
       
        #region ButtonEvents

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Common.AppLib.WriteLog(string.Format("Fund Master Save=>Begins=>Id=>{0}", data.Id));

            if (data.Id==0&& !BLL.UserAccount.AllowInsert(Forms.frmFundMaster))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName));
            }
            else if (data.Id!=0&& !BLL.UserAccount.AllowUpdate(Forms.frmFundMaster))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName));
            }
           
            else if (data.Save() == true)
            {
                Common.AppLib.WriteLog(string.Format("Fund Master Saved Successfully=>Id=>{0}", data.Id));

                MessageBox.Show(string.Format(Message.PL.Saved_Alert), FormName, MessageBoxButton.OK, MessageBoxImage.Information);
                    App.frmHome.ShowWelcome();
                }
            

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            
            if (!BLL.FundMaster.UserPermission.AllowDelete)
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
                f.LoadWindow();
                f.Title = string.Format("Login Users - {0}", BLL.UserAccount.User.UserType.Fund.FundName);
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

        private void btnYearEnd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Tools.frmYearEnd f = new Tools.frmYearEnd();
                f.ShowDialog();
            }
            catch (Exception ex) { }
        }
    }
}
