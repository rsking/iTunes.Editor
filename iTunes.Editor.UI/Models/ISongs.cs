// -----------------------------------------------------------------------
// <copyright file="ISongs.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Models
{
    /// <summary>
    /// Interface for <see cref="SongInformation"/>.
    /// </summary>
    public interface ISongs
    {
        /// <summary>
        /// Gets the songs.
        /// </summary>
        System.Collections.Generic.IEnumerable<SongInformation> Songs { get; }
    }
}