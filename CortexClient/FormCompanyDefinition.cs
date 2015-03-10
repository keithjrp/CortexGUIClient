using System;
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
    public partial class FormCompanyDefinition : Form
    {
        /// <summary>
        /// Form attributes
        /// </summary>
        CortexWCFServiceClient wc;
        Company currentCompany;

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
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
