using EFS.Global.Exceptions;
using EFS.Global.Models;
using FluentFTP;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace EFS.Utilities.FileTransfer
{
    public class FileSendTransferThread
    {
        private readonly OnFileTransferStatusChanged _statusUpdateDelegateMethod;
        private readonly string _destinationIP;
        private readonly string _sourceFile;
        public FileTransferStatus TransferStatus { get; set; }

        private Thread _transferThread;

        public delegate void OnFileTransferStatusChanged(FileTransferStatus fileTransferStatus);

        public FileSendTransferThread(Guid transferID, string destinationIP, string sourceFile, long fileSizeBytes, OnFileTransferStatusChanged onFileTransferStatusChangedDelegate)
        {
            _destinationIP = destinationIP;
            _sourceFile = sourceFile;
            _statusUpdateDelegateMethod = onFileTransferStatusChangedDelegate;
            TransferStatus = new FileTransferStatus()
            {
                TransferID = transferID,
                DestinationIP = destinationIP,
                SourceFile = sourceFile,
                Complete = false,
                FileSizeBytes = fileSizeBytes,
                TransferredSizeBytes = 0,
                Exception = null,
                Successful = false
            };
        }

        // Must be a non blocking call to release the UI [#13]
        public bool StartTransfer()
        {
            _transferThread = new Thread(new ThreadStart(DoTransfer));
            _transferThread.Start();
            return true;
        }

        private void FtpProgressAction(FtpProgress progress)
        {
            // Only reports pecentage, so up to us to work out file transfer bytes
            TransferStatus.TransferredSizeBytes = Convert.ToInt64((TransferStatus.FileSizeBytes / 100) * progress.Progress);
            TransferStatus.SpeedBytesPerSecond = progress.TransferSpeed;
            _statusUpdateDelegateMethod(TransferStatus);
        }

        private void DoTransfer()
        {
            try
            {
                FtpClient client = new FtpClient(_destinationIP);
                client.Credentials = new NetworkCredential("anonymous", "anon@anon.com");
                client.Connect();

                if (client.FileExists(Path.GetFileName(_sourceFile)))
                {
                    throw new FileAlreadyExistsException("File : " + Path.GetFileName(_sourceFile) + " already exists on destination.");
                }

                TransferStatus.DateTimeStarted = DateTime.Now;
                using(FileStream sourceStream = File.OpenRead(_sourceFile))
                {
                    client.Upload(sourceStream, Path.GetFileName(_sourceFile), FtpRemoteExists.Skip, false, FtpProgressAction);
                    TransferStatus.TransferredSizeBytes = TransferStatus.FileSizeBytes;
                    // Update the overall throughput of the transfer based on time
                    TransferStatus.SpeedBytesPerSecond = GetBytesPerSecondFromDateTime(TransferStatus.DateTimeStarted, TransferStatus.FileSizeBytes);
                    TransferStatus.TransferredSizeBytes = TransferStatus.FileSizeBytes;
                }

                client.Disconnect();

                TransferStatus.Complete = true;
                TransferStatus.Successful = true;
            }
            catch (Exception ex)
            {
                TransferStatus.Complete = true;
                TransferStatus.Successful = false;
                TransferStatus.Exception = ex;
            }
        }

        public bool StopTransfer()
        {
            if (_transferThread.IsAlive)
            {
                _transferThread.Join();
            }
            return true;
        }

        private static double GetBytesPerSecondFromDateTime(DateTime dateTimeStarted, long totalBytes)
        {
            TimeSpan timeElapsed = DateTime.Now - dateTimeStarted;
            return totalBytes / timeElapsed.TotalSeconds;
        }
    }
}
