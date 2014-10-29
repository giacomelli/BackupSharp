using System;

namespace BackupSharp
{
    /// <summary>
    /// Defines an interface of a backup destination.
    /// </summary>
    public interface IBackupDestination : IBackupStep
    {
        /// <summary>
        /// Occurs when an item is stored.
        /// </summary>
        event EventHandler<BackupItemStoredEventArgs> ItemStored;

        /// <summary>
        /// Stores the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="data">The data.</param>
        void StoreItem(IBackupItem item, byte[] data);
    }
}
