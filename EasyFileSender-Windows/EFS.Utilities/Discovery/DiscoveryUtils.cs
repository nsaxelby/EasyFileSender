using EFS.Global.Exceptions;
using EFS.Global.Models;
using System.Linq;
using System.Net;

namespace EFS.Utilities.Discovery
{
    public static class DiscoveryUtils
    { 
        /// <summary>
        /// Replaces the last byte ( host part ) of the input IP with 255
        /// </summary>
        /// <param name="ipv4Address"></param>
        /// <returns></returns>
        public static string GetBroadcastAddress(string ipv4Address)
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
