using EFS.Global.Enums;
using EFS.Global.Models;
using static EFS.Utilities.Discovery.DiscoveryListener;

namespace EFS.Utilities.Discovery
{
    public class DiscoveryService
    {
        private DiscoveryAdvertiser _discoveryAdvertiser;
        private DiscoveryListener _discoveryListener;

        public DiscoveryService(int port, OnRecievedClientData delMethod, OnClientExpired delMethodClientExpired, ClientInfo clientInfo, int delayMs = 5000)
        {
            string broadcastAddress = DiscoveryUtils.GetBroadcastAddress(clientInfo.IpAddress);
            _discoveryAdvertiser = new DiscoveryAdvertiser(broadcastAddress, port, clientInfo, delayMs);
            _discoveryListener = new DiscoveryListener(clientInfo.IpAddress, port, delMethod, delMethodClientExpired);
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
