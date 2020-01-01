using EFS.Utilities;
using System;
using System.Windows.Forms;

namespace EFS.WindowsFormApp
{
    public partial class EFSForm : Form
    {
        private string _myIpAddress;

        public EFSForm()
        {
            InitializeComponent();
        }

        private void EFSForm_Load(object sender, EventArgs e)
        {
            LoadIpAddressLabel();
            InitializeTimer();
            StartBroadcastTimer();
        }

        private void LoadIpAddressLabel()
        {
            try
            {
                _myIpAddress = EnvironmentTools.GetMyIP4IpAddress();
                yourIPLabel.Text = _myIpAddress;
            }
            catch
            {
                yourIPLabel.Text = "Unknown - Error getting IPV4";
            }
        }


        private void InitializeTimer()
        {
            BroadcastClientTimer.Interval = 5000;
            BroadcastClientTimer.Tick += new EventHandler(DoBroadcastTimerTick);
        }

        private void StartBroadcastTimer()
        {
            BroadcastClientTimer.Start();
        }

        private void DoBroadcastTimerTick(object sender, EventArgs a)
        {
            DiscoveryTools.SendDiscoveryPacket(_myIpAddress, 3040);
        }
    }
}
