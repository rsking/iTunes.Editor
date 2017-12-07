// -----------------------------------------------------------------------
// <copyright file="SongsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Abstract class for loading songs.
    /// </summary>
    public abstract class SongsProvider : ISongsProvider
    {
        /// <inheritdoc/>
        public abstract string Name { get; }

        /// <inheritdoc/>
        public abstract IEnumerable<SongInformation> GetTagInformation();

        /// <inheritdoc/>
        public virtual Task<IEnumerable<SongInformation>> GetTagInformationAsync() => Task.Run(() => this.GetTagInformation());
    }
}
