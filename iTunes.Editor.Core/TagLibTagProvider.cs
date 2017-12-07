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
            using (var file = TagLibHelper.GetFile(this.File))
            {
                return file.Tag;
            }
        }
    }
}
