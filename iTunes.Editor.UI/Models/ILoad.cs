// -----------------------------------------------------------------------
// <copyright file="ILoad.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Models
{
    /// <summary>
    /// Interface for loading songs.
    /// </summary>
    public interface ILoad
    {
        /// <summary>
        /// Gets the providers.
        /// </summary>
        System.Collections.Generic.IEnumerable<ISongsProvider> Providers { get; }

        /// <summary>
        /// Gets or sets the selected provider.
        /// </summary>
        ISongsProvider SelectedProvider { get; set; }

        /// <summary>
        /// Loads the songs using <see cref="SelectedProvider"/>.
        /// </summary>
        /// <returns>The task associated with loading songs.</returns>
        System.Threading.Tasks.Task LoadAsync();

        /// <summary>
        /// Loads the songs using the specified <see cref="ISongsProvider"/>.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>The task associated with loading songs.</returns>
        System.Threading.Tasks.Task LoadAsync(ISongsProvider provider);
    }
}