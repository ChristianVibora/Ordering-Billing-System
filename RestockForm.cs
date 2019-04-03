using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Ordering_and_Billing_System
{
    public partial class restockFrm : Form
    {
        BindingManagerBase myBindingManager;
        int id;
        int stocksleft;
        int addstocks = 0;

        public restockFrm()
        {
            InitializeComponent();
        }

        private void restockFrm_Load(object sender, EventArgs e)
        {
            if (Tag == null)
            {
                LoadStocksInformation();
            }
            else
            {
                LoadStockInformation();
                previousBttn.Enabled = false;
                nextBttn.Enabled = false;
            }
            addstocksTxtbx.Text = "0";
            addstocksTxtbx.Focus();
            stocksleft = Convert.ToInt32(stocksleftLbl.Text);
            id = Convert.ToInt32(itemidLbl.Tag);
        }

        private void previousBttn_Click(object sender, EventArgs e)
        {
            if (myBindingManager.Position == 0)
            {
                MessageBox.Show("This Is The First Item", "Restock", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (addstocksTxtbx.Text == "")
                {
                    addstocksTxtbx.Text = "0";
                    addstocksTxtbx.Focus();
                }
                else if (Regex.IsMatch(addstocksTxtbx.Text, @"^\d+$") == false)
                {
                    MessageBox.Show("Invalid Input!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    addstocksTxtbx.Clear();
                    addstocksTxtbx.Focus();
                }
                else
                {
                    addstocks = Convert.ToInt32(addstocksTxtbx.Text);
                    if (addstocks < 0)
                    {
                        MessageBox.Show("Added Stocks Should Be Positive Number!", "Restock", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        addstocksTxtbx.Clear();
                        addstocksTxtbx.Focus();
                    }
                    else if (addstocks == 0)
                    {
                    }
                    else
                    {
                        stocksleft = Convert.ToInt32(stocksleftLbl.Text);
                        id = Convert.ToInt32(itemidLbl.Tag);
                        stocksleft += addstocks;
                        UpdateInventory(id, stocksleft);
                        UpdateInventoryLog(addstocks);
                        addstocksTxtbx.Text = "0";
                        addstocksTxtbx.Focus();
                    }
                }
                myBindingManager.Position -= 1;
            }
        }

        private void nextBttn_Click(object sender, EventArgs e)
        {
            if (myBindingManager.Position == myBindingManager.Count - 1)
            {
                MessageBox.Show("This Is The Last Item", "Restock", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (addstocksTxtbx.Text == "")
                {
                    addstocksTxtbx.Text = "0";
                    addstocksTxtbx.Focus();
                }
                else if (Regex.IsMatch(addstocksTxtbx.Text, @"^\d+$") == false)
                {
                    MessageBox.Show("Invalid Input!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    addstocksTxtbx.Clear();
                    addstocksTxtbx.Focus();
                }
                else
                {
                    addstocks = Convert.ToInt32(addstocksTxtbx.Text);
                    if (addstocks < 0)
                    {
                        MessageBox.Show("Added Stocks Should Be Positive Number!", "Restock", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        addstocksTxtbx.Clear();
                        addstocksTxtbx.Focus();
                    }
                    else if (addstocks == 0)
                    {
                    }
                    else
                    {
                        stocksleft = Convert.ToInt32(stocksleftLbl.Text);
                        id = Convert.ToInt32(itemidLbl.Tag);
                        stocksleft += addstocks;
                        UpdateInventory(id, stocksleft);
                        UpdateInventoryLog(addstocks);
                        addstocksTxtbx.Text = "0";
                        addstocksTxtbx.Focus();
                    }
                }
                myBindingManager.Position += 1;
            }
        }

        private void saveBttn_Click(object sender, EventArgs e)
        {
            if (addstocksTxtbx.Text == "")
            {
                addstocksTxtbx.Text = "0";
                addstocksTxtbx.Focus();
            }
            else if (Regex.IsMatch(addstocksTxtbx.Text, @"^\d+$") == false)
            {
                MessageBox.Show("Invalid Input!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                addstocksTxtbx.Clear();
                addstocksTxtbx.Focus();
            }
            else
            {
                addstocks = Convert.ToInt32(addstocksTxtbx.Text);
                if (addstocks < 0)
                {
                    MessageBox.Show("Added Stocks Should Be Positive Number!", "Restock", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    addstocksTxtbx.Clear();
                    addstocksTxtbx.Focus();
                }
                else if (addstocks == 0)
                {
                    MessageBox.Show("Item Restock Successful! Your Stocks Have Been Updated.", "Restock Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                else
                {
                    stocksleft = Convert.ToInt32(stocksleftLbl.Text);
                    id = Convert.ToInt32(itemidLbl.Tag);
                    stocksleft += addstocks;
                    UpdateInventory(id, stocksleft);
                    UpdateInventoryLog(addstocks);
                    MessageBox.Show("Item Restock Successful! Your Stocks Have Been Updated.", "Restock Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
            }
        }

        private void deleteitemBttn_Click(object sender, EventArgs e)
        {
            deleteitemBttn.Tag = itemidLbl.Tag;

            if (categoryLbl.Text == "Regular")
            {
                MessageBox.Show("You Cannot Delete Regular Items!", "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                DialogResult result;
                result = MessageBox.Show("Are You Sure You Want To Delete The Selected Item?", "Delete User?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    DeleteSeasonalItem();
                    UpdateInventoryLog(itemnameLbl.Text, stocksleftLbl.Text);
                }
                else { }
                Close();
            }
        }

        public void LoadStocksInformation()
        {
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbConnection myConnection;
            OleDbDataAdapter myDataAdapter;
            DataSet myDataSet;

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "SELECT [itemid], 'ITM-NUM-' & [itemid] as [Item_ID], [itemname], [category], [description], '₱' & [costprice] & '.00' as [Cost_Price], '₱' & [sellprice] & '.00' as [Sell_Price], [stocksleft] FROM [inventory]";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myConnection.Open();
                myDataAdapter = new OleDbDataAdapter(myCommand);
                myDataSet = new DataSet();
                myDataAdapter.Fill(myDataSet, "inventory");
                itemidLbl.DataBindings.Add("Tag", myDataSet, "inventory.itemid");
                itemidLbl.DataBindings.Add("Text", myDataSet, "inventory.Item_ID");
                itemnameLbl.DataBindings.Add("Text", myDataSet, "inventory.itemname");
                categoryLbl.DataBindings.Add("Text", myDataSet, "inventory.category");
                descriptionLbl.DataBindings.Add("Text", myDataSet, "inventory.description");
                costpriceLbl.DataBindings.Add("Text", myDataSet, "inventory.Cost_Price");
                sellpriceLbl.DataBindings.Add("Text", myDataSet, "inventory.Sell_Price");
                stocksleftLbl.DataBindings.Add("Text", myDataSet, "inventory.stocksleft");
                myBindingManager = BindingContext[myDataSet, "inventory"];
                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadStockInformation()
        {
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbConnection myConnection;
            OleDbDataReader myDataReader;

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "SELECT * FROM [inventory] WHERE [itemid] = @itemid";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@itemid", (Tag.ToString())));
                myConnection.Open();
                myDataReader = myCommand.ExecuteReader();

                string itemid = "";
                string itemname = "";
                string category = "";
                string description = "";
                string costprice = "";
                string sellprice = "";
                string stocksleft = "";

                while (myDataReader.Read())
                {
                    itemid = myDataReader["itemid"].ToString();
                    itemname = myDataReader["itemname"].ToString();
                    category = myDataReader["category"].ToString();
                    description = myDataReader["description"].ToString();
                    costprice = myDataReader["costprice"].ToString();
                    sellprice = myDataReader["sellprice"].ToString();
                    stocksleft = myDataReader["stocksleft"].ToString();
                }

                myConnection.Close();

                itemidLbl.Tag = itemid;
                itemidLbl.Text = "ITM-NUM-" + itemid;
                categoryLbl.Text = category;
                itemnameLbl.Text = itemname;
                descriptionLbl.Text = description;
                costpriceLbl.Text = "₱" + costprice + ".00";
                sellpriceLbl.Text = "₱" + sellprice + ".00";
                stocksleftLbl.Text = stocksleft;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void UpdateInventory(int id, int stocksleft)
        {
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbConnection myConnection;
            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "UPDATE [inventory] SET [stocksleft] = @stocksleft WHERE [itemid] = @itemid";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@stocksleft", (stocksleft)));
                myCommand.Parameters.Add(new OleDbParameter("@itemid", (id)));
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                myCommand.Dispose();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void UpdateInventoryLog(int addstocks)
        {
            DateTime logdate = DateTime.Now;

            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbConnection myConnection;
            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "INSERT INTO [inventorylog] ([logtype], [itemname], [addedquantity], [logdate]) VALUES (@logtype, @itemname, @addedquantity, @logdate)";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@logtype", ("Added")));
                myCommand.Parameters.Add(new OleDbParameter("@itemname", (itemnameLbl.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@addedquantity", (addstocks)));
                myCommand.Parameters.Add(new OleDbParameter("@logdate", (String.Format("{0:G}", logdate))));
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                myCommand.Dispose();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void DeleteSeasonalItem()
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
                myCommand.Parameters.Add(new OleDbParameter("@itemid", (deleteitemBttn.Tag.ToString())));
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

        public void UpdateInventoryLog(string itemname, string deletedquantity)
        {
            DateTime logdate = DateTime.Now;

            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbConnection myConnection;
            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "INSERT INTO [inventorylog] ([logtype], [itemname], [deletedquantity], [logdate]) VALUES (@logtype, @itemname, @soldquantity, @logdate)";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@logtype", ("Deleted")));
                myCommand.Parameters.Add(new OleDbParameter("@itemname", (itemname)));
                myCommand.Parameters.Add(new OleDbParameter("@soldquantity", (deletedquantity)));
                myCommand.Parameters.Add(new OleDbParameter("@logdate", (String.Format("{0:G}", logdate))));
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                myCommand.Dispose();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

