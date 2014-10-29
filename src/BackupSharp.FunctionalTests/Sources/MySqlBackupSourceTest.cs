using System;
using System.Linq;
using System.Text;
using BackupSharp.Sources;
using NUnit.Framework;

namespace BackupSharp.FunctionalTests
{
    [TestFixture()]
    [Category("Functional")]
    public class MySqlBackupSourceTest
    {
        [Test()]
        public void GetItems_ValidConnectionString_SqlDumpFileFound()
        {
            var connectionString = Environment.GetEnvironmentVariable("BackupSharpMySqlConnectionString");
            var target = new MySqlBackupSource(connectionString);

            var actual = target.GetItems();
            Assert.AreEqual(1, actual.Count());

            var data = target.ReadItem(actual.First());
            var content = Encoding.UTF8.GetString(data);
            StringAssert.Contains("DROP TABLE IF EXISTS", content);
        }
    }
}

