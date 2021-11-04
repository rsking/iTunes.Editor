// -----------------------------------------------------------------------
// <copyright file="ISelectFile.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Services.Contracts;

/// <summary>
/// Service contract for selecting a file.
/// </summary>
public interface ISelectFile
{
    /// <summary>
    /// Gets or sets the default extension.
    /// </summary>
    string? DefaultExtension { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    string? Title { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to add this to the most recent list.
    /// </summary>
    bool AddToMostRecent { get; set; }

    /// <summary>
    /// Gets the filters.
    /// </summary>
    System.Collections.Generic.IList<string> Filters { get; }

    /// <summary>
    /// Gets the selected filter.
    /// </summary>
    int SelectedFilter { get; }

    /// <summary>
    /// Gets the file name using the specified <paramref name="path"/> as a starting point.
    /// </summary>
    /// <param name="path">The starting path.</param>
    /// <returns>The file name if successful; otherwise <see langword="null"/>.</returns>
    string? GetFileName(string? path = default);

    /// <summary>
    /// Gets the file name using the specified <paramref name="path"/> as a starting point asynchronously.
    /// </summary>
    /// <param name="path">The starting path.</param>
    /// <returns>The file name if successful; otherwise <see langword="null"/>.</returns>
    System.Threading.Tasks.ValueTask<string?> GetFileNameAsync(string? path = default);
}
