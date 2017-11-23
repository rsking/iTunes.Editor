﻿// -----------------------------------------------------------------------
// <copyright file="IUpdateLyricsService.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// The composer updater service.
    /// </summary>
    public interface IUpdateLyricsService
    {
        /// <summary>
        /// Updates the lyrics.
        /// </summary>
        /// <param name="songInformation">The song information.</param>
        /// <param name="force">Set to <see langword="true"/> to force the update.</param>
        /// <returns>The updated <see cref="SongInformation"/>.</returns>
        SongInformation Update(SongInformation songInformation, bool force = false);

        /// <summary>
        /// Updates the lyrics.
        /// </summary>
        /// <param name="songInformation">The song information.</param>
        /// <param name="force">Set to <see langword="true"/> to force the update.</param>
        /// <returns>The updated <see cref="SongInformation"/>.</returns>
        System.Threading.Tasks.Task<SongInformation> UpdateAsync(SongInformation songInformation, bool force = false);
    }
}