﻿// -----------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ITunes.Editor.Core.Tests")]

namespace ITunes.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        private const string NoLyrics = "No Lyrics";

        private const string TitleCased = "Lyrics Title Cased";

        private const string HasLyrics = "Has Lyrics";

        private static readonly TagLib.ByteVector Rating = new(new byte[] { 114, 116, 110, 103 });
        private static readonly TagLib.ByteVector ExplicitRatingData = new(new byte[] { 0x04 });
        private static readonly TagLib.ByteVector CleanRatingData = new(new byte[] { 0x02 });
        private static readonly TagLib.ByteVector UnratedRatingData = new(new byte[] { 0x00 });

        /// <summary>
        /// Gets the media type.
        /// </summary>
        /// <param name="songInformation">The song information.</param>
        /// <returns>The media type.</returns>
        public static MediaKind GetMediaKind(this SongInformation songInformation)
        {
            if (songInformation?.Name is null)
            {
                return MediaKind.Unknown;
            }

            return songInformation.Name.GetMediaKind();
        }

        /// <summary>
        /// Gets the media type.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The media type.</returns>
        public static MediaKind GetMediaKind(this string path)
        {
            var values = path.Split(System.IO.Path.DirectorySeparatorChar);

            for (var i = 0; i < values.Length; i++)
            {
                if (string.Equals(values[i], "iTunes Media", StringComparison.Ordinal))
                {
                    if (i + 1 < values.Length)
                    {
                        return values[i + 1] switch
                        {
                            "Audiobooks" => MediaKind.Audiobook,
                            "Home Videos" => MediaKind.HomeVideo,
                            "Movies" => MediaKind.Movie,
                            "Music" => MediaKind.Song,
                            "TV Shows" => MediaKind.TVShow,
                            _ => MediaKind.Unknown,
                        };
                    }

                    break;
                }
            }

            return MediaKind.Unknown;
        }

        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal to the value of the specified <see cref="DateTime"/> instance, optionally ignoring the <see cref="DateTime.Kind"/> value.
        /// </summary>
        /// <param name="dateTime">This date time value.</param>
        /// <param name="other">The object to compare to this instance.</param>
        /// <param name="ignoreDateTimeKind">Set to <see langword="true"/> to ignore <see cref="DateTime.Kind"/>.</param>
        /// <returns><see langword="true"/> if the <paramref name="other"/> parameter equals the value of <paramref name="dateTime"/> instance; otherwise, <see langword="false"/>.</returns>
        public static bool Equals(this DateTime dateTime, DateTime other, bool ignoreDateTimeKind)
        {
            if (ignoreDateTimeKind)
            {
                return dateTime.Equals(other);
            }

            // check to see if these are "null" dates
            if (dateTime.IsNull() && other.IsNull())
            {
                return true;
            }

            return dateTime.Year == other.Year
                && dateTime.Month == other.Month
                && dateTime.Day == other.Day
                && dateTime.Hour == other.Hour
                && dateTime.Minute == other.Minute
                && dateTime.Second == other.Second
                && dateTime.Millisecond == other.Millisecond;
        }

        /// <summary>
        /// Gets a value indicating whether the tags contain the "no lyrics" tag.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <returns><see langword="true"/> if the tags contain the "no lyrics" tag; otherwise, <see langword="false"/>.</returns>
        public static bool HasNoLyrics(this string? tags) => tags
#if NETSTANDARD2_1_OR_GREATER
            ?.Contains(NoLyrics, StringComparison.Ordinal)
#else
            ?.Contains(NoLyrics)
