// ***********************************************************************
// Assembly         : Cortex
// Author           : JRP-Dell-01
// Created          : 03-10-2015
//
// Last Modified By : JRP-Dell-01
// Last Modified On : 03-20-2015
// ***********************************************************************
// <copyright file="Program.cs" company="">
//     Copyright (c) . All rights reserved.
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
                FormOverview.importPrices(Properties.Settings.Default.PriceFile);

            }
        }
    }
}
