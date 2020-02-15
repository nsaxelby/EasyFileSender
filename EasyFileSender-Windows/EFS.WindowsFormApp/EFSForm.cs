using EFS.Global.Enums;
using EFS.Global.Models;
using EFS.Shared.EventModels;
using EFS.Utilities;
using EFS.Utilities.Discovery;
using EFS.Utilities.FileTransfer;
using EFS.WindowsFormApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private BindingList<FileTransferStatus> _selectedClientSendTransferList = new BindingList<FileTransferStatus>();
        private string _downloadsDirectory;

        public EFSForm()
        {
            _downloadsDirectory = EnvironmentTools.GetDownloadsFolder();
            _ftpServerService = new FTPServerService(_downloadsDirectory, 21);
            InitializeComponent();
        }

        private void EFSForm_Load(object sender, EventArgs e)
        {
            clientListBox.DataSource = _clientList;
            transfersListBox.DataSource = _selectedClientSendTransferList;

            // Change to either list can be handled by the same event handler
            _selectedClientSendTransferList.ListChanged += _selectedClientTransferList_ListChanged;
            _ftpServerService._myIncommingTransfers.ListChanged += _selectedClientTransferList_ListChanged;

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
            
            _ftpServerService.StartService();

            _sendFileService = new SendFileService();
        }

        private void _selectedClientTransferList_ListChanged(object sender, ListChangedEventArgs e)
        {
            Invoke(new MethodInvoker(() =>
            {
                if (e.ListChangedType == ListChangedType.ItemAdded && ((ClientInfoViewModelListItem)clientListBox.SelectedItem).IsSelfClient)
                {
                    // Only need to call this when it's an Add, works fine when it's an update. Adding doesn't refresh for some reason.
                    ((CurrencyManager)clientListBox.BindingContext[_ftpServerService._myIncommingTransfers]).Refresh();
                }
            }));
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

        public void DrawSelectedClientTransfers()
        {
            if(clientListBox.SelectedItem != null)
            {
                ClientInfoViewModelListItem clientObjectSelected = (ClientInfoViewModelListItem)clientListBox.SelectedItem;
                if(clientObjectSelected.IsSelfClient)
                {
                    transfersListBox.DataSource = _ftpServerService._myIncommingTransfers;
                    selectedIpLabel.Text = "You: " + clientObjectSelected.IpAddress + " - Receiving Files";
                    sendFileButton.Visible = false;
                    dragAndDropPanel.Visible = false;
                    transfersPanel.Size = new System.Drawing.Size(transfersPanel.Size.Width, this.Size.Height - 89);
                }
                else
                {
                    transfersListBox.DataSource = _selectedClientSendTransferList;
                    selectedIpLabel.Text = clientObjectSelected.IpAddress;
                    AddSelectedClientTransfersToBindingList(clientObjectSelected.IpAddress);
                    sendFileButton.Visible = true;
                    dragAndDropPanel.Visible = true;
                    transfersPanel.Size = new System.Drawing.Size(transfersPanel.Size.Width, this.Size.Height - 118);
                }
            }
            else
            {
                _selectedClientSendTransferList.Clear();
            }
        }

        private void AddSelectedClientTransfersToBindingList(string destinationIP)
        {
            _selectedClientSendTransferList.Clear();
            foreach (FileTransferStatus clientTransfers in _sendFileService.GetTransfersByDestIP(destinationIP))
            {
                _selectedClientSendTransferList.Add(clientTransfers);
            }
        }

        private void ClientListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            DrawSelectedClientTransfers();
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
            long fileSizeBytes = new FileInfo(fileName).Length;
            _selectedClientSendTransferList.Add(_sendFileService.StartNewTransferThread(selectedIpLabel.Text, fileName, fileSizeBytes));
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
