// -----------------------------------------------------------------------
// <copyright file="MediaInfoTag.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.MediaInfo;

using System;
using System.Collections.Generic;
using System.Linq;
using TagLib;

/// <summary>
/// The <see cref="MediaInfo"/> <see cref="Tag"/>.
/// </summary>
public class MediaInfoTag : Tag
{
    private const string GeneralCategory = "General";

    private const string AlbumKey = nameof(Album);

    private const string PerformerKey = "Performer";

    private const string TrackNameKey = "Track name";

    private const string ComposerKey = "Composer";

    private const string SortedByKey = "Sorted by";

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
        this.lookup = new Dictionary<string, IDictionary<string, string>>(StringComparer.Ordinal);
        if (tags is null)
        {
            return;
        }

        // split this up
        IDictionary<string, string>? currentMainValue = null;

        foreach (var line in tags.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (string.IsNullOrEmpty(line))
            {
                currentMainValue = null;
                continue;
            }

            var index = line.IndexOf(":", StringComparison.InvariantCulture);
            if (index == -1)
            {
                currentMainValue = new Dictionary<string, string>(StringComparer.Ordinal);
                this.lookup.Add(line, currentMainValue);
                continue;
            }

            var key = line.Substring(0, index).Trim();
            var value =
#if NETSTANDARD2_1_OR_GREATER
                    line[(index + 1)..]
#else
                    line.Substring(index + 1)
#endif
                    .Trim();

            if (currentMainValue is null)
            {
                throw new InvalidOperationException();
            }

            currentMainValue.Add(key, value);
        }
    }

    /// <inheritdoc/>
    public override TagTypes TagTypes => TagTypes.Apple;

    /// <inheritdoc/>
    public override string? Album { get => this.GetString(GeneralCategory, AlbumKey); set => this.SetString(GeneralCategory, AlbumKey, value); }

    /// <inheritdoc/>
    public override string[] AlbumArtists { get => this.GetStringArray(GeneralCategory, $"{AlbumKey}/{PerformerKey}"); set => this.SetStringArray(GeneralCategory, $"{AlbumKey}/{PerformerKey}", value); }

    /// <inheritdoc/>
    public override string[] AlbumArtistsSort { get => this.GetStringArray(GeneralCategory, $"{AlbumKey}/{PerformerKey}/Sort"); set => this.SetStringArray(GeneralCategory, $"{AlbumKey}/{PerformerKey}/Sort", value); }

    /// <inheritdoc/>
    public override string? AlbumSort { get => this.GetString(GeneralCategory, $"{AlbumKey}/{SortedByKey}"); set => this.SetString(GeneralCategory, $"{AlbumKey}/{SortedByKey}", value); }

    /// <inheritdoc/>
    public override uint BeatsPerMinute { get => this.GetUInt32(GeneralCategory, "BPM"); set => this.SetUInt32(GeneralCategory, "BPM", value); }

    /// <inheritdoc/>
    public override string? Comment { get => this.GetString(GeneralCategory, "Comment"); set => this.SetString(GeneralCategory, "Comment", value); }

    /// <inheritdoc/>
    public override string[] Composers { get => this.GetStringArray(GeneralCategory, ComposerKey); set => this.SetStringArray(GeneralCategory, ComposerKey, value); }

    /// <inheritdoc/>
    public override string[] ComposersSort { get => this.GetStringArray(GeneralCategory, $"{ComposerKey}/{SortedByKey}"); set => this.SetStringArray(GeneralCategory, $"{ComposerKey}/{SortedByKey}", value); }

    /// <inheritdoc/>
    public override uint Disc { get => this.GetUInt32(GeneralCategory, "Part/Position"); set => this.SetUInt32(GeneralCategory, "Part/Position", value); }

    /// <inheritdoc/>
    public override uint DiscCount { get => this.GetUInt32(GeneralCategory, "Part/Total"); set => this.SetUInt32(GeneralCategory, "Part/Total", value); }

    /// <inheritdoc/>
    public override string[] Genres { get => this.GetStringArray(GeneralCategory, "Genre"); set => this.SetStringArray(GeneralCategory, "Genre", value); }

    /// <inheritdoc/>
    public override string? Grouping { get => this.GetString(GeneralCategory, "Grouping"); set => this.SetString(GeneralCategory, "Grouping", value); }

    /// <inheritdoc/>
    public override string? Lyrics { get => this.GetString(GeneralCategory, "Lyrics").SafeReplace(" / ", Environment.NewLine); set => this.SetString(GeneralCategory, "Lyrics", value.SafeReplace(Environment.NewLine, " / ")); }

    /// <inheritdoc/>
    public override string[] Performers { get => this.GetStringArray(GeneralCategory, PerformerKey); set => this.SetStringArray(GeneralCategory, PerformerKey, value); }

    /// <inheritdoc/>
    public override string[] PerformersSort { get => this.GetStringArray(GeneralCategory, $"{PerformerKey}/{SortedByKey}"); set => this.SetStringArray(GeneralCategory, $"{PerformerKey}/{SortedByKey}", value); }

    /// <inheritdoc/>
    public override string? Title { get => this.GetString(GeneralCategory, TrackNameKey); set => this.SetString(GeneralCategory, TrackNameKey, value); }

    /// <inheritdoc/>
    public override string? TitleSort { get => this.GetString(GeneralCategory, "Title/Sort"); set => this.SetString(GeneralCategory, "Title/Sort", value); }

    /// <inheritdoc/>
    public override uint Track { get => this.GetUInt32(GeneralCategory, $"{TrackNameKey}/Position"); set => this.SetUInt32(GeneralCategory, $"{TrackNameKey}/Position", value); }

    /// <inheritdoc/>
    public override uint TrackCount { get => this.GetUInt32(GeneralCategory, $"{TrackNameKey}/Total"); set => this.SetUInt32(GeneralCategory, $"{TrackNameKey}/Total", value); }

    /// <inheritdoc/>
    public override uint Year { get => this.GetUInt32(GeneralCategory, "Recorded date"); set => this.SetUInt32(GeneralCategory, "Recorded date", value); }

    /// <inheritdoc/>
    public override void Clear() => this.lookup.Clear();

    private string? GetString(string category, string key) => this.lookup[category].TryGetValue(key, out var value) ? value : null;

    private void SetString(string category, string key, string? value)
    {
        if (value is null)
        {
            this.lookup[category].Remove(key);
            return;
        }

        this.lookup[category][key] = value;
    }

    private string[] GetStringArray(string category, string key)
    {
        var stringValue = this.GetString(category, key);
        return string.IsNullOrEmpty(stringValue) ? Array.Empty<string>() : stringValue!.Split(';').Select(_ => _.Trim()).ToArray();
    }

    private void SetStringArray(string category, string key, string[] value) => this.SetString(category, key, value is null ? null : string.Join("; ", value));

    private uint GetUInt32(string category, string key) => this.GetString(category, key) switch
    {
        string stringValue when uint.TryParse(stringValue, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out var value) => value,
        _ => 0,
    };

    private void SetUInt32(string category, string key, uint value) => this.SetString(category, key, value == 0 ? null : value.ToString(System.Globalization.CultureInfo.InvariantCulture));
}
