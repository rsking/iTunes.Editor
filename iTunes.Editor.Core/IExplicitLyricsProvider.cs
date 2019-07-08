// -----------------------------------------------------------------------
// <copyright file="IExplicitLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// Provider for whether lyrics are explicit.
    /// </summary>
    public interface IExplicitLyricsProvider
    {
        /// <summary>
        /// Returns a value indicating whether the lyrics are explicit.
        /// </summary>
        /// <param name="lyrics">The lyrics.</param>
        /// <returns>Returns <see langword="true"/> if <paramref name="lyrics"/> are explicit; otherwise <see langword="false"/>.</returns>
        bool? IsExplicit(string lyrics);

        /// <summary>
        /// Returns a value indicating whether the lyrics are explicit.
        /// </summary>
        /// <param name="lyrics">The lyrics.</param>
        /// <returns>Returns <see langword="true"/> if <paramref name="lyrics"/> are explicit; otherwise <see langword="false"/>.</returns>
        System.Threading.Tasks.Task<bool?> IsExplicitAsync(string lyrics);
    }
}
