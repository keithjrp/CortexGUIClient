using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CortexClient.ServiceReference1;
using System.Deployment.Application;


namespace CortexClient
{
    public partial class Login : Form
    {

        CortexWCFServiceClient wc;
        Analyst a;
        public Login()
        {
            InitializeComponent();
            wc = new CortexWCFServiceClient();

        }

        /// <summary>
        /// method handles Submit button on Login form
        /// validates login and retrieves Analyst details from database
        /// saves the login credentials in memory
        /// loads Cortex Application with Analyst info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                String login = txtLogin.Text;
                String pwd = txtPassword.Text;

                a = wc.getAnalystByName(login);

                if (a != null && pwd == a.Password)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    FormOverview o = new FormOverview(a);
                    o.Show();
                    this.Hide();
                    Cursor.Current = Cursors.Default;
                    o.FormClosing += o_FormClosing;
                    ApplicationUser usr = new ApplicationUser()
                    {
                        Name = a.Login,
                        MachineAddress = Environment.MachineName
                        //+ " (" + System.Net.Dns.GetHostAddresses(Environment.MachineName)[0].ToString() + ")"
                    };
                    wc.AuditTrailLogin(usr);
                }
                else
                    MessageBox.Show("Invalid Login!", "Login Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// triggers on Cortex Application Logout/Exit/Quit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void o_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Show();
            txtPassword.Text = "";
            ApplicationUser usr = new ApplicationUser()
            {
                Name = a.Login,
                MachineAddress = Environment.MachineName
                //+ " (" + System.Net.Dns.GetHostAddresses(Environment.MachineName)[0].ToString() + ")"
            };
            wc.AuditTrailLogout(usr);
        }

        private void Login_Load(object sender, EventArgs e)
        {
            try
            {
                ApplicationDeployment deployment = ApplicationDeployment.CurrentDeployment;
                Version version = deployment.CurrentVersion;

                lblVersion.Text = String.Format("Version: {0}.{1}{2}{3}",
                    version.Major, version.Minor, version.Build, version.Revision);
            }
            catch (Exception ex)
            {
                lblVersion.Text = "Version: " + Application.ProductVersion;
            }
        }


    }
}
