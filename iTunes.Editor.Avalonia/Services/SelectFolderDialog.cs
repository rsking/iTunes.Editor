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
    public string? GetSelectedPath(string? path = default) => this.GetSelectedPathAsync(path) switch
    {
        { IsCompletedSuccessfully: true } task => task.Result,
        var task => task.AsTask().Result,
    };

    /// <summary>
    /// Gets the path using the specified <paramref name="path"/> as a starting point.
    /// </summary>
    /// <param name="path">The initial path.</param>
    /// <returns>The selected path; otherwise <see langword="null"/>.</returns>
    public async ValueTask<string?> GetSelectedPathAsync(string? path = default)
    {
        if (global::Avalonia.Application.Current?.GetActiveWindow() is { } window)
        {
            var storageProvider = window.StorageProvider;
            var options = new global::Avalonia.Platform.Storage.FolderPickerOpenOptions();
            if (path is not null && Directory.Exists(path))
            {
                var builder = new UriBuilder
                {
                    Path = path,
                    Scheme = "file://",
                };

                options.SuggestedStartLocation = await storageProvider.TryGetFolderFromPathAsync(builder.Uri).ConfigureAwait(false);
            }

            var values = await window.StorageProvider.OpenFolderPickerAsync(options).ConfigureAwait(false);
            return values?.Select(v => v.Path.LocalPath).FirstOrDefault();
        }

        return default;
    }
}
