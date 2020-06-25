// -----------------------------------------------------------------------
// <copyright file="DialogExtensions.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Services
{
    using System.Linq;

    /// <summary>
    /// Dialog extension methods.
    /// </summary>
    internal static class DialogExtensions
    {
        /// <summary>
        /// Gets the active window.
        /// </summary>
        /// <param name="application">The application object.</param>
        /// <returns>The active window.</returns>
        internal static global::Avalonia.Controls.Window? GetActiveWindow(this global::Avalonia.Application application)
        {
            var windows = application.Windows;
            var window = windows.SingleOrDefault(x => x.IsActive);

            if (window == null)
            {
                return null;
            }

            // see if any of the other has this as a parent
            return windows.FirstOrDefault(x => window.Equals(x.Parent)) ?? window;
        }

        /// <summary>
        /// Adds the specified filter to the collection.
        /// </summary>
        /// <param name="filterCollection">The filter collection.</param>
        /// <param name="filter">The filter to add.</param>
        internal static void Add(this System.Collections.Generic.IList<global::Avalonia.Controls.FileDialogFilter> filterCollection, string filter)
        {
            var split = filter.Split('|');
            var displayName = split[0];
            var index = displayName.IndexOf("(", System.StringComparison.OrdinalIgnoreCase);
            if (index > 0)
            {
                displayName = displayName.Substring(0, index);
            }

            var dialogFilter = new global::Avalonia.Controls.FileDialogFilter { Name = displayName };

            // get the extensions
            if (split.Length > 1)
            {
                foreach (var extension in split[1].Split(';').Select(value => System.IO.Path.GetExtension(value)?.TrimStart('.')))
                {
                    dialogFilter.Extensions.Add(extension);
                }
            }

            filterCollection.Add(dialogFilter);
        }
    }
}
