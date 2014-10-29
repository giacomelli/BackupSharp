using System.Diagnostics;
using System.IO;
using HelperSharp;

namespace BackupSharp.Destinations
{
    /// <summary>
    /// A local folder backup destination.
    /// </summary>
    [DebuggerDisplay("LocalFolder: {Id}")]
    public class LocalFolderBackupDestination : BackupDestinationBase
    {
        #region Fields
        private string m_folderPath;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFolderBackupDestination"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="folderPath">The folder path.</param>
        public LocalFolderBackupDestination(string id, string folderPath)
            : base(id)
        {
            ExceptionHelper.ThrowIfNullOrEmpty("folderPath", folderPath);

            m_folderPath = PathHelper.GetFullPath(folderPath);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFolderBackupDestination"/> class.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        public LocalFolderBackupDestination(string folderPath)
            : this(Path.GetDirectoryName(folderPath), folderPath)
        {
        }
        #endregion

        #region implemented abstract members of BackupDestinationBase
        /// <summary>
        /// Stores the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="data">The data.</param>
        public override void StoreItem(IBackupItem item, byte[] data)
        {
            var itemPath = PathHelper.Combine(m_folderPath, item.DestinationFullName);

            if (item.Kind == BackupItemKind.File)
            {
                var folderPath = Path.GetDirectoryName(itemPath);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                File.WriteAllBytes(itemPath, data);
            }
            else
            {
                Directory.CreateDirectory(itemPath);
            }

            OnItemStored(new BackupItemStoredEventArgs(item));
        }
        #endregion
    }
}