using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows.Forms;


namespace Ordering_and_Billing_System
{
    public partial class orderqueueFrm : Form
    {
        delegate void SetTextCallback(string text);
        TcpListener listener;
        TcpClient client;
        NetworkStream ns;
        Thread t = null;

        public orderqueueFrm()
        {
            InitializeComponent();
        }

        private void connecttoorderingmenuBttn_Click(object sender, EventArgs e)
        {
            try
            {
                Int32 port = 4546;
                IPAddress localaddr = IPAddress.Parse("127.0.0.1");
                listener = new TcpListener(localaddr, port);
                listener.Start();
                client = listener.AcceptTcpClient();
                ns = client.GetStream();
                t = new Thread(DoWork);
                t.Start();

                connecttoorderingmenuBttn.Text = "Connected ";
                connecttoorderingmenuBttn.Enabled = false;
                MessageBox.Show("Ordering Menu Connected!", "Connected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void finishBttn_Click(object sender, EventArgs e)
        {
            if (connecttoorderingmenuBttn.Enabled == true)
            {
                MessageBox.Show("Please Connect To Ordering Menu!", "Order Items", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (ordereditemsLstbx.Items.ToString() == "") {
                MessageBox.Show("Please Wait For Some Orders!", "Order Items", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else {
                for (int i = ordereditemsLstbx.SelectedIndices.Count - 1; i >= 0; i--)
                {
                    int ii = ordereditemsLstbx.SelectedIndices[i];
                    ordereditemsLstbx.Items.RemoveAt(ii);
                }
            }
        }

        void ordereditemsLstbx_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = (int)((CountOccurences(((ListBox)sender).Items[e.Index].ToString(), "\n") + 1) * ((ListBox)sender).Font.GetHeight() + 2);
        }

        void ordereditemsLstbx_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index != -1) {
            string text = ((ListBox)sender).Items[e.Index].ToString();
            e.DrawBackground();
            TextRenderer.DrawText(e.Graphics, ordereditemsLstbx.GetItemText(ordereditemsLstbx.Items[e.Index]), e.Font, e.Bounds, e.ForeColor);
            e.DrawFocusRectangle();
            }
        }

        private void orderqueueFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connecttoorderingmenuBttn.Enabled == false)
            {
                String s = "Order Queue Disconnected.";
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
                    ordereditemsLstbx.Items.Add(receiveTxtbx.Text);
                    receiveTxtbx.Clear();
                }
            }
        }

        internal int CountOccurences(string haystack, string needle)
        {
            int n = 0;
            int pos = 0;
            while ((pos = haystack.IndexOf(needle, pos)) != -1)
            {
                n++;
                pos += needle.Length;
            }
            return n;
        }
    }
}

