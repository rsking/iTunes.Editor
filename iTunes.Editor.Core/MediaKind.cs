// <copyright file="MediaKind.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor
{
    /// <summary>
    /// The media kind.
    /// </summary>
    public enum MediaKind
    {
        /// <summary>
        /// The media item kind is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The media item is a song.
        /// </summary>
        Song,

        /// <summary>
        /// The media item is a movie.
        /// </summary>
        Movie,

        /// <summary>
        /// The media item is an audio or a video podcast.
        /// </summary>
        Podcast,

        /// <summary>
        /// The media item is an audiobook.
        /// </summary>
        Audiobook,

        /// <summary>
        /// The media item is an unwrapped PDF file that’s part of a music album.
        /// </summary>
        PDFBooklet,

        /// <summary>
        /// The media item is a music video.
        /// </summary>
        MusicVideo,

        /// <summary>
        /// The media item is a TV show.
        /// </summary>
        TVShow,

        /// <summary>
        /// The media item is a QuickTime movie with embedded Flash.
        /// </summary>
        InteractiveBooklet,

        /// <summary>
        /// The media item is a non-iTunes Store movie.
        /// </summary>
        HomeVideo,

        /// <summary>
        /// The media item is an iOS ringtone.
        /// </summary>
        Ringtone,

        /// <summary>
        /// The media item is an iTunes Extra or an iTunes LP item.
        /// </summary>
        DigitalBooklet,

        /// <summary>
        /// The media item is an iOS app.
        /// </summary>
        IOSApplication,

        /// <summary>
        /// The media item is a recorded voice memo.
        /// </summary>
        VoiceMemo,

        /// <summary>
        /// The media item is an iTunes U audio or video file.
        /// </summary>
        iTunesU,

        /// <summary>
        /// The media item is an EPUB file or an iBooks Author book.
        /// </summary>
        Book,

        /// <summary>
        /// The media item is a PDF file that iTunes treats as a book unless the user overrides it.
        /// </summary>
        PDFBook,

        /// <summary>
        /// The media item is an audio tone that’s not a protected ringtone on an iOS device.
        /// </summary>
        AlertTone,
    }
}
