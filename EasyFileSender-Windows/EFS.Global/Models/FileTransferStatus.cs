using System;

namespace EFS.Global.Models
{
    public class FileTransferStatus
    {
        public Guid TransferID { get; set; }
        public String DestinationIP { get; set; }
        public string SourceFile { get; set; }
        public double Progress { get; set; }
        public long FileSizeBytes { get; set; }
        public long TransferredSizeBytes { get; set; }
        public bool Complete { get; set; }
        public bool Successful { get; set; }
        public Exception Exception { get; set; }
    }
}
