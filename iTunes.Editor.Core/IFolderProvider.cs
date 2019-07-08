// -----------------------------------------------------------------------
// <copyright file="IFolderProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// The folder song loader.
    /// </summary>
    public interface IFolderProvider
    {
        /// <summary>
        /// Gets or sets the folder.
        /// </summary>
        string Folder { get; set; }
    }
}
