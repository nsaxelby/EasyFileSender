﻿using EFS.Global.Enums;
using EFS.Global.Models;
using EFS.Utilities;
using EFS.Utilities.Discovery;
using EFS.WindowsFormApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace EFS.WindowsFormApp
{
    public partial class EFSForm : Form
    {
        private string _myIpAddress;
        private DiscoveryService _discoveryService;
        private int _port = 3008;
        private BindingList<ClientInfoViewModelListItem> _clientList = new BindingList<ClientInfoViewModelListItem>();

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
    }
}
