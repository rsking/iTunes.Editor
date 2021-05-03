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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "This is for iTunes")]
    public class ITunesSongsProvider : ISongsProvider
    {
        /// <inheritdoc />
        public string Name => Properties.Resources.ITunesName;

        /// <summary>
        /// Gets or sets a value indicating whether to update the metadata.
        /// </summary>
        public bool UpdateMetadata { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to set the album artist.
        /// </summary>
        public bool SetAlbumArtist { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to update the grouping.
        /// </summary>
        public bool UpdateGrouping { get; set; }

        /// <inheritdoc />
#if COM_AVAILABLE
        public async IAsyncEnumerable<SongInformation> GetTagInformationAsync([System.Runtime.CompilerServices.EnumeratorCancellation] System.Threading.CancellationToken cancellationToken = default)
        {
            var library = await System.Threading.Tasks.Task.Run(() =>
            {
                var app = new iTunesLib.iTunesApp();
                return app.LibraryPlaylist;
            }).ConfigureAwait(false);

            for (var i = 1; i <= library.Tracks.Count; i++)
            {
                if (library.Tracks[i] is iTunesLib.IITFileOrCDTrack track)
                {
                    if (this.UpdateMetadata)
                    {
                        track.UpdateInfoFromFile();
                    }

                    if (track.Location is null)
                    {
                        continue;
                    }

                    if (this.SetAlbumArtist && !track.Compilation)
                    {
                        if (string.IsNullOrWhiteSpace(track.AlbumArtist)
                            && !string.IsNullOrWhiteSpace(track.Artist))
                        {
                            track.AlbumArtist = track.Artist;
                        }

                        if (string.IsNullOrWhiteSpace(track.SortAlbumArtist)
                            && string.Equals(track.AlbumArtist, track.Artist, System.StringComparison.Ordinal)
                            && !string.IsNullOrWhiteSpace(track.SortArtist))
                        {
                            track.SortAlbumArtist = track.SortArtist;
                        }
                    }

                    var hasLyrics = !string.IsNullOrEmpty(track.Lyrics);
                    if (this.UpdateGrouping)
                    {
                        var original = track.Grouping;
                        var grouping = original.RemoveHasLyrics();
                        if (!hasLyrics && track.Location.GetMediaKind() == MediaKind.Song)
                        {
                            grouping = grouping.AddNoLyrics();
                        }

                        if (!string.Equals(grouping, original, System.StringComparison.Ordinal))
                        {
                            track.Grouping = grouping ?? string.Empty;
                        }
                    }

                    yield return new SongInformation(
                        track.Name,
                        track.Artist,
                        track.SortArtist ?? track.Artist,
                        track.AlbumArtist,
                        track.SortAlbumArtist,
                        track.Album,
                        track.Location,
                        track.Rating,
                        hasLyrics);
                }
            }
        }
#else
        public IAsyncEnumerable<SongInformation> GetTagInformationAsync(System.Threading.CancellationToken cancellationToken = default) => throw new System.NotSupportedException("Compiled without iTunes support");
#endif
    }
}
