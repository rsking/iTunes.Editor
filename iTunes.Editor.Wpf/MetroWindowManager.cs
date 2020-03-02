// -----------------------------------------------------------------------
// <copyright file="MetroWindowManager.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// A service that manages <see cref="MahApps.Metro.Controls.MetroWindow"/> instances.
    /// </summary>
    internal class MetroWindowManager : Caliburn.Micro.WindowManager
    {
        /// <summary>
        /// Makes sure the view is a window is is wrapped by one.
        /// </summary>
        /// <param name="model">The view model.</param>
        /// <param name="view">The view.</param>
        /// <param name="isDialog">Whether or not the window is being shown as a dialog.</param>
        /// <returns>The window.</returns>
        protected override System.Windows.Window EnsureWindow(object model, object view, bool isDialog)
        {
            if (view is System.Windows.Window)
            {
                return base.EnsureWindow(model, view, isDialog);
            }

            var window = new MahApps.Metro.Controls.MetroWindow { Content = view, SizeToContent = System.Windows.SizeToContent.WidthAndHeight };
            window.SetValue(Caliburn.Micro.View.IsGeneratedProperty, true);

            var owner = this.InferOwnerOf(window);
            if (owner != null)
            {
                window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                window.Owner = owner;
            }
            else
            {
                window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            }

            return window;
        }
    }
}
