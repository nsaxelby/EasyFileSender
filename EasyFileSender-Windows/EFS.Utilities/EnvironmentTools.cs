using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;

namespace EFS.Utilities
{
    public class IPAddressInfo
    {
        public string IpAddress { get; set; }
        public string AdapterDisplayLabel { get; set; }
    }

    public static class EnvironmentTools
    {
        /// <summary>
        /// Favors addresses starting with 192, so they appear first
        /// Only displays network adapters that are "Up". Wireless802 or Ethernet.
        /// </summary>
        /// <returns></returns>
        public static List<IPAddressInfo> GetIPV4Addresses()
        {
            List<IPAddressInfo> toReturnNonPriorIPs = new List<IPAddressInfo>();
            List<IPAddressInfo> toReturnPriorityList = new List<IPAddressInfo>();

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces().Where(a => a.OperationalStatus == OperationalStatus.Up))
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            if(ip.Address.ToString().StartsWith("192"))
                            {
                                toReturnPriorityList.Add(new IPAddressInfo() { IpAddress = ip.Address.ToString(), AdapterDisplayLabel = ni.Name + " - " + ip.Address.ToString() });
                            }
                            else
                            {
                                toReturnNonPriorIPs.Add(new IPAddressInfo() { IpAddress = ip.Address.ToString(), AdapterDisplayLabel = ni.Name + " - " + ip.Address.ToString() });
                            }
                        }
                    }
                }
            }
            toReturnPriorityList.AddRange(toReturnNonPriorIPs);
            return toReturnPriorityList;
        }

        // Possible TODO here, should we defalt to Downloads? Or should we keep it within the eexecuting directory for simplicity and separation
        public static string GetDownloadsFolder()
        {
            return Path.Combine(Environment.CurrentDirectory, "Downloads");
        }

        // Source: https://stackoverflow.com/questions/23010910/how-to-retrieve-a-jpg-image-using-getmanifestresourcestream-method
        public static Stream ExtractResourceFile(Assembly assembly, String fileName)
        {
            // get all embedded resource file names including namespace
            string[] fileNames = assembly.GetManifestResourceNames();

            string resourceName = null;
            string temp = "." + fileName.ToUpper();
            foreach (var item in fileNames)
            {
                if (item.ToUpper().EndsWith(temp))
                {
                    resourceName = item;
                }
            }
            if (resourceName == null)
            {
                throw new Exception("Embedded resource [" + fileName + "] not found");
            }

            // get stream
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new Exception("Embedded resource [" + resourceName + "] could not be opened.");
            }
            return stream;
        }
    }
}
