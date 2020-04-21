// -----------------------------------------------------------------------
// <copyright file="ISelectFolder.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Services.Contracts
{
    /// <summary>
    /// Service contract for selecting a folder.
    /// </summary>
    public interface ISelectFolder
    {
        /// <summary>
        /// Gets the path using the specified <paramref name="path"/> as a starting point.
        /// </summary>
        /// <param name="path">The initial path.</param>
        /// <returns>The selected path; otherwise <see langword="null"/>.</returns>
        string? GetSelectedPath(string? path = default);

        /// <summary>
        /// Gets the path using the specified <paramref name="path"/> as a starting point asynchronously.
        /// </summary>
        /// <param name="path">The initial path.</param>
        /// <returns>The selected path; otherwise <see langword="null"/>.</returns>
        System.Threading.Tasks.Task<string?> GetSelectedPathAsync(string? path = default);
    }
}
