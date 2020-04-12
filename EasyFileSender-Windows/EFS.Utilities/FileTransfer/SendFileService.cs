using EFS.Shared.EventModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFS.Utilities.FileTransfer
{
    public class SendFileService
    {        
        private readonly List<FileSendTransferThread> _transferThreads = new List<FileSendTransferThread>();
        public SendFileService()
        {

        }

        public FileTransferStatus StartNewTransferThread(string destinationIP, string sourceFile, long fileSizeBytes)
        {
            FileSendTransferThread fileSendTransferThread = new FileSendTransferThread(Guid.NewGuid(), destinationIP, sourceFile, fileSizeBytes);
            fileSendTransferThread.StartTransfer();
            _transferThreads.Add(fileSendTransferThread);
            return fileSendTransferThread.TransferStatus;
        }

        // Blocking Thread
        public bool StopAllThreads()
        {
            foreach(FileSendTransferThread transferThreads in _transferThreads)
            {
                transferThreads.StopTransfer();
            }
            return true;
        }

        public void CancelTransfer(FileTransferStatus fileTransferStatus)
        {
            var toCancel = _transferThreads.SingleOrDefault(a => a.TransferStatus.TransferID == fileTransferStatus.TransferID);
            if(toCancel != null)
            {
                toCancel.StopTransfer();
            }
        }

        public List<SendFileTransferStatus> GetTransfersByDestIP(string ipAddress)
        {
            return _transferThreads.Where(a => a.TransferStatus.DestinationIP == ipAddress).Select(a => a.TransferStatus).ToList();
        }
    }
}
