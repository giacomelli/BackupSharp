using System.Diagnostics;

namespace BackupSharp
{
    /// <summary>
    /// Arguments for event ItemStored.
    /// </summary>
    [DebuggerDisplay("{Item.DestinationFullName} stored")]
    public class BackupItemStoredEventArgs : BackupItemEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackupItemStoredEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public BackupItemStoredEventArgs(IBackupItem item)
            : base(item)
        {
        }
    }
}