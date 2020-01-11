using EFS.Global.Enums;
using EFS.Global.Models;
using EFS.Utilities;
using EFS.Utilities.Discovery;
using EFS.Utilities.FileTransfer;
using EFS.WindowsFormApp.ViewModels;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace EFS.WindowsFormApp
{
    public partial class EFSForm : Form
    {
        private string _myIpAddress;
        private DiscoveryService _discoveryService;
        private FTPServerService _ftpServerService;
        private int _port = 3008;
        private BindingList<ClientInfoViewModelListItem> _clientList = new BindingList<ClientInfoViewModelListItem>();
        private string _downloadsDirectory;

        public EFSForm()
        {
            InitializeComponent();
        }

        private void EFSForm_Load(object sender, EventArgs e)
        {
            clientListBox.DataSource = _clientList;
            LoadIpAddressLabel();

            // Add my client to top of list
            _clientList.Add(new ClientInfoViewModelListItem()
            {
                ClientType = ClientTypeEnum.windows.ToString(),
                IpAddress = _myIpAddress,
                IsSelfClient = true,
                Version = VersionNumberEnum.v1.ToString()
            });

            _discoveryService = new DiscoveryService(_myIpAddress, _port, OnRecievedClientData, 500);
            _discoveryService.StartDiscoveryService();

            _downloadsDirectory = EnvironmentTools.GetDownloadsFolder();
            _ftpServerService = new FTPServerService(_downloadsDirectory, 21);
            _ftpServerService.StartService();
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

        private void EFSForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _discoveryService.StopDiscoveryService();
            _ftpServerService.StopService();
        }

        private void OnRecievedClientData(ClientInfo clientInfo)
        {
            Invoke(new MethodInvoker(() =>
            {
                if (!_clientList.Any(a => a.IpAddress == clientInfo.IpAddress))
                {
                    _clientList.Add(new ClientInfoViewModelListItem(clientInfo, string.Equals(_myIpAddress, clientInfo.IpAddress)));
                }
            }));           
        }

        public void DrawSelectedClient()
        {
            if(clientListBox.SelectedItem != null)
            {
                ClientInfoViewModelListItem clientObjectSelected = (ClientInfoViewModelListItem)clientListBox.SelectedItem;
                if(clientObjectSelected.IsSelfClient)
                {
                    selectedIpLabel.Text = "You: " + clientObjectSelected.IpAddress + " - Receiving Files";
                    sendFileButton.Visible = false;
                    transfersPanel.Size = new System.Drawing.Size(transfersPanel.Size.Width, this.Size.Height - 89);
                }
                else
                {
                    selectedIpLabel.Text = clientObjectSelected.IpAddress;
                    sendFileButton.Visible = true;
                    transfersPanel.Size = new System.Drawing.Size(transfersPanel.Size.Width, this.Size.Height - 118);
                }
            }
        }

        private void ClientListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            DrawSelectedClient();
        }

        private void SendFileButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // Get the object used to communicate with the server.
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + selectedIpLabel.Text);
                    request.Method = WebRequestMethods.Ftp.UploadFile;

                    // This example assumes the FTP site uses anonymous logon.
                    request.Credentials = new NetworkCredential("anonymous", "anon@anon.com");

                    // Copy the contents of the file to the request stream.
                    using (StreamReader sourceStream = new StreamReader(ofd.FileName))
                    using (Stream ftpStream = request.GetRequestStream())
                    {
                        sourceStream.BaseStream.CopyTo(ftpStream);
                    }

                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                    }
                }
            }
        }
    }
}
