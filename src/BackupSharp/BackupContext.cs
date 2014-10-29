using System;
using BackupSharp.Naming;

namespace BackupSharp
{
    /// <summary>
    /// Represents a backup context.
    /// </summary>
    public class BackupContext
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BackupContext"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="naming">The naming strategy.</param>
        public BackupContext(IBackupSource source, IBackupDestination destination, INamingStrategy naming = null)
        {
            Source = source;
            Destination = destination;
            Naming = naming == null ? new DateNamingStrategy() : naming;

            Time = DateTime.UtcNow;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public IBackupSource Source { get; private set; }

        /// <summary>
        /// Gets the destination.
        /// </summary>
        /// <value>
        /// The destination.
        /// </value>
        public IBackupDestination Destination { get; private set; }

        /// <summary>
        /// Gets the naming strategy.
        /// </summary>
        /// <value>
        /// The naming strategy.
        /// </value>
        public INamingStrategy Naming { get; private set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public DateTime Time { get; set; }
        #endregion
    }
}