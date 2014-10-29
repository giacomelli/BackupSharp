using System;
using System.Linq;
using BackupSharp.Sources;
using NUnit.Framework;
using Rhino.Mocks;

namespace BackupSharp.FunctionalTests
{
    [TestFixture()]
    [Category("Functional")]
    public class FtpBackupSourceTest
    {
        [Test()]
        public void GetItems_ValidSever_FilesAndFoldersFound()
        {
            var ftpServer = Environment.GetEnvironmentVariable("BackupSharpFtpServer");
            var ftpUserName = Environment.GetEnvironmentVariable("BackupSharpFtpUserName");
            var ftpPassword = Environment.GetEnvironmentVariable("BackupSharpFtpPassword");
            var target = new FtpBackupSource(ftpServer, ftpUserName, ftpPassword);
            target.Folder = Environment.GetEnvironmentVariable("BackupSharpFtpFolder");

            target.Initialize(new BackupContext(target, MockRepository.GenerateMock<IBackupDestination>()));

            target.ItemFound += (sender, e) =>
            {
                e.Ignored = e.Item.SourceFullName.EndsWith("SubFolder2");
            };
            var actual = target.GetItems();

            Assert.AreEqual(2, actual.Count(f => f.Kind == BackupItemKind.Folder));
            Assert.AreEqual(3, actual.Count(f => f.Kind == BackupItemKind.File));
        }
    }
}

