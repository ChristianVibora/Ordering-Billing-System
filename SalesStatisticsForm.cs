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
    public partial class salesstatisticsForm : Form
    {
        int highestnumberofitemsold = 0;
        string bestsellingitem = "";
        int lowestnumberofitemsold = 0;
        string leastsellingitem = "";
        int totalnumberofitemssold = 0;
        int highestsaleofanitem = 0;
        string highestsellingitem = "";
        int lowestsaleofanitem = 0;
        string lowestselligingitem = "";
        int totalsalesofitems = 0;
        int totalprofit = 0;

        public salesstatisticsForm()
        {
            InitializeComponent();
        }

        private void salesstatisticsForm_Load(object sender, EventArgs e)
        {
            viewCmbbx.DropDownStyle = ComboBoxStyle.DropDownList;
            viewCmbbx.SelectedIndex = 0;
            DisplayItemNames();
            DisplayNumberOfItemSold();
            DisplayItemSales();
            DisplayItemProfits();
            DisplaySalesData();
            DisplaySaleStatistics();
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

        private void viewDttmpckr_ValueChanged(object sender, EventArgs e)
        {
            backDttmpckr.Value = viewDttmpckr.Value;
        }

        private void searchBttn_Click(object sender, EventArgs e)
        {
            totalnumberofitemssold = 0;
            totalsalesofitems = 0;
            totalprofit = 0;
            bestsellingitem = "";
            leastsellingitem = "";
            highestsellingitem = "";
            lowestselligingitem = "";
            DisplayItemNames();
            DisplayNumberOfItemSold();
            DisplayItemSales();
            DisplayItemProfits();
            DisplaySalesData();
            DisplaySaleStatistics();
        }

        private void exportBttn_Click(object sender, EventArgs e)
        {
            DateTime statsdate = DateTime.Now;
            string statsfilenname = String.Format("STATS-{0:MMddyyyyhhmmss}", statsdate);

            printForm1.PrintFileName = "Statistics/" + statsfilenname + ".eps";
            printForm1.PrinterSettings.DefaultPageSettings.Landscape = true;
            printForm1.Print();
        }

        public void DisplayItemNames()
        {
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
                mySQLQuery = "SELECT [itemname] as [Item Name] FROM [inventory]";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myConnection.Open();
                myDataAdapter = new OleDbDataAdapter(myCommand);
                myDataAdapter.Fill(myDataTable = new DataTable());
                salesstatisticsitemnamesDtgrdvw.DataSource = myDataTable;
                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public int GetTotalNumberOfItemSold(string itemname)
        {
            string date = backDttmpckr.Text;
            int itemsold = 0;

            string MyConnectionString;
            string mySQLQuery = "";
            OleDbCommand myCommand;
            OleDbDataReader myDataReader;
            OleDbConnection myConnection;

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);

                if (viewCmbbx.Text == "All")
                {
                    mySQLQuery = "SELECT [soldquantity] FROM [inventorylog] WHERE [itemname] = @itemname AND [logtype] = 'Sold' ORDER BY [logdate]";
                }
                else if (viewCmbbx.Text == "Daily")
                {
                    mySQLQuery = "SELECT [soldquantity] FROM [inventorylog] WHERE [itemname] = @itemname AND (DATEDIFF(\"d\", [logdate], @date) = 0) AND (DATEDIFF(\"m\", [logdate], @date) = 0) AND (DATEDIFF(\"yyyy\", [logdate], @date) = 0) AND [logtype] = 'Sold' ORDER BY [logdate]";
                }
                else if (viewCmbbx.Text == "Weekly")
                {
                    mySQLQuery = "SELECT [soldquantity] FROM [inventorylog] WHERE [itemname] = @itemname AND (DATEDIFF(\"ww\", [logdate], @date, 2, 1) = 0) AND (DATEDIFF(\"yyyy\", [logdate], @date) = 0) AND [logtype] = 'Sold' ORDER BY [logdate]";
                }
                else if (viewCmbbx.Text == "Monthly")
                {
                    mySQLQuery = "SELECT [soldquantity] FROM [inventorylog] WHERE [itemname] = @itemname AND (DATEDIFF(\"m\", [logdate], @date) = 0) AND (DATEDIFF(\"yyyy\", [logdate], @date) = 0) AND [logtype] = 'Sold' ORDER BY [logdate]";
                }
                else if (viewCmbbx.Text == "Yearly")
                {
                    mySQLQuery = "SELECT [soldquantity] FROM [inventorylog] WHERE [itemname] = @itemname AND (DATEDIFF(\"yyyy\", [logdate], @date) = 0) AND [logtype] = 'Sold' ORDER BY [logdate]";
                }

                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@itemname", (itemname)));
                myCommand.Parameters.Add(new OleDbParameter("@date", (date)));
                myConnection.Open();
                myDataReader = myCommand.ExecuteReader();

                while (myDataReader.Read())
                {
                    itemsold += Convert.ToInt32(myDataReader["soldquantity"].ToString());
                }

                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return itemsold;
        }

        public void DisplayNumberOfItemSold()
        {
            DataTable myDataTable;
            DataColumn myDataColumn;
            DataRow myDataRow;
            DataView myDataView;

            myDataTable = new DataTable();

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "Number Of Item Sold";
            myDataTable.Columns.Add(myDataColumn);

            for (int i = 0; i < salesstatisticsitemnamesDtgrdvw.Rows.Count; i++)
            {
                myDataRow = myDataTable.NewRow();
                myDataRow["Number Of Item Sold"] = GetTotalNumberOfItemSold(salesstatisticsitemnamesDtgrdvw.Rows[i].Cells[0].Value.ToString());
                myDataTable.Rows.Add(myDataRow);
            }

            myDataView = new DataView(myDataTable);
            salesstatisticsnumberofitemsoldDtgrdvw.DataSource = myDataView;
        }

        public int GetItemSellPrice(string itemname)
        {
            string date = backDttmpckr.Text;
            int itemsellprice = 0;

            string MyConnectionString;
            string mySQLQuery = "";
            OleDbCommand myCommand;
            OleDbDataReader myDataReader;
            OleDbConnection myConnection;

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "SELECT [sellprice] FROM [inventory] WHERE [itemname] = @itemname";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@itemname", (itemname)));
                myConnection.Open();
                myDataReader = myCommand.ExecuteReader();

                while (myDataReader.Read())
                {
                    itemsellprice = Convert.ToInt32(myDataReader["sellprice"].ToString());
                }

                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return itemsellprice;
        }

        public void DisplayItemSales()
        {
            DataTable myDataTable;
            DataColumn myDataColumn;
            DataRow myDataRow;
            DataView myDataView;

            myDataTable = new DataTable();

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Item Sales";
            myDataTable.Columns.Add(myDataColumn);

            for (int i = 0; i < salesstatisticsitemnamesDtgrdvw.Rows.Count; i++)
            {
                myDataRow = myDataTable.NewRow();
                myDataRow["Item Sales"] = "₱" + (GetItemSellPrice(salesstatisticsitemnamesDtgrdvw.Rows[i].Cells[0].Value.ToString()) * Convert.ToInt32(salesstatisticsnumberofitemsoldDtgrdvw.Rows[i].Cells[0].Value)).ToString() + ".00";
                myDataTable.Rows.Add(myDataRow);
            }

            myDataView = new DataView(myDataTable);
            salesstatisticsitemsalesDtgrdvw.DataSource = myDataView;
        }

        public int GetItemMarkUpPrice(string itemname)
        {
            string date = backDttmpckr.Text;
            int itemcostprice = 0;
            int itemsellprice = 0;
            int itemmarkupprice = 0;
            string MyConnectionString;
            string mySQLQuery = "";
            OleDbCommand myCommand;
            OleDbDataReader myDataReader;
            OleDbConnection myConnection;

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "SELECT [costprice], [sellprice] FROM [inventory] WHERE [itemname] = @itemname";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@itemname", (itemname)));
                myConnection.Open();
                myDataReader = myCommand.ExecuteReader();

                while (myDataReader.Read())
                {
                    itemcostprice = Convert.ToInt32(myDataReader["costprice"].ToString());
                    itemsellprice = Convert.ToInt32(myDataReader["sellprice"].ToString());
                }
                itemmarkupprice = itemsellprice - itemcostprice;
                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return itemmarkupprice;
        }

        public void DisplayItemProfits()
        {
            DataTable myDataTable;
            DataColumn myDataColumn;
            DataRow myDataRow;
            DataView myDataView;

            myDataTable = new DataTable();

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Item Profit";
            myDataTable.Columns.Add(myDataColumn);

            for (int i = 0; i < salesstatisticsitemnamesDtgrdvw.Rows.Count; i++)
            {
                myDataRow = myDataTable.NewRow();
                myDataRow["Item Profit"] = "₱" + (GetItemMarkUpPrice(salesstatisticsitemnamesDtgrdvw.Rows[i].Cells[0].Value.ToString()) * Convert.ToInt32(salesstatisticsnumberofitemsoldDtgrdvw.Rows[i].Cells[0].Value)).ToString() + ".00";
                myDataTable.Rows.Add(myDataRow);
            }

            myDataView = new DataView(myDataTable);
            salesstatisticsitemprofitsDtgrdvw.DataSource = myDataView;
        }

        public void GetStatistics()
        {
            highestnumberofitemsold = Convert.ToInt32(salesstatisticsnumberofitemsoldDtgrdvw.Rows[0].Cells[0].Value);
            lowestnumberofitemsold = Convert.ToInt32(salesstatisticsnumberofitemsoldDtgrdvw.Rows[0].Cells[0].Value);
            highestsaleofanitem = (GetItemSellPrice(salesstatisticsitemnamesDtgrdvw.Rows[0].Cells[0].Value.ToString()) * Convert.ToInt32(salesstatisticsnumberofitemsoldDtgrdvw.Rows[0].Cells[0].Value));
            lowestsaleofanitem = (GetItemSellPrice(salesstatisticsitemnamesDtgrdvw.Rows[0].Cells[0].Value.ToString()) * Convert.ToInt32(salesstatisticsnumberofitemsoldDtgrdvw.Rows[0].Cells[0].Value));

            for (int i = 0; i < salesstatisticsitemnamesDtgrdvw.Rows.Count; i++)
            {
                int salesvalue = (GetItemSellPrice(salesstatisticsitemnamesDtgrdvw.Rows[i].Cells[0].Value.ToString()) * Convert.ToInt32(salesstatisticsnumberofitemsoldDtgrdvw.Rows[i].Cells[0].Value));
                int profits = (GetItemMarkUpPrice(salesstatisticsitemnamesDtgrdvw.Rows[i].Cells[0].Value.ToString()) * Convert.ToInt32(salesstatisticsnumberofitemsoldDtgrdvw.Rows[i].Cells[0].Value));
                
                if (highestnumberofitemsold == Convert.ToInt32(salesstatisticsnumberofitemsoldDtgrdvw.Rows[i].Cells[0].Value))
                {
                    highestnumberofitemsold = Convert.ToInt32(salesstatisticsnumberofitemsoldDtgrdvw.Rows[i].Cells[0].Value);
                    bestsellingitem += salesstatisticsitemnamesDtgrdvw.Rows[i].Cells[0].Value.ToString() + Environment.NewLine;
                }
                else if (highestnumberofitemsold < Convert.ToInt32(salesstatisticsnumberofitemsoldDtgrdvw.Rows[i].Cells[0].Value))
                {
                    highestnumberofitemsold = Convert.ToInt32(salesstatisticsnumberofitemsoldDtgrdvw.Rows[i].Cells[0].Value);
                    bestsellingitem = salesstatisticsitemnamesDtgrdvw.Rows[i].Cells[0].Value.ToString() + Environment.NewLine;
                }

                if (lowestnumberofitemsold == Convert.ToInt32(salesstatisticsnumberofitemsoldDtgrdvw.Rows[i].Cells[0].Value))
                {
                    lowestnumberofitemsold = Convert.ToInt32(salesstatisticsnumberofitemsoldDtgrdvw.Rows[i].Cells[0].Value);
                    leastsellingitem += salesstatisticsitemnamesDtgrdvw.Rows[i].Cells[0].Value.ToString() + Environment.NewLine;
                }
                else if (lowestnumberofitemsold > Convert.ToInt32(salesstatisticsnumberofitemsoldDtgrdvw.Rows[i].Cells[0].Value))
                {
                    lowestnumberofitemsold = Convert.ToInt32(salesstatisticsnumberofitemsoldDtgrdvw.Rows[i].Cells[0].Value);
                    leastsellingitem = salesstatisticsitemnamesDtgrdvw.Rows[i].Cells[0].Value.ToString() + Environment.NewLine;
                }

                totalnumberofitemssold += Convert.ToInt32(salesstatisticsnumberofitemsoldDtgrdvw.Rows[i].Cells[0].Value);

                if (highestsaleofanitem == salesvalue)
                {
                    highestsaleofanitem = salesvalue;
                    highestsellingitem += salesstatisticsitemnamesDtgrdvw.Rows[i].Cells[0].Value.ToString() + Environment.NewLine;
                }
                else if (highestsaleofanitem < salesvalue)
                {
                    highestsaleofanitem = salesvalue;
                    highestsellingitem = salesstatisticsitemnamesDtgrdvw.Rows[i].Cells[0].Value.ToString() + Environment.NewLine;
                }

                if (lowestsaleofanitem == salesvalue)
                {
                    lowestsaleofanitem = salesvalue;
                    lowestselligingitem += salesstatisticsitemnamesDtgrdvw.Rows[i].Cells[0].Value.ToString() + Environment.NewLine;
                }
                else if (lowestsaleofanitem > salesvalue)
                {
                    lowestsaleofanitem = salesvalue;
                    lowestselligingitem = salesstatisticsitemnamesDtgrdvw.Rows[i].Cells[0].Value.ToString() + Environment.NewLine;
                }

                totalsalesofitems += salesvalue;
                totalprofit += profits;
            }
        }

        public void DisplaySaleStatistics()
        {
            GetStatistics();

            DataTable myDataTable;
            DataColumn myDataColumn;
            DataRow myDataRow;
            DataView myDataView;

            myDataTable = new DataTable();

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Category";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Item Name";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Statistics";
            myDataTable.Columns.Add(myDataColumn);

            myDataRow = myDataTable.NewRow();
            myDataRow["Category"] = "Best Selling Item";
            myDataRow["Item Name"] = Environment.NewLine + bestsellingitem;
            myDataRow["Statistics"] = highestnumberofitemsold;
            myDataTable.Rows.Add(myDataRow);

            myDataRow = myDataTable.NewRow();
            myDataRow["Category"] = "Least Selling Item";
            myDataRow["Item Name"] = Environment.NewLine + leastsellingitem;
            myDataRow["Statistics"] = lowestnumberofitemsold;
            myDataTable.Rows.Add(myDataRow);

            myDataRow = myDataTable.NewRow();
            myDataRow["Category"] = "Total Number Of Items Sold";
            myDataRow["Item Name"] = "";
            myDataRow["Statistics"] = totalnumberofitemssold;
            myDataTable.Rows.Add(myDataRow);

            myDataRow = myDataTable.NewRow();
            myDataRow["Category"] = "Highest Selling Item";
            myDataRow["Item Name"] = Environment.NewLine + highestsellingitem;
            myDataRow["Statistics"] = "₱" + highestsaleofanitem.ToString() + ".00";
            myDataTable.Rows.Add(myDataRow);

            myDataRow = myDataTable.NewRow();
            myDataRow["Category"] = "Lowest Selling Item";
            myDataRow["Item Name"] = Environment.NewLine + lowestselligingitem;
            myDataRow["Statistics"] = "₱" + lowestsaleofanitem.ToString() + ".00";
            myDataTable.Rows.Add(myDataRow);

            myDataRow = myDataTable.NewRow();
            myDataRow["Category"] = "Total Sales";
            myDataRow["Item Name"] = "";
            myDataRow["Statistics"] = "₱" + totalsalesofitems.ToString() + ".00";
            myDataTable.Rows.Add(myDataRow);

            myDataRow = myDataTable.NewRow();
            myDataRow["Category"] = "Total Profit";
            myDataRow["Item Name"] = "";
            myDataRow["Statistics"] = "₱" + totalprofit.ToString() + ".00";
            myDataTable.Rows.Add(myDataRow);

            myDataView = new DataView(myDataTable);
            salesstatisticsDtgrdvw.DataSource = myDataView;
        }

        public void DisplaySalesData()
        {
            DataTable myDataTable;
            DataColumn myDataColumn;
            DataRow myDataRow;
            DataView myDataView;

            myDataTable = new DataTable();

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Item Name";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Number Of Item Sold";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Item Sales";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Item Profit";
            myDataTable.Columns.Add(myDataColumn);

            for (int i = 0; i < salesstatisticsitemnamesDtgrdvw.Rows.Count; i++)
            {
                myDataRow = myDataTable.NewRow();
                myDataRow["Item Name"] = salesstatisticsitemnamesDtgrdvw.Rows[i].Cells[0].Value.ToString();
                myDataRow["Number Of Item Sold"] = salesstatisticsnumberofitemsoldDtgrdvw.Rows[i].Cells[0].Value.ToString();
                myDataRow["Item Sales"] = salesstatisticsitemsalesDtgrdvw.Rows[i].Cells[0].Value.ToString();
                myDataRow["Item Profit"] = salesstatisticsitemprofitsDtgrdvw.Rows[i].Cells[0].Value.ToString();
                myDataTable.Rows.Add(myDataRow);
            }

            myDataView = new DataView(myDataTable);
            salesdataDtgrdvw.DataSource = myDataView;

        }
    }
}
