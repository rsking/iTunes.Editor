namespace ITunes.Editor.Controls.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;

    public static class DialogExtensions
    {
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
}
