﻿// -----------------------------------------------------------------------
// <copyright file="IOpenFile.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Services.Contracts;

/// <summary>
/// Service contract for selecting a file to open.
/// </summary>
public interface IOpenFile : ISelectFile
{
    /// <summary>
    /// Gets multiple file names.
    /// </summary>
    /// <returns>The list of file names.</returns>
    IEnumerable<string> GetFileNames();

    /// <summary>
    /// Gets multiple file names asynchronously.
    /// </summary>
    /// <returns>The list of file names.</returns>
    ValueTask<IEnumerable<string>> GetFileNamesAsync();
}
