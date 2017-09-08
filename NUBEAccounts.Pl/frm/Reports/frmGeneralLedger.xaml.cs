using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
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
using Microsoft.Reporting.WinForms;
using Microsoft.Win32;

namespace NUBEAccounts.Pl.frm.Reports
{
    /// <summary>
    /// Interaction logic for frmGeneralLedger.xaml
    /// </summary>
    public partial class frmGeneralLedger : UserControl
    {
        private int m_currentPageIndex;
        private IList<Stream> m_streams;

        public frmGeneralLedger()
        {
            InitializeComponent();
            rptViewer.SetDisplayMode(DisplayMode.PrintLayout);

            int yy = BLL.UserAccount.User.UserType.Company.LoginAccYear;

            DateTime? dtFrom = new DateTime(yy, 4, 1);
            DateTime? dtTo = new DateTime(yy + 1, 3, 31);

            dtpDateFrom.SelectedDate = dtFrom;
            dtpDateTo.SelectedDate = dtTo;

            cmbAccountName.ItemsSource = BLL.Ledger.toList;
            cmbAccountName.DisplayMemberPath = "AccountName";
            cmbAccountName.SelectedValuePath = "Id";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            try
            {
                List<BLL.GeneralLedger> list = BLL.GeneralLedger.ToList((int)cmbAccountName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
                list = list.Select(x => new BLL.GeneralLedger()
                { AccountName = x.Ledger.AccountName, Particular = x.Particular, CrAmt = x.CrAmt, DrAmt = x.DrAmt, BalAmt = x.BalAmt, EDate = x.EDate, VoucherNo = x.VoucherNo, EType = x.EType, Ledger = x.Ledger, RefNo = x.RefNo }).ToList();

                try
                {
                    rptViewer.Reset();
                    ReportDataSource data = new ReportDataSource("GeneralLedger", list);
                   // ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.CompanyId).ToList());
                    rptViewer.LocalReport.DataSources.Add(data);
                   // rptViewer.LocalReport.DataSources.Add(data1);
                    rptViewer.LocalReport.ReportPath = @"Reports\rptGeneralLedger.rdlc";

                    ReportParameter[] par = new ReportParameter[2];
                    par[0] = new ReportParameter("DateFrom", dtpDateFrom.SelectedDate.Value.ToString());
                    par[1] = new ReportParameter("DateTo", dtpDateTo.SelectedDate.Value.ToString());
                    rptViewer.LocalReport.SetParameters(par);

                    rptViewer.RefreshReport();

                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (cmbAccountName.SelectedValue == null)
            {
                MessageBox.Show("Enter AccountName..");
                cmbAccountName.Focus();
            }
            else
            {
                if (cmbAccountName.SelectedValue != null) dgvGeneralLedger.ItemsSource = BLL.GeneralLedger.ToList((int)cmbAccountName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
                LoadReport();
            }

        }

        private void dgvGeneralLedger_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var gl = dgvGeneralLedger.SelectedItem as BLL.GeneralLedger;
            if (gl != null)
            {
                if (gl.EType != null)
                {
                    if (gl.EType.StartsWith(BLL.FormPrefix.Payment))
                    {
                        Transaction.frmPayment f = new Transaction.frmPayment();
                        App.frmHome.ShowForm(f);
                        System.Windows.Forms.Application.DoEvents();
                        f.data.EntryNo = gl.EntryNo;
                        System.Windows.Forms.Application.DoEvents();
                        f.data.Find();
                        f.btnPrint.IsEnabled = true;
                        if (f.data.RefCode != null)
                        {
                            f.btnSave.IsEnabled = false;
                            f.btnDelete.IsEnabled = false;
                        }
                    }
                    else if (gl.EType.StartsWith(BLL.FormPrefix.Receipt))
                    {
                        Transaction.frmReceipt f = new Transaction.frmReceipt();
                        App.frmHome.ShowForm(f);
                        System.Windows.Forms.Application.DoEvents();
                        f.data.EntryNo = gl.EntryNo;
                        System.Windows.Forms.Application.DoEvents();
                        f.data.Find();
                        f.btnPrint.IsEnabled = true;
                        if (f.data.RefCode != null)
                        {
                            f.btnSave.IsEnabled = false;
                            f.btnDelete.IsEnabled = false;
                        }
                    }
                    else if (gl.EType.StartsWith(BLL.FormPrefix.Journal))
                    {
                        Transaction.frmJournal f = new Transaction.frmJournal();
                        App.frmHome.ShowForm(f);
                        System.Windows.Forms.Application.DoEvents();
                        f.data.EntryNo = gl.EntryNo;
                        System.Windows.Forms.Application.DoEvents();
                        f.data.Find();
                        f.btnPrint.IsEnabled = true;
                        if (f.data.RefCode != null)
                        {
                            f.btnSave.IsEnabled = false;
                            f.btnDelete.IsEnabled = false;
                        }
                    }
                 
                }
                else
                {
                    Transaction.frmJournal f = new Transaction.frmJournal();
                    App.frmHome.ShowForm(f);
                    System.Windows.Forms.Application.DoEvents();
                    f.data.EntryNo = gl.EntryNo;
                    System.Windows.Forms.Application.DoEvents();
                    f.data.Find();
                    f.btnPrint.IsEnabled = true;
                    //if (f.data.RefCode != null)
                    //{
                    //    f.btnSave.IsEnabled = false;
                    //    f.btnDelete.IsEnabled = false;
                    //}
                }
            }
        }

        #region Button Events
        private Stream CreateStream(string name,
  string fileNameExtension, Encoding encoding,
  string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                byte[] bytes = rptViewer.LocalReport.Render(
                   "PDF", null, out mimeType, out encoding,
                    out extension,
                   out streamids, out warnings);

                SaveFileDialog SaveFileDialog1 = new SaveFileDialog();

                SaveFileDialog1.ShowDialog();
                string file = string.Format(@"{0}.pdf", SaveFileDialog1.FileName);
                FileStream fs = new FileStream(file,
                   FileMode.Create);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();

                //MessageBox.Show("Completed Exporting");
            }
            catch (Exception ex)
            {
            }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Export(rptViewer.LocalReport);
            Print();
        }

        private void Export(LocalReport report)
        {
            try
            {
                string deviceInfo =
             @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>11.6in</PageWidth>
                <PageHeight>8.2</PageHeight>
                <MarginTop>0.7in</MarginTop>
                <MarginLeft>0.7in</MarginLeft>
                <MarginRight>0.7in</MarginRight>
                <MarginBottom>0.7in</MarginBottom>
            </DeviceInfo>";
                Warning[] warnings;
                m_streams = new List<Stream>();
                report.Render("Image", deviceInfo, CreateStream,
                  out warnings);
                foreach (Stream stream in m_streams)
                    stream.Position = 0;
            }
            catch (Exception ex)
            { }
        }

        private void Print()
        {
            try
            {
                if (m_streams == null || m_streams.Count == 0)
                    throw new Exception("Error: no stream to print.");
                PrintDocument printDoc = new PrintDocument();
                if (!printDoc.PrinterSettings.IsValid)
                {
                    throw new Exception("Error: cannot find the default printer.");
                }
                else
                {
                    printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                    m_currentPageIndex = 0;
                    printDoc.DefaultPageSettings.Landscape = true;
                    printDoc.Print();
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
           Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            System.Drawing.Rectangle adjustedRect = new System.Drawing.Rectangle(
            ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
            ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
            ev.PageBounds.Width,
            ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(System.Drawing.Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }

        private void btnPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            if (dgvGeneralLedger.Items.Count != 0)
            {
                frmGeneralLedgerPrint f = new frmGeneralLedgerPrint();
                f.LoadReport((int)cmbAccountName.SelectedValue, dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value);
                f.ShowDialog();
            }
            else
            {
                MessageBox.Show("Enter AccountName");
            }

        }

        #endregion

        private void cmbAccountName_Loaded(object sender, RoutedEventArgs e)
        {
            cmbAccountName.ItemsSource = BLL.Ledger.toList.ToList();
            cmbAccountName.DisplayMemberPath = "AccountName";
            cmbAccountName.SelectedValuePath = "Id";
        }
    }
}
