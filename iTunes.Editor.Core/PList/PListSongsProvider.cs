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
    public class PListSongsProvider : ISongsProvider, IFileProvider
    {
        /// <inheritdoc />
        public string? File { get; set; }

        /// <inheritdoc />
        public string Name => Properties.Resources.PListName;

        /// <inheritdoc />
        public async IAsyncEnumerable<SongInformation> GetTagInformationAsync([System.Runtime.CompilerServices.EnumeratorCancellation] System.Threading.CancellationToken cancellationToken = default)
        {
            PList plist;
            var stream = System.IO.File.OpenRead(this.File);
#if NETSTANDARD2_1_OR_GREATER
            await
#endif
            using (stream)
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(PList));
                using var reader = System.Xml.XmlReader.Create(stream, new System.Xml.XmlReaderSettings
                {
                    DtdProcessing = System.Xml.DtdProcessing.Parse,
                    IgnoreWhitespace = true,
                });

                plist = await System.Threading.Tasks.Task.Run(() => (PList)serializer.Deserialize(reader), cancellationToken).ConfigureAwait(false);
            }

            if (plist["Tracks"] is IDictionary<string, object?> dictionary)
            {
                foreach (var track in dictionary
                    .Select(kvp => kvp.Value as IDictionary<string, object?>)
                    .Where(dict => !(dict is null))
                    .Select(dict => new Track(dict!)))
                {
                    yield return (SongInformation)track;
                }
            }
        }
    }
}
