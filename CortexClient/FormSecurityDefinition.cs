﻿// ***********************************************************************
// Assembly         : Cortex
// Author           : ktam
// Created          : 12-05-2014
//
// Last Modified By : ktam
// Last Modified On : 03-23-2015
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
    /// Class FormSecurityDefinition.
    /// </summary>
    public partial class FormSecurityDefinition : Form
            {
        /// <summary>
        /// Form attributes
        /// </summary>
        CortexWCFServiceClient wc;
        /// <summary>
        /// The current security
        /// </summary>
        Security currentSecurity;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormSecurityDefinition"/> class.
        /// </summary>
        /// <param name="sec">The sec.</param>
        public FormSecurityDefinition(Security sec = null)
        {
            InitializeComponent();
            wc = new CortexWCFServiceClient();
            currentSecurity = sec;
        }

        /// <summary>
        /// Method call before Security form screen is loaded
        /// Loadeds existing Security data if passed in from Deal form
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void FormSecurityDefinition_Load(object sender, EventArgs e)
        {
            ListItem.loadListItems(cbxSecurityTypeID, ListItem.loadTypeList(wc));

            ListItem.loadListItems(cbxCurrencyID, ListItem.loadCCYList(wc));

            if (currentSecurity != null)
            {
                txtSecName.Text = currentSecurity.Name;
                txtSecDescription.Text = currentSecurity.Description;
                txtSecCode.Text = currentSecurity.Code;
                cbxCurrencyID.SelectedValue = currentSecurity.CurrencyID;
                cbxSecurityTypeID.SelectedValue = currentSecurity.SecurityTypeID;
                lblTickerValue.Text = currentSecurity.Ticker;
                lblCusipValue.Text = currentSecurity.Cusip;
                lblSedolValue.Text = currentSecurity.Sedol;
                lblIsinValue.Text = currentSecurity.Isin;
            }

        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the cbxSecurityGroupID control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cbxSecurityGroupID_SelectedIndexChanged(object sender, EventArgs e)
        {
            CortexGUIProcesses.showSecurityGroup(cbxSecurityTypeID, lblSecTypeDescr, lblSecTypeName, wc);
        }

        /// <summary>
        /// Method called when a user loads an existing Security to definition screen
        /// loads Security Currency value to dropdown
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cbxCurrencyID_SelectedIndexChanged(object sender, EventArgs e)
        {
            //showCurrency(cbxCurrencyID, lblCurrencyName);
        }

        /// <summary>
        /// Event handler for tool strip Button
        /// Update/Save Security information on the definition screen
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {

            Boolean  saveSec = txtSecCode.Text != "" && txtSecName.Text != "" && txtSecDescription.Text != "" ? true : false;

            if (saveSec)
            {
                if (currentSecurity == null)
                {
                    Security newSec = new Security()
                    {
                        SecurityID = wc.getLastSecurity().SecurityID + 100,
                        Description = txtSecDescription.Text,
                        Code = txtSecCode.Text,
                        Name = txtSecName.Text,
                        SecurityTypeID = (int)cbxSecurityTypeID.SelectedValue,
                        CurrencyID = (int)cbxCurrencyID.SelectedValue

                    };
                    wc.addSecurity(newSec); 
                }
                else
                {
                    currentSecurity.Description = txtSecDescription.Text;
                    currentSecurity.Code = txtSecCode.Text;
                    currentSecurity.Name = txtSecName.Text;
                    currentSecurity.SecurityTypeID = (int)cbxSecurityTypeID.SelectedValue;
                    currentSecurity.CurrencyID = (int)cbxCurrencyID.SelectedValue;

                    wc.updateSecurity(currentSecurity);
                }
                MessageBox.Show("Security Saved", "Confirm", MessageBoxButtons.OK);

            }
            if(!saveSec)
            {
               //no data entered, save nothing, do nothing

            }
            else
            {
                this.Close();

            }

        }
    }
}
