namespace BackupSharp.Naming
{
    /// <summary>
    /// An INamingStrategy that use the backup source id as root folder name.
    /// </summary>
    public class SourceIdNamingStrategy : NamingStrategyBase
    {
        #region Methods
        /// <summary>
        /// Initializes the naming strategy.
        /// </summary>
        /// <param name="context">The backup context.</param>
        public override void Initialize(BackupContext context)
        {
            base.Initialize(context);
            RootPath = context.Source.Id;
        }
        #endregion
    }
}
