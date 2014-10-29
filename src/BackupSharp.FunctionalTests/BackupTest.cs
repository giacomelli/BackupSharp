using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BackupSharp.Destinations;
using BackupSharp.Naming;
using BackupSharp.Sources;
using DropNet;
using HelperSharp;
using Ionic.Utils.Zip;
using NUnit.Framework;
using TestSharp;

namespace BackupSharp.FunctionalTests
{
    [TestFixture()]
    [Category("Functional")]
    public class BackupTest
    {
        #region Tests

        #region FTP source
        [Test]
        [Category("Ftp")]
        [Category("Dropbox")]
        public void Run_Ftp2Dropbox_Done()
        {
            var source = CreateFtpSource();
            var destination = CreateDropboxBackupDestination();

            var target = CreateBackup(source, destination);
            var actual = target.Run();

            AssertBackupResult(actual);
            AssertResultToDropbox(source);
        }

        [Test]
        [Category("Ftp")]
        [Category("LocalFolder")]
        public void Run_Ftp2LocalFolder_Done()
        {
            var source = CreateFtpSource();
            var localFolderDestination = Path.Combine(Path.GetTempPath());
            var destination = new LocalFolderBackupDestination(localFolderDestination);
            var target = CreateBackup(source, destination);
            var actual = target.Run();
            AssertBackupResult(actual);

            var rootFolder = Path.Combine(localFolderDestination, target.Naming.RootPath);
            AssertResultToLocalFolder(rootFolder);
        }

        [Test]
        [Category("Ftp")]
        [Category("Zip")]
        public void Run_Ftp2Zip_Done()
        {
            var source = CreateFtpSource();
            var localFolderDestination = Path.Combine(Path.GetTempPath());
            var destination = new ZipBackupDestination(localFolderDestination);
            var target = CreateBackup(source, destination);
            var actual = target.Run();
            AssertBackupResult(actual);
            AssertResultToZip(destination);
        }
        #endregion

        #region Local folder source
        [Test]
        [Category("LocalFolder")]
        [Category("Dropbox")]
        public void Run_LocalFolder2Dropbox_Done()
        {
            var source = CreateLocalFolderSource();
            var destination = CreateDropboxBackupDestination();
            var target = CreateBackup(source, destination);
            var actual = target.Run();

            AssertBackupResult(actual);
            AssertResultToDropbox(source);
        }

        [Test]
        [Category("LocalFolder")]
        public void Run_LocalFolder2LocalFolder_Done()
        {
            var source = CreateLocalFolderSource();
            var localFolderDestination = Path.Combine(Path.GetTempPath());
            var destination = new LocalFolderBackupDestination(localFolderDestination);
            var target = CreateBackup(source, destination);
            var actual = target.Run();
            AssertBackupResult(actual);

            var rootFolder = Path.Combine(localFolderDestination, target.Naming.RootPath);
            AssertResultToLocalFolder(rootFolder);
        }

        [Test]
        [Category("LocalFolder")]
        [Category("Zip")]
        public void Run_LocalFolder2Zip_Done()
        {
            var source = CreateLocalFolderSource();
            var localFolderDestination = Path.Combine(Path.GetTempPath());
            var destination = new ZipBackupDestination(localFolderDestination);
            var target = CreateBackup(source, destination);
            var actual = target.Run();

            AssertBackupResult(actual);
            AssertResultToZip(destination);
        }
        #endregion

        #region MySQL source
        [Test]
        [Category("MySQL")]
        [Category("Dropbox")]
        public void Run_MySql2Dropbox_Done()
        {
            var source = CreateMySqlSource();
            var destination = CreateDropboxBackupDestination();
            var target = CreateBackup(source, destination);
            var actual = target.Run();

            AssertMySqlBackupSourceResult(actual);
            var dp = CreateDropboxClient ();

            var rootFolder = dp.GetMetaData(source.Id);
            Assert.AreEqual(1, rootFolder.Contents.Count);
            Assert.IsFalse (rootFolder.Contents [0].Is_Dir);
        }

