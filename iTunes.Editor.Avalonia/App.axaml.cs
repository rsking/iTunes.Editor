// <copyright file="App.axaml.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor
{
    using global::Avalonia;
    using global::Avalonia.Controls.ApplicationLifetimes;
    using global::Avalonia.Markup.Xaml;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Toolkit.Mvvm.DependencyInjection;

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
                .ConfigureServices((_, services) =>
                {
                    // services
                    services.AddSingleton<IEventAggregator, EventAggregator>();
                    services.AddTransient<Services.Contracts.IOpenFile, Services.OpenFileDialog>();
                    services.AddTransient<Services.Contracts.ISelectFolder, Services.SelectFolderDialog>();
                    services
                        .AddTransient<IConfigurator<ITunesLib.ITunesSongsProvider>, Services.ConfiguratorDialog<ITunesLib.ITunesSongsProvider, ViewModels.ITunesConfigureViewModel>>()
                        .AddSingleton<System.Func<ITunesLib.ITunesSongsProvider, ViewModels.ITunesConfigureViewModel>>(model => new ViewModels.ITunesConfigureViewModel(model))
                        .AddTransient(Create<ViewModels.ITunesConfigureViewModel>);

                    // view models
                    services.AddSingleton<ReactiveUI.IScreen, ViewModels.ShellViewModel>();
                    services.AddSingleton<Models.ILoad, ViewModels.LoadRoutableViewModel>();
                    services.AddSingleton<Models.ISongs, ViewModels.SongsRoutableViewModel>();

                    // views
                    services.AddSingleton<Views.ShellView>();
                    services.AddTransient<Views.LoadView>();
                    services.AddTransient<Views.SongsView>();

                    Movere.Services.IContentDialogService<T> Create<T>(System.IServiceProvider services)
                        where T : Models.IConfigure
                    {
                        global::Avalonia.Controls.Window owner = this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
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
                    screen.Router.Navigate.Execute(routable);
                }
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
