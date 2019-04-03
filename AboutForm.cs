using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Deployment.Application;
using System.Windows.Forms;

namespace Ordering_and_Billing_System
{
    public partial class aboutFrm : Form
    {
        public aboutFrm()
        {
            InitializeComponent();
            label2.Text = "Version " + (ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() : Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
