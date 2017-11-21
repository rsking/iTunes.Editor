// -----------------------------------------------------------------------
// <copyright file="FolderSongLoader.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// A <see cref="ISongLoader"/> for a folder of files.
    /// </summary>
    public class FolderSongLoader : ISongLoader
    {
        /// <inheritdoc />
        public IEnumerable<SongInformation> GetTagInformation(string input)
        {
            var directoryInfo = new System.IO.DirectoryInfo(input);
            if (!directoryInfo.Exists)
            {
                throw new System.IO.DirectoryNotFoundException($"Failed to find {input}");
            }

            // get all the files
            return directoryInfo.EnumerateFiles("*", System.IO.SearchOption.AllDirectories)
                .Select(_ => TagLibHelper.GetFile(_.FullName))
                .Where(_ => _ != null)
                .Cast<SongInformation>();
        }

        /// <inheritdoc />
        public Task<IEnumerable<SongInformation>> GetTagInformationAsync(string input)
        {
            return Task.Run(() => this.GetTagInformation(input));
        }
    }
}
