using EFS.Global.Exceptions;
using EFS.Global.Models;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EFS.Utilities
{
    public static class DiscoveryTools
    { 
        public static void SendDiscoveryPacket(string ipv4Address, int port)
        {
            using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                // Parsing ensures IP is valid
                IPAddress serverAddr = IPAddress.Parse(ipv4Address);

                string broadcastIpStr = ObtainBroadcastAddress(serverAddr.ToString());

                IPAddress broadcastAdd = IPAddress.Parse(broadcastIpStr);

                IPEndPoint endPoint = new IPEndPoint(broadcastAdd, port);

                ClientInfo myClient = new ClientInfo() { ClientType = "windows", IpAddress = ipv4Address, Version = "v1" };

                string text = GetDiscoveryPacketStr(myClient);
                byte[] send_buffer = Encoding.ASCII.GetBytes(text);

                sock.SendTo(send_buffer, endPoint);
            }
        }

        /// <summary>
        /// Replaces the last byte ( host part ) of the input IP with 255
        /// </summary>
        /// <param name="ipv4Address"></param>
        /// <returns></returns>
        public static string ObtainBroadcastAddress(string ipv4Address)
        {
            // Parsing ensures IP is valid
            IPAddress inputIP = IPAddress.Parse(ipv4Address);
            byte[] ipBytes = inputIP.GetAddressBytes();

            // Broadcast byte for now is hard coded to 255.
            // Other broadcast possibilities handled in future features
            ipBytes[3] = 255;

            IPAddress inputIPd = new IPAddress(ipBytes);
            return inputIPd.ToString();
        }

        public static string GetDiscoveryPacketStr(ClientInfo clientInfoIn)
        {
            return $"efsip {clientInfoIn.IpAddress} {clientInfoIn.ClientType} {clientInfoIn.Version}";
        }

        public static ClientInfo GetClientInfoFromString(string clientInfoString)
        {
            ClientInfo toReturn = new ClientInfo();

            string[] splitStr = clientInfoString.Split(' ');
            if(splitStr.Count() != 4)
            {
                throw new MalformedUDPBroadcastException("'" + clientInfoString + "' is not valid, doesn't contain four parts");
            }

            toReturn.IpAddress = splitStr[1];
            toReturn.ClientType = splitStr[2];
            toReturn.Version = splitStr[3];

            return toReturn;
        }
    }


}
