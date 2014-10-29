using System;
using System.Collections.Generic;

namespace BackupSharp
{
    /// <summary>
    /// The result of a backup.
    /// </summary>
    public class BackupResult
    {
        #region Fields
        private static object s_lock = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BackupResult"/> class.
        /// </summary>
        public BackupResult()
        {
            Files = new BackupItemStats(BackupItemKind.File);
            Folders = new BackupItemStats(BackupItemKind.Folder);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the files.
        /// </summary>
        public BackupItemStats Files { get; private set; }

        /// <summary>
        /// Gets the folders.
        /// </summary>
        public BackupItemStats Folders { get; private set; }

        /// <summary>
        /// Gets or sets the elapsed time.
        /// </summary>
        public TimeSpan ElapsedTime { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Registers a success backup item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void RegisterSuccess(IBackupItem item)
        {
            lock (s_lock)
            {
                GetStatsByKind(item.Kind).SuccessfulCount++;
            }
        }

        /// <summary>
        /// Registers a ignored backup item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void RegisterIgnored(IBackupItem item)
        {
            lock (s_lock)
            {
                GetStatsByKind(item.Kind).RegisterIgnored(item);
            }
        }

        /// <summary>
        /// Registers a failed backup item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="reason">The reason of the fail.</param>
        public void RegisterFailed(IBackupItem item, Exception reason)
        {
            lock (s_lock)
            {
                var stat = GetStatsByKind(item.Kind);
                stat.RegisterFailed(item, reason);
            }
        }

        private BackupItemStats GetStatsByKind(BackupItemKind kind)
        {
            return kind == BackupItemKind.File ? Files : Folders;
        }
        #endregion
    }
}
