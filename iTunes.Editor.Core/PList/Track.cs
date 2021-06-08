// -----------------------------------------------------------------------
// <copyright file="Track.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PList
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The track.
    /// </summary>
    internal class Track
    {
        private const string LocalHostString = "localhost/";

        /// <summary>
        /// The dictionary.
        /// </summary>
        private readonly IDictionary<string, object?> dict;

        /// <summary>
        /// Initializes a new instance of the <see cref="Track"/> class.
        /// </summary>
        /// <param name="dict">The dictionary.</param>
        public Track(IDictionary<string, object?> dict) => this.dict = dict;

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public int Id => this.dict.GetInt32("Track ID");

        /// <summary>
        /// Gets the size.
        /// </summary>
        public int Size => this.dict.GetInt32("Size");

        /// <summary>
        /// Gets the total time.
        /// </summary>
        public TimeSpan TotalTime => TimeSpan.FromMilliseconds(this.dict.GetInt64("Total Time"));

        /// <summary>
        /// Gets the number.
        /// </summary>
        public int Number => this.dict.GetInt32("Track Number");

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count => this.dict.GetInt32("Track Count");

        /// <summary>
        /// Gets the year.
        /// </summary>
        public int Year => this.dict.GetInt32("Year");

        /// <summary>
        /// Gets the beats per minute.
        /// </summary>
        public int? BeatsPerMinute => this.dict.GetNullableInt32("BPM");

        /// <summary>
        /// Gets the date modified.
        /// </summary>
        public DateTime DateModified => this.dict.GetDateTime("Date Modified");

        /// <summary>
        /// Gets the date added.
        /// </summary>
        public DateTime DateAdded => this.dict.GetDateTime("Date Added");

        /// <summary>
        /// Gets the bit rate.
        /// </summary>
        public int BitRate => this.dict.GetInt32("Bit Rate");

        /// <summary>
        /// Gets the play count.
        /// </summary>
        public int? PlayCount => this.dict.GetNullableInt32("Play Count");

        /// <summary>
        /// Gets the play date.
        /// </summary>
        public long? PlayDate => this.dict.GetNullableInt64("Play Date");

        /// <summary>
        /// Gets the play date in UTC.
        /// </summary>
        public DateTime? PlayDateUtc => this.dict.GetNullableDateTime("Play Date UTC");

        /// <summary>
        /// Gets the rating.
        /// </summary>
        public int? Rating => this.dict.GetNullableInt32("Rating");

        /// <summary>
        /// Gets the album rating.
        /// </summary>
        public int? AlbumRating => this.dict.GetNullableInt32("Album Rating");

        /// <summary>
        /// Gets a value indicating whether the <see cref="AlbumRating"/> is computed.
        /// </summary>
        public bool? AlbumRatingComputed => this.dict.GetNullableBoolean("Album Rating Computed");

        /// <summary>
        /// Gets the artwork count.
        /// </summary>
        public int ArtworkCount => this.dict.GetInt32("Artwork Count");

        /// <summary>
        /// Gets a value indicating whether this instance is clean.
        /// </summary>
        public bool? Clean => this.dict.GetNullableBoolean("Clean");

        /// <summary>
        /// Gets the track type.
        /// </summary>
        public string TrackType => this.dict.GetString("Track Type");

        /// <summary>
        /// Gets a value indicating whether this instance has video.
        /// </summary>
        public bool? HasVideo => this.dict.GetNullableBoolean("Has Video");

        /// <summary>
        /// Gets the file folder count.
        /// </summary>
        public int FileFolderCount => this.dict.GetInt32("File Folder Count");

        /// <summary>
        /// Gets the library folder count.
        /// </summary>
        public int LibraryFolderCount => this.dict.GetInt32("Library Folder Count");

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name => this.dict.GetString("Name");

        /// <summary>
        /// Gets the artist.
        /// </summary>
        public string? Artist => this.dict.GetNullableString("Artist");

        /// <summary>
        /// Gets the album artist.
        /// </summary>
        public string? AlbumArtist => this.dict.GetNullableString("Album Artist");

        /// <summary>
        /// Gets the composer.
        /// </summary>
        public string Composer => this.dict.GetString("Composer");

        /// <summary>
        /// Gets the album.
        /// </summary>
        public string? Album => this.dict.GetNullableString("Album");

        /// <summary>
        /// Gets the genre.
        /// </summary>
        public string? Genre => this.dict.GetNullableString("Genre");

        /// <summary>
        /// Gets the grouping.
        /// </summary>
        public string? Grouping => this.dict.GetNullableString("Grouping");

        /// <summary>
        /// Gets the kind.
        /// </summary>
        public string Kind => this.dict.GetString("Kind");

        /// <summary>
        /// Gets the sort name.
        /// </summary>
        public string SortName => this.dict.GetString("Sort Name");

        /// <summary>
        /// Gets the sort album.
        /// </summary>
        public string? SortAlbum => this.dict.GetNullableString("Sort Album");

        /// <summary>
        /// Gets the sort artist.
        /// </summary>
        public string? SortArtist => this.dict.GetNullableString("Sort Artist");

        /// <summary>
        /// Gets the sort album artist.
        /// </summary>
        public string? SortAlbumArtist => this.dict.GetNullableString("Sort Album Artist");

        /// <summary>
        /// Gets the location.
        /// </summary>
        public string? Location => this.dict.GetNullableString("Location");

        /// <summary>
        /// Converts a <see cref="TagLib.File"/> to a <see cref="SongInformation"/>.
        /// </summary>
        /// <param name="track">The track to convert.</param>
        public static explicit operator SongInformation(Track track)
        {
            var path = track.Location;
            if (path is not null)
            {
                var uri = new Uri(path.Replace(LocalHostString, string.Empty));
                path = System.IO.Path.GetFullPath(uri.LocalPath);
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
            };
        }
    }
}
