// -----------------------------------------------------------------------
// <copyright file="IPodSongsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.IPod;

using global::IPod;

/// <summary>
/// The iPod <see cref="ISongsProvider"/>.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "This is for iPod")]
public class IPodSongsProvider : ISongsProvider, IFolderProvider
{
    /// <inheritdoc />
    public string? Folder { get; set; }

    /// <inheritdoc />
    public string Name => Properties.Resources.IPodName;

    /// <inheritdoc />
    public async IAsyncEnumerable<SongInformation> GetTagInformationAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // see if this has the requisit parts
        var controlDirectory = Path.Combine(this.Folder, "iPod_Control");
        if (!Directory.Exists(controlDirectory))
        {
            throw new DirectoryNotFoundException($"Failed to find iPod Control folder under {this.Folder}");
        }

        var iTunesDB = Path.Combine(controlDirectory, "iTunes\\iTunesDB");

        if (!File.Exists(iTunesDB))
        {
            throw new FileNotFoundException($"Failed to find iTunesDB file under {controlDirectory}");
        }

        // read in the database
        var trackDatabase = await Task.Run(() => new TrackDatabase(iTunesDB), cancellationToken).ConfigureAwait(false);
        var count = trackDatabase.Tracks.Count;

        for (var i = 0; i < count; i++)
        {
            var track = trackDatabase.Tracks[i];

            yield return new SongInformation(track.Title)
            {
                Performers = track.Artist.FromJoinedString().ToArray(),
                SortPerformers = track.SortArtist.FromJoinedString().ToArray(),
                AlbumPerformer = track.AlbumArtist,
                SortAlbumPerformer = track.AlbumArtist,
                Album = track.Album,
                Name = track.FileName,
                Genre = track.Genre,
                Rating = (int)track.Rating,
                HasLyrics = !track.Grouping.HasNoLyrics(),
                Number = track.TrackNumber is 0 ? null : track.TrackNumber,
                Total = track.TotalTracks is 0 ? null : track.TotalTracks,
                Disc = track.DiscNumber is 0 ? null : track.DiscNumber,
            };
        }
    }
}
