using EFS.Global.Exceptions;
using EFS.Global.Models;
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
        private bool _stopTransfer = false;

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

        private void DoTransfer()
        {
            try
            {
                string destinationFileURI = "ftp://" + _destinationIP + "/" + Path.GetFileName(_sourceFile);

                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(destinationFileURI);
                request.Credentials = new NetworkCredential("anonymous", "anon@anon.com");
                request.Method = WebRequestMethods.Ftp.GetFileSize;

                bool fileAlreadyExists = false;

                try
                {
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    fileAlreadyExists = true;
                }
                catch (WebException ex)
                {
                    FtpWebResponse response = (FtpWebResponse)ex.Response;
                    if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        //Does not exist
                        fileAlreadyExists = false;
                    }
                }

                if (fileAlreadyExists)
                {
                    throw new FileAlreadyExistsException("File : " + Path.GetFileName(_sourceFile) + " already exists on destination.");
                }


                request = (FtpWebRequest)WebRequest.Create(destinationFileURI);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential("anonymous", "anon@anon.com");

                var buffer = new byte[1024 * 1024];
                int readBytesCount;
                TransferStatus.DateTimeStarted = DateTime.UtcNow;
                using (FileStream sourceStream = File.OpenRead(_sourceFile))
                using (Stream ftpStream = request.GetRequestStream())
                {
                    TransferStatus.FileSizeBytes = sourceStream.Length;
                    TransferStatus.TransferredSizeBytes = 0;
                    while (((readBytesCount = sourceStream.Read(buffer, 0, buffer.Length)) > 0) && _stopTransfer == false)
                    {
                        ftpStream.Write(buffer, 0, readBytesCount);
                        TransferStatus.TransferredSizeBytes += readBytesCount;                        
                        _statusUpdateDelegateMethod(TransferStatus);
                    }
                }
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
            _stopTransfer = true;
            if (_transferThread.IsAlive)
            {
                _transferThread.Join();
            }
            return true;
        }
    }
}
