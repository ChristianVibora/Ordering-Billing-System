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
    public partial class addedituserFrm : Form
    {
        public addedituserFrm()
        {
            InitializeComponent();
        }

        private void addedituserFrm_Load(object sender, EventArgs e)
        {
            roleCmbbx.DropDownStyle = ComboBoxStyle.DropDownList;
            if (Text == "Add User") { }
            else if (Text == "Edit User")
            {
                LoadUserInfo();
            }
        }

        private void submitupdateBttn_Click(object sender, EventArgs e)
        {
            if (usernameTxtbx.Text == "" || passwordTxtbx.Text == "" || verifypasswordTxtbx.Text == "" || roleCmbbx.Text == "" || firstnameTxtbx.Text == "" || middlenameTxtbx.Text == "" || lastnameTxtbx.Text == "" || (femaleRdbttn.Checked == false && maleRdbttn.Checked == false) || contactnumberTxtbx.Text == "" || emailaddressTxtbx.Text == "" || birthdateDttmpckr.Text == "" || addressTxtbx.Text == "")
            {
                MessageBox.Show("Enter All The Required Information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (passwordTxtbx.Text != verifypasswordTxtbx.Text)
            {
                MessageBox.Show("Your Password Must Match!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                verifypasswordTxtbx.Clear();
                verifypasswordTxtbx.Focus();
            }
            else if (Regex.IsMatch(contactnumberTxtbx.Text, @"^\d+$") == false)
            {
                MessageBox.Show("Invalid Contact Number!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                contactnumberTxtbx.Clear();
                contactnumberTxtbx.Focus();
            }
            else
            {
                bool validateusername = ValidateUsername(usernameTxtbx.Text);
                bool validatename = ValidateName(firstnameTxtbx.Text, lastnameTxtbx.Text);


                if (validateusername == true || validatename == true)
                {
                    if (validateusername == true && usernameTxtbx.Modified == true)
                    {
                        MessageBox.Show("That Username Is Already Registered. Please Try A New One", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        usernameTxtbx.Clear();
                        usernameTxtbx.Focus();
                        return;
                    }

                    else if (validatename == true && (firstnameTxtbx.Modified == true || lastnameTxtbx.Modified == true))
                    {
                        MessageBox.Show("That First Name And Last Name Is Already Registered. Please Try A New One", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        firstnameTxtbx.Clear();
                        lastnameTxtbx.Clear();
                        firstnameTxtbx.Focus();
                        return;
                    }
                    else
                    {
                        if (Text == "Add User")
                        {
                            AddUser();
                            Close();
                        }
                        else if (Text == "Edit User")
                        {
                            EditUser();
                            Close();
                        }
                    }
                }
                else
                {
                    if (Text == "Add User")
                    {
                        AddUser();
                        Close();
                    }
                    else if (Text == "Edit User")
                    {
                        EditUser();
                        Close();
                    }
                }
            }
        }

        private void clearBttn_Click(object sender, EventArgs e)
        {
            usernameTxtbx.Clear();
            passwordTxtbx.Clear();
            verifypasswordTxtbx.Clear();
            roleCmbbx.SelectedIndex = -1;
            firstnameTxtbx.Clear();
            middlenameTxtbx.Clear();
            lastnameTxtbx.Clear();
            femaleRdbttn.Checked = false;
            maleRdbttn.Checked = false;
            contactnumberTxtbx.Clear();
            emailaddressTxtbx.Clear();
            birthdateDttmpckr.ResetText();
            addressTxtbx.Clear();
        }

        private void cancelBttn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoadUserInfo()
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
                myConnection.Open();

                myCommand.Parameters.Add(new OleDbParameter("@employeeid", (Tag.ToString())));

                myDataReader = myCommand.ExecuteReader();

                string username = "";
                string password = "";
                string role = "";
                string firstname = "";
                string middlename = "";
                string lastname = "";
                string gender = "";
                string contactnumber = "";
                string emailaddress = "";
                string birthdate = "";
                string address = "";

                while (myDataReader.Read())
                {
                    username = myDataReader["username"].ToString();
                    password = myDataReader["password"].ToString();
                    role = myDataReader["role"].ToString();
                    firstname = myDataReader["firstname"].ToString();
                    middlename = myDataReader["middlename"].ToString();
                    lastname = myDataReader["lastname"].ToString();
                    gender = myDataReader["gender"].ToString();
                    contactnumber = myDataReader["contactnumber"].ToString();
                    emailaddress = myDataReader["emailaddress"].ToString();
                    birthdate = myDataReader["birthdate"].ToString();
                    address = myDataReader["address"].ToString();
                }

                myConnection.Close();

                usernameTxtbx.Text = username;
                passwordTxtbx.Text = password;
                if (roleCmbbx.Tag.ToString() == "1")
                {
                    roleCmbbx.Items.Add("Admin");
                    roleCmbbx.SelectedItem = "Admin";
                    roleCmbbx.Enabled = false;
                }
                else
                    roleCmbbx.Text = role;
                firstnameTxtbx.Text = firstname;
                middlenameTxtbx.Text = middlename;
                lastnameTxtbx.Text = lastname;
                if (gender == "Female")
                    femaleRdbttn.Checked = true;
                else if (gender == "Male")
                    maleRdbttn.Checked = true;
                contactnumberTxtbx.Text = contactnumber;
                emailaddressTxtbx.Text = emailaddress;
                birthdateDttmpckr.Text = birthdate;
                addressTxtbx.Text = address;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void AddUser()
        {
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbConnection myConnection;

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "INSERT INTO [users] ([username], [password], [role], [firstname], [middlename], [lastname], [gender], [contactnumber], [emailaddress], [birthdate], [address]) VALUES (@username, @password, @role, @firstname, @middlename, @lastname, @gender, @contactnumber, @emailaddress, @birthdate, @address)";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@username", (usernameTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@password", (passwordTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@role", (roleCmbbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@firstname", (firstnameTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@middlename", (middlenameTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@lastname", (lastnameTxtbx.Text)));
                if (femaleRdbttn.Checked)
                    myCommand.Parameters.Add(new OleDbParameter("@gender", (femaleRdbttn.Text)));
                else if (maleRdbttn.Checked)
                    myCommand.Parameters.Add(new OleDbParameter("@gender", (maleRdbttn.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@contactnumber", (contactnumberTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@emailaddress", (emailaddressTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@birthdate", (birthdateDttmpckr.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@address", (addressTxtbx.Text)));
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                myCommand.Dispose();
                myConnection.Close();

                MessageBox.Show("Registration Successful! User Can Now Log In With The Username and Password.", "Registration Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void EditUser()
        {
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbConnection myConnection;

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "UPDATE [users] SET [username] = @username, [password] = @password, [role] = @role, [firstname] = @fisrtname, [middlename] = @middlename, [lastname] = @lastname, [gender] = @gender, [contactnumber] = @contactnumber, [emailaddress] = @emailaddress, [birthdate] = @birthdate, [address] = @address WHERE [employeeid] = @employeeid";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@username", (usernameTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@password", (passwordTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@role", (roleCmbbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@firstname", (firstnameTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@middlename", (middlenameTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@lastname", (lastnameTxtbx.Text)));
                if (femaleRdbttn.Checked)
                    myCommand.Parameters.Add(new OleDbParameter("@gender", (femaleRdbttn.Text)));
                else if (maleRdbttn.Checked)
                    myCommand.Parameters.Add(new OleDbParameter("@gender", (maleRdbttn.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@contactnumber", (contactnumberTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@emailaddress", (emailaddressTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@birthdate", (birthdateDttmpckr.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@address", (addressTxtbx.Text)));
                myCommand.Parameters.Add(new OleDbParameter("@employeeid", (Tag.ToString())));
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                myCommand.Dispose();
                myConnection.Close();

                MessageBox.Show("User Edit Successful! Your Informations Have Been Updated.", "Edit Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool ValidateUsername(string vusername)
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
                mySQLQuery = "SELECT * FROM [users] WHERE [username] = @vusername";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@vusername", (vusername)));
                myConnection.Open();
                myDataReader = myCommand.ExecuteReader();

                bool userFound = false;

                while (myDataReader.Read())
                {
                    userFound = true;
                }

                if (userFound == true)
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

        public bool ValidateName(string vfirstname, string vlastname)
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
                mySQLQuery = "SELECT * FROM [users] WHERE [firstname] = @vfirstname AND [lastname] = @vlastname";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@vfirstname", (vfirstname)));
                myCommand.Parameters.Add(new OleDbParameter("@vlastname", (vlastname)));
                myConnection.Open();
                myDataReader = myCommand.ExecuteReader();

                bool userFound = false;

                while (myDataReader.Read())
                {
                    userFound = true;
                }
           
                if (userFound == true)
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
