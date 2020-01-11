using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.WindowsAPICodePack.Shell;

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

        public static string GetMyDownloadsFolder()
        {
            return KnownFolders.Downloads.Path;
        }
    }
}
