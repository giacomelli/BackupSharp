using System;
using System.IO;
using HelperSharp;
using NUnit.Framework;
using TestSharp;

namespace BackupSharp.CommandLine.FunctionalTests
{
    [TestFixture()]
    [Category("Functional")]
    public class BackupRunnerTest
    {
        [SetUp]
        public void InitializeTest()
        {
            var folder = PathHelper.GetFullPath("temp");

            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }

            folder = PathHelper.GetFullPath("temp/source");
            Directory.CreateDirectory(folder);
            var stubsFolder = VSProjectHelper.GetProjectFolderPath("BackupSharp.FunctionalTests");
            stubsFolder = Path.Combine(stubsFolder, "Stubs");
            TestSharp.DirectoryHelper.CopyDirectory(stubsFolder, folder);
        }

        [Test()]
        public void Run_BackupConfigurationFileOptionZipBackupDestination_RunBackupsDefinedOnFile()
        {
            var target = new CommandLineRunner();
            var filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupConfigurationFileOption_ZipBackupDestination.config");
            target.Run("-f {0}".With(filename));

            TestSharp.DirectoryAssert.IsFilesCount(1, PathHelper.GetFullPath("temp/SourceFolder"), "*.zip");
        }

        [Test()]
        public void Run_LocalFolder2Zip_Done()
        {
            var target = new CommandLineRunner();
            target.Run(
                "--sourceName=LocalFolder",
                "--sourceArgs=local,temp/source",
                "--destinationName=Zip",
                "--destinationArgs=temp");
        }
    }
}

