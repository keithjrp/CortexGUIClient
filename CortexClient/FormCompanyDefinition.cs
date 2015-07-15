// ***********************************************************************
// Assembly         : Cortex
// Author           : ktam
// Created          : 12-05-2014
//
// Last Modified By : ktam
// Last Modified On : 02-13-2015
// ***********************************************************************
// <copyright file="FormCompanyDefinition.cs" company="Amazon.com">
//     Copyright © Amazon.com 2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CortexClient.ServiceReference1;

/// <summary>
/// The CortexClient namespace.
/// </summary>
namespace CortexClient
{
    /// <summary>
    /// Class FormCompanyDefinition.
    /// </summary>
    public partial class FormCompanyDefinition : Form
    {
        /// <summary>
        /// Form attributes
        /// </summary>
        CortexWCFServiceClient wc;
        /// <summary>
        /// The current company
        /// </summary>
        Company currentCompany;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormCompanyDefinition"/> class.
        /// </summary>
        /// <param name="c">The c.</param>
        public FormCompanyDefinition(Company c = null)
        {
            InitializeComponent();
            wc = new CortexWCFServiceClient();
            currentCompany = c;
        }

        /// <summary>
        /// Event handler for tool strip Button
        /// Update/Save Company information on the definition screen
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            Boolean saveCA = false;

            if (!saveCA)
            {
                if (currentCompany == null)
                {
                    Company newComp = new Company()
                    {
                        Description = txtCompanyDesc.Text,
                        Name = txtCompanyName.Text,
                        Code = txtCompanyCode.Text,
                        CompanyID = wc.getLastCompany().CompanyID + 100
                    };

                    wc.addCompany(newComp);
                }
                else
                {
                    currentCompany.Code = txtCompanyCode.Text;
                    currentCompany.Description = txtCompanyDesc.Text;
                    currentCompany.Name = txtCompanyName.Text;

                    wc.updateCompany(currentCompany);
                }
                
                MessageBox.Show("Company Saved", "Confirm", MessageBoxButtons.OK); 
            }

            this.Close();
        }


        /// <summary>
        /// Method call before Company form screen is loaded
        /// Loadeds existing Company data if passed in from Deal form
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void FormCompanyDefinition_Load(object sender, EventArgs e)
        {

            if(currentCompany != null)
            {
                txtCompanyCode.Text = currentCompany.Code;
                txtCompanyDesc.Text = currentCompany.Description;
                txtCompanyName.Text = currentCompany.Name;
            }

        }

    }
}
