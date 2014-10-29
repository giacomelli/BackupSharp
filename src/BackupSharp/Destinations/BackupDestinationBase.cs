using System;
using System.Diagnostics;

namespace BackupSharp.Destinations
{
    /// <summary>
    /// A base class for backup destinations.
    /// </summary>
    public abstract class BackupDestinationBase : IBackupDestination
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BackupDestinationBase"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        protected BackupDestinationBase(string id)
        {
            Id = id;
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when an item is stored.
        /// </summary>
        public event EventHandler<BackupItemStoredEventArgs> ItemStored;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        protected BackupContext Context { get; private set; }
        #endregion

        #region IBackupSource implementation

        /// <summary>
        /// Initializes the destination step.
        /// </summary>
        /// <param name="context">The backup context.</param>
        public virtual void Initialize(BackupContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Stores the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="data">The data.</param>
        public abstract void StoreItem(IBackupItem item, byte[] data);

        /// <summary>
        /// Terminates the destination step.
        /// </summary>
        /// <param name="context">The backup context.</param>
        public virtual void Terminate(BackupContext context)
        {
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
        /// Raises the <see cref="E:ItemStored" /> event.
        /// </summary>
        /// <param name="args">The <see cref="BackupItemStoredEventArgs"/> instance containing the event data.</param>
        protected virtual void OnItemStored(BackupItemStoredEventArgs args)
        {
            if (ItemStored != null)
            {
                ItemStored(this, args);
            }
        }
        #endregion
    }
}
