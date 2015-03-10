using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CortexClient.ServiceReference1;

namespace CortexClient
{
    /// <summary>
    /// Custom class definition to help load Entity Objects into Cortex controls
    /// </summary>
    public class ListItem
    {
        public string Text { get; set; }
        public object Value { get; set; }


        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// populate Combo Box
        /// </summary>
        /// <param name="cbx"></param>
        /// <param name="list"></param>
        public static void loadListItems(ComboBox cbx, List<ListItem> list)
        {
            try
            {
                cbx.DisplayMember = "Text";
                cbx.ValueMember = "Value";
                List<ListItem> newList = new List<ListItem>();
                newList.AddRange(list);
                cbx.DataSource = newList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// populate DataGrid control
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="list"></param>
        public static void loadListItems(DataGridView dgv, List<ListItem> list)
        {
            try
            {
                List<ListItem> newList = new List<ListItem>();
                newList.AddRange(list);
                dgv.DataSource = newList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// retrieve list of Security Types
        /// </summary>
        /// <param name="wc"></param>
        /// <returns></returns>
        public static List<ListItem> loadTypeList(CortexWCFServiceClient wc)
        {
            SecurityType[] types = wc.getSecurityTypes();
            List<ListItem> typeList = new List<ListItem>();
            foreach (SecurityType t in types)
            {
                ListItem item = new ListItem()
                {
                    Text = t.Name,
                    Value = t.SecurityTypeID
                };
                typeList.Add(item);
            }

            return typeList;
        }

        /// <summary>
        /// retrieve list of Currencies
        /// </summary>
        /// <param name="wc"></param>
        /// <returns></returns>
        public static List<ListItem> loadCCYList(CortexWCFServiceClient wc)
        {
            Currency[] types = wc.getCurrencies();
            List<ListItem> ccyList = new List<ListItem>();
            foreach (Currency t in types)
            {
                ListItem item = new ListItem()
                {
                    Text = t.CurrencyCode,
                    Value = t.CurrencyID
                };
                ccyList.Add(item);
            }

            return ccyList;
        }

        /// <summary>
        /// retrieve list of Deal Status values
        /// </summary>
        /// <param name="wc"></param>
        /// <returns></returns>
        public static List<ListItem> loadDealStatusList(CortexWCFServiceClient wc)
        {
            DealStatus[] types = wc.getDealStatuses();
            List<ListItem> dealStatusList = new List<ListItem>();
            foreach (DealStatus t in types)
            {
                ListItem item = new ListItem()
                {
                    Text = t.Code,
                    Value = t.DealStatusID
                };
                dealStatusList.Add(item);
            }

            return dealStatusList;
        }

        /// <summary>
        /// retrieve list of Event Types
        /// </summary>
        /// <param name="wc"></param>
        /// <returns></returns>
        public static List<ListItem> loadEventTypes(CortexWCFServiceClient wc)
        {
            EventType[] ets = wc.getEventTypes();
            List<ListItem> eventTypeList = new List<ListItem>();
            foreach (EventType et in ets)
            {
                ListItem ee = new ListItem()
                {
                    Text = et.Code,
                    Value = et.EventTypeID
                };
                eventTypeList.Add(ee);
            }

            return eventTypeList;
        }

        /// <summary>
        /// retrieve list of Analysts
        /// </summary>
        /// <param name="wc"></param>
        /// <returns></returns>
        public static List<ListItem> loadAnalystList(CortexWCFServiceClient wc)
        {
            Analyst[] analysts = wc.getAnalysts();
            List<ListItem> analystList = new List<ListItem>();
            foreach (Analyst a in analysts)
            {
                ListItem an = new ListItem()
                {
                    Text = a.Login,
                    Value = a.AnalystID
                };
                analystList.Add(an);
            }

            return analystList;
        }

        /// <summary>
        /// retrieve list of Securities
        /// </summary>
        /// <param name="wc"></param>
        /// <returns></returns>
        public static List<ListItem> loadSecurityList(CortexWCFServiceClient wc)
        {
            Security[] securities = wc.getSecurities();
            List<ListItem> secList = new List<ListItem>();
            foreach (Security s in securities)
            {
                ListItem sec = new ListItem()
                {
                    Text = s.Code,
                    Value = s.SecurityID
                };
                secList.Add(sec);
            }

            return secList;
        }

        /// <summary>
        /// retrieve list of Companies
        /// </summary>
        /// <param name="wc"></param>
        /// <returns></returns>
        public static List<ListItem> loadCompanyList(CortexWCFServiceClient wc)
        {
            Company[] companies = wc.getCompanies();
            List<ListItem> compList = new List<ListItem>();
            foreach (Company c in companies)
            {
                ListItem comp = new ListItem()
                {
                    Text = c.Description,
                    Value = c.CompanyID
                };
                compList.Add(comp);
            }

            return compList;
        }

        /// <summary>
        /// retrieve list of Categories
        /// </summary>
        /// <param name="wc"></param>
        /// <returns></returns>
        public static List<ListItem> loadCategoryList(CortexWCFServiceClient wc)
        {
            Category[] categories = wc.getCategories();
            List<ListItem> catList = new List<ListItem>();
            foreach (Category c in categories)
            {
                ListItem cat = new ListItem()
                {
                    Text = c.Name,
                    Value = c.CategoryID
                };
                catList.Add(cat);
            }

            return catList;
        }

        /// <summary>
        /// retrieve list of Category Class
        /// </summary>
        /// <param name="wc"></param>
        /// <returns></returns>
        public static List<ListItem> loadClassList(CortexWCFServiceClient wc)
        {
            Category[] categories = wc.getCategoryClasses();
            List<ListItem> catList = new List<ListItem>();
            foreach (Category c in categories)
            {
                ListItem cat = new ListItem()
                {
                    Text = c.Class,
                    Value = c.CategoryID
                };
                catList.Add(cat);
            }

            return catList;
        }
    }
}
