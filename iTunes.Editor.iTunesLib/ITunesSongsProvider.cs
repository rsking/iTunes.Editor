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
    public class ITunesSongsProvider : SongsProvider
    {
        /// <inheritdoc />
        public override string Name => Properties.Resources.ITunesName;

        /// <inheritdoc />
        public override IEnumerable<SongInformation> GetTagInformation()
        {
#if NO_ITUNES
            throw new System.NotImplementedException("Compiled without iTunes support");
#else
            var app = new iTunesLib.iTunesApp();
            var library = app.LibraryPlaylist;

            for (int i = 1; i <= library.Tracks.Count; i++)
            {
                if (library.Tracks[i] is iTunesLib.IITFileOrCDTrack track)
                {
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
#endif
        }
    }
}
