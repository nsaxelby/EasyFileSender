using EFS.Utilities;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EFS.WindowsFormApp
{
    public partial class SelectNetworkAdapterDialog : Form
    {
        public string SelectedIpAddress { get; set; }
        public SelectNetworkAdapterDialog(List<IPAddressInfo> ipAddressInfos)
        {
            InitializeComponent();

            networkAdapterListCombo.Items.Clear();
            networkAdapterListCombo.DataSource = ipAddressInfos;
            networkAdapterListCombo.ValueMember = "IpAddress";
            networkAdapterListCombo.DisplayMember = "AdapterDisplayLabel";
            networkAdapterListCombo.SelectedValueChanged += NetworkAdapterListCombo_SelectedValueChanged;
            if (ipAddressInfos.Count >= 1)
            {
                SelectedIpAddress = ipAddressInfos[0].IpAddress;
            }
        }

        private void NetworkAdapterListCombo_SelectedValueChanged(object sender, EventArgs e)
        {
            SelectedIpAddress = (string)((ComboBox)sender).SelectedValue;
        }
    }
}
