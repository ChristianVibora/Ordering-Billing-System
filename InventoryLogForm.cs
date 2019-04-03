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
    public partial class inventorylogFrm : Form
    {
        string logtype = "";
        public inventorylogFrm()
        {
            InitializeComponent();
        }

        private void InventoryLogForm_Load(object sender, EventArgs e)
        {
            viewCmbbx.DropDownStyle = ComboBoxStyle.DropDownList;
            viewCmbbx.SelectedIndex = 0;
            logtypeCmbbx.DropDownStyle = ComboBoxStyle.DropDownList;
            logtypeCmbbx.SelectedIndex = 0;
            LoadInventoryLog();
        }

        private void viewDttmpckr_ValueChanged(object sender, EventArgs e)
        {
            backDttmpckr.Value = viewDttmpckr.Value;
        }

        private void viewCmbbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (viewCmbbx.Text == "All")
            {
                viewDttmpckr.Format = DateTimePickerFormat.Short;
            }
            else if (viewCmbbx.Text == "Daily")
            {
                viewDttmpckr.Format = DateTimePickerFormat.Short;
            }
            else if (viewCmbbx.Text == "Weekly")
            {
                viewDttmpckr.Format = DateTimePickerFormat.Short;
            }
            else if (viewCmbbx.Text == "Monthly")
            {
                viewDttmpckr.Format = DateTimePickerFormat.Custom;
                viewDttmpckr.CustomFormat = "MMMM/yyyy";
            }
            else if (viewCmbbx.Text == "Yearly")
            {
                viewDttmpckr.Format = DateTimePickerFormat.Custom;
                viewDttmpckr.CustomFormat = "yyyy";
            }
        }

        private void searchBttn_Click(object sender, EventArgs e)
        {
            if (logtypeCmbbx.Text == "Added")
            {
                logtype = "Added";
            }
            else if (logtypeCmbbx.Text == "Sold")
            {
                logtype = "Sold";
            }
            else if (logtypeCmbbx.Text == "Deleted")
            {
                logtype = "Deleted";
            }
            LoadInventoryLog();
        }

        public void LoadInventoryLog()
        {
            string date = backDttmpckr.Text;
            string MyConnectionString;
            string mySQLQuery = "";
            OleDbCommand myCommand = new OleDbCommand();
            OleDbConnection myConnection;
            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter();
            DataTable myDataTable = new DataTable();

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);

                if (viewCmbbx.Text == "All")
                {
                    if (logtypeCmbbx.Text == "All")
                    {
                        mySQLQuery = "SELECT 'INVNTRY-LG-NUM-' & [inventorylogid] as [Inventory Log ID], [logtype] as [Log Type], [itemname] as [Item Name], [addedquantity] as [Added Quantity], [soldquantity] as [Sold Quantity], [deletedquantity] as [Deleted Quantity], FormatDateTime([logdate],0) as [Log Date] FROM [inventorylog] ORDER BY [inventorylogid]";
                        myCommand = new OleDbCommand(mySQLQuery, myConnection);
                    }
                    else
                    {
                        mySQLQuery = "SELECT 'INVNTRY-LG-NUM-' & [inventorylogid] as [Inventory Log ID], [logtype] as [Log Type], [itemname] as [Item Name], [addedquantity] as [Added Quantity], [soldquantity] as [Sold Quantity], [deletedquantity] as [Deleted Quantity], FormatDateTime([logdate],0) as [Log Date] FROM [inventorylog] WHERE [logtype] = @logtype ORDER BY [inventorylogid]";
                        myCommand = new OleDbCommand(mySQLQuery, myConnection);
                        myCommand.Parameters.Add(new OleDbParameter("@logtype", (logtype)));
                        myCommand.Parameters.Add(new OleDbParameter("@date", (date)));
                    }
                }
                else if (viewCmbbx.Text == "Daily")
                {
                    if (logtypeCmbbx.Text == "All")
                    {
                        mySQLQuery = "SELECT 'INVNTRY-LG-NUM-' & [inventorylogid] as [Inventory Log ID], [logtype] as [Log Type], [itemname] as [Item Name], [addedquantity] as [Added Quantity], [soldquantity] as [Sold Quantity], [deletedquantity] as [Deleted Quantity], FormatDateTime([logdate],0) as [Log Date] FROM [inventorylog] WHERE (DATEDIFF(\"d\", [logdate], @date) = 0) AND (DATEDIFF(\"m\", [logdate], @date) = 0) AND (DATEDIFF(\"yyyy\", [logdate], @date) = 0) ORDER BY [inventorylogid]";
                        myCommand = new OleDbCommand(mySQLQuery, myConnection);
                        myCommand.Parameters.Add(new OleDbParameter("@date", (date)));
                    }
                    else
                    {
                        mySQLQuery = "SELECT 'INVNTRY-LG-NUM-' & [inventorylogid] as [Inventory Log ID], [logtype] as [Log Type], [itemname] as [Item Name], [addedquantity] as [Added Quantity], [soldquantity] as [Sold Quantity], [deletedquantity] as [Deleted Quantity], FormatDateTime([logdate],0) as [Log Date] FROM [inventorylog] WHERE ([logtype] = @logtype) AND (DATEDIFF(\"d\", [logdate], @date) = 0) AND (DATEDIFF(\"m\", [logdate], @date) = 0) AND (DATEDIFF(\"yyyy\", [logdate], @date) = 0) ORDER BY [inventorylogid]";
                        myCommand = new OleDbCommand(mySQLQuery, myConnection);
                        myCommand.Parameters.Add(new OleDbParameter("@logtype", (logtype)));
                        myCommand.Parameters.Add(new OleDbParameter("@date", (date)));
                    }
                }
                else if (viewCmbbx.Text == "Weekly")
                {
                    if (logtypeCmbbx.Text == "All")
                    {
                        mySQLQuery = "SELECT 'INVNTRY-LG-NUM-' & [inventorylogid] as [Inventory Log ID], [logtype] as [Log Type], [itemname] as [Item Name], [addedquantity] as [Added Quantity], [soldquantity] as [Sold Quantity], [deletedquantity] as [Deleted Quantity], FormatDateTime([logdate],0) as [Log Date] FROM [inventorylog] WHERE (DATEDIFF(\"ww\", [logdate], @date, 2, 1) = 0) AND (DATEDIFF(\"yyyy\", [logdate], @date) = 0) ORDER BY [inventorylogid]";
                        myCommand = new OleDbCommand(mySQLQuery, myConnection);
                        myCommand.Parameters.Add(new OleDbParameter("@date", (date)));
                    }
                    else
                    {
                        mySQLQuery = "SELECT 'INVNTRY-LG-NUM-' & [inventorylogid] as [Inventory Log ID], [logtype] as [Log Type], [itemname] as [Item Name], [addedquantity] as [Added Quantity], [soldquantity] as [Sold Quantity], [deletedquantity] as [Deleted Quantity], FormatDateTime([logdate],0) as [Log Date] FROM [inventorylog] WHERE ([logtype] = @logtype) AND (DATEDIFF(\"ww\", [logdate], @date, 2, 1) = 0) AND (DATEDIFF(\"yyyy\", [logdate], @date) = 0) ORDER BY [inventorylogid]";
                        myCommand = new OleDbCommand(mySQLQuery, myConnection);
                        myCommand.Parameters.Add(new OleDbParameter("@logtype", (logtype)));
                        myCommand.Parameters.Add(new OleDbParameter("@date", (date)));
                    }
                }
                else if (viewCmbbx.Text == "Monthly")
                {
                    if (logtypeCmbbx.Text == "All")
                    {
                        mySQLQuery = "SELECT 'INVNTRY-LG-NUM-' & [inventorylogid] as [Inventory Log ID], [logtype] as [Log Type], [itemname] as [Item Name], [addedquantity] as [Added Quantity], [soldquantity] as [Sold Quantity], [deletedquantity] as [Deleted Quantity], FormatDateTime([logdate],0) as [Log Date] FROM [inventorylog] WHERE (DATEDIFF(\"m\", [logdate], @date) = 0) AND (DATEDIFF(\"yyyy\", [logdate], @date) = 0) ORDER BY [inventorylogid]";
                        myCommand = new OleDbCommand(mySQLQuery, myConnection);
                        myCommand.Parameters.Add(new OleDbParameter("@date", (date)));
                    }
                    else
                    {
                        mySQLQuery = "SELECT 'INVNTRY-LG-NUM-' & [inventorylogid] as [Inventory Log ID], [logtype] as [Log Type], [itemname] as [Item Name], [addedquantity] as [Added Quantity], [soldquantity] as [Sold Quantity], [deletedquantity] as [Deleted Quantity], FormatDateTime([logdate],0) as [Log Date] FROM [inventorylog] WHERE ([logtype] = @logtype) AND (DATEDIFF(\"m\", [logdate], @date) = 0) AND (DATEDIFF(\"yyyy\", [logdate], @date) = 0) ORDER BY [inventorylogid]";
                        myCommand = new OleDbCommand(mySQLQuery, myConnection);
                        myCommand.Parameters.Add(new OleDbParameter("@logtype", (logtype)));
                        myCommand.Parameters.Add(new OleDbParameter("@date", (date)));
                    }
                }
                else if (viewCmbbx.Text == "Yearly")
                {
                    if (logtypeCmbbx.Text == "All")
                    {
                        mySQLQuery = "SELECT 'INVNTRY-LG-NUM-' & [inventorylogid] as [Inventory Log ID], [logtype] as [Log Type], [itemname] as [Item Name], [addedquantity] as [Added Quantity], [soldquantity] as [Sold Quantity], [deletedquantity] as [Deleted Quantity], FormatDateTime([logdate],0) as [Log Date] FROM [inventorylog] WHERE (DATEDIFF(\"yyyy\", [logdate], @date) = 0) ORDER BY [inventorylogid]";
                        myCommand = new OleDbCommand(mySQLQuery, myConnection);
                        myCommand.Parameters.Add(new OleDbParameter("@date", (date)));
                    }
                    else
                    {
                        mySQLQuery = "SELECT 'INVNTRY-LG-NUM-' & [inventorylogid] as [Inventory Log ID], [logtype] as [Log Type], [itemname] as [Item Name], [addedquantity] as [Added Quantity], [soldquantity] as [Sold Quantity], [deletedquantity] as [Deleted Quantity], FormatDateTime([logdate],0) as [Log Date] FROM [inventorylog] WHERE ([logtype] = @logtype) AND (DATEDIFF(\"yyyy\", [logdate], @date) = 0) ORDER BY [inventorylogid]";
                        myCommand = new OleDbCommand(mySQLQuery, myConnection);
                        myCommand.Parameters.Add(new OleDbParameter("@logtype", (logtype)));
                        myCommand.Parameters.Add(new OleDbParameter("@date", (date)));
                    }
                }

               
                myConnection.Open();
                myDataAdapter.SelectCommand = myCommand;
                myDataAdapter.Fill(myDataTable);
                inventorylogDtgrdvw.DataSource = myDataTable;
                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
