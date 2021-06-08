// -----------------------------------------------------------------------
// <copyright file="SongInformation.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using System.Linq;

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
        public System.Collections.Generic.IEnumerable<string> Performers { get; init; } = Enumerable.Empty<string>();

        /// <summary>
        /// Gets the sort performers.
        /// </summary>
        public System.Collections.Generic.IEnumerable<string> SortPerformers { get; init; } = Enumerable.Empty<string>();

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
        /// Converts a <see cref="TagLib.File"/> to a <see cref="SongInformation"/>.
        /// </summary>
        /// <param name="file">The file to convert.</param>
        public static explicit operator SongInformation(TagLib.File file) => file is null
            ? throw new System.ArgumentNullException(nameof(file))
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
            };

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
        public static System.Threading.Tasks.Task<SongInformation> FromFileAsync(string path, System.Threading.CancellationToken cancellationToken = default) =>
            System.Threading.Tasks.Task.Run(
                () => FromFile(path),
                cancellationToken);

        /// <inheritdoc/>
        public override string ToString()
        {
            var performers = this.Performers?.ToJoinedString();
            return $"{performers}|{this.Album}|{this.Title}";
        }
    }
}
