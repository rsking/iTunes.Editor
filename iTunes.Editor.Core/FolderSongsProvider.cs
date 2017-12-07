// -----------------------------------------------------------------------
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
    public class FolderSongsProvider : SongsProvider, IFolderProvider
    {
        /// <inheritdoc />
        public string Folder { get; set; }

        /// <inheritdoc />
        public override string Name => Properties.Resources.FolderName;

        /// <inheritdoc />
        public override IEnumerable<SongInformation> GetTagInformation()
        {
            var directoryInfo = new System.IO.DirectoryInfo(this.Folder);
            if (!directoryInfo.Exists)
            {
                throw new System.IO.DirectoryNotFoundException($"Failed to find {this.Folder}");
            }

            // get all the files
            return directoryInfo.EnumerateFiles("*", System.IO.SearchOption.AllDirectories)
                .Select(_ => TagLibHelper.GetFile(_.FullName))
                .Where(_ => _ != null)
                .Cast<SongInformation>();
        }
    }
}
