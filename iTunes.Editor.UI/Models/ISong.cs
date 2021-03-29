// <copyright file="ISong.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Models
{
    /// <summary>
    /// Represents a song.
    /// </summary>
    public interface ISong : INamed
    {
        /// <summary>
        /// Gets the album.
        /// </summary>
        IAlbum Album { get; }
    }
}
