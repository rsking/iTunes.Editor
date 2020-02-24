// -----------------------------------------------------------------------
// <copyright file="ILyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// Providers for lyrics.
    /// </summary>
    public interface ILyricsProvider
    {
        /// <summary>
        /// Gets the lyrics.
        /// </summary>
        /// <param name="tagInformation">The tag information.</param>
        /// <returns>The lyrics.</returns>
        string? GetLyrics(SongInformation tagInformation);

        /// <summary>
        /// Gets the lyrics.
        /// </summary>
        /// <param name="tagInformation">The tag information.</param>
        /// <returns>The lyrics.</returns>
        System.Threading.Tasks.Task<string?> GetLyricsAsync(SongInformation tagInformation);
    }
}
