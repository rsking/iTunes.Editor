// -----------------------------------------------------------------------
// <copyright file="ListExplicitLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// An <see cref="IExplicitLyricsProvider"/> based all a list.
    /// </summary>
    public class ListExplicitLyricsProvider : IExplicitLyricsProvider
    {
        private static readonly IEnumerable<string> ExplicitWords = new string[]
        {
            "fuck",
            "f***",
            "f**k",
            "cunt",
            "c***",
            "c**t",
            "shit",
            "s***",
            "s**t",
            "dick",
            "d***",
            "d**k",
        };

        /// <inheritdoc/>
        public bool? IsExplicit(string lyrics) => ExplicitWords.Any(explicitWord => lyrics.IndexOf(explicitWord, StringComparison.OrdinalIgnoreCase) >= 0);

        /// <inheritdoc/>
        public Task<bool?> IsExplicitAsync(string lyrics) => Task.Run(() => this.IsExplicit(lyrics));
    }
}
