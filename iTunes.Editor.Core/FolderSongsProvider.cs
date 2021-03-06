﻿// -----------------------------------------------------------------------
// <copyright file="FolderSongsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using System.Collections.Generic;
    using System.Linq;

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
        public async IAsyncEnumerable<SongInformation> GetTagInformationAsync([System.Runtime.CompilerServices.EnumeratorCancellation] System.Threading.CancellationToken cancellationToken = default)
        {
            var directoryInfo = new System.IO.DirectoryInfo(this.Folder);
            if (!directoryInfo.Exists)
            {
                throw new System.IO.DirectoryNotFoundException($"Failed to find {this.Folder}");
            }

            // get all the files
            foreach (var fileInfo in directoryInfo.EnumerateFiles("*", System.IO.SearchOption.AllDirectories)
                .Where(fileInfo => (fileInfo.Attributes & System.IO.FileAttributes.Hidden) == 0 || (fileInfo.Attributes & System.IO.FileAttributes.System) == 0))
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
}
