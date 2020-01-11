using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace EFS.Utilities
{
    public static class EnvironmentTools
    {
        public static string GetMyIP4IpAddress()
        {
            IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostEntry.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            return ipAddress.ToString();
        }
        // Possible TODO here, should we defalt to Downloads? Or should we keep it within the eexecuting directory for simplicity and separation
        public static string GetDownloadsFolder()
        {
            return Path.Combine(Environment.CurrentDirectory, "Downloads");
        }
    }
}
