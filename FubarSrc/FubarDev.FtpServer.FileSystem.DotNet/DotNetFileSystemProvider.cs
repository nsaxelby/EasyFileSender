//-----------------------------------------------------------------------
// <copyright file="DotNetFileSystemProvider.cs" company="Fubar Development Junker">
//     Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>
// <author>Mark Junker</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using EFS.Shared.EventModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FubarDev.FtpServer.FileSystem.DotNet
{
    /// <summary>
    /// A <see cref="IFileSystemClassFactory"/> implementation that uses
    /// the standard .NET functionality to provide file system access.
    /// </summary>
    public class DotNetFileSystemProvider : IFileSystemClassFactory
    {
        private readonly IAccountDirectoryQuery _accountDirectoryQuery;
        private readonly ILogger<DotNetFileSystemProvider>? _logger;
        private readonly string _rootPath;
        private readonly int _streamBufferSize;
        private readonly bool _allowNonEmptyDirectoryDelete;
        private readonly bool _flushAfterWrite;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetFileSystemProvider"/> class.
        /// </summary>
        /// <param name="options">The file system options.</param>
        /// <param name="accountDirectoryQuery">Interface to query account directories.</param>
        /// <param name="logger">The logger.</param>
        public DotNetFileSystemProvider(
            IOptions<DotNetFileSystemOptions> options,
            IAccountDirectoryQuery accountDirectoryQuery,
            ILogger<DotNetFileSystemProvider>? logger = null)
        {
            _accountDirectoryQuery = accountDirectoryQuery;
            _logger = logger;
            _rootPath = string.IsNullOrEmpty(options.Value.RootPath)
                ? Path.GetTempPath()
                : options.Value.RootPath!;
            _streamBufferSize = options.Value.StreamBufferSize ?? DotNetFileSystem.DefaultStreamBufferSize;
            _allowNonEmptyDirectoryDelete = options.Value.AllowNonEmptyDirectoryDelete;
            _flushAfterWrite = options.Value.FlushAfterWrite;
        }

        public event EventHandler<IncomingFileTransferStatus> FileDataReceived;

        protected virtual void OnFileDataReceived(IncomingFileTransferStatus e)
        {
            EventHandler<IncomingFileTransferStatus> handler = FileDataReceived;
            handler?.Invoke(this, e);
        }

        /// <inheritdoc/>
        public Task<IUnixFileSystem> Create(IAccountInformation accountInformation)
        {
            var path = _rootPath;
            var directories = _accountDirectoryQuery.GetDirectories(accountInformation);
            if (!string.IsNullOrEmpty(directories.RootPath))
            {
                path = Path.Combine(path, directories.RootPath);
            }

            _logger?.LogDebug("The root directory for {userName} is {rootPath}", accountInformation.FtpUser.Identity.Name, path);
            var dotNetFS = new DotNetFileSystem(path, _allowNonEmptyDirectoryDelete, _streamBufferSize, _flushAfterWrite);
            dotNetFS.FileDataReceived += DotNetFS_FileDataReceived;
            return Task.FromResult<IUnixFileSystem>(dotNetFS);
        }

        private void DotNetFS_FileDataReceived(object sender, IncomingFileTransferStatus e)
        {
            OnFileDataReceived(e);
        }
    }
}
