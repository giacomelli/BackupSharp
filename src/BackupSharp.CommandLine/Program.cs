using Skahal.Infrastructure.Framework.Logging;
using Skahal.Infrastructure.Logging.Log4net;

namespace BackupSharp
{
    /// <summary>
    /// The main entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            LogService.Initialize(new Log4netLogStrategy());

            var runner = new CommandLineRunner();
            runner.Run(args);
        }
    }
}
