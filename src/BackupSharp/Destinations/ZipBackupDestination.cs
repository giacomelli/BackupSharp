using System.Diagnostics;
using System.IO;
using HelperSharp;
using Ionic.Zip;

namespace BackupSharp.Destinations
{
    /// <summary>
    /// A Zip backup destination.
    /// </summary>
    [DebuggerDisplay("Zip: {Id}")]
    public class ZipBackupDestination : BackupDestinationBase
    {
        #region Fields
        private string m_folderPath;
        private ZipFile m_zipFile;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ZipBackupDestination"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="folderPath">The folder path to save zip file.</param>
        public ZipBackupDestination(string id, string folderPath)
            : base(id)
        {
            ExceptionHelper.ThrowIfNullOrEmpty("folderPath", folderPath);

            m_folderPath = PathHelper.GetFullPath(folderPath);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipBackupDestination"/> class.
        /// </summary>
        /// <param name="folderPath">The folder path to save zip file.</param>
        public ZipBackupDestination(string folderPath)
            : this(Path.GetDirectoryName(folderPath), folderPath)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the zip file.
        /// </summary>
        public string ZipFileName { get; private set; }
        #endregion

        #region implemented abstract members of BackupDestinationBase
        /// <summary>
        /// Initializes the destination step.
        /// </summary>
        /// <param name="context">The backup context.</param>
        public override void Initialize(BackupContext context)
        {
            base.Initialize(context);

            ZipFileName = Path.ChangeExtension(Path.Combine(m_folderPath, context.Naming.RootPath), ".zip");
            m_zipFile = new ZipFile();
        }

        /// <summary>
        /// Terminates the destination step.
        /// </summary>
        /// <param name="context">The backup context.</param>
        public override void Terminate(BackupContext context)
        {
            base.Terminate(context);

            PathHelper.EnsureFolderExists(ZipFileName);
            m_zipFile.Save(ZipFileName);
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
                m_zipFile.AddEntry(GetEntryName(item), data);
            }
            else
            {
                m_zipFile.AddDirectoryByName(GetEntryName(item));
            }

            OnItemStored(new BackupItemStoredEventArgs(item));
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_zipFile != null)
                {
                    m_zipFile.Dispose();
                }
            }
        }

        private string GetEntryName(IBackupItem item)
        {
            return Context.Source.GetRelativePath(item);
        }
        #endregion
    }
}
