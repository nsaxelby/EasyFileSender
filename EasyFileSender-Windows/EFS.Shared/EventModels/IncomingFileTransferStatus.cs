using FubarDev.FtpServer;

namespace EFS.Shared.EventModels
{
    public class IncomingFileTransferStatus : FileTransferStatus
    {
        public string SourceIP { get; set; }
        public IFtpDataConnection FtpDataConnection { get; set; }
    }
}
