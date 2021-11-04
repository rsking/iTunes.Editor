// -----------------------------------------------------------------------
// <copyright file="OpenFileDialog.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Services;

/// <summary>
/// Represents the service implementation for an open file dialog.
/// </summary>
public class OpenFileDialog : SelectFile, Contracts.IOpenFile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenFileDialog"/> class.
    /// </summary>
    public OpenFileDialog() => this.Title = Avalonia.Properties.Resources.OpenFileDialogTitle;

    /// <summary>
    /// Gets the file name using the specified <paramref name="path"/> as a starting point.
    /// </summary>
    /// <param name="path">The starting file.</param>
    /// <returns>The file name if successful; otherwise <see langword="null"/>.</returns>
    public override string? GetFileName(string? path = null)
    {
        var task = this.GetFileNameAsync(path);
        if (task.IsCompletedSuccessfully)
        {
            return task.Result;
        }

        return task.AsTask().Result;
    }

    /// <summary>
    /// Gets the file name using the specified <paramref name="path"/> as a starting point asynchronously.
    /// </summary>
    /// <param name="path">The starting path.</param>
    /// <returns>The file name if successful; otherwise <see langword="null"/>.</returns>
    public override async ValueTask<string?> GetFileNameAsync(string? path = default)
    {
        var fileNames = await this.GetFileNamesImpl(path, multiselect: false).ConfigureAwait(false);
        if (fileNames is null)
        {
            return default;
        }

        return fileNames.FirstOrDefault();
    }

    /// <summary>
    /// Gets multiple file names.
    /// </summary>
    /// <returns>The list of file names.</returns>
    public IEnumerable<string> GetFileNames()
    {
        var task = this.GetFileNamesAsync();
        if (task.IsCompletedSuccessfully)
        {
            return task.Result;
        }

        return task.AsTask().Result;
    }

    /// <summary>
    /// Gets multiple file names asynchronously.
    /// </summary>
    /// <returns>The list of file names.</returns>
    public ValueTask<IEnumerable<string>> GetFileNamesAsync() => this.GetFileNamesImpl(path: null, multiselect: true);

    /// <summary>
    /// Internal implementation to get the filenames.
    /// </summary>
    /// <param name="path">The starting file.</param>
    /// <param name="multiselect">Whether to select more than one file.</param>
    /// <returns>The list of file names.</returns>
    private async ValueTask<IEnumerable<string>> GetFileNamesImpl(string? path, bool multiselect)
    {
        var dialog = new global::Avalonia.Controls.OpenFileDialog
        {
            InitialFileName = path,
            Title = this.Title,
            AllowMultiple = multiselect,
        };

        foreach (var filter in this.Filters)
        {
            dialog.Filters.Add(filter);
        }

        return await dialog.ShowAsync(global::Avalonia.Application.Current.GetActiveWindow()).ConfigureAwait(false);
    }
}
