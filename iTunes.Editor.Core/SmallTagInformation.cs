// -----------------------------------------------------------------------
// <copyright file="SmallTagInformation.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// The small tag information.
    /// </summary>
    public class SmallTagInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SmallTagInformation"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="performers">The performers.</param>
        /// <param name="sortPerformers">The sort performers.</param>
        /// <param name="album">The album.</param>
        /// <param name="name">The file name.</param>
        /// <param name="rating">The rating.</param>
        public SmallTagInformation(string title, string performers, string sortPerformers, string album, string name, int? rating = null)
        {
            this.Title = title;
            this.Performers = performers;
            this.SortPerformers = sortPerformers;
            this.Album = album;
            this.Name = name;
            this.Rating = rating;
        }

        /// <summary>
        /// Gets the sort performers.
        /// </summary>
        public string SortPerformers { get; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the performers.
        /// </summary>
        public string Performers { get; }

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
        /// Converts a <see cref="TagLib.File"/> to a <see cref="SmallTagInformation"/>.
        /// </summary>
        /// <param name="file">The file to convert.</param>
        public static explicit operator SmallTagInformation(TagLib.File file)
        {
            return new SmallTagInformation(
                file.Tag.Title,
                file.Tag.JoinedPerformers,
                file.Tag.JoinedPerformersSort ?? file.Tag.JoinedPerformers,
                file.Tag.Album,
                file.Name);
        }
    }
}
