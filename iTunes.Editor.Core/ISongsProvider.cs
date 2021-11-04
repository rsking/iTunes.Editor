// -----------------------------------------------------------------------
// <copyright file="ISongsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor;

/// <summary>
/// Interface for loading songs.
/// </summary>
public interface ISongsProvider : INamedProvider
{
    /// <summary>
    /// Gets the tag information.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async enumerable to get the tag information.</returns>
    System.Collections.Generic.IAsyncEnumerable<SongInformation> GetTagInformationAsync(System.Threading.CancellationToken cancellationToken = default);
}
