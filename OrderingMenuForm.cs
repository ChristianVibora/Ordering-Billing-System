using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Data.OleDb;

namespace Ordering_and_Billing_System
{
    public partial class orderingmenuFrm : Form
    {
        private const int port = 4545;
        delegate void SetTextCallback(string text);
        TcpClient client;
        NetworkStream ns;
        Thread t = null;
        private const String hostName = "localhost";

        private const int port1 = 4546;
        delegate void SetTextCallback1(string text);
        TcpClient client1;
        NetworkStream ns1;
        Thread t1 = null;
        private const String hostName1 = "localhost";

        int totalprice = 0;
        int ordercount = 0;

        string[] namelist = null;
        int[] quantitylist = null;

        public orderingmenuFrm()
        {
            InitializeComponent();

        }

        private void orderingmenuFrm_Load(object sender, EventArgs e)
        {
            tablenumberCmbbx.DropDownStyle = ComboBoxStyle.DropDownList;
            FormLoad();
        }

        private void connecttocounterBttn_Click(object sender, EventArgs e)
        {
            try
            {
                client = new TcpClient(hostName, port);
                ns = client.GetStream();
                t = new Thread(DoWork);
                t.Start();

                FormLoad();
                connecttocounterBttn.Text = "Connected To Counter";
                connecttocounterBttn.Enabled = false;
                buyBttn.Enabled = true;
                MessageBox.Show("Counter Connected!", "Connected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void connecttoorderqueueBttn_Click(object sender, EventArgs e)
        {
            try
            {
                client1 = new TcpClient(hostName1, port1);
                ns1 = client1.GetStream();
                t1 = new Thread(DoWork1);
                t1.Start();

                FormLoad();
                connecttoorderqueueBttn.Text = "Connected To Order Queue";
                connecttoorderqueueBttn.Enabled = false;
                buyBttn.Enabled = true;
                MessageBox.Show("Order Queue Connected!", "Connected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void buyBttn_Click(object sender, EventArgs e)
        {
            if (connecttocounterBttn.Enabled == true || connecttoorderqueueBttn.Enabled == true)
            {
                MessageBox.Show("Please Connect To Counter And Order Queue!", "Order Items", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (totalprice == 0)
            {
                MessageBox.Show("Please Order Some Items!", "Order Items", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (tablenumberCmbbx.Text == "")
            {
                MessageBox.Show("Please Choose Table Number!", "Order Items", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                tablenumberCmbbx.Focus();
            }
            else if (ordereditemsLstbx.Items.Count != itemnameLstbx.Items.Count || ordereditemsLstbx.Items.Count != itemquantityLstbx.Items.Count)
            {
                MessageBox.Show("Internal Data Error! Please Re-Order Again", "Order Items", MessageBoxButtons.OK, MessageBoxIcon.Error);
                clearBttn.PerformClick();
            }
            else
            {
                DialogResult res;
                res = MessageBox.Show("Are You Sure You Want To Buy These Items?", "Order Items", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    buyBttn.Enabled = false;
                    
                    ordereditemsLstbx.Items.Add("Total Price:    ₱" + totalprice + ".00");
                    foreach (string item in ordereditemsLstbx.Items)
                    {
                        sendTxtbx.AppendText(item);
                    }
                    send1Txtbx.AppendText("----------------------------------" + Environment.NewLine);
                    send1Txtbx.AppendText("Table #" + tablenumberCmbbx.Text + Environment.NewLine);
                    send1Txtbx.AppendText("----------------------------------" + Environment.NewLine);
                    foreach (string item in ordereditemsLstbx1.Items)
                    {
                        send1Txtbx.AppendText(item);
                    }
                    send1Txtbx.AppendText("----------------------------------");

                    WriteToCounter();

                    namelist = new string[itemnameLstbx.Items.Count];
                    for (int a = 0; a < itemnameLstbx.Items.Count; a++)
                    {
                        namelist[a] = itemnameLstbx.Items[a].ToString();
                    }

                    quantitylist = new int[itemquantityLstbx.Items.Count];
                    for (int a = 0; a < itemquantityLstbx.Items.Count; a++)
                    {
                        quantitylist[a] = Convert.ToInt32(itemquantityLstbx.Items[a]);
                    }

                    FormLoad();
                }
            }
        }

        private void clearBttn_Click(object sender, EventArgs e)
        {
            FormLoad();
            send1Txtbx.Clear();
            ordereditemsLstbx1.Items.Clear();
        }

        private void finishBttn_Click(object sender, EventArgs e)
        {
            FormLoad();
            buyBttn.Enabled = true;
            clearBttn.Enabled = true;
            send1Txtbx.Clear();
            ordereditemsLstbx1.Items.Clear();
        }

        private void itemsDtgrdvw_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            bool ValidRow = (e.RowIndex != -1);
            var datagridview = sender as DataGridView;

            if (datagridview.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && ValidRow)
            {
                datagridview.BeginEdit(true);
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
            }
        }

        private void itemsDtgrdvw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var datagridview = sender as DataGridView;
            datagridview.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void shortordersDtgrdvw_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            var datagridview = sender as DataGridView;
            var checkbox = sender as DataGridViewCheckBoxColumn;

            if ((e.ColumnIndex == dataGridViewCheckBoxColumn.Index || e.ColumnIndex == dataGridViewCheckBoxColumn1.Index || e.ColumnIndex == dataGridViewCheckBoxColumn1.Index) && e.RowIndex != -1)
            {
                datagridview.EndEdit();
            }
        }

        private void itemsDtgrdvw_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            string itemquantity = "";
            string itemname = "";
            string itemprice = "";
            int quantity;
            int price;
            int orderprice = 0;
            int count = 1;
            var datagridview = sender as DataGridView;


            if (datagridview.DataSource != null)
            {
                if ((e.ColumnIndex == dataGridViewCheckBoxColumn.Index || e.ColumnIndex == dataGridViewCheckBoxColumn1.Index || e.ColumnIndex == dataGridViewCheckBoxColumn1.Index) && e.RowIndex != -1)
                {
                    if (datagridview.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value != null)
                    {
                        itemquantity = datagridview.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value.ToString();
                        itemname = datagridview.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value.ToString();
                        itemprice = datagridview.Rows[e.RowIndex].Cells[e.ColumnIndex + 2].Value.ToString();

                        quantity = Convert.ToInt32(datagridview.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value);
                        price = Convert.ToInt32(datagridview.Rows[e.RowIndex].Cells[e.ColumnIndex + 3].Value);
                        orderprice = quantity * price;

                        int stocksleft = CheckInventory(itemname, quantity);

                        if (datagridview.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "True")
                        {
                            ordereditemsLstbx.Items.Add("x" + itemquantity + " " + itemname + " (" + itemprice + ")     ₱" + orderprice + ".00" + Environment.NewLine);
                            ordereditemsLstbx1.Items.Add("x" + itemquantity + " " + itemname + Environment.NewLine);
                            itemnameLstbx.Items.Add(itemname);
                            itemquantityLstbx.Items.Add(itemquantity);
                            ordercount += count;
                            totalprice += orderprice;

                            if (ordercount > 10)
                            {
                                MessageBox.Show("Maximum Order Count Is 10!", "Not Enough Stocks", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                count = 0;
                                price = 0;
                                datagridview.Rows[e.RowIndex].Cells[1].Value = false;
                            }

                            if (stocksleft < Convert.ToInt32(itemquantity))
                            {
                                MessageBox.Show("Not Enough Stocks For: " + itemname + ". (Stocks Left: " + stocksleft + ")", "Not Enough Stocks", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                count = 0;
                                price = 0;
                                datagridview.Rows[e.RowIndex].Cells[1].Value = false;
                            }
                        }
                        else
                        {
                            ordereditemsLstbx.Items.Remove("x" + itemquantity + " " + itemname + " (" + itemprice + ")     ₱" + orderprice + ".00" + Environment.NewLine);
                            ordereditemsLstbx1.Items.Remove("x" + itemquantity + " " + itemname + Environment.NewLine);
                            itemnameLstbx.Items.Remove(itemname);
                            itemquantityLstbx.Items.Remove(itemquantity);
                            ordercount -= count;
                            totalprice -= orderprice;
                        }
                        ordercountLbl.Text = ordercount.ToString() + "/10";
                        totalpriceLbl.Text = "₱" + totalprice.ToString() + ".00";
                    }
                    else
                    {
                        datagridview.Rows[e.RowIndex].Cells[1].Value = false;
                    }
                }
            }
        }

        private void orderingmenuFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connecttocounterBttn.Enabled == false)
            {
                String s = "Ordering Menu Disconnected.";
                byte[] byteTime = Encoding.UTF8.GetBytes(s);
                ns.Write(byteTime, 0, byteTime.Length);
            }
            if (t != null)
            {
                ns.Close();
                t.Abort();
                client.Close();
            }

            if (connecttoorderqueueBttn.Enabled == false)
            {
                String s = "Ordering Menu Disconnected.";
                byte[] byteTime = Encoding.UTF8.GetBytes(s);
                ns1.Write(byteTime, 0, byteTime.Length);
            }
            if (t1 != null)
            {
                ns1.Close();
                t1.Abort();
                client1.Close();
            }
        }

        public void FormLoad()
        {
            finishBttn.Enabled = false;

            shortordersDtgrdvw.DataSource = GetItems("Short Order");
            drinksDtgrdvw.DataSource = GetItems("Drinks");
            dessertsDtgrdvw.DataSource = GetItems("Dessert");

            tablenumberCmbbx.SelectedIndex = -1;
            sendTxtbx.Clear();
            receiveTxtbx.Clear();
            receive1Txtbx.Clear();
            ordereditemsLstbx.Items.Clear();
            itemnameLstbx.Items.Clear();
            itemquantityLstbx.Items.Clear();
            ordercount = 0;
            totalprice = 0;
            ordercountLbl.Text = ordercount.ToString() + "/10";
            totalpriceLbl.Text = "₱" + totalprice.ToString() + ".00";
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

        public void DoWork1()
        {
            byte[] bytes = new byte[1024];
            while (t != null)
            {
                int bytesRead = ns1.Read(bytes, 0, bytes.Length);
                this.SetText1(Encoding.UTF8.GetString(bytes, 0, bytesRead));
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

                if (receiveTxtbx.Text == "Counter Disconnected.")
                {
                    ns.Close();
                    t.Abort();
                    client.Close();

                    connecttocounterBttn.Text = "Connect To Counter";
                    connecttocounterBttn.Enabled = true;
                    receiveTxtbx.Clear();
                    MessageBox.Show("Counter Disconnected!", "Disconnected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    foreach (string line in receiveTxtbx.Lines)
                    {
                        ordereditemsLstbx.Items.Add(line);
                    }

                        for (int n = 0; n < namelist.Count(); n++)
                        {
                            UpdateInventory(namelist[n], CheckInventory(namelist[n], quantitylist[n]) - quantitylist[n]);
                            UpdateInventoryLog(namelist[n], quantitylist[n]);
                            UpdateInventoryNotifications(namelist[n], CheckInventory(namelist[n], quantitylist[n]));
                        }
        
                    WriteToOrderQueue();

                    namelist = null;
                    quantitylist = null;
                    receiveTxtbx.Clear();
                    ordereditemsLstbx1.Items.Clear();
                    send1Txtbx.Clear();
                    finishBttn.Enabled = true;
                    clearBttn.Enabled = false;
                }
            }
        }

        private void SetText1(string text)
        {
            if (this.receive1Txtbx.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText1);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.receive1Txtbx.Text = this.receive1Txtbx.Text + text;

                if (receive1Txtbx.Text == "Order Queue Disconnected.")
                {
                    ns1.Close();
                    t1.Abort();
                    client1.Close();

                    connecttoorderqueueBttn.Text = "Connect To Order Queue";
                    connecttoorderqueueBttn.Enabled = true;
                    receive1Txtbx.Clear();
                    MessageBox.Show("Order Queue Disconnected!", "Disconnected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        public void WriteToCounter()
        {
            try
            {
                String s = sendTxtbx.Text;
                byte[] byteTime = Encoding.UTF8.GetBytes(s);
                ns.Write(byteTime, 0, byteTime.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void WriteToOrderQueue()
        {
            try
            {
                String s = send1Txtbx.Text;
                byte[] byteTime = Encoding.UTF8.GetBytes(s);
                ns1.Write(byteTime, 0, byteTime.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public DataTable GetItems(string category)
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
                mySQLQuery = "SELECT [itemname] as [Item Name], '₱' & [sellprice] & '.00' as [Item Price], [sellprice] FROM [inventory] WHERE [category] = @category";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@category", (category)));
                myConnection.Open();
                myDataAdapter = new OleDbDataAdapter(myCommand);
                myDataAdapter.Fill(myDataTable = new DataTable());
                myConnection.Close();

                return myDataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        public int CheckInventory(string ordereditem, int orderedquantity)
        {
            int stocksleft = 0;
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbDataReader myDataReader;
            OleDbConnection myConnection;

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "SELECT [itemname], [stocksleft] FROM [inventory] WHERE [itemname] = @ordereditem";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@ordereditem", (ordereditem)));
                myConnection.Open();
                myDataReader = myCommand.ExecuteReader();

                while (myDataReader.Read())
                {
                    stocksleft = Convert.ToInt32(myDataReader["stocksleft"].ToString());
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return stocksleft;
        }

        public void UpdateInventory(string itemname, int stocksleft)
        {
            string MyConnectionString;
            string mySQLQuery;
            OleDbCommand myCommand;
            OleDbConnection myConnection;

            try
            {
                MyConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Eric's Lomi Haus Database.mdb;Jet OLEDB:Database Password=SoftwareEngineering";
                myConnection = new OleDbConnection(MyConnectionString);
                mySQLQuery = "UPDATE [inventory] SET [stocksleft] = @stocksleft WHERE [itemname] = @itemname";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@stocksleft", (stocksleft)));
                myCommand.Parameters.Add(new OleDbParameter("@itemname", (itemname)));
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

        public void UpdateInventoryNotifications(string itemname, int stocksleft)
        {
            if (stocksleft <= 30)
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
                    mySQLQuery = "INSERT INTO [inventorynotifications] ([itemname], [stocksleft], [notifdate]) VALUES (@itemname, @stocksleft, @logdate)";
                    myCommand = new OleDbCommand(mySQLQuery, myConnection);
                    myCommand.Parameters.Add(new OleDbParameter("@itemname", (itemname)));
                    myCommand.Parameters.Add(new OleDbParameter("@stocksleft", (stocksleft)));
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
            else { }
        }

        public void UpdateInventoryLog(string itemname, int soldquantity)
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
                mySQLQuery = "INSERT INTO [inventorylog] ([logtype], [itemname], [soldquantity], [logdate]) VALUES (@logtype, @itemname, @soldquantity, @logdate)";
                myCommand = new OleDbCommand(mySQLQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("@logtype", ("Sold")));
                myCommand.Parameters.Add(new OleDbParameter("@itemname", (itemname)));
                myCommand.Parameters.Add(new OleDbParameter("@soldquantity", (soldquantity)));
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