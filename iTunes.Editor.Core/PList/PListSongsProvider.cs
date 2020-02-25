// -----------------------------------------------------------------------
// <copyright file="PListSongsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PList
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A <see cref="ISongsProvider"/> that loads from an Apple plist file.
    /// </summary>
    public class PListSongsProvider : SongsProvider, IFileProvider
    {
        /// <inheritdoc />
        public string? File { get; set; }

        /// <inheritdoc />
        public override string Name => Properties.Resources.PListName;

        /// <inheritdoc />
        public override IEnumerable<SongInformation> GetTagInformation()
        {
            PList plist;
            using (var stream = System.IO.File.OpenRead(this.File))
            {
                using var reader = System.Xml.XmlReader.Create(stream, new System.Xml.XmlReaderSettings
                {
                    DtdProcessing = System.Xml.DtdProcessing.Parse,
                    IgnoreWhitespace = true,
                });
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(PList));
                plist = (PList)serializer.Deserialize(reader);
            }

            return plist["Tracks"] is IDictionary<string, object> dictionary
                ? dictionary
                    .Where(kvp => kvp.Value is IDictionary<string, object>)
                    .Select(value => new Track((IDictionary<string, object>)value.Value))
                    .Select(track => (SongInformation)track)
                : Enumerable.Empty<SongInformation>();
        }
    }
}
