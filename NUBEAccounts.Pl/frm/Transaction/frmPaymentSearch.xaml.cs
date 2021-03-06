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
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace NUBEAccounts.Pl.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmPaymentSearch.xaml
    /// </summary>
    public partial class frmPaymentSearch : MetroWindow
    {
        public frmPaymentSearch()
        {
            InitializeComponent();
            int yy = BLL.UserAccount.User.UserType.Fund.LoginAccYear;

            DateTime? dtFrom = new DateTime(yy, 4, 1);
            DateTime? dtTo = new DateTime(yy + 1, 3, 31);
            dtpDateFrom.SelectedDate =dtFrom;
            dtpDateTo.SelectedDate = DateTime.Now;
            dtpDateFrom.DisplayDateStart = Common.AppLib.minDate;
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cmbAccountName.ItemsSource = BLL.Ledger.toList.ToList();
            cmbAccountName.DisplayMemberPath = "AccountName";
            cmbAccountName.SelectedValuePath = "Id";
            dgvDetails.ItemsSource = BLL.Payment.ToList((int?)cmbAccountName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, cmbstatus.Text);

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dgvDetails.ItemsSource = BLL.Payment.ToList((int?)cmbAccountName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value, cmbstatus.Text);
            
        }
        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var rp = dgvDetails.SelectedItem as BLL.Payment;
            if (rp != null)
            {
                    Transaction.frmPayment f = new Transaction.frmPayment();
                    App.frmHome.ShowForm(f);
                    System.Windows.Forms.Application.DoEvents();
                    f.data.EntryNo = rp.EntryNo;
                f.btnPrint.IsEnabled = true;
                    f.data.Find();
                    System.Windows.Forms.Application.DoEvents();
                this.Close();             
            }
        }

    }
}
