using HelperSharp;

namespace BackupSharp.Naming
{
    /// <summary>
    /// An INamingStrategy that use a root folder with the format {1:yyyyMMdd-HHmm}.
    /// </summary>
    public class DateNamingStrategy : NamingStrategyBase
    {
        #region Methods
        /// <summary>
        /// Initializes the naming strategy.
        /// </summary>
        /// <param name="context">The backup context.</param>
        public override void Initialize(BackupContext context)
        {
            base.Initialize(context);

            RootPath = PathHelper.Combine(context.Source.Id, "{0:yyyyMMdd-HHmm}".With(context.Time));
        }
        #endregion
    }
}
