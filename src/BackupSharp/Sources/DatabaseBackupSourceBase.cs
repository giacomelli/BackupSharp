using System.Collections.Generic;
using HelperSharp;

namespace BackupSharp.Sources
{
    /// <summary>
    /// A base class for database backup sources.
    /// </summary>    
    public abstract class DatabaseBackupSourceBase : BackupSourceBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseBackupSourceBase"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        protected DatabaseBackupSourceBase(string connectionString)
            : this(connectionString, connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseBackupSourceBase"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="connectionString">The connection string.</param>
        protected DatabaseBackupSourceBase(string id, string connectionString)
            : base(id, "/")
        {
            ExceptionHelper.ThrowIfNullOrEmpty("connectionString", connectionString);

            ConnectionString = connectionString;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the connection string.
        /// </summary> 
        protected string ConnectionString { get; private set; }
        #endregion

        #region implemented abstract members of BackupSourceBase
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <returns>
        /// The available items on source.
        /// </returns>
        public override IEnumerable<IBackupItem> GetItems()
        {
            return new BackupItem[] { new BackupItem("/database.backup.sql", BackupItemKind.File) };
        }

        /// <summary>
        /// Reads the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The item byte array.
        /// </returns>
        public abstract override byte[] ReadItem(IBackupItem item);
        #endregion
    }
}