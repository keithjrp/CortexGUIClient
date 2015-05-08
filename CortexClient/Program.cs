using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CortexClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Properties.Settings.Default.PriceFile.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Login()); 
            }
            else
            {
                FormOverview.importPrices(Properties.Settings.Default.PriceFile);

            }
        }
    }
}
