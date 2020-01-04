using EFS.Global.Enums;
using EFS.Global.Models;
using static EFS.Utilities.Discovery.DiscoveryListener;

namespace EFS.Utilities.Discovery
{
    public class DiscoveryService
    {
        private DiscoveryAdvertiser _discoveryAdvertiser;
        private DiscoveryListener _discoveryListener;

        public DiscoveryService(string myIpAddress, int port, OnRecievedClientData delMethod, int delayMs = 5000)
        {
            ClientInfo ci = new ClientInfo()
            {
                ClientType = ClientTypeEnum.windows.ToString(),
                IpAddress = myIpAddress,
                Version = VersionNumberEnum.v1.ToString()
            };

            string broadcastAddress = DiscoveryUtils.GetBroadcastAddress(myIpAddress);
            _discoveryAdvertiser = new DiscoveryAdvertiser(myIpAddress, broadcastAddress, port, ci, delayMs);
            _discoveryListener = new DiscoveryListener(myIpAddress, port, delMethod);
        }

        public bool StartDiscoveryService()
        {
            _discoveryAdvertiser.StartDiscoveryPolling();
            _discoveryListener.StartListener();
            return true;
        }

        public bool StopDiscoveryService()
        {
            _discoveryAdvertiser.StopDiscoveryPolling();
            _discoveryListener.StopListener();
            return true;
        }
    }
}
