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
    public partial class profileFrm : Form
    {
        adminpanelFrm adminpanelform;
        orderingmenuFrm orderingmenuform;
        counterFrm counterform;
        orderqueueFrm orderqueueform;
        inventoryFrm inventoryform;
        mainFrm mainform;
        saleslogFrm saleslogform;

        string username1 = "";

        public profileFrm()
        {
            InitializeComponent();
        }

        private void profileFrm_Load(object sender, EventArgs e)
        {
            LoadUserDetails();
        }

        private void adminpanelBttn_Click(object sender, EventArgs e)
        {
            if (adminpanelform == null)
            {
                adminpanelform = new adminpanelFrm();
                adminpanelform.FormClosed += adminpanelform_FormClosed;
            }
            adminpanelform.Show(this);
            Hide();
        }

        private void orderingmenuBttn_Click(object sender, EventArgs e)
        {
            if (orderingmenuform == null)
            {
                orderingmenuform = new orderingmenuFrm();
                orderingmenuform.FormClosed += orderingmenuform_FormClosed;
            }
            orderingmenuform.Show(this);
            Hide();
        }

        private void counterBttn_Click(object sender, EventArgs e)
        {
            if (counterform == null)
            {
                counterform = new counterFrm();
                counterform.Tag = username1;
                counterform.FormClosed += counterform_FormClosed;
            }
            counterform.Show(this);
            Hide();
        }

        private void orderqueueBttn_Click(object sender, EventArgs e)
        {
            if (orderqueueform == null)
            {
                orderqueueform = new orderqueueFrm();
                orderqueueform.FormClosed += orderqueueform_FormClosed;
            }
            orderqueueform.Show(this);
            Hide();
        }

        private void inventoryBttn_Click(object sender, EventArgs e)
        {
            if (inventoryform == null)
            {
                inventoryform = new inventoryFrm();
                inventoryform.FormClosed += inventoryform_FormClosed;
            }
            inventoryform.Show(this);
            Hide();
        }

        private void saleslogBttn_Click(object sender, EventArgs e)
        {
            if (saleslogform == null)
            {
                saleslogform = new saleslogFrm();
                saleslogform.FormClosed += saleslogform_FormClosed;
            }
            saleslogform.Show(this);
            Hide();
        }

        private void logoutBttn_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Are You Sure You Want To Log Out? ", "Log Out", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (mainform == null)
                {
                    mainform = new mainFrm();
                }
                mainform.Show(this);
                Hide();
            }
            else { }
        }

        private void exitBttn_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Are You Sure You Want To Exit? ", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        void adminpanelform_FormClosed(object sender, FormClosedEventArgs e)
        {
            adminpanelform = null;
            LoadUserDetails();
            Show();
        }

        void orderingmenuform_FormClosed(object sender, FormClosedEventArgs e)
        {
            orderingmenuform = null;
            Show();
        }

        void counterform_FormClosed(object sender, FormClosedEventArgs e)
        {
            counterform = null;
            Show();
        }

        void inventoryform_FormClosed(object sender, FormClosedEventArgs e)
        {
            inventoryform = null;
            Show();
        }

        void orderqueueform_FormClosed(object sender, FormClosedEventArgs e)
        {
            orderqueueform = null;
            Show();
        }

        void saleslogform_FormClosed(object sender, FormClosedEventArgs e)
        {
            saleslogform = null;
            Show();
        }

        void ButtonChange(string role)
        {
            if (role == "Admin")
            {
                adminpanelBttn.Enabled = true;
                orderingmenuBttn.Enabled = true;
                counterBttn.Enabled = true;
                inventoryBttn.Enabled = true;
                orderqueueBttn.Enabled = true;
                saleslogBttn.Enabled = true;
            }
            if (role == "Cashier")
            {
                counterBttn.Enabled = true;
            }
            if (role == "Chef")
            {
                orderqueueBttn.Enabled = true;
            }
            if (role == "Waiter")
            {
                orderqueueBttn.Enabled = true;
            }
            if (role == "Manager")
            {
                saleslogBttn.Enabled = true;
            }
        }

        public void LoadUserDetails()
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
                mySQLQuery = "SELECT * FROM [users] WHERE [employeeid] = @employeeid";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@employeeid", (Tag.ToString())));
                myConnection.Open();
                myDataReader = myCommand.ExecuteReader();

                string employeeid = "";
                string username = "";
                string password = "";
                string role = "";
                string firstname = "";
                string middlename = "";
                string lastname = "";
                string gender = "";
                string contactnumber = "";
                string emailaddress = "";
                object birthdate = "";
                string address = "";

                while (myDataReader.Read())
                {
                    employeeid = myDataReader["employeeid"].ToString();
                    username = myDataReader["username"].ToString();
                    password = myDataReader["password"].ToString();
                    role = myDataReader["role"].ToString();
                    firstname = myDataReader["firstname"].ToString();
                    middlename = myDataReader["middlename"].ToString();
                    lastname = myDataReader["lastname"].ToString();
                    gender = myDataReader["gender"].ToString();
                    contactnumber = myDataReader["contactnumber"].ToString();
                    emailaddress = myDataReader["emailaddress"].ToString();
                    birthdate = myDataReader["birthdate"];
                    address = myDataReader["address"].ToString();
                }

                myConnection.Close();

                employeeidLbl.Text = "EMP-NUM-" + employeeid;
                usernameLbl.Text = username;
                roleLbl.Text = role;
                firstnameLbl.Text = firstname;
                middlenameLbl.Text = middlename;
                lastnameLbl.Text = lastname;
                genderLbl.Text = gender;
                contactnumberLbl.Text = contactnumber;
                emailaddressLbl.Text = emailaddress;
                birthdateLbl.Text = String.Format("{0:d}",birthdate);
                addressLbl.Text = address;
                username1 = username;
                ButtonChange(role);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
