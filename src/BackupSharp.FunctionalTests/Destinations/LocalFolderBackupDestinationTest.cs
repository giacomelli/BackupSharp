using System.IO;
using BackupSharp.Destinations;
using NUnit.Framework;
using TestSharp;

namespace BackupSharp.FunctionalTests
{
    [TestFixture()]
    [Category("Functional")]
    public class LocalFolderBackupDestinationTest
    {
        #region Fields
        private LocalFolderBackupDestination m_target;
        private string m_sourceFolder;
        private string m_destinationFolder;
        #endregion

        #region Initialize
        [SetUp]
        public void InitializeTest()
        {
            m_sourceFolder = Path.Combine(VSProjectHelper.GetProjectFolderPath("BackupSharp.FunctionalTests"), "Stubs");
            m_destinationFolder = Path.Combine(Path.GetTempPath(), "LocalFolderBackupDestinationTest");

            if (Directory.Exists(m_destinationFolder))
            {
                Directory.Delete(m_destinationFolder, true);
            }

            m_target = new LocalFolderBackupDestination(m_destinationFolder);
            m_target.Initialize(new BackupContext(null, null));
        }
        #endregion

        #region Tests
        [Test()]
        public void Store_FolderAndFiles_Stored()
        {
            var item = new BackupItem(Path.Combine(m_sourceFolder, "SubFolder1"), BackupItemKind.Folder);
            item.DestinationFullName = "SubFolder1";
            m_target.StoreItem(item, null);
            TestSharp.DirectoryAssert.Exists(Path.Combine(m_destinationFolder, "SubFolder1"));

            item = new BackupItem(Path.Combine(m_sourceFolder, "SubFolder1/File1.1.txt"), BackupItemKind.File);
            item.DestinationFullName = "SubFolder1/File1.1.txt";
            m_target.StoreItem(item, File.ReadAllBytes(item.SourceFullName));
            TestSharp.FileAssert.Exists(Path.Combine(m_destinationFolder, "SubFolder1/File1.1.txt"));
        }
        #endregion
    }
}