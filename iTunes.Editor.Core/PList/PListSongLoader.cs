// -----------------------------------------------------------------------
// <copyright file="PListSongLoader.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PList
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A <see cref="ISongLoader"/> that loads from an Apple plist file.
    /// </summary>
    public class PListSongLoader : ISongLoader
    {
        /// <inheritdoc />
        public IEnumerable<SongInformation> GetTagInformation(string input)
        {
            PList plist;
            using (var stream = System.IO.File.OpenRead(input))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(PList));
                plist = serializer.Deserialize(stream) as PList;
            }

            var dictionary = plist["Tracks"] as IDictionary<string, object>;

            if (dictionary == null)
            {
                return null;
            }

            return dictionary.Where(_ => _.Value is IDictionary<string, object>).Select(_ => new Track(_.Value as IDictionary<string, object>)).Cast<SongInformation>();
        }

        /// <inheritdoc />
        public System.Threading.Tasks.Task<IEnumerable<SongInformation>> GetTagInformationAsync(string input)
        {
            return System.Threading.Tasks.Task.Run(() => this.GetTagInformation(input));
        }
    }
}
