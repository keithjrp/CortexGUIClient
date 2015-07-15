// ***********************************************************************
// Assembly         : Cortex
// Author           : ktam
// Created          : 11-18-2014
//
// Last Modified By : ktam
// Last Modified On : 03-20-2015
// ***********************************************************************
// <copyright file="Program.cs" company="Amazon.com">
//     Copyright © Amazon.com 2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

/// <summary>
/// The CortexClient namespace.
/// </summary>
namespace CortexClient
{
    /// <summary>
    /// Class Program.
    /// </summary>
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
                CortexGUIProcesses.importPrices(Properties.Settings.Default.PriceFile);

            }
        }
    }
}
