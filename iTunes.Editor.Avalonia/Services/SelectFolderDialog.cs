// -----------------------------------------------------------------------
// <copyright file="SelectFolderDialog.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Services;

/// <summary>
/// Service for selecting a folder.
/// </summary>
public class SelectFolderDialog : Contracts.ISelectFolder
{
    /// <summary>
    /// Gets the path using the specified <paramref name="path"/> as a starting point.
    /// </summary>
    /// <param name="path">The initial path.</param>
    /// <returns>The selected path; otherwise <see langword="null"/>.</returns>
    public string? GetSelectedPath(string? path = default)
    {
        var task = this.GetSelectedPathAsync(path);
        return task.IsCompletedSuccessfully ? task.Result : task.AsTask().Result;
    }

    /// <summary>
    /// Gets the path using the specified <paramref name="path"/> as a starting point.
    /// </summary>
    /// <param name="path">The initial path.</param>
    /// <returns>The selected path; otherwise <see langword="null"/>.</returns>
    public ValueTask<string?> GetSelectedPathAsync(string? path = default)
    {
        var dialog = new global::Avalonia.Controls.OpenFolderDialog { Directory = path };
        var application = global::Avalonia.Application.Current!;
        var window = application.GetActiveWindow();
        return new(dialog.ShowAsync(window!));
    }
}
