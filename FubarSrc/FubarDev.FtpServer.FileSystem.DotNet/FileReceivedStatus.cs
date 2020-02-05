// <copyright file="FileReceivedStatus.cs" company="Fubar Development Junker">
// Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>

using System;

namespace FubarDev.FtpServer.FileSystem.DotNet
{
    public class FileReceivedStatus
    {
        public Guid TransferID { get; set; }
        public string SourceIP { get; set; }
        public string SourceFile { get; set; }
        public double Progress { get { return TransferredSizeBytes * 100.0 / FileSizeBytes; } }
        public long FileSizeBytes { get; set; }
        public long TransferredSizeBytes { get; set; }
        public bool Complete { get; set; }
        public bool Successful { get; set; }
        public Exception Exception { get; set; }
        public DateTime DateTimeStarted { get; set; }
        public double SpeedBytesPerSecond { get; set; }
    }
}
