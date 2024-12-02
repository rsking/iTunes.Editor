// <copyright file="ConfiguratorDialog{TModel,TViewModel}.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Services;

using ITunes.Editor.Controls.Dialogs;

/// <summary>
/// The configurator dialog.
/// </summary>
/// <typeparam name="TModel">The model.</typeparam>
/// <typeparam name="TViewModel">The view model.</typeparam>
public class ConfiguratorDialog<TModel, TViewModel> : Configurator<TModel, TViewModel>
    where TViewModel : Models.IConfigure
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfiguratorDialog{TModel, TViewModel}"/> class.
    /// </summary>
    /// <param name="viewModelCreator">The view model creator.</param>
    public ConfiguratorDialog(Func<TModel, TViewModel> viewModelCreator)
        : base(viewModelCreator)
    {
    }

    /// <inheritdoc/>
    protected override ValueTask<bool> ConfigureViewModelAsync(TViewModel source) =>
        new(System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                // create the view
                var dialog = new ConfigureDialog();
                dialog.Owner = System.Windows.Application.Current.MainWindow;
                dialog.DataContext = source;

                return dialog.ShowDialog() is true;
            }));
}
