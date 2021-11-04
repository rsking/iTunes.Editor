// <copyright file="ConfiguratorDialog{TModel,TViewModel}.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Services;

/// <summary>
/// The configurator.
/// </summary>
/// <typeparam name="TModel">The type to configure.</typeparam>
/// <typeparam name="TViewModel">The view model.</typeparam>
public class ConfiguratorDialog<TModel, TViewModel> : Configurator<TModel, TViewModel>
    where TViewModel : Models.IConfigure
{
    private readonly Movere.Services.IContentDialogService<TViewModel> contentDialogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfiguratorDialog{TModel, TViewModel}"/> class.
    /// </summary>
    /// <param name="contentDialogService">The content dialog service.</param>
    /// <param name="viewModelCreator">The view model creator.</param>
    public ConfiguratorDialog(
        Movere.Services.IContentDialogService<TViewModel> contentDialogService,
        System.Func<TModel, TViewModel> viewModelCreator)
        : base(viewModelCreator) => this.contentDialogService = contentDialogService;

    /// <inheritdoc/>
    protected override async System.Threading.Tasks.ValueTask<bool> ConfigureViewModelAsync(TViewModel source)
    {
        var options = Movere.Models.ContentDialogOptions.Create(
            this.Title,
            source,
            Movere.Models.DialogResultSet.OKCancel);

        var result = await global::Avalonia.Threading.Dispatcher.UIThread
            .InvokeAsync(() => this.contentDialogService.ShowDialogAsync(options))
            .ConfigureAwait(true);
        return result == Movere.Models.DialogResult.OK;
    }
}
