// -----------------------------------------------------------------------
// <copyright file="Track.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PList;

using Formatters.PList;

/// <summary>
/// The track.
/// </summary>
/// <param name="dict">The dictionary.</param>
internal sealed class Track(IDictionary<string, object?> dict)
{
    private const string LocalHostString = "localhost/";

    /// <summary>
    /// Gets the ID.
    /// </summary>
    public int Id => dict.GetInt32("Track ID");

    /// <summary>
    /// Gets the size.
    /// </summary>
    public int Size => dict.GetInt32("Size");

    /// <summary>
    /// Gets the total time.
    /// </summary>
    public TimeSpan TotalTime => TimeSpan.FromMilliseconds(dict.GetInt64("Total Time"));

    /// <summary>
    /// Gets the number.
    /// </summary>
    public int? Number => dict.GetNullableInt32("Track Number");

    /// <summary>
    /// Gets the count.
    /// </summary>
    public int? Count => dict.GetNullableInt32("Track Count");

    /// <summary>
    /// Gets the year.
    /// </summary>
    public int Year => dict.GetInt32("Year");

    /// <summary>
    /// Gets the beats per minute.
    /// </summary>
    public int? BeatsPerMinute => dict.GetNullableInt32("BPM");

    /// <summary>
    /// Gets the date modified.
    /// </summary>
    public DateTime DateModified => dict.GetDateTime("Date Modified");

    /// <summary>
    /// Gets the date added.
    /// </summary>
    public DateTime DateAdded => dict.GetDateTime("Date Added");

    /// <summary>
    /// Gets the bit rate.
    /// </summary>
    public int BitRate => dict.GetInt32("Bit Rate");

    /// <summary>
    /// Gets the play count.
    /// </summary>
    public int? PlayCount => dict.GetNullableInt32("Play Count");

    /// <summary>
    /// Gets the play date.
    /// </summary>
    public long? PlayDate => dict.GetNullableInt64("Play Date");

    /// <summary>
    /// Gets the play date in UTC.
    /// </summary>
    public DateTime? PlayDateUtc => dict.GetNullableDateTime("Play Date UTC");

    /// <summary>
    /// Gets the rating.
    /// </summary>
    public int? Rating => dict.GetNullableInt32("Rating");

    /// <summary>
    /// Gets the album rating.
    /// </summary>
    public int? AlbumRating => dict.GetNullableInt32("Album Rating");

    /// <summary>
    /// Gets a value indicating whether the <see cref="AlbumRating"/> is computed.
    /// </summary>
    public bool? AlbumRatingComputed => dict.GetNullableBoolean("Album Rating Computed");

    /// <summary>
    /// Gets the artwork count.
    /// </summary>
    public int ArtworkCount => dict.GetInt32("Artwork Count");

    /// <summary>
    /// Gets a value indicating whether this instance is clean.
    /// </summary>
    public bool? Clean => dict.GetNullableBoolean("Clean");

    /// <summary>
    /// Gets the track type.
    /// </summary>
    public string TrackType => dict.GetString("Track Type");

    /// <summary>
    /// Gets a value indicating whether this instance has video.
    /// </summary>
    public bool? HasVideo => dict.GetNullableBoolean("Has Video");

    /// <summary>
    /// Gets the file folder count.
    /// </summary>
    public int FileFolderCount => dict.GetInt32("File Folder Count");

    /// <summary>
    /// Gets the library folder count.
    /// </summary>
    public int LibraryFolderCount => dict.GetInt32("Library Folder Count");

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name => dict.GetString("Name");

    /// <summary>
    /// Gets the artist.
    /// </summary>
    public string? Artist => dict.GetNullableString("Artist");

    /// <summary>
    /// Gets the album artist.
    /// </summary>
    public string? AlbumArtist => dict.GetNullableString("Album Artist");

    /// <summary>
    /// Gets the composer.
    /// </summary>
    public string Composer => dict.GetString("Composer");

    /// <summary>
    /// Gets the album.
    /// </summary>
    public string? Album => dict.GetNullableString("Album");

    /// <summary>
    /// Gets the genre.
    /// </summary>
    public string? Genre => dict.GetNullableString("Genre");

    /// <summary>
    /// Gets the grouping.
    /// </summary>
    public string? Grouping => dict.GetNullableString("Grouping");

    /// <summary>
    /// Gets the kind.
    /// </summary>
    public string Kind => dict.GetString("Kind");

    /// <summary>
    /// Gets the sort name.
    /// </summary>
    public string SortName => dict.GetString("Sort Name");

    /// <summary>
    /// Gets the sort album.
    /// </summary>
    public string? SortAlbum => dict.GetNullableString("Sort Album");

    /// <summary>
    /// Gets the sort artist.
    /// </summary>
    public string? SortArtist => dict.GetNullableString("Sort Artist");

    /// <summary>
    /// Gets the sort album artist.
    /// </summary>
    public string? SortAlbumArtist => dict.GetNullableString("Sort Album Artist");

    /// <summary>
    /// Gets the location.
    /// </summary>
    public string? Location => dict.GetNullableString("Location");

    /// <summary>
    /// Gets the disc number.
    /// </summary>
    public int? Disc => dict.GetNullableInt32("Disc Number");

    /// <summary>
    /// Gets a value indicating whether this is part of a compilation.
    /// </summary>
    public bool? Compilation => dict.GetNullableBoolean("Compilation");

    /// <summary>
    /// Converts a <see cref="TagLib.File"/> to a <see cref="SongInformation"/>.
    /// </summary>
    /// <param name="track">The track to convert.</param>
    public static explicit operator SongInformation(Track track)
    {
        var path = track.Location;
        if (path is not null)
        {
            path =
#if NETSTANDARD2_1_OR_GREATER
                path.Replace(LocalHostString, string.Empty, StringComparison.Ordinal);
#else
                path.Replace(LocalHostString, string.Empty);
#endif
            var uri = new Uri(path);
            path = Path.GetFullPath(uri.LocalPath);
        }

        var artist = track.Artist.FromJoinedString().ToArray();
        return new SongInformation(track.Name)
        {
            Performers = artist,
            SortPerformers = track.SortArtist?.FromJoinedString().ToArray() ?? artist,
            AlbumPerformer = track.AlbumArtist,
            SortAlbumPerformer = track.SortAlbum,
            Album = track.Album,
            Name = path,
            Genre = track.Genre,
            Rating = track.Rating,
            HasLyrics = !track.Album.HasNoLyrics(),
            Number = track.Number,
            Total = track.Count,
            Disc = track.Disc,
            IsCompilation = track.Compilation,
        };
    }
}