        [Test]
        [Category("MySQL")]
        [Category("LocalFolder")]
        public void Run_MySql2LocalFolder_Done()
        {
            var source = CreateMySqlSource();
            var localFolderDestination = Path.Combine(Path.GetTempPath());
            var destination = new LocalFolderBackupDestination(localFolderDestination);
            var target = CreateBackup(source, destination);
            var actual = target.Run();
            
            AssertMySqlBackupSourceResult(actual);

            var rootFolder = Path.Combine(localFolderDestination, target.Naming.RootPath);
            TestSharp.FileAssert.Exists(Path.Combine(rootFolder, "database.backup.sql"));
        }

        [Test]
        [Category("MySQL")]
        [Category("Zip")]
        public void Run_MySql2Zip_Done()
        {
            var source = CreateMySqlSource();
            var localFolderDestination = Path.Combine(Path.GetTempPath());
            var destination = new ZipBackupDestination(localFolderDestination);
            var target = CreateBackup(source, destination);
            var actual = target.Run();

            AssertMySqlBackupSourceResult(actual);

            TestSharp.FileAssert.Exists(destination.ZipFileName);
            var extractFolder = Path.Combine(Path.GetDirectoryName(destination.ZipFileName), "extract");

            PathHelper.EnsureClearFolder (extractFolder);

            using (var zipFile = ZipFile.Read(destination.ZipFileName))
            {
                Assert.AreEqual (0, zipFile.Count (e => e.IsDirectory));
                Assert.AreEqual (1, zipFile.Count (e => e.FileName.EndsWith(".sql")));
            }
        }
        #endregion
        #endregion

        #region  Helpers
        private static FtpBackupSource CreateFtpSource()
        {
            var sourceId = "FunctionalTests_{0}".With(Guid.NewGuid());
            var ftpServer = Environment.GetEnvironmentVariable("BackupSharpFtpServer");
            var ftpUserName = Environment.GetEnvironmentVariable("BackupSharpFtpUserName");
            var ftpPassword = Environment.GetEnvironmentVariable("BackupSharpFtpPassword");
            var ftpSource = new FtpBackupSource(sourceId, ftpServer, ftpUserName, ftpPassword);
            ftpSource.Folder = Environment.GetEnvironmentVariable("BackupSharpFtpFolder");

            return ftpSource;
        }

        private static LocalFolderBackupSource CreateLocalFolderSource()
        {
            var sourceId = "FunctionalTests_{0}".With(Guid.NewGuid());
            var sourceFolder = VSProjectHelper.GetProjectFolderPath("BackupSharp.FunctionalTests");
            sourceFolder = Path.Combine(sourceFolder, "Stubs");

            return new LocalFolderBackupSource(sourceId, sourceFolder);
        }

        private static MySqlBackupSource CreateMySqlSource()
        {
            var connectionString = Environment.GetEnvironmentVariable("BackupSharpMySqlConnectionString");
            return new MySqlBackupSource(connectionString);
        }


        private static DropboxBackupDestination CreateDropboxBackupDestination()
        {
            var dpApiKey = Environment.GetEnvironmentVariable("BackupSharpDropboxApiKey");
            var dpApiSecret = Environment.GetEnvironmentVariable("BackupSharpDropboxApiSecret");
            var dpAccessToken = Environment.GetEnvironmentVariable("BackupSharpDropboxAccessToken");
            var dropboxDestination = new DropboxBackupDestination(dpApiKey, dpApiSecret, dpAccessToken);

            var dp = new DropNetClient(dpApiKey, dpApiSecret, dpAccessToken);
            dp.UseSandbox = true;

            try
            {
                dp.Delete("FunctionalTests");
            }
            catch { }

            return dropboxDestination;
        }

