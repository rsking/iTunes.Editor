// -----------------------------------------------------------------------
// <copyright file="ITunesSongsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.ITunesLib;

using System.Linq;

/// <summary>
/// The iTunes song loader.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "This is for iTunes")]
public class ITunesSongsProvider : ISongsProvider
{
    private readonly IConfigurator<ITunesSongsProvider> configurator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ITunesSongsProvider"/> class.
    /// </summary>
    /// <param name="configurator">The configurator.</param>
    public ITunesSongsProvider(IConfigurator<ITunesSongsProvider> configurator) => this.configurator = configurator;

    /// <inheritdoc />
    public string Name => Properties.Resources.ITunesName;

    /// <summary>
    /// Gets the playlists.
    /// </summary>
    public System.Collections.Generic.IEnumerable<string> Playlists { get; private set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets the selected playlist.
    /// </summary>
    public string? SelectedPlaylist { get; set; }

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

    /// <summary>
    /// Gets or sets a value indicating whether to update the comments.
    /// </summary>
    public bool UpdateComments { get; set; }

    /// <inheritdoc />
#if COM_AVAILABLE
    public async System.Collections.Generic.IAsyncEnumerable<SongInformation> GetTagInformationAsync([System.Runtime.CompilerServices.EnumeratorCancellation] System.Threading.CancellationToken cancellationToken = default)
    {
        var librarySource = await System.Threading.Tasks.Task.Run(() =>
        {
            var app = new iTunesLib.iTunesApp();
            return app.LibrarySource;
        }).ConfigureAwait(false);

        var playlists = new string[librarySource.Playlists.Count];
        var index = 0;
        foreach (var pl in librarySource.Playlists
            .OfType<iTunesLib.IITPlaylist>())
        {
            playlists[index] = pl.Name;
            index++;
        }

        this.Playlists = playlists;
        var configured = await this.configurator.ConfigureAsync(this).ConfigureAwait(false);
        if (!configured)
        {
            yield break;
        }

        var library = this.SelectedPlaylist is null
            ? await GetLibraryPlaylist().ConfigureAwait(false)
            : librarySource.Playlists.ItemByName[this.SelectedPlaylist];

        for (var i = 1; i <= library.Tracks.Count; i++)
        {
            if (library.Tracks[i] is iTunesLib.IITFileOrCDTrack track)
            {
                SongInformation? songInformation;

                try
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

                    if (this.UpdateComments)
                    {
                        const string By = nameof(By) + " ";
                        const string From = nameof(From) + " ";
                        const string Produced = nameof(Produced) + " ";

                        var original = track.Comment;
                        var comment = original switch
                        {
                            _ when original.StartsWith(By, System.StringComparison.Ordinal) => LowerPrefix(original, By),
                            _ when original.StartsWith(From, System.StringComparison.Ordinal) => LowerPrefix(original, From),
                            _ when original.StartsWith(Produced, System.StringComparison.Ordinal) => LowerPrefix(original, Produced),
                            _ => original,
                        };

                        if (!string.Equals(comment, original, System.StringComparison.Ordinal))
                        {
                            track.Comment = comment ?? string.Empty;
                        }

                        static string LowerPrefix(string original, string prefix)
                        {
                            return string.Concat(
                                prefix.ToLowerInvariant(),
#if NETSTANDARD2_1_OR_GREATER
                                    original[prefix.Length..]);
#else
                                    original.Substring(prefix.Length));
#endif
                        }
                    }

                    var artist = track.Artist.FromJoinedString().ToArray();
                    songInformation = new SongInformation(track.Name)
                    {
                        Performers = artist,
                        SortPerformers = track.SortArtist?.FromJoinedString().ToArray() ?? artist,
                        AlbumPerformer = track.AlbumArtist,
                        SortAlbumPerformer = track.SortAlbumArtist,
                        Album = track.Album,
                        Name = track.Location,
                        Genre = track.Genre,
                        Rating = track.Rating,
                        HasLyrics = hasLyrics,
                    };
                }
                catch (System.Runtime.InteropServices.COMException exception) when (exception.Message
#if NETSTANDARD2_1_OR_GREATER
                        .Contains("deleted", System.StringComparison.Ordinal))
#else
                        .Contains("deleted"))
#endif
                {
                    // this has been removed from the playlist, so ignore it.
                    continue;
                }

                yield return songInformation;
            }
        }

        static System.Threading.Tasks.Task<iTunesLib.IITPlaylist> GetLibraryPlaylist()
        {
            return System.Threading.Tasks.Task.Run<iTunesLib.IITPlaylist>(() =>
            {
                var app = new iTunesLib.iTunesApp();
                return app.LibraryPlaylist;
            });
        }
    }
#else
        public System.Collections.Generic.IAsyncEnumerable<SongInformation> GetTagInformationAsync(System.Threading.CancellationToken cancellationToken = default) => throw new System.NotSupportedException("Compiled without iTunes support");
#endif
}
