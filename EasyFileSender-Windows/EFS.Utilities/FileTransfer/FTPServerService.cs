using FubarDev.FtpServer;
using FubarDev.FtpServer.FileSystem;
using FubarDev.FtpServer.FileSystem.DotNet;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace EFS.Utilities.FileTransfer
{
    public class FTPServerService
    {
        private readonly string _localDirectory;
        private readonly int _port;
        private readonly IFtpServerHost _ftpHost;

        public FTPServerService(string localDirectory, int port)
        {
            _localDirectory = localDirectory;
            _port = port;

            // Setup dependency injection
            var services = new ServiceCollection();

            services.Configure<DotNetFileSystemOptions>(opt => { 
                opt.RootPath = localDirectory;
            });

            // Add FTP server services
            // DotNetFileSystemProvider = Use the .NET file system functionality
            // AnonymousMembershipProvider = allow only anonymous logins
            services.AddFtpServer(builder => builder
                .UseDotNetFileSystem() // Use the .NET file system functionality
                .EnableAnonymousAuthentication()); // allow anonymous logins

            // Configure the FTP server
            // Listen on all addresses
            services.Configure<FtpServerOptions>(opt =>
            {
                opt.Port = _port;
            });

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();
            // Initialize the FTP server
            _ftpHost = serviceProvider.GetRequiredService<IFtpServerHost>();

            var provider = serviceProvider.GetRequiredService<IFileSystemClassFactory>();
            if(provider.GetType() == typeof(DotNetFileSystemProvider))
            {
                var pp = (DotNetFileSystemProvider)provider;
                pp.FileDataReceived += Pp_FileDataReceived;
            }
        }

        private void Pp_FileDataReceived(object sender, FileReceivedStatus e)
        {
            Debug.WriteLine("Percentage progress: " + e.Progress + " file : " + e.SourceFile);
        }

        public bool StartService()
        {
            _ftpHost.StartAsync();
            return true;
        }

        public bool StopService()
        {
            _ftpHost.StopAsync();
            return true;
        }
    }
}
