// <copyright file="App.xaml.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor;

using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/// <summary>
/// The application.
/// </summary>
public partial class App : Application
{
    private readonly IHost host;

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
        this.host = Host.CreateDefaultBuilder()
            .UseDefaultITunes()
            .ConfigureServices((_, services) =>
            {
                // services
                services.AddSingleton<CommunityToolkit.Mvvm.Messaging.IMessenger>(CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default);
                services.AddTransient<Services.Contracts.IOpenFile, Services.OpenFileDialog>();
                services.AddTransient<Services.Contracts.ISelectFolder, Services.SelectFolderDialog>();
                services
                    .AddTransient<IConfigurator<ITunesLib.ITunesSongsProvider>, Services.ConfiguratorDialog<ITunesLib.ITunesSongsProvider, ViewModels.ITunesConfigureViewModel>>()
                    .AddSingleton<Func<ITunesLib.ITunesSongsProvider, ViewModels.ITunesConfigureViewModel>>(model => new ViewModels.ITunesConfigureViewModel(model));

                // view models
                services.AddSingleton<Models.ILoad, ViewModels.LoadViewModel>();
                services.AddSingleton<Models.ISongs, ViewModels.SongsViewModel>();

                // views
                services.AddSingleton<Views.ShellView>();
                services.AddTransient<Views.LoadView>();
                services.AddTransient<Views.SongsView>();

                services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);
            })
            .Build();

        Collections.ObservableHelper.SetObservableCollectionType(typeof(Collections.ObservableImmutableList<>));
        Collections.ObservableHelper.SetObservableListType(typeof(Collections.ObservableImmutableList<>));

        Ioc.Default.ConfigureServices(this.host.Services);
    }

    /// <inheritdoc/>
    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);

        if (this.MainWindow is System.Windows.Navigation.NavigationWindow navigationWindow)
        {
            if (navigationWindow.Content is null)
            {
                navigationWindow.NavigationFailed += OnNavigationFailed;
                _ = navigationWindow.Navigate(Ioc.Default.GetRequiredService<Views.LoadView>());
            }

            this.MainWindow.Activate();

            return;
        }

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (this.MainWindow.Content is not Frame rootFrame)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = new Frame
            {
                // Set the default language
                Language = System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentUICulture.Name),
                NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden,
            };

            rootFrame.NavigationFailed += OnNavigationFailed;

            // Place the frame in the current Window
            this.MainWindow.Content = rootFrame;
        }

        if (rootFrame.Content is null)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            _ = rootFrame.Navigate(Ioc.Default.GetRequiredService<Views.LoadView>());
        }

        // Ensure the current window is active
        this.MainWindow.Activate();

        static void OnNavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            throw new InvalidOperationException("Failed to load Page " + e.Exception);
        }
    }

    /// <inheritdoc/>
    protected override async void OnStartup(StartupEventArgs e)
    {
        await this.host.StartAsync().ConfigureAwait(true);

        this.host.Services.GetRequiredService<Views.ShellView>().Show();

        base.OnStartup(e);
    }

    /// <inheritdoc/>
    protected override async void OnExit(ExitEventArgs e)
    {
        using (this.host)
        {
            await this.host.StopAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(true);
        }

        base.OnExit(e);
    }
}
