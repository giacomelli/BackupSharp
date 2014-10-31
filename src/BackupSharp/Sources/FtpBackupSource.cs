using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.FtpClient;
using Skahal.Infrastructure.Framework.Logging;

namespace BackupSharp.Sources
{
    /// <summary>
    /// A FTP backup source.
    /// </summary>
    [DebuggerDisplay("FTP: {Id}")]
    public class FtpBackupSource : BackupSourceBase
    {
        #region Fields
        private FtpClient m_ftpClient;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FtpBackupSource"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="server">The FTP server.</param>
        /// <param name="userName">The FTP username.</param>
        /// <param name="password">The FTP password.</param>
        public FtpBackupSource(string id, string server, string userName, string password)
            : base(id, "/")
        {
            Server = server;
            UserName = userName;
            Password = password;
            Folder = "/";
            MaxTimeoutRetries = 5;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpBackupSource"/> class.
        /// </summary>
        /// <param name="server">The FTP server.</param>
        /// <param name="userName">The FTP username.</param>
        /// <param name="password">The FTP password.</param>
        public FtpBackupSource(string server, string userName, string password)
            : this(server, server, userName, password)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the FTP server.
        /// </summary>
        public string Server { get; private set; }

        /// <summary>
        /// Gets the FTP username.
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the FTP password.
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Gets or sets the folder.
        /// </summary>
        /// <value>
        /// The folder.
        /// </value>
        public string Folder { get; set; }

        /// <summary>
        /// Gets or sets the maximum timeout retries.
        /// </summary>
        /// <value>
        /// The maximum timeout retries.
        /// </value>
        public int MaxTimeoutRetries { get; set; }
        #endregion

        #region implemented abstract members of BackupSourceBase
        /// <summary>
        /// Initializes the step.
        /// </summary>
        /// <param name="context">The backup context.</param>
        public override void Initialize(BackupContext context)
        {
            RootPath = Folder;
            base.Initialize(context);

            m_ftpClient = new FtpClient();
            m_ftpClient.Host = Server;
            m_ftpClient.Credentials = new NetworkCredential(UserName, Password);
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <returns>
        /// The available items on source.
        /// </returns>
        public override IEnumerable<IBackupItem> GetItems()
        {
            var result = new List<IBackupItem>();

            // Recursively collect the items.
            CollectItems(Folder, result);

            return result;
        }

        /// <summary>
        /// Reads the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The item byte array.
        /// </returns>
        public override byte[] ReadItem(IBackupItem item)
        {
            if (item.Kind == BackupItemKind.File)
            {
                using (var ftpFileStream = m_ftpClient.OpenRead(item.SourceFullName))
                {
                    return FileHelper.ReadFully(ftpFileStream);
                }
            }

            return new byte[0];
        }

        /// <summary>
        /// Terminates the source step.
        /// </summary>
        /// <param name="context">The backup context.</param>
        public override void Terminate(BackupContext context)
        {
            base.Terminate(context);
            m_ftpClient.Disconnect();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_ftpClient != null)
                {
                    m_ftpClient.Dispose();
                }
            }
        }
        #endregion

        #region Private methos
        private void CollectItems(string ftpFolder, List<IBackupItem> items)
        {
            foreach (var f in m_ftpClient.GetListing(ftpFolder))
            {
                if (f.Type == FtpFileSystemObjectType.Directory)
                {
                    CollectItemFolder(f, ftpFolder, items);
                }
                else if (f.Type == FtpFileSystemObjectType.File)
                {
                    var remainingRetries = MaxTimeoutRetries;

                    do
                    {
                        try
                        {
                            var item = new BackupItem(f.FullName, BackupItemKind.File);

                            if (!IgnoreItemFound(item))
                            {
                                items.Add(item);
                            }

                            break;
                        }
                        catch (TimeoutException)
                        {
                            LogService.Warning("Timeout reading file {0}. Remaining retries: {1}", f.FullName, remainingRetries);
                            remainingRetries--;
                        }
                    }
                    while (remainingRetries > 0);
                }
            }
        }

        private void CollectItemFolder(FtpListItem folder, string ftpFolder, List<IBackupItem> items)
        {
            var ftpSubFolder = PathHelper.Combine(ftpFolder, folder.Name);
            var item = new BackupItem(ftpSubFolder, BackupItemKind.Folder);

            if (!IgnoreItemFound(item))
            {
                items.Add(new BackupItem(ftpSubFolder, BackupItemKind.Folder));
                CollectItems(ftpSubFolder, items);
            }
        }

        private bool IgnoreItemFound(IBackupItem item)
        {
            var args = new BackupItemFoundEventArgs(item);
            OnItemFound(args);

            return args.Ignored;
        }
        #endregion
    }
}
