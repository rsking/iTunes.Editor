// -----------------------------------------------------------------------
// <copyright file="SelectFolderDialog.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Services
{
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
        public string? GetSelectedPath(string? path = default) => this.GetSelectedPathAsync(path).Result;

        /// <summary>
        /// Gets the path using the specified <paramref name="path"/> as a starting point.
        /// </summary>
        /// <param name="path">The initial path.</param>
        /// <returns>The selected path; otherwise <see langword="null"/>.</returns>
        public System.Threading.Tasks.Task<string?> GetSelectedPathAsync(string? path = default)
        {
            var dialog = new Avalonia.Controls.OpenFolderDialog { Directory = path };
            return dialog.ShowAsync(Avalonia.Application.Current.GetActiveWindow());
        }
    }
}
