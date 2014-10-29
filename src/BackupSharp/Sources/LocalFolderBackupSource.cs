using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BackupSharp.Sources
{
    /// <summary>
    /// A local folder backup source.
    /// </summary>
    [DebuggerDisplay("LocalFolder: {Id}")]
    public class LocalFolderBackupSource : BackupSourceBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFolderBackupSource"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="folderPath">The folder path.</param>
        public LocalFolderBackupSource(string id, string folderPath)
            : base(id, PathHelper.GetFullPath(folderPath))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFolderBackupSource"/> class.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        public LocalFolderBackupSource(string folderPath)
            : this(Guid.NewGuid().ToString(), folderPath)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <returns>
        /// The available items on source.
        /// </returns>
        public override IEnumerable<IBackupItem> GetItems()
        {
            return GetItems(RootPath);
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
                return File.ReadAllBytes(item.SourceFullName);
            }

            return new byte[0];
        }
        #endregion

        #region Private methods
        private IEnumerable<IBackupItem> GetItems(string folder)
        {
            var files = Directory.GetFiles(folder);

            foreach (var f in files)
            {
                var item = new BackupItem(f, BackupItemKind.File);
                OnItemFound(new BackupItemFoundEventArgs(item));

                yield return item;
            }

            var subFolders = Directory.GetDirectories(folder);

            foreach (var subFolder in subFolders)
            {
                var item = new BackupItem(subFolder, BackupItemKind.Folder);
                OnItemFound(new BackupItemFoundEventArgs(item));

                yield return item;

                foreach (var s in GetItems(subFolder))
                {
                    yield return s;
                }
            }
        }
        #endregion
    }
}
