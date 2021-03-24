// <copyright file="INamed.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Models
{
    /// <summary>
    /// A named item.
    /// </summary>
    public interface INamed
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        string? Name { get; }
    }
}
