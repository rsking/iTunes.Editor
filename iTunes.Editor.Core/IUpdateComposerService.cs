﻿// -----------------------------------------------------------------------
// <copyright file="IUpdateComposerService.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// The composer updater service.
    /// </summary>
    public interface IUpdateComposerService
    {
        /// <summary>
        /// Updates the composer.
        /// </summary>
        /// <param name="songInformation">The song information.</param>
        /// <param name="force">Set to <see langword="true"/> to force the update.</param>
        /// <returns>The updated <see cref="SongInformation"/>.</returns>
        SongInformation Update(SongInformation songInformation, bool force = false);

        /// <summary>
        /// Updates the composer.
        /// </summary>
        /// <param name="songInformation">The song information.</param>
        /// <param name="force">Set to <see langword="true"/> to force the update.</param>
        /// <returns>The updated <see cref="SongInformation"/>.</returns>
        System.Threading.Tasks.Task<SongInformation> UpdateAsync(SongInformation songInformation, bool force = false);
    }
}