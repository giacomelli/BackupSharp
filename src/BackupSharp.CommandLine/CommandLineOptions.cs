using CommandLine;
using CommandLine.Text;

namespace BackupSharp
{
    /// <summary>
    /// Command-line options.
    /// </summary>
    public class CommandLineOptions
    {
        #region Source
        /// <summary>
        /// Gets or sets the name of the source.
        /// </summary>
        /// <value>
        /// The name of the source.
        /// </value>
        [Option('s', "sourceName", MutuallyExclusiveSet = "DefaultConfig",
            HelpText = "The backup source class name")]
        public string SourceName { get; set; }

        /// <summary>
        /// Gets or sets the source arguments.
        /// </summary>
        /// <value>
        /// The source arguments.
        /// </value>
        [Option('r', "sourceArgs", MutuallyExclusiveSet = "DefaultConfig",
            HelpText = "The backup source class constructor args")]
        public string SourceArgs { get; set; }
        #endregion

        #region Destination
        /// <summary>
        /// Gets or sets the name of the destination.
        /// </summary>
        /// <value>
        /// The name of the destination.
        /// </value>
        [Option('d', "destinationName", MutuallyExclusiveSet = "DefaultConfig",
            HelpText = "The backup destination class name")]
        public string DestinationName { get; set; }

        /// <summary>
        /// Gets or sets the destination arguments.
        /// </summary>
        /// <value>
        /// The destination arguments.
        /// </value>
        [Option('t', "destinationArgs", MutuallyExclusiveSet = "DefaultConfig",
            HelpText = "The backup destination class constructor args")]
        public string DestinationArgs { get; set; }
        #endregion

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        [Option('f', "file", MutuallyExclusiveSet = "File",
            HelpText = "The backup configuration file")]
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the name of the backup.
        /// </summary>
        /// <value>
        /// The name of the backup.
        /// </value>
        [Option('b', "backupName", MutuallyExclusiveSet = "File",
            HelpText = "Only backup with specified name will be run")]
        public string BackupName { get; set; }

        /// <summary>
        /// Gets or sets the source starts with.
        /// </summary>
        /// <value>
        /// The source starts with.
        /// </value>
        [Option("sourceStartsWith", MutuallyExclusiveSet = "File",
            HelpText = "Only backup with source ID that starts with specified text")]
        public string SourceStartsWith { get; set; }

        /// <summary>
        /// Gets or sets the source ends with.
        /// </summary>
        /// <value>
        /// The source ends with.
        /// </value>
        [Option("sourceEndsWith", MutuallyExclusiveSet = "File",
            HelpText = "Only backup with source ID that ends with specified text")]
        public string SourceEndsWith { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CommandLineOptions"/> is verbose.
        /// </summary>
        /// <value>
        ///   <c>true</c> if verbose; otherwise, <c>false</c>.
        /// </value>
        [Option('v', "verbose", HelpText = "Should log everything")]
        public bool Verbose { get; set; }

        /// <summary>
        /// Gets the usage.
        /// </summary>
        /// <returns>The usage text.</returns>
        [HelpOption]
        public string BuildUsage()
        {
            return HelpText.AutoBuild(
                this,
                (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
