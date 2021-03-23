// -----------------------------------------------------------------------
// <copyright file="TagLibTagProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// The <see cref="ITagProvider"/> for <see cref="TagLib"/>.
    /// </summary>
    public class TagLibTagProvider : ITagProvider, IFileProvider
    {
        /// <inheritdoc/>
        public string? File { get; set; }

        /// <inheritdoc/>
        public System.Threading.Tasks.Task<TagLib.Tag?> GetTagAsync(System.Threading.CancellationToken cancellationToken)
        {
            if (this.File is null)
            {
                return System.Threading.Tasks.Task.FromResult<TagLib.Tag?>(null);
            }

            return System.Threading.Tasks.Task.Run(
                () =>
                {
                    using var file = GetFile(this.File);
                    return file?.Tag;
                },
                cancellationToken);
        }

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The file.</returns>
        private static TagLib.File? GetFile(string path)
        {
            TagLib.File? file = null;

            try
            {
                file = TagLib.File.Create(path);
            }
            catch (TagLib.UnsupportedFormatException)
            {
                // ignore the error
            }

            if (file is null)
            {
                try
                {
                    file = new TagLib.Mpeg4.File(path);
                }
                catch (TagLib.CorruptFileException)
                {
                    // This is not a music file
                }
                catch (TagLib.UnsupportedFormatException)
                {
                    // ignore the error
                }
            }

            return file;
        }
    }
}
