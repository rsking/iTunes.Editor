﻿// -----------------------------------------------------------------------
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
        /// <returns>The selected path; otherwise <see langword="null"/></returns>
        public string GetSelectedPath(string path = "")
        {
            if (Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialog.IsPlatformSupported)
            {
                var dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    InitialDirectory = string.IsNullOrEmpty(path) ? null : path
                };

                if (dialog.ShowDialog(System.Windows.Application.Current.GetActiveWindow()) == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
                {
                    return dialog.FileName;
                }
            }
            else
            {
                // use the platform folder dialog
                var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog { SelectedPath = string.IsNullOrEmpty(path) ? null : path };
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return folderBrowserDialog.SelectedPath;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the path using the specified <paramref name="path"/> as a starting point.
        /// </summary>
        /// <param name="path">The initial path.</param>
        /// <returns>The selected path; otherwise <see langword="null"/></returns>
        public System.Threading.Tasks.Task<string> GetSelectedPathAsync(string path = "") => System.Threading.Tasks.Task.FromResult(this.GetSelectedPath(path));
    }
}