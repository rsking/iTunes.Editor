// -----------------------------------------------------------------------
// <copyright file="SongsLoadedEvent.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains information when songs are loaded.
    /// </summary>
    public class SongsLoadedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SongsLoadedEvent"/> class.
        /// </summary>
        /// <param name="information">The song information.</param>
        public SongsLoadedEvent(IEnumerable<SongInformation> information)
        {
            this.Information = information;
        }

        /// <summary>
        /// Gets the song information.
        /// </summary>
        public IEnumerable<SongInformation> Information { get; }
    }
}
