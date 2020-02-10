namespace EFS.Shared.EventModels
{
    public class SendFileTransferStatus : FileTransferStatus
    {
        public string DestinationIP { get; set; }
    }
}
