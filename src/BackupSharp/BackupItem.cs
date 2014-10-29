using System.Diagnostics;
using HelperSharp;

namespace BackupSharp
{
    /// <summary>
    /// The possible kinds of a backup item.
    /// </summary>
    public enum BackupItemKind
    {
        /// <summary>
        /// A file.
        /// </summary>
        File,

        /// <summary>
        /// A folder.
        /// </summary>
        Folder
    }

    /// <summary>
    /// An item of the backup, like a file or a folder.
    /// </summary>
    [DebuggerDisplay("{SourceFullName}: {Kind}")]
    public class BackupItem : IBackupItem
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BackupItem"/> class.
        /// </summary>
        /// <param name="sourceFullName">The full name of the source.</param>
        /// <param name="kind">The kind.</param>
        public BackupItem(string sourceFullName, BackupItemKind kind)
        {
            ExceptionHelper.ThrowIfNull("sourceFullName", sourceFullName);

            SourceFullName = sourceFullName;
            Kind = kind;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the kind.
        /// </summary>
        /// <value>
        /// The kind.
        /// </value>
        public BackupItemKind Kind { get; private set; }

        /// <summary>
        /// Gets the full name of the source.
        /// </summary>
        /// <value>
        /// The full name of the source.
        /// </value>
        public string SourceFullName { get; private set; }

        /// <summary>
        /// Gets or sets the full name of the destination.
        /// </summary>
        /// <value>
        /// The full name of the destination.
        /// </value>
        public string DestinationFullName { get; set; }
        #endregion
    }
}