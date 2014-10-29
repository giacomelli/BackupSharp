using System;

namespace BackupSharp
{
    /// <summary>
    /// Defines an interface to a step of the backup process.
    /// </summary>
    public interface IBackupStep : IDisposable
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Initializes the step.
        /// </summary>
        /// <param name="context">The backup context.</param>
        void Initialize(BackupContext context);

        /// <summary>
        /// Terminates the step.
        /// </summary>
        /// <param name="context">The backup context.</param>
        void Terminate(BackupContext context);
    }
}
