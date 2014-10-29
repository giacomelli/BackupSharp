using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BackupSharp.Naming;
using HelperSharp;
using Skahal.Infrastructure.Framework.Commons;
using Skahal.Infrastructure.Framework.Logging;

namespace BackupSharp
{
    /// <summary>
    /// Represents a backup.
    /// <remarks>
    /// A backup is made of two steps: a source and a destination.
    /// </remarks>
    /// </summary>
    [DebuggerDisplay("{Source.Id} => {Destination.Id}")]
    public class Backup
    {
        #region Fields
        private static readonly Regex s_invalidIdRegex = new Regex("[^a-z0-9_-]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private BackupContext m_context;
        private BackupResult m_result;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Backup"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="naming">The naming strategy.</param>
        public Backup(string name, IBackupSource source, IBackupDestination destination, INamingStrategy naming = null)
        {
            ExceptionHelper.ThrowIfNull("source", source);
            ExceptionHelper.ThrowIfNull("destination", destination);

            if (string.IsNullOrWhiteSpace(name))
            {
                Name = "{0}2{1}".With(source.Id, destination.Id);
            }
            else
            {
                Name = name;
            }

            Source = source;
            Destination = destination;
            Naming = naming == null ? new DateNamingStrategy() : naming;
            m_context = new BackupContext(source, destination, Naming);
            Naming.Initialize(m_context);

            MaxItemRetries = 100;
            MaxThreads = 20;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Backup"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="namingStrategy">The naming strategy.</param>
        public Backup(IBackupSource source, IBackupDestination destination, INamingStrategy namingStrategy = null)
            : this(null, source, destination, namingStrategy)
        {
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a backup item is found.
        /// </summary>
        public event EventHandler<BackupItemFoundEventArgs> ItemFound;

        /// <summary>
        /// Occurs when a copy of a backup item start.
        /// </summary>
        public event EventHandler<BackupItemEventArgs> ItemCopying;

        /// <summary>
        /// Occurs when a backup item failed.
        /// </summary>
        public event EventHandler<BackupItemEventArgs> ItemFailed;

        /// <summary>
        /// Occurs when a copy of backup item end.
        /// </summary>
        public event EventHandler<BackupItemEventArgs> ItemCopied;

        /// <summary>
        /// Occurs when a backup item is ignored.
        /// </summary>
        public event EventHandler<BackupItemEventArgs> ItemIgnored;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

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
        /// The naming.
        /// </value>
        public INamingStrategy Naming { get; private set; }

        /// <summary>
        /// Gets or sets the ignore item pattern.
        /// </summary>
        /// <value>
        /// The ignore item pattern.
        /// </value>
        public Regex IgnoreItemPattern { get; set; }

        /// <summary>
        /// Gets or sets the maximum item retries.
        /// </summary>
        /// <value>
        /// The maximum item retries.
        /// </value>
        public int MaxItemRetries { get; set; }

        /// <summary>
        /// Gets or sets the maximum threads used to copy the backup items.
        /// </summary>
        /// <value>
        /// The maximum threads.
        /// </value>
        public int MaxThreads { get; set; }
        #endregion

        #region Public methods
        /// <summary>
        /// Sanitizes the identifier.
        /// </summary>        
        /// <param name="id">The identifier.</param>
        /// <returns>The sanitized identifier.</returns>
        public static string SanitizeId(string id)
        {
            if (s_invalidIdRegex.IsMatch(id))
            {
                return s_invalidIdRegex.Replace(id, "_");
            }

            return id;
        }

        /// <summary>
        /// Performs the backup.
        /// </summary>
        /// <returns>The backup result.</returns>
        public BackupResult Run()
        {
            m_result = new BackupResult();
            var sw = new Stopwatch();
            sw.Start();

            var src = m_context.Source;
            var dst = m_context.Destination;

            src.Initialize(m_context);
            dst.Initialize(m_context);

            src.ItemFound += (sender, e) =>
            {
                OnItemFound(e);

                e.Ignored = IgnoreItem(e.Item);

                if (e.Ignored)
                {
                    OnItemIgnored(e);
                }
            };

            var items = src.GetItems();

            var parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = MaxThreads > 0 ? MaxThreads : 20
            };

            Parallel.ForEach(
                items,
                parallelOptions,
                item =>
                {
                    var eventArgs = new BackupItemEventArgs(item);

                    Run(eventArgs, MaxItemRetries);
                });

            src.Terminate(m_context);
            dst.Terminate(m_context);

            sw.Stop();
            m_result.ElapsedTime = sw.Elapsed;

            return m_result;
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// Raises the <see cref="E:ItemFound" /> event.
        /// </summary>
        /// <param name="args">The <see cref="BackupItemFoundEventArgs"/> instance containing the event data.</param>
        protected virtual void OnItemFound(BackupItemFoundEventArgs args)
        {
            ItemFound.Raise(this, args);
        }

        /// <summary>
        /// Raises the <see cref="E:ItemCopying" /> event.
        /// </summary>
        /// <param name="args">The <see cref="BackupItemEventArgs"/> instance containing the event data.</param>
        protected virtual void OnItemCopying(BackupItemEventArgs args)
        {
            ItemCopying.Raise(this, args);
        }

        /// <summary>
        /// Raises the <see cref="E:ItemCopied" /> event.
        /// </summary>
        /// <param name="args">The <see cref="BackupItemEventArgs"/> instance containing the event data.</param>
        protected virtual void OnItemCopied(BackupItemEventArgs args)
        {
            ItemCopied.Raise(this, args);
        }

        /// <summary>
        /// Raises the <see cref="E:ItemFailed" /> event.
        /// </summary>
        /// <param name="args">The <see cref="BackupItemEventArgs"/> instance containing the event data.</param>
        protected virtual void OnItemFailed(BackupItemEventArgs args)
        {
            ItemFailed.Raise(this, args);
        }

        /// <summary>
        /// Raises the <see cref="E:ItemIgnored" /> event.
        /// </summary>
        /// <param name="args">The <see cref="BackupItemEventArgs"/> instance containing the event data.</param>
        protected virtual void OnItemIgnored(BackupItemEventArgs args)
        {
            m_result.RegisterIgnored(args.Item);
            ItemIgnored.Raise(this, args);
        }
        #endregion

        #region Private methods
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Everty exception will be registered on BackupResult.")]
        private void Run(BackupItemEventArgs eventArgs, int remainingRetries)
        {
            var item = eventArgs.Item;

            try
            {
                if (IgnoreItem(item))
                {
                    OnItemIgnored(eventArgs);
                }
                else
                {
                    OnItemCopying(eventArgs);

                    var data = ReadItem(item);
                    StoreItem(item, data);

                    OnItemCopied(eventArgs);
                    m_result.RegisterSuccess(item);
                }
            }
            catch (Exception ex)
            {
                OnItemFailed(eventArgs);
                m_result.RegisterFailed(item, ex);

                remainingRetries--;

                if (remainingRetries > 0)
                {
                    Run(eventArgs, remainingRetries);
                }
            }
        }

        private byte[] ReadItem(IBackupItem item)
        {
            try
            {
                return m_context.Source.ReadItem(item);
            }
            catch (Exception ex)
            {
                LogService.Error("Error reading item {0} from source: {1}", item.SourceFullName, ex.Message);
                throw;
            }
        }

        private void StoreItem(IBackupItem item, byte[] data)
        {
            try
            {
                ((BackupItem)item).DestinationFullName = Naming.GetFullName(item);

                m_context.Destination.StoreItem(item, data);
            }
            catch (Exception ex)
            {
                LogService.Error("Error storing item {0} to destination: {1}", item.SourceFullName, ex.Message);
                throw;
            }
        }

        private bool IgnoreItem(IBackupItem item)
        {
            var result = false;

            if (IgnoreItemPattern != null)
            {
                result = IgnoreItemPattern.IsMatch(item.SourceFullName);
            }

            return result;
        }
        #endregion
    }
}