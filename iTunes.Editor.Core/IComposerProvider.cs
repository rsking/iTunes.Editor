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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The composers.</returns>
        System.Collections.Generic.IAsyncEnumerable<Name> GetComposersAsync(SongInformation tagInformation, System.Threading.CancellationToken cancellationToken);
    }
}
