using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NUBEAccounts.Pl.frm;

namespace NUBEAccounts.Pl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static frmHome frmHome;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            BLL.NubeAccountClient.URLPath = ConfigurationManager.AppSettings["SLPath"];
            Window frm = new frmLogin();
            frm.Show();

        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {

        }
    }
}
