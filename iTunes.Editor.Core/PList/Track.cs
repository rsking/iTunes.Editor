// -----------------------------------------------------------------------
// <copyright file="Track.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PList
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The track.
    /// </summary>
    internal class Track
    {
        private const string LocalHostString = "localhost/";

        /// <summary>
        /// The dictionary.
        /// </summary>
        private readonly IDictionary<string, object> dict;

        /// <summary>
        /// Initializes a new instance of the <see cref="Track"/> class.
        /// </summary>
        /// <param name="dict">The dictionary.</param>
        public Track(IDictionary<string, object> dict) => this.dict = dict;

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public int Id => (int)(long)this.dict["Track ID"];

        /// <summary>
        /// Gets the size.
        /// </summary>
        public int Size => (int)(long)this.dict["Size"];

        /// <summary>
        /// Gets the total time.
        /// </summary>
        public TimeSpan TotalTime => TimeSpan.FromMilliseconds((long)this.dict["Total Time"]);

        /// <summary>
        /// Gets the number.
        /// </summary>
        public int Number => (int)(long)this.dict["Track Number"];

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count => (int)(long)this.dict["Track Count"];

        /// <summary>
        /// Gets the year.
        /// </summary>
        public int Year => (int)(long)this.dict["Year"];

        /// <summary>
        /// Gets the beats per minute.
        /// </summary>
        public int? BeatsPerMinute => this.dict.GetNullableInt32("BPM");

        /// <summary>
        /// Gets the date modified.
        /// </summary>
        public DateTime DateModified => (DateTime)this.dict["Date Modified"];

        /// <summary>
        /// Gets the date added.
        /// </summary>
        public DateTime DateAdded => (DateTime)this.dict["Date Added"];

        /// <summary>
        /// Gets the bit rate.
        /// </summary>
        public int BitRate => (int)(long)this.dict["Bit Rate"];

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
        public int ArtworkCount => (int)(long)this.dict["Artwork Count"];

        /// <summary>
        /// Gets a value indicating whether this instance is clean.
        /// </summary>
        public bool? Clean => this.dict.GetNullableBoolean("Clean");

        /// <summary>
        /// Gets the track type.
        /// </summary>
        public string TrackType => (string)this.dict["Track Type"];

        /// <summary>
        /// Gets a value indicating whether this instance has video.
        /// </summary>
        public bool? HasVideo => this.dict.GetNullableBoolean("Has Video");

        /// <summary>
        /// Gets the file folder count.
        /// </summary>
        public int FileFolderCount => (int)(long)this.dict["File Folder Count"];

        /// <summary>
        /// Gets the library folder count.
        /// </summary>
        public int LibraryFolderCount => (int)(long)this.dict["Library Folder Count"];

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name => (string)this.dict["Name"];

        /// <summary>
        /// Gets the artist.
        /// </summary>
        public string Artist => this.dict.GetString("Artist");

        /// <summary>
        /// Gets the composer.
        /// </summary>
        public string Composer => (string)this.dict["Composer"];

        /// <summary>
        /// Gets the album.
        /// </summary>
        public string Album => this.dict.GetString("Album");

        /// <summary>
        /// Gets the genre.
        /// </summary>
        public string Genre => (string)this.dict["Genre"];

        /// <summary>
        /// Gets the kind.
        /// </summary>
        public string Kind => (string)this.dict["Kind"];

        /// <summary>
        /// Gets the sort name.
        /// </summary>
        public string SortName => (string)this.dict["Sort Name"];

        /// <summary>
        /// Gets the sort album.
        /// </summary>
        public string SortAlbum => this.dict.GetString("Sort Album");

        /// <summary>
        /// Gets the sort artist.
        /// </summary>
        public string SortArtist => this.dict.GetString("Sort Artist");

        /// <summary>
        /// Gets the location.
        /// </summary>
        public string Location => this.dict.GetString("Location");

        /// <summary>
        /// Converts a <see cref="TagLib.File"/> to a <see cref="SongInformation"/>.
        /// </summary>
        /// <param name="track">The track to convert.</param>
        public static explicit operator SongInformation(Track track)
        {
            var path = track.Location;
            if (path != null)
            {
                var uri = new Uri(path.Replace(LocalHostString, string.Empty));
                path = System.IO.Path.GetFullPath(uri.LocalPath);
            }

            return new SongInformation(track.Name, track.Artist, track.SortArtist ?? track.Artist, track.Album, path, track.Rating);
        }
    }
}
