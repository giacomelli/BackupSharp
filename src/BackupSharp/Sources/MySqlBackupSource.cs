using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using HelperSharp;
using MySql.Data.MySqlClient;

namespace BackupSharp.Sources
{
    /// <summary>
    /// A MySQL backup source.
    /// </summary>
    [DebuggerDisplay("MySql: {Id}")]
    public class MySqlBackupSource : DatabaseBackupSourceBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlBackupSource"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public MySqlBackupSource(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlBackupSource"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="connectionString">The connection string.</param>
        public MySqlBackupSource(string id, string connectionString)
            : base(id, connectionString)
        {
        }
        #endregion

        #region implemented abstract members of DatabaseBackupSourceBase
        /// <summary>
        /// Reads the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The item byte array.
        /// </returns>
        public override byte[] ReadItem(IBackupItem item)
        {
            var fileName = PathHelper.Combine(AppDomain.CurrentDomain.BaseDirectory, item.SourceFullName);

            using (var conn = new MySqlConnection(ConnectionString))
            {
                using (var cmd = new MySqlCommand())
                {
                    using (var mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        cmd.CommandTimeout = 600;
                        conn.Open();
                        mb.ExportToFile(fileName);
                        conn.Close();
                    }
                }
            }

            return File.ReadAllBytes(fileName);
        }
        #endregion
    }
}