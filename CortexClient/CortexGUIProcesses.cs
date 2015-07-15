// ***********************************************************************
// Assembly         : Cortex
// Author           : ktam
// Created          : 05-19-2015
//
// Last Modified By : ktam
// Last Modified On : 05-19-2015
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
using RyanUtils;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using System.Data.SqlClient;

/// <summary>
/// The CortexClient namespace.
/// </summary>
namespace CortexClient
{
    /// <summary>
    /// Class CortexGUIProcesses. Logic layer for GUI controls
    /// </summary>
    public class CortexGUIProcesses
    {
        /// <summary>
        /// Extracts the path from line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>System.String.</returns>
        public static string ExtractPathFromLine(string line)
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
        /// logic to handle reading data from Companies worksheet and load to Cortex DB
        /// </summary>
        /// <param name="CortexLoader">The cortex loader.</param>
        public static void importPrices(String CortexLoader)
        {
            try
            {
                MyDt comp = CortexLoader.EndsWith(".csv") ?
                    ReconTools.ReconUtils.loadCSVReport(CortexLoader, "CortexPrices") :
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
        /// logic to handle reading data from Events worksheet and load to Cortex DB
        /// </summary>
        /// <param name="CortexLoader">The cortex loader.</param>
        /// <param name="wc">The wc.</param>
        public static void importEvents(String CortexLoader, CortexWCFServiceClient wc)
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
        /// logic to handle reading data from Securities worksheet and load to Cortex DB
        /// </summary>
        /// <param name="CortexLoader">The cortex loader.</param>
        /// <param name="wc">The wc.</param>
        public static void importSecurities(String CortexLoader, CortexWCFServiceClient wc)
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
        /// <param name="wc">The wc.</param>
        public static void importDocuments(String CortexLoader, CortexWCFServiceClient wc)
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
        /// <param name="wc">The wc.</param>
        public static void importCompanies(String CortexLoader, CortexWCFServiceClient wc)
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
        /// <param name="wc">The wc.</param>
        public static void importDeals(String CortexLoader, CortexWCFServiceClient wc)
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
        /// logic to handle reading data from Merger Arb worksheet and load to Cortex DB
        /// </summary>
        /// <param name="CortexLoader">The cortex loader.</param>
        /// <param name="wc">The wc.</param>
        /// <param name="deal">The deal.</param>
        public static void importMergerArb(String CortexLoader, CortexWCFServiceClient wc, Deal deal)
        {
            try
            {
                #region Legacy Code
                //MyDt MergerArb = ExcelTools.parseExcel(CortexLoader, 1, 8);
                //if (MergerArb.Columns.Count > 10)
                //{
                //    MergerArb ma = new MergerArb();

                //    MergerArb.removeWhen(x => x["Field"].ToString().Equals("x"));
                //    foreach (DataRow row in MergerArb.Rows)
                //    {
                //        String field, value;
                //        field = "C" + row["Field"].ToString()
                //            .Replace(".", "_")
                //           .Replace(" ", "_")
                //            .Replace("(", "_")
                //            .Replace(")", "_")
                //            ;
                //        value = row["Value"].ToString() == null ? "N.A." : row["Value"].ToString();
                //        foreach (PropertyInfo s in ma.GetType().GetProperties())
                //        {
                //            if (s.Name == field && s.CanWrite)
                //            {
                //                if (field.ToLower().Contains("dealid"))
                //                    s.SetValue(ma, Convert.ToInt32(value), null);
                //                else
                //                    s.SetValue(ma, value, null);
                //            }
                //        }
                //    }
                //    wc.addMergerArb(ma);
                //}
                //else
                //{
                //    MergerArbNew man;
                //    foreach (DataRow row in MergerArb.Rows)
                //    {
                //        man = new MergerArbNew()
                //        {
                //            Field_Name = row["Field Name"].ToString(),
                //            Field_Updated = DateTime.FromOADate(Double.Parse(row["Field Updated"].ToString())),
                //            Field_Value = row["Field Value"].ToString(),
                //            Field_Type = row["Field Type"].ToString(),
                //            Calendar_Comment = row["Calendar Comment"].ToString(),
                //            Calendar_Flag = row["Calendar Flag"].ToString(),
                //            Calendar_Time = row["Calendar Time"].ToString()

                //        };
                //        if (MergerArb.Columns.Contains("DealId"))
                //            man.DealId = Convert.ToInt16(row["DealId"].ToString()); //bulk upload logic
                //        else
                //            man.DealId = deal.DealID;

                //        wc.addMergerArbNew(man);
                //    }
                //} 
                #endregion

                MyDt Dealbook = new MyDt();
                ExcelPackage ep = new ExcelPackage(new FileInfo(CortexLoader));
                ExcelWorksheet dealbook;

                dealbook = ep.Workbook.Worksheets["Data"];

                if(dealbook.Index == 1)
                {
                    dealbook = ep.Workbook.Worksheets["DEALBOOK"];

                }
                MergerArbNew man;
                MergerArbNew[] maList;
                
                maList = wc.getMergerArbNewByDealId(deal.DealID);
                Dealbook = ExcelTools.parseExcel(CortexLoader, 1, dealbook.Index);

                if (maList.Length > 0) wc.removeMergerArbNewByDealID(deal.DealID);

                foreach (DataRow row in Dealbook.Rows)
                {
                    man = new MergerArbNew()
                    {
                        Field_Name = row["Field_Name_Detail"].ToString(),
                        Field_Updated = DateTime.FromOADate(Double.Parse(row["Field_Updated_Audit_Trail"].ToString())),
                        Field_Value = row["Field_Value"].ToString(),
                        Field_Type = row["Field_Type"].ToString(),
                        Calendar_Comment = row["Calendar_Comment"].ToString(),
                        Calendar_Flag = row["Calendar_Flag"].ToString(),
                        Calendar_Time = row["Calendar_Time"].ToString(),
                        DealId = deal.DealID

                    };

                    wc.addMergerArbNew(man);
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
        /// <param name="dataGridView1">The data grid view1.</param>
        /// <param name="wc">The wc.</param>
        /// <param name="count">The count.</param>
        public static void LoadDealList(Deal[] dealFiltered, DataGridView dataGridView1, 
            CortexWCFServiceClient wc, int count = 0)
        {
            try
            {
                #region MyRegion
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
                #endregion
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
        /// <param name="cbxLeadAnalyst">The CBX lead analyst.</param>
        /// <param name="wc">The wc.</param>
        /// <param name="usr">The usr.</param>
        public static void updateLeadAnalyst(Deal deal, ComboBox cbxLeadAnalyst, CortexWCFServiceClient wc, ApplicationUser usr)
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
                    addNewAnalyst(deal, cbxLeadAnalyst,wc,usr, true);
                }
            }
        }

        /// <summary>
        /// Update or Assign new Security to current Deal
        /// </summary>
        /// <param name="deal">The deal.</param>
        /// <param name="sg">The sg.</param>
        /// <param name="cbxSecurity1">The CBX security1.</param>
        /// <param name="wc">The wc.</param>
        public static void updateSecurity(Deal deal, SecurityGroup sg, ComboBox cbxSecurity1, CortexWCFServiceClient wc)
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
        /// <param name="cbxLeadAnalyst">The CBX lead analyst.</param>
        /// <param name="wc">The wc.</param>
        /// <param name="usr">The usr.</param>
        /// <param name="isLead">The is lead.</param>
        public static void addNewAnalyst(Deal deal, ComboBox cbxLeadAnalyst, CortexWCFServiceClient wc, ApplicationUser usr, 
            Boolean isLead = false)
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
        /// retrieve selected Deal info to memory from user double-click on Overview Grid
        /// </summary>
        /// <param name="dataGridView1">The data grid view1.</param>
        /// <param name="wc">The wc.</param>
        /// <param name="frm">The FRM.</param>
        /// <param name="deal">The deal.</param>
        /// <returns>Deal.</returns>
        public static Deal getSelectedDeal(DataGridView dataGridView1, CortexWCFServiceClient wc, Form frm, Deal deal = null)
        {
            try
            {
                if (deal == null)
                {
                    int row = dataGridView1.CurrentRow.Index;

                    int d =
                        Convert.ToInt16(dataGridView1.Rows[row].Cells["ID"].Value);

                    deal = wc.getDeal(d);
                    frm.Text = "Deal: " + d + " - " + dataGridView1.Rows[row].Cells[1].Value;
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
        /// Handles logic to update each modified fields on the Merger Arb tab.
        /// Gets called when user updates the current Deal
        /// </summary>
        /// <param name="deal">The deal.</param>
        /// <param name="wc">The wc.</param>
        /// <param name="dgvMergArb">The DGV merg arb.</param>
        public static void updateMergArb(Deal deal, CortexWCFServiceClient wc, DataGridView dgvMergArb)
        {
            try
            {
                MergerArb mb = wc.getMergerArbByDealId(deal.DealID);
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
                    MergerArbNew[] nmb = wc.getMergerArbNewByDealId(deal.DealID);
                    int id;
                    foreach (MergerArbNew mm in nmb)
                    {
                        for (rowIndex = 1; rowIndex < dgvMergArb.Rows.Count; rowIndex++)
                        {
                            id = (int)dgvMergArb["ID", rowIndex].Value;
                            if (id == mm.ID)
                            {
                                if (mm.Field_Value != (String)dgvMergArb["[Field Value]", rowIndex].Value ||
                                    mm.Calendar_Time != (String)dgvMergArb["[Calendar Time]", rowIndex].Value ||
                                    mm.Calendar_Comment != (String)dgvMergArb["[Calendar Comment]", rowIndex].Value)
                                {
                                    mm.Field_Updated = DateTime.Now;
                                }

                                mm.Field_Value = (String)dgvMergArb["[Field Value]", rowIndex].Value;
                                mm.Calendar_Time = (String)dgvMergArb["[Calendar Time]", rowIndex].Value;
                                mm.Calendar_Comment = (String)dgvMergArb["[Calendar Comment]", rowIndex].Value;
                            }
                        }
                        wc.updateMergerArbNew(mm);
                    }
                }
            }
            catch (NullReferenceException nullex)
            {
                throw nullex;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Shows the security group.
        /// </summary>
        /// <param name="cbxSec">The CBX sec.</param>
        /// <param name="lblDescr">The label description.</param>
        /// <param name="lblName">Name of the label.</param>
        /// <param name="wc">The wc.</param>
        public static void showSecurityGroup(ComboBox cbxSec, Label lblDescr, Label lblName, CortexWCFServiceClient wc)
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
        /// <param name="cbxSec">The CBX sec.</param>
        /// <param name="lblName">Name of the label.</param>
        /// <param name="wc">The wc.</param>
        public static void showCurrency(ComboBox cbxSec, Label lblName, CortexWCFServiceClient wc)
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

        /// <summary>
        /// Runs Full Import update SQL Job
        /// </summary>
        public static void fullImportUpdate()
        {
            try
            {
                SqlConnection connNYVMDEVS1 = new SqlConnection();
                String connNYVMDEVS1_String = Properties.Settings.Default.Cortex_DevConnectionString;
                DataSet dsNYVMDEVS1 = new DataSet();
                SqlCommand sqlComm = new SqlCommand();
                //Data Source=nyvmdevs1,1439;Initial Catalog=Cortex_Dev;Integrated Security=True
                //connNYVMDEVS1_String = connNYVMDEVS1_String.Replace("","");

                connNYVMDEVS1.ConnectionString = connNYVMDEVS1_String;
                if (connNYVMDEVS1.State == ConnectionState.Closed) connNYVMDEVS1.Open();

                sqlComm.CommandText = "EXEC msdb.dbo.sp_start_job 'Cortex_Import_DEALBOOK'";
                sqlComm.CommandType = CommandType.Text;
                sqlComm.Connection = connNYVMDEVS1;

                String progress = sqlComm.ExecuteNonQuery().ToString();

                connNYVMDEVS1.Close();

                MessageBox.Show("Import Complete");
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message);
            }

        }
    }

}
