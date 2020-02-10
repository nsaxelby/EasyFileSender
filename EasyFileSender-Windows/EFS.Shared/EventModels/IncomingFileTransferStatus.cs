namespace EFS.Shared.EventModels
{
    public class IncomingFileTransferStatus : FileTransferStatus
    {
        public string SourceIP { get; set; }
    }
}
