using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp;

namespace BackupSharp.UnitTests
{
    [TestFixture()]
    [Category("Unit")]
    public class BackupTest
    {
        [Test()]
        public void Constructor_SourceDestinationNull_Exception()
        {
            var source = MockRepository.GenerateMock<IBackupSource>();
            var destination = MockRepository.GenerateMock<IBackupDestination>();

            ExceptionAssert.IsThrowing(new ArgumentNullException("source"), () =>
            {
                new Backup(null, destination);
            });

            ExceptionAssert.IsThrowing(new ArgumentNullException("destination"), () =>
            {
                new Backup(source, null);
            });
        }

        [Test()]
        public void Constructor_NoName_CompositeName()
        {
            var source = MockRepository.GenerateMock<IBackupSource>();
            source.Expect(s => s.Id).Return("MySource");

            var destination = MockRepository.GenerateMock<IBackupDestination>();
            destination.Expect(s => s.Id).Return("MyDestination");

            var actual = new Backup("test", source, destination);
            Assert.AreEqual("test", actual.Name);

            actual = new Backup(source, destination);
            Assert.AreEqual("MySource2MyDestination", actual.Name);
        }

        [Test()]
        public void Run_SourceGetItemsThrowException_Exception()
        {
            var source = MockRepository.GenerateMock<IBackupSource>();
            source.Expect(s => s.GetItems()).Throw(new Exception("TEST"));

            var destination = MockRepository.GenerateMock<IBackupDestination>();
            var target = new Backup(source, destination);

            ExceptionAssert.IsThrowing(new Exception("TEST"), () =>
            {
                target.Run();
            });
        }

        [Test()]
        public void Run_SourceAndDestinationOk_BackupDone()
        {
            var source = MockRepository.GenerateMock<IBackupSource>();
            source.Expect(s => s.GetItems()).Return(new BackupItem[] { 
                new BackupItem("folder1", BackupItemKind.Folder),
                    new BackupItem("file1.1", BackupItemKind.File),
                    new BackupItem("file1.2", BackupItemKind.File),
                new BackupItem("folder2", BackupItemKind.Folder),
                   new BackupItem("file2.1", BackupItemKind.File),
            });

            source.Expect(s => s.GetRelativePath(null)).IgnoreArguments().Return("");

            var destination = MockRepository.GenerateMock<IBackupDestination>();

            var target = new Backup(source, destination);
            var actual = target.Run();
            Assert.AreEqual(2, actual.Folders.SuccessfulCount);
            Assert.AreEqual(3, actual.Files.SuccessfulCount);
            Assert.AreEqual(0, actual.Folders.Failed.Count);
            Assert.AreEqual(0, actual.Files.Failed.Count);

            source.VerifyAllExpectations();
            destination.VerifyAllExpectations();
        }

        [Test()]
        public void Run_IgnoreItemPattern_SomeItemsIgnored()
        {
            var source = MockRepository.GenerateMock<IBackupSource>();
            source.Expect(s => s.GetItems()).Return(new BackupItem[] { 
                new BackupItem("folder1", BackupItemKind.Folder),
                    new BackupItem("file1.1", BackupItemKind.File),
                    new BackupItem("file1.2", BackupItemKind.File),
                new BackupItem("folder2", BackupItemKind.Folder),
                    new BackupItem("file2.1", BackupItemKind.File),
            });

            source.Expect(s => s.GetRelativePath(null)).IgnoreArguments().Return("");

            var destination = MockRepository.GenerateMock<IBackupDestination>();

            var target = new Backup(source, destination);
            target.IgnoreItemPattern = new Regex(".*folder.*");
            var actual = target.Run();
            Assert.AreEqual(0, actual.Folders.SuccessfulCount);
            Assert.AreEqual(3, actual.Files.SuccessfulCount);
            Assert.AreEqual(0, actual.Folders.Failed.Count);
            Assert.AreEqual(0, actual.Files.Failed.Count);
            Assert.AreEqual(2, actual.Folders.Ignored.Count);
            Assert.AreEqual(0, actual.Files.Ignored.Count);

            source.VerifyAllExpectations();
            destination.VerifyAllExpectations();
        }

        [Test()]
        public void Run_ItemsFailed_Retries()
        {
            var source = MockRepository.GenerateMock<IBackupSource>();
            source.Expect(s => s.GetItems()).Return(new BackupItem[] { 
                new BackupItem("failedFile", BackupItemKind.File)
            });

            source.Expect(s => s.ReadItem(null)).IgnoreArguments().Throw(new Exception("test"));
            var destination = MockRepository.GenerateMock<IBackupDestination>();

            var target = new Backup(source, destination);
            target.MaxItemRetries = 3;
            var actual = target.Run();
            Assert.AreEqual(0, actual.Folders.SuccessfulCount);
            Assert.AreEqual(0, actual.Files.SuccessfulCount);
            Assert.AreEqual(0, actual.Folders.Failed.Count);
            Assert.AreEqual(1, actual.Files.Failed.Count);
            Assert.AreEqual(3, actual.Files.GetFailedExceptions("failedFile").Count);

            Assert.AreEqual(0, actual.Folders.Ignored.Count);
            Assert.AreEqual(0, actual.Files.Ignored.Count);

            source.VerifyAllExpectations();
            destination.VerifyAllExpectations();
        }
    }
}