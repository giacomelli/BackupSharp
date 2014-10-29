using System.Diagnostics;

namespace BackupSharp
{
    /// <summary>
    /// Arguments to event ItemFound.
    /// </summary>
    [DebuggerDisplay("{Item.SourceFullName} found")]
    public class BackupItemFoundEventArgs : BackupItemEventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BackupItemFoundEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public BackupItemFoundEventArgs(IBackupItem item)
            : base(item)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BackupItemFoundEventArgs"/> is ignored.
        /// </summary>
        /// <value>
        ///   <c>true</c> if ignored; otherwise, <c>false</c>.
        /// </value>
        public bool Ignored { get; set; }
        #endregion
    }
}