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
        public string File { get; set; }

        /// <inheritdoc />
        public override string Name => Properties.Resources.PListName;

        /// <inheritdoc />
        public override IEnumerable<SongInformation> GetTagInformation()
        {
            PList plist;
            using (var stream = System.IO.File.OpenRead(this.File))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(PList));
                plist = serializer.Deserialize(stream) as PList;
            }

            var dictionary = plist["Tracks"] as IDictionary<string, object>;

            if (dictionary == null)
            {
                return null;
            }

            return dictionary.Where(_ => _.Value is IDictionary<string, object>).Select(_ => new Track(_.Value as IDictionary<string, object>)).Select(_ => (SongInformation)_);
        }
    }
}
