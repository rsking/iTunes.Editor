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
    public class SongInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SongInformation"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="performers">The performers.</param>
        /// <param name="sortPerformers">The sort performers.</param>
        /// <param name="album">The album.</param>
        /// <param name="name">The file name.</param>
        /// <param name="rating">The rating.</param>
        public SongInformation(string title, string performers, string sortPerformers, string album, string name, int? rating = null)
            : this(
                title,
                performers?.Split(';').Select(_ => _.Trim()).ToArray() ?? Enumerable.Empty<string>(),
                sortPerformers?.Split(';').Select(_ => _.Trim()).ToArray() ?? Enumerable.Empty<string>(),
                album,
                name,
                rating)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SongInformation"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="performers">The performers.</param>
        /// <param name="sortPerformers">The sort performers.</param>
        /// <param name="album">The album.</param>
        /// <param name="name">The file name.</param>
        /// <param name="rating">The rating.</param>
        public SongInformation(
            string title,
            System.Collections.Generic.IEnumerable<string> performers,
            System.Collections.Generic.IEnumerable<string> sortPerformers,
            string album,
            string name,
            int? rating = null)
        {
            this.Title = title;
            this.Performers = performers;
            this.SortPerformers = sortPerformers;
            this.Album = album;
            this.Name = name;
            this.Rating = rating;
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the performers.
        /// </summary>
        public System.Collections.Generic.IEnumerable<string> Performers { get; }

        /// <summary>
        /// Gets the sort performers.
        /// </summary>
        public System.Collections.Generic.IEnumerable<string> SortPerformers { get; }

        /// <summary>
        /// Gets the album.
        /// </summary>
        public string Album { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the rating.
        /// </summary>
        public int? Rating { get; }

        /// <summary>
        /// Converts a <see cref="TagLib.File"/> to a <see cref="SongInformation"/>.
        /// </summary>
        /// <param name="file">The file to convert.</param>
        public static explicit operator SongInformation(TagLib.File file) => new SongInformation(
            file.Tag.Title,
            file.Tag.Performers,
            file.Tag.PerformersSort ?? file.Tag.Performers,
            file.Tag.Album,
            file.Name);

        /// <summary>
        /// Creates a new <see cref="SongInformation" /> from a file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The song information.</returns>
        public static SongInformation FromFile(string path)
        {
            using (var fileAbstraction = new LocalFileAbstraction(path))
            {
                using (var tagLibFile = TagLib.File.Create(fileAbstraction))
                {
                    return (SongInformation)tagLibFile;
                }
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var performers = this.Performers == null ? null : string.Join("; ", this.Performers);
            return $"{performers}|{this.Album}|{this.Title}";
        }
    }
}
