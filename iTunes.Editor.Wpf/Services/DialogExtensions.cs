// -----------------------------------------------------------------------
// <copyright file="DialogExtensions.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Services
{
    using System.Linq;

    /// <summary>
    /// Dialog extension methods
    /// </summary>
    internal static class DialogExtensions
    {
        /// <summary>
        /// Gets the active window.
        /// </summary>
        /// <param name="application">The application object.</param>
        /// <returns>The active window.</returns>
        internal static System.Windows.Window GetActiveWindow(this System.Windows.Application application)
        {
            var windows = application.Windows.OfType<System.Windows.Window>().ToList();
            var window = windows.SingleOrDefault(x => x.IsActive);

            if (window == null)
            {
                return null;
            }

            // see if any of the other has this as a parent
            return windows.Find(x => window.Equals(x.Owner)) ?? window;
        }

        /// <summary>
        /// Adds the specified filter to the collection.
        /// </summary>
        /// <param name="filterCollection">The filter collection.</param>
        /// <param name="filter">The filter to add.</param>
        internal static void Add(this Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilterCollection filterCollection, string filter)
        {
            var split = filter.Split('|');
            var displayName = split[0];
            var index = displayName.IndexOf('(');
            if (index > 0)
            {
                displayName = displayName.Substring(0, index);
            }

            var dialogFilter = new Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter { DisplayName = displayName };

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
