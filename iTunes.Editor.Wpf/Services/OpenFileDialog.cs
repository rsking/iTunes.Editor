﻿// -----------------------------------------------------------------------
// <copyright file="OpenFileDialog.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Services
{
    using System.Linq;
    using Microsoft.WindowsAPICodePack.Dialogs;

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
        public override string GetFileName(string path = "") => this.GetFileNamesImpl(path, false)?.FirstOrDefault();

        /// <summary>
        /// Gets the file name using the specified <paramref name="path"/> as a starting point asynchronously.
        /// </summary>
        /// <param name="path">The starting path.</param>
        /// <returns>The file name if successful; otherwise <see langword="null"/>.</returns>
        public override System.Threading.Tasks.Task<string> GetFileNameAsync(string path = "") => System.Threading.Tasks.Task.FromResult(this.GetFileName(path));

        /// <summary>
        /// Gets multiple file names.
        /// </summary>
        /// <returns>The list of file names.</returns>
        public System.Collections.Generic.IEnumerable<string> GetFileNames() => this.GetFileNamesImpl(null, true);

        /// <summary>
        /// Gets multiple file names asynchronously.
        /// </summary>
        /// <returns>The list of file names.</returns>
        public System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<string>> GetFileNamesAsync() => System.Threading.Tasks.Task.FromResult(this.GetFileNames());

        /// <summary>
        /// Internal implementation to get the filenames
        /// </summary>
        /// <param name="path">The starting file.</param>
        /// <param name="multiselect">Whether to select more than one file.</param>
        /// <returns>The list of file names.</returns>
        private System.Collections.Generic.IEnumerable<string> GetFileNamesImpl(string path, bool multiselect)
        {
            if (CommonFileDialog.IsPlatformSupported)
            {
                using (
                    var commonOpenFileDialog = new CommonOpenFileDialog
                    {
                        AllowNonFileSystemItems = true,
                        AddToMostRecentlyUsedList = this.AddToMostRecent,
                        DefaultExtension = this.DefaultExtension,
                        Title = this.Title,
                        Multiselect = multiselect,
                        EnsureFileExists = true,
                        EnsurePathExists = true
                    })
                {
                    foreach (var filter in this.Filters)
                    {
                        commonOpenFileDialog.Filters.Add(filter);
                    }

                    if (!string.IsNullOrEmpty(path))
                    {
                        commonOpenFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(path);
                        commonOpenFileDialog.DefaultFileName = System.IO.Path.GetFileName(path);
                    }

                    if (commonOpenFileDialog.ShowDialog() != CommonFileDialogResult.Ok)
                    {
                        return null;
                    }

                    this.SelectedFilter = commonOpenFileDialog.SelectedFileTypeIndex - 1;
                    commonOpenFileDialog.DefaultExtension = null;
                    return commonOpenFileDialog.FileNames;
                }
            }

            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = this.DefaultExtension,
                FileName = path,
                Filter = string.Join("|", this.Filters),
                Title = this.Title,
                Multiselect = multiselect,
                FilterIndex = this.SelectedFilter >= 0 ? this.SelectedFilter + 1 : 1,
                CheckFileExists = true,
                CheckPathExists = true
            };

            var returnValue = openFileDialog.ShowDialog();
            if (!returnValue.HasValue || !returnValue.Value)
            {
                return null;
            }

            this.SelectedFilter = openFileDialog.FilterIndex - 1;
            return openFileDialog.FileNames;
        }
    }
}