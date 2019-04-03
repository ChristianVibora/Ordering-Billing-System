using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Globalization;
namespace Ordering_and_Billing_System
{
    public partial class saleslogFrm : Form
    {
        salesstatisticsForm salesstatisticsform;
    
        public saleslogFrm()
        {
            InitializeComponent();
        }

        private void saleslogFrm_Load(object sender, EventArgs e)
        {
            viewCmbbx.DropDownStyle = ComboBoxStyle.DropDownList;
            viewCmbbx.SelectedIndex = 0;
            LoadSalesLog();
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
            LoadSalesLog();
        }

        private void salestatisticstBttn_Click(object sender, EventArgs e)
        {
            if (salesstatisticsform == null)
            {
                salesstatisticsform = new salesstatisticsForm();
                salesstatisticsform.FormClosed += salesstatisticsform_FormClosed;
            }
            salesstatisticsform.Show(this);
            Hide();
        }

        void salesstatisticsform_FormClosed(object sender, FormClosedEventArgs e)
        {
            salesstatisticsform = null;
            Show();
        }

        public void LoadSalesLog()
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
                    saleslogLbl.Text = "Sales Log";
                    mySQLQuery = "SELECT [totalprice], 'SLS-NUM-' & [salesid] as [Sales ID], [cashier] as [Cashier], [receiptnumber] as [Receipt Number], [solditems] as [Sold Items], '₱' & [totalprice] & '.00' as [Total Price], '₱' & [amountpaid] & '.00' as [Amount Paid], '₱' & [change] & '.00' as [Change], FormatDateTime([salesdate],0) as [Sales Date] FROM [saleslog] ORDER BY [salesdate]";
                }
                else if (viewCmbbx.Text == "Daily")
                {
                    saleslogLbl.Text = "Daily Sales Log";
                    mySQLQuery = "SELECT [totalprice], 'SLS-NUM-' & [salesid] as [Sales ID], [cashier] as [Cashier], [receiptnumber] as [Receipt Number], [solditems] as [Sold Items], '₱' & [totalprice] & '.00' as [Total Price], '₱' & [amountpaid] & '.00' as [Amount Paid], '₱' & [change] & '.00' as [Change], FormatDateTime([salesdate],0) as [Sales Date] FROM [saleslog] WHERE (DATEDIFF(\"d\", [salesdate], @date) = 0) AND (DATEDIFF(\"m\", [salesdate], @date) = 0) AND (DATEDIFF(\"yyyy\", [salesdate], @date) = 0) ORDER BY [salesdate]";
                }
                else if (viewCmbbx.Text == "Weekly")
                {
                    saleslogLbl.Text = "Weekly Sales Log";
                    mySQLQuery = "SELECT [totalprice], 'SLS-NUM-' & [salesid] as [Sales ID], [cashier] as [Cashier], [receiptnumber] as [Receipt Number], [solditems] as [Sold Items], '₱' & [totalprice] & '.00' as [Total Price], '₱' & [amountpaid] & '.00' as [Amount Paid], '₱' & [change] & '.00' as [Change], FormatDateTime([salesdate],0) as [Sales Date] FROM [saleslog] WHERE (DATEDIFF(\"ww\", [salesdate], @date, 2, 1) = 0) AND (DATEDIFF(\"yyyy\", [salesdate], @date) = 0) ORDER BY [salesdate]";
                }
                else if (viewCmbbx.Text == "Monthly")
                {
                    saleslogLbl.Text = "Monthly Sales Log";
                    mySQLQuery = "SELECT [totalprice], 'SLS-NUM-' & [salesid] as [Sales ID], [cashier] as [Cashier], [receiptnumber] as [Receipt Number], [solditems] as [Sold Items], '₱' & [totalprice] & '.00' as [Total Price], '₱' & [amountpaid] & '.00' as [Amount Paid], '₱' & [change] & '.00' as [Change], FormatDateTime([salesdate],0) as [Sales Date] FROM [saleslog] WHERE (DATEDIFF(\"m\", [salesdate], @date) = 0) AND (DATEDIFF(\"yyyy\", [salesdate], @date) = 0) ORDER BY [salesdate]";
                }
                else if (viewCmbbx.Text == "Yearly")
                {
                    saleslogLbl.Text = "Yearly Sales Log";
                    mySQLQuery = "SELECT [totalprice], 'SLS-NUM-' & [salesid] as [Sales ID], [cashier] as [Cashier], [receiptnumber] as [Receipt Number], [solditems] as [Sold Items], '₱' & [totalprice] & '.00' as [Total Price], '₱' & [amountpaid] & '.00' as [Amount Paid], '₱' & [change] & '.00' as [Change], FormatDateTime([salesdate],0) as [Sales Date] FROM [saleslog] WHERE (DATEDIFF(\"yyyy\", [salesdate], @date) = 0) ORDER BY [salesdate]";
                }

                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@date", (date)));
                myConnection.Open();
                myDataAdapter.SelectCommand = myCommand;
                myDataAdapter.Fill(myDataTable);
                saleslogDtgrdvw.DataSource = myDataTable;
                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
