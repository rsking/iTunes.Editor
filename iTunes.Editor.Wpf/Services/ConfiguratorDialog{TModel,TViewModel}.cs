namespace ITunes.Editor.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ITunes.Editor.Controls.Dialogs;

    public class ConfiguratorDialog<TModel, TViewModel> : Configurator<TModel, TViewModel>
        where TViewModel : Models.IConfigure
    {
        MahApps.Metro.Controls.Dialogs.IDialogCoordinator dialogCoordinator;

        public ConfiguratorDialog(
            MahApps.Metro.Controls.Dialogs.IDialogCoordinator dialogCoordinator,
            System.Func<TModel, TViewModel> viewModelCreator)
            : base(viewModelCreator)
        {
            this.dialogCoordinator = dialogCoordinator;
        }

        /// <inheritdoc/>
        protected override ValueTask<bool> ConfigureViewModelAsync(TViewModel source) =>
            new(System.Windows.Application.Current.Dispatcher.Invoke(async () =>
                await this.dialogCoordinator
                    .ShowConfigureAsync(source)
                    .ConfigureAwait(true)));
    }
}
