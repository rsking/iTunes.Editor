// -----------------------------------------------------------------------
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

        private static readonly TagLib.ByteVector Rating = new TagLib.ByteVector(new byte[] { 114, 116, 110, 103 });
        private static readonly TagLib.ByteVector ExplicitRatingData = new TagLib.ByteVector(new byte[] { 0x04 });
        private static readonly TagLib.ByteVector CleanRatingData = new TagLib.ByteVector(new byte[] { 0x02 });
        private static readonly TagLib.ByteVector UnratedRatingData = new TagLib.ByteVector(new byte[] { 0x00 });

        /// <summary>
        /// Gets the media type.
        /// </summary>
        /// <param name="songInformation">The song information.</param>
        /// <returns>The media type.</returns>
        public static string? GetMediaType(this SongInformation songInformation)
        {
            if (songInformation?.Name is null)
            {
                return null;
            }

            var name = songInformation.Name;
            var values = name.Split(System.IO.Path.DirectorySeparatorChar);

            for (var i = 0; i < values.Length; i++)
            {
                if (values[i] == "iTunes Media")
                {
                    if (i + 1 < values.Length)
                    {
                        return values[i + 1];
                    }

                    break;
                }
            }

            return null;
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

            if (lyrics != appleTag.Lyrics)
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
        public static string? CleanLyrics(this string? lyrics, string newLine = "\r") => string.IsNullOrEmpty(lyrics)
                ? null
                : string.Join(
                    newLine,
                    lyrics!
                        .SplitLines()
                        .Select(line => line.Trim().Capitalize())
                        .RemoveMultipleNull());

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
            return original == titleCase;
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
        /// Removes the "no lyrics" flag from the grouping.
        /// </summary>
        /// <param name="appleTag">The apple tag.</param>
        /// <returns>Returns <see langword="true"/> if the "no lyics" has been removed; otherwise <see langword="false"/>.</returns>
        public static bool RemoveNoLyrics(this TagLib.Tag appleTag) => appleTag.RemoveTag(NoLyrics, HasLyrics);

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

            var lyrics = tag.Lyrics;
            if (string.IsNullOrEmpty(lyrics))
            {
                if (tag is TagLib.Mpeg4.AppleTag appleTag)
                {
                    return appleTag.SetUnrated();
                }
            }
            else if (tag is TagLib.Mpeg4.AppleTag appleTag && !appleTag.HasRating())
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

            if (string.IsNullOrEmpty(appleTag.Grouping))
            {
                appleTag.Grouping = tag;
                return true;
            }

            var grouping = appleTag.Grouping.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
            if (grouping.Contains(tag))
            {
                return false;
            }

            Array.Resize(ref grouping, grouping.Length + 1);
            grouping[grouping.Length - 1] = tag;

            var update = string.Join("; ", grouping);

            if (update != appleTag.Grouping)
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

            if (!string.IsNullOrEmpty(appleTag.Grouping) && appleTag.Grouping.Contains(tag))
            {
                var grouping = appleTag.Grouping.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
                var index = -1;

                // See if No Lyrics is already in there
                for (var j = 0; j < grouping.Length; j++)
                {
                    if (grouping[j] == tag)
                    {
                        index = j;
                        break;
                    }
                }

                if (index >= 0)
                {
                    var groupingList = new List<string>(grouping);
                    groupingList.RemoveAt(index);
                    appleTag.Grouping = groupingList.Count == 0 ? replacement : string.Join("; ", groupingList);
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<string> RemoveMultipleNull(this IEnumerable<string> source)
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

        private static IEnumerable<string> SplitLines(this string lyrics)
        {
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
