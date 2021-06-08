// <copyright file="ConfiguratorDialog{TModel,TViewModel}.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Services
{
    using System.Threading.Tasks;
    using ITunes.Editor.Controls.Dialogs;

    /// <summary>
    /// The configurator dialog.
    /// </summary>
    /// <typeparam name="TModel">The model.</typeparam>
    /// <typeparam name="TViewModel">The view model.</typeparam>
    public class ConfiguratorDialog<TModel, TViewModel> : Configurator<TModel, TViewModel>
        where TViewModel : Models.IConfigure
    {
        private readonly MahApps.Metro.Controls.Dialogs.IDialogCoordinator dialogCoordinator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfiguratorDialog{TModel, TViewModel}"/> class.
        /// </summary>
        /// <param name="dialogCoordinator">The dialog coordinator.</param>
        /// <param name="viewModelCreator">The view model creator.</param>
        public ConfiguratorDialog(
            MahApps.Metro.Controls.Dialogs.IDialogCoordinator dialogCoordinator,
            System.Func<TModel, TViewModel> viewModelCreator)
            : base(viewModelCreator) => this.dialogCoordinator = dialogCoordinator;

        /// <inheritdoc/>
        protected override ValueTask<bool> ConfigureViewModelAsync(TViewModel source) =>
            new(System.Windows.Application.Current.Dispatcher.Invoke(async () =>
                await this.dialogCoordinator
                    .ShowConfigureAsync(source)
                    .ConfigureAwait(true)));
    }
}
