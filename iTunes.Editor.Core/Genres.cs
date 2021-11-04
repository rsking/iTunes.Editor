// <copyright file="Genres.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor;

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
            .Select(genre => CreateGenre(default, genre!))
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
                        internalGenre,
                        internalGenre is null ? internalGenres : internalGenre.SubGenres,
                        item);
                }
            }

            return internalGenres;
        }

        static Genre CreateGenre(Genre? parent, InternalGenre internalGenre)
        {
            var genre = new Genre(internalGenre.Name, parent, Array.Empty<Genre>());
            return genre with { SubGenres = internalGenre.SubGenres.ConvertAll(subGenre => CreateGenre(genre, subGenre)) };
        }

        static InternalGenre GetGenre(InternalGenre? parent, List<InternalGenre> genres, string name)
        {
            var index = genres.Count > 0
                ? genres.FindIndex(genre => string.Equals(genre.Name, name, StringComparison.Ordinal))
                : -1;
            if (index >= 0)
            {
                return genres[index];
            }

            var genre = new InternalGenre(name, parent);
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

    private sealed class InternalGenre
    {
        public InternalGenre(string name, InternalGenre? parent) => (this.Name, this.Parent) = (name, parent);

        public string Name { get; }

        public InternalGenre? Parent { get; }

        public List<InternalGenre> SubGenres { get; } = new List<InternalGenre>();
    }
}

/// <summary>
/// A Genre.
/// </summary>
/// <param name="Name">The name of the genre.</param>
/// <param name="Parent">The parent genre.</param>
/// <param name="SubGenres">The sub genres.</param>
public record Genre(string Name, Genre? Parent, IReadOnlyCollection<Genre> SubGenres)
{
    /// <inheritdoc/>
    public override string ToString()
    {
        var name = this.Name;
        var genre = this;
        while (genre.Parent is not null)
        {
            name = string.Concat(genre.Parent.Name, "|", name);
            genre = genre.Parent;
        }

        return name;
    }
}
