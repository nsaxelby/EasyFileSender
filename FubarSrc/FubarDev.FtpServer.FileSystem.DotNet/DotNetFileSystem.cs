//-----------------------------------------------------------------------
// <copyright file="DotNetFileSystem.cs" company="Fubar Development Junker">
//     Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>
// <author>Mark Junker</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EFS.Shared.EventModels;
using FubarDev.FtpServer.BackgroundTransfer;

namespace FubarDev.FtpServer.FileSystem.DotNet
{
    /// <summary>
    /// A <see cref="IUnixFileSystem"/> implementation that uses the
    /// standard .NET functionality to access the file system.
    /// </summary>
    public class DotNetFileSystem : IUnixFileSystem
    {
        /// <summary>
        /// The default buffer size for copying from one stream to another.
        /// </summary>
        public static readonly int DefaultStreamBufferSize = 4096;

        private readonly int _streamBufferSize;
        private readonly bool _flushStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetFileSystem"/> class.
        /// </summary>
        /// <param name="rootPath">The path to use as root.</param>
        /// <param name="allowNonEmptyDirectoryDelete">Defines whether the deletion of non-empty directories is allowed.</param>
        public DotNetFileSystem(string rootPath, bool allowNonEmptyDirectoryDelete)
            : this(rootPath, allowNonEmptyDirectoryDelete, DefaultStreamBufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetFileSystem"/> class.
        /// </summary>
        /// <param name="rootPath">The path to use as root.</param>
        /// <param name="allowNonEmptyDirectoryDelete">Defines whether the deletion of non-empty directories is allowed.</param>
        /// <param name="streamBufferSize">Buffer size to be used in async IO methods.</param>
        public DotNetFileSystem(string rootPath, bool allowNonEmptyDirectoryDelete, int streamBufferSize)
            : this(rootPath, allowNonEmptyDirectoryDelete, streamBufferSize, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetFileSystem"/> class.
        /// </summary>
        /// <param name="rootPath">The path to use as root.</param>
        /// <param name="allowNonEmptyDirectoryDelete">Defines whether the deletion of non-empty directories is allowed.</param>
        /// <param name="streamBufferSize">Buffer size to be used in async IO methods.</param>
        /// <param name="flushStream">Flush the stream after every write operation.</param>
        public DotNetFileSystem(string rootPath, bool allowNonEmptyDirectoryDelete, int streamBufferSize, bool flushStream)
        {
            FileSystemEntryComparer = StringComparer.OrdinalIgnoreCase;
            Root = new DotNetDirectoryEntry(Directory.CreateDirectory(rootPath), true, allowNonEmptyDirectoryDelete);
            SupportsNonEmptyDirectoryDelete = allowNonEmptyDirectoryDelete;
            _streamBufferSize = streamBufferSize;
            _flushStream = flushStream;
        }

        public event EventHandler<IncomingFileTransferStatus> FileDataReceived;
        protected virtual void OnFileDataReceived(IncomingFileTransferStatus e)
        {
            EventHandler<IncomingFileTransferStatus> handler = FileDataReceived;
            handler?.Invoke(this, e);
        }

        /// <inheritdoc/>
        public bool SupportsNonEmptyDirectoryDelete { get; }

        /// <inheritdoc/>
        public StringComparer FileSystemEntryComparer { get; }

        /// <inheritdoc/>
        public IUnixDirectoryEntry Root { get; }

        /// <inheritdoc/>
        public bool SupportsAppend => true;

        /// <inheritdoc/>
        public Task<IReadOnlyList<IUnixFileSystemEntry>> GetEntriesAsync(IUnixDirectoryEntry directoryEntry, CancellationToken cancellationToken)
        {
            var result = new List<IUnixFileSystemEntry>();
            var searchDirInfo = ((DotNetDirectoryEntry)directoryEntry).DirectoryInfo;
            foreach (var info in searchDirInfo.EnumerateFileSystemInfos())
            {
                if (info is DirectoryInfo dirInfo)
                {
                    result.Add(new DotNetDirectoryEntry(dirInfo, false, SupportsNonEmptyDirectoryDelete));
                }
                else
                {
                    if (info is FileInfo fileInfo)
                    {
                        result.Add(new DotNetFileEntry(fileInfo));
                    }
                }
            }
            return Task.FromResult<IReadOnlyList<IUnixFileSystemEntry>>(result);
        }

        /// <inheritdoc/>
        public Task<IUnixFileSystemEntry?> GetEntryByNameAsync(IUnixDirectoryEntry directoryEntry, string name, CancellationToken cancellationToken)
        {
            var searchDirInfo = ((DotNetDirectoryEntry)directoryEntry).Info;
            var fullPath = Path.Combine(searchDirInfo.FullName, name);
            IUnixFileSystemEntry? result;
            if (File.Exists(fullPath))
            {
                result = new DotNetFileEntry(new FileInfo(fullPath));
            }
            else if (Directory.Exists(fullPath))
            {
                result = new DotNetDirectoryEntry(new DirectoryInfo(fullPath), false, SupportsNonEmptyDirectoryDelete);
            }
            else
            {
                result = null;
            }

            return Task.FromResult(result);
        }

        /// <inheritdoc/>
        public Task<IUnixFileSystemEntry> MoveAsync(IUnixDirectoryEntry parent, IUnixFileSystemEntry source, IUnixDirectoryEntry target, string fileName, CancellationToken cancellationToken)
        {
            var targetEntry = (DotNetDirectoryEntry)target;
            var targetName = Path.Combine(targetEntry.Info.FullName, fileName);

            if (source is DotNetFileEntry sourceFileEntry)
            {
                sourceFileEntry.FileInfo.MoveTo(targetName);
                return Task.FromResult<IUnixFileSystemEntry>(new DotNetFileEntry(new FileInfo(targetName)));
            }

            var sourceDirEntry = (DotNetDirectoryEntry)source;
            sourceDirEntry.DirectoryInfo.MoveTo(targetName);
            return Task.FromResult<IUnixFileSystemEntry>(new DotNetDirectoryEntry(new DirectoryInfo(targetName), false, SupportsNonEmptyDirectoryDelete));
        }

        /// <inheritdoc/>
        public Task UnlinkAsync(IUnixFileSystemEntry entry, CancellationToken cancellationToken)
        {
            if (entry is DotNetDirectoryEntry dirEntry)
            {
                dirEntry.DirectoryInfo.Delete(SupportsNonEmptyDirectoryDelete);
            }
            else
            {
                var fileEntry = (DotNetFileEntry)entry;
                fileEntry.Info.Delete();
            }

            return Task.FromResult(0);
        }

        /// <inheritdoc/>
        public Task<IUnixDirectoryEntry> CreateDirectoryAsync(IUnixDirectoryEntry targetDirectory, string directoryName, CancellationToken cancellationToken)
        {
            var targetEntry = (DotNetDirectoryEntry)targetDirectory;
            var newDirInfo = targetEntry.DirectoryInfo.CreateSubdirectory(directoryName);
            return Task.FromResult<IUnixDirectoryEntry>(new DotNetDirectoryEntry(newDirInfo, false, SupportsNonEmptyDirectoryDelete));
        }

        /// <inheritdoc/>
        public Task<Stream> OpenReadAsync(IUnixFileEntry fileEntry, long startPosition, CancellationToken cancellationToken)
        {
            var fileInfo = ((DotNetFileEntry)fileEntry).FileInfo;
            var input = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            if (startPosition != 0)
            {
                input.Seek(startPosition, SeekOrigin.Begin);
            }

            return Task.FromResult<Stream>(input);
        }

        /// <inheritdoc/>
        public async Task<IBackgroundTransfer?> AppendAsync(IUnixFileEntry fileEntry, long? startPosition, Stream data, CancellationToken cancellationToken)
        {
            var fileInfo = ((DotNetFileEntry)fileEntry).FileInfo;
            using (var output = fileInfo.OpenWrite())
            {
                if (startPosition == null)
                {
                    startPosition = fileInfo.Length;
                }

                output.Seek(startPosition.Value, SeekOrigin.Begin);
                await data.CopyToAsync(output, _streamBufferSize, _flushStream, cancellationToken).ConfigureAwait(false);
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task<IBackgroundTransfer?> CreateAsync(IUnixDirectoryEntry targetDirectory, string fileName, Stream data, CancellationToken cancellationToken, long expectedFileSize, string remoteIPAddress, IFtpDataConnection ftpDataConnection)
        {
            IncomingFileTransferStatus frs = new IncomingFileTransferStatus()
            {
                Complete = false,
                DateTimeStarted = DateTime.Now,
                Exception = null,
                FileSizeBytes = expectedFileSize,
                TransferredSizeBytes = 0,
                FileName = fileName,
                SourceIP = remoteIPAddress,
                Successful = false,
                SpeedBytesPerSecond = 0.0,
                TransferID = Guid.NewGuid(),
                FtpDataConnection = ftpDataConnection,
            };

            try
            {
                var targetEntry = (DotNetDirectoryEntry)targetDirectory;
                var fileInfo = new FileInfo(Path.Combine(targetEntry.Info.FullName, fileName));
                Console.WriteLine("Transferring file start : " + fileName);
                int totalBytesRead = 0;

                using (var output = fileInfo.Create())
                {
                    byte[] buffer = new byte[_streamBufferSize];
                    DateTime datetimeLastSent = default;

                    int read;
                    try
                    {
                        while (ftpDataConnection.Cancelled == false && (read = data.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            totalBytesRead += read;
                            output.Write(buffer, 0, read);

                            // Don't send it every update, wait as to not overload the events
                            if (datetimeLastSent <= DateTime.Now.AddMilliseconds(-300))
                            {
                                frs.TransferredSizeBytes = totalBytesRead;
                                frs.SpeedBytesPerSecond = GetBytesPerSecondFromDateTime(frs.DateTimeStarted, totalBytesRead);
                                datetimeLastSent = DateTime.Now;
                                OnFileDataReceived(frs);
                            }
                        }
                    }
                    catch (Exception ex1)
                    {
                        if (ftpDataConnection.Cancelled)
                        {
                            // Purposely ignore
                        }
                        else
                        {
                            throw ex1;
                        }
                    }
                }
                frs.SpeedBytesPerSecond = GetBytesPerSecondFromDateTime(frs.DateTimeStarted, totalBytesRead);
                frs.TransferredSizeBytes = totalBytesRead;
                if (totalBytesRead != expectedFileSize || ftpDataConnection.Cancelled)
                {
                    // We count this as cancelled, as opposed to exceptioned. Realstically we don't know if it was either.
                    frs.Complete = true;
                    frs.Successful = false;
                    frs.Cancelled = true;

                    // Delete cancelled/incomplete file on cancel or incomplete file
                    fileInfo.Delete();
                }
                else
                {
                    frs.Complete = true;
                    frs.Successful = true;
                }

                // The final send to complete the transfer
                OnFileDataReceived(frs);
                return null;
            }
            catch (Exception exx)
            {
                frs.Complete = true;
                frs.Exception = exx;
                frs.Successful = false;
                OnFileDataReceived(frs);
                throw exx;
            }
        }

        /// <inheritdoc/>
        public async Task<IBackgroundTransfer?> ReplaceAsync(IUnixFileEntry fileEntry, Stream data, CancellationToken cancellationToken)
        {
            var fileInfo = ((DotNetFileEntry)fileEntry).FileInfo;
            Console.WriteLine("Replacing file start : " + fileInfo.Name);

            using (var output = fileInfo.OpenWrite())
            {
                await data.CopyToAsync(output, _streamBufferSize, _flushStream, cancellationToken).ConfigureAwait(false);
                output.SetLength(output.Position);
            }

            return null;
        }

        /// <summary>
        /// Sets the modify/access/create timestamp of a file system item.
        /// </summary>
        /// <param name="entry">The <see cref="IUnixFileSystemEntry"/> to change the timestamp for.</param>
        /// <param name="modify">The modification timestamp.</param>
        /// <param name="access">The access timestamp.</param>
        /// <param name="create">The creation timestamp.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The modified <see cref="IUnixFileSystemEntry"/>.</returns>
        public Task<IUnixFileSystemEntry> SetMacTimeAsync(IUnixFileSystemEntry entry, DateTimeOffset? modify, DateTimeOffset? access, DateTimeOffset? create, CancellationToken cancellationToken)
        {
            var item = ((DotNetFileSystemEntry)entry).Info;

            if (access != null)
            {
                item.LastAccessTimeUtc = access.Value.UtcDateTime;
            }

            if (modify != null)
            {
                item.LastWriteTimeUtc = modify.Value.UtcDateTime;
            }

            if (create != null)
            {
                item.CreationTimeUtc = create.Value.UtcDateTime;
            }

            if (entry is DotNetDirectoryEntry dirEntry)
            {
                return Task.FromResult<IUnixFileSystemEntry>(new DotNetDirectoryEntry((DirectoryInfo)item, dirEntry.IsRoot, SupportsNonEmptyDirectoryDelete));
            }

            return Task.FromResult<IUnixFileSystemEntry>(new DotNetFileEntry((FileInfo)item));
        }

        private static double GetBytesPerSecondFromDateTime(DateTime dateTimeStarted, long totalBytes)
        {
            TimeSpan timeElapsed = DateTime.Now - dateTimeStarted;
            return totalBytes / timeElapsed.TotalSeconds;
        }
    }
}
