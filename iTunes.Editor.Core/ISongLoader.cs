// -----------------------------------------------------------------------
// <copyright file="ISongLoader.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// Interface for loading songs.
    /// </summary>
    public interface ISongLoader
    {
        /// <summary>
        /// Gets the tag information.
        /// </summary>
        /// <param name="input">The input value.</param>
        /// <returns>The tag information.</returns>
        System.Collections.Generic.IEnumerable<SongInformation> GetTagInformation(string input);

        /// <summary>
        /// Gets the tag information.
        /// </summary>
        /// <param name="input">The input value.</param>
        /// <returns>The task to get the tag information.</returns>
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<SongInformation>> GetTagInformationAsync(string input);
    }
}
