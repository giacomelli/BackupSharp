namespace BackupSharp.Naming
{
    /// <summary>
    /// A base class for naming strategies.
    /// </summary>
    public abstract class NamingStrategyBase : INamingStrategy
    {
        #region Properties
        /// <summary>
        /// Gets or sets the root path.
        /// </summary>
        public string RootPath { get; protected set; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BackupContext Context { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the naming strategy.
        /// </summary>
        /// <param name="context">The backup context.</param>
        public virtual void Initialize(BackupContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Gets the full name to backup item.
        /// </summary>
        /// <param name="item">The backup item.</param>
        /// <returns>
        /// The backup item full name.
        /// </returns>
        public virtual string GetFullName(IBackupItem item)
        {
            return PathHelper.Combine(RootPath, Context.Source.GetRelativePath(item));
        }
        #endregion
    }
}
