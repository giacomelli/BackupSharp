using System.IO;
using System.Linq;
using BackupSharp.Sources;
using NUnit.Framework;
using TestSharp;

namespace BackupSharp.FunctionalTests
{
    [TestFixture()]
    [Category("Functional")]
    public class LocalFolderBackupSourceTest
    {
        #region Fields
        private LocalFolderBackupSource m_target;
        #endregion

        #region Initialize
        [SetUp]
        public void InitializeTest()
        {
            var folderPath = Path.Combine(VSProjectHelper.GetProjectFolderPath("BackupSharp.FunctionalTests"), "Stubs");
            m_target = new LocalFolderBackupSource(folderPath);
            m_target.Initialize(new BackupContext(null, null));
        }
        #endregion

        #region Tests
        [Test()]
        public void ReadItem_Item_Bytes()
        {
            var items = m_target.GetItems().ToList();
            var actual = m_target.ReadItem(items.First(f => f.Kind == BackupItemKind.File));
            Assert.AreNotEqual(0, actual.Length);

            actual = m_target.ReadItem(items.First(f => f.Kind == BackupItemKind.Folder));
            Assert.AreEqual(0, actual.Length);
        }

        [Test()]
        public void GetItems_ValidFolderPath_SubFoldersAndFiles()
        {
            var actual = m_target.GetItems().ToList();
            Assert.AreEqual(6, actual.Count);
            Assert.AreEqual(3, actual.Count(a => a.Kind == BackupItemKind.File));
            Assert.AreEqual(3, actual.Count(a => a.Kind == BackupItemKind.Folder));
        }
        #endregion
    }
}