﻿// -----------------------------------------------------------------------
// <copyright file="SongsLoadedEvent.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Models;

/// <summary>
/// Contains information when songs are loaded.
/// </summary>
public class SongsLoadedEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SongsLoadedEvent"/> class.
    /// </summary>
    /// <param name="information">The song information.</param>
    public SongsLoadedEvent(IAsyncEnumerable<SongInformation> information) => this.Information = information;

    /// <summary>
    /// Gets the song information.
    /// </summary>
    public IAsyncEnumerable<SongInformation> Information { get; }
}