#endif
            == true;

        /// <summary>
        /// Cleans the lyrics.
        /// </summary>
        /// <param name="appleTag">The apple tag.</param>
        /// <param name="newLine">The newline string.</param>
        /// <returns>Returns <see langword="true"/> if the lyics have been cleaned; otherwise <see langword="false"/>.</returns>
        public static bool CleanLyrics(this TagLib.Tag appleTag, string newLine = "\r")
        {
            if (appleTag is null)
            {
                return false;
            }

            var lyrics = appleTag.Lyrics.CleanLyrics(newLine);

            if (!string.Equals(lyrics, appleTag.Lyrics, StringComparison.Ordinal))
            {
                appleTag.Lyrics = lyrics;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Cleans the lyrics.
        /// </summary>
        /// <param name="lyrics">The lyrics.</param>
        /// <param name="newLine">The newline.</param>
        /// <returns>The cleaned lyrics.</returns>
        public static string? CleanLyrics(this string? lyrics, string newLine = "\r")
        {
            if (string.IsNullOrEmpty(lyrics))
            {
                return default;
            }

            return string.Join(
                    newLine,
                    RemoveMultipleNull(lyrics
                        .SplitLines()
                        .Select(line => line.Trim().Capitalize())));

            static IEnumerable<string?> RemoveMultipleNull(IEnumerable<string?> source)
            {
                var last = false;
                foreach (var item in source)
                {
                    var current = string.IsNullOrEmpty(item);
                    if (!current || !last)
                    {
                        last = current;
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the lyrics are title cased.
        /// </summary>
        /// <param name="appleTag">The tag.</param>
        /// <param name="newLine">The newline.</param>
        /// <returns><see langword="true"/> if <paramref name="appleTag"/><see cref="TagLib.Tag.Lyrics"/> are title cased; otherwise <see langword="false"/>. </returns>
        public static bool LyricsAreTitleCased(this TagLib.Tag appleTag, string newLine = "\r")
        {
            if (appleTag is null || appleTag.Lyrics is null || appleTag.Lyrics.Length == 0)
            {
                return false;
            }

            var original = appleTag.Lyrics;
            var titleCase = string.Join(newLine, original.SplitLines().Select(line => Humanizer.To.TitleCase.Transform(line)));
            return string.Equals(original, titleCase, StringComparison.Ordinal);
        }

        /// <summary>
        /// Adds the "title cased lyrics" flag to the grouping.
        /// </summary>
        /// <param name="appleTag">The apple tag.</param>
        /// <returns>Returns <see langword="true"/> if the "title cased lyrics" has been added; otherwise <see langword="false"/>.</returns>
        public static bool AddTitleCased(this TagLib.Tag appleTag) => appleTag.AddTag(TitleCased);

        /// <summary>
        /// Adds the "no lyrics" flag to the grouping.
        /// </summary>
        /// <param name="appleTag">The apple tag.</param>
        /// <returns>Returns <see langword="true"/> if the "no lyics" has been added; otherwise <see langword="false"/>.</returns>
        public static bool AddNoLyrics(this TagLib.Tag appleTag) => appleTag.AddTag(NoLyrics);

        /// <summary>
        /// Adds the "no lyrics" flag to the grouping.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <returns><paramref name="tags"/> with "no lyrics" added.</returns>
        public static string AddNoLyrics(this string? tags) => tags.AddTagImpl(NoLyrics);

        /// <summary>
        /// Removes the "no lyrics" flag from the grouping.
        /// </summary>
        /// <param name="appleTag">The apple tag.</param>
        /// <returns>Returns <see langword="true"/> if the "no lyics" has been removed; otherwise <see langword="false"/>.</returns>
        public static bool RemoveNoLyrics(this TagLib.Tag appleTag) => appleTag.RemoveTag(NoLyrics, HasLyrics);

        /// <summary>
        /// Removes the "no lyrics" flag from the tags.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <returns><paramref name="tags"/> with "no lyrics" removed.</returns>
        public static string? RemoveNoLyrics(this string? tags) => tags.RemoveTagImpl(NoLyrics);

        /// <summary>
        /// Removes the "has lyrics" flag from the tags.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <returns><paramref name="tags"/> with "has lyrics" removed.</returns>
        public static string? RemoveHasLyrics(this string? tags) => tags.RemoveTagImpl(HasLyrics);

        /// <summary>
        /// Updates the explicit value.
        /// </summary>
        /// <param name="tag">The tag to update.</param>
        /// <param name="explicitLyricsProvider">The explicit lyrics provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns <see langword="true"/> if the explicit flag is updates; otherwise <see langword="false"/>.</returns>
        public static async System.Threading.Tasks.Task<bool> UpdateRatingAsync(this TagLib.Tag tag, IExplicitLyricsProvider explicitLyricsProvider, System.Threading.CancellationToken cancellationToken)
        {
            if (tag is null)
            {
                return false;
            }

            if (tag is TagLib.Mpeg4.AppleTag appleTag)
            {
                var lyrics = tag.Lyrics;

                if (string.IsNullOrEmpty(lyrics))
                {
                    return appleTag.SetUnrated();
                }

                if (!appleTag.HasRating())
                {
                    if (explicitLyricsProvider is null)
                    {
                        return false;
                    }

                    var @explicit = await explicitLyricsProvider.IsExplicitAsync(lyrics, cancellationToken).ConfigureAwait(false);
                    if (@explicit.HasValue)
                    {
                        return @explicit.Value ? appleTag.SetExplicit() : appleTag.SetClean();
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="TagLib.Mpeg4.AppleTag"/> has rating data.
        /// </summary>
        /// <param name="appleTag">The apple tag.</param>
        /// <returns><see langword="true"/> if <paramref name="appleTag"/> has rating data; otherwise <see langword="false"/>.</returns>
        public static bool HasRating(this TagLib.Mpeg4.AppleTag appleTag)
        {
            if (appleTag is null)
            {
                return false;
            }

            var dataBoxes = appleTag.DataBoxes(Rating);
            return dataBoxes.Any()
                && dataBoxes.All(box => box.Data != UnratedRatingData || box.Flags != (uint)TagLib.Mpeg4.AppleDataBox.FlagType.ContainsData);
        }

        /// <summary>
        /// Sets the rating flag to explicit.
        /// </summary>
        /// <param name="appleTag">The apple tag.</param>
        /// <returns>Returns <see langword="true"/> if the explicit flag is updated; otherwise <see langword="false"/>.</returns>
        public static bool SetExplicit(this TagLib.Mpeg4.AppleTag appleTag) => appleTag?.SetRating(ExplicitRatingData) ?? false;

        /// <summary>
        /// Sets the rating flag to clean.
        /// </summary>
        /// <param name="appleTag">The apple tag.</param>
        /// <returns>Returns <see langword="true"/> if the clean flag is updated; otherwise <see langword="false"/>.</returns>
        public static bool SetClean(this TagLib.Mpeg4.AppleTag appleTag) => appleTag?.SetRating(CleanRatingData) ?? false;

        /// <summary>
        /// Sets the rating flag to unrated.
        /// </summary>
        /// <param name="appleTag">The apple tag.</param>
        /// <returns>Returns <see langword="true"/> if the unrated flag is updated; otherwise <see langword="false"/>.</returns>
        public static bool SetUnrated(this TagLib.Mpeg4.AppleTag appleTag) => appleTag?.SetRating(UnratedRatingData) ?? false;

        /// <summary>
        /// Concatenates the members of the <see cref="IEnumerable{T}"/> collection of type <see cref="string"/>, using the specified separator between each member.
        /// </summary>
        /// <param name="source">A collection that contains the strings to concatenate.</param>
        /// <returns>A string that consists of the members of <paramref name="source"/> delimited by the separator string. -or- <see cref="string.Empty"/> if values has zero elements or all the elements of values are null. -or- <see langword="null" /> if <paramref name="source"/> is <see langword="null"/>.</returns>
        public static string ToJoinedString(this IEnumerable<string> source) => source.Join("; ")!;

        /// <summary>
        /// Splits a string into substrings.
        /// </summary>
        /// <param name="source">The string to split.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> collection of type <see cref="string"/> whose elements contain the substrings from this instance.</returns>
        public static IEnumerable<string> FromJoinedString(this string? source) => source?.Split(';').Select(_ => _.Trim()) ?? Enumerable.Empty<string>();

        /// <summary>
        /// Concatenates the members of the <see cref="IEnumerable{T}"/> collection of type <see cref="string"/>, using the specified separator between each member.
        /// </summary>
        /// <param name="source">A collection that contains the strings to concatenate.</param>
        /// <param name="separator">The string to use as a separator. <paramref name="separator"/> is included in the returned string only if <paramref name="source"/> has more than one element.</param>
        /// <returns>A string that consists of the members of <paramref name="source"/> delimited by the separator string. -or- <see cref="string.Empty"/> if values has zero elements or all the elements of values are null. -or- <see langword="null" /> if <paramref name="source"/> is <see langword="null"/>.</returns>
        public static string? Join(this IEnumerable<string?> source, string? separator) => source is null ? null : string.Join(separator, source);

        /// <summary>
        /// Concatenates the members of the <see cref="IEnumerable{T}"/> collection, using the specified separator between each member.
        /// </summary>
        /// <typeparam name="T">The type of the members of values.</typeparam>
        /// <param name="source">A collection that contains the objects to concatenate.</param>
        /// <param name="separator">The string to use as a separator. <paramref name="separator"/> is included in the returned string only if <paramref name="source"/> has more than one element.</param>
        /// <returns>A string that consists of the members of <paramref name="source"/> delimited by the separator string. -or- <see cref="string.Empty"/> if values has zero elements or all the elements of values are null. -or- <see langword="null" /> if <paramref name="source"/> is <see langword="null"/>.</returns>
        public static string? Join<T>(this IEnumerable<T> source, string? separator) => source is null ? null : string.Join(separator, source);

        /// <summary>
        /// Sets the path for the <see cref="ISongsProvider"/>.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="path">The path.</param>
        /// <returns>The songs provider.</returns>
        public static ISongsProvider SetPath(this ISongsProvider provider, System.IO.FileSystemInfo path)
        {
            switch (provider)
            {
                case IFolderProvider folderProvider:
                    folderProvider.Folder = path.FullName;
                    break;
                case IFileProvider fileProvider:
                    fileProvider.File = path.FullName;
                    break;
            }

            return provider;
        }

        /// <summary>
        /// Produces the set difference of two sequences.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
        /// <typeparam name="TKey">The type of the elements of the test sequence.</typeparam>
        /// <param name="first">An <see cref="IEnumerable{T}"/> whose elements that are not also in <paramref name="second"/> will be returned.</param>
        /// <param name="second">An <see cref="IEnumerable{T}"/> whose elements that also occur in the first sequence will cause those elements to be removed from the returned sequence.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
        public static IEnumerable<TSource> ExceptBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector) => ExceptBy(first, second, keySelector, comparer: null);

        /// <summary>
        /// Produces the set difference of two sequences.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
        /// <typeparam name="TKey">The type of the elements of the test sequence.</typeparam>
        /// <param name="first">An <see cref="IEnumerable{T}"/> whose elements that are not also in <paramref name="second"/> will be returned.</param>
        /// <param name="second">An <see cref="IEnumerable{T}"/> whose elements that also occur in the first sequence will cause those elements to be removed from the returned sequence.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="comparer">The equality comparer.</param>
        /// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
        public static IEnumerable<TSource> ExceptBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
        {
            if (first is null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second is null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            if (keySelector is null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            return ExceptByIterator(first, second, keySelector, comparer);
        }

        /// <summary>
        /// Returns a value indicating whether a specified substring occurs within this string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="value">The string to seek.</param>
        /// <returns><see langword="true"/> if <paramref name="input"/> is not <see langword="null"/> and the value parameter occurs within this string, or if value is the empty string (""); otherwise, <see langword="false"/>.</returns>
        public static bool IsNotNullAndContains([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] this string? input, string value)
        {
            if (input is null)
            {
                return false;
            }

            return
#if NETSTANDARD2_1_OR_GREATER
                    input.Contains(value, StringComparison.Ordinal);
#else
                    input.Contains(value);
#endif
        }

        /// <summary>
        /// Returns a new string in which all occurrences of a specified string in the input are replaced with another specified string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="oldValue">The string to be replaced.</param>
        /// <param name="newValue">The string to replace all occurrences of <paramref name="oldValue"/>.</param>
        /// <returns>A string that is equivalent to <paramref name="input"/> except that all instances of <paramref name="oldValue"/> are replaced with <paramref name="newValue"/>. If oldValue is not found in the current instance, the method returns the current instance unchanged.</returns>
        public static string? SafeReplace(this string? input, string oldValue, string? newValue)
        {
            if (input is null)
            {
                return default;
            }

            return input
#if NETSTANDARD2_1_OR_GREATER
                .Replace(oldValue, newValue, StringComparison.Ordinal);
#else
                .Replace(oldValue, newValue);
#endif
        }

        private static IEnumerable<TSource> ExceptByIterator<TSource, TKey>(IEnumerable<TSource> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
        {
            var set = new HashSet<TKey>(second, comparer);

            foreach (var element in first)
            {
                if (set.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        private static string AddTagImpl(this string? tags, string tag)
        {
            if (tags is null || string.IsNullOrEmpty(tags))
            {
                return tag;
            }

            if (tags
#if NETSTANDARD2_1_OR_GREATER
                .Contains(tag, StringComparison.Ordinal))
#else
                .Contains(tag))
#endif
            {
                return tags;
            }

            var grouping = tags.FromJoinedString().ToList();
            if (grouping.Contains(tag, StringComparer.Ordinal))
            {
                return tags;
            }

            grouping.Add(tag);

            return grouping.ToJoinedString();
        }

        private static string? RemoveTagImpl(this string? tags, string tag)
        {
            if (tags.IsNotNullAndContains(tag))
            {
                var grouping = tags.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
                var index = -1;

                // See if the tag is already in there
                for (var j = 0; j < grouping.Length; j++)
                {
                    if (string.Equals(grouping[j], tag, StringComparison.Ordinal))
                    {
                        index = j;
                        break;
                    }
                }

                if (index >= 0)
                {
                    var groupingList = new List<string>(grouping);
                    groupingList.RemoveAt(index);
                    return groupingList.Count == 0
                        ? default
                        : groupingList.ToJoinedString();
                }
            }

            return tags;
        }

        private static bool SetRating(this TagLib.Mpeg4.AppleTag appleTag, TagLib.ByteVector rating)
        {
            var rtng = appleTag.DataBoxes(Rating);
            if (rtng.Any(box => box.Data == rating && box.Flags == (uint)TagLib.Mpeg4.AppleDataBox.FlagType.ContainsData))
            {
                return false;
            }

            appleTag.SetData(Rating, new[] { new TagLib.Mpeg4.AppleDataBox(rating, (uint)TagLib.Mpeg4.AppleDataBox.FlagType.ContainsData) });
            return true;
        }

        private static bool AddTag(this TagLib.Tag appleTag, string tag)
        {
            if (appleTag is null)
            {
                return false;
            }

            var original = appleTag.Grouping;
            var update = original.AddTagImpl(tag);
            if (!string.Equals(update, original, StringComparison.Ordinal))
            {
                appleTag.Grouping = update;
                return true;
            }

            return false;
        }

        private static bool RemoveTag(this TagLib.Tag appleTag, string tag, string replacement)
        {
            if (appleTag is null)
            {
                return false;
            }

            var original = appleTag.Grouping;
            var updated = original.RemoveTagImpl(tag);
            if (!string.Equals(original, updated, StringComparison.Ordinal))
            {
                appleTag.Grouping = updated ?? replacement;
                return true;
            }

            return false;
        }

        private static IEnumerable<string> SplitLines(this string? lyrics)
        {
            if (lyrics is null)
            {
                yield break;
            }

            var returnString = new System.Text.StringBuilder();

            // find the first and last actual characters
            var first = 0;
            for (var i = 0; i < lyrics.Length; i++)
            {
                if (!char.IsWhiteSpace(lyrics[i]))
                {
                    first = i;
                    break;
                }
            }

            var last = lyrics.Length - 1;
            for (var i = last; i >= 0; i--)
            {
                if (!char.IsWhiteSpace(lyrics[i]))
                {
                    last = i;
                    break;
                }
            }

            for (var i = first; i <= last; i++)
            {
                var character = lyrics[i];
                switch (character)
                {
                    case '\r':
                        if (i + 1 < lyrics.Length && lyrics[i + 1] == '\n')
                        {
                            i++;
                        }

                        yield return returnString.ToString();
                        returnString.Clear();
                        break;
                    case '\n':
                        if (i <= 0 || lyrics[i - 1] != '\r')
                        {
                            yield return returnString.ToString();
                            returnString.Clear();
                        }

                        break;
                    default:
                        returnString.Append(character);
                        break;
                }
            }

            yield return returnString.ToString();
        }

        private static string Capitalize(this string line) => Humanizer.To.SentenceCase.Transform(line);

        private static bool IsNull(this DateTime dateTime) => dateTime.Year == 1 || dateTime.Year == 1899;
    }
}
