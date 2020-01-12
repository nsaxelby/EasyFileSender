using EFS.Global.Enums;
using EFS.Global.Models;
using EFS.Utilities;
using EFS.Utilities.Discovery;
using EFS.Utilities.FileTransfer;
using EFS.WindowsFormApp.ViewModels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EFS.WindowsFormApp
{
    public partial class EFSForm : Form
    {
        private string _myIpAddress;
        private DiscoveryService _discoveryService;
        private FTPServerService _ftpServerService;
        private SendFileService _sendFileService;
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

            _sendFileService = new SendFileService();
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
            _sendFileService.StopAllThreads();
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
                    dragAndDropPanel.Visible = false;
                    transfersPanel.Size = new System.Drawing.Size(transfersPanel.Size.Width, this.Size.Height - 89);
                }
                else
                {
                    selectedIpLabel.Text = clientObjectSelected.IpAddress;
                    sendFileButton.Visible = true;
                    dragAndDropPanel.Visible = true;
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
                    try
                    {
                        CommonTransfer(ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error Starting Transfer: " + ex.Message, "Transfer Error");
                    }
                }
            }
        }

        private void OnFileTransferStatusChanged(FileTransferStatus fileTransferStatus)
        {
            Debug.WriteLine("Transfer guid: " + fileTransferStatus.TransferID.ToString() + " + file : " + fileTransferStatus.SourceFile + " percentage: " + fileTransferStatus.Progress.ToString("00.00"));
        }

        private void DragAndDropPanel_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                try
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    foreach (var FilesDroppedName in files)
                    {
                        Guid returnedGuid = CommonTransfer(FilesDroppedName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Starting Transfer: " + ex.Message, "Transfer Error");
                }
            }
        }

        private Guid CommonTransfer(string fileName)
        {
            long fileSizeBytes = 0;
            fileSizeBytes = new FileInfo(fileName).Length;
            Guid guid = Guid.NewGuid();
            _sendFileService.StartNewTransferThread(guid, selectedIpLabel.Text, fileName, fileSizeBytes, OnFileTransferStatusChanged);
            return guid;
        }

        private void DragAndDropPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            { 
                e.Effect = DragDropEffects.Copy;
            }
        }
    }
}
