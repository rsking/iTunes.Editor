﻿// -----------------------------------------------------------------------
// <copyright file="OpenFileDialog.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Services
{
    using System.Linq;

    /// <summary>
    /// Represents the service implementation for an open file dialog.
    /// </summary>
    public class OpenFileDialog : SelectFile, Contracts.IOpenFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileDialog"/> class.
        /// </summary>
        public OpenFileDialog() => this.Title = Properties.Resources.OpenFileDialogTitle;

        /// <summary>
        /// Gets the file name using the specified <paramref name="path"/> as a starting point.
        /// </summary>
        /// <param name="path">The starting file.</param>
        /// <returns>The file name if successful; otherwise <see langword="null"/>.</returns>
        public override string GetFileName(string path = "") => this.GetFileNameAsync(path).Result;

        /// <summary>
        /// Gets the file name using the specified <paramref name="path"/> as a starting point asynchronously.
        /// </summary>
        /// <param name="path">The starting path.</param>
        /// <returns>The file name if successful; otherwise <see langword="null"/>.</returns>
        public async override System.Threading.Tasks.Task<string> GetFileNameAsync(string path = "") => (await this.GetFileNamesImpl(path, false).ConfigureAwait(false))?.FirstOrDefault();

        /// <summary>
        /// Gets multiple file names.
        /// </summary>
        /// <returns>The list of file names.</returns>
        public System.Collections.Generic.IEnumerable<string> GetFileNames() => this.GetFileNamesAsync().Result;

        /// <summary>
        /// Gets multiple file names asynchronously.
        /// </summary>
        /// <returns>The list of file names.</returns>
        public System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<string>> GetFileNamesAsync() => this.GetFileNamesImpl(null, true);

        /// <summary>
        /// Internal implementation to get the filenames
        /// </summary>
        /// <param name="path">The starting file.</param>
        /// <param name="multiselect">Whether to select more than one file.</param>
        /// <returns>The list of file names.</returns>
        private async System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<string>> GetFileNamesImpl(string path, bool multiselect)
        {
            var dialog = new Avalonia.Controls.OpenFileDialog
            {
                InitialFileName = path,
                Title = this.Title,
                AllowMultiple = multiselect,
            };

            foreach (var filter in this.Filters)
            {
                dialog.Filters.Add(filter);
            }

            return await dialog.ShowAsync(Avalonia.Application.Current.GetActiveWindow()).ConfigureAwait(false);
        }
    }
}
