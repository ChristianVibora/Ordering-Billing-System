using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using System.Data.OleDb;

namespace Ordering_and_Billing_System
{
    public partial class counterFrm : Form
    {
        delegate void SetTextCallback(string text);
        TcpListener listener;
        TcpClient client;
        NetworkStream ns;
        Thread t = null;

        string solditems = "";
        string extractprice = "";
        string price = "";
        int totalprice = 0;
        int change;
        int amountpaid;
        string receiptnumber = "";
        DateTime salesdate;

        public counterFrm()
        {
            InitializeComponent();
        }

        private void connecttoorderingmenuBttn_Click(object sender, EventArgs e)
        {
            try
            {
                Int32 port = 4545;
                IPAddress localaddr = IPAddress.Parse("127.0.0.1");
                listener = new TcpListener(localaddr, port);
                listener.Start();
                client = listener.AcceptTcpClient();
                ns = client.GetStream();
                t = new Thread(DoWork);
                t.Start();

                receiveTxtbx.Clear();
                ordereditemsLstbx.Items.Clear();
                connecttoorderingmenuBttn.Text = "Connected";
                connecttoorderingmenuBttn.Enabled = false;
                MessageBox.Show("Ordering Menu Connected!", "Connected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void loadBttn_Click(object sender, EventArgs e)
        {
            solditems = "";
            try
            {
                if (totalprice == 0)
                {
                    int last = ordereditemsLstbx.Items.Count - 1;
                    ordereditemsLstbx.SetSelected(last, true);
                    price = ordereditemsLstbx.SelectedItem.ToString();
                    ordereditemsLstbx.Items.RemoveAt(last);
                    extractprice = Regex.Match(price, @"\d+").Value;
                    totalprice = Convert.ToInt32(extractprice);
                    totalpriceLbl.Text = "₱" + totalprice + ".00";
                }
                else
                {
                    amountpaidTxtbx.Clear();
                    int last = ordereditemsLstbx.Items.Count - 1;
                    ordereditemsLstbx.SetSelected(last, true);
                    price = ordereditemsLstbx.SelectedItem.ToString();
                    ordereditemsLstbx.Items.RemoveAt(last);
                    extractprice = Regex.Match(price, @"\d+").Value;
                    totalprice += Convert.ToInt32(extractprice);
                    totalpriceLbl.Text = "₱" + totalprice + ".00";
                }
                foreach (string item in ordereditemsLstbx.Items)
                {
                    if (string.IsNullOrWhiteSpace(item) == true)
                    {
                    }
                    else
                    {
                        solditems += item + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void calculateBttn_Click(object sender, EventArgs e)
        {
            try
            {
                if (totalprice == 0)
                {
                    MessageBox.Show("Please Wait For Some Orders!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    amountpaidTxtbx.Clear();
                }
                else if (amountpaidTxtbx.Text == "")
                {
                    MessageBox.Show("Please Enter Amount!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    amountpaidTxtbx.Clear();
                }
                else if (Regex.IsMatch(amountpaidTxtbx.Text, @"^\d+$") == false)
                {
                    MessageBox.Show("Invalid Amount!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    amountpaidTxtbx.Clear();
                    amountpaidTxtbx.Focus();
                }
                else
                {
                    amountpaid = Convert.ToInt32(amountpaidTxtbx.Text);
                    if (amountpaid < totalprice)
                    {
                        MessageBox.Show("Please Enter Enough Amount!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        amountpaidTxtbx.Clear();
                    }
                    else
                    {
                        change = amountpaid - totalprice;
                        amountpaidTxtbx.Text = "₱" + amountpaid + ".00";
                        changeLbl.Text = "₱" + change + ".00";
                        finishBttn.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void finishBttn_Click(object sender, EventArgs e)
        {
            salesdate = DateTime.Now;
            receiptnumber = String.Format("OR-{0:MMddyyyyhhmmss}", salesdate);
            try
            {
                if (connecttoorderingmenuBttn.Enabled == true)
                {
                    MessageBox.Show("Please Connect To Ordering Menu!", "Order Items", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    sendTxtbx.AppendText("Eric's Lomi Haus" + Environment.NewLine);
                    sendTxtbx.AppendText("Barangay San Pedro, Sto. Tomas, Batangas" + Environment.NewLine + Environment.NewLine);
                    sendTxtbx.AppendText(String.Format("{0:F}", salesdate) + Environment.NewLine + Environment.NewLine);
                    sendTxtbx.AppendText("Cashier: " + Tag.ToString() + Environment.NewLine );
                    sendTxtbx.AppendText("Receipt Number: " + receiptnumber + Environment.NewLine + Environment.NewLine + Environment.NewLine);
                    foreach (string item in ordereditemsLstbx.Items)
                    {
                        if (string.IsNullOrWhiteSpace(item) == true)
                        {
                        }
                        else
                        {
                            sendTxtbx.AppendText(item + Environment.NewLine);
                        }
                    }
                    sendTxtbx.AppendText(Environment.NewLine + Environment.NewLine + "Total Amount: ₱" + totalprice + ".00" + Environment.NewLine);
                    sendTxtbx.AppendText("Amount Paid: ₱" + amountpaid + ".00" + Environment.NewLine);
                    sendTxtbx.AppendText("Change: ₱" + change + ".00" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
                    sendTxtbx.AppendText("Transaction Complete!" + Environment.NewLine);
                    sendTxtbx.AppendText("Press The Finish Button!" + Environment.NewLine);
                    sendTxtbx.AppendText("Please Come Back Again Later!" + Environment.NewLine);
                    
                    String s = sendTxtbx.Text;
                    byte[] byteTime = Encoding.UTF8.GetBytes(s);
                    ns.Write(byteTime, 0, byteTime.Length);

                    File.WriteAllText("Receipts/" +receiptnumber + ".txt", sendTxtbx.Text);
                    
                    UpdateSalesLog();
                    sendTxtbx.Clear();
                    receiveTxtbx.Clear();
                    ordereditemsLstbx.Items.Clear();
                    amountpaidTxtbx.Clear();
                    totalpriceLbl.Text = "₱0.00";
                    changeLbl.Text = "₱0.00";
                    totalprice = 0;
                    amountpaid = 0;
                    change = 0;
                    finishBttn.Enabled = false;
                    MessageBox.Show("Transaction Successful!", "Transaction Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void oneBttn_Click(object sender, EventArgs e)
        {
            amountpaidTxtbx.AppendText(oneBttn.Text);
        }

        private void twoBttn_Click(object sender, EventArgs e)
        {
            amountpaidTxtbx.AppendText(twoBttn.Text);
        }

        private void threeBttn_Click(object sender, EventArgs e)
        {
            amountpaidTxtbx.AppendText(threeBttn.Text);
        }

        private void fourBttn_Click(object sender, EventArgs e)
        {
            amountpaidTxtbx.AppendText(fourBttn.Text);
        }

        private void fiveBttn_Click(object sender, EventArgs e)
        {
            amountpaidTxtbx.AppendText(fiveBttn.Text);
        }

        private void sixBttn_Click(object sender, EventArgs e)
        {
            amountpaidTxtbx.AppendText(sixBttn.Text);
        }

        private void sevenBttn_Click(object sender, EventArgs e)
        {
            amountpaidTxtbx.AppendText(sevenBttn.Text);
        }

        private void eigthBttn_Click(object sender, EventArgs e)
        {
            amountpaidTxtbx.AppendText(eigthBttn.Text);
        }

        private void nineBttn_Click(object sender, EventArgs e)
        {
            amountpaidTxtbx.AppendText(nineBttn.Text);
        }

        private void zeroBttn_Click(object sender, EventArgs e)
        {
            amountpaidTxtbx.AppendText(zeroBttn.Text);
        }

        public void backBttn_Click(object sender, EventArgs e)
        {
            if (amountpaidTxtbx.SelectionStart > 0)
            {
                int index = amountpaidTxtbx.SelectionStart;
                amountpaidTxtbx.Text = amountpaidTxtbx.Text.Remove(amountpaidTxtbx.SelectionStart - 1, 1);
                amountpaidTxtbx.Select(index - 1, 0);
                amountpaidTxtbx.Focus();
            }
        }

        private void clearBttn_Click(object sender, EventArgs e)
        {
            amountpaidTxtbx.Clear();
            change = 0;
            changeLbl.Text = "₱" + change + ".00";
            
        }

        public void counterFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connecttoorderingmenuBttn.Enabled == false)
            {
                String s = "Counter Disconnected.";
                byte[] byteTime = Encoding.UTF8.GetBytes(s);
                ns.Write(byteTime, 0, byteTime.Length);
            }

            if (t != null)
            {
                ns.Close();
                t.Abort();
                client.Close();
                listener.Stop();
            }
        }

        public void DoWork()
        {
            byte[] bytes = new byte[1024];
            while (t != null)
            {
                int bytesRead = ns.Read(bytes, 0, bytes.Length);
                this.SetText(Encoding.UTF8.GetString(bytes, 0, bytesRead));
            }
        }

        private void SetText(string text)
        {
            if (this.receiveTxtbx.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.receiveTxtbx.Text = this.receiveTxtbx.Text + text;

                if (receiveTxtbx.Text == "Ordering Menu Disconnected.")
                {
                    ns.Close();
                    t.Abort();
                    client.Close();
                    listener.Stop();

                    connecttoorderingmenuBttn.Text = "Connect To Ordering Menu";
                    connecttoorderingmenuBttn.Enabled = true;
                    receiveTxtbx.Clear();
                    MessageBox.Show("Ordering Menu Disconnected!", "Disconnected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    foreach (string line in receiveTxtbx.Lines)
                    {
                        ordereditemsLstbx.Items.Add(line);
                    }
                    receiveTxtbx.Clear();
                    loadBttn.PerformClick();
                }
            }
        }

        public void UpdateSalesLog()
        {
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbConnection myConnection;

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "INSERT INTO [saleslog] ([cashier], [receiptnumber], [solditems], [totalprice], [amountpaid], [change], [salesdate]) VALUES ([@cashier], [@receiptnumber], [@solditems], [@totalprice], [@amountpaid], [@change], [@salesdate])";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@cashier", (Tag.ToString())));
                myCommand.Parameters.Add(new OleDbParameter("@receiptnumber", (receiptnumber)));
                myCommand.Parameters.Add(new OleDbParameter("@solditems", (Environment.NewLine + solditems)));
                myCommand.Parameters.Add(new OleDbParameter("@totalprice", (totalprice)));
                myCommand.Parameters.Add(new OleDbParameter("@amountpaid", (amountpaid)));
                myCommand.Parameters.Add(new OleDbParameter("@change", (change)));
                myCommand.Parameters.Add(new OleDbParameter("@salesdate", String.Format("{0:G}", salesdate)));
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
