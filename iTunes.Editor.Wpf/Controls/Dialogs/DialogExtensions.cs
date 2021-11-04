// <copyright file="DialogExtensions.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Controls.Dialogs;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

/// <summary>
/// The dialog extensions.
/// </summary>
public static class DialogExtensions
{
    /// <summary>
    /// Show the configure dialog asynchronously.
    /// </summary>
    /// <param name="dialogCoordinator">The dialog coordinator.</param>
    /// <param name="context">The context.</param>
    /// <param name="settings">The settings.</param>
    /// <returns>The result of the dialog.</returns>
    public static async Task<bool> ShowConfigureAsync(this IDialogCoordinator dialogCoordinator, object context, MetroDialogSettings? settings = null)
    {
        var metroWindow = System.Windows.Application.Current.MainWindow as MetroWindow;
        var dialog = new ConfigureDialog(metroWindow, settings);
        if (metroWindow is not null)
        {
            DialogParticipation.SetRegister(metroWindow, context);
        }

        dialog.DataContext = context;

        await dialogCoordinator.ShowMetroDialogAsync(context, dialog, settings).ConfigureAwait(true);
        var result = await dialog.WaitForButtonPressAsync().ConfigureAwait(true);
        await dialogCoordinator.HideMetroDialogAsync(context, dialog, settings).ConfigureAwait(true);
        return result;
    }
}
