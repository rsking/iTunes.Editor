// -----------------------------------------------------------------------
// <copyright file="ListExplicitLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor;

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
    public ValueTask<bool?> IsExplicitAsync(string lyrics, CancellationToken cancellationToken) => new(ExplicitWords.Any(explicitWord =>
#if NETSTANDARD2_1_OR_GREATER
            lyrics.Contains(explicitWord, StringComparison.OrdinalIgnoreCase)));
#else
            lyrics.IndexOf(explicitWord, StringComparison.OrdinalIgnoreCase) >= 0));
#endif
}
