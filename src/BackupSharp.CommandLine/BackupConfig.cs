using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using HelperSharp;

namespace BackupSharp
{
    /// <summary>
    /// Represents a backup config.
    /// </summary>
    public class BackupConfig
    {
        #region Properties
        /// <summary>
        /// Gets the sources.
        /// </summary>
        public IList<IBackupSource> Sources { get; private set; }

        /// <summary>
        /// Gets the destinations.
        /// </summary>
        public IList<IBackupDestination> Destinations { get; private set; }

        /// <summary>
        /// Gets the backups.
        /// </summary>        
        public IList<Backup> Backups { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Loads from a file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public void Load(string fileName)
        {
            Sources = new List<IBackupSource>();
            Destinations = new List<IBackupDestination>();
            Backups = new List<Backup>();

            var xml = new XmlDocument();
            xml.Load(fileName);

            foreach (XmlNode source in xml.SelectNodes("//source"))
            {
                var id = GetAttrValue(source, "id");
                var type = GetAttrValue(source, "type");
                var args = GetAttrValue(source, "args");

                Sources.Add(
                    TypeHelper.CreateSource(
                        type,
                        "{0},{1}".With(id, args),
                        Sources));
            }

            foreach (XmlNode destination in xml.SelectNodes("//destination"))
            {
                var id = GetAttrValue(destination, "id");
                var type = GetAttrValue(destination, "type");
                var args = GetAttrValue(destination, "args");

                Destinations.Add(TypeHelper.CreateDestination(type, "{0},{1}".With(id, args)));
            }

            foreach (XmlNode backup in xml.SelectNodes("//backup"))
            {
                var name = GetAttrValue(backup, "name");
                var sourceId = Backup.SanitizeId(GetAttrValue(backup, "source"));
                var destinationId = GetAttrValue(backup, "destination");

                var source = Sources.FirstOrDefault(s => s.Id.Equals(sourceId, StringComparison.OrdinalIgnoreCase));

                if (source == null)
                {
                    throw new InvalidOperationException("Cannot find a source with id '{0}'. ".With(sourceId));
                }

                var destination = Destinations.FirstOrDefault(d => d.Id.Equals(destinationId, StringComparison.OrdinalIgnoreCase));

                if (destination == null)
                {
                    throw new InvalidOperationException("Cannot find a destination with id '{0}'. ".With(destinationId));
                }

                var b = new Backup(
                    name,
                    source,
                    destination);

                GetAttrValue(
                    backup,
                    "ignoreItemPattern",
                    v => b.IgnoreItemPattern = new Regex(v));

                b.MaxItemRetries = GetAttrIntValue(backup, "maxItemRetries");
                b.MaxThreads = GetAttrIntValue(backup, "maxThreads");

                Backups.Add(b);
            }
        }

        private static string GetAttrValue(XmlNode node, string attrName)
        {
            var attr = node.Attributes[attrName];

            if (attr != null)
            {
                return attr.Value;
            }

            return null;
        }

        private static int GetAttrIntValue(XmlNode node, string attrName)
        {
            return Convert.ToInt32(GetAttrValue(node, attrName), CultureInfo.InvariantCulture);
        }

        private static void GetAttrValue(XmlNode node, string attrName, Action<string> valueRead)
        {
            var attr = node.Attributes[attrName];

            if (attr != null)
            {
                valueRead(attr.Value);
            }
        }
        #endregion
    }
}
