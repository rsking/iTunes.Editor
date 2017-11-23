// -----------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

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

        private const string HasLyrics = "Has Lyrics";

        /// <summary>
        /// Cleans the lyrics.
        /// </summary>
        /// <param name="appleTag">The apple tag.</param>
        /// <returns>Returns <see langword="true"/> if the lyics have been cleaned; otherwise <see langword="false"/>.</returns>
        public static bool CleanLyrics(this TagLib.Tag appleTag)
        {
            string lyrics = null;
            if (string.IsNullOrEmpty(appleTag.Lyrics))
            {
                lyrics = null;
            }
            else
            {
                string.Join(
                    Environment.NewLine,
                    appleTag.Lyrics
                        .SplitLines()
                        .Select(line => line.Trim().Capitalize())
                        .RemoveMultipleNull());
            }

            if (lyrics != appleTag.Lyrics)
            {
                appleTag.Lyrics = lyrics;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds the "no lyrics" flag to the grouping.
        /// </summary>
        /// <param name="appleTag">The apple tag.</param>
        /// <returns>Returns <see langword="true"/> if the "no lyics" has been added; otherwise <see langword="false"/>.</returns>
        public static bool AddNoLyrics(this TagLib.Tag appleTag)
        {
            if (string.IsNullOrEmpty(appleTag.Grouping))
            {
                appleTag.Grouping = NoLyrics;
                return true;
            }

            var grouping = appleTag.Grouping.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
            if (grouping.Contains(NoLyrics))
            {
                return false;
            }

            Array.Resize(ref grouping, grouping.Length + 1);
            grouping[grouping.Length - 1] = NoLyrics;

            var update = string.Join("; ", grouping);

            if (update != appleTag.Grouping)
            {
                appleTag.Grouping = update;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the "no lyrics" flag from the grouping.
        /// </summary>
        /// <param name="appleTag">The apple tag.</param>
        /// <returns>Returns <see langword="true"/> if the "no lyics" has been removed; otherwise <see langword="false"/>.</returns>
        public static bool RemoveNoLyrics(this TagLib.Tag appleTag)
        {
            if (!string.IsNullOrEmpty(appleTag.Grouping) && appleTag.Grouping.Contains(NoLyrics))
            {
                var grouping = appleTag.Grouping.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
                int index = -1;

                // See if No Lyrics is already in there
                for (int j = 0; j < grouping.Length; j++)
                {
                    if (grouping[j] == NoLyrics)
                    {
                        index = j;
                        break;
                    }
                }

                if (index >= 0)
                {
                    var groupingList = new List<string>(grouping);
                    groupingList.RemoveAt(index);
                    if (groupingList.Count == 0)
                    {
                        appleTag.Grouping = HasLyrics;
                    }
                    else
                    {
                        appleTag.Grouping = string.Join("; ", groupingList.ToArray());
                    }

                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<string> RemoveMultipleNull(this IEnumerable<string> source)
        {
            bool last = false;
            foreach (string item in source)
            {
                bool current = string.IsNullOrEmpty(item);
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

            for (int i = 0; i < lyrics.Length; i++)
            {
                char character = lyrics[i];
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
                        if (i <= 0 || !(lyrics[i - 1] == '\r'))
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
        }

        private static string Capitalize(this string line)
        {
            return line.Length <= 0 || !char.IsLower(line[0])
                ? line :
                line.Substring(0, 1).ToUpper() + line.Substring(1);
        }
    }
}
