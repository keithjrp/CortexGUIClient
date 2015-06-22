// ***********************************************************************
// Assembly         : Cortex
// Author           : JRP-Dell-01
// Created          : 03-10-2015
//
// Last Modified By : JRP-Dell-01
// Last Modified On : 04-24-2015
// ***********************************************************************
// <copyright file="FormOverview.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CortexClient.ServiceReference1;
using Microsoft.VisualBasic;
using Microsoft.Office.Interop.Excel;
using RyanUtils;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;

/// <summary>
/// The CortexClient namespace.
/// </summary>
namespace CortexClient
{
    /// <summary>
    /// Class FormOverview.
    /// </summary>
    public partial class FormOverview : Form
    {
        /// <summary>
        /// Form attributes and local Cortex objects
        /// </summary>
        CortexWCFServiceClient wc;
        /// <summary>
        /// The new deal
        /// </summary>
        Boolean newDeal = false;
        /// <summary>
        /// The analyst access level
        /// </summary>
        int analystAccessLevel;
        /// <summary>
        /// The numeric format
        /// </summary>
        String numericFormat = "N2";
        /// <summary>
        /// The analyst
        /// </summary>
        Analyst analyst;
        /// <summary>
        /// The deal
        /// </summary>
        Deal deal;
        /// <summary>
        /// The usr
        /// </summary>
        ApplicationUser usr;
        /// <summary>
        /// The comp list
        /// </summary>
        List<ListItem> compList,analystList,dealStatusList;
        /// <summary>
        /// The mb
        /// </summary>
        MergerArb mb;
        /// <summary>
        /// The NMB
        /// </summary>
        MergerArbNew[] nmb;
        /// <summary>
        /// The current price date
        /// </summary>
        DateTime currentPriceDate;

        /// <summary>
        /// Set up the Cortex Application Window, logs user info
        /// </summary>
        /// <param name="a">a.</param>
        public FormOverview(Analyst a = null)
        {
            InitializeComponent();
            //ServiceReference1.CortexWCFServiceClient wc = new CortexWCFServiceClient();
            wc = new CortexWCFServiceClient();

            //set permissions
            //if (a != null) analystAccessLevel = (int)a.; else analystAccessLevel = 0;
            analyst = a;
            usr = new ApplicationUser()
            {
                Name = analyst.Login,
                MachineAddress = Environment.MachineName
                //+ " (" + System.Net.Dns.GetHostAddresses(Environment.MachineName)[0].ToString() + ")"
            };
        }

        /// <summary>
        /// Event handler for user double-click on Overview screen
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellMouseEventArgs" /> instance containing the event data.</param>
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            LoadDeal();
        }

