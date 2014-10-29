using System;
using System.Diagnostics;
using HelperSharp;

namespace BackupSharp
{
    /// <summary>
    /// Arguments to events based on BackupItem.
    /// </summary>
    [DebuggerDisplay("{Item.SourceFullName}")]
    public class BackupItemEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BackupItemEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public BackupItemEventArgs(IBackupItem item)
        {
            ExceptionHelper.ThrowIfNull("item", item);
            Item = item;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public IBackupItem Item { get; private set; }
        #endregion
    }
}
