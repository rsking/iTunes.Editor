// -----------------------------------------------------------------------
// <copyright file="TagProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// An abstract <see cref="ITagProvider"/>.
    /// </summary>
    public abstract class TagProvider : ITagProvider
    {
        /// <inheritdoc/>
        public abstract TagLib.Tag GetTag();

        /// <inheritdoc/>
        public virtual System.Threading.Tasks.Task<TagLib.Tag> GetTagAsync() => System.Threading.Tasks.Task.Run(() => this.GetTag());
    }
}
