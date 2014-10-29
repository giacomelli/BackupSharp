using System;
using System.Collections.Generic;
using System.Linq;
using HelperSharp;

namespace BackupSharp
{
    internal static class TypeHelper
    {
        public static IBackupSource CreateSource(string type, string args, IEnumerable<IBackupStep> availableSources = null)
        {
            return CreateInstance<IBackupSource>("Source", type, args, availableSources);
        }

        public static IBackupDestination CreateDestination(string type, string args)
        {
            return CreateInstance<IBackupDestination>("Destination", type, args);
        }

        private static T CreateInstance<T>(string kind, string type, string args, IEnumerable<IBackupStep> availableSteps = null)
        {
            var assembly = typeof(Backup).Assembly;
            var typeFullName = "BackupSharp.{0}s.{1}Backup{0}".With(kind, type);
            var sourceType = assembly.GetType(typeFullName);

            if (sourceType == null)
            {
                Console.WriteLine("Could not found a {0} with name '{1}'.", kind, typeFullName);
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            if (availableSteps != null)
            {
                var argsParts = args.Split(',');

                if (argsParts.Length > 1)
                {
                    var referencedStepName = argsParts[1];
                    var sourceReferenced = availableSteps.FirstOrDefault(f => f.Id.Equals(referencedStepName, StringComparison.OrdinalIgnoreCase));

                    if (sourceReferenced != null)
                    {
                        return (T)Activator.CreateInstance(sourceType, argsParts[0], sourceReferenced);
                    }
                }
            }

            return (T)Activator.CreateInstance(sourceType, args.Split(','));
        }
    }
}
