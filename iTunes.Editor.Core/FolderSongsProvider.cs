// -----------------------------------------------------------------------
// <copyright file="FolderSongsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor;

/// <summary>
/// A <see cref="ISongsProvider"/> for a folder of files.
/// </summary>
public class FolderSongsProvider : ISongsProvider, IFolderProvider
{
    /// <inheritdoc />
    public string? Folder { get; set; }

    /// <inheritdoc />
    public string Name => Properties.Resources.FolderName;

    /// <inheritdoc />
    public async IAsyncEnumerable<SongInformation> GetTagInformationAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var directoryInfo = new DirectoryInfo(this.Folder);
        if (!directoryInfo.Exists)
        {
            throw new DirectoryNotFoundException($"Failed to find {this.Folder}");
        }

        const FileAttributes None = default;

        // get all the files
        foreach (var fileInfo in directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories)
            .Where(fileInfo => (fileInfo.Attributes & FileAttributes.Hidden) == None || (fileInfo.Attributes & FileAttributes.System) == 0))
        {
            SongInformation? songInformation = null;
            try
            {
                songInformation = await SongInformation.FromFileAsync(fileInfo.FullName, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                continue;
            }

            if (songInformation.Title is null)
            {
                continue;
            }

            yield return songInformation;
        }
    }
}
