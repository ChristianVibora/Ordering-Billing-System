using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Ordering_and_Billing_System
{
    public partial class inventoryFrm : Form
    {
        restockFrm restockform;
        inventorylogFrm inventorylogform;
        addedititemFrm addedititemform;

        public inventoryFrm()
        {
            InitializeComponent();
        }

        private void inventoryFrm_Load(object sender, EventArgs e)
        {
            LoadIventory();
        }

        private void additemBttn_Click(object sender, EventArgs e)
        {
            if (addedititemform == null)
            {
                addedititemform = new addedititemFrm();
                addedititemform.Text = "Add Item";
                addedititemform.submitupdateBttn.Text = "Submit";
                addedititemform.FormClosed += addedititemform_FormClosed;
            }
            addedititemform.Show(this);
            Hide();
        }

        private void edititemBttn_Click(object sender, EventArgs e)
        {
            if (addedititemform == null)
            {
                addedititemform = new addedititemFrm();
                addedititemform.Tag = inventoryDtgrdvw[0, inventoryDtgrdvw.CurrentRow.Index].Value;
                addedititemform.Text = "Edit Item";
                addedititemform.submitupdateBttn.Text = "Update";
                addedititemform.FormClosed += addedititemform_FormClosed;
            }
            addedititemform.Show(this);
            Hide();
        }

        private void deleteitemBttn_Click(object sender, EventArgs e)
        {
            Tag = inventoryDtgrdvw[0, inventoryDtgrdvw.CurrentRow.Index].Value.ToString();
            DialogResult result;
            result = MessageBox.Show("Are You Sure You Want To Delete The Selected Item?", "Delete Item?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                DeleteItem();
                LoadIventory();
            }
            else { }
        }

        private void inventoryDtgrdvw_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (restockform == null)
            {
                restockform = new restockFrm();
                restockform.FormClosed += restockform_FormClosed;
                restockform.Tag = inventoryDtgrdvw[0, inventoryDtgrdvw.CurrentRow.Index].Value;
            }
            restockform.Show(this);
            Hide();
        }

        private void restockBttn_Click(object sender, EventArgs e)
        {
            if (restockform == null)
            {
                restockform = new restockFrm();
                restockform.FormClosed += restockform_FormClosed;
            }
            restockform.Show(this);
            Hide();
        }

        private void inventorylogBttn_Click(object sender, EventArgs e)
        {
            if (inventorylogform == null)
            {
                inventorylogform = new inventorylogFrm();
                inventorylogform.FormClosed += inventorylogform_FormClosed;
            }
            inventorylogform.Show(this);
            Hide();
        }

        void inventorylogform_FormClosed(object sender, FormClosedEventArgs e)
        {
            inventorylogform = null;
            Show();
        }

        void restockform_FormClosed(object sender, FormClosedEventArgs e)
        {
            restockform = null;
            LoadIventory();
            Show();
        }

        void addedititemform_FormClosed(object sender, FormClosedEventArgs e)
        {
            addedititemform = null;
            LoadIventory();
            Show();
        }
 
        public void LoadIventory()
        {
            inventoryDtgrdvw.Update();
            inventoryDtgrdvw.Refresh();
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbConnection myConnection;
            OleDbDataAdapter myDataAdapter;
            DataTable myDataTable;
   
            try
            {

                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "SELECT [itemid], 'ITM-NUM-' & [itemid] as [Item ID], [itemname] as [Item Name], [category] as [Category], '₱' & [costprice] & '.00' as [Cost Price], '₱' & [sellprice] & '.00' as [Sell Price], [stocksleft] as [Stocks Left] FROM [inventory]";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myConnection.Open();
                myDataAdapter = new OleDbDataAdapter(myCommand);
                myDataAdapter.Fill(myDataTable = new DataTable());
                inventoryDtgrdvw.DataSource = myDataTable;
                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void DeleteItem()
        {
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbConnection myConnection;

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "DELETE FROM [inventory] WHERE [itemid] = @itemid";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@itemid", (Tag.ToString())));
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                myConnection.Close();

                MessageBox.Show("Item Delete Successful!", "Delete Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
