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
        internal static System.Windows.Window? GetActiveWindow(this System.Windows.Application application)
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
    }
}
