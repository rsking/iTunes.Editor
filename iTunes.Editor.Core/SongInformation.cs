// -----------------------------------------------------------------------
// <copyright file="SongInformation.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor;

/// <summary>
/// The song information.
/// </summary>
public record SongInformation
{
    /// <summary>
    /// An empty song information.
    /// </summary>
    public static readonly SongInformation Empty = new(string.Empty);

    /// <summary>
    /// Initializes a new instance of the <see cref="SongInformation"/> class.
    /// </summary>
    /// <param name="title">The title.</param>
    public SongInformation(string title) => this.Title = title;

    /// <summary>
    /// Gets the title.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// Gets the performers.
    /// </summary>
    public IEnumerable<string> Performers { get; init; } = [];

    /// <summary>
    /// Gets the sort performers.
    /// </summary>
    public IEnumerable<string> SortPerformers { get; init; } = [];

    /// <summary>
    /// Gets the album performers.
    /// </summary>
    public string? AlbumPerformer { get; init; }

    /// <summary>
    /// Gets the sort album performers.
    /// </summary>
    public string? SortAlbumPerformer { get; init; }

    /// <summary>
    /// Gets the album.
    /// </summary>
    public string? Album { get; init; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the number.
    /// </summary>
    public int? Number { get; init; }

    /// <summary>
    /// Gets the total.
    /// </summary>
    public int? Total { get; init; }

    /// <summary>
    /// Gets the disk.
    /// </summary>
    public int? Disc { get; init; }

    /// <summary>
    /// Gets the rating.
    /// </summary>
    public int? Rating { get; init; }

    /// <summary>
    /// Gets the genre.
    /// </summary>
    public string? Genre { get; init; }

    /// <summary>
    /// Gets a value indicating whether this instance has lyrics.
    /// </summary>
    public bool? HasLyrics { get; init; }

    /// <summary>
    /// Gets a value indicating whether this instance is part of a compilation.
    /// </summary>
    public bool? IsCompilation { get; init; }

    /// <summary>
    /// Converts a <see cref="TagLib.File"/> to a <see cref="SongInformation"/>.
    /// </summary>
    /// <param name="file">The file to convert.</param>
    public static explicit operator SongInformation(TagLib.File file)
    {
        return file is null
            ? throw new ArgumentNullException(nameof(file))
            : new SongInformation(file.Tag.Title)
            {
                Performers = file.Tag.Performers,
                SortPerformers = file.Tag.PerformersSort ?? file.Tag.Performers,
                AlbumPerformer = file.Tag.AlbumArtists.ToJoinedString(),
                SortAlbumPerformer = file.Tag.AlbumArtistsSort.ToJoinedString(),
                Album = file.Tag.Album,
                Name = file.Name,
                Genre = file.Tag.Genres.ToJoinedString(),
                HasLyrics = !string.IsNullOrEmpty(file.Tag.Lyrics),
                Number = GetValue(file.Tag.Track),
                Total = GetValue(file.Tag.TrackCount),
                Disc = GetValue(file.Tag.Disc),
                IsCompilation = GetIsCompilation(file),
            };

        static int? GetValue(uint value)
        {
            return value is 0U ? null : (int)value;
        }

        static bool? GetIsCompilation(TagLib.File file)
        {
            if (file.GetTag(TagLib.TagTypes.Apple, create: false) is TagLib.Mpeg4.AppleTag appleTag)
            {
                return appleTag.IsCompilation;
            }

            return default;
        }
    }

    /// <summary>
    /// Creates a new <see cref="SongInformation" /> from a file.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The song information.</returns>
    public static SongInformation FromFile(string path)
    {
        var fileAbstraction = new TagLib.File.LocalFileAbstraction(path);
        using var tagLibFile = TagLib.File.Create(fileAbstraction);
        return (SongInformation)tagLibFile;
    }

    /// <summary>
    /// Creates a new <see cref="SongInformation" /> from a file.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The song information.</returns>
    public static Task<SongInformation> FromFileAsync(string path, CancellationToken cancellationToken = default) => Task.Run(() => FromFile(path), cancellationToken);

    /// <inheritdoc/>
    public override string ToString() => $"{this.GetPerformer()}|{this.Album}|{this.Title}";

    /// <summary>
    /// Gets the performer.
    /// </summary>
    /// <returns>The performer.</returns>
    public string GetPerformer() => this.IsCompilation is not true && this.AlbumPerformer is { } albumPerformer ? albumPerformer : this.Performers.ToJoinedString();
}
