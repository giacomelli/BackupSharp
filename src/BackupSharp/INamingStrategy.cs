namespace BackupSharp
{
    /// <summary>
    /// Defines an interface naming strategy.
    /// </summary>
    public interface INamingStrategy
    {
        /// <summary>
        /// Gets the root path.
        /// </summary>
        string RootPath { get; }

        /// <summary>
        /// Initializes the naming strategy.
        /// </summary>
        /// <param name="context">The backup context.</param>
        void Initialize(BackupContext context);

        /// <summary>
        /// Gets the full name to backup item.
        /// </summary>
        /// <param name="item">The backup item.</param>
        /// <returns>The backup item full name.</returns>
        string GetFullName(IBackupItem item);
    }
}
