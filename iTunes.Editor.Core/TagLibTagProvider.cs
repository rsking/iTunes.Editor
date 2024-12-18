﻿// -----------------------------------------------------------------------
// <copyright file="TagLibTagProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor;

/// <summary>
/// The <see cref="ITagProvider"/> for <see cref="TagLib"/>.
/// </summary>
public class TagLibTagProvider : ITagProvider, IFileProvider
{
    /// <inheritdoc/>
    public string? File { get; set; }

    /// <inheritdoc/>
    public ValueTask<TagLib.Tag?> GetTagAsync(CancellationToken cancellationToken)
    {
        if (this.File is null)
        {
            return new(default(TagLib.Tag));
        }

        return new(Task.Run(
            () =>
            {
                using var file = GetFile(this.File);
                return file?.Tag;
            },
            cancellationToken));
    }

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
