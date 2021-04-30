// -----------------------------------------------------------------------
// <copyright file="IUpdateLyricsService.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// The lyrics update service.
    /// </summary>
    public interface IUpdateLyricsService : IUpdateService
    {
        /// <summary>
        /// Updates the song.
        /// </summary>
        /// <param name="songInformation">The song information.</param>
        /// <param name="forceSearch">Set to <see langword="true"/> to force the search.</param>
        /// <param name="forceExplicit">Set to <see langword="true"/> to force the explicit validation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated <see cref="SongInformation"/>.</returns>
        System.Threading.Tasks.Task<SongInformation> UpdateAsync(SongInformation songInformation, bool forceSearch = false, bool forceExplicit = false, System.Threading.CancellationToken cancellationToken = default);
    }
}
