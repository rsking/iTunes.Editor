// -----------------------------------------------------------------------
// <copyright file="ITagProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// Interface for getting tags.
    /// </summary>
    public interface ITagProvider
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The tag for <paramref name="input"/>.</returns>
        TagLib.Tag GetTag(string input);

        /// <summary>
        /// Gets the tag asynchronously.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The tag for <paramref name="input"/>.</returns>
        System.Threading.Tasks.Task<TagLib.Tag> GetTagAsync(string input);
    }
}
