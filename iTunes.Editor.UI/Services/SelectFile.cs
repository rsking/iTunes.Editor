// -----------------------------------------------------------------------
// <copyright file="SelectFile.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Services
{
    /// <summary>
    /// Represents the service implementation for a select file dialog.
    /// </summary>
    public abstract class SelectFile : Contracts.ISelectFile
    {
        /// <summary>
        /// Gets or sets the default extension.
        /// </summary>
        public string? DefaultExtension { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to add this to the most recent list.
        /// </summary>
        public bool AddToMostRecent { get; set; }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        public System.Collections.Generic.IList<string> Filters { get; } = new System.Collections.Generic.List<string>();

        /// <summary>
        /// Gets or sets the selected filter.
        /// </summary>
        public int SelectedFilter { get; protected set; }

        /// <summary>
        /// Gets the file name using the specified <paramref name="path"/> as a starting point.
        /// </summary>
        /// <param name="path">The starting file.</param>
        /// <returns>The file name if successful; otherwise <see langword="null"/>.</returns>
        public abstract string? GetFileName(string path = "");

        /// <summary>
        /// Gets the file name using the specified <paramref name="path"/> as a starting point asynchronously.
        /// </summary>
        /// <param name="path">The starting path.</param>
        /// <returns>The file name if successful; otherwise <see langword="null"/>.</returns>
        public abstract System.Threading.Tasks.Task<string?> GetFileNameAsync(string path = "");
    }
}
