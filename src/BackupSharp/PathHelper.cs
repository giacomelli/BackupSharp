using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;
using HelperSharp;

namespace BackupSharp
{
    /// <summary>
    /// Path helpers.
    /// </summary>
    public static class PathHelper
    {
        #region Fields
        private static Regex s_fixDirectorySeparatorRegex = new Regex(@"(/|\\)+", RegexOptions.Compiled);
        private static Regex s_removeDriveRegex = new Regex(@"[a-z]+:(/|\\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="PathHelper"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Is a property, not a field.")]
        static PathHelper()
        {
            DirectorySeparator = Path.DirectorySeparatorChar.ToString();
        }
        #endregion

        #region Properties
        public static string DirectorySeparator { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Combines the paths.
        /// </summary>
        /// <param name="firstPath">The first path.</param>
        /// <param name="secondPath">The second path.</param>
        /// <returns>The path.</returns>
        public static string Combine(string firstPath, string secondPath)
        {
            var secondPathSanitized = s_removeDriveRegex.Replace(secondPath, string.Empty);
            var result = "{0}/{1}".With(firstPath, secondPathSanitized);
            result = s_fixDirectorySeparatorRegex.Replace(result, DirectorySeparator);

            return result;
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>        
        /// <param name="path">The partial path.</param>
        /// <returns>The full path.</returns>
        public static string GetFullPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }

        /// <summary>
        /// Ensures the folder exists.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void EnsureFolderExists(string path)
        {
            var folder = GetFolder(path);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        /// <summary>
        /// Ensures the clear folder.
        /// </summary>
        /// <param name="path">The folder path.</param>
        public static void EnsureClearFolder(string path)
        {
            var folder = GetFolder(path);

            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }

            Directory.CreateDirectory(folder);
        }

        private static string GetFolder(string path)
        {
            if (Path.HasExtension(path))
            {
                return Path.GetDirectoryName(path);
            }

            return path;
        }
        #endregion
    }
}
