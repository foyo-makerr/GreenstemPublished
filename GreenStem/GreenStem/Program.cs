
using GreenStem.ClassModules;
using GreenStem.Std;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GreenStem
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ApplicationExit += Application_ApplicationExit;
            Application.Run(new frmMultiCompanies());

           
        }
        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            LogGuserLocked.DeleteLogsForComputer(Environment.MachineName);
            LicenseControl.DecreaseConcurrentUsing(modPublicVariable.Company, modPublicVariable.CompanyName);

        }
    
    }
}
