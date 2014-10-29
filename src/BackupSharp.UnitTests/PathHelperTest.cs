using NUnit.Framework;

namespace BackupSharp.UnitTests
{
    [TestFixture()]
    [Category("Unit")]
    public class PathHelperTest
    {
        #region Windows path
        [Test()]
        public void Combine_WindowsSecondPathRootedAsUnix_Combined()
        {
            PathHelper.DirectorySeparator = @"\";
            var actual = PathHelper.Combine(@"c:\test", "/a/b/c");
            Assert.AreEqual(actual, @"c:\test\a\b\c");
        }

        [Test()]
        public void Combine_WindowsSecondPathRootedAsWindows_Combined()
        {
            PathHelper.DirectorySeparator = @"\";
            var actual = PathHelper.Combine(@"c:\test", @"c:\a\b\c");
            Assert.AreEqual(actual, @"c:\test\a\b\c");
        }

        [Test()]
        public void Combine_WindowsSecondPathNotRootedAsUnix_Combined()
        {
            PathHelper.DirectorySeparator = @"\";
            var actual = PathHelper.Combine(@"c:\test", "a/b/c");
            Assert.AreEqual(actual, @"c:\test\a\b\c");
        }

        [Test()]
        public void Combine_WindowsSecondPathNotRootedAsWindows_Combined()
        {
            PathHelper.DirectorySeparator = @"\";
            var actual = PathHelper.Combine(@"c:\test", @"\a\b\c");
            Assert.AreEqual(actual, @"c:\test\a\b\c");
        }
        #endregion

        #region Unix path
        [Test()]
        public void Combine_UnixSecondPathRootedAsUnix_Combined()
        {
            PathHelper.DirectorySeparator = "/";
            var actual = PathHelper.Combine("/test", "/a/b/c");
            Assert.AreEqual(actual, @"/test/a/b/c");
        }

        [Test()]
        public void Combine_UnixSecondPathRootedAsWindows_Combined()
        {
            PathHelper.DirectorySeparator = "/";
            var actual = PathHelper.Combine("/test", @"c:\a\b\c");
            Assert.AreEqual(actual, @"/test/a/b/c");
        }

        [Test()]
        public void Combine_UnixSecondPathNotRootedAsUnix_Combined()
        {
            PathHelper.DirectorySeparator = "/";
            var actual = PathHelper.Combine("/test", "a/b/c");
            Assert.AreEqual(actual, @"/test/a/b/c");
        }

        [Test()]
        public void Combine_UnixSecondPathNotRootedAsWindows_Combined()
        {
            PathHelper.DirectorySeparator = "/";
            var actual = PathHelper.Combine("/test", @"\a\b\c");
            Assert.AreEqual(actual, @"/test/a/b/c");
        }
        #endregion
    }
}