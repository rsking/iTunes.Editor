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

            try
            {
                file = TagLib.File.Create(path);
            }
            catch (TagLib.UnsupportedFormatException)
            {
                // ignore the error
            }

            if (file == null)
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
