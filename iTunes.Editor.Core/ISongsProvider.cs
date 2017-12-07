// -----------------------------------------------------------------------
// <copyright file="ISongsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// Interface for loading songs.
    /// </summary>
    public interface ISongsProvider : INamedProvider
    {
        /// <summary>
        /// Gets the tag information.
        /// </summary>
        /// <returns>The tag information.</returns>
        System.Collections.Generic.IEnumerable<SongInformation> GetTagInformation();

        /// <summary>
        /// Gets the tag information.
        /// </summary>
        /// <returns>The task to get the tag information.</returns>
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<SongInformation>> GetTagInformationAsync();
    }
}
