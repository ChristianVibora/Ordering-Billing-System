using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Windows.Forms;

namespace Ordering_and_Billing_System
{
    public partial class addedititemFrm : Form
    {
        public addedititemFrm()
        {
            InitializeComponent();
        }

        private void addedititemFrm_Load(object sender, EventArgs e)
        {
            categoryCmbbx.DropDownStyle = ComboBoxStyle.DropDownList;
            if (Text == "Add Item") { }
            else if (Text == "Edit Item")
            {
                LoadItemInfo();
            }
        }

        private void submitupdateBttn_Click(object sender, EventArgs e)
        {
            if (itemnameTxtbx.Text == "" || categoryCmbbx.Text == "" || descriptionTxtbx.Text == "" || sellpriceTxtbx.Text == "" || costpriceTxtbx.Text == "" || stocksTxtbx.Text == "")
            {
                MessageBox.Show("Enter All The Required Information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (Regex.IsMatch(costpriceTxtbx.Text, @"^\d+$") == false)
            {
                MessageBox.Show("Invalid Cost Price!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                costpriceTxtbx.Clear();
                costpriceTxtbx.Focus();
            }
            else if (Regex.IsMatch(sellpriceTxtbx.Text, @"^\d+$") == false)
            {
                MessageBox.Show("Invalid Sell Price!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                sellpriceTxtbx.Clear();
                sellpriceTxtbx.Focus();
            }
            else if (Regex.IsMatch(stocksTxtbx.Text, @"^\d+$") == false)
            {
                MessageBox.Show("Invalid Initial Stocks!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                stocksTxtbx.Clear();
                stocksTxtbx.Focus();
            }
            else
            {
                bool validateitemname = ValidateItemName(itemnameTxtbx.Text);
                if (Text == "Add Item")
                {
                    if (validateitemname == true)
                    {
                        MessageBox.Show("That Item Name Is Already Registered. Please Try A New One", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        itemnameTxtbx.Clear();
                        itemnameTxtbx.Focus();
                    }
                    else
                    {
                        AddItem();
                        UpdateInventoryLog();
                        Close();
                    }
                }
                else if (Text == "Edit Item")
                {
                    if (itemnameTxtbx.Modified == true)
                    {
                        if (validateitemname == true)
                        {
                            MessageBox.Show("That Item Name Is Already Registered. Please Try A New One", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            itemnameTxtbx.Clear();
                            itemnameTxtbx.Focus();
                        }
                        else
                        {
                            EditItem();
                            Close();
                        }
                    }
                    else
                    {
                        EditItem();
                        Close();
                    }
                }
            }
        }

        private void clearBttn_Click(object sender, EventArgs e)
        {
            itemnameTxtbx.Clear();
            categoryCmbbx.SelectedIndex = -1;
            descriptionTxtbx.Clear();
            costpriceTxtbx.Clear();
            sellpriceTxtbx.Clear();
            stocksTxtbx.Clear();
        }

        private void cancelBttn_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void LoadItemInfo() {
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbDataReader myDataReader;
            OleDbConnection myConnection;

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "SELECT * FROM [inventory] WHERE [itemid] = @itemid";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myConnection.Open();

                myCommand.Parameters.Add(new OleDbParameter("@itemid", (Tag.ToString())));

                myDataReader = myCommand.ExecuteReader();

                string itemname = "";
                string category = "";
                string description = "";
                string costprice = "";
                string sellprice = "";
                string stocks = "";

                while (myDataReader.Read())
                {
                    itemname = myDataReader["itemname"].ToString();
                    category = myDataReader["category"].ToString();
                    description = myDataReader["description"].ToString();
                    costprice = myDataReader["costprice"].ToString();
                    sellprice = myDataReader["sellprice"].ToString();
                    stocks = myDataReader["stocksleft"].ToString();
                }

                myConnection.Close();

                itemnameTxtbx.Text = itemname;
                categoryCmbbx.Text = category;
                descriptionTxtbx.Text = description;
                costpriceTxtbx.Text = costprice;
                sellpriceTxtbx.Text = sellprice;
                stocksTxtbx.Text = stocks;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void AddItem()
        {
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbConnection myConnection;

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "INSERT INTO [inventory] ([itemname], [category], [description], [costprice], [sellprice], [stocksleft]) VALUES (@itemname, @category, @description, @costprice, @sellprice, @initialstocks)";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@itemname", (itemnameTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@category", (categoryCmbbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@description", (descriptionTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@costprice", (costpriceTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@sellprice", (sellpriceTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@initialstocks", (stocksTxtbx.Text)));
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                myCommand.Dispose();
                myConnection.Close();

                costpriceTxtbx.Text = "₱" + costpriceTxtbx.Text + ".00";
                sellpriceTxtbx.Text = "₱" + sellpriceTxtbx.Text + ".00";
                
                MessageBox.Show("Registration Successful! Customers Can Now Order The New Item.", "Registration Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void EditItem() {
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbConnection myConnection;

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "UPDATE [inventory] SET [itemname] = @itemname, [category] = @category, [description] = @description, [costprice] = @costprice, [sellprice] = @sellprice, [stocksleft] = @stocksleft WHERE [itemid] = @itemid";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@itemname", (itemnameTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@category", (categoryCmbbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@description", (descriptionTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@costprice", (costpriceTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@sellprice", (sellpriceTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@stocksleft", (stocksTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@itemid", (Tag.ToString())));
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                myCommand.Dispose();
                myConnection.Close();

                costpriceTxtbx.Text = "₱" + costpriceTxtbx.Text + ".00";
                sellpriceTxtbx.Text = "₱" + sellpriceTxtbx.Text + ".00";

                MessageBox.Show("User Edit Successful! Your Informations Have Been Updated.", "Edit Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void UpdateInventoryLog()
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
                myCommand.Parameters.Add(new OleDbParameter("@itemname", (itemnameTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@addedquantity", (stocksTxtbx.Text)));
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

        public bool ValidateItemName(string vitemname)
        {
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbDataReader myDataReader;
            OleDbConnection myConnection;

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "SELECT * FROM [inventory] WHERE [itemname] = @vitemname";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@vitemname", (vitemname)));
                myConnection.Open();
                myDataReader = myCommand.ExecuteReader();

                bool itemFound = false;

                while (myDataReader.Read())
                {
                    itemFound = true;
                }

                if (itemFound == true)
                {
                    return true;
                }

                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }
    }
}
