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
    public partial class adminpanelFrm : Form
    {
        addedituserFrm addedituserform;
        inventorynotificationsFrm inventorynotificationsform;

        public adminpanelFrm()
        {
            InitializeComponent();
        }

        private void adminpanelFrm_Load(object sender, EventArgs e)
        {
            LoadUsers();

            if (inventorynotificationsform == null)
            {
                inventorynotificationsform = new inventorynotificationsFrm();
                inventorynotificationsform.refreshBttn.Click += refreshBttn_Click;
            }

            LoadNewNotifications();
            LoadOldNotifications();
        }

        private void addBttn_Click(object sender, EventArgs e)
        {
            if (addedituserform == null)
            {
                addedituserform = new addedituserFrm();
                addedituserform.Text = "Add User";
                addedituserform.submitupdateBttn.Text = "Submit";
                addedituserform.FormClosed += addedituserform_FormClosed;
            }
            addedituserform.Show(this);
            Hide();
        }

        private void editBttn_Click(object sender, EventArgs e)
        {
            if (addedituserform == null)
            {
                addedituserform = new addedituserFrm();
                addedituserform.Tag = usersDtgrdvw[0, usersDtgrdvw.CurrentRow.Index].Value;
                addedituserform.roleCmbbx.Tag = usersDtgrdvw[0, usersDtgrdvw.CurrentRow.Index].Value;
                addedituserform.Text = "Edit User";
                addedituserform.submitupdateBttn.Text = "Update";
                addedituserform.FormClosed += addedituserform_FormClosed;
            }
            addedituserform.Show(this);
            Hide();
        }

        private void deleteBttn_Click(object sender, EventArgs e)
        {
            Tag = usersDtgrdvw[0, usersDtgrdvw.CurrentRow.Index].Value.ToString();
            if (Tag.ToString() == "1")
            {
                MessageBox.Show("You Cannot Delete The Admin Profile!", "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                DialogResult result;
                result = MessageBox.Show("Are You Sure You Want To Delete The Selected User?", "Delete User?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    DeleteUser();
                }
                else { }
            }
            LoadUsers();
        }

        private void inventorynnotificationsBttn_Click(object sender, EventArgs e)
        {
            if (inventorynotificationsform.IsDisposed)
            {
                inventorynotificationsform = new inventorynotificationsFrm();
                inventorynotificationsform.refreshBttn.Click += refreshBttn_Click;
            }
            LoadNewNotifications();
            LoadOldNotifications();
            inventorynotificationsform.Show(this);
        }

        void refreshBttn_Click(object sender, EventArgs e) {
            LoadNewNotifications();
            LoadOldNotifications();
        }

        void addedituserform_FormClosed(object sender, FormClosedEventArgs e)
        {
            addedituserform = null;
            LoadUsers();
            Show();
        }

        public void LoadUsers()
        {
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbConnection myConnection;
            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter();
            DataTable myDataTable = new DataTable();

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "SELECT [employeeid], 'EMP-NUM-' & [employeeid] as [Employee Number], [firstname] + ' ' + [middlename] + ' ' + [lastname] as [Full Name], [role] as [Role], [username] as [Username], [password] as [Password], [gender] as [Gender], [contactnumber] as [Contact Number], [emailaddress] as [Email Address], [birthdate] as [Birthdate], [address] as [Address] FROM [users]";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myConnection.Open();
                myDataAdapter.SelectCommand = myCommand;
                myDataAdapter.Fill(myDataTable);
                usersDtgrdvw.DataSource = myDataTable;
                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadNewNotifications()
        {
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbConnection myConnection;
            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter();
            DataTable myDataTable = new DataTable();

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "SELECT 'INVNTRY-NTF-NUM-' & [inventorynotifID] as [Notification ID], [itemname] as [Item Name], [stocksleft] as [Stocks Left], FormatDateTime([notifdate]) as [Date] FROM [inventorynotifications] WHERE (DATEDIFF(\"d\", [notifdate], @date) = 0) AND (DATEDIFF(\"m\", [notifdate], @date) = 0) AND (DATEDIFF(\"yyyy\", [notifdate], @date) = 0) ORDER BY [notifdate] DESC";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@date", (DateTime.Now.ToString())));
                myConnection.Open();
                myDataAdapter.SelectCommand = myCommand;
                myDataAdapter.Fill(myDataTable);
               inventorynotificationsform.newDtgrdvw.DataSource = myDataTable;
                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (inventorynotificationsform.newDtgrdvw.Rows.Count >= 0)
            {
                inventorynnotificationsBttn.Text = "Inventory Notifications (" + inventorynotificationsform.newDtgrdvw.Rows.Count.ToString() + " new)";
            }
        }

        public void LoadOldNotifications()
        {
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbConnection myConnection;
            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter();
            DataTable myDataTable = new DataTable();

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "SELECT 'INVNTRY-NTF-NUM-' & [inventorynotifID] as [Notification ID], [itemname] as [Item Name], [stocksleft] as [Stocks Left], FormatDateTime([notifdate]) as [Date] FROM [inventorynotifications] WHERE NOT (DATEDIFF(\"d\", [notifdate], @date) = 0) OR NOT (DATEDIFF(\"m\", [notifdate], @date) = 0) OR NOT (DATEDIFF(\"yyyy\", [notifdate], @date) = 0) ORDER BY [notifdate] DESC";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@date", (DateTime.Now.ToString())));
                myConnection.Open();
                myDataAdapter.SelectCommand = myCommand;
                myDataAdapter.Fill(myDataTable);
               inventorynotificationsform.oldDtgrdvw.DataSource = myDataTable;
                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (inventorynotificationsform.newDtgrdvw.Rows.Count >= 0)
            {
                inventorynnotificationsBttn.Text = inventorynnotificationsBttn.Text + " (" + inventorynotificationsform.oldDtgrdvw.Rows.Count.ToString() + " old)";
            }
        }

        public void DeleteUser()
        {
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbConnection myConnection;
            
            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "DELETE FROM [users] WHERE [employeeid] = @employeeid";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@employeeid", (Tag.ToString())));
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                myConnection.Close();

                MessageBox.Show("User Delete Successful!", "Delete Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
