// <copyright file="ISelectable.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Models;

/// <summary>
/// Represents something that is selectable.
/// </summary>
public interface ISelectable
{
    /// <summary>
    /// Gets or sets a value indicating whether this instance is selected.
    /// </summary>
    bool IsSelected { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is expanded.
    /// </summary>
    bool IsExpanded { get; set; }

    /// <summary>
    /// Gets the children.
    /// </summary>
    IEnumerable<ISelectable> Children { get; }

    /// <summary>
    /// Gets the parent.
    /// </summary>
    ISelectable? Parent { get; }
}
