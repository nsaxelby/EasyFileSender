using EFS.Shared.EventModels;
using FubarDev.FtpServer.FileSystem;
using FubarDev.FtpServer.FileSystem.DotNet;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Linq;
using FubarDev.FtpServer;
using EFS.Global.Models;
using EFS.Utilities.Discovery;
using FubarDev.FtpServer.Features;
using System.Threading;

namespace EFS.Utilities.FileTransfer
{
    public class FTPServerService
    {
        private readonly int _port;
        private readonly IFtpServerHost _ftpHost;
        public BindingList<IncomingFileTransferStatus> _myIncommingTransfers = new BindingList<IncomingFileTransferStatus>();
        private ServiceProvider _servProvider;
        
        public FTPServerService(string localDirectory, int port, ClientInfo clientInfo)
        {
            _myIncommingTransfers = new BindingList<IncomingFileTransferStatus>();
            _myIncommingTransfers.AllowNew = true;
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

            // Feature #7 This allows the FTP to respons via SYST, and tacks onto the end "efsversion:" and has a ClientInfo string after that
            services.Configure<SystCommandOptions>(opt =>
            {
                opt.EasyFileSenderVersion = DiscoveryUtils.GetDiscoveryPacketStr(clientInfo);
            });

            // Build the service provider
            _servProvider = services.BuildServiceProvider();
            // Initialize the FTP server
            _ftpHost = _servProvider.GetRequiredService<IFtpServerHost>();

            var provider = _servProvider.GetRequiredService<IFileSystemClassFactory>();
            if(provider.GetType() == typeof(DotNetFileSystemProvider))
            {
                var pp = (DotNetFileSystemProvider)provider;
                pp.FileDataReceived += FileDataReceived;
            }
        }

        private void FileDataReceived(object sender, IncomingFileTransferStatus e)
        {
            IncomingFileTransferStatus record = _myIncommingTransfers.SingleOrDefault(a => a.TransferID == e.TransferID);
            if (record != default(IncomingFileTransferStatus))
            {
                // Update
                record.Complete = e.Complete;
                record.DateTimeStarted = e.DateTimeStarted;
                record.Exception = e.Exception;
                record.FileName = e.FileName;
                record.FileSizeBytes = e.FileSizeBytes;
                record.SourceIP = e.SourceIP;
                record.SpeedBytesPerSecond = e.SpeedBytesPerSecond;
                record.Successful = e.Successful;
                record.TransferID = e.TransferID;
                record.TransferredSizeBytes = e.TransferredSizeBytes;
            }
            else
            {
                _myIncommingTransfers.Add(e);
            }
        }

        public void CancelIncommingTransfer(IncomingFileTransferStatus toCancel)
        {
            IncomingFileTransferStatus record = _myIncommingTransfers.SingleOrDefault(a => a.TransferID == toCancel.TransferID);
            record.FtpDataConnection.CancelAsync();
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
