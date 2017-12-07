// -----------------------------------------------------------------------
// <copyright file="IPodSongsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.IPod
{
    using System.Collections.Generic;
    using global::IPod;

    /// <summary>
    /// The iPod <see cref="ISongsProvider"/>.
    /// </summary>
    public class IPodSongsProvider : SongsProvider, IFolderProvider
    {
        /// <inheritdoc />
        public string Folder { get; set; }

        /// <inheritdoc />
        public override string Name => Properties.Resources.IPodName;

        /// <inheritdoc />
        public override IEnumerable<SongInformation> GetTagInformation()
        {
            // see if this has the requisit parts
            var directory = new System.IO.DirectoryInfo(this.Folder);
            var controlDirectory = System.IO.Path.Combine(this.Folder, "iPod_Control");
            if (!System.IO.Directory.Exists(controlDirectory))
            {
                throw new System.IO.DirectoryNotFoundException($"Failed to find iPod Control folder under {this.Folder}");
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

                yield return new SongInformation(track.Title, track.Artist, track.SortArtist, track.Album, track.FileName, (int)track.Rating);
            }
        }
    }
}
