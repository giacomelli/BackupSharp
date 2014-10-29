using System;
using System.Diagnostics;
using System.IO;
using DropNet;
using DropNet.Exceptions;
using HelperSharp;

namespace BackupSharp.Destinations
{
    /// <summary>
    /// A Dropbox backup destination.
    /// </summary>
    [DebuggerDisplay("Dropbox: {Id}")]
    public class DropboxBackupDestination : BackupDestinationBase
    {
        #region Fields
        private DropNetClient m_client;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DropboxBackupDestination"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="apiSecret">The API secret.</param>
        /// <param name="accessToken">The access token.</param>
        public DropboxBackupDestination(string id, string apiKey, string apiSecret, string accessToken)
            : base(id)
        {
            ApiKey = apiKey;
            ApiSecret = apiSecret;
            AccessToken = accessToken;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DropboxBackupDestination"/> class.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="apiSecret">The API secret.</param>
        /// <param name="accessToken">The access token.</param>
        public DropboxBackupDestination(string apiKey, string apiSecret, string accessToken)
            : this("Dropbox", apiKey, apiSecret, accessToken)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the API key.
        /// </summary>    
        public string ApiKey { get; private set; }

        /// <summary>
        /// Gets the API secret.
        /// </summary>
        public string ApiSecret { get; private set; }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        public string AccessToken { get; private set; }
        #endregion

        #region IBackupDestination implementation
        /// <summary>
        /// Initializes the destination step.
        /// </summary>
        /// <param name="context">The backup context.</param>
        public override void Initialize(BackupContext context)
        {
            // DropBox setup.
            m_client = new DropNetClient(ApiKey, ApiSecret, AccessToken);
            m_client.UseSandbox = true;
        }

        /// <summary>
        /// Stores the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="data">The data.</param>
        public override void StoreItem(IBackupItem item, byte[] data)
        {
            if (item.Kind == BackupItemKind.File)
            {
                var folder = Path.GetDirectoryName(item.DestinationFullName);
                var file = Path.GetFileName(item.DestinationFullName);

                CallClient((c) => c.UploadFile(NormalizePath(folder), NormalizePath(file), data));
                OnItemStored(new BackupItemStoredEventArgs(item));
            }
            else
            {
                CallClient((c) => c.CreateFolder(NormalizePath(item.DestinationFullName)));
                OnItemStored(new BackupItemStoredEventArgs(item));
            }
        }

        private static string NormalizePath(string path)
        {
            return path.Replace("\\", "/");
        }

        private void CallClient(Action<DropNetClient> call)
        {
            try
            {
                call(m_client);
            }
            catch (DropboxException ex)
            {
                if (!ex.Response.Content.Contains("folder already exists at path"))
                {
                    throw new InvalidOperationException("Error calling Dropbox client: {0}".With(ex.Response.Content));
                }
            }
        }
        #endregion
    }
}