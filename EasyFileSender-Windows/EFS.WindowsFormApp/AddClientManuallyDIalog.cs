using EFS.Global.Models;
using EFS.Utilities.Discovery;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EFS.WindowsFormApp
{
    public partial class AddClientManuallyDialog : Form
    {
        public ClientInfo _clientInfo { get; set; }
        public AddClientManuallyDialog()
        {
            InitializeComponent();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            addButton.Text = "Checking..";
            addButton.Enabled = false;

            CheckFtpClientResult discoverResult = DiscoveryUtils.CheckFTPCLientExists(ipAddressText.Text);
            if (discoverResult.ClientFound)
            {
                _clientInfo = discoverResult.ClientInfo;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Failed to add client : " + discoverResult.FailureMessage, "Failed", MessageBoxButtons.OK);
            }

            addButton.Text = "Add Client";
            addButton.Enabled = true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
