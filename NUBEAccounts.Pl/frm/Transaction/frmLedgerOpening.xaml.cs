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

namespace NUBEAccounts.Pl.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmLedgerOpening.xaml
    /// </summary>
    public partial class frmLedgerOpening : UserControl
    {
        List<BLL.Ledger> lstLedgerOld = new List<BLL.Ledger>();
        private int m_currentPageIndex;
        private IList<Stream> m_streams;

        public frmLedgerOpening()
        {
            InitializeComponent();
            RptLedger.SetDisplayMode(DisplayMode.PrintLayout);
           
        }
        private Stream CreateStream(string name,
    string fileNameExtension, Encoding encoding,
    string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tc = sender as TabControl;

            if (tc.SelectedIndex == 1)
            {
                LoadReport();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvLedger.ItemsSource = BLL.Ledger.toList;
            lstLedgerOld = BLL.Ledger.toList.Select(x => new BLL.Ledger() { Id = x.Id, OPDr = x.OPDr, OPCr = x.OPCr }).ToList();
            BLL.Ledger data = new BLL.Ledger();
            CollectionViewSource.GetDefaultView(dgvLedger.ItemsSource).Filter = Ledger_Filter;
            CollectionViewSource.GetDefaultView(dgvLedger.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.AccountName), System.ComponentModel.ListSortDirection.Ascending));
            FindDiff();

            LoadReport();
        }
        private bool Ledger_Filter(object obj)
        {
            bool RValue = false;
            var d = obj as BLL.Ledger;

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string strSearch = cbxCase.IsChecked == true ? txtSearch.Text : txtSearch.Text.ToLower();
                string strValue = "";

                foreach (var p in d.GetType().GetProperties())
                {
                    if (p.Name.ToLower().Contains("id") || p.GetValue(d) == null) continue;
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
                CollectionViewSource.GetDefaultView(dgvLedger.ItemsSource).Refresh();
                FindDiff();
            }
            catch (Exception ex) { };

        }

        private void LoadReport()
        {
            try
            {
                RptLedger.Reset();
                ReportDataSource data = new ReportDataSource("Ledger", BLL.Ledger.toList.Where(x => Ledger_Filter(x)).ToList());
                ReportDataSource data1 = new ReportDataSource("FundMaster", BLL.FundMaster.toList.Where(x => x.Id == BLL.UserAccount.User.UserType.Fund.Id).ToList());
                RptLedger.LocalReport.DataSources.Add(data);
                RptLedger.LocalReport.DataSources.Add(data1);
                RptLedger.LocalReport.ReportPath = @"Transaction\rptLedgerOpening.rdlc";

                RptLedger.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }
        private void FindDiff()
        {
            var l1 = BLL.Ledger.toList.Where(x => Ledger_Filter(x)).ToList();
            decimal drAmt = l1.Sum(x => x.OPDr);
            decimal crAmt = l1.Sum(x => x.OPCr);

            lblMsg.Text = string.Format("Total Debit Balance : {0:N2}, Total Credit Balance : {1:N2}\nDifference : {2:N2}", drAmt, crAmt, Math.Abs(drAmt - crAmt));
            lblMsg.Foreground = drAmt == crAmt ? new SolidColorBrush(Color.FromRgb(0, 0, 255)) : new SolidColorBrush(Color.FromRgb(255, 0, 0));
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

        private void rptStartWith_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptContain_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptEndWith_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void dgvLedger_CurrentCellChanged(object sender, EventArgs e)
        {
            FindDiff();
        }

        private void dgvLedger_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            FindDiff();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!BLL.UserAccount.AllowUpdate(Common.Forms.frmLedger))
            {
                MessageBox.Show(string.Format(Message.PL.DenyDelete, "Ledger Opening"));
            }
            else
            {
                foreach (var l1 in BLL.Ledger.toList)
                {
                    var l2 = lstLedgerOld.Where(x => x.Id == l1.Id).FirstOrDefault();
                    if (l1.OPDr != l2.OPDr || l1.OPCr != l2.OPCr)
                    {
                        l1.Save();
                    }
                }
                MessageBox.Show(Message.PL.Saved_Alert);
                App.frmHome.ShowWelcome();
                BLL.Ledger.Init();
            }
            
        }

        private void dgvLedger_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            FindDiff();
        }

        #region Button Events
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                byte[] bytes = RptLedger.LocalReport.Render(
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

                MessageBox.Show("Completed Exporting");
            }
            catch (Exception ex)
            {
            }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Export(RptLedger.LocalReport);
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
            frmLedgerOpeningPrint f = new frmLedgerOpeningPrint();
            f.ShowDialog();
        }

        #endregion
    }
}
