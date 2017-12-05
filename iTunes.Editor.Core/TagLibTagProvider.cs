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
    public class TagLibTagProvider : ITagProvider
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The tag for <paramref name="input"/>.</returns>
        public TagLib.Tag GetTag(string input)
        {
            using (var file = TagLibHelper.GetFile(input))
            {
                return file.Tag;
            }
        }

        /// <summary>
        /// Gets the tag asynchronously.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The tag for <paramref name="input"/>.</returns>
        public System.Threading.Tasks.Task<TagLib.Tag> GetTagAsync(string input)
        {
            return System.Threading.Tasks.Task.Run(() => this.GetTag(input));
        }
    }
}
