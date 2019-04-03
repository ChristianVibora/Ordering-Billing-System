using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Diagnostics;

namespace Ordering_and_Billing_System
{
    public partial class mainFrm : Form
    {
        profileFrm profileform;
        orderingmenuFrm orderingmenuform;

        public mainFrm()
        {
            InitializeComponent();
        }

        private void loginBttn_Click(object sender, EventArgs e)
        {

            if (usernameTxtbx.Text == "" || passwordTxtbx.Text == "")
            {
                MessageBox.Show("Enter Your Log In Details!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                usernameTxtbx.Focus();
            }
            else
            {
                LogIn();
            }
        }

        private void cancelBttn_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void orderingmenuBttn_Click(object sender, EventArgs e)
        {
            Clear();

            if (orderingmenuform == null)
            {
                orderingmenuform = new orderingmenuFrm();
                orderingmenuform.FormClosed += orderingmenuform_FormClosed;
            }
            orderingmenuform.Show(this);
            Hide();
        }

        private void aboutLnklbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            aboutFrm aboutform = new aboutFrm();
            aboutform.Show();
        }

        void profileform_FormClosed(object sender, FormClosedEventArgs e)
        {
            profileform = null;
            Application.Exit();
        }

        void orderingmenuform_FormClosed(object sender, FormClosedEventArgs e)
        {
            orderingmenuform = null;
            Show();
        }

        private void mainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread();
        }

        public void Clear()
        {
            usernameTxtbx.Clear();
            passwordTxtbx.Clear();
        }

        public void LogIn()
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
                mySQLQuery = "SELECT * FROM [users] WHERE [username] = @username AND [password] = @password";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@username", (usernameTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@password", (passwordTxtbx.Text)));
                myConnection.Open();
                myDataReader = myCommand.ExecuteReader();

                bool userFound = false;
                string username = "";
                string password = "";
                string employeeid = "";

                while (myDataReader.Read())
                {
                    username = myDataReader["username"].ToString();
                    password = myDataReader["password"].ToString();
                    employeeid = myDataReader["employeeid"].ToString();
                }

                if (password == passwordTxtbx.Text)
                {
                    userFound = true;
                }

                if (userFound == true)
                {
                    MessageBox.Show("Log In Successful. Welcome " + username + "!", "Log In Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (profileform == null)
                    {
                        profileform = new profileFrm();
                        profileform.Tag = employeeid;
                        profileform.FormClosed += profileform_FormClosed;
                    }
                    profileform.Show(this);
                    Hide();
                }
                else
                    MessageBox.Show("Log in Failed. Username and Password Does Not Matched! Please Try Again.", "Log In Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Clear();
                usernameTxtbx.Focus();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }         
}