        /// <summary>
        /// Loads exiting Deal to definition screen
        /// </summary>
        /// <param name="d">The d.</param>
        private void LoadDeal(Deal d = null)
        {
            try
            {
                clearAllControls();

                //FormDealDefinition f2 = new FormDealDefinition();
                deal = getSelectedDeal(d);

                fillAllBasicInfo(deal);
                fillAllCompanyControls(wc, deal);
                fillAllEventControls(wc, deal);
                fillCurrencyControls(wc, deal);
                fillAllDocumentControls(wc, deal);
                fillAllSecurityControls(wc, deal);
                fillAnalystControls(wc, deal);
                fillMergerArbInfo();

                tabCtrlDealDef.Visible = true;

                if (deal.DealTypeID != 1) //DealType != MA
                {
                    ((Control)this.tabMA).Enabled = false;
                    tabMA.Visible = false;
                }
                else
                {
                    ((Control)this.tabMA).Enabled = true;
                    tabMA.Visible = true;
                }

                gbOverview.Visible = false;

                wc.AuditTrailView(usr, deal);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Fills the merger arb information.
        /// </summary>
        private void fillMergerArbInfo()
        {
            try
            {
                clearMergerArbs();

                mb = wc.getMergerArbByDealId(deal.DealID);

                if (mb != null)
                {
                    List<ListItem> MergerArbByDealId = new List<ListItem>();
                    foreach (PropertyInfo s in mb.GetType().GetProperties())
                    {
                        if (s.CanWrite
                            && s.Name != "ExtensionData"
                            && s.Name != "MergerArbID"
                            )
                        {
                            ListItem item = new ListItem()
                            {
                                Text = s.Name.Substring(5)
                                .Replace("_Required_", "")
                                .Replace("_Optional_", "")
                                .Replace("_", " ")
                                ,
                                Value = s.GetValue(mb, null)
                            };
                            MergerArbByDealId.Add(item);
                        }
                    }
                    ListItem.loadListItems(dgvMergArb, MergerArbByDealId);
                    dgvMergArb.Columns[0].Width = 300;
                    dgvMergArb.Columns[0].HeaderText = "Field";
                    dgvMergArb.Columns[0].ReadOnly = true;
                    dgvMergArb.Columns[1].HeaderText = "Value";
                    dgvMergArb.Columns[1].Width = 550;
                }
                else
                {
                    nmb = wc.getMergerArbNewByDealId(deal.DealID);

                    dgvMergArb.Columns.Add("ID", "ID");
                    dgvMergArb.Columns["ID"].Visible = false;
                    dgvMergArb.Columns.Add("[Field Updated]", "Field Updated");
                    dgvMergArb.Columns.Add("[Field Type]", "Field Type");
                    dgvMergArb.Columns.Add("[Field Name]", "Field Name");
                    dgvMergArb.Columns["[Field Name]"].Width = 350;
                    dgvMergArb.Columns["[Field Name]"].ReadOnly = true;
                    dgvMergArb.Columns.Add("[Calendar Flag]", "Calendar Flag");
                    dgvMergArb.Columns.Add("[Field Value]", "Field Value");
                    dgvMergArb.Columns["[Field Value]"].Width = 350;
                    dgvMergArb.Columns.Add("[Calendar Time]", "Calendar Time");
                    dgvMergArb.Columns.Add("[Calendar Comment]", "Calendar Comment");

                    foreach (MergerArbNew mm in nmb)
                    {
                        String fieldValue = "";
                        try
                        {
                            fieldValue = mm.Field_Value != null ? mm.Field_Value.Replace("#NAME?","TBD") : "";
                            Double d;

                            if (mm.Field_Type == "Date" && Double.TryParse(fieldValue, out d))
                                fieldValue = DateTime.FromOADate(Double.Parse(fieldValue)).ToShortDateString();
                        }
                        catch (Exception ex)
                        {
                            //do nothing
                            MessageBox.Show(ex.Message);

                        }
                        dgvMergArb.Rows.Add(
                            mm.ID,
                            mm.Field_Updated,
                            mm.Field_Type,
                            mm.Field_Name,
                            mm.Calendar_Flag,
                            fieldValue,
                            mm.Calendar_Time,
                            mm.Calendar_Comment);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Clears the merger arbs.
        /// </summary>
        private void clearMergerArbs()
        {
            dgvMergArb.DataSource = null;
            dgvMergArb.Rows.Clear();
            dgvMergArb.Columns.Clear();
        }

        /// <summary>
        /// Method called when a user loads an existing Deal to definition screen
        /// loads basic Deal values to textbox and dropdowns
        /// </summary>
        /// <param name="deal">The deal.</param>
        private void fillAllBasicInfo(Deal deal)
        {
            try
            {
                txtDealDescription.Text = deal.Description;
                txtInvestmentThesis.Text = deal.InvestmentThesis;
                txtRecommendation.Text = deal.Recommendation;
                txtTargetPrice.Text = deal.TargetPrice.Value.ToString(numericFormat);
                txtTargetPriceValuation.Text = deal.TargetPriceValuation;
                txtCatalyst.Text = deal.Catalyst;
                txtKeyRisk.Text = deal.KeyRisks;
                txtComps.Text = deal.Comps;
                txtDownsidePrice.Text = deal.DownsidePrice.Value.ToString(numericFormat);
                txtDownsidePriceValuation.Text = deal.DownsidePriceValuation;
                txtCurrentValuation.Text = deal.CurrentValuation;
                txtValuation.Text = deal.ValuationMethodology;

                DealStatus d = wc.getDealStatus((int)deal.DealStatusID);
                cbxStatus.Text = d.Code;

                Category c = wc.getCategory((int)deal.CategoryID);
                cbxCategory.Text = c.Name;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// retrieve selected Deal info to memory from user double-click on Overview Grid
        /// </summary>
        /// <param name="deal">The deal.</param>
        /// <returns>Deal.</returns>
        private Deal getSelectedDeal(Deal deal = null)
        {
            try
            {
                if (deal == null)
                {
                    int row = dataGridView1.CurrentRow.Index;

                    int d =
                        Convert.ToInt16(dataGridView1.Rows[row].Cells["ID"].Value);

                    deal = wc.getDeal(d);
                    this.Text = "Deal: " + d + " - " + dataGridView1.Rows[row].Cells[1].Value;
                }

                return deal;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new Deal();
            }

        }

        /// <summary>
        /// Method called when a user loads an existing Deal to definition screen
        /// loads Deal Deal Currency value to dropdown
        /// </summary>
        /// <param name="wc">The wc.</param>
        /// <param name="deal">The deal.</param>
        private void fillCurrencyControls(CortexWCFServiceClient wc, Deal deal)
        {
            Currency c = wc.getCurrency((int)deal.CurrencyID);
            cbxCurrencyID.Text = c.CurrencyCode;
        }

        /// <summary>
        /// Method called when a user loads an existing Deal to definition screen
        /// loads Deal Events value to datagrid
        /// </summary>
        /// <param name="wc">The wc.</param>
        /// <param name="deal">The deal.</param>
        private void fillAllEventControls(CortexWCFServiceClient wc, Deal deal)
        {
            try
            {
                clearEvents();

                //DataGridViewLinkColumn note = new DataGridViewLinkColumn();
                //note.DataPropertyName = "Note";
                //note.Name = "Note";
                //note.LinkBehavior = LinkBehavior.SystemDefault;

                dgEvents.Columns.Add("Desc", "Event Description");
                dgEvents.Columns[0].Width = 250;
                dgEvents.Columns.Add("Date", "Event Date");
                dgEvents.Columns.Add("Note", "Event Note");
                //dgEvents.Columns["Note"].HeaderText = "Event Note";
                dgEvents.Columns[2].Width = 250;
                dgEvents.Columns.Add("Type", "Event Type");
                dgEvents.Columns.Add("ID", "Event ID");
                dgEvents.Columns["ID"].Visible = false;

                Event[] events = wc.getEventsByDeal(deal.DealID);
                foreach(Event ee in events)
                {
                    dgEvents.Rows.Add(
                        ee.Description,
                        ee.EventDate.ToShortDateString(),
                        ee.Note,
                        wc.getEventType(ee.EventTypeID).Code,
                        ee.EventID
                        );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Clears the Event tab controls for New Deal
        /// </summary>
        private void clearEvents()
        {
            dgEvents.Rows.Clear();
            dgEvents.Columns.Clear();
            txtEventDescription.Text = String.Empty;
            txtEventNote.Text = String.Empty;
            dtpEventDate.Value = DateTime.Today;
            cbxEventType.Text = String.Empty;
        }

        /// <summary>
        /// Method called when a user loads an existing Deal to definition screen
        /// loads Deal Company1 value to dropdown
        /// </summary>
        /// <param name="wc">The wc.</param>
        /// <param name="deal">The deal.</param>
        private void fillAllCompanyControls(CortexWCFServiceClient wc, Deal deal)
        {
            Company c1;

            try
            {
                c1 = wc.getCompany((int)deal.CompanyID1);

                cbxCompany1.Text = c1.Description;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// Method called when a user loads an existing Deal to definition screen
        /// loads Deal Documents to Document Grid
        /// </summary>
        /// <param name="wc">The wc.</param>
        /// <param name="deal">The deal.</param>
        private void fillAllDocumentControls(CortexWCFServiceClient wc, Deal deal)
        {
            try
            {
                clearDocuments();

                DataGridViewLinkColumn uri = new DataGridViewLinkColumn();
                uri.DataPropertyName = "URI";
                uri.Name = "URI";
                uri.LinkBehavior = LinkBehavior.SystemDefault;

                dgDocuments.Columns.Add(uri);
                dgDocuments.Columns["URI"].Width = 100;

                dgDocuments.Columns.Add("Description", "Description");
                dgDocuments.Columns["Description"].Width = 250;

                dgDocuments.Columns.Add("Name", "Name");
                dgDocuments.Columns["Name"].Width = 150;

                dgDocuments.Columns.Add("ID", "ID");
                dgDocuments.Columns["ID"].Visible = false;
                dgDocuments.Columns["ID"].Width = 150;


                Document[] docs = wc.getDocumentsByDeal(deal.DealID);
                foreach (Document d1 in docs)
                {
                    dgDocuments.Rows.Add(d1.URI, d1.Description, d1.Name, d1.DocumentID);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Clears the controls on Document tab for New Deal
        /// </summary>
        private void clearDocuments()
        {
            dgDocuments.Rows.Clear();
            dgDocuments.Columns.Clear();
            txtDocumentDescription.Text = String.Empty;
            txtDocumentName.Text = String.Empty;
            txtFileUpload.Text = String.Empty;
        }

        /// <summary>
        /// Method called when a user loads an existing Deal to definition screen
        /// loads Deal Security1 value to dropdown
        /// </summary>
        /// <param name="wc">The wc.</param>
        /// <param name="deal">The deal.</param>
        private void fillAllSecurityControls(CortexWCFServiceClient wc, Deal deal)
        {
            Security s1;
            SecurityGroup sg;
            Price currPrc;
            try
            {
                if (deal.SecurityGroupID != 0)
                {
                    sg = wc.getSecurityGroup((int)deal.SecurityGroupID);

                    s1 = wc.getSecurity((int)sg.SecurityID1);

                    cbxSecurity1.Text = s1.Code;
                    currPrc = wc.getPrice(s1.SecurityID);
                    txtCurrentPrice.Text = currPrc.Price1.ToString(numericFormat);
                    currentPriceDate = currPrc.PriceDateTime;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method called when a user loads an existing Deal to definition screen
        /// loads Deal Lead Analyst value to dropdown and rest of the Deal Team to datagrid
        /// </summary>
        /// <param name="wc">The wc.</param>
        /// <param name="deal">The deal.</param>
        private void fillAnalystControls(CortexWCFServiceClient wc, Deal deal)
        {
            try
            {
                MapDealAnalyst[] m = wc.getDealTeam(deal.DealID);
                Analyst currentAnalyst;
                clearAnalysts();
                dgAnalysts.Columns.Add("ID", "ID");
                dgAnalysts.Columns["ID"].Visible = false;
                dgAnalysts.Columns.Add("Name", "Name");

                foreach (MapDealAnalyst mm in m)
                {
                    currentAnalyst = wc.getAnalyst((int)mm.AnalystID);
                    if ((bool)mm.IsLeadAnalyst)
                    {
                        cbxLeadAnalyst.Text = currentAnalyst.Login;
                        lblLeadAnalyst.Text = currentAnalyst.Login;
                    }
                    else
                    {
                        dgAnalysts.Rows.Add(mm.MapDealAnalystID, currentAnalyst.Login);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Clears Analyst controls for New Deal
        /// </summary>
        private void clearAnalysts()
        {
            dgAnalysts.Rows.Clear();
            dgAnalysts.Columns.Clear();
            cbxLeadAnalyst.Text = String.Empty;
        }

        /// <summary>
        /// Method called before user start entering a new Deal
        /// Clears all text controls on definition screen
        /// </summary>
        private void clearAllControls()
        {
            #region Clear All TextBox
            txtDealDescription.Text = String.Empty;
            txtInvestmentThesis.Text = String.Empty;
            txtRecommendation.Text = String.Empty;
            txtTargetPrice.Text = String.Empty;
            txtTargetPriceValuation.Text = String.Empty;
            txtCatalyst.Text = String.Empty;
            txtKeyRisk.Text = String.Empty;
            txtComps.Text = String.Empty;
            txtDownsidePrice.Text = String.Empty;
            txtDownsidePriceValuation.Text = String.Empty;
            txtCurrentValuation.Text = String.Empty;
            txtValuation.Text = String.Empty;
            txtCurrentPrice.Text = String.Empty;
            cbxCompany1.Text = String.Empty;
            cbxCurrencyID.Text = String.Empty;
            cbxSecurity1.Text = String.Empty;
            cbxCategory.Text = String.Empty;
            cbxLeadAnalyst.Text = String.Empty;

            clearEvents();
            clearAnalysts();
            clearDocuments();
            #endregion
        }

        /// <summary>
        /// Event handler for top menu Add New | Deal
        /// Load blank Deal definition scrren
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void menuNewDeal_Click(object sender, EventArgs e)
        {
            if (gbOverview.Visible)
            {
                tabCtrlDealDef.Visible = true;
                gbOverview.Visible = false;
            }
            clearAllControls();
            newDeal = true;
        }

        /// <summary>
        /// Loaded data into Grids and Dropdowns as the Application launches
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void FormOverview_Load(object sender, EventArgs e)
        {
            try
            {
                //compList = ListItem.loadCompanyList(wc);
                //ListItem.loadListItems(cbxSearchByCompany, compList);
                //ListItem.loadListItems(cbxCompany1, compList);

                //ListItem.loadListItems(cbxSecurity1, ListItem.loadSecurityList(wc));

                //analystList = ListItem.loadAnalystList(wc);
                //ListItem.loadListItems(cbxLeadAnalyst, analystList);
                //ListItem.loadListItems(cbxSearchByAnalyst, analystList);
                //ListItem.loadListItems(dgAnalystPool, analystList);
                //dgAnalystPool.Columns[0].Name = "Login";
                //dgAnalystPool.Columns[0].HeaderText = "Login";
                //dgAnalystPool.Columns[1].Visible = false;

                //ListItem.loadListItems(cbxEventType, ListItem.loadEventTypes(wc));
                //ListItem.loadListItems(cbxCurrencyID, ListItem.loadCCYList(wc));

                //dealStatusList = ListItem.loadDealStatusList(wc);
                //ListItem.loadListItems(cbxStatus, dealStatusList);
                //ListItem.loadListItems(cbxSearchStatus, dealStatusList);

                //ListItem.loadListItems(cbxCategory, ListItem.loadCategoryList(wc));
                //ListItem.loadListItems(cbxSearchCategoryClass, ListItem.loadClassList(wc));

                tabCtrlDealDef.Visible = false;
                LoadDealList(wc.getDeals(0), 30);
                lblAnalyst.Text = analyst.Login;
                gbOverview.Visible = true;

                cbxSearchByAnalyst.Text = "";
                cbxSearchByCompany.Text = "";
                cbxSearchCategoryClass.Text = "";
                cbxSearchStatus.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// Event handler for top menu Add New | Security
        /// Load blank Security form
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void menuSecurity_Click(object sender, EventArgs e)
        {
            FormSecurityDefinition secForm = new FormSecurityDefinition();
            secForm.Visible = true;
        }

        /// <summary>
        /// Event handler for top menu Add New | Company
        /// Load blank Company form
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void menuCompany_Click(object sender, EventArgs e)
        {
            FormCompanyDefinition compForm = new FormCompanyDefinition();
            compForm.Visible = true;
        }

        /// <summary>
        /// Event handler for top menu Logout, exits the application
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (deal != null)
            {
                wc.AuditTrailUpdate(usr, deal); 
            }
            wc.Close();
            this.Close();
        }

        /// <summary>
        /// Event handler for top menu Go | Update (CTRL+U)
        /// Update/Save Deal information on the definition screen
        /// Goes back to Overview Grid and hide Deal definition screen
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!gbOverview.Visible && !newDeal)
                {
                    updateExistingDeal();
                }
                else if(newDeal)
                {
                    addNewDeal();
                }

                tabCtrlDealDef.Visible = false;
                gbOverview.Visible = true;
                newDeal = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// Method to Handle Update Menu control for saving a new Deal to the database
        /// </summary>
        private void addNewDeal()
        {
            try
            {
                if (txtDealDescription.Text != "" && txtTargetPrice.Text != "" &&
                    MessageBox.Show("Save Deal?", "Confirm", MessageBoxButtons.YesNo).ToString() == "Yes")
                {
                    Deal deal = new Deal();
                    Currency c = wc.getCurrencyByCodeOrName(cbxCurrencyID.Text);

                    if (txtDownsidePrice.Text == "") txtDownsidePrice.Text = "0";

                    deal.InvestmentThesis = txtInvestmentThesis.Text;
                    deal.Recommendation = txtRecommendation.Text;
                    deal.TargetPrice = Convert.ToDecimal(txtTargetPrice.Text);
                    deal.DealCurrencyID = Convert.ToInt16(c.CurrencyID);
                    deal.CurrencyID = Convert.ToInt16(c.CurrencyID);
                    deal.ValuationMethodology = txtValuation.Text;
                    deal.TargetPriceValuation = txtTargetPriceValuation.Text;
                    deal.KeyRisks = txtKeyRisk.Text;
                    deal.DownsidePriceValuation = txtDownsidePriceValuation.Text;
                    deal.DownsidePrice = Convert.ToDecimal(txtDownsidePrice.Text);
                    deal.Description = txtDealDescription.Text;
                    deal.CurrentValuation = txtCurrentValuation.Text;
                    deal.Comps = txtComps.Text;
                    deal.CompanyID1 = (int)cbxCompany1.SelectedValue;
                    deal.Catalyst = txtCatalyst.Text;
                    deal.DealTypeID = 1;
                    deal.ExpirationDate = new DateTime(2020, 1, 25);
                    deal.DealStatusID = (int)cbxStatus.SelectedValue;
                    deal.CategoryID = (int)cbxCategory.SelectedValue;
                    addNewAnalyst(deal, true);

                    int newSecGroupId = wc.getLastSecurityGroup().SecurityGroupID + 100;

                    SecurityGroup newSG = new SecurityGroup()
                    {
                        SecurityGroupID = newSecGroupId,
                        SecurityID1 = (int)cbxSecurity1.SelectedValue
                    };

                    wc.addSecurityGroup(newSG);

                    deal.SecurityGroupID = newSecGroupId;
                    deal.DocumentGroupID = 100;
                    deal.DealID = wc.getLastDeal().DealID + 1;
                    wc.addDeal(deal);

                    MessageBox.Show("Deal Saved", "Confirm", MessageBoxButtons.OK);
                    wc.AuditTrailAdd(usr, deal);
                }
                else
                    MessageBox.Show("Description and/or Target Price empty", "Validation Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method to Handle Update Menu control for updating an Existing Deal
        /// </summary>
        private void updateExistingDeal()
        {
            try
            {
                //Deal definition screen loaded
                if (gbOverview.Visible != true &&
                    MessageBox.Show("Update Deal?", "Confirm", MessageBoxButtons.YesNo).ToString() == "Yes")
                {
                    ///Collect Deal info
                    Deal deal = getSelectedDeal();

                    Currency c = wc.getCurrencyByCodeOrName(cbxCurrencyID.Text);
                    DocumentGroup dg = wc.getDocumentGroup((int)deal.DocumentGroupID);
                    SecurityGroup sg = wc.getSecurityGroup((int)deal.SecurityGroupID);
                    Document temp = new Document();

                    deal.InvestmentThesis = txtInvestmentThesis.Text;
                    deal.Recommendation = txtRecommendation.Text;
                    deal.TargetPrice = Convert.ToDecimal(txtTargetPrice.Text);
                    deal.DealCurrencyID = Convert.ToInt16(c.CurrencyID);
                    deal.CurrencyID = Convert.ToInt16(c.CurrencyID);
                    deal.ValuationMethodology = txtValuation.Text;
                    deal.TargetPriceValuation = txtTargetPriceValuation.Text;
                    deal.KeyRisks = txtKeyRisk.Text;
                    deal.DownsidePriceValuation = txtDownsidePriceValuation.Text;
                    deal.DownsidePrice = Convert.ToDecimal(txtDownsidePrice.Text == "" ? "0" : txtDownsidePrice.Text);
                    deal.Description = txtDealDescription.Text;
                    deal.CurrentValuation = txtCurrentValuation.Text;
                    deal.Comps = txtComps.Text;
                    deal.CompanyID1 = (int)cbxCompany1.SelectedValue;
                    deal.CompanyID2 = (int)cbxCompany1.SelectedValue;
                    deal.CompanyID3 = (int)cbxCompany1.SelectedValue;
                    deal.Catalyst = txtCatalyst.Text;
                    deal.DealTypeID = 1;
                    deal.DealStatusID = (int)cbxStatus.SelectedValue;
                    deal.CategoryID = (int)cbxCategory.SelectedValue;

                    updateMergArb();
                    updateLeadAnalyst(deal);
                    updateSecurity(deal, sg);

                    wc.updateDeal(deal);

                    MessageBox.Show("Deal Updated", "Confirm", MessageBoxButtons.OK);
                    wc.AuditTrailUpdate(usr, deal);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Update or Assign new Lead Analyst to current Deal
        /// </summary>
        /// <param name="deal">The deal.</param>
        private void updateLeadAnalyst(Deal deal)
        {

            if (cbxLeadAnalyst.SelectedValue != null)
            {
                MapDealAnalyst[] dealteam = wc.getDealTeam(deal.DealID);
                Boolean updatedLeadAnalyst = false;
                foreach (MapDealAnalyst mm in dealteam)
                {
                    if ((bool)mm.IsLeadAnalyst)
                    {
                        mm.AnalystID = (int)cbxLeadAnalyst.SelectedValue;
                        wc.updateMapDealAnalyst(mm);
                        updatedLeadAnalyst = true;
                    }
                }
                if (dealteam.Length == 0 || !updatedLeadAnalyst)
                {
                    addNewAnalyst(deal, true);
                }
            }
        }

        /// <summary>
        /// Update or Assign new Security to current Deal
        /// </summary>
        /// <param name="deal">The deal.</param>
        /// <param name="sg">The sg.</param>
        private void updateSecurity(Deal deal, SecurityGroup sg)
        {
            int sec1id = (int)cbxSecurity1.SelectedValue;

            sg.SecurityID1 = sec1id;
            sg.SecurityID2 = (int)(sg.SecurityID2 == 0 || sg.SecurityID2 == null ? 610 : sg.SecurityID2);
            sg.SecurityID3 = (int)(sg.SecurityID3 == 0 || sg.SecurityID3 == null ? 610 : sg.SecurityID3);
            sg.SecurityID4 = (int)(sg.SecurityID4 == 0 || sg.SecurityID4 == null ? 610 : sg.SecurityID4);
            sg.SecurityID5 = (int)(sg.SecurityID5 == 0 || sg.SecurityID5 == null ? 610 : sg.SecurityID5);
            if (sg.SecurityGroupID == 0)
            {
                sg.SecurityGroupID = wc.getLastSecurityGroup().SecurityGroupID + 1;
                wc.addSecurityGroup(sg);
                deal.SecurityGroupID = sg.SecurityGroupID;
            }
            else
            {
                wc.updateSecurityGroup(sg);
            }

            deal.DocumentGroupID = 100;
        }

        /// <summary>
        /// Method to handle updates to Deal Team
        /// </summary>
        /// <param name="deal">The deal.</param>
        /// <param name="isLead">The is lead.</param>
        private void addNewAnalyst(Deal deal, Boolean isLead = false)
        {
            try
            {
                MapDealAnalyst mda = new MapDealAnalyst()
                {
                    AnalystID = (int)cbxLeadAnalyst.SelectedValue,
                    DealID = deal.DealID,
                    IsLeadAnalyst = isLead
                };

                wc.addMapDealAnalyst(mda);
                usr.Actions = "[Analyst " + cbxLeadAnalyst.Text + " added to Deal " + deal.DealID + "]";
                wc.AuditTrailUpdate(usr, deal);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Event handler for top menu Go | Delete (CTRL+D)
        /// Removes Deal from database
        /// Goes back to Overview Grid and hide Deal definition screen
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //Check if Deal definition screen is loaded and user confirm
                if (gbOverview.Visible != true &&
                    MessageBox.Show("Remove Deal?", 
                    "Confirm", MessageBoxButtons.YesNo).ToString() == "Yes")
                {
                    Deal deal = getSelectedDeal();//get the Deal selected

                    wc.removeDealById(deal.DealID);//removed from database
                    wc.AuditTrailDelete(usr, deal);

                    //go back to Overview screen
                    tabCtrlDealDef.Visible = false;
                    gbOverview.Visible = true; 

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Event handler for top menu Go | Home (CTRL+H)
        /// Goes back to Overview Grid and hide Deal definition screen
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //wc.AuditTrailUpdate(usr, deal);
            tabCtrlDealDef.Visible = false;
            gbOverview.Visible = true;
        }

        /// <summary>
        /// Event handler for context menu item to Add New Company from Dropdown on Deal definition screen
        /// Opens blank Company form to enter a new Company
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void smiNewCompany_Click(object sender, EventArgs e)
        {
            menuCompany_Click(sender, e);
        }

        /// <summary>
        /// Event handler for context menu item to Add New Security from Dropdown on Deal definition screen
        /// Opens blank Security form to enter a new Security
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void smiAddSecurity_Click(object sender, EventArgs e)
        {
            menuSecurity_Click(sender, e);
        }

        /// <summary>
        /// Event handler for context menu item to Edit Company from Dropdown on Deal definition screen
        /// Opens Company form and load selected Company for edit
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void smiEditCompany_Click(object sender, EventArgs e)
        {
            int id = (int)cbxCompany1.SelectedValue;
            Company c = wc.getCompany(id);
            FormCompanyDefinition compForm = new FormCompanyDefinition(c);
            compForm.Visible = true;
        }

        /// <summary>
        /// Event handler for context menu item to Edit Security from Dropdown on Deal definition screen
        /// Opens Security form and load selected Security for edit
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void smiEditSecurity_Click(object sender, EventArgs e)
        {
            int id = (int)cbxSecurity1.SelectedValue;
            Security s = wc.getSecurity(id);

            FormSecurityDefinition secForm = new FormSecurityDefinition(s);
            secForm.Visible = true;
        }

        /// <summary>
        /// Event handler for context menu item to Refresh Company Dropdown on Deal definition screen
        /// reloads the dropdown list and jump back to the selected item
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void smiRefreshCompany_Click(object sender, EventArgs e)
        {
            int index = cbxCompany1.SelectedIndex;

            ListItem.loadListItems(cbxCompany1, ListItem.loadCompanyList(wc));

            cbxCompany1.SelectedIndex = index;
        }

        /// <summary>
        /// Event handler for context menu item to Refresh Security Dropdown on Deal definition screen
        /// reloads the dropdown list and jump back to the selected item
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void smiRefreshSecurity_Click(object sender, EventArgs e)
        {
            int index = cbxSecurity1.SelectedIndex;

            // TODO: This line of code loads data into the 'cortex_DevSecurityDS.Securities' table. You can move, or remove it, as needed.
            ListItem.loadListItems(cbxSecurity1, ListItem.loadSecurityList(wc));

            cbxSecurity1.SelectedIndex = index;
        }

        /// <summary>
        /// Event handler for Search button under Overview Grid to load Deals with matching Companies.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Company searchCompany = wc.getCompany((int)cbxSearchByCompany.SelectedValue);
                Deal[] dealFiltered = wc.getDealByCompany(searchCompany);

                if (dealFiltered != null)
                {
                    LoadDealList(dealFiltered);
                }
                else
                {
                    MessageBox.Show(
                        "Deal with Company Name \"" + searchCompany.Name + "\" Not Found",
                        "Confirm", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Load Deal List to Overview Grid
        /// </summary>
        /// <param name="dealFiltered">The deal filtered.</param>
        /// <param name="count">The count.</param>
        private void LoadDealList(Deal[] dealFiltered, int count = 0)
        {
            try
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                dataGridView1.Columns.Add("ID", "ID");
                dataGridView1.Columns.Add("Desc", "Description");
                dataGridView1.Columns.Add("Security", "Security");
                dataGridView1.Columns.Add("Current", "Current Price");
                dataGridView1.Columns.Add("Target", "Target Price");
                dataGridView1.Columns.Add("Downside", "Downside Price");
                dataGridView1.Columns.Add("Comp", "Comps");
                dataGridView1.Columns["ID"].Visible = false;
                dataGridView1.Columns["Desc"].Width = 350;
                dataGridView1.Columns["Downside"].Width = 125;
                dataGridView1.Columns["Comp"].Width = 250;

                Security sec = new Security(); sec.Code = "";
                Price pr = new Price(); pr.Price1 = (Decimal)1.00;
                String numeralFormat = "N4";
                int currentRow = 0;

                if (count == 0)
                {
                    dealFiltered = dealFiltered.OrderBy(x => x.Description).ToArray<Deal>(); 
                }
                else
                {
                    dealFiltered = dealFiltered.OrderBy(x => x.Description).Take(count).ToArray<Deal>(); 

                }
                foreach (Deal deal in dealFiltered)
                {
                    int id = wc.getSecurityGroup((int)deal.SecurityGroupID).SecurityID1;
                    sec = wc.getSecurity(id);
                    pr = wc.getPrice(id);
                    dataGridView1.Rows.Add(
                        deal.DealID,
                        deal.Description,
                        sec.Code,
                        pr.Price1.ToString(numeralFormat),
                        deal.TargetPrice.Value.ToString(numeralFormat),
                        deal.DownsidePrice.Value.ToString(numeralFormat),
                        deal.Comps);

                    if (deal.DealStatusID == 6) //highlight HOT deals
                    {
                        dataGridView1.Rows[currentRow].DefaultCellStyle.BackColor = Color.GreenYellow;
                    }
                    currentRow++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Event handler for context menu when you right-click on Overview Grid to reload Deal list
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void smiRefreshOverview_Click(object sender, EventArgs e)
        {
            LoadDealList(wc.getDeals(0));
        }

        /// <summary>
        /// Event handler for button under the Overview Grid to clear Company filters and reload Deals list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            LoadDealList(wc.getDeals(0));
        }

        /// <summary>
        /// Quit Application completely, Audit Trail will log Exit on the Login.o_FormClosing()
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wc.Close();
            System.Windows.Forms.Application.Exit();
        }

        /// <summary>
        /// Method handles Document selection dialog on the Documents tab
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void txtFileUpload_TextChanged_1(object sender, EventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "All Files (*.*)|*.*";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = true;

            if (choofdlog.ShowDialog() == DialogResult.OK)
                txtFileUpload.Text = choofdlog.FileName;
            else
                txtFileUpload.Text = string.Empty;       

        }

        /// <summary>
        /// Method handles Document upload logic
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtFileUpload.Text != String.Empty)
                {
                    //Get document path
                    String documentRepo = Properties.Settings.Default.DocumentFolder;
                    String URI = documentRepo + Path.GetFileName(txtFileUpload.Text);

                    //get Deal and build new Document object
                    Deal deal = getSelectedDeal();
                    Document d = new Document()
                    {
                        URI = URI,
                        Name = txtDocumentName.Text,
                        Description = txtDocumentDescription.Text,
                        DealId = deal.DealID,
                        DocumentID = wc.getLastDocument().DocumentID + 1
                    };
                    File.Copy(txtFileUpload.Text, URI, true); //save document to Deal repository
                    wc.addDocument(d); //saves Document details to database
                    MessageBox.Show("Document Saved", "Upload Confirm");

                    fillAllDocumentControls(wc, deal); //reload Document datagrid to reflect new document

                    //save audit trail of this action
                    usr.Actions = "[Document " + d.DocumentID + " uploaded to Deal " + deal.DealID + "]";
                    wc.AuditTrailUpdate(usr, deal);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Loads Deals matching selected Analyst from dropdown on Overview screen
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnSearchByAnalyst_Click(object sender, EventArgs e)
        {
            try
            {
                Analyst a = wc.getAnalystByName(cbxSearchByAnalyst.Text);
                Deal[] dealFiltered = wc.getDealByAnalyst(a);

                if (dealFiltered != null)
                {
                    LoadDealList(dealFiltered);
                }
                else
                {
                    MessageBox.Show(
                        "Deal with Analyst Name " + cbxSearchByAnalyst.SelectedText + "\n Not Found",
                        "Confirm", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method handles Analyst tab control to add Analyst to Deal Team
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnAddAnalyst_Click(object sender, EventArgs e)
        {
            try
            {
                //retrieve relevant objects for Deal/Analyst mapping
                int index = dgAnalystPool.CurrentRow.Index;
                Analyst a = wc.getAnalystByName(dgAnalystPool.Rows[index].Cells[0].Value.ToString());
                Deal d = getSelectedDeal();
                MapDealAnalyst mda = wc.getMapDealAnalyst(d.DealID, a.AnalystID);

                //block adding Lead analyst or duplicates to team
                if (cbxNewAnalyst.Text != cbxLeadAnalyst.Text
                    && mda.MapDealAnalystID == 0)
                {
                    mda = new MapDealAnalyst()
                    {
                        AnalystID = a.AnalystID,
                        DealID = d.DealID,
                        IsLeadAnalyst = false
                    };

                    //map Analyst to current Deal, Audit Trail records the action executed
                    wc.addMapDealAnalyst(mda);
                    usr.Actions = "[Analyst " + a.Login + " uploaded to Deal " + deal.DealID + "]";
                    wc.AuditTrailUpdate(usr, deal);

                    fillAnalystControls(wc, d);//refresh Deal Team list to reflect new Analyst
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// Method handles Event tab control to add Event to Deal
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnAddEvent_Click(object sender, EventArgs e)
        {
            try
            {
                //build the new Event object
                Deal deal = getSelectedDeal();
                Event ev = new Event()
                {
                    EventTypeID = (int)cbxEventType.SelectedValue,
                    Note = txtEventNote.Text,
                    Description = txtEventDescription.Text,
                    EventDate = dtpEventDate.Value,
                    DealID = deal.DealID,
                    EventID = wc.getLastEvent().EventID + 1
                };

                //Add event to Deal, refresh Event datagrid and records the action to Audit Trail
                wc.addEvent(ev);
                fillAllEventControls(wc, deal);
                usr.Actions = "[Event " + ev.EventID + " (" + ev.Description + ") " 
                    + " added to Deal " + ev.DealID + "]";
                wc.AuditTrailUpdate(usr, deal);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method handles Deal Team tab control to add Analyst to the Deal
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void dgAnalystPool_CellMouseDoubleClick(object sender, MouseEventArgs e)
        {
            btnAddAnalyst_Click(sender, e);
        }

        /// <summary>
        /// Method handles Right-Click menu on datagrids for Event, Documents and Analysts
        /// Removes the selected object from datagrid and database records
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cmiRemove_Click(object sender, EventArgs e)
        {
            try
            {
                int row, id;
                //remove Analyst from datagrid and database
                if (dgAnalysts.Focused)
                {
                    removeAnalyst(out row, out id);
                }

                //remove Event from datagrid and database
                if (dgEvents.Focused)
                {
                    row = dgEvents.CurrentRow.Index;
                    id = Convert.ToInt16(dgEvents.Rows[row].Cells["ID"].Value);
                    wc.removeEvent(id);
                    fillAllEventControls(wc, deal);
                    usr.Actions = "[Event " + id + " removed from Deal " + deal.DealID + "]";
                    wc.AuditTrailUpdate(usr, deal);
                }

                //remove Document from datagrid and database
                if (dgDocuments.Focused)
                {
                    row = dgDocuments.CurrentRow.Index;
                    id = Convert.ToInt16(dgDocuments.Rows[row].Cells["ID"].Value);
                    wc.removeDocument(id);
                    fillAllDocumentControls(wc, deal);
                    usr.Actions = "[Document " + id + " removed from Deal " + deal.DealID + "]";
                    wc.AuditTrailUpdate(usr, deal);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// remove Analyst from datagrid and database and records teh action to Audit Trail
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="id">The identifier.</param>
        private void removeAnalyst(out int row, out int id)
        {
            row = dgAnalysts.CurrentRow.Index;
            id = Convert.ToInt16(dgAnalysts.Rows[row].Cells["ID"].Value);

            try
            {
                wc.removeMapDealAnalyst(id);
                usr.Actions = "[Analyst " + analyst.Login + " removed from Deal " + deal.DealID + "]";
                wc.AuditTrailUpdate(usr, deal);
                fillAnalystControls(wc, deal);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method handles control on Deal Team tab to
        /// remove Analyst from datagrid and database and records teh action to Audit Trail
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnRemoveAnalyst_Click(object sender, EventArgs e)
        {
            int row, id;
            removeAnalyst(out row, out id);
        }

        /// <summary>
        /// Method handles Lead Analyst control on Deal definition screen
        /// records updates to this control to Audit Trail
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cbxLeadAnalyst_SelectedIndexChanged(object sender, EventArgs e)
        {
            //usr.Actions = "[Analyst " + cbxLeadAnalyst.Text + " Selected as Lead to Deal " + deal.DealID + "]";
            //wc.AuditTrailUpdate(usr, deal);
            lblLeadAnalyst.Text = cbxLeadAnalyst.Text;
        }

        /// <summary>
        /// Method handles control from Overview screen to filter Deal datagrid
        /// by Deals with matching Deal Status to the dropdown
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnSearchStatus_Click(object sender, EventArgs e)
        {
            try
            {
                DealStatus ds = wc.getDealStatus((int)cbxSearchStatus.SelectedValue);
                Deal[] dealFiltered = wc.getDealByStatus(ds);

                if (dealFiltered != null)
                {
                    LoadDealList(dealFiltered);
                }
                else
                {
                    MessageBox.Show(
                        "Deal with Status Name " + cbxSearchStatus.SelectedText + "\n Not Found",
                        "Confirm", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method handles URI link behavior on Document datagrid
        /// opens deal documents
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellEventArgs" /> instance containing the event data.</param>
        private void dgDocuments_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string filename = dgDocuments[e.ColumnIndex, e.RowIndex].Value.ToString();
                if (e.ColumnIndex == 0 && File.Exists(filename))
                {
                    Process.Start(filename);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method handles control from Overview screen to filter Deal datagrid
        /// by Deals with matching Deal Strategy Class to the dropdown
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnSearchCategoryClass_Click(object sender, EventArgs e)
        {
            try
            {
                Deal[] dealFiltered = wc.getDealsByCategoryClass(cbxSearchCategoryClass.Text);

                if (dealFiltered != null)
                {
                    LoadDealList(dealFiltered);
                }
                else
                {
                    MessageBox.Show(
                        "Deal with Status Name " + cbxSearchStatus.SelectedText + "\n Not Found",
                        "Confirm", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Search All function logic, search Deals by Class, Company, Status, Analyst and Deal data
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnSearchByDescription_Click(object sender, EventArgs e)
        {
            try
            {
                String desc = txtSearchByDescription.Text,
                    categoryClass = cbxSearchCategoryClass.Text,
                    analyst = cbxSearchByAnalyst.Text;
                int companyID = cbxSearchByCompany.Text == "" ? 0 : (int)cbxSearchByCompany.SelectedValue,
                    dealStatusID = cbxSearchStatus.Text == "" ? 0 : (int)cbxSearchStatus.SelectedValue;

                Cursor.Current = Cursors.WaitCursor;

                Deal[] dealByDesc = wc.getDealsByCriteria(
                    desc, companyID, dealStatusID, categoryClass, analyst);

                if (dealByDesc != null)
                {
                    LoadDealList(dealByDesc);
                }
                else
                {
                    MessageBox.Show(
                        "Deal with Status Name " + cbxSearchStatus.SelectedText + "\n Not Found",
                        "Confirm", MessageBoxButtons.OK);
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Handles Quick Import function.
        /// Read selected file template and loads data from each worksheet
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog choofdlog = new OpenFileDialog();
                choofdlog.Filter = "All Files (*.xls)|*.xls";
                choofdlog.FilterIndex = 1;
                choofdlog.Multiselect = false;
                String CortexLoader = "";

                if (choofdlog.ShowDialog() == DialogResult.OK)
                { 
                    CortexLoader = choofdlog.FileName;
                    //Show hour glass
                    Cursor.Current = Cursors.WaitCursor;

                    importSecurities(CortexLoader);
                    importCompanies(CortexLoader);
                    importDocuments(CortexLoader);
                    importDeals(CortexLoader);
                    importEvents(CortexLoader);
                    importMergerArb(CortexLoader);

                    //stop hour glass
                    Cursor.Current = Cursors.Default;

                    MessageBox.Show("Deal Data Imported", "Confirm");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Cursor.Current = Cursors.Default;

            }
        }

        /// <summary>
        /// logic to handle reading data from Merger Arb worksheet and load to Cortex DB
        /// </summary>
        /// <param name="CortexLoader">The cortex loader.</param>
        private void importMergerArb(String CortexLoader)
        {
            try
            {
                MyDt MergerArb = ExcelTools.parseExcel(CortexLoader, 1, 8);
                if (MergerArb.Columns.Count > 10)
                {
                    MergerArb ma = new MergerArb();

                    MergerArb.removeWhen(x => x["Field"].ToString().Equals("x"));
                    foreach (DataRow row in MergerArb.Rows)
                    {
                        String field, value;
                        field = "C" + row["Field"].ToString()
                            .Replace(".", "_")
                            .Replace(" ", "_")
                            .Replace("(", "_")
                            .Replace(")", "_")
                            ;
                        value = row["Value"].ToString() == null ? "N.A." : row["Value"].ToString();
                        foreach (PropertyInfo s in ma.GetType().GetProperties())
                        {
                            if (s.Name == field && s.CanWrite)
                            {
                                if (field.ToLower().Contains("dealid"))
                                    s.SetValue(ma, Convert.ToInt32(value), null);
                                else
                                    s.SetValue(ma, value, null);
                            }
                        }
                    }
                    wc.addMergerArb(ma); 
                }
                else
                {
                    MergerArbNew man;
                    foreach (DataRow row in MergerArb.Rows)
                    {
                        man = new MergerArbNew()
                        {
                            Field_Name = row["Field Name"].ToString(),
                            Field_Updated = DateTime.FromOADate(Double.Parse(row["Field Updated"].ToString())),
                            Field_Value = row["Field Value"].ToString(),
                            Field_Type = row["Field Type"].ToString(),
                            Calendar_Comment = row["Calendar Comment"].ToString(),
                            Calendar_Flag = row["Calendar Flag"].ToString(),
                            Calendar_Time = row["Calendar Time"].ToString()

                        };
                        if (MergerArb.Columns.Contains("DealId"))
                            man.DealId = Convert.ToInt16(row["DealId"].ToString()); //bulk upload logic
                        else
                            man.DealId = deal.DealID;

                        wc.addMergerArbNew(man);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// logic to handle reading data from Securities worksheet and load to Cortex DB
        /// </summary>
        /// <param name="CortexLoader">The cortex loader.</param>
        private void importSecurities(String CortexLoader)
        {
            try
            {
                MyDt sec = ExcelTools.parseExcel(CortexLoader, 1);
                foreach (DataRow dr in sec.Rows)
                {
                    Security newSec = new Security()
                    {
                        Code = dr["Code"].ToString(),
                        Name = dr["Name"].ToString(),
                        Description = dr["Description"].ToString(),
                        CurrencyID = (int)Convert.ToInt16(dr["CurrencyID"].ToString()),
                        SecurityTypeID = (int)Convert.ToInt16(dr["SecurityTypeID"].ToString()),
                        SecurityID = wc.getLastSecurity().SecurityID + 1,
                        Isin = dr["Isin"].ToString(),
                        Cusip = dr["Cusip"].ToString(),
                        Ticker = dr["Ticker"].ToString(),
                        Sedol = dr["Sedol"].ToString()
                    };

                    wc.addSecurity(newSec);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        /// <summary>
        /// logic to handle reading data from Documents worksheet and load to Cortex DB
        /// </summary>
        /// <param name="CortexLoader">The cortex loader.</param>
        private void importDocuments(String CortexLoader)
        {
            try
            {
                MyDt doc = ExcelTools.parseExcel(CortexLoader, 1, 4);
                foreach (DataRow dr in doc.Rows)
                {
                    Document newDoc = new Document()
                    {
                        DocumentID = wc.getLastDocument().DocumentID + 1,
                        URI = dr["URI"].ToString(),
                        Name = dr["Name"].ToString(),
                        Description = dr["Description"].ToString(),
                        DealId = (int)Convert.ToInt16(dr["DealId"].ToString())
                    };
                    wc.addDocument(newDoc);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        /// <summary>
        /// logic to handle reading data from Companies worksheet and load to Cortex DB
        /// </summary>
        /// <param name="CortexLoader">The cortex loader.</param>
        private void importCompanies(String CortexLoader)
        {
            try
            {
                MyDt comp = ExcelTools.parseExcel(CortexLoader, 1, 3);
                foreach (DataRow dr in comp.Rows)
                {
                    Company newComp = new Company()
                    {
                        CompanyID = wc.getLastCompany().CompanyID + 1,
                        Code = dr["Code"].ToString(),
                        Name = dr["Name"].ToString(),
                        Description = dr["Description"].ToString()
                    };
                    wc.addCompany(newComp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        /// <summary>
        /// logic to handle reading data from Deals worksheet and load to Cortex DB
        /// </summary>
        /// <param name="CortexLoader">The cortex loader.</param>
        private void importDeals(String CortexLoader)
        {
            try
            {
                MyDt deal = ExcelTools.parseExcel(CortexLoader, 1, 6);
                foreach (DataRow dr in deal.Rows)
                {
                    int currID = wc.getCurrencyByCodeOrName(dr["CurrencyID"].ToString()).CurrencyID;
                    Category[] cat = wc.getCategoriesByName(dr["Category"].ToString());
                    int catID = cat.Length > 0 ? cat[0].CategoryID : 1;
                    int StatusID = wc.getDealStatusByName(dr["DealStatus"].ToString()).DealStatusID;
                    int compID = wc.getCompanyByName(dr["Company Name"].ToString()).CompanyID;

                    Deal d = new Deal();
                    d.DealID = wc.getLastDeal().DealID + 1;
                    d.Description = Convert.ToString(dr["Description"].ToString());
                    d.CategoryID = catID;
                    d.DealStatusID = StatusID;
                    d.CurrencyID = currID;
                    d.DealCurrencyID = currID;
                    d.TargetPrice = Convert.ToDecimal(dr["TargetPrice"].ToString());
                    d.DownsidePrice = Convert.ToDecimal(dr["DownsidePrice"].ToString());
                    d.ExpirationDate = DateTime.FromOADate(Convert.ToDouble(dr["ExpirationDate"].ToString()));
                    d.DealTypeID = Convert.ToInt16(dr["DealTypeID"].ToString());
                    d.Comps = Convert.ToString(dr["Comps"].ToString());
                    d.DownsidePriceValuation = Convert.ToString(dr["DownsidePriceValuation"].ToString());
                    d.Background = Convert.ToString(dr["Background"].ToString());
                    d.ValuationMethodology = Convert.ToString(dr["ValuationMethodology"].ToString());
                    d.KeyRisks = Convert.ToString(dr["KeyRisks"].ToString());
                    d.Catalyst = Convert.ToString(dr["Catalyst"].ToString());
                    d.CurrentValuation = Convert.ToString(dr["CurrentValuation"].ToString());
                    d.TargetPriceValuation = Convert.ToString(dr["TargetPriceValuation"].ToString());
                    d.Recommendation = Convert.ToString(dr["Recommendation"].ToString());
                    d.CompanyID1 = compID;

                    int newSecGroupId = wc.getLastSecurityGroup().SecurityGroupID + 100;

                    SecurityGroup newSG = new SecurityGroup()
                    {
                        SecurityGroupID = newSecGroupId,
                        SecurityID1 = wc.getSecurityByCode(dr["SecurityCode"].ToString()).SecurityID
                    };

                    wc.addSecurityGroup(newSG);

                    d.SecurityGroupID = newSecGroupId;

                    wc.addDeal(d);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        /// <summary>
        /// logic to handle reading data from Events worksheet and load to Cortex DB
        /// </summary>
        /// <param name="CortexLoader">The cortex loader.</param>
        private void importEvents(String CortexLoader)
        {
            try
            {
                MyDt ev = ExcelTools.parseExcel(CortexLoader, 1, 7);
                foreach (DataRow dr in ev.Rows)
                {
                    Event newEvent = new Event();
                    newEvent.EventID = Convert.ToInt16(dr["EventID"].ToString());
                    newEvent.EventDate = DateTime.FromOADate(Double.Parse(dr["EventDate"].ToString()));
                    newEvent.Description = Convert.ToString(dr["Description"].ToString());
                    newEvent.EventTypeID = Convert.ToInt16(dr["EventTypeID"].ToString());
                    newEvent.Note = Convert.ToString(dr["Note"].ToString());
                    newEvent.DealID = Convert.ToInt16(dr["DealID"].ToString());
                    wc.addEvent(newEvent);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }


        /// <summary>
        /// logic to handle reading data from Companies worksheet and load to Cortex DB
        /// </summary>
        /// <param name="CortexLoader">The cortex loader.</param>
        public static void importPrices(String CortexLoader)
        {
            try
            {
                MyDt comp = CortexLoader.EndsWith(".csv") ? 
                    ReconTools.ReconUtils.loadCSVReport(CortexLoader,"CortexPrices") : 
                    ExcelTools.parseExcel(CortexLoader, 1);

                CortexWCFServiceClient wc1 = new CortexWCFServiceClient();

                foreach (DataRow dr in comp.Rows)
                {
                    try
                    {
                        DateTime d = CortexLoader.EndsWith(".csv") ?
                            DateTime.Parse(dr["PriceDateTime"].ToString()) : 
                            DateTime.FromOADate(Convert.ToDouble(dr["PriceDateTime"].ToString()));
                        Price pr = new Price()
                        {
                            SecurityID = wc1.getSecurityByCode(dr["SecurityID"].ToString()).SecurityID,
                            Price1 = Convert.ToDecimal(dr["Price"].ToString()),
                            PriceDateTime = d,
                            PriceSource = dr["PriceSource"].ToString()
                        };
                        wc1.addPrice(pr);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
        /// <summary>
        /// Handles the KeyPress event of the txtSearchByDescription control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyPressEventArgs" /> instance containing the event data.</param>
        private void txtSearchByDescription_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter) btnSearchByDescription_Click(sender, e);
        }

        /// <summary>
        /// Handles the Keypress event of the txtSearchCategoryClass control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyPressEventArgs" /> instance containing the event data.</param>
        private void txtSearchCategoryClass_Keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.F2) cbxSearchCategoryClass.Text = "";

        }

        /// <summary>
        /// Handles the KeyPress event of the cbxSearchStatus control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyPressEventArgs" /> instance containing the event data.</param>
        private void cbxSearchStatus_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.F2) cbxSearchStatus.Text = "";

        }

        /// <summary>
        /// Handles the KeyPress event of the cbxSearchByCompany control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyPressEventArgs" /> instance containing the event data.</param>
        private void cbxSearchByCompany_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.F2) cbxSearchByCompany.Text = "";

        }

        /// <summary>
        /// Handles the KeyPress event of the cbxSearchByAnalyst control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyPressEventArgs" /> instance containing the event data.</param>
        private void cbxSearchByAnalyst_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.F2) cbxSearchByAnalyst.Text = "";

        }

        /// <summary>
        /// Handles logic to update each modified fields on the Merger Arb tab.
        /// Gets called when user updates the current Deal
        /// </summary>
        private void updateMergArb()
        {
            try
            {
                mb = wc.getMergerArbByDealId(deal.DealID);
                int rowIndex = 1;
                String value;
                if (mb != null)
                {
                    foreach (PropertyInfo s in mb.GetType().GetProperties())
                    {
                        if (s.CanWrite
                            && s.Name != "ExtensionData"
                            && s.Name != "MergerArbID"
                            && !s.Name.Contains("DealId")
                            )
                        {
                            value = dgvMergArb[1, rowIndex].Value != null
                                ? dgvMergArb[1, rowIndex].Value.ToString()
                                : "";

                            s.SetValue(mb, value, null);

                            rowIndex++;

                        }
                    }
                    wc.updateMergerArb(mb);
                }
                else
                {
                    nmb = wc.getMergerArbNewByDealId(deal.DealID);
                    int id;
                    foreach (MergerArbNew mm in nmb)
                    {
                        for (rowIndex = 1; rowIndex < dgvMergArb.Rows.Count; rowIndex++)
                        {
                            id = (int)dgvMergArb["ID", rowIndex].Value;
                            if(id == mm.ID)
                            {
                                if(mm.Field_Value != (String)dgvMergArb["[Field Value]",rowIndex].Value ||
                                    mm.Calendar_Time != (String)dgvMergArb["[Calendar Time]", rowIndex].Value ||
                                    mm.Calendar_Comment != (String)dgvMergArb["[Calendar Comment]", rowIndex].Value)
                                {
                                    mm.Field_Updated = DateTime.Now;
                                }

                                mm.Field_Value = (String)dgvMergArb["[Field Value]",rowIndex].Value;
                                mm.Calendar_Time = (String)dgvMergArb["[Calendar Time]", rowIndex].Value;
                                mm.Calendar_Comment = (String)dgvMergArb["[Calendar Comment]", rowIndex].Value;
                            }
                        }
                        wc.updateMergerArbNew(mm);
                    }
                }
            }
            catch(NullReferenceException nullex)
            {
                throw nullex;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the txtEventNote control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void txtEventNote_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "All Files (*.*)|*.*";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = true;

            if (choofdlog.ShowDialog() == DialogResult.OK)
                txtEventNote.Text += " " + choofdlog.FileName;
        }

        /// <summary>
        /// Handles the CellDoubleClick event of the dgEvents control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellEventArgs" /> instance containing the event data.</param>
        private void dgEvents_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string filename = ExtractPathFromLine(
                dgEvents[e.ColumnIndex, e.RowIndex].Value.ToString());

            if (e.ColumnIndex == 2 && File.Exists(filename))
            {
                Process.Start(filename);
            }
        }

        /// <summary>
        /// Extracts the path from line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>System.String.</returns>
        static string ExtractPathFromLine(string line)
        {
            
            Regex PathRegex = new Regex(@"^[^ \t]+[ \t]+(.*)$");
            Match match = PathRegex.Match(line);
            if (!match.Success)
            {
                //throw new ArgumentException("Invalid line");
                return "";
            }
            return match.Groups[1].Value;
        }
        /// <summary>
        /// The is shown
        /// </summary>
        private bool isShown = false;
        /// <summary>
        /// Handles the MouseMove event of the txtCurrentPrice control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void txtCurrentPrice_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isShown)
            {
                toolTip1.Show(currentPriceDate.ToShortDateString(), txtCurrentPrice, e.Location);
                isShown = true;
            }
            else
            {
                toolTip1.Hide(txtCurrentPrice);
                isShown = false;
            }
        }

        //speed up GUI home screen loading time
        #region Click to Load ComboBox Items
        /// <summary>
        /// Handles the Click event of the cbxSearchStatus control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cbxSearchStatus_Click(object sender, EventArgs e)
        {
            if (cbxSearchStatus.Items.Count == 0)
            {
                if (dealStatusList == null) dealStatusList = ListItem.loadDealStatusList(wc);
                ListItem.loadListItems(cbxSearchStatus, dealStatusList);
            }
        }

        /// <summary>
        /// Handles the Click event of the cbxStatus control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cbxStatus_Click(object sender, EventArgs e)
        {
            if (cbxStatus.Items.Count == 0)
            {
                if (dealStatusList == null) dealStatusList = ListItem.loadDealStatusList(wc);
                ListItem.loadListItems(cbxStatus, dealStatusList);
            }
        }

        /// <summary>
        /// Handles the Click event of the cbxCategory control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cbxCategory_Click(object sender, EventArgs e)
        {
            if (cbxCategory.Items.Count == 0)
            {
                ListItem.loadListItems(cbxCategory, ListItem.loadCategoryList(wc));
            }
        }

        /// <summary>
        /// Handles the Click event of the cbxSearchCategoryClass control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cbxSearchCategoryClass_Click(object sender, EventArgs e)
        {
            if (cbxSearchCategoryClass.Items.Count == 0)
            {
                ListItem.loadListItems(cbxSearchCategoryClass, ListItem.loadClassList(wc));
            }

        }

        /// <summary>
        /// Handles the Click event of the cbxCompany1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cbxCompany1_Click(object sender, EventArgs e)
        {
            if (cbxCompany1.Items.Count == 0)
            {
                if (compList == null) compList = ListItem.loadCompanyList(wc);
                ListItem.loadListItems(cbxCompany1, compList);
            }
        }

        /// <summary>
        /// Handles the Click event of the cbxSecurity1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cbxSecurity1_Click(object sender, EventArgs e)
        {

            if (cbxSecurity1.Items.Count == 0)
            {
                ListItem.loadListItems(cbxSecurity1, ListItem.loadSecurityList(wc));
            }

        }

        /// <summary>
        /// Handles the Click event of the cbxLeadAnalyst control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cbxLeadAnalyst_Click(object sender, EventArgs e)
        {
            if (cbxLeadAnalyst.Items.Count == 0)
            {
                if(analystList == null) analystList = ListItem.loadAnalystList(wc);
                ListItem.loadListItems(cbxLeadAnalyst, analystList);
            }

        }

        /// <summary>
        /// Handles the Click event of the cbxSearchByCompany control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cbxSearchByCompany_Click(object sender, EventArgs e)
        {
            if (cbxSearchByCompany.Items.Count == 0)
            {
                if (compList == null) compList = ListItem.loadCompanyList(wc);
                ListItem.loadListItems(cbxSearchByCompany, compList);
            }
        }

        /// <summary>
        /// Handles the Click event of the cbxSearchByAnalyst control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cbxSearchByAnalyst_Click(object sender, EventArgs e)
        {
            if (cbxSearchByAnalyst.Items.Count == 0)
            {
                if (analystList == null) analystList = ListItem.loadAnalystList(wc); 
                ListItem.loadListItems(cbxSearchByAnalyst, analystList);
            }
        }

        /// <summary>
        /// Handles the Click event of the cbxEventType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cbxEventType_Click(object sender, EventArgs e)
        {
            if (cbxEventType.Items.Count == 0)
            {
                ListItem.loadListItems(cbxEventType, ListItem.loadEventTypes(wc));
            }

        }

        /// <summary>
        /// Handles the Click event of the cbxCurrencyID control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cbxCurrencyID_Click(object sender, EventArgs e)
        {
            if (cbxCurrencyID.Items.Count == 0)
            {
                ListItem.loadListItems(cbxCurrencyID, ListItem.loadCCYList(wc));
            }

        }

        /// <summary>
        /// Handles the Enter event of the tabDealTeam control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void tabDealTeam_Enter(object sender, EventArgs e)
        {
            if (dgAnalystPool.Rows.Count == 0)
            {
                if (analystList == null) analystList = ListItem.loadAnalystList(wc);
                //ListItem.loadListItems(cbxLeadAnalyst, analystList);
                //ListItem.loadListItems(cbxSearchByAnalyst, analystList);
                ListItem.loadListItems(dgAnalystPool, analystList);
                dgAnalystPool.Columns[0].Name = "Login";
                dgAnalystPool.Columns[0].HeaderText = "Login";
                dgAnalystPool.Columns[1].Visible = false; 
            }
        }
        #endregion

        /// <summary>
        /// Handles the Click event of the allToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://nyvmdevs1/Reports/Pages/Report.aspx?ItemPath=%2fCortex_Summary&ViewMode=Detail");
        }

        /// <summary>
        /// Handles the Click event of the byNameToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void byNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://nyvmdevs1/Reports/Pages/Report.aspx?ItemPath=%2fCortex_DealDefinition&ViewMode=Detail");
        }

        /// <summary>
        /// Handles the Click event of the byFilterToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void byFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://nyvmdevs1/Reports/Pages/Report.aspx?ItemPath=%2fCortex_Search&ViewMode=Detail");
        }

        /// <summary>
        /// Handles the Click event of the allToolStripMenuItem1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void allToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://nyvmdevs1/Reports/Pages/Report.aspx?ItemPath=%2fMergerArb_Summary&ViewMode=Detail");

        }

        /// <summary>
        /// Handles the Click event of the byDealToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void byDealToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://nyvmdevs1/Reports/Pages/Report.aspx?ItemPath=%2fMergerArb_SummaryDetail&ViewMode=Detail");

        }

    }
}
