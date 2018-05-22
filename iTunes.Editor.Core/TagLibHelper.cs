// -----------------------------------------------------------------------
// <copyright file="TagLibHelper.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// The tag library helper.
    /// </summary>
    internal static class TagLibHelper
    {
        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The file.</returns>
        public static TagLib.File GetFile(string path)
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

            return file;
        }
    }
}
