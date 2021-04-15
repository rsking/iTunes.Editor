// -----------------------------------------------------------------------
// <copyright file="IUpdateService.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// The updater service.
    /// </summary>
    public interface IUpdateService
    {
        /// <summary>
        /// Updates the song.
        /// </summary>
        /// <param name="songInformation">The song information.</param>
        /// <param name="force">Set to <see langword="true"/> to force the update.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated <see cref="SongInformation"/>.</returns>
        System.Threading.Tasks.Task<SongInformation> UpdateAsync(SongInformation songInformation, bool force = false, System.Threading.CancellationToken cancellationToken = default);
    }
}
