//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Diagnostics;
//using System.IO;
//using HelperSharp;
//using MySql.Data.MySqlClient;

//namespace BackupSharp.Sources
//{
//    /// <summary>
//    /// A MSSQL (SQL Server) backup source.
//    /// </summary>
//    [DebuggerDisplay("MSSQL: {Id}")]
//    public class MSSqlBackupSource : DatabaseBackupSourceBase
//    {
//        #region Constructors
//        /// <summary>
//        /// Initializes a new instance of the <see cref="MSSqlBackupSource"/> class.
//        /// </summary>
//        /// <param name="connectionString">The connection string.</param>
//        public MSSqlBackupSource(string connectionString)
//            : base(connectionString)
//        {
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="MSSqlBackupSource"/> class.
//        /// </summary>
//        /// <param name="id">The identifier.</param>
//        /// <param name="connectionString">The connection string.</param>
//        public MSSqlBackupSource(string id, string connectionString)
//            : base(id, connectionString)
//        {
//        }
//        #endregion

//        #region implemented abstract members of DatabaseBackupSourceBase
//        /// <summary>
//        /// Reads the item.
//        /// </summary>
//        /// <param name="item">The item.</param>
//        /// <returns>
//        /// The item byte array.
//        /// </returns>
//        public override byte[] ReadItem(IBackupItem item)
//        {
//            var fileName = PathHelper.Combine(AppDomain.CurrentDomain.BaseDirectory, item.SourceFullName);

//            using (var conn = new SqlConnection(ConnectionString))
//            {
//                using (var cmd = new SqlCommand())
//                {
//                    using (var mb = new SqlBackup(cmd))
//                    {
//                        cmd.Connection = conn;
//                        cmd.CommandTimeout = 600;
//                        conn.Open();
//                        mb.ExportToFile(fileName);
//                        conn.Close();
//                    }
//                }
//            }

//            return File.ReadAllBytes(fileName);
//        }
//        #endregion
//    }
//}