// -----------------------------------------------------------------------
// <copyright file="PListSongsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PList;

/// <summary>
/// A <see cref="ISongsProvider"/> that loads from an Apple plist file.
/// </summary>
public class PListSongsProvider : ISongsProvider, IFileProvider
{
    /// <inheritdoc />
    public string? File { get; set; }

    /// <inheritdoc />
    public string Name => Properties.Resources.PListName;

    /// <inheritdoc />
    public async IAsyncEnumerable<SongInformation> GetTagInformationAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Formatters.PList.PList plist;
        var stream = System.IO.File.OpenRead(this.File);
#if NETSTANDARD2_1_OR_GREATER
        await using (stream.ConfigureAwait(false))
#else
        using (stream)
#endif
        {
            var formatter = new Formatters.PList.PListAsciiFormatter();
            plist = await Task.Run(() => (Formatters.PList.PList)formatter.Deserialize(stream), cancellationToken).ConfigureAwait(false);
        }

        if (plist["Tracks"] is IDictionary<string, object?> dictionary)
        {
            foreach (var track in dictionary
                .Select(kvp => kvp.Value as IDictionary<string, object?>)
                .Where(dict => dict is not null)
                .Select(dict => new Track(dict!)))
            {
                yield return (SongInformation)track;
            }
        }
    }
}
