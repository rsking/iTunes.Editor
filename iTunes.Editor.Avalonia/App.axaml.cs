// <copyright file="App.axaml.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor;

using CommunityToolkit.Mvvm.DependencyInjection;
using global::Avalonia;
using global::Avalonia.Controls.ApplicationLifetimes;
using global::Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/// <summary>
/// The application.
/// </summary>
public class App : Application
{
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
        var host = Host.CreateDefaultBuilder()
            .UseDefaultITunes()
            .ConfigureServices(services =>
            {
                // services
                _ = services.AddSingleton<CommunityToolkit.Mvvm.Messaging.IMessenger>(CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default);
                _ = services.AddTransient<Services.Contracts.IOpenFile, Services.OpenFileDialog>();
                _ = services.AddTransient<Services.Contracts.ISelectFolder, Services.SelectFolderDialog>();
                _ = services
                    .AddTransient<IConfigurator<ITunesLib.ITunesSongsProvider>, Services.ConfiguratorDialog<ITunesLib.ITunesSongsProvider, ViewModels.ITunesConfigureViewModel>>()
                    .AddSingleton<Func<ITunesLib.ITunesSongsProvider, ViewModels.ITunesConfigureViewModel>>(model => new ViewModels.ITunesConfigureViewModel(model))
                    .AddTransient(Create<ViewModels.ITunesConfigureViewModel>);

                // view models
                _ = services.AddSingleton<ReactiveUI.IScreen, ViewModels.ShellViewModel>();
                _ = services.AddSingleton<Models.ILoad, ViewModels.LoadRoutableViewModel>();
                _ = services.AddSingleton<Models.ISongs, ViewModels.SongsRoutableViewModel>();

                // views
                _ = services.AddSingleton<Views.ShellView>();
                _ = services.AddTransient<Views.LoadView>();
                _ = services.AddTransient<Views.SongsView>();

                [System.Diagnostics.CodeAnalysis.SuppressMessage("Redundancy", "RCS1163:Unused parameter.", Justification = "This is required for the signature")]
                Movere.Services.IContentDialogService<T> Create<T>(IServiceProvider services)
                    where T : Models.IConfigure
                {
                    var owner = this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                        ? desktop.MainWindow
                        : Ioc.Default.GetRequiredService<Views.ShellView>();

                    return new Movere.Services.ContentDialogService<T>(owner, new Services.CustomContentViewResolver());
                }
            })
            .Build();

        Ioc.Default.ConfigureServices(host.Services);

        Splat.Locator.CurrentMutable.Register(() => Ioc.Default.GetRequiredService<Views.LoadView>(), typeof(ReactiveUI.IViewFor<ViewModels.LoadRoutableViewModel>));
        Splat.Locator.CurrentMutable.Register(() => Ioc.Default.GetRequiredService<Views.SongsView>(), typeof(ReactiveUI.IViewFor<ViewModels.SongsRoutableViewModel>));
    }

    /// <inheritdoc/>
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    /// <inheritdoc/>
    public override void OnFrameworkInitializationCompleted()
    {
        if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Ioc.Default.GetRequiredService<Views.ShellView>();
            if (desktop.MainWindow.DataContext is ReactiveUI.IScreen screen
                && Ioc.Default.GetRequiredService<Models.ILoad>() is ReactiveUI.IRoutableViewModel routable)
            {
                _ = screen.Router.Navigate.Execute(routable);
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}
