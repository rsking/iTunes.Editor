// <copyright file="Genres.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Static class for genres.
    /// </summary>
    public static class Genres
    {
        static Genres()
        {
            var genres = GetInternalGenres()
                .Select(genre => CreateGenre(genre!))
                .ToList();

            Music = genres.Find(genre => string.Equals(genre.Name, "Music", StringComparison.OrdinalIgnoreCase));
            MusicVideos = genres.Find(genre => string.Equals(genre.Name, "Music Videos", StringComparison.OrdinalIgnoreCase));
            Ringtones = genres.Find(genre => string.Equals(genre.Name, "Ringtones", StringComparison.OrdinalIgnoreCase));

            static IEnumerable<InternalGenre> GetInternalGenres()
            {
                var internalGenres = new List<InternalGenre>();

                foreach (var line in Split(Properties.Resources.Genres))
                {
                    InternalGenre? internalGenre = default;
                    foreach (var item in line.Split('|'))
                    {
                        internalGenre = GetGenre(
                            internalGenre is null ? internalGenres : internalGenre.SubGenres,
                            item);
                    }
                }

                return internalGenres;
            }

            static Genre CreateGenre(InternalGenre genre)
            {
                return new Genre(genre.Name, genre.SubGenres.ConvertAll(CreateGenre));
            }

            static InternalGenre GetGenre(List<InternalGenre> genres, string name)
            {
                var index = genres.Count > 0
                    ? genres.FindIndex(genre => string.Equals(genre.Name, name, StringComparison.Ordinal))
                    : -1;
                if (index >= 0)
                {
                    return genres[index];
                }

                var genre = new InternalGenre(name);
                genres.Add(genre);
                return genre;
            }

            static IEnumerable<string> Split(string input)
            {
                using var reader = new System.IO.StringReader(input);

                for (var line = reader.ReadLine(); line is not null; line = reader.ReadLine())
                {
                    yield return line;
                }
            }
        }

        /// <summary>
        /// Gets the music genres.
        /// </summary>
        public static Genre Music { get; }

        /// <summary>
        /// Gets the music video genres.
        /// </summary>
        public static Genre MusicVideos { get; }

        /// <summary>
        /// Gets the ringtone genres.
        /// </summary>
        public static Genre Ringtones { get; }

        private class InternalGenre
        {
            public InternalGenre(string name) => this.Name = name;

            public string Name { get; }

            public List<InternalGenre> SubGenres { get; } = new List<InternalGenre>();
        }
    }

    /// <summary>
    /// A Genre.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "This is justified in this case")]
    public record Genre(string Name, IReadOnlyCollection<Genre> SubGenres);
}
