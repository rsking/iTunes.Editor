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
    private readonly Movere.Services.IContentDialogService<TViewModel, Movere.Models.DialogResult> contentDialogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfiguratorDialog{TModel, TViewModel}"/> class.
    /// </summary>
    /// <param name="contentDialogService">The content dialog service.</param>
    /// <param name="viewModelCreator">The view model creator.</param>
    public ConfiguratorDialog(
        Movere.Services.IContentDialogService<TViewModel, Movere.Models.DialogResult> contentDialogService,
        Func<TModel, TViewModel> viewModelCreator)
        : base(viewModelCreator) => this.contentDialogService = contentDialogService;

    /// <inheritdoc/>
    protected override async ValueTask<bool> ConfigureViewModelAsync(TViewModel source)
    {
        var actions = System.Collections.Immutable.ImmutableArray.Create(
            Movere.ViewModels.DialogAction.Create(Movere.Models.DialogResult.OK.Name, ReactiveUI.ReactiveCommand.Create((TViewModel _) => Movere.Models.DialogResult.OK)),
            Movere.ViewModels.DialogAction.Create(Movere.Models.DialogResult.Cancel.Name, ReactiveUI.ReactiveCommand.Create((TViewModel _) => Movere.Models.DialogResult.Cancel)));
        var actionSet = new Movere.ViewModels.DialogActionSet<TViewModel, Movere.Models.DialogResult>(actions);

        var options = Movere.Services.ContentDialogOptions.Create(
            this.Title,
            source,
            actionSet);

        var result = await global::Avalonia.Threading.Dispatcher.UIThread
            .InvokeAsync(() => this.contentDialogService.ShowDialogAsync(options))
            .ConfigureAwait(true);

        return result == Movere.Models.DialogResult.OK;
    }
}
