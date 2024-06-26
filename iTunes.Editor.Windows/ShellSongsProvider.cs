﻿// <copyright file="ShellSongsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Windows;

using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

/// <summary>
/// A <see cref="ISongsProvider"/> for a folder of files.
/// </summary>
public class ShellSongsProvider : ISongsProvider, IFolderProvider
{
    /// <inheritdoc />
    public string? Folder { get; set; }

    /// <inheritdoc />
    public string Name => Properties.Resources.FolderName;

    /// <inheritdoc />
    public IAsyncEnumerable<SongInformation> GetTagInformationAsync(CancellationToken cancellationToken = default)
    {
        return GetEnumerable().ToAsyncEnumerable();

        IEnumerable<SongInformation> GetEnumerable()
        {
            if (this.Folder is null)
            {
                yield break;
            }

            var directoryInfo = new DirectoryInfo(this.Folder);
            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException($"Failed to find {this.Folder}");
            }

            const FileAttributes None = default;

            // get all the files
            foreach (var fullName in directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories)
                .Where(fileInfo => (fileInfo.Attributes & FileAttributes.Hidden) == None || (fileInfo.Attributes & FileAttributes.System) == None)
                .Select(fileInfo => fileInfo.FullName))
            {
                using var shellObject = ShellObject.FromParsingName(fullName);

                var title = shellObject.Properties.GetProperty<string>(SystemProperties.System.Title);
                var performers = shellObject.Properties.GetProperty<string[]>(SystemProperties.System.Music.Artist);
                var sortPerformers = shellObject.Properties.GetProperty<string[]>(SystemProperties.System.Music.ArtistSortOverride);
                var albumPerformers = shellObject.Properties.GetProperty<string>(SystemProperties.System.Music.AlbumArtist);
                var sortAlbumPerformers = shellObject.Properties.GetProperty<string>(SystemProperties.System.Music.AlbumArtistSortOverride);
                var album = shellObject.Properties.GetProperty<string>(SystemProperties.System.Music.AlbumTitle);
                var genres = shellObject.Properties.GetProperty<string[]>(SystemProperties.System.Music.Genre);
                var number = shellObject.Properties.GetProperty<int>(SystemProperties.System.Music.TrackNumber);
                var disc = shellObject.Properties.GetProperty<int>(SystemProperties.System.Music.DiscNumber);

                var performersValue = performers.Value ?? Enumerable.Empty<string>();
                var albumPerformersValue = albumPerformers.Value ?? default;

                if (title.Value is not null)
                {
                    yield return new SongInformation(title.Value)
                    {
                        Performers = performersValue,
                        SortPerformers = sortPerformers.Value ?? performersValue,
                        AlbumPerformer = albumPerformersValue,
                        SortAlbumPerformer = sortAlbumPerformers.Value ?? albumPerformersValue,
                        Album = album.Value,
                        Name = fullName,
                        Genre = genres.Value?.ToJoinedString(),
                        Number = number?.Value,
                        Disc = disc?.Value,
                    };
                }
            }
        }
    }
}
