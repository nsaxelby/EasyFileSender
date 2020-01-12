using System;

namespace EFS.Global.Models
{
    public class FileTransferStatus
    {
        public Guid TransferID { get; set; }
        public String DestinationIP { get; set; }
        public string SourceFile { get; set; }
        public double Progress { get { return TransferredSizeBytes * 100.0 / FileSizeBytes; } }
        public long FileSizeBytes { get; set; }
        public long TransferredSizeBytes { get; set; }
        public bool Complete { get; set; }
        public bool Successful { get; set; }
        public Exception Exception { get; set; }
        public DateTime DateTimeStarted { get; set; }
        public double SpeedBytesPerSecond
        {
            get
            {
                if (DateTimeStarted != null)
                {
                    TimeSpan timeElapsed = DateTime.UtcNow - DateTimeStarted;
                    return TransferredSizeBytes / timeElapsed.TotalSeconds;
                }
                else
                {
                    return 0.0;
                }
            }
        }
    }
}
