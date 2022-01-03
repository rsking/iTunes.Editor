// -----------------------------------------------------------------------
// <copyright file="DialogExtensions.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Services;

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
        return application.ApplicationLifetime switch
        {
            global::Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop => GetWindowFromWindows(desktop.Windows),
            global::Avalonia.Controls.ApplicationLifetimes.ISingleViewApplicationLifetime spa => GetWindowFromControl(spa.MainView),
            _ => default,
        };

        static global::Avalonia.Controls.Window? GetWindowFromWindows(IEnumerable<global::Avalonia.Controls.Window> windows)
        {
            var window = windows.SingleOrDefault(x => x.IsActive);
            if (window is null)
            {
                return default;
            }

            // see if any of the other has this as a parent
            return windows.FirstOrDefault(x => x.Parent is not null && window.Equals(x.Parent)) ?? window;
        }

        static global::Avalonia.Controls.Window? GetWindowFromControl(global::Avalonia.Controls.IControl control)
        {
            if (control is global::Avalonia.Controls.Window window)
            {
                return window;
            }

            if (control.Parent is null)
            {
                return default;
            }

            return GetWindowFromControl(control.Parent);
        }
    }

    /// <summary>
    /// Adds the specified filter to the collection.
    /// </summary>
    /// <param name="filterCollection">The filter collection.</param>
    /// <param name="filter">The filter to add.</param>
    internal static void Add(this IList<global::Avalonia.Controls.FileDialogFilter> filterCollection, string filter)
    {
        var split = filter.Split('|');
        var displayName = split[0];
        var index = displayName.IndexOf("(", StringComparison.OrdinalIgnoreCase);
        if (index > 0)
        {
            displayName =
#if NETCOREAPP3_1_OR_GREATER
                displayName[..index];
#else
                displayName.Substring(0, index);
#endif
        }

        var dialogFilter = new global::Avalonia.Controls.FileDialogFilter { Name = displayName };

        // get the extensions
        if (split.Length > 1)
        {
            foreach (var extension in split[1]
                .Split(';')
                .Select(value => Path.GetExtension(value)?.TrimStart('.'))
                .WhereNotNull())
            {
                dialogFilter.Extensions.Add(extension);
            }
        }

        filterCollection.Add(dialogFilter);
    }
}
