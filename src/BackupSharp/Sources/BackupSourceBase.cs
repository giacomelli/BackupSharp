using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using HelperSharp;

namespace BackupSharp.Sources
{
    /// <summary>
    /// A base class to backup sources.
    /// </summary>
    public abstract class BackupSourceBase : IBackupSource
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BackupSourceBase"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="rootPath">The root path.</param>
        protected BackupSourceBase(string id, string rootPath)
        {
            ExceptionHelper.ThrowIfNullOrEmpty("id", id);
            ExceptionHelper.ThrowIfNullOrEmpty("rootPath", rootPath);

            Id = Backup.SanitizeId(id);
            RootPath = rootPath;
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a item is found.
        /// </summary>
        public event EventHandler<BackupItemFoundEventArgs> ItemFound;
        #endregion

        #region IBackupSource implementation
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the root path.
        /// </summary>
        public string RootPath { get; protected set; }

        /// <summary>
        /// Initializes the source step.
        /// </summary>
        /// <param name="context">The backup context.</param>
        public virtual void Initialize(BackupContext context)
        {
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <returns>
        /// The available items on source.
        /// </returns>
        public abstract IEnumerable<IBackupItem> GetItems();

        /// <summary>
        /// Reads the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The item byte array.
        /// </returns>
        public abstract byte[] ReadItem(IBackupItem item);

        /// <summary>
        /// Terminates the source step.
        /// </summary>
        /// <param name="context">The backup context.</param>
        public virtual void Terminate(BackupContext context)
        {
        }

        /// <summary>
        /// Gets relative path of backup item on backup source.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The relative path of backup item on backup source.
        /// </returns>
        public virtual string GetRelativePath(IBackupItem item)
        {
            return Regex.Replace(item.SourceFullName, "^(\\\\|/)*" + Regex.Escape(RootPath), string.Empty);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Raises the <see cref="E:ItemFound" /> event.
        /// </summary>
        /// <param name="args">The <see cref="BackupItemFoundEventArgs"/> instance containing the event data.</param>
        protected virtual void OnItemFound(BackupItemFoundEventArgs args)
        {
            if (ItemFound != null)
            {
                ItemFound(this, args);
            }
        }
        #endregion
    }
}
