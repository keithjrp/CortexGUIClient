﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CortexClient.ServiceReference1;

namespace CortexClient
{
    public partial class FormSecurityDefinition : Form
    {
        /// <summary>
        /// Form attributes
        /// </summary>
        CortexWCFServiceClient wc;
        Security currentSecurity;

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
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            }

        }

        private void showSecurityGroup(ComboBox cbxSec, Label lblDescr, Label lblName)
        {
            try
            {
                if (cbxSec.SelectedValue != null)
                {
                    SecurityType s = wc.getSecurityType((int)cbxSec.SelectedValue);

                    lblDescr.Text = s.Description;
                    lblName.Text = s.Name;
                }
                else
                {
                    lblDescr.Text = String.Empty;
                    lblName.Text = String.Empty;

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving Security object");
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Method called when a user loads an existing Security to definition screen
        /// loads Security Currency value to dropdown
        /// </summary>
        /// <param name="cbxSec"></param>
        /// <param name="lblName"></param>
        private void showCurrency(ComboBox cbxSec, Label lblName)
        {
            try
            {
                if (cbxSec.SelectedValue != null)
                {
                    Currency s = wc.getCurrency((int)cbxSec.SelectedValue);

                    lblName.Text = s.CurrencyName;
                }
                else
                {
                    lblName.Text = String.Empty;

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving Security object");
                Console.WriteLine(ex.Message);
            }
        }

        private void cbxSecurityGroupID_SelectedIndexChanged(object sender, EventArgs e)
        {
            showSecurityGroup(cbxSecurityTypeID, lblSecTypeDescr, lblSecTypeName);
        }

        /// <summary>
        /// Method called when a user loads an existing Security to definition screen
        /// loads Security Currency value to dropdown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxCurrencyID_SelectedIndexChanged(object sender, EventArgs e)
        {
            //showCurrency(cbxCurrencyID, lblCurrencyName);
        }

        /// <summary>
        /// Event handler for tool strip Button
        /// Update/Save Security information on the definition screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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