        private static Backup CreateBackup(IBackupSource source, IBackupDestination destination)
        {
            var backup = new Backup(source, destination, new SourceIdNamingStrategy());
            backup.IgnoreItemPattern = new Regex("SubFolder2");
            backup.MaxItemRetries = 1;
            backup.MaxThreads = 1;

            return backup;
        }

        private static void AssertBackupResult(BackupResult actual)
        {
            Assert.AreEqual(2, actual.Folders.SuccessfulCount);
            Assert.AreEqual(0, actual.Folders.Failed.Count);
            Assert.AreEqual(1, actual.Folders.Ignored.Count);

            Assert.AreEqual(3, actual.Files.SuccessfulCount);
            Assert.AreEqual(0, actual.Files.Failed.Count);
            Assert.AreEqual(0, actual.Files.Ignored.Count);
        }

        private static void AssertMySqlBackupSourceResult(BackupResult actual)
        {
            Assert.AreEqual(0, actual.Folders.SuccessfulCount);
            Assert.AreEqual(0, actual.Folders.Failed.Count);
            Assert.AreEqual(0, actual.Folders.Ignored.Count);

            Assert.AreEqual(1, actual.Files.SuccessfulCount);
            Assert.AreEqual(0, actual.Files.Failed.Count);
            Assert.AreEqual(0, actual.Files.Ignored.Count);
        }

        private static void AssertResultToLocalFolder(string rootFolder)
        {
            TestSharp.DirectoryAssert.IsFilesCount(3, rootFolder, "*.txt", true);
            var expected = Path.Combine(rootFolder, "SubFolder1/File1.1.txt");
            TestSharp.FileAssert.Exists(expected);
            TestSharp.FileAssert.IsContent("Content of File1.1.txt.", expected);

            expected = Path.Combine(rootFolder, "SubFolder1/File1.2.txt");
            TestSharp.FileAssert.Exists(expected);
            TestSharp.FileAssert.IsContent("Content of File1.2.txt.", expected);

            expected = Path.Combine(rootFolder, "SubFolder1/SubFolder12/File12.1.txt");
            TestSharp.FileAssert.IsContent("Content of File12.1.txt.", expected);
            TestSharp.FileAssert.Exists(expected);
        }

        private static void AssertResultToDropbox(IBackupSource source)
        {
            var dp = CreateDropboxClient ();

            var rootFolder = dp.GetMetaData(source.Id);
            Assert.AreEqual(1, rootFolder.Contents.Count);

            var subfolder1 = dp.GetMetaData(rootFolder.Contents[0].Path);
            Assert.AreEqual(3, subfolder1.Contents.Count);

            Assert.AreEqual(1, subfolder1.Contents.Count(c => c.Is_Dir));
            Assert.AreEqual(2, subfolder1.Contents.Count(c => !c.Is_Dir));

            var subfolder12 = dp.GetMetaData(subfolder1.Contents.First(c => c.Is_Dir).Path);
            Assert.AreEqual(1, subfolder12.Contents.Count);
        }

        private static void AssertResultToZip(ZipBackupDestination destination)
        {
            TestSharp.FileAssert.Exists(destination.ZipFileName);
            var extractFolder = Path.Combine(Path.GetDirectoryName(destination.ZipFileName), "extract");

            PathHelper.EnsureClearFolder (extractFolder);

            using (var zipFile = ZipFile.Read(destination.ZipFileName))
            {
                zipFile.ExtractAll(extractFolder, true);
            }

            AssertResultToLocalFolder(extractFolder);
        }

        private static DropNetClient CreateDropboxClient ()
        {
            var dpApiKey = Environment.GetEnvironmentVariable ("BackupSharpDropboxApiKey");
            var dpApiSecret = Environment.GetEnvironmentVariable ("BackupSharpDropboxApiSecret");
            var dpAccessToken = Environment.GetEnvironmentVariable ("BackupSharpDropboxAccessToken");
            var dp = new DropNetClient (dpApiKey, dpApiSecret, dpAccessToken);
            dp.UseSandbox = true;

            return dp;
        }
        #endregion
    }
}

