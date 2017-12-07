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
        /// <returns>The tag.</returns>
        TagLib.Tag GetTag();

        /// <summary>
        /// Gets the tag asynchronously.
        /// </summary>
        /// <returns>The tag.</returns>
        System.Threading.Tasks.Task<TagLib.Tag> GetTagAsync();
    }
}
