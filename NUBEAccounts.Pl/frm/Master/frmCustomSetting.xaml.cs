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
using Microsoft.AspNet.SignalR.Client;

namespace NUBEAccounts.Pl.frm.Master
{
    /// <summary>
    /// Interaction logic for frmCustomSetting.xaml
    /// </summary>
    public partial class frmCustomSetting : MetroWindow
    {
        BLL.CustomFormat data = new BLL.CustomFormat();
        public string FormName = "CustomFormat";
        double n = 123456789.12;
        int number2 = 10;
        string words = "";
        public bool IsForcedClose = false;
        public frmCustomSetting()
        {
            InitializeComponent();
            this.DataContext = data;

            onClientEvents();
        }
        private void onClientEvents()
        {
            BLL.NubeAccountClient.NubeAccountHub.On<BLL.CustomFormat>("CustomFormat_Save", (cs) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    cs.Save(true);
                });

            });


        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWindow();
            data.SampleCurrency= (decimal)n;
        }

        public void LoadWindow()
        {
            BLL.CustomFormat.Init();
            data.Find(BLL.UserAccount.User.UserType.CompanyId);
           
            data.SampleCurrency = (decimal)n;

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
                    IsForcedClose = true;
                    Close();
                }
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!BLL.CustomFormat.UserPermission.AllowDelete)
                MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName));
            else if (MessageBox.Show(Message.PL.Delete_confirmation, "", MessageBoxButton.YesNo) != MessageBoxResult.No)

                if (data.Delete() == true)
                {
                    MessageBox.Show(Message.PL.Delete_Alert);

                }
        }



        #endregion

        //private void setSample()
        //{
        //    if (data.CurrencyPositiveSymbolPrefix !=null )
        //    {
        //        words = string.Format("{0}{1} {2} ", txtCurrencyName1.Text, number1 > 1 ? "S" : "", "One Hundred And Twenty Three Million Four Hundred And Fifty Six Thousand Seven Hundred And Eighty Nine");
        //       // words = string.Format("{0} {2} ", txtCurrencyName1.Text, number1 > 1 ? "S" : "", "One Hundred And Twenty Three Million Four Hundred And Fifty Six Thousand Seven Hundred And Eighty Nine");

        //    }
        //    else
        //    {
        //        words = string.Format("{0} {1}{2} ", "One Hundred And Twenty Three Million Four Hundred And Fifty Six Thousand Seven Hundred And Eighty Nine", txtCurrencyName1.Text, number1 > 1 ? "S" : "");

        //    }
        //    if (number2 > 0) words = string.Format("{0} AND {1} {2}{3}", words, "Ten", txtCurrencyName2.Text, number2 > 1 ? "s" : "");
        //    words = string.Format("{0} ONLY", words).ToUpper();

        //    txtSampleCurrencyName1.Text = words;

        //}

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsForcedClose == false && MessageBox.Show("Are you sure to close the Custom Settings?", "Close", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
            BLL.CustomFormat.SetDataFormat();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            Close();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

    }
}
