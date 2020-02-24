// -----------------------------------------------------------------------
// <copyright file="IComposerProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// Providers for composers.
    /// </summary>
    public interface IComposerProvider
    {
        /// <summary>
        /// Gets the composers.
        /// </summary>
        /// <param name="tagInformation">The tag information.</param>
        /// <returns>The composers.</returns>
        System.Collections.Generic.IEnumerable<Name> GetComposers(SongInformation tagInformation);

        /// <summary>
        /// Gets the composers.
        /// </summary>
        /// <param name="tagInformation">The tag information.</param>
        /// <returns>The composers.</returns>
        System.Collections.Generic.IAsyncEnumerable<Name> GetComposersAsync(SongInformation tagInformation);
    }
}
