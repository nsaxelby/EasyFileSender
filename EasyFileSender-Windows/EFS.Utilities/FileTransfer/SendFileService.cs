using System;
using System.Collections.Generic;
using static EFS.Utilities.FileTransfer.FileSendTransferThread;

namespace EFS.Utilities.FileTransfer
{
    public class SendFileService
    {
        private readonly List<FileSendTransferThread> _transferThreads = new List<FileSendTransferThread>();
        public SendFileService()
        {

        }

        public bool StartNewTransferThread(Guid transferID, string destinationIP, string sourceFile, long fileSizeBytes, OnFileTransferStatusChanged onFileTransferStatusChanged)
        {
            FileSendTransferThread fileSendTransferThread = new FileSendTransferThread(transferID, destinationIP, sourceFile, fileSizeBytes, onFileTransferStatusChanged);
            fileSendTransferThread.StartTransfer();
            _transferThreads.Add(fileSendTransferThread);
            return true;
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
    }
}
