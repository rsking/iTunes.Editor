// -----------------------------------------------------------------------
// <copyright file="IFileProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// The file song loader.
    /// </summary>
    public interface IFileProvider
    {
        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        string? File { get; set; }
    }
}
