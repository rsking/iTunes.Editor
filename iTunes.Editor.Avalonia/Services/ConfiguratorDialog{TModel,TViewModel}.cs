// <copyright file="ConfiguratorDialog{TModel,TViewModel}.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// The configurator.
    /// </summary>
    /// <typeparam name="TModel">The type to configure.</typeparam>
    /// <typeparam name="TViewModel">The view model.</typeparam>
    public class ConfiguratorDialog<TModel, TViewModel> : IConfigurator<TModel>
        where TViewModel : Models.IConfigure
    {
        private readonly Movere.Services.IContentDialogService<TViewModel> contentDialogService;

        private readonly System.Func<TModel, TViewModel> viewModelCreator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfiguratorDialog{TModel, TViewModel}"/> class.
        /// </summary>
        /// <param name="contentDialogService">The content dialog service.</param>
        /// <param name="viewModelCreator">The view model creator.</param>
        public ConfiguratorDialog(
            Movere.Services.IContentDialogService<TViewModel> contentDialogService,
            System.Func<TModel, TViewModel> viewModelCreator)
        {
            this.contentDialogService = contentDialogService;
            this.viewModelCreator = viewModelCreator;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; } = "Configure";

        /// <inheritdoc/>
        public bool Configure(TModel source) => throw new System.NotSupportedException();

        /// <inheritdoc/>
        public async ValueTask<bool> ConfigureAsync(TModel source)
        {
            var configure = this.viewModelCreator(source);

            var options = Movere.Models.ContentDialogOptions.Create(
                this.Title,
                configure,
                Movere.Models.DialogResultSet.OKCancel);

            Movere.Models.DialogResult result;
            if (global::Avalonia.Application.Current.CheckAccess())
            {
                result = await this.contentDialogService
                    .ShowDialogAsync(options)
                    .ConfigureAwait(true);
            }
            else
            {
                result = await global::Avalonia.Threading.Dispatcher.UIThread
                    .InvokeAsync(() => this.contentDialogService.ShowDialogAsync(options))
                    .ConfigureAwait(true);
            }

            return result == Movere.Models.DialogResult.OK;
        }
    }
}
