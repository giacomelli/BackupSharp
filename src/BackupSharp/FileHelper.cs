using System.IO;

namespace BackupSharp
{
    /// <summary>
    /// File helpers.
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Reads the fully stream to a byte array..
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The stream byte array.</returns>
        public static byte[] ReadFully(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Deletes the specified file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public static void Delete(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
    }
}