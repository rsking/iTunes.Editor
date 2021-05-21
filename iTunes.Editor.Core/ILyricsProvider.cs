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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The lyrics.</returns>
        System.Threading.Tasks.ValueTask<string?> GetLyricsAsync(SongInformation tagInformation, System.Threading.CancellationToken cancellationToken = default);
    }
}
