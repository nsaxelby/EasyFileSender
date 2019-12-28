using EFS.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EFS.WindowsFormApp
{
    public partial class EFSForm : Form
    {
        public EFSForm()
        {
            InitializeComponent();
        }

        private void EFSForm_Load(object sender, EventArgs e)
        {
            LoadIpAddressLabel();
        }

        private void LoadIpAddressLabel()
        {
            try
            {
                yourIPLabel.Text = EnvironmentTools.GetMyIP4IpAddress();
            }
            catch
            {
                yourIPLabel.Text = "Unknown - Error getting IPV4";
            }
        }
    }
}
