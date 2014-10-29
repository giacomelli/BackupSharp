using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Skahal.Infrastructure.Framework.Logging;

namespace BackupSharp
{
    /// <summary>
    /// The command-line runner.
    /// </summary>
    public class CommandLineRunner
    {
        #region Fields
        private CommandLineOptions m_options;
        #endregion

        #region Public methods
        /// <summary>
        /// Runs the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Run(params string[] args)
        {
            LogSeparator();
            Log("BackupSharp v.{0}", typeof(Backup).Assembly.GetName().Version);
            Log("by Diego Giacomelli (@g1acomell1)");
            LogSeparator();

            m_options = new CommandLineOptions();

            if (!CommandLine.Parser.Default.ParseArguments(args, m_options))
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            if (string.IsNullOrEmpty(m_options.File))
            {
                var source = TypeHelper.CreateSource(m_options.SourceName, m_options.SourceArgs);
                var destination = TypeHelper.CreateDestination(m_options.DestinationName, m_options.DestinationArgs);

                Run(new Backup(source, destination));
            }
            else
            {
                RunFromFile();
            }
        }
        #endregion        

        #region Log methods
        private static void ShowResult(BackupResult result)
        {
            LogInfo(@"Elapsed time: {0}", result.ElapsedTime.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            LogStats(result.Folders);
            LogStats(result.Files);

            LogSeparator();
        }

        private static void LogStats(BackupItemStats stats)
        {
            LogInfo(stats.Kind.ToString().ToUpperInvariant());
            LogInfo("Success: {0}", stats.SuccessfulCount);
            LogInfo("Ignored: {0}", stats.Ignored.Count);
            LogInfo("Failed: {0}", stats.Failed.Count);

            foreach (var failed in stats.Failed)
            {
                LogInfo("Failed: {0} - {1}", failed, string.Join("\n", stats.GetFailedExceptions(failed).Select(e => e.Message)));
            }
        }

        private static void LogSeparator()
        {
            LogInfo(string.Empty.PadRight(80, '-'));
        }

        private static void LogInfo(string msg, params object[] args)
        {
            LogService.Warning(msg, args);
        }

        private static void LogError(Exception ex)
        {
            LogService.Error("{0}: {1}", ex.Message, ex.StackTrace);
        }

        private static void LogError(string msg, params object[] args)
        {
            LogService.Error(msg, args);
        }

        private void Log(string msg, params object[] args)
        {
            if (m_options == null || m_options.Verbose)
            {
                LogService.Debug(msg, args);
            }
        }
        #endregion

        #region Run methods
        private void Run(Backup backup)
        {
            var sourceName = backup.Source.GetType().Name.Replace("BackupSource", string.Empty);
            var destinationName = backup.Destination.GetType().Name.Replace("BackupDestination", string.Empty);
            LogInfo("Running backup from {0} {1} to {2} {3}...", sourceName, backup.Source.Id, destinationName, backup.Destination.Id);
            LogInfo("Max threads: {0}.", backup.MaxThreads);
            LogInfo("Max item retries: {0}.", backup.MaxItemRetries);

            if (backup.IgnoreItemPattern != null)
            {
                LogInfo("Ignoring items with pattern: {0}.", backup.IgnoreItemPattern);
            }

            backup.ItemFound += (sender, e) => Log("FOUND: {0}", e.Item.SourceFullName);
            backup.ItemCopying += (sender, e) => Log(e.Item.SourceFullName);
            backup.ItemFailed += (sender, e) => LogError("FAILED: {0}", e.Item.SourceFullName);
            backup.ItemIgnored += (sender, e) => Log("IGNORED: {0}", e.Item.SourceFullName);

            var result = backup.Run();

            LogInfo("Backup from {0}(id={1}) to {2}(id={3}) finished.", sourceName, backup.Source.Id, destinationName, backup.Destination.Id);

            ShowResult(result);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Try to run all backups on file.")]
        private void RunFromFile()
        {
            LogInfo("Reading configuration file {0}...", m_options.File);
            var config = new BackupConfig();
            config.Load(m_options.File.Trim());

            LogInfo("Found {0} sources, {1} destinations and {2} backups on file.", config.Sources.Count, config.Destinations.Count, config.Backups.Count);
            var selectedBackups = config.Backups;

            if (!string.IsNullOrEmpty(m_options.BackupName))
            {
                selectedBackups = selectedBackups.Where(b => b.Name.Equals(m_options.BackupName, StringComparison.OrdinalIgnoreCase)).ToList();

                LogInfo("Selected {0} backups with name '{1}'.", selectedBackups.Count, m_options.BackupName);
            }

            if (!string.IsNullOrEmpty(m_options.SourceStartsWith))
            {
                selectedBackups = selectedBackups.Where(b => b.Source.Id.StartsWith(m_options.SourceStartsWith, StringComparison.OrdinalIgnoreCase)).ToList();

                LogInfo("Selected {0} backups that source ID starts with '{1}'.", selectedBackups.Count, m_options.SourceStartsWith);
            }

            if (!string.IsNullOrEmpty(m_options.SourceEndsWith))
            {
                selectedBackups = selectedBackups.Where(b => b.Source.Id.EndsWith(m_options.SourceEndsWith, StringComparison.OrdinalIgnoreCase)).ToList();

                LogInfo("Selected {0} backups that source ID ends with '{1}'.", selectedBackups.Count, m_options.SourceEndsWith);
            }

            foreach (var backup in selectedBackups)
            {
                try
                {
                    Run(backup);
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }
            }
        }
        #endregion
    }
}