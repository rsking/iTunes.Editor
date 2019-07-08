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
    public class TagLibTagProvider : TagProvider, IFileProvider
    {
        /// <inheritdoc/>
        public string File { get; set; }

        /// <inheritdoc/>
        public override TagLib.Tag GetTag()
        {
            var (file, fileAbstraction) = GetFile(this.File);
            var tag = file?.Tag;
            file?.Dispose();
            if (fileAbstraction is System.IDisposable disposable)
            {
                disposable.Dispose();
            }

            return tag;
        }

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The file.</returns>
        private static (TagLib.File file, TagLib.File.IFileAbstraction fileAbstraction) GetFile(string path)
        {
            TagLib.File file = null;
            TagLib.File.IFileAbstraction fileAbstraction = default;

            try
            {
                fileAbstraction = new LocalFileAbstraction(path);
                file = TagLib.File.Create(fileAbstraction);
            }
            catch (TagLib.UnsupportedFormatException)
            {
                // ignore the error
            }
            finally
            {
                if (file == null && fileAbstraction is System.IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            if (file == null)
            {
                try
                {
                    fileAbstraction = new LocalFileAbstraction(path);
                    file = new TagLib.Mpeg4.File(fileAbstraction);
                }
                catch (TagLib.CorruptFileException)
                {
                    // This is not a music file
                }
                catch (TagLib.UnsupportedFormatException)
                {
                    // ignore the error
                }
                finally
                {
                    if (file == null && fileAbstraction is System.IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }

            return (file, fileAbstraction);
        }
    }
}
