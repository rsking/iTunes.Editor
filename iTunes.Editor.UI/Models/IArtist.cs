// <copyright file="IArtist.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Models;

/// <summary>
/// Represents an artist.
/// </summary>
public interface IArtist : INamed
{
    /// <summary>
    /// Gets the albums.
    /// </summary>
    System.Collections.Generic.IEnumerable<IAlbum> Albums { get; }
}
