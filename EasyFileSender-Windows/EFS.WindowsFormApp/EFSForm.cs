using EFS.Global.Enums;
using EFS.Global.Models;
using EFS.Utilities;
using EFS.Utilities.Discovery;
using EFS.Utilities.FileTransfer;
using EFS.WindowsFormApp.ViewModels;
using System;
using System.Collections.Generic;
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
        private BindingList<FileTransferStatus> _selectedClientTransferList = new BindingList<FileTransferStatus>();
        private string _downloadsDirectory;

        public EFSForm()
        {
            InitializeComponent();
        }

        private void EFSForm_Load(object sender, EventArgs e)
        {
            clientListBox.DataSource = _clientList;
            transfersListBox.DataSource = _selectedClientTransferList;
            _selectedClientTransferList.ListChanged += _selectedClientTransferList_ListChanged;

            // Pops up a box to select adapter/network/IP to broadcast to others
            ConfigureMyIPAddress();

            LoadIpAddressLabel();

            // Add my client to top of list
            _clientList.Add(new ClientInfoViewModelListItem()
            {
                ClientType = ClientTypeEnum.windows.ToString(),
                IpAddress = _myIpAddress,
                IsSelfClient = true,
                Version = VersionNumberEnum.v1.ToString()
            });

            if (string.IsNullOrEmpty(_myIpAddress) == false)
            {
                _discoveryService = new DiscoveryService(_myIpAddress, _port, OnRecievedClientData, 500);
                _discoveryService.StartDiscoveryService();
            }

            _downloadsDirectory = EnvironmentTools.GetDownloadsFolder();
            _ftpServerService = new FTPServerService(_downloadsDirectory, 21);
            _ftpServerService.StartService();

            _sendFileService = new SendFileService();
        }

        private void _selectedClientTransferList_ListChanged(object sender, ListChangedEventArgs e)
        {
            // TODO, is there a better way to do this?
            transfersListBox.Invalidate();
        }

        private void ConfigureMyIPAddress()
        {
            List<IPAddressInfo> ipAddresses = EnvironmentTools.GetIPV4Addresses();
            if(ipAddresses.Count == 0)
            {
                MessageBox.Show("Unable to find Network to bind to");
                Application.Exit();
            }
            else if(ipAddresses.Count == 0)
            {
                _myIpAddress = ipAddresses[0].IpAddress;
            }
            else
            {
                // Multiple
                using(SelectNetworkAdapterDialog snad = new SelectNetworkAdapterDialog(ipAddresses))
                {
                    if(snad.ShowDialog() == DialogResult.OK)
                    {
                        _myIpAddress = snad.SelectedIpAddress;
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
            }
        }

        private void LoadIpAddressLabel()
        {
            yourIPLabel.Text = _myIpAddress;
        }

        private void EFSForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_discoveryService != null)
            {
                _discoveryService.StopDiscoveryService();
            }
            if (_ftpServerService != null)
            {
                _ftpServerService.StopService();
            }
            if (_sendFileService != null)
            {
                _sendFileService.StopAllThreads();
            }
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
                    _selectedClientTransferList.Clear();
                    selectedIpLabel.Text = "You: " + clientObjectSelected.IpAddress + " - Receiving Files";
                    sendFileButton.Visible = false;
                    dragAndDropPanel.Visible = false;
                    transfersPanel.Size = new System.Drawing.Size(transfersPanel.Size.Width, this.Size.Height - 89);
                }
                else
                {
                    selectedIpLabel.Text = clientObjectSelected.IpAddress;
                    AddSelectedClientTransfersToBindingList(clientObjectSelected.IpAddress);
                    sendFileButton.Visible = true;
                    dragAndDropPanel.Visible = true;
                    transfersPanel.Size = new System.Drawing.Size(transfersPanel.Size.Width, this.Size.Height - 118);
                }
            }
            else
            {
                _selectedClientTransferList.Clear();
            }
        }

        private void AddSelectedClientTransfersToBindingList(string destinationIP)
        {
            _selectedClientTransferList.Clear();
            foreach (FileTransferStatus clientTransfers in _sendFileService.GetTransfersByDestIP(destinationIP))
            {
                _selectedClientTransferList.Add(clientTransfers);
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
                        CommonTransfer(FilesDroppedName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Starting Transfer: " + ex.Message, "Transfer Error");
                }
            }
        }

        private void CommonTransfer(string fileName)
        {
            long fileSizeBytes = 0;
            fileSizeBytes = new FileInfo(fileName).Length;
            _selectedClientTransferList.Add(_sendFileService.StartNewTransferThread(selectedIpLabel.Text, fileName, fileSizeBytes, OnFileTransferStatusChanged));
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
