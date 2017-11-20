// -----------------------------------------------------------------------
// <copyright file="iPodSongLoader.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.IPod
{
    using System.Collections.Generic;
    using global::IPod;

    /// <summary>
    /// The iPod <see cref="ISongLoader"/>.
    /// </summary>
    public class IPodSongLoader : ISongLoader
    {
        /// <inheritdoc />
        public IEnumerable<SmallTagInformation> GetTagInformation(string input)
        {
            // see if this has the requisit parts
            var directory = new System.IO.DirectoryInfo(input);
            var controlDirectory = System.IO.Path.Combine(input, "iPod_Control");
            if (!System.IO.Directory.Exists(controlDirectory))
            {
                throw new System.IO.DirectoryNotFoundException($"Failed to find iPod Control folder under {input}");
            }

            var iTunesDB = System.IO.Path.Combine(controlDirectory, "iTunes\\iTunesDB");

            if (!System.IO.File.Exists(iTunesDB))
            {
                throw new System.IO.FileNotFoundException($"Failed to find iTunesDB file under {controlDirectory}");
            }

            // read in the database
            var trackDatabase = new TrackDatabase(iTunesDB);
            var count = trackDatabase.Tracks.Count;

            for (int i = 0; i < count; i++)
            {
                var track = trackDatabase.Tracks[i];

                yield return new SmallTagInformation(track.Title, track.Artist, track.SortArtist, track.Album, track.FileName, (int)track.Rating);
            }
        }

        /// <inheritdoc />
        public System.Threading.Tasks.Task<IEnumerable<SmallTagInformation>> GetTagInformationAsync(string input)
        {
            return System.Threading.Tasks.Task.Run(() => this.GetTagInformation(input));
        }
    }
}
