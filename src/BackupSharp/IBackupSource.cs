using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BackupSharp
{
    /// <summary>
    /// Defines an interface to a backup source.
    /// </summary>
    public interface IBackupSource : IBackupStep
    {
        /// <summary>
        /// Occurs when a item is found.
        /// </summary>
        event EventHandler<BackupItemFoundEventArgs> ItemFound;

        /// <summary>
        /// Gets the root path.
        /// </summary>
        string RootPath { get; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <returns>The available items on source.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "The method is appropriate in this case.")]
        IEnumerable<IBackupItem> GetItems();

        /// <summary>
        /// Reads the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The item byte array.</returns>
        byte[] ReadItem(IBackupItem item);

        /// <summary>
        /// Gets relative path of backup item on backup source.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The relative path of backup item on backup source.</returns>
        string GetRelativePath(IBackupItem item);
    }
}
