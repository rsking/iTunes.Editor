// -----------------------------------------------------------------------
// <copyright file="MediaInfoTag.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.MediaInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TagLib;

    /// <summary>
    /// The <see cref="MediaInfo"/> <see cref="Tag"/>.
    /// </summary>
    public class MediaInfoTag : Tag
    {
        /// <summary>
        /// The lookup.
        /// </summary>
        private readonly IDictionary<string, IDictionary<string, string>> lookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaInfoTag"/> class.
        /// </summary>
        /// <param name="tags">The tags string.</param>
        public MediaInfoTag(string tags)
        {
            // split this up
            IDictionary<string, string> currentMainValue = null;
            this.lookup = new Dictionary<string, IDictionary<string, string>>();

            foreach (var line in tags.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrEmpty(line))
                {
                    currentMainValue = null;
                    continue;
                }

                var index = line.IndexOf(":");
                if (index == -1)
                {
                    this.lookup.Add(line, currentMainValue = new Dictionary<string, string>());
                    continue;
                }

                var key = line.Substring(0, index).Trim();
                var value = line.Substring(index + 1).Trim();

                currentMainValue.Add(key, value);
            }
        }

        /// <inheritdoc/>
        public override TagTypes TagTypes => TagTypes.Apple;

        /// <inheritdoc/>
        public override string Album { get => this.GetString("General", "Album"); set => this.SetString("General", "Album", value); }

        /// <inheritdoc/>
        public override string[] AlbumArtists { get => this.GetStringArray("General", "Album/Performer"); set => this.SetStringArray("General", "Album/Performer", value); }

        /// <inheritdoc/>
        public override string[] AlbumArtistsSort { get => this.GetStringArray("General", "Album/Performer/Sort"); set => this.SetStringArray("General", "Album/Performer/Sort", value); }

        /// <inheritdoc/>
        public override string AlbumSort { get => this.GetString("General", "Album/Sorted by"); set => this.SetString("General", "Album/Sorted by", value); }

        /// <inheritdoc/>
        public override uint BeatsPerMinute { get => this.GetUInt32("General", "BPM"); set => this.SetUInt32("General", "BPM", value); }

        /// <inheritdoc/>
        public override string Comment { get => this.GetString("General", "Comment"); set => this.SetString("General", "Comment", value); }

        /// <inheritdoc/>
        public override string[] Composers { get => this.GetStringArray("General", "Composer"); set => this.SetStringArray("General", "Composer", value); }

        /// <inheritdoc/>
        public override string[] ComposersSort { get => this.GetStringArray("General", "Composer/Sorted by"); set => this.SetStringArray("General", "Composer/Sorted by", value); }

        /// <inheritdoc/>
        public override uint Disc { get => this.GetUInt32("General", "Part/Position"); set => this.SetUInt32("General", "Part/Position", value); }

        /// <inheritdoc/>
        public override uint DiscCount { get => this.GetUInt32("General", "Part/Total"); set => this.SetUInt32("General", "Part/Total", value); }

        /// <inheritdoc/>
        public override string[] Genres { get => this.GetStringArray("General", "Genre"); set => this.SetStringArray("General", "Genre", value); }

        /// <inheritdoc/>
        public override string Grouping { get => this.GetString("General", "Grouping"); set => this.SetString("General", "Grouping", value); }

        /// <inheritdoc/>
        public override string Lyrics { get => this.GetString("General", "Lyrics")?.Replace(" / ", Environment.NewLine); set => this.SetString("General", "Lyrics", value?.Replace(Environment.NewLine, " / ")); }

        /// <inheritdoc/>
        public override string[] Performers { get => this.GetStringArray("General", "Performer"); set => this.SetStringArray("General", "Performer", value); }

        /// <inheritdoc/>
        public override string[] PerformersSort { get => this.GetStringArray("General", "Performer/Sorted by"); set => this.SetStringArray("General", "Performer/Sorted by", value); }

        /// <inheritdoc/>
        public override string Title { get => this.GetString("General", "Track name"); set => this.SetString("General", "Track name", value); }

        /// <inheritdoc/>
        public override string TitleSort { get => this.GetString("General", "Title/Sort"); set => this.SetString("General", "Title/Sort", value); }

        /// <inheritdoc/>
        public override uint Track { get => this.GetUInt32("General", "Track name/Position"); set => this.SetUInt32("General", "Track name/Position", value); }

        /// <inheritdoc/>
        public override uint TrackCount { get => this.GetUInt32("General", "Track name/Total"); set => this.SetUInt32("General", "Track name/Total", value); }

        /// <inheritdoc/>
        public override uint Year { get => this.GetUInt32("General", "Recorded date"); set => this.SetUInt32("General", "Recorded date", value); }

        /// <inheritdoc/>
        public override void Clear() => this.lookup.Clear();

        private string GetString(string category, string key) => this.lookup[category].TryGetValue(key, out var value) ? value : null;

        private void SetString(string category, string key, string value)
        {
            if (value == null)
            {
                this.lookup[category].Remove(key);
                return;
            }

            this.lookup[category][key] = value;
        }

        private string[] GetStringArray(string category, string key)
        {
            var stringValue = this.GetString(category, key);
            return string.IsNullOrEmpty(stringValue) ? new string[0] : stringValue.Split(';').Select(_ => _.Trim()).ToArray();
        }

        private void SetStringArray(string category, string key, string[] value) => this.SetString(category, key, value == null ? null : string.Join("; ", value));

        private uint GetUInt32(string category, string key)
        {
            var stringValue = this.GetString(category, key);
            if (stringValue != null && uint.TryParse(stringValue, out var value))
            {
                return value;
            }

            return 0;
        }

        private void SetUInt32(string category, string key, uint value) => this.SetString(category, key, value == 0 ? null : value.ToString());
    }
}
