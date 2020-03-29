// -----------------------------------------------------------------------
// <copyright file="ITunesSongsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.ITunesLib
{
    using System.Collections.Generic;

    /// <summary>
    /// The iTunes song loader.
    /// </summary>
    public class ITunesSongsProvider : ISongsProvider
    {
        /// <inheritdoc />
        public string Name => Properties.Resources.ITunesName;

        /// <summary>
        /// Gets or sets a value indicating whether to update the metadata.
        /// </summary>
        public bool UpdateMetadata { get; set; }

        /// <inheritdoc />

#if NO_ITUNES
        public IAsyncEnumerable<SongInformation> GetTagInformationAsync([System.Runtime.CompilerServices.EnumeratorCancellation] System.Threading.CancellationToken cancellationToken) => throw new System.NotImplementedException("Compiled without iTunes support");
#else
        public async IAsyncEnumerable<SongInformation> GetTagInformationAsync([System.Runtime.CompilerServices.EnumeratorCancellation] System.Threading.CancellationToken cancellationToken)
        {
            var library = await System.Threading.Tasks.Task.Run(() =>
            {
                var app = new iTunesLib.iTunesApp();
                return app.LibraryPlaylist;
            }).ConfigureAwait(false);

            for (int i = 1; i <= library.Tracks.Count; i++)
            {
                if (library.Tracks[i] is iTunesLib.IITFileOrCDTrack track)
                {
                    if (this.UpdateMetadata)
                    {
                        track.UpdateInfoFromFile();
                    }

                    if (track.Location == null)
                    {
                        continue;
                    }

                    yield return new SongInformation(
                        track.Name,
                        track.Artist,
                        track.SortArtist ?? track.Artist,
                        track.Album,
                        track.Location,
                        track.Rating);
                }
            }
        }
#endif
    }
}
