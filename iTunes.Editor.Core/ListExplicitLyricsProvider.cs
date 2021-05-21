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
        public System.Threading.Tasks.ValueTask<bool?> IsExplicitAsync(string lyrics, System.Threading.CancellationToken cancellationToken) => new(ExplicitWords.Any(explicitWord => lyrics.IndexOf(explicitWord, StringComparison.OrdinalIgnoreCase) >= 0));
    }
}
