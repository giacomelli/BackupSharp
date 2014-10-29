namespace BackupSharp
{
    /// <summary>
    /// Defines an interface to a backup item.
    /// </summary>
    public interface IBackupItem
    {
        #region Properties
        /// <summary>
        /// Gets the kind.
        /// </summary>
        BackupItemKind Kind { get; }

        /// <summary>
        /// Gets the full name of the source.
        /// </summary>
        string SourceFullName { get; }

        /// <summary>
        /// Gets the full name of the destination.
        /// </summary>
        string DestinationFullName { get; }
        #endregion
    }
}
