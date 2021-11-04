// <copyright file="IAlbum.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Models;

/// <summary>
/// Represents an album.
/// </summary>
public interface IAlbum : INamed
{
    /// <summary>
    /// Gets the artist.
    /// </summary>
    IArtist Artist { get; }

    /// <summary>
    /// Gets the songs.
    /// </summary>
    IEnumerable<ISong> Songs { get; }
}
