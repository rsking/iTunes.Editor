// -----------------------------------------------------------------------
// <copyright file="ISongs.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Models;

/// <summary>
/// Interface for <see cref="SongInformation"/>.
/// </summary>
public interface ISongs
{
    /// <summary>
    /// Gets a command to update the lyrics.
    /// </summary>
    Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand UpdateLyricsCommand { get; }

    /// <summary>
    /// Gets a command to update the composers.
    /// </summary>
    Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand UpdateComposersCommand { get; }

    /// <summary>
    /// Gets the songs.
    /// </summary>
    System.Collections.Generic.IEnumerable<SongInformation> Songs { get; }

    /// <summary>
    /// Gets or sets the selected song.
    /// </summary>
    SongInformation? SelectedSong { get; set; }

    /// <summary>
    /// Gets the selected tag.
    /// </summary>
    TagLib.Tag? SelectedTag { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is loading.
    /// </summary>
    public bool IsLoading { get; }

    /// <summary>
    /// Gets the progress.
    /// </summary>
    string? Progress { get; }

    /// <summary>
    /// Gets the percentage.
    /// </summary>
    int Percentage { get; }
}
