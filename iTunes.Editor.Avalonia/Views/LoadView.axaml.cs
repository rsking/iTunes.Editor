// <copyright file="LoadView.axaml.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Views;

using global::Avalonia;
using global::Avalonia.Markup.Xaml;
using ReactiveUI;

/// <summary>
/// The load view.
/// </summary>
public class LoadView : global::Avalonia.ReactiveUI.ReactiveUserControl<ViewModels.LoadRoutableViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadView"/> class.
    /// </summary>
    public LoadView()
    {
        this.WhenActivated(_ => { });
        this.InitializeComponent();
    }

    /// <summary>
    /// Handles the click event for the provider button.
    /// </summary>
    /// <param name="sender">The button.</param>
    /// <param name="e">The arguments.</param>
    internal async void OnProviderButton(object sender, global::Avalonia.Interactivity.RoutedEventArgs e)
    {
        var dataContextProvider = sender as IDataContextProvider ?? throw new ArgumentException($"{nameof(sender)} must be a {nameof(IDataContextProvider)}", nameof(sender));
        var provider = dataContextProvider.DataContext as ISongsProvider ?? throw new ArgumentException($"{nameof(sender)} must have a data context of {nameof(ISongsProvider)}", nameof(sender));
        var songs = CommunityToolkit.Mvvm.DependencyInjection.Ioc.Default.GetRequiredService<Models.ISongs>();

        if (this.DataContext is Models.ILoad load)
        {
            await load.LoadAsync(provider).ConfigureAwait(true);
        }

        if (this.DataContext is IRoutableViewModel routableDataContext
            && songs is IRoutableViewModel routable)
        {
            routableDataContext.HostScreen.Router.Navigate.Execute(routable);
        }
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
