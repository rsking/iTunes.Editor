// -----------------------------------------------------------------------
// <copyright file="ITunesSongLoader.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.ITunesLib
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The iTunes song loader.
    /// </summary>
    public class ITunesSongLoader : ISongLoader
    {
        /// <inheritdoc />
        public IEnumerable<SongInformation> GetTagInformation(string input)
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

        /// <inheritdoc />
        public Task<IEnumerable<SongInformation>> GetTagInformationAsync(string input)
        {
            return Task.Run(() => this.GetTagInformation(input));
        }
    }
}
