using System;
using System.Collections.Generic;
using HelperSharp;

namespace BackupSharp
{
    /// <summary>
    /// Statistics about backup items.
    /// </summary>
    public class BackupItemStats
    {
        #region Fields
        private Dictionary<string, IList<Exception>> m_failedExceptions = new Dictionary<string, IList<Exception>>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BackupItemStats"/> class.
        /// </summary>
        /// <param name="kind">The backup item kind.</param>
        public BackupItemStats(BackupItemKind kind)
        {
            Kind = kind;
            Ignored = new List<string>();
            Failed = new List<string>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the kind.
        /// </summary>
        /// <value>
        /// The kind.
        /// </value>
        public BackupItemKind Kind { get; private set; }

        /// <summary>
        /// Gets or sets the successful count.
        /// </summary>
        /// <value>
        /// The successful count.
        /// </value>
        public int SuccessfulCount { get; set; }

        /// <summary>
        /// Gets the ignored backup items.
        /// </summary>
        /// <value>
        /// The ignored.
        /// </value>
        public IList<string> Ignored { get; private set; }

        /// <summary>
        /// Gets failed backup items.
        /// </summary>
        /// <value>
        /// The failed.
        /// </value>
        public IList<string> Failed { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Registers a failed backup item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="reason">The reason of the fail.</param>
        public void RegisterFailed(IBackupItem item, Exception reason)
        {
            var key = item.SourceFullName;

            if (!m_failedExceptions.ContainsKey(key))
            {
                Failed.Add(key);
                m_failedExceptions.Add(key, new List<Exception>());
            }

            m_failedExceptions[key].Add(reason);
        }

        /// <summary>
        /// Registers a ignored backup item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void RegisterIgnored(IBackupItem item)
        {
            var name = item.SourceFullName;

            if (!Ignored.Contains(name))
            {
                Ignored.Add(name);
            }
        }

        /// <summary>
        /// Gets the failed exceptions.
        /// </summary>
        /// <param name="itemFullName">The full name of the item.</param>
        /// <returns>The exceptions that cause the item failed.</returns>
        public IList<Exception> GetFailedExceptions(string itemFullName)
        {
            if (!m_failedExceptions.ContainsKey(itemFullName))
            {
                throw new ArgumentException("There is no failed exceptions register to item '{0}'.".With(itemFullName));
            }

            return m_failedExceptions[itemFullName];
        }
        #endregion
    }
}
