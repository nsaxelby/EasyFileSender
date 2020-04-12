using System;
using System.IO;
using System.Net;
using System.Threading;
using EFS.Global.Exceptions;
using EFS.Shared.EventModels;
using FluentFTP;

namespace EFS.Utilities.FileTransfer
{
    public class FileSendTransferThread
    {
        private readonly string _destinationIP;
        private readonly string _sourceFile;
        public SendFileTransferStatus TransferStatus { get; set; }

        private Thread _transferThread;

        private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        public FileSendTransferThread(Guid transferID, string destinationIP, string sourceFile, long fileSizeBytes)
        {
            _destinationIP = destinationIP;
            _sourceFile = sourceFile;
            TransferStatus = new SendFileTransferStatus()
            {
                TransferID = transferID,
                DestinationIP = destinationIP,
                FileName = sourceFile,
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

                var progressDel = new Progress<FtpProgress>(progress =>
                {
                    TransferStatus.TransferredSizeBytes = Convert.ToInt64((TransferStatus.FileSizeBytes / 100) * progress.Progress);
                    TransferStatus.SpeedBytesPerSecond = progress.TransferSpeed;
                });

                TransferStatus.DateTimeStarted = DateTime.Now;
                using(FileStream sourceStream = File.OpenRead(_sourceFile))
                {
                    var x = client.UploadAsync(sourceStream, Path.GetFileName(_sourceFile), FtpRemoteExists.Skip, false, progressDel, _cancelTokenSource.Token).Result;
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
                if (_cancelTokenSource.IsCancellationRequested)
                {
                    TransferStatus.Complete = true;
                    TransferStatus.Successful = false;
                    TransferStatus.Cancelled = true;
                }
                else
                {
                    TransferStatus.Complete = true;
                    TransferStatus.Successful = false;
                    TransferStatus.Exception = ex;
                }
            }
        }

        public bool StopTransfer()
        {
            if(TransferStatus.Complete == false)
            {
                _cancelTokenSource.Cancel();
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
