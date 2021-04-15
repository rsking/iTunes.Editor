// -----------------------------------------------------------------------
// <copyright file="ISongs.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Models
{
    /// <summary>
    /// Interface for <see cref="SongInformation"/>.
    /// </summary>
    public interface ISongs
    {
        /// <summary>
        /// Gets a command to update the lyrics.
        /// </summary>
        System.Windows.Input.ICommand UpdateLyrics { get; }

        /// <summary>
        /// Gets a command to update the composers.
        /// </summary>
        System.Windows.Input.ICommand UpdateComposers { get; }

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
    }
